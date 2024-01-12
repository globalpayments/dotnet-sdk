using System;
using System.Linq;
using System.Collections.Generic;
using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Utils;
using System.Text.RegularExpressions;
using System.Reflection;

namespace GlobalPayments.Api.Gateways {
    internal class PorticoConnector : XmlGateway, IPaymentGateway, IReportingService {
        public int SiteId { get; set; }
        public int LicenseId { get; set; }
        public int DeviceId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string SecretApiKey { get; set; }
        public string DeveloperId { get; set; }
        public string VersionNumber { get; set; }
        public bool SupportsHostedPayments => false;
        public string UniqueDeviceId { get; set; }
        public string SDKNameVersion { get; set; }
        public bool SupportsOpenBanking => false;
        public bool? IsSafDataSupported = null;

        public PorticoConnector() {
        }


        #region processing
        public Transaction ProcessAuthorization(AuthorizationBuilder builder) {
            var et = new ElementTree();

            string transactionName = MapTransactionType(builder);

            // build request
            var transaction = et.Element(transactionName);
            var block1 = et.SubElement(transaction, "Block1");
            if (builder.TransactionType.HasFlag(TransactionType.Sale) || builder.TransactionType.HasFlag(TransactionType.Auth)) {
                if (builder.PaymentMethod.PaymentMethodType != PaymentMethodType.Gift && builder.PaymentMethod.PaymentMethodType != PaymentMethodType.ACH) {
                    et.SubElement(block1, "AllowDup", builder.AllowDuplicates ? "Y" : "N");
                    if (builder.TransactionModifier.Equals(TransactionModifier.None) && builder.PaymentMethod.PaymentMethodType != PaymentMethodType.EBT && builder.PaymentMethod.PaymentMethodType != PaymentMethodType.Recurring)
                        et.SubElement(block1, "AllowPartialAuth", builder.AllowPartialAuth ? "Y" : "N");
                }
            }
            et.SubElement(block1, "Amt", builder.Amount);
            et.SubElement(block1, "GratuityAmtInfo", builder.Gratuity);
            et.SubElement(block1, "ConvenienceAmtInfo", builder.ConvenienceAmount);
            et.SubElement(block1, "ShippingAmtInfo", builder.ShippingAmt);
            et.SubElement(block1, "SurchargeAmtInfo", builder.SurchargeAmount);
            // because plano...
            et.SubElement(block1, builder.PaymentMethod.PaymentMethodType == PaymentMethodType.Debit ? "CashbackAmtInfo" : "CashBackAmount", builder.CashBackAmount);

            // offline auth code
            et.SubElement(block1, "OfflineAuthCode", builder.OfflineAuthCode);

            // alias action
            if (builder.TransactionType == TransactionType.Alias) {
                et.SubElement(block1, "Action").Text(builder.AliasAction.ToString());
                et.SubElement(block1, "Alias").Text(builder.Alias);
            }

            #region card holder
            if (builder.PaymentMethod.PaymentMethodType != PaymentMethodType.Recurring
                && builder.PaymentMethod.PaymentMethodType != PaymentMethodType.Gift
                && builder.TransactionType != TransactionType.Tokenize
                && builder.TransactionType != TransactionType.Reversal
                && transactionName != "CreditAdditionalAuth"
                && transactionName != "CreditIncrementalAuth"
            ) {
                var isCheck = (builder.PaymentMethod.PaymentMethodType == PaymentMethodType.ACH);
                var holder = et.SubElement(block1, isCheck ? "ConsumerInfo" : "CardHolderData");

                if (builder.BillingAddress != null) {
                    et.SubElement(holder, isCheck ? "Address1" : "CardHolderAddr", builder.BillingAddress.StreetAddress1);
                    et.SubElement(holder, isCheck ? "City" : "CardHolderCity", builder.BillingAddress.City);
                    et.SubElement(holder, isCheck ? "State" : "CardHolderState", builder.BillingAddress.Province ?? builder.BillingAddress.State);
                    et.SubElement(holder, isCheck ? "Zip" : "CardHolderZip", builder.BillingAddress.PostalCode);
                }

                if (builder.CustomerData != null) {
                    et.SubElement(holder, isCheck ? "EmailAddress" : "CardHolderEmail", builder.CustomerData.Email);
                }

                if (isCheck) {
                    var check = builder.PaymentMethod as eCheck;
                    if (!string.IsNullOrEmpty(check.CheckHolderName)) {
                        if (check.CheckHolderName.Contains(' ')) {
                            var names = check.CheckHolderName.Split(new char[] { ' ' }, 2);
                            et.SubElement(holder, "FirstName", names[0]);
                            et.SubElement(holder, "LastName", names[1]);
                        }
                        else {
                            et.SubElement(holder, "FirstName", check.CheckHolderName);
                        }
                    }
                    et.SubElement(holder, "CheckName", check.CheckName);
                    et.SubElement(holder, "PhoneNumber", check.PhoneNumber);
                    et.SubElement(holder, "DLNumber", check.DriversLicenseNumber);
                    et.SubElement(holder, "DLState", check.DriversLicenseState);

                    if (!string.IsNullOrEmpty(check.SsnLast4) || check.BirthYear != default(int)) {
                        var identity = et.SubElement(holder, "IdentityInfo");
                        et.SubElement(identity, "SSNL4", check.SsnLast4);
                        et.SubElement(identity, "DOBYear", check.BirthYear);
                    }
                }
                else {
                    var card = builder.PaymentMethod as CreditCardData;
                    if (card != null && !string.IsNullOrEmpty(card.CardHolderName)) {
                        if (card.CardHolderName.Trim().Contains(' ')) {
                            var names = card.CardHolderName.Split(new char[] {' '}, 2);
                            et.SubElement(holder, "CardHolderFirstName", names[0]);
                            et.SubElement(holder, "CardHolderLastName", names[1]);
                        }
                        else {
                            et.SubElement(holder, "CardHolderFirstName", card.CardHolderName.Trim());
                        }
                    }
                }
            }

            #endregion

            // card data
            string tokenValue = null;
            var hasToken = HasToken(builder.PaymentMethod, out tokenValue);

            // because debit is weird (Ach too)
            Element cardData = null;
            if (builder.PaymentMethod.PaymentMethodType == PaymentMethodType.Debit || builder.PaymentMethod.PaymentMethodType == PaymentMethodType.ACH)
                cardData = block1;
            else cardData = et.Element("CardData");

            #region ICardData
            if (builder.PaymentMethod is ICardData) {
                var card = builder.PaymentMethod as ICardData;

                //credential on file
                if (builder.TransactionInitiator != null) {
                    Element cardOnFileData = et.SubElement(block1, "CardOnFileData");
                    et.SubElement(cardOnFileData, "CardOnFile", EnumConverter.GetMapping(Target.Portico, builder.TransactionInitiator));
                    et.SubElement(cardOnFileData, "CardBrandTxnId", builder.CardBrandTransactionId);
                }

                var manualEntry = et.SubElement(cardData, hasToken ? "TokenData" : "ManualEntry");
                et.SubElement(manualEntry, hasToken ? "TokenValue" : "CardNbr").Text(tokenValue ?? card.Number);
                et.SubElement(manualEntry, "ExpMonth", card.ExpMonth?.ToString());
                et.SubElement(manualEntry, "ExpYear", card.ExpYear?.ToString());
                et.SubElement(manualEntry, "CVV2", card.Cvn);
                et.SubElement(manualEntry, "ReaderPresent", card.ReaderPresent ? "Y" : "N");
                et.SubElement(manualEntry, "CardPresent", card.CardPresent ? "Y" : "N");
                block1.Append(cardData);

                // secure 3d
                if (card is CreditCardData)
                {
                    var CreditCardData = (card as CreditCardData);
                    var secureEcom = CreditCardData.ThreeDSecure;
                    if (secureEcom != null)
                    {
                        //Secure3D Element
                        if (!string.IsNullOrEmpty(secureEcom.Eci) && (!IsAppleOrGooglePay(secureEcom.PaymentDataSource)))
                        {
                            var Secure3D = et.SubElement(block1, "Secure3D");
                            et.SubElement(Secure3D, "Version", (int)(secureEcom.Version ?? Secure3dVersion.One));
                            et.SubElement(Secure3D, "AuthenticationValue", secureEcom.Cavv);
                            et.SubElement(Secure3D, "ECI", secureEcom.Eci);
                            et.SubElement(Secure3D, "DirectoryServerTxnId", secureEcom.Xid);
                        }
                        if (IsAppleOrGooglePay(secureEcom.PaymentDataSource) && builder.TransactionType != TransactionType.Refund)
                        {
                            var WalletData = et.SubElement(block1, "WalletData");
                            et.SubElement(WalletData, "PaymentSource", secureEcom.PaymentDataSource);
                            et.SubElement(WalletData, "Cryptogram", secureEcom.Cavv);
                            et.SubElement(WalletData, "ECI", secureEcom.Eci);
                        }
                    }
                    //WalletData Element
                    if ((CreditCardData.MobileType == MobilePaymentMethodType.APPLEPAY || CreditCardData.MobileType == MobilePaymentMethodType.GOOGLEPAY)
                        && !string.IsNullOrEmpty(CreditCardData.PaymentSource) && IsAppleOrGooglePay(CreditCardData.PaymentSource) && builder.TransactionType != TransactionType.Refund)
                    {
                        var WalletData = et.SubElement(block1, "WalletData");
                        et.SubElement(WalletData, "PaymentSource", CreditCardData.PaymentSource);
                        et.SubElement(WalletData, "Cryptogram", CreditCardData.Cryptogram);
                        et.SubElement(WalletData, "ECI", CreditCardData.Eci);
                        if (CreditCardData.Token != null)
                        {
                            et.SubElement(WalletData, "DigitalPaymentToken", CreditCardData.Token);
                            block1.Remove("CardData");
                            block1.Remove("CardHolderData");
                        }
                    }
                }
                // recurring data
                if (builder.TransactionModifier == TransactionModifier.Recurring) {
                    var recurring = et.SubElement(block1, "RecurringData");
                    et.SubElement(recurring, "ScheduleID", builder.ScheduleId);
                    et.SubElement(recurring, "OneTime").Text(builder.OneTimePayment ? "Y" : "N");
                }
            }
            #endregion
            #region ITrackData
            else if (builder.PaymentMethod is ITrackData) {
                var track = builder.PaymentMethod as ITrackData;

                var trackData = et.SubElement(cardData, hasToken ? "TokenData" : "TrackData");
                if (!hasToken) {
                    trackData.Text(track.Value);
                    trackData.Set("method", track.EntryMethod == EntryMethod.Swipe ? "swipe" : "proximity");
                    if (builder.PaymentMethod.PaymentMethodType == PaymentMethodType.Credit) {
                        // Tag data
                        if (!string.IsNullOrEmpty(builder.TagData)) {
                            var tagData = et.SubElement(block1, "EMVData");
                            et.SubElement(tagData, "EMVTagData", builder.TagData);
                        }
                    }
                    if (builder.PaymentMethod.PaymentMethodType == PaymentMethodType.Debit) {
                        et.SubElement(block1, "AccountType", builder.AccountType?.ToString() ?? null);
                        et.SubElement(block1, "EMVChipCondition", EnumConverter.GetMapping(Target.Portico, builder.EmvLastChipRead));
                        et.SubElement(block1, "MessageAuthenticationCode", builder.MessageAuthenticationCode);
                        et.SubElement(block1, "PosSequenceNbr", builder.PosSequenceNumber);
                        et.SubElement(block1, "ReversalResonCode", builder.ReversalReasonCode);
                        if (!string.IsNullOrEmpty(builder.TagData)) {
                            var tagData = et.SubElement(block1, "TagData");
                            et.SubElement(tagData, "TagValues", builder.TagData).Set("source", "chip");
                        }
                    }
                    else block1.Append(cardData);
                }
                else et.SubElement(trackData, "TokenValue").Text(tokenValue);
            }
            #endregion
            #region GIFT
            else if (builder.PaymentMethod is GiftCard) {
                var card = builder.PaymentMethod as GiftCard;

                // currency
                et.SubElement(block1, "Currency", builder.Currency);

                // if it's replace add the new card and change the card data name to be old card data
                if (builder.TransactionType == TransactionType.Replace) {
                    var newCardData = et.SubElement(block1, "NewCardData");
                    et.SubElement(newCardData, builder.ReplacementCard.ValueType, builder.ReplacementCard.Value);
                    et.SubElement(newCardData, "PIN", builder.ReplacementCard.Pin);

                    cardData = et.Element("OldCardData");
                }
                et.SubElement(cardData, card.ValueType, card.Value);
                et.SubElement(cardData, "PIN", card.Pin);

                if (builder.AliasAction != AliasAction.CREATE)
                    block1.Append(cardData);
            }
            #endregion
            #region eCheck
            else if (builder.PaymentMethod is eCheck) {
                var check = builder.PaymentMethod as eCheck;

                // check action
                et.SubElement(block1, "CheckAction").Text("SALE");

                var accountInfo = et.SubElement(block1, "AccountInfo");
                // account info
                if (string.IsNullOrEmpty(check.Token)) {
                    et.SubElement(accountInfo, "RoutingNumber", check.RoutingNumber);
                    et.SubElement(accountInfo, "AccountNumber", check.AccountNumber);
                    et.SubElement(accountInfo, "CheckNumber", check.CheckNumber);
                    et.SubElement(accountInfo, "MICRData", check.MicrNumber);
                }
                else et.SubElement(block1, "TokenValue").Text(tokenValue);

                et.SubElement(accountInfo, "AccountType", check.AccountType.ToString());
                et.SubElement(block1, "DataEntryMode", check.EntryMode.ToString().ToUpper());
                et.SubElement(block1, "CheckType", check.CheckType.ToString());
                et.SubElement(block1, "SECCode", check.SecCode);

                // verify info
                var verify = et.SubElement(block1, "VerifyInfo");
                et.SubElement(verify, "CheckVerify").Text(check.CheckVerify ? "Y" : "N");
                et.SubElement(verify, "ACHVerify").Text(check.AchVerify ? "Y" : "N");
            }
            #endregion
            #region TransactionReference
            if (builder.PaymentMethod is TransactionReference) {
                var reference = builder.PaymentMethod as TransactionReference;
                et.SubElement(block1, "GatewayTxnId", reference.TransactionId);
                et.SubElement(block1, "ClientTxnId", reference.ClientTransactionId);
            }
            #endregion
            #region RecurringPaymentMethod
            if (builder.PaymentMethod is RecurringPaymentMethod) {
                var method = builder.PaymentMethod as RecurringPaymentMethod;
                //credential on file
                if (builder.TransactionInitiator != null) {
                    Element cardOnFileData = et.SubElement(block1, "CardOnFileData");
                    et.SubElement(cardOnFileData, "CardOnFile", EnumConverter.GetMapping(Target.Portico, builder.TransactionInitiator));
                    et.SubElement(cardOnFileData, "CardBrandTxnId", builder.CardBrandTransactionId);
                }

                // check action
                if (method.PaymentType == "ACH") {
                    block1.Remove("AllowDup"); // nix the allow duplication
                    et.SubElement(block1, "CheckAction").Text("SALE");
                }

                // payment method stuff
                et.SubElement(block1, "PaymentMethodKey").Text(method.Key);
                if (method.PaymentMethod != null && method.PaymentMethod is CreditCardData) {
                    var card = method.PaymentMethod as CreditCardData;
                    var data = et.SubElement(block1, "PaymentMethodKeyData");
                    et.SubElement(data, "ExpMonth", card.ExpMonth);
                    et.SubElement(data, "ExpYear", card.ExpYear);
                    et.SubElement(data, "CVV2", card.Cvn);
                }

                if (method.PaymentType == "ACH" && (method.SecCode != null)) {
                    et.SubElement(block1, "SECCode").Text(method.SecCode);
                }

                // recurring data
                var recurring = et.SubElement(block1, "RecurringData");
                et.SubElement(recurring, "ScheduleID", builder.ScheduleId);
                et.SubElement(recurring, "OneTime").Text(builder.OneTimePayment ? "Y" : "N");
            }
            #endregion

            // pin block
            if (builder.PaymentMethod is IPinProtected) {
                if(!builder.TransactionType.HasFlag(TransactionType.Reversal))
                    et.SubElement(block1, "PinBlock", ((IPinProtected)builder.PaymentMethod).PinBlock);
            }

            // encryption
            if (builder.PaymentMethod is IEncryptable) {
                var encryptionData = ((IEncryptable)builder.PaymentMethod).EncryptionData;

                if (encryptionData != null) {
                    var enc = et.SubElement(cardData, "EncryptionData");
                    et.SubElement(enc, "Version").Text(encryptionData.Version);
                    et.SubElement(enc, "EncryptedTrackNumber", encryptionData.TrackNumber);
                    et.SubElement(enc, "KTB", encryptionData.KTB);
                    et.SubElement(enc, "KSN", encryptionData.KSN);
                }
            }

            // set token flag
            // eCheck cannot be tokenized w/Portico Gateway
            if (builder.PaymentMethod is ITokenizable && !(builder.PaymentMethod is eCheck)) {
                et.SubElement(cardData, "TokenRequest").Text(builder.RequestMultiUseToken ? "Y" : "N");
            }

            // balance inquiry type
            if (builder.PaymentMethod is IBalanceable)
                et.SubElement(block1, "BalanceInquiryType", builder.BalanceInquiryType);

            // cpc request
            if (builder.CommercialRequest) {
                et.SubElement(block1, "CPCReq", "Y");
            }

            // details
            if (!string.IsNullOrEmpty(builder.CustomerId) || !string.IsNullOrEmpty(builder.Description) || !string.IsNullOrEmpty(builder.InvoiceNumber)) {
                var addons = et.SubElement(block1, "AdditionalTxnFields");
                et.SubElement(addons, "CustomerID", builder.CustomerId);
                et.SubElement(addons, "Description", builder.Description);
                et.SubElement(addons, "InvoiceNbr", builder.InvoiceNumber);
            }

            // ecommerce info
            if (builder.EcommerceInfo != null) {
                et.SubElement(block1, "Ecommerce", builder.EcommerceInfo.Channel.ToString());
                if (!string.IsNullOrEmpty(builder.InvoiceNumber) || builder.EcommerceInfo.ShipMonth != default(int)) {
                    var direct = et.SubElement(block1, "DirectMktData");
                    et.SubElement(direct, "DirectMktInvoiceNbr").Text(builder.InvoiceNumber);
                    et.SubElement(direct, "DirectMktShipDay").Text(builder.EcommerceInfo.ShipDay.ToString());
                    et.SubElement(direct, "DirectMktShipMonth").Text(builder.EcommerceInfo.ShipMonth.ToString());
                }
            }

            if (builder.CommercialData != null)
            {
                var cd = builder.CommercialData;
                var cpc = et.SubElement(block1, "CPCData");
                et.SubElement(cpc, "CardHolderPONbr", cd.PoNumber);
                et.SubElement(cpc, "TaxType", cd.TaxType.ToString());
                et.SubElement(cpc, "TaxAmt", cd.TaxAmount);
            }
            // dynamic descriptor
            et.SubElement(block1, "TxnDescriptor", builder.DynamicDescriptor);

            // auto substantiation
            if (builder.AutoSubstantiation != null) {
                var autoSub = et.SubElement(block1, "AutoSubstantiation");

                var index = 0;
                var fieldNames = new string[] { "First", "Second", "Third", "Fourth" };
                foreach (var amount in builder.AutoSubstantiation.Amounts) {
                    if (amount.Value != default(decimal)) {
                        if (index > 3) {
                            throw new BuilderException("You may only specify three different subtotals in a single transaction.");
                        }

                        var amountNode = et.SubElement(autoSub, fieldNames[index++] + "AdditionalAmtInfo");
                        et.SubElement(amountNode, "AmtType", amount.Key);
                        et.SubElement(amountNode, "Amt", amount.Value?.ToString());
                    }
                }

                et.SubElement(autoSub, "MerchantVerificationValue", builder.AutoSubstantiation.MerchantVerificationValue);
                et.SubElement(autoSub, "RealTimeSubstantiation", builder.AutoSubstantiation.RealTimeSubstantiation ? "Y" : "N");
            }

            #region LodgingData
            if (builder.LodgingData != null) {
                var lodging = builder.LodgingData;

                Element lodgingElement = et.SubElement(block1, "LodgingData");
                et.SubElement(lodgingElement, "PrestigiousPropertyLimit", lodging.PrestigiousPropertyLimit);
                et.SubElement(lodgingElement, "NoShow", lodging.NoShow ? "Y" : "N");
                et.SubElement(lodgingElement, "AdvancedDepositType", lodging.AdvancedDepositType);
                et.SubElement(lodgingElement, "PreferredCustomer", lodging.PreferredCustomer ? "Y" : "N");

                if ((!string.IsNullOrEmpty(lodging.FolioNumber)) || (lodging.StayDuration != default(int)) || (lodging.CheckInDate != null) || (lodging.CheckOutDate != null) ||
                    (lodging.Rate != default(decimal)) || (lodging.ExtraCharges != null)) {
                    Element lodgingDataEdit = et.SubElement(lodgingElement, "LodgingDataEdit");
                    et.SubElement(lodgingDataEdit, "FolioNumber", lodging.FolioNumber);
                    et.SubElement(lodgingDataEdit, "Duration", lodging.StayDuration);
                    et.SubElement(lodgingDataEdit, "CheckInDate", lodging.CheckInDate?.ToString("yyyy-MM-dd"));
                    et.SubElement(lodgingDataEdit, "CheckOutDate", lodging.CheckOutDate?.ToString("yyyy-MM-dd"));
                    et.SubElement(lodgingDataEdit, "Rate", lodging.Rate);

                    if (lodging.ExtraCharges != null) {
                        Element extraChargesElement = et.SubElement(lodgingDataEdit, "ExtraCharges");

                        foreach (var chargeType in lodging.ExtraCharges.Keys) {
                            et.SubElement(extraChargesElement, chargeType.ToString(), "Y");
                        }
                        et.SubElement(lodgingDataEdit, "ExtraChargeAmtInfo", lodging.ExtraChargeAmount);
                    }
                }
            }
            #endregion

            var response = DoTransaction(BuildEnvelope(et, transaction, builder.ClientTransactionId));
            return MapResponse(response, builder.PaymentMethod);
        }

