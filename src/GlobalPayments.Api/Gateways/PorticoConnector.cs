using System;
using System.Collections.Generic;
using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Gateways {
    internal class PorticoConnector : XmlGateway, IPaymentGateway {
        public int SiteId { get; set; }
        public int LicenseId { get; set; }
        public int DeviceId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string SecretApiKey { get; set; }
        public string DeveloperId { get; set; }
        public string VersionNumber { get; set; }
        public bool SupportsHostedPayments { get { return false; } }

        public PorticoConnector() {
        }

        #region processing
        public Transaction ProcesAuthorization(AuthorizationBuilder builder) {
            var et = new ElementTree();

            // build request
            var transaction = et.Element(MapTransactionType(builder));
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
            et.SubElement(block1, "ConvenienceAmtInfo", builder.ConvenienceAmt);
            et.SubElement(block1, "ShippingAmtInfo", builder.ShippingAmt);
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
            var isCheck = (builder.PaymentMethod.PaymentMethodType == PaymentMethodType.ACH);
            if (isCheck || builder.BillingAddress != null) {
                var holder = et.SubElement(block1, isCheck ? "ConsumerInfo" : "CardHolderData");

                if (builder.BillingAddress != null) {
                    et.SubElement(holder, isCheck ? "Address1" : "CardHolderAddr", builder.BillingAddress.StreetAddress1);
                    et.SubElement(holder, isCheck ? "City" : "CardHolderCity", builder.BillingAddress.City);
                    et.SubElement(holder, isCheck ? "State" : "CardHolderState", builder.BillingAddress.Province ?? builder.BillingAddress.State);
                    et.SubElement(holder, isCheck ? "Zip" : "CardHolderZip", builder.BillingAddress.PostalCode);
                }

                if (isCheck) {
                    var check = builder.PaymentMethod as eCheck;
                    if (!string.IsNullOrEmpty(check.CheckHolderName)) {
                        var names = check.CheckHolderName.Split(new char[] {' '}, 2);
                        et.SubElement(holder, "FirstName", names[0]);
                        et.SubElement(holder, "LastName", names[1]);
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
                    // TODO: card holder name
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

                var manualEntry = et.SubElement(cardData, hasToken ? "TokenData" : "ManualEntry");
                et.SubElement(manualEntry, hasToken ? "TokenValue" : "CardNbr").Text(tokenValue ?? card.Number);
                et.SubElement(manualEntry, "ExpMonth").Text(card.ExpMonth.ToString());
                et.SubElement(manualEntry, "ExpYear").Text(card.ExpYear.ToString());
                et.SubElement(manualEntry, "CVV2", card.Cvn);
                et.SubElement(manualEntry, "ReaderPresent", card.ReaderPresent ? "Y" : "N");
                et.SubElement(manualEntry, "CardPresent", card.CardPresent ? "Y" : "N");
                block1.Append(cardData);

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
                    if (builder.PaymentMethod.PaymentMethodType != PaymentMethodType.Debit) {
                        trackData.Set("method", track.EntryMethod == EntryMethod.Swipe ? "swipe" : "proximity");
                        block1.Append(cardData);
                    }
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

                // account info
                if (string.IsNullOrEmpty(check.Token)) {
                    var accountInfo = et.SubElement(block1, "AccountInfo");
                    et.SubElement(accountInfo, "RoutingNumber", check.RoutingNumber);
                    et.SubElement(accountInfo, "AccountNumber", check.AccountNumber);
                    et.SubElement(accountInfo, "CheckNumber", check.CheckNumber);
                    et.SubElement(accountInfo, "MICRData", check.MicrNumber);
                    et.SubElement(accountInfo, "AccountType", check.AccountType.ToString());
                }
                else et.SubElement(block1, "TokenValue").Text(tokenValue);

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
            if (builder.PaymentMethod is ITokenizable) {
                et.SubElement(cardData, "TokenRequest").Text(builder.RequestMultiUseToken ? "Y" : "N");
            }

            // balance inquiry type
            if (builder.PaymentMethod is IBalanceable)
                et.SubElement(block1, "BalanceInquiryType", builder.BalanceInquiryType);

            // cpc request
            if (builder.Level2Request)
                et.SubElement(block1, "CPCReq", "Y");

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

                if (!string.IsNullOrEmpty(builder.EcommerceInfo.Cavv) || !string.IsNullOrEmpty(builder.EcommerceInfo.Eci) || !string.IsNullOrEmpty(builder.EcommerceInfo.Xid)) {
                    var secureEcommerce = et.SubElement(block1, "SecureECommerce");
                    et.SubElement(secureEcommerce, "PaymentDataSource", builder.EcommerceInfo.PaymentDataSource);
                    et.SubElement(secureEcommerce, "TypeOfPaymentData", builder.EcommerceInfo.PaymentDataType);
                    et.SubElement(secureEcommerce, "PaymentData", builder.EcommerceInfo.Cavv);
                    et.SubElement(secureEcommerce, "ECommerceIndicator", builder.EcommerceInfo.Eci);
                    et.SubElement(secureEcommerce, "XID", builder.EcommerceInfo.Xid);
                }
            }

            // dynamic descriptor
            et.SubElement(block1, "TxnDescriptor", builder.DynamicDescriptor);

            // auto substantiation

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
                    || builder.PaymentMethod.PaymentMethodType == PaymentMethodType.Gift
                    || builder.PaymentMethod.PaymentMethodType == PaymentMethodType.ACH)
                    root = et.SubElement(transaction, "Block1");
                else root = transaction;

                // amount
                if (builder.Amount != null)
                    et.SubElement(root, "Amt").Text(builder.Amount.ToString());

                // gratuity
                if (builder.Gratuity != null)
                    et.SubElement(root, "GratuityAmtInfo").Text(builder.Gratuity.ToString());

                // Transaction ID
                et.SubElement(root, "GatewayTxnId", builder.TransactionId);

                // Client Txn Id
                if (builder.TransactionType == TransactionType.Reversal)
                    et.SubElement(root, "ClientTxnId", builder.ClientTransactionId);

                // Level II Data
                if (builder.TransactionType == TransactionType.Edit && builder.TransactionModifier == TransactionModifier.LevelII) {
                    var cpc = et.SubElement(root, "CPCData");
                    et.SubElement(cpc, "CardHolderPONbr", builder.PoNumber);
                    et.SubElement(cpc, "TaxType", builder.TaxType.ToString());
                    et.SubElement(cpc, "TaxAmt", builder.TaxAmount);
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
                et.SubElement(transaction, "DeviceId", trb.DeviceId);
                if(trb.StartDate.HasValue)
                    et.SubElement(transaction, "RptStartUtcDT", trb.StartDate.Value.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz"));
                if (trb.EndDate.HasValue)
                    et.SubElement(transaction, "RptEndUtcDT", trb.EndDate.Value.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz"));
                et.SubElement(transaction, "TxnId", trb.TransactionId);
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
            et.SubElement(header, "DeveloperId", DeveloperId);
            et.SubElement(header, "VersionNumber", VersionNumber);
            et.SubElement(header, "ClientTxnId", clientTransactionId);

            // Transaction
            var trans = et.SubElement(version1, "Transaction");
            trans.Append(transaction);

            return et.ToString(envelope);
        }
        #endregion

        #region response mapping
        private Transaction MapResponse(string rawResponse, IPaymentMethod payment) {
            var result = new Transaction();

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
                result.AuthorizedAmount = root.GetValue<decimal>("AuthAmt");
                result.AvailableBalance = root.GetValue<decimal>("AvailableBalance");
                result.AvsResponseCode = root.GetValue<string>("AVSRsltCode");
                result.AvsResponseMessage = root.GetValue<string>("AVSRsltText");
                result.BalanceAmount = root.GetValue<decimal>("BalanceAmt");
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
                result.ResponseCode = NormalizeResponse(root.GetValue<string>("RspCode")) ?? gatewayRspCode;
                result.ResponseMessage = root.GetValue<string>("RspText", "RspMessage") ?? gatewayRspText;
                result.TransactionDescriptor = root.GetValue<string>("TxnDescriptor");
                if (payment != null) {
                    result.TransactionReference = new TransactionReference {
                        PaymentMethodType = payment.PaymentMethodType,
                        TransactionId = root.GetValue<string>("GatewayTxnId"),
                        AuthCode = root.GetValue<string>("AuthCode")
                    };
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
            }
            return result;
        }

        private T MapReportResponse<T>(string rawResponse, ReportType reportType) where T : class {
            var doc = new ElementTree(rawResponse).Get(MapReportType(reportType));

            T rvalue = Activator.CreateInstance<T>();
            if (reportType.HasFlag(ReportType.Activity) || reportType.HasFlag(ReportType.TransactionDetail)) {
                Func<Element, TransactionSummary> hydrateTransactionSummary = (root) => {
                    return new TransactionSummary {
                        Amount = root.GetValue<decimal>("Amt"),
                        AuthorizedAmount = root.GetValue<decimal>("AuthAmt"),
                        AuthCode = root.GetValue<string>("AuthCode"),
                        ClientTransactionId = root.GetValue<string>("ClientTxnId"),
                        DeviceId = root.GetValue<int>("DeviceId"),
                        IssuerResponseCode = root.GetValue<string>("IssuerRspCode", "RspCode"),
                        IssuerResponseMessage = root.GetValue<string>("IssuerRspText", "RspText"),
                        MaskedCardNumber = root.GetValue<string>("MaskedCardNbr"),
                        OriginalTransactionId = root.GetValue<string>("OriginalGatewayTxnId"),
                        GatewayResponseCode = NormalizeResponse(root.GetValue<string>("GatewayRspCode")),
                        GatewayResponseMessage = root.GetValue<string>("GatewayRspMsg"),
                        ReferenceNumber = root.GetValue<string>("RefNbr"),
                        ServiceName = root.GetValue<string>("ServiceName"),
                        SettlementAmount = root.GetValue<decimal>("SettlementAmt"),
                        Status = root.GetValue<string>("Status", "TxnStatus"),
                        TransactionDate = root.GetValue<DateTime>("TxnUtcDT", "ReqUtcDT"),
                        TransactionId = root.GetValue<string>("GatewayTxnId"),
                        ConvenienceAmt = root.GetValue<decimal>("ConvenienceAmtInfo"),
                        ShippingAmt = root.GetValue<decimal>("ShippingAmtInfo")
                    };
                };

                // Activity
                if (rvalue is IEnumerable<TransactionSummary>) {
                    var list = rvalue as List<TransactionSummary>;
                    foreach (var detail in doc.GetAll("Details")) {
                        list.Add(hydrateTransactionSummary(detail));
                    }
                }
                else {
                    rvalue = hydrateTransactionSummary(doc) as T;
                }
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
                case TransactionType.BatchClose:
                    return "BatchClose";
                case TransactionType.Decline: {
                        if (builder.TransactionModifier == TransactionModifier.ChipDecline)
                            return "ChipCardDecline"; // ChipCardDecline : Decline (Chip)
                        else if (builder.TransactionModifier == TransactionModifier.FraudDecline)
                            return "OverrideFraudDecline"; // OverrideFraudDecline : Decline (Fraud)
                        throw new UnsupportedTransactionException();
                    }
                case TransactionType.Verify:
                    return "CreditAccountVerify"; // CreditAccountVerify : Verify
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
                        if (builder.TransactionModifier.HasFlag(TransactionModifier.LevelII))
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

        private string MapReportType(ReportType type) {
            switch (type) {
                case ReportType.Activity:
                    return "ReportActivity";
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
            return false;
        }
        #endregion
    }
}