        public string SerializeRequest(AuthorizationBuilder builder) {
            throw new UnsupportedTransactionException("Portico does not support hosted payments.");
        }

        public Transaction ManageTransaction(ManagementBuilder builder) {
            var et = new ElementTree();

            // build request
            var transaction = et.Element(MapTransactionType(builder));

            if (builder.TransactionType != TransactionType.BatchClose) {
                Element root = null;
                if (builder.TransactionType == TransactionType.Reversal
                    || builder.TransactionType == TransactionType.Refund
                    || builder.TransactionType == TransactionType.Increment
                    || builder.PaymentMethod.PaymentMethodType == PaymentMethodType.Gift
                    || builder.PaymentMethod.PaymentMethodType == PaymentMethodType.ACH) {
                    root = et.SubElement(transaction, "Block1");
                }
                else { root = transaction; }

                if (builder.EcommerceInfo != null) {
                    et.SubElement(root, "Ecommerce", builder.EcommerceInfo.Channel.ToString());
                }

                // amount
                if (builder.Amount != null) {
                    et.SubElement(root, "Amt").Text(builder.Amount.ToString());
                }

                // auth amount
                if (builder.AuthAmount != null) {
                    et.SubElement(root, "AuthAmt").Text(builder.AuthAmount.ToString());
                }

                if (builder.SurchargeAmount != null) {
                    et.SubElement(root, "SurchargeAmtInfo", builder.SurchargeAmount);
                }

                // gratuity
                if (builder.Gratuity != null) {
                    et.SubElement(root, "GratuityAmtInfo").Text(builder.Gratuity.ToString());
                }

                // Transaction ID
                et.SubElement(root, "GatewayTxnId", builder.TransactionId);

                // Client Txn Id
                if (
                    builder.TransactionType == TransactionType.Reversal
                    || (
                        builder.PaymentMethod != null
                        && builder.PaymentMethod.PaymentMethodType == PaymentMethodType.ACH
                    )
                ) {
                    et.SubElement(root, "ClientTxnId", builder.ClientTransactionId);
                }

                if (builder.AllowDuplicates) {
                    et.SubElement(root, "AllowDup", "Y");
                }

                // Level II/III Data
                if (builder.CommercialData != null) {
                    var cd = builder.CommercialData;

                    if (cd.CommercialIndicator == CommercialIndicator.Level_II || cd.CommercialIndicator == CommercialIndicator.Level_III) {
                        var cpc = et.SubElement(root, "CPCData");
                        et.SubElement(cpc, "CardHolderPONbr", cd.PoNumber);
                        et.SubElement(cpc, "TaxType", cd.TaxType.ToString());
                        et.SubElement(cpc, "TaxAmt", cd.TaxAmount);
                    }

                    if (cd.CommercialIndicator == CommercialIndicator.Level_III && builder.PaymentMethod.PaymentMethodType is PaymentMethodType.Credit) {
                       
                        var isVisa = (builder.CardType.ToLowerInvariant() == "visa");
                        var cpc = et.SubElement(root, "CorporateData");
                        var data = et.SubElement(cpc, isVisa ? "Visa" : "MC");

                        BuildLineItems(et, data, isVisa, cd.LineItems);

                        if (isVisa) {
                            et.SubElement(data, "SummaryCommodityCode", cd.SummaryCommodityCode);
                            et.SubElement(data, "DiscountAmt", cd.DiscountAmount);
                            et.SubElement(data, "FreightAmt", cd.FreightAmount);
                            et.SubElement(data, "DutyAmt", cd.DutyAmount);
                            et.SubElement(data, "DestinationPostalZipCode", cd.DestinationPostalCode);
                            et.SubElement(data, "ShipFromPostalZipCode", cd.OriginPostalCode);
                            et.SubElement(data, "DestinationCountryCode", cd.DestinationCountryCode);
                            et.SubElement(data, "InvoiceRefNbr", cd.VAT_InvoiceNumber);
                            et.SubElement(data, "OrderDate", cd.OrderDate?.ToString("yyyy-MM-ddTHH:mm:ss.FFFK"));
                            et.SubElement(data, "VATTaxAmtFreight", cd.VATTaxAmtFreight ?? cd.AdditionalTaxDetails?.TaxAmount ??  cd.TaxAmount);
                            et.SubElement(data, "VATTaxRateFreight", cd.VATTaxRateFreight ?? cd.AdditionalTaxDetails?.TaxRate );
                            // et.SubElement(data, "TaxTreatment", null);
                            // et.SubElement(data, "DiscountTreatment", null);
                        }
                       
                    }
                }

                // Lodging Data
                if (builder.LodgingData != null) {
                    LodgingData lodging = builder.LodgingData;

                    if (lodging.ExtraCharges != null) {
                        Element lodgingDataEdit = et.SubElement(root, "LodgingDataEdit");
                        Element extraChargesElement = et.SubElement(lodgingDataEdit, "ExtraCharges");
                        foreach (var chargeType in lodging.ExtraCharges.Keys) {
                            et.SubElement(extraChargesElement, chargeType.ToString(), "Y");
                        }
                        et.SubElement(lodgingDataEdit, "ExtraChargeAmtInfo", lodging.ExtraChargeAmount);
                    }
                }

                // Additional Txn Fields
                if (builder.TransactionType == TransactionType.Refund || builder.TransactionType == TransactionType.Reversal) {
                    if (!string.IsNullOrEmpty(builder.CustomerId) || !string.IsNullOrEmpty(builder.Description) || !string.IsNullOrEmpty(builder.InvoiceNumber)) {
                        var addons = et.SubElement(root, "AdditionalTxnFields");
                        et.SubElement(addons, "CustomerID", builder.CustomerId);
                        et.SubElement(addons, "Description", builder.Description);
                        et.SubElement(addons, "InvoiceNbr", builder.InvoiceNumber);
                    }
                }

                // Token Management
                if (builder.TransactionType.Equals(TransactionType.TokenUpdate) || builder.TransactionType.Equals(TransactionType.TokenDelete)) {
                    ITokenizable token = (ITokenizable)builder.PaymentMethod;

                    // Set the token value
                    et.SubElement(root, "TokenValue").Text(token.Token);

                    var tokenActions = et.SubElement(root, "TokenActions");
                    if (builder.TransactionType.Equals(TransactionType.TokenUpdate)) {
                        CreditCardData card = (CreditCardData)builder.PaymentMethod;

                        var setElement = et.SubElement(tokenActions, "Set");

                        var expMonth = et.SubElement(setElement, "Attribute");
                        et.SubElement(expMonth, "Name", "ExpMonth");
                        et.SubElement(expMonth, "Value", card.ExpMonth);

                        var expYear = et.SubElement(setElement, "Attribute");
                        et.SubElement(expYear, "Name", "ExpYear");
                        et.SubElement(expYear, "Value", card.ExpYear);
                    }
                    else {
                        et.SubElement(tokenActions, "Delete");
                    }
                }
            }
            else
            {
                if (builder.TransactionType == TransactionType.BatchClose)
                {                    
                    if (builder.batchDeviceId != null)
                    {
                        var batchDeviceId = builder.batchDeviceId.ToString();
                        transaction.Set("deviceId", batchDeviceId);
                    }
                }
            }

            var response = DoTransaction(BuildEnvelope(et, transaction, builder.ClientTransactionId));
            return MapResponse(response, builder.PaymentMethod);
        }

        public T ProcessReport<T>(ReportBuilder<T> builder) where T : class {
            var et = new ElementTree();

            var transaction = et.Element(MapReportType(builder.ReportType));
            et.SubElement(transaction, "TzConversion", builder.TimeZoneConversion);
            if (builder is TransactionReportBuilder<T>) {
                var trb = builder as TransactionReportBuilder<T>;
                if (trb.TransactionId != null)
                    et.SubElement(transaction, "TxnId", trb.TransactionId);
                else {
                    var criteria = et.SubElement(transaction, "Criteria");
                    et.SubElement(criteria, "StartUtcDT", trb.SearchBuilder.StartDate?.ToString("yyyy-MM-ddTHH:mm:ss.FFFK"));
                    et.SubElement(criteria, "EndUtcDT", trb.SearchBuilder.EndDate?.ToString("yyyy-MM-ddTHH:mm:ss.FFFK"));
                    et.SubElement(criteria, "AuthCode", trb.SearchBuilder.AuthCode);
                    et.SubElement(criteria, "CardHolderLastName", trb.SearchBuilder.CardHolderLastName);
                    et.SubElement(criteria, "CardHolderFirtName", trb.SearchBuilder.CardHolderFirstName);
                    et.SubElement(criteria, "CardNbrFirstSix", trb.SearchBuilder.CardNumberFirstSix);
                    et.SubElement(criteria, "CardNbrLastFour", trb.SearchBuilder.CardNumberLastFour);
                    et.SubElement(criteria, "InvoiceNbr", trb.SearchBuilder.InvoiceNumber);
                    et.SubElement(criteria, "CardHolderPONbr", trb.SearchBuilder.CardHolderPoNumber);
                    et.SubElement(criteria, "CustomerID", trb.SearchBuilder.CustomerId);
                    //et.SubElement(criteria, "ServiceName", trb.SearchBuilder.);
                    //et.SubElement(criteria, "PaymentType", trb.SearchBuilder.);
                    //et.SubElement(criteria, "CardType", trb.SearchBuilder.);
                    et.SubElement(criteria, "IssuerResult", trb.SearchBuilder.IssuerResult);
                    et.SubElement(criteria, "SettlementAmt", trb.SearchBuilder.SettlementAmount);
                    et.SubElement(criteria, "IssTxnId", trb.SearchBuilder.IssuerTransactionId);
                    et.SubElement(criteria, "RefNbr", trb.SearchBuilder.ReferenceNumber);
                    et.SubElement(criteria, "UserName", trb.SearchBuilder.Username);
                    et.SubElement(criteria, "ClerkID", trb.SearchBuilder.ClerkId);
                    et.SubElement(criteria, "BatchSeqNbr", trb.SearchBuilder.BatchSequenceNumber);
                    et.SubElement(criteria, "BatchId", trb.SearchBuilder.BatchId);
                    et.SubElement(criteria, "SiteTrace", trb.SearchBuilder.SiteTrace);
                    et.SubElement(criteria, "DisplayName", trb.SearchBuilder.DisplayName);
                    et.SubElement(criteria, "ClientTxnId", trb.SearchBuilder.ClientTransactionId);
                    et.SubElement(criteria, "UniqueDeviceId", trb.SearchBuilder.UniqueDeviceId);
                    et.SubElement(criteria, "AcctNbrLastFour", trb.SearchBuilder.AccountNumberLastFour);
                    et.SubElement(criteria, "BankRountingNbr", trb.SearchBuilder.BankRoutingNumber);
                    et.SubElement(criteria, "CheckNbr", trb.SearchBuilder.CheckNumber);
                    et.SubElement(criteria, "CheckFirstName", trb.SearchBuilder.CheckFirstName);
                    et.SubElement(criteria, "CheckLastName", trb.SearchBuilder.CheckLastName);
                    et.SubElement(criteria, "CheckName", trb.SearchBuilder.CheckName);
                    et.SubElement(criteria, "GiftCurrency", trb.SearchBuilder.GiftCurrency);
                    et.SubElement(criteria, "GiftMaskedAlias", trb.SearchBuilder.GiftMaskedAlias);
                    et.SubElement(criteria, "OneTime", trb.SearchBuilder.OneTime);
                    et.SubElement(criteria, "PaymentMethodKey", trb.SearchBuilder.PaymentMethodKey);
                    et.SubElement(criteria, "ScheduleID", trb.SearchBuilder.ScheduleId);
                    et.SubElement(criteria, "BuyerEmailAddress", trb.SearchBuilder.BuyerEmailAddress);
                    et.SubElement(criteria, "AltPaymentStatus", trb.SearchBuilder.AltPaymentStatus);
                    et.SubElement(criteria, "FullyCapturedInd", trb.SearchBuilder.FullyCaptured);
                    et.SubElement(criteria, "SAFIndicator", trb.SearchBuilder.SAFIndicator);
                }
            }

            var response = DoTransaction(BuildEnvelope(et, transaction));
            return MapReportResponse<T>(response, builder.ReportType);
        }

        private string BuildEnvelope(ElementTree et, Element transaction, string clientTransactionId = null) {
            var envelope = et.Element("soap:Envelope");
            var body = et.SubElement(envelope, "soap:Body");
            var request = et.SubElement(body, "PosRequest")
                .Set("xmlns", "http://Hps.Exchange.PosGateway");
            var version1 = et.SubElement(request, "Ver1.0");

            // Header
            var header = et.SubElement(version1, "Header");
            et.SubElement(header, "SecretAPIKey", SecretApiKey);
            et.SubElement(header, "SiteId", SiteId);
            et.SubElement(header, "LicenseId", LicenseId);
            et.SubElement(header, "DeviceId", DeviceId);
            et.SubElement(header, "UserName", Username);
            et.SubElement(header, "Password", Password);
            et.SubElement(header, "DeveloperID", DeveloperId);
            et.SubElement(header, "VersionNbr", VersionNumber);
            et.SubElement(header, "ClientTxnId", clientTransactionId);
            et.SubElement(header, "PosReqDT", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.FFFK"));
            et.SubElement(header, "UniqueDeviceId", UniqueDeviceId);

            et.SubElement(header, "SDKNameVersion", SDKNameVersion != null ? SDKNameVersion : "net;version=" + getReleaseVersion());

            if (IsSafDataSupported != null) {
                Element safData = et.SubElement(header, "SAFData");
                et.SubElement(safData, "SAFIndicator", (bool)IsSafDataSupported ? "Y" : "N");
                et.SubElement(safData, "SAFOrigDT", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.FFFK"));
            }
            
            // Transaction
            var trans = et.SubElement(version1, "Transaction");
            trans.Append(transaction);

            return et.ToString(envelope);
        }
        #endregion

        #region response mapping
        private Transaction MapResponse(string rawResponse, IPaymentMethod payment) {
            var result = new Transaction();
            result.CheckResponseErrorMessages = new List<CheckResponseErrorMessage>();

            var root = new ElementTree(rawResponse).Get("PosResponse");
            var acceptedCodes = new List<string>() { "00", "0", "85", "10" };

            // Check gateway responses
            var gatewayRspCode = NormalizeResponse(root.GetValue<string>("GatewayRspCode"));
            var gatewayRspText = root.GetValue<string>("GatewayRspMsg");

            if (!acceptedCodes.Contains(gatewayRspCode)) {
                throw new GatewayException(
                    string.Format("Unexpected Gateway Response: {0} - {1}", gatewayRspCode, gatewayRspText),
                    gatewayRspCode,
                    gatewayRspText
                );
            }
            else {
                result.AuthorizedAmount = root.GetValue<decimal>("SplitTenderCardAmt", "AuthAmt");
                result.AvailableBalance = root.GetValue<decimal>("AvailableBalance");
                result.AvsResponseCode = root.GetValue<string>("AVSRsltCode");
                result.AvsResponseMessage = root.GetValue<string>("AVSRsltText");
                result.BalanceAmount = root.GetValue<decimal>("BalanceAmt", "AvailableBalance");
                result.CardType = root.GetValue<string>("CardType");
                result.CardLast4 = root.GetValue<string>("TokenPANLast4");
                result.CavvResponseCode = root.GetValue<string>("CAVVResultCode");
                result.CommercialIndicator = root.GetValue<string>("CPCInd");
                result.CvnResponseCode = root.GetValue<string>("CVVRsltCode");
                result.CvnResponseMessage = root.GetValue<string>("CVVRsltText");
                result.EmvIssuerResponse = root.GetValue<string>("EMVIssuerResp");
                result.PointsBalanceAmount = root.GetValue<decimal>("PointsBalanceAmt");
                result.RecurringDataCode = root.GetValue<string>("RecurringDataCode");
                result.ReferenceNumber = root.GetValue<string>("RefNbr");
                result.CardBrandTransactionId = root.GetValue<string>("CardBrandTxnId");
                result.ResponseCode = NormalizeResponse(root.GetValue<string>("RspCode")) ?? gatewayRspCode;
                result.ResponseMessage = root.GetValue<string>("RspText", "RspMessage") ?? gatewayRspText;
                result.TransactionDescriptor = root.GetValue<string>("TxnDescriptor");
                result.HostResponseDate = root.GetValue<DateTime>("HostRspDT");
                if (payment != null) {
                    result.TransactionReference = new TransactionReference {
                        ClientTransactionId = root.GetValue<string>("ClientTxnId"),
                        PaymentMethodType = payment.PaymentMethodType,
                        TransactionId = root.GetValue<string>("GatewayTxnId"),
                        AuthCode = root.GetValue<string>("AuthCode"),
                        CardType = root.GetValue<string>("CardType")
                    };
                }
                // Add additional error messages
                if (root.Has("CheckSale")) {
                    foreach (Element checkResponse in root.GetAll("CheckSale")) {
                        if (checkResponse.GetValue<string>("Type") == "Error") {
                            var errorMessage = new CheckResponseErrorMessage();
                            errorMessage.RspMessage = checkResponse.GetValue<string>("RspMessage");
                            errorMessage.Type = checkResponse.GetValue<string>("Type");
                            errorMessage.Code = checkResponse.GetValue<string>("Code");
                            errorMessage.Message = checkResponse.GetValue<string>("Message");
                            result.CheckResponseErrorMessages.Add(errorMessage);
                        }
                    }
                }

                // gift card create data
                if (root.Has("CardData")) {
                    result.GiftCard = new GiftCard {
                        Number = root.GetValue<string>("CardNbr"),
                        Alias = root.GetValue<string>("Alias"),
                        Pin = root.GetValue<string>("PIN")
                    };
                }

                // token data
                if (root.Has("TokenData")) {
                    result.Token = root.GetValue<string>("TokenValue");
                }

                // batch information
                if (root.Has("BatchId")) {
                    result.BatchSummary = new BatchSummary {
                        Id = root.GetValue<int>("BatchId"),
                        TransactionCount = root.GetValue<int>("TxnCnt"),
                        TotalAmount = root.GetValue<decimal>("TotalAmt"),
                        SequenceNumber = root.GetValue<string>("BatchSeqNbr")
                    };
                }

                // debit mac
                if (root.Has("DebitMac")) {
                    result.DebitMac = new DebitMac {
                        TransactionCode = root.GetValue<string>("TransactionCode"),
                        TransmissionNumber = root.GetValue<string>("TransmissionNumber"),
                        BankResponseCode = root.GetValue<string>("BankResponseCode"),
                        MacKey = root.GetValue<string>("MacKey"),
                        PinKey = root.GetValue<string>("PinKey"),
                        FieldKey = root.GetValue<string>("FieldKey"),
                        TraceNumber = root.GetValue<string>("TraceNumber"),
                        MessageAuthenticationCode = root.GetValue<string>("MessageAuthenticationCode"),
                    };
                }

                // PayFac elements
                if (root.Has("PaymentFacilitatorTxnId") || root.Has("PaymentFacilitatorTxnNbr")) {
                    result.PayFacData = new Entities.PayFac.PayFacResponseData()
                    {
                        TransactionId = root.GetValue<string>("PaymentFacilitatorTxnId"),
                        TransactionNumber = root.GetValue<string>("PaymentFacilitatorTxnNbr")
                    };
                }
            }
            return result;
        }

        private T MapReportResponse<T>(string rawResponse, ReportType reportType) where T : class {
            var response = new ElementTree(rawResponse).Get("PosResponse");
            var acceptedCodes = new List<string>() { "00", "0" };

            // Check gateway responses
            var gatewayRspCode = NormalizeResponse(response.GetValue<string>("GatewayRspCode"));
            var gatewayRspText = response.GetValue<string>("GatewayRspMsg");
            if (!acceptedCodes.Contains(gatewayRspCode)) {
                throw new GatewayException(
                    string.Format("Unexpected Gateway Response: {0} - {1}", gatewayRspCode, gatewayRspText),
                    gatewayRspCode,
                    gatewayRspText
                );
            }

            var doc = new ElementTree(rawResponse).Get(MapReportType(reportType));

            T rvalue = Activator.CreateInstance<T>();
            if (reportType.HasFlag(ReportType.FindTransactions) | reportType.HasFlag(ReportType.Activity)) {           
                Func<Element, TransactionSummary> hydrateTransactionSummary = (root) =>
                {
                    var headerElement = response.Get("Header");
            
                    var summary = new TransactionSummary {
                        AccountDataSource = root.GetValue<string>("AcctDataSrc"),
                        Amount = root.GetValue<decimal>("Amt"),
                        AuthorizedAmount = root.GetValue<decimal>("AuthAmt"),
                        AuthCode = root.GetValue<string>("AuthCode"),
                        BatchCloseDate = root.GetValue<DateTime>("BatchCloseDT"),
                        BatchSequenceNumber = root.GetValue<string>("BatchSeqNbr"),
                        CardSwiped = root.GetValue<string>("CardSwiped"),
                        CardType = root.GetValue<string>("CardType"),
                        ClerkId = root.GetValue<string>("ClerkID"),
                        ClientTransactionId = root.GetValue<string>("ClientTxnId"),
                        ConvenienceAmount = root.GetValue<decimal>("ConvenienceAmtInfo"),
                        DeviceId = root.GetValue<int>("DeviceId"),
                        GratuityAmount = root.GetValue<decimal>("GratuityAmtInfo"),
                        IssuerResponseCode = root.GetValue<string>("IssuerRspCode", "RspCode"),
                        IssuerResponseMessage = root.GetValue<string>("IssuerRspText", "RspText"),
                        IssuerTransactionId = root.GetValue<string>("IssTxnId"),
                        MaskedCardNumber = root.GetValue<string>("MaskedCardNbr"),
                        GatewayResponseCode = NormalizeResponse(root.GetValue<string>("GatewayRspCode")),
                        GatewayResponseMessage = root.GetValue<string>("GatewayRspMsg"),
                        OriginalTransactionId = root.GetValue<string>("OriginalGatewayTxnId"),
                        PaymentType = root.GetValue<string>("PaymentType"),
                        PoNumber = root.GetValue<string>("CardHolderPONbr"),
                        ReferenceNumber = root.GetValue<string>("RefNbr"),
                        ResponseDate = headerElement.GetValue<DateTime>("RspDT"),
                        ServiceName = root.GetValue<string>("ServiceName"),
                        SettlementAmount = root.GetValue<decimal>("SettlementAmt"),
                        ShippingAmount = root.GetValue<decimal>("ShippingAmtInfo"),
                        SiteTrace = root.GetValue<string>("SiteTrace"),
                        Status = root.GetValue<string>("Status", "TxnStatus"),
                        TaxAmount = root.GetValue<decimal>("TaxAmt", "TaxAmtInfo"),
                        TaxType = root.GetValue<string>("TaxType"),
                        TransactionDate = root.GetValue<DateTime>("RspDT"),
                        TransactionId = root.GetValue<string>("GatewayTxnId"),
                        TransactionStatus = root.GetValue<string>("TxnStatus"),
                        Username = root.GetValue<string>("UserName"),

                        Description = root.GetValue<string>("Description"),
                        InvoiceNumber = root.GetValue<string>("InvoiceNbr"),
                        CustomerId = root.GetValue<string>("CustomerID"),
                        UniqueDeviceId = root.GetValue<string>("UniqueDeviceId"),
                        TransactionDescriptor = root.GetValue<string>("TxnDescriptor"),
                        GiftCurrency = root.GetValue<string>("GiftCurrency"),
                        MaskedAlias = root.GetValue<string>("GiftMaskedAlias"),
                        PaymentMethodKey = root.GetValue<string>("PaymentMethodKey"),
                        ScheduleId = root.GetValue<string>("ScheduleID"),
                        OneTimePayment = root.GetValue<string>("OneTime") == "1",
                        RecurringDataCode = root.GetValue<string>("RecurringDataCode"),
                        SurchargeAmount = root.GetValue<decimal>("SurchargeAmtInfo"),
                        FraudRuleInfo = root.GetValue<string>("FraudInfoRule"),
                        RepeatCount = root.GetValue<int>("RepeatCount"),
                        EmvChipCondition = root.GetValue<string>("EMVChipCondition"),
                        HasEmvTags = root.GetValue<string>("HasEMVTag") == "1",
                        HasEcomPaymentData = root.GetValue<string>("HasEComPaymentData") == "1",
                        CavvResponseCode = root.GetValue<string>("CAVVResultCode"),
                        TokenPanLastFour = root.GetValue<string>("TokenPANLast4"),
                        CompanyName = root.GetValue<string>("Company"),
                        CustomerFirstName = root.GetValue<string>("CustomerFirstname"),
                        CustomerLastName = root.GetValue<string>("CustomerLastname"),
                        DebtRepaymentIndicator = root.GetValue<string>("DebtRepaymentIndicator") == "1",
                        CaptureAmount = root.GetValue<decimal>("CaptureAmtInfo"),
                        FullyCaptured = root.GetValue<string>("FullyCapturedInd") == "1",
                        HasLevelIII = root.GetValue<string>("HasLevelIII"),
                    };

                    // card holder data

                    // lodging data
                    if (root.Has("LodgingData")) {
                        summary.LodgingData = new LodgingData {
                            PrestigiousPropertyLimit = root.GetValue<PrestigiousPropertyLimit>("PrestigiousPropertyLimit"),
                            NoShow = root.GetValue<bool>("NoShow"),
                            AdvancedDepositType = root.GetValue<AdvancedDepositType>("AdvancedDepositType"),
                            LodgingDataEdit = root.GetValue<string>("LodgingDataEdit"),
                            PreferredCustomer = root.GetValue<bool>("PReferredCustomer"),
                        };
                    }

                    // check data
                    if (root.Has("CheckData")) {
                        summary.CheckData = new CheckData {
                            AccountInfo = root.GetValue<string>("AccountInfo"),
                            ConsumerInfo = root.GetValue<string>("ConsumerInfo"),
                            DataEntryMode = root.GetValue<string>("DataEntryMode"),
                            CheckType = root.GetValue<string>("CheckType"),
                            SECCode = root.GetValue<string>("SECCode"),
                            CheckAction = root.GetValue<string>("CheckAction")
                        };
                    }

                    // alt payment data
                    if (root.Has("AltPaymentData")) {
                        summary.AltPaymentData = new AltPaymentData {
                            BuyerEmailAddress = root.GetValue<string>("BuyerEmailAddress"),
                            StateDate = root.GetValue<DateTime>("StatusDT"),
                            Status = root.GetValue<string>("Status"),
                            StatusMessage = root.GetValue<string>("StatusMsg"),
                        };

                        summary.AltPaymentData.ProcessorResponseInfo = new List<AltPaymentProcessorInfo>();
                        foreach(var info in root.GetAll("ProcessorRspInfo")) {
                            var pri = new AltPaymentProcessorInfo {
                                Code = info.GetValue<string>("Code"),
                                Message = info.GetValue<string>("Message"),
                                Type = info.GetValue<string>("Type")
                            };
                            summary.AltPaymentData.ProcessorResponseInfo.Add(pri);
                        }
                    }

                    return summary;
                };

                // FindTransaction
                if (rvalue is IEnumerable<TransactionSummary>) {
                    var list = rvalue as List<TransactionSummary>;
                    foreach (var transaction in doc.GetAll("Transactions")) {
                        list.Add(hydrateTransactionSummary(transaction));
                    }
                }
                else {
                    var trans = doc.GetAll("Transactions").FirstOrDefault();
                    if(trans != null)
                        rvalue = hydrateTransactionSummary(trans) as T;
                }
            }
            if (reportType.HasFlag(ReportType.TransactionDetail))
            {
                var reportDetailsElement = response.Get("ReportTxnDetail");
             
                var summary = new TransactionSummary {
                    ResponseDate = response.GetValue<DateTime>("RspDT"),
                    TransactionDate = reportDetailsElement.GetValue<DateTime>("RspDT"),
                    TransactionId = response.GetValue<string>("GatewayTxnId"),
                    SiteId = response.GetValue<int>("SiteId"),
                    MerchantName = response.GetValue<string>("MerchName"),
                    DeviceId = response.GetValue<int>("DeviceId"),
                    Username = response.GetValue<string>("UserName"),
                    ServiceName = response.GetValue<string>("ServiceName"),
                    GatewayResponseCode = NormalizeResponse(response.GetValue<string>("GatewayRspCode")),
                    GatewayResponseMessage = response.GetValue<string>("GatewayRspMsg"),
                    OriginalTransactionId = response.GetValue<string>("OriginalGatewayTxnId"),
                    MerchantNumber = response.GetValue<string>("MerchNbr"),
                    TermOrdinal = response.GetValue<int>("TermOrdinal"),
                    MerchantAddr1 = response.GetValue<string>("MerchAddr1"),
                    MerchantAddr2 = response.GetValue<string>("MerchAddr2"),
                    MerchantCity = response.GetValue<string>("MerchCity"),
                    MerchantState = response.GetValue<string>("MerchState"),
                    MerchantZip = response.GetValue<string>("MerchZip"),
                    MerchantPhone = response.GetValue<string>("MerchPhone"),         
                    TransactionStatus = response.GetValue<string>("TxnStatus"),
                    CardType = response.GetValue<string>("CardType"),
                    MaskedCardNumber = response.GetValue<string>("MaskedCardNbr"),
                    CardPresent = response.GetValue<string>("CardPresent"),
                    ReaderPresent = response.GetValue<string>("ReaderPresent"),
                    CardSwiped = response.GetValue<string>("CardSwiped"),
                    DebitCreditIndicator = response.GetValue<string>("DebitCreditInd"),
                    Amount = response.GetValue<decimal>("Amt"),
                    GratuityAmount = response.GetValue<decimal>("GratuityAmtInfo"),
                    SettlementAmount = response.GetValue<decimal>("SettlementAmt"),
                    AuthorizedAmount = response.GetValue<decimal>("AuthAmt"),
                    CashBackAmount = response.GetValue<decimal>("CashbackAmtInfo"),
                    CardHolderFirstName = response.GetValue<string>("CardHolderFirstName"),
                    CardHolderLastName = response.GetValue<string>("CardHolderLastName"),
                    Email = response.GetValue<string>("CardHolderEmail"),
                    ConvenienceAmount = response.GetValue<decimal>("ConvenienceAmtInfo"),
                    IssuerResponseCode = response.GetValue<string>("IssuerRspCode", "RspCode"),
                    IssuerResponseMessage = response.GetValue<string>("IssuerRspText", "RspText"),
                    IssuerTransactionId = response.GetValue<string>("IssTxnId"),
                    SDKNameVersion = response.GetValue<string>("SDKNameVersion"),
                    InvoiceNumber = response.GetValue<string>("InvoiceNbr"),  // not in the raw response
                    ShippingInvoiceNbr = response.GetValue<string>("DirectMktInvoiceNbr"),
                    ShippingDay = response.GetValue<int>("DirectMktShipDay"),
                    ShippingMonth = response.GetValue<int>("DirectMktShipMonth"),
                    TaxAmount = response.GetValue<decimal>("TaxAmt", "TaxAmtInfo", "CPCTaxAmt"),
                    SurchargeAmount = response.GetValue<decimal>("SurchargeAmtInfo"),
                    Currency = response.GetValue<string>("MerchCurrencyText"),
                    TaxType = response.GetValue<string>("CPCTaxType"),
                    PoNumber = response.GetValue<string>("CPCCardHolderPONbr"),
                };
                if (response.Has("CorporateData") && response.Has("CPCTaxType"))
                {
                    
                    bool ignoreCase = true;
                    string taxType = response.GetValue<string>("CPCTaxType").Substring(response.GetValue<string>("CPCTaxType").IndexOf('-') + 1, response.GetValue<string>("CPCTaxType").Length-(response.GetValue<string>("CPCTaxType").IndexOf('-') + 1));
                    if(response.Has("Visa"))
                    { 
                        CommercialData commercialData = new CommercialData((TaxType)Enum.Parse(typeof(TaxType), taxType,ignoreCase))
                        {
                            SummaryCommodityCode = response.GetValue<string>("SummaryCommodityCode"),
                            FreightAmount = response.GetValue<decimal>("FreightAmt"),
                            DutyAmount = response.GetValue<decimal>("DutyAmt"),
                            DestinationPostalCode = response.GetValue<string>("DestinationPostalZipCode"),
                            OriginPostalCode = response.GetValue<string>("ShipFromPostalZipCode"),
                            DestinationCountryCode = response.GetValue<string>("DestinationCountryCode"),
                            VAT_InvoiceNumber = response.GetValue<string>("InvoiceRefNbr"),
                            OrderDate = response.GetValue<DateTime>("OrderDate"),
                            PoNumber = response.GetValue<string>("CPCCardHolderPONbr"),
                        };
                        summary.CommercialData = commercialData;
                        
                        foreach (var lineItemDetail in response.GetAll("LineItemDetail"))
                        {
                            var lid = new CommercialLineItem
                            {
                                Description = lineItemDetail.GetValue<string>("ItemDescription"),
                                ProductCode = lineItemDetail.GetValue<string>("ProductCode"),
                                Quantity = lineItemDetail.GetValue<decimal>("Quantity"),
                                UnitOfMeasure = lineItemDetail.GetValue<string>("UnitOfMeasure"),
                                DiscountDetails = new DiscountDetails
                                {
                                    DiscountAmount = lineItemDetail.GetValue<decimal>("DiscountAmt"),
                                },
                        };

                            summary.CommercialData.AddLineItems(lid);
                        }
                    }
                    else  //Mastercard
                    {
                        CommercialData commercialData = new CommercialData((TaxType)System.Enum.Parse(typeof(TaxType), taxType, ignoreCase))
                        {
                            SummaryCommodityCode = response.GetValue<string>("SummaryCommodityCode"),
                            FreightAmount = response.GetValue<decimal>("FreightAmt"),
                            DutyAmount = response.GetValue<decimal>("DutyAmt"),
                            DestinationPostalCode = response.GetValue<string>("DestinationPostalZipCode"),
                            OriginPostalCode = response.GetValue<string>("ShipFromPostalZipCode"),
                            DestinationCountryCode = response.GetValue<string>("DestinationCountryCode"),
                            VAT_InvoiceNumber = response.GetValue<string>("InvoiceRefNbr"),
                            OrderDate = response.GetValue<DateTime>("OrderDate"),
                            PoNumber = response.GetValue<string>("CPCCardHolderPONbr"),
                            
                        };
                        summary.CommercialData = commercialData;

                        foreach (var lineItemDetail in response.GetAll("LineItemDetail"))
                        {
                            var lid = new CommercialLineItem
                            {
                                Description = lineItemDetail.GetValue<string>("ItemDescription"),
                                ProductCode = lineItemDetail.GetValue<string>("ProductCode"),
                                Quantity = lineItemDetail.GetValue<decimal>("Quantity"),
                                UnitOfMeasure = lineItemDetail.GetValue<string>("UnitOfMeasure"),

                            };
                            summary.CommercialData.AddLineItems(lid);
                        }
                    }

                }
                rvalue = summary as T;
                return rvalue;
            }
            return rvalue;
        }

        private string NormalizeResponse(string input) {
            if (input == "0" || input == "85")
                return "00";
            return input;
        }
        #endregion

        #region transaction mapping
        private string MapTransactionType<T>(T builder) where T : TransactionBuilder<Transaction> {
            switch (builder.TransactionType) {
                case TransactionType.Tokenize:
                    return "Tokenize";
                case TransactionType.BatchClose:
                    return "BatchClose";
                case TransactionType.Decline: {
                        if (builder.TransactionModifier == TransactionModifier.ChipDecline)
                            return "ChipCardDecline"; // ChipCardDecline : Decline (Chip)
                        else if (builder.TransactionModifier == TransactionModifier.FraudDecline)
                            return "OverrideFraudDecline"; // OverrideFraudDecline : Decline (Fraud)
                        throw new UnsupportedTransactionException();
                    }
                case TransactionType.Verify: {
                        if (builder.TransactionModifier == TransactionModifier.EncryptedMobile)
                            throw new UnsupportedTransactionException();
                        return "CreditAccountVerify"; // CreditAccountVerify : Verify
                    }
                case TransactionType.Capture:
                    return "CreditAddToBatch"; // CreditAddToBatch : Capture
                case TransactionType.Auth: {
                        if (builder.PaymentMethod.PaymentMethodType == PaymentMethodType.Credit) {
                            if (builder.TransactionModifier == TransactionModifier.Additional)
                                return "CreditAdditionalAuth"; // CreditAdditionalAuth : Auth (Additional)
                            else if (builder.TransactionModifier == TransactionModifier.Incremental)
                                return "CreditIncrementalAuth"; // CreditIncrementalAuth : Auth (Incremental)
                            else if (builder.TransactionModifier == TransactionModifier.Offline)
                                return "CreditOfflineAuth"; // CreditOfflineAuth : Auth (Offline|Credit)
                            else if (builder.TransactionModifier == TransactionModifier.Recurring)
                                return "RecurringBillingAuth"; // RecurringBillingAuth : Auth (Recurring)
                           // else if (builder.TransactionModifier == TransactionModifier.EncryptedMobile)
                             //   throw new UnsupportedTransactionException();
                            return "CreditAuth"; // CreditAuth : Auth (Credit)
                        }
                        else if (builder.PaymentMethod.PaymentMethodType == PaymentMethodType.Recurring)
                            return "RecurringBillingAuth"; // RecurringBillingAuth : Auth (Recurring)
                        throw new UnsupportedTransactionException();
                    }
                case TransactionType.Sale: {
                        if (builder.PaymentMethod.PaymentMethodType == PaymentMethodType.Credit) {
                            if (builder.TransactionModifier == TransactionModifier.Offline)
                                return "CreditOfflineSale"; // CreditOfflineSale : Sale (Offline|Credit)
                         else if (builder.TransactionModifier == TransactionModifier.Recurring)
                                return "RecurringBilling";
                            else return "CreditSale"; // CreditSale : Sale (Credit)
                        }
                        else if (builder.PaymentMethod.PaymentMethodType == PaymentMethodType.Recurring) {
                            if ((builder.PaymentMethod as RecurringPaymentMethod).PaymentType == "ACH")
                                return "CheckSale";
                            return "RecurringBilling"; // RecurringBilling : Sale (Recurring)
                        }
                        else if (builder.PaymentMethod.PaymentMethodType == PaymentMethodType.Debit)
                            return "DebitSale"; // DebitSale : Sale (Debit)
                        else if (builder.PaymentMethod.PaymentMethodType == PaymentMethodType.Cash)
                            return "CashSale"; // CashSale : Sale (Cash)
                        else if (builder.PaymentMethod.PaymentMethodType == PaymentMethodType.ACH)
                            return "CheckSale"; // CheckSale : Sale (Check)
                        else if (builder.PaymentMethod.PaymentMethodType == PaymentMethodType.EBT) {
                            if (builder.TransactionModifier == TransactionModifier.CashBack)
                                return "EBTCashBackPurchase"; // EBTCashBackPurchase : Sale (CashBack | EBT)
                            else if (builder.TransactionModifier == TransactionModifier.Voucher)
                                return "EBTVoucherPurchase"; // EBTVoucherPurchase : Sale (Voucher)
                            else return "EBTFSPurchase"; // EBTFSPurchase : Sale (EBT)
                        }
                        else if (builder.PaymentMethod.PaymentMethodType == PaymentMethodType.Gift)
                            return "GiftCardSale"; // GiftCardSale : Sale (Gift)
                        throw new UnsupportedTransactionException();
                    }
                case TransactionType.Refund: {
                        if (builder.PaymentMethod.PaymentMethodType == PaymentMethodType.Credit)
                            return "CreditReturn"; // CreditReturn : Return (Credit)
                        else if (builder.PaymentMethod.PaymentMethodType == PaymentMethodType.Debit) {
                            if (builder.PaymentMethod is TransactionReference) {
                                throw new UnsupportedTransactionException();
                            }
                            return "DebitReturn"; // DebitReturn : Return (Debit)
                        }
                        else if (builder.PaymentMethod.PaymentMethodType == PaymentMethodType.Cash)
                            return "CashReturn"; // CashReturn : Return (Cash)
                        else if (builder.PaymentMethod.PaymentMethodType == PaymentMethodType.EBT) {
                            if (builder.PaymentMethod is TransactionReference) {
                                throw new UnsupportedTransactionException();
                            }
                            return "EBTFSReturn"; // EBTFSReturn : Return (EBT)
                        }
                        throw new UnsupportedTransactionException();
                    }
                case TransactionType.Reversal: {
                        if (builder.PaymentMethod.PaymentMethodType == PaymentMethodType.Credit)
                            return "CreditReversal"; // CreditReversal : Reversal (Credit)
                        else if (builder.PaymentMethod.PaymentMethodType == PaymentMethodType.Debit) {
                            if (builder.PaymentMethod is TransactionReference) {
                                throw new UnsupportedTransactionException();
                            }
                            return "DebitReversal"; // DebitReversal : Reversal (Debit)
                        }
                        else if (builder.PaymentMethod.PaymentMethodType == PaymentMethodType.Gift)
                            return "GiftCardReversal"; // GiftCardReversal : Reversal (Gift)
                        throw new UnsupportedTransactionException();
                    }
                case TransactionType.Edit: {
                        if (builder.TransactionModifier.HasFlag(TransactionModifier.Level_II) || builder.TransactionModifier.HasFlag(TransactionModifier.Level_III))
                            return "CreditCPCEdit"; // CreditCPCEdit : Edit (Level II)
                        else return "CreditTxnEdit";  // CreditTxnEdit : Edit (Credit)
                    }
                case TransactionType.Void: {
                        if (builder.PaymentMethod.PaymentMethodType == PaymentMethodType.Credit)
                            return "CreditVoid"; // CreditVoid :  Void (Credit)
                        else if (builder.PaymentMethod.PaymentMethodType == PaymentMethodType.ACH)
                            return "CheckVoid"; // CheckVoid : Void (Check)
                        else if (builder.PaymentMethod.PaymentMethodType == PaymentMethodType.Gift)
                            return "GiftCardVoid"; // GiftCardVoid : Void (Gift)
                        throw new UnsupportedTransactionException();
                    }
                case TransactionType.AddValue: {
                        if (builder.PaymentMethod.PaymentMethodType == PaymentMethodType.Credit)
                            return "PrePaidAddValue"; // PrePaidAddValue : AddValue (Credit)
                        else if (builder.PaymentMethod.PaymentMethodType == PaymentMethodType.Debit)
                            return "DebitAddValue"; // DebitAddValue : AddValue (Debit)
                        else if (builder.PaymentMethod.PaymentMethodType == PaymentMethodType.Gift)
                            return "GiftCardAddValue"; // GiftCardAddValue : AddValue (Gift)
                        throw new UnsupportedTransactionException();
                    }
                case TransactionType.Balance: {
                        if (builder.PaymentMethod.PaymentMethodType == PaymentMethodType.EBT)
                            return "EBTBalanceInquiry"; // EBTBalanceInquiry : Balance (EBT)
                        else if (builder.PaymentMethod.PaymentMethodType == PaymentMethodType.Gift)
                            return "GiftCardBalance"; // GiftCardBalance : Balance (Gift)
                        else if (builder.PaymentMethod.PaymentMethodType == PaymentMethodType.Credit)
                            return "PrePaidBalanceInquiry"; // PrePaidBalanceInquiry : Balance (Credit)
                        throw new UnsupportedTransactionException();
                    }
                case TransactionType.BenefitWithdrawal:
                    return "EBTCashBenefitWithdrawal"; // EBTCashBenefitWithdrawl : BenefitWithdrawl
                case TransactionType.Activate:
                    return "GiftCardActivate"; // GiftCardActivate : Activate
                case TransactionType.Alias:
                    return "GiftCardAlias"; // GiftCardAlias :  Alias
                case TransactionType.Deactivate:
                    return "GiftCardDeactivate"; // GiftCardDeactivate : Deactivate
                case TransactionType.Replace:
                    return "GiftCardReplace"; // GiftCardReplace : Replace
                case TransactionType.Reward:
                    return "GiftCardReward"; // GiftCardReward : Reward
                case TransactionType.Increment:
                    return "CreditIncrementalAuth";
                case TransactionType.TokenDelete:
                case TransactionType.TokenUpdate:
                    return "ManageTokens";
                default:
                    throw new UnsupportedTransactionException();
            }

            // EBTCashBenefitWithdrawl
            // GiftCardPreviousDayTotals
            // GiftCardCurrentDayTotals

            #region Management calls should be handled differently
            // AddAttachment
            // GetAttachments
            // ManageTokens
            // ParameterDownload
            // TestCredentials

            // BatchClose
            // FindTransactions
            // ReportActivity
            // ReportBatchDetail
            // ReportBatchHistory
            // ReportBatchSummary
            // ReportOpenAuths
            // ReportSearch
            // ReportTxnDetail
            #endregion

            throw new NotImplementedException();
        }

        private string MapReportType(ReportType type)
        {
            switch (type)
            {
                case ReportType.Activity:
                case ReportType.FindTransactions:
                    return "FindTransactions";
                case ReportType.TransactionDetail:
                    return "ReportTxnDetail";
                default:
                    throw new UnsupportedTransactionException();
            }
        }
        private bool HasToken(IPaymentMethod paymentMethod, out string tokenValue) {
            tokenValue = null;

            if (paymentMethod is ITokenizable && !string.IsNullOrEmpty(((ITokenizable)paymentMethod).Token)) {
                tokenValue = ((ITokenizable)paymentMethod).Token;
                return true;
            }

            if (paymentMethod is eCheck && !string.IsNullOrEmpty(((eCheck)paymentMethod).Token)) {
                tokenValue = ((eCheck)paymentMethod).Token;
                return true;
            }
            return false;
        }
        private bool IsAppleOrGooglePay(string paymentDataSource)
        {
            return paymentDataSource == PaymentDataSourceType.APPLEPAY
                || paymentDataSource == PaymentDataSourceType.APPLEPAYAPP
                || paymentDataSource == PaymentDataSourceType.APPLEPAYWEB
                || paymentDataSource == PaymentDataSourceType.GOOGLEPAYAPP
                || paymentDataSource == PaymentDataSourceType.GOOGLEPAYWEB;
        }
        //Get the SDK release version from Assembly info
        private string getReleaseVersion()
        {
            try
            {
                return Assembly.Load(new AssemblyName("GlobalPayments.Api"))?.GetName()?.Version?.ToString();
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
        #endregion

        private void BuildLineItems(ElementTree et, Element parent, bool isVisa, List<CommercialLineItem> items) {
            if (items == null || items.Count == 0) {
                return;
            }

            var lineItems = et.SubElement(parent, "LineItems");

            foreach (var item in items) 
            {
                var lineItem = et.SubElement(lineItems, "LineItemDetail");
                et.SubElement(lineItem, "ItemDescription", item.Description);
                et.SubElement(lineItem, "ProductCode", item.ProductCode);
                et.SubElement(lineItem, "Quantity", item.Quantity);
                et.SubElement(lineItem, "ItemTotalAmt", item.TotalAmount);

                if (isVisa )
                {
                    et.SubElement(lineItem, "ItemTaxTreatment", item.TaxTreatment);

                    if (item.TotalAmount == null)
                    {
                        et.SubElement(lineItem, "UnitOfMeasure", item.UnitOfMeasure);
                        et.SubElement(lineItem, "DiscountAmt", item.DiscountDetails?.DiscountAmount);
                    }
                    // The schema says these fields should be allowed, but they are not currently accepted.
                    // et.SubElement(lineItem, "ItemCommodityCode", item.CommodityCode);
                    // et.SubElement(lineItem, "UnitCost", item.UnitCost);
                    // et.SubElement(lineItem, "VATTaxAmt", item.TaxAmount);
                }
                else 
                {
                    et.SubElement(lineItem, "UnitOfMeasure", item.UnitOfMeasure);
                }
                //et.SubElement(lineItem, "ExtendedItemAmount", item.ExtendedAmount);
                // The schema says this field should exist, but it's not currently allowed.
                // et.SubElement(lineItem, "VATTaxRate", null);
            }
        }
    }
}
