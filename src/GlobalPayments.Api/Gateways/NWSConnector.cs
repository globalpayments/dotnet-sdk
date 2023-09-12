using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Payroll;
using GlobalPayments.Api.Network;
using GlobalPayments.Api.Network.Elements;
using GlobalPayments.Api.Network.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Terminals;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Gateways {
    internal class NWSConnector : NetworkConnector {
        List<TransactionType> _adminServiceTypes = new List<TransactionType>() { 
            TransactionType.SiteConfig,
            TransactionType.TimeRequest
        };

        public override Transaction ProcessAuthorization(AuthorizationBuilder builder) {
            if (_adminServiceTypes.Contains(builder.TransactionType)) {
                return ProcessAdministrationRequests(builder);
            }

            Validate(builder);
            byte[] orgCorr1 = new byte[2];
            byte[] orgCorr2 = new byte[8];

            NetworkMessage request = new NetworkMessage();
            IPaymentMethod paymentMethod = builder.PaymentMethod;
            PaymentMethodType paymentMethodType = builder.PaymentMethod.PaymentMethodType;
            TransactionType transactionType = builder.TransactionType;
            Iso4217_CurrencyCode currencyCode = new Iso4217_CurrencyCode();
            if (!string.IsNullOrEmpty(builder.Currency)) {
                currencyCode = builder.Currency.ToUpper().Equals("USD") ? Iso4217_CurrencyCode.USD : Iso4217_CurrencyCode.CAD;
            }
            //MTI
            string mti = MapMTI(builder);
            request.MessageTypeIndicator = mti;

            // pos data code
            DE22_PosDataCode dataCode = new DE22_PosDataCode();
            // handle the payment methods
            if (paymentMethod is ICardData card) {
                // DE 2: Primary Account Number (PAN) - LLVAR // 1100, 1200, 1220, 1300, 1310, 1320, 1420
                request.Set(DataElementId.DE_002, card.Number);

                // DE 14: Date, Expiration - n4 (YYMM) // 1100, 1200, 1220, 1420
                var month = (card.ExpMonth.HasValue) ? card.ExpMonth.ToString().PadLeft(2, '0') : string.Empty;
                var year = (card.ExpYear.HasValue) ? card.ExpYear.ToString().PadLeft(4, '0').Substring(2, 2) : string.Empty;
                request.Set(DataElementId.DE_014, year + month);

                // set data codes
                dataCode.CardDataInputMode = card.ReaderPresent ? DE22_CardDataInputMode.KeyEntry : DE22_CardDataInputMode.Manual;
                dataCode.CardHolderPresence = card.CardPresent ? DE22_CardHolderPresence.CardHolder_Present : DE22_CardHolderPresence.CardHolder_NotPresent;
                dataCode.CardPresence = card.CardPresent ? DE22_CardPresence.CardPresent : DE22_CardPresence.CardNotPresent;
            }
            else if (paymentMethod is ITrackData track) {
                // put the track data
                if (track.TrackNumber.Equals(TrackNumber.TrackTwo)) {
                    // DE 35: Track 2 Data - LLVAR ns.. 37
                    request.Set(DataElementId.DE_035, track.TrackData);
                }
                else if (track.TrackNumber.Equals(TrackNumber.TrackOne)) {
                    // DE 45: Track 1 Data - LLVAR ans.. 76
                    request.Set(DataElementId.DE_045, track.TrackData);
                }
                else {
                    if (track is IEncryptable && ((IEncryptable)track).EncryptionData != null) {
                        EncryptionData encryptionData = ((IEncryptable)track).EncryptionData;
                        if (encryptionData.TrackNumber.Equals("1")) {
                            // DE 45: Track 1 Data - LLVAR ans.. 76
                            request.Set(DataElementId.DE_045, track.Value);
                        }
                        else if (encryptionData.TrackNumber.Equals("2")) {
                            // DE 35: Track 2 Data - LLVAR ns.. 37
                            request.Set(DataElementId.DE_035, track.Value);
                        }
                    }
                }

                // set data codes
                if (paymentMethodType.Equals(PaymentMethodType.Credit) || paymentMethodType.Equals(PaymentMethodType.Debit)) {
                    dataCode.CardHolderPresence = DE22_CardHolderPresence.CardHolder_Present;
                    dataCode.CardPresence = DE22_CardPresence.CardPresent;
                    if (builder.TagData != null) {
                        if (track.EntryMethod.Equals(EntryMethod.Proximity)) {
                            dataCode.CardDataInputMode = DE22_CardDataInputMode.ContactlessEmv;
                        }
                        else dataCode.CardDataInputMode = DE22_CardDataInputMode.ContactEmv;
                    }
                    else {
                        if (track.EntryMethod.Equals(EntryMethod.Swipe)) {
                            dataCode.CardDataInputMode = DE22_CardDataInputMode.MagStripe;
                        }
                        else if (track.EntryMethod.Equals(EntryMethod.Proximity)) {
                            dataCode.CardDataInputMode = DE22_CardDataInputMode.ContactlessMsd;
                        }
                        else if (builder.EmvFallbackCondition != null) {
                            dataCode.CardDataInputMode = DE22_CardDataInputMode.MagStripe_Fallback;
                        }
                        else {
                            dataCode.CardDataInputMode = DE22_CardDataInputMode.UnalteredTrackData;
                        }
                    }
                }
            }
            else if (paymentMethod is GiftCard giftCard) {
                // put the track data
                if (giftCard.ValueType.Equals("TrackData")) {
                    if (giftCard.TrackNumber.Equals(TrackNumber.TrackTwo)) {
                        // DE 35: Track 2 Data - LLVAR ns.. 37
                        request.Set(DataElementId.DE_035, giftCard.TrackData);
                    }
                    else if (giftCard.TrackNumber.Equals(TrackNumber.TrackOne)) {
                        // DE 45: Track 1 Data - LLVAR ans.. 76
                        request.Set(DataElementId.DE_045, giftCard.TrackData);
                    }
                }
                else {
                    request.Set(DataElementId.DE_002, giftCard.Number);
                    //request.set(DataElementId.DE_014, giftCard.getExpiry());
                }

                // set data codes
                dataCode.CardDataInputMode = DE22_CardDataInputMode.MagStripe;
                dataCode.CardHolderPresence = DE22_CardHolderPresence.CardHolder_Present;
                dataCode.CardPresence = DE22_CardPresence.CardPresent;
            }
            else if (paymentMethod is eCheck) {
                dataCode.CardHolderPresence = DE22_CardHolderPresence.CardHolder_Present;
                dataCode.CardPresence = DE22_CardPresence.CardPresent;
                dataCode.CardDataInputMode = DE22_CardDataInputMode.MagStripe;
            }

            if (paymentMethod is IPinProtected pin) {
                string pinBlock = pin.PinBlock;
                if (!string.IsNullOrEmpty(pinBlock)) {
                    // DE 52: Personal Identification Number (PIN) Data - b8
                    request.Set(DataElementId.DE_052, StringUtils.BytesFromHex(pinBlock.Substring(0, 16)));

                    // DE 53: Security Related Control Information - LLVAR an..48
                    request.Set(DataElementId.DE_053, pinBlock.Substring(16));
                }
            }

            // DE 1: Secondary Bitmap - b8 // M (AUTO GENERATED IN NetworkMessage)
            // DE 3: Processing Code - n6 (n2: TRANSACTION TYPE, n2: ACCOUNT TYPE 1, n2: ACCOUNT TYPE 2) // M 1100, 1200, 1220, 1420
            DE3_ProcessingCode processingCode = MapProcessingCode(builder);
            request.Set(DataElementId.DE_003, processingCode);

            // DE 4: Amount, Transaction - n12 // C 1100, 1200, 1220, 1420
            if (transactionType == TransactionType.AddValue) {
                VerifyReadyLinkAmountLimits(paymentMethod, builder.Amount);
            }
            request.Set(DataElementId.DE_004, StringUtils.ToNumeric(builder.Amount, 12));

            // DE 7: Date and Time, Transmission - n10 (MMDDhhmmss) // C
            request.Set(DataElementId.DE_007, DateTime.UtcNow.ToString("MMddhhmmss"));

            // DE 11: System Trace Audit Number (STAN) - n6 // M
            int stan = builder.SystemTraceAuditNumber;
            if (stan == 0 && StanProvider != null) {
                stan = StanProvider.GenerateStan();
            }
            request.Set(DataElementId.DE_011, StringUtils.PadLeft(stan, 6, '0'));

            // DE 12: Date and Time, Transaction - n12 (YYMMDDhhmmss)
            string timestamp = builder.Timestamp;
            if (string.IsNullOrEmpty(timestamp)) {
                timestamp = DateTime.Now.ToString("yyMMddhhmmss");
            }
            request.Set(DataElementId.DE_012, timestamp);

            // DE 18: Merchant Type - n4 // C 1100, 1200, 1220, 1300, 1320, 1420 (Same as MCC Code - Add to config since will be same for all transactions)
            request.Set(DataElementId.DE_018, MerchantType);

            // DE 19: Country Code, Acquiring Institution - n3 (ISO 3166) // C Config value perhaps? Same for each message
            //request.Set(DataElementId.DE_019, "840");

            /* DE 22: Point of Service Data Code - an12 //  M 1100, 1200, 1220, 1420 // C 1300, 1320 // O 1500, 1520
                22.1 CARD DATA INPUT CAPABILITY an1 The devices/methods available for card/check data input.
                22.2 CARDHOLDER AUTHENTICATION CAPABILITY an1 The methods available for authenticating the cardholder.
                22.3 CARD CAPTURE CAPABILITY an1 Indicates whether the POS application can retain the card if required to do so.
                22.4 OPERATING ENVIRONMENT an1 Indicates whether the POS application is attended by a clerk and the location of the POS application.
                    22.5 CARDHOLDER PRESENT an1 Indicates whether or not the cardholder is present and if not present then why.
                    22.6 CARD PRESENT an1 Indicates whether the card is present.
                    22.7 CARD DATA INPUT MODE an1 The method used for inputting the card data.
                    22.8 CARDHOLDER AUTHENTICATION METHOD an1 The method used for verifying the cardholder identity.
                22.9 CARDHOLDER AUTHENTICATION ENTITY an1 The entity used for verifying the cardholder identity.
                22.10 CARD DATA OUTPUT CAPABILITY an1 The methods available for updating the card data.
                22.11 TERMINAL OUTPUT CAPABILITY an1 The print/display capabilities of the POS.
                22.12 PIN CAPTURE CAPABILITY an1 Indicates whether the PIN data can be captured and if so the maximum PIN data length that can be captured.
            */
            dataCode.CardDataInputCapability = AcceptorConfig.CardDataInputCapability;
            dataCode.CardHolderAuthenticationCapability = AcceptorConfig.CardHolderAuthenticationCapability;
            dataCode.CardCaptureCapability = AcceptorConfig.CardCaptureCapability;
            dataCode.OperatingEnvironment = AcceptorConfig.OperatingEnvironment;

            if (builder.AuthenticationMethod != null) {
                dataCode.CardHolderAuthenticationMethod = (CardHolderAuthenticationMethod)builder.AuthenticationMethod;
            }
            else {
                dataCode.CardHolderAuthenticationMethod = MapAuthenticationMethod(builder);
            }

            dataCode.CardHolderAuthenticationEntity = AcceptorConfig.CardHolderAuthenticationEntity;
            dataCode.CardDataOutputCapability = AcceptorConfig.CardDataOutputCapability;
            dataCode.TerminalOutputCapability = AcceptorConfig.TerminalOutputCapability;
            dataCode.PinCaptureCapability = AcceptorConfig.PinCaptureCapability;
            request.Set(DataElementId.DE_022, dataCode);

            // DE 24: Function Code - n3 // M
            request.Set(DataElementId.DE_024, MapFunctionCode(builder));

            // DE 38: Approval Code - anp6
            request.Set(DataElementId.DE_038, builder.OfflineAuthCode);

            // DE 41: Card Acceptor Terminal Identification Code - ans8
            string companyIdValue = builder.CustomerId;
            if (string.IsNullOrEmpty(companyIdValue)) {
                companyIdValue = CompanyId;
            }
            request.Set(DataElementId.DE_041, StringUtils.PadRight(companyIdValue, 8, ' '));


            // DE 42: Card Acceptor Identification Code - ans15
            request.Set(DataElementId.DE_042, StringUtils.PadRight(TerminalId, 15, ' '));

            /* DE 43: Card Acceptor Name/Location - LLVAR ans.. 99
                43.1 NAME-STREET-CITY ans..83 Name\street\city\
                43.2 POSTAL-CODE ans10
                43.3 REGION ans3 Two letter state/province code for the United States and Canada. Refer to the Heartland Integrator’s Guide.
                43.4 COUNTRY-CODE a3 See A.30.1 ISO 3166-1: Country Codes, p. 809.
            */
            if (AcceptorConfig.Address != null) {
                DE43_CardAcceptorData cardAcceptorData = new DE43_CardAcceptorData {
                    Address = AcceptorConfig.Address
                };
                request.Set(DataElementId.DE_043, cardAcceptorData);
            }

            //        /* DE 46: Amounts, Fees - LLLVAR ans..204
            //            46.1 FEE TYPE CODE n2
            //            46.2 CURRENCY CODE, FEE n3
            //            46.3 AMOUNT, FEE x+n8
            //            46.4 CONVERSION RATE, FEE n8
            //            46.5 AMOUNT, RECONCILIATION FEE x+n8
            //            46.6 CURRENCY CODE, RECONCILIATION FEE n3
            //            46.6 CURRENCY CODE, RECONCILIATION FEE n3
            //         */
            if (builder.FeeAmount != 0) {
                DE46_FeeAmounts feeAmounts = new DE46_FeeAmounts {
                    FeeTypeCode = builder.FeeType,
                    CurrencyCode = currencyCode,
                    Amount = builder.FeeAmount,
                    ReconciliationCurrencyCode = currencyCode
                };
                request.Set(DataElementId.DE_046, feeAmounts);
                if(feeAmounts.FeeTypeCode == FeeType.Surcharge) {
                    request.Set(DataElementId.DE_004, StringUtils.ToNumeric(builder.Amount + feeAmounts.Amount, 12));
                }
            }

            /* DE 48: Message Control - LLLVAR ans..999
                48-0 BIT MAP b8 C Specifies which data elements are present.
                48-1 COMMUNICATION DIAGNOSTICS n4 C Data on communication connection.
                48-2 HARDWARE & SOFTWARE CONFIGURATION ans20 C Version information from POS application.
                48-3 LANGUAGE CODE a2 F Language used for display or print.
                48-4 BATCH NUMBER n10 C Current batch.
                48-5 SHIFT NUMBER n3 C Identifies shift for reconciliation and tracking.
                48-6 CLERK ID LVAR an..9 C Identification of clerk operating the terminal.
                48-7 MULTIPLE TRANSACTION CONTROL n9 F Parameters to control multiple related messages.
                48-8 CUSTOMER DATA LLLVAR ns..250 C Data entered by customer or clerk.
                48-9 TRACK 2 FOR SECOND CARD LLVAR ns..37 C Used to specify the second card in a transaction by the Track 2 format.
                48-10 TRACK 1 FOR SECOND CARD LLVAR ans..76 C Used to specify the second card in a transaction by the Track 1 format.
                48-11 CARD TYPE anp4 C Card type.
                48-12 ADMINISTRATIVELY DIRECTED TASK b1 C Notice to or direction for action to be taken by POS application.
                48-13 RFID DATA LLVAR ans..99 C Data received from RFID transponder.
                48-14 PIN ENCRYPTION METHODOLOGY ans2 C Used to identify the type of encryption methodology.
                48-15, 48-32 RESERVED FOR ANSI USE LLVAR ans..99 These are reserved for future use.
                48-33 POS CONFIGURATION LLVAR ans..99 C Values that indicate to the Heartland system capabilities and configuration of the POS application.
                48-34 MESSAGE CONFIGURATION LLVAR ans..99 C Information regarding the POS originating message and the host generated response message.
                48-35 NAME 1 LLVAR ans..99 D
                48-36 NAME 2 LLVAR ans..99 D
                48-37 SECONDARY ACCOUNT NUMBER LLVAR ans..28 C Second Account Number for manually entered transactions requiring 2 account numbers.
                48-38 RESERVED FOR HEARTLAND USE LLVAR ans..99 F
                48-39 PRIOR MESSAGE INFORMATION LLVAR ans..99 C Information regarding the status of the prior message sent by the POS.
                48-40, 48-49 ADDRESS 1 THROUGH ADDRESS 10 LLVAR ans..99 D One or more types of addresses.
                48-50, 48-64 RESERVED FOR HEARTLAND USE LLVAR ans..99 F
            */
            // DE48-5
            //        messageControl.setShiftNumber(builder.getShiftNumber());

            DE48_MessageControl messageControl = MapMessageControl(builder);
            request.Set(DataElementId.DE_048, messageControl);

            //        /* DE 54: Amounts, Additional - LLVAR ans..120
            //            54.1 ACCOUNT TYPE, ADDITIONAL AMOUNTS n2 Positions 3 and 4 or positions 5 and 6 of the processing code data element.
            //            54.2 AMOUNT TYPE, ADDITIONAL AMOUNTS n2 Identifies the purpose of the transaction amounts.
            //            54.3 CURRENCY CODE, ADDITIONAL AMOUNTS n3 Use DE 49 codes.
            //            54.4 AMOUNT, ADDITIONAL AMOUNTS x + n12 See Use of the Terms Credit and Debit under Table 1-2 Transaction Processing, p. 61.
            //         */
            if (builder.CashBackAmount != null || transactionType.Equals(TransactionType.BenefitWithdrawal) || builder.AmountTaxed != null) {
                DE54_AmountsAdditional amountsAdditional = new DE54_AmountsAdditional();
                if (paymentMethod is GiftCard) {
                    amountsAdditional.Put(DE54_AmountTypeCode.AmountCash, DE3_AccountType.CashCardAccount, currencyCode, builder.CashBackAmount);
                    amountsAdditional.Put(DE54_AmountTypeCode.AmountGoodsAndServices, DE3_AccountType.CashCardAccount, currencyCode, builder.Amount - builder.CashBackAmount);
                }
                else if (paymentMethod is EBT) {
                    if (transactionType.Equals(TransactionType.BenefitWithdrawal)) {
                        amountsAdditional.Put(DE54_AmountTypeCode.AmountCash, DE3_AccountType.CashBenefitAccount, currencyCode, builder.Amount);
                    }
                    else {
                        amountsAdditional.Put(DE54_AmountTypeCode.AmountCash, DE3_AccountType.CashBenefitAccount, currencyCode, builder.CashBackAmount);
                        amountsAdditional.Put(DE54_AmountTypeCode.AmountGoodsAndServices, DE3_AccountType.CashBenefitAccount, currencyCode, builder.Amount - builder.CashBackAmount);
                    }
                }
                else if (paymentMethod is Debit) {
                    amountsAdditional.Put(DE54_AmountTypeCode.AmountCash, DE3_AccountType.PinDebitAccount, currencyCode, builder.CashBackAmount);
                    amountsAdditional.Put(DE54_AmountTypeCode.AmountGoodsAndServices, DE3_AccountType.PinDebitAccount, currencyCode, builder.Amount - builder.CashBackAmount);
                }
                else if (paymentMethod is Credit credit) {
                    if (credit.CardType.Contains("Purchasing")) {
                        amountsAdditional.Put(DE54_AmountTypeCode.AmountTax, DE3_AccountType.PurchaseAccount, currencyCode, builder.AmountTaxed);
                        request.Set(DataElementId.DE_004, StringUtils.ToNumeric(builder.Amount + builder.AmountTaxed, 12));
                    }
                }

                if (amountsAdditional.Size() > 0) {
                    request.Set(DataElementId.DE_054, amountsAdditional);
                }
            }

            // DE 55: Integrated Circuit Card (ICC) Data - LLLVAR b..512
            if (!string.IsNullOrEmpty(builder.TagData)) {
                EmvData tagData = EmvUtils.ParseTagData(builder.TagData, EnableLogging);
                if (!string.IsNullOrEmpty(tagData.GetCardSequenceNumber())) {
                    request.Set(DataElementId.DE_023, StringUtils.PadLeft(tagData.GetCardSequenceNumber(), 3, '0'));
                }
                request.Set(DataElementId.DE_055, tagData.GetSendBuffer());
            }

            // DE 59: Transport Data - LLLVAR ans..999
            request.Set(DataElementId.DE_059, builder.TransportData);

            // DE 62: Card Issuer Data - LLLVAR ans..999
            DE62_CardIssuerData cardIssuerData = MapCardIssuerData(builder);
            request.Set(DataElementId.DE_062, cardIssuerData);

            // DE 63: Product Data - LLLVAR ans…999
            if (builder.ProductData != null) {
                if (paymentMethod is Credit credit) {
                    if (credit.FleetCard) {
                        VerifyFleetCardProductLimits(builder, credit);
                    }
                }
                DE63_ProductData productData = builder.ProductData.ToDataElement();
                request.Set(DataElementId.DE_063, productData);
            }
            // DE 72: Data Record - LLLVAR ans..999
            if (builder.POSSiteConfigRecord != null) {
                DE72_DataRecord dataRecord = new DE72_DataRecord("SCFG", builder.POSSiteConfigRecord);
                request.Set(DataElementId.DE_072, dataRecord.ToByteArray());
            }

            // DE 73: Date, Action - n6 (YYMMDD)
            // DE 96: Key Management Data - LLLVAR b..999
            // DE 97: Amount, Net Reconciliation - x + n16
            // DE 102: Account Identification 1 - LLVAR ans..28
            // DE 103: Check MICR Data (Account Identification 2) - LLVAR ans..28
            if (paymentMethod is eCheck check) {
                request.Set(DataElementId.DE_102, builder.CheckCustomerId);

                if (string.IsNullOrEmpty(builder.RawMICRData)) {
                    DE103_Check_MICR_Data checkData = new DE103_Check_MICR_Data()
                    {
                        AccountNumber = check.AccountNumber,
                        TransitNumber = check.RoutingNumber,
                        SequenceNumber = check.CheckNumber
                    };

                    if (checkData.ToByteArray().Length <= 28) {
                        request.Set(DataElementId.DE_103, checkData.ToByteArray());
                    }
                }
            }

            // DE 115: eWIC Overflow Data - LLLVAR ansb..999
            // DE 116: eWIC Overflow Data - LLLVAR ansb..999
            // DE 117: eWIC Data - LLLVAR ansb..999
            if (builder.EwicData != null) {
                DE117_WIC_Data_Fields ewicData = builder.EwicData.ToDataElement();
                request.Set(DataElementId.DE_117, ewicData);
            }
            // DE 123: Reconciliation Totals - LLLVAR ans..999
            // DE 124: Sundry Data - LLLVAR ans..999
            // DE 125: Extended Response Data 1 - LLLVAR ans..999
            // DE 126: Extended Response Data 2 - LLLVAR ans..999

            // DE 127: Forwarding Data - LLLVAR ans..999
            if (paymentMethod is IEncryptable encryptable) {
                EncryptionData encryptionData = encryptable.EncryptionData;
                if (encryptionData != null) {
                    DE127_ForwardingData forwardingData = new DE127_ForwardingData();
                    forwardingData.AddEncryptionData(AcceptorConfig.SupportedEncryptionType, encryptionData.KTB);

                    // check for encrypted cid
                    if (paymentMethod is ICardData cardData) {
                        string encryptedCvn = cardData.Cvn;
                        if (!string.IsNullOrEmpty(encryptedCvn)) {
                            forwardingData.AddEncryptionData(AcceptorConfig.SupportedEncryptionType, encryptionData.KTB, encryptedCvn);
                        }
                    }

                    request.Set(DataElementId.DE_127, forwardingData);
                }
            }

            return SendRequest(request, builder, orgCorr1, orgCorr2);
        }

        public override Transaction ManageTransaction(ManagementBuilder builder) {
            Validate(builder);

            //        // TODO: These should come from the builder somehow
            byte[] orgCorr1 = new byte[2];
            byte[] orgCorr2 = new byte[8];

            NetworkMessage request = new NetworkMessage();
            IPaymentMethod paymentMethod = builder.PaymentMethod;
            TransactionType transactionType = builder.TransactionType;
            Iso4217_CurrencyCode currencyCode = Iso4217_CurrencyCode.USD;
            if (!string.IsNullOrEmpty(builder.Currency)) {
                currencyCode = builder.Currency.ToUpper().Equals("USD") ? Iso4217_CurrencyCode.USD : Iso4217_CurrencyCode.CAD;
            }

            decimal? transactionAmount = builder.Amount;
            if (transactionAmount == null && paymentMethod is TransactionReference) {
                transactionAmount = (paymentMethod as TransactionReference).OriginalApprovedAmount;
            }

            // MTI
            string mti = MapMTI(builder);
            request.MessageTypeIndicator = mti;

            // pos data code
            DE22_PosDataCode dataCode = new DE22_PosDataCode();

            // DE 1: Secondary Bitmap - b8 // M
            request.Set(DataElementId.DE_001, new byte[8]); // TODO: This should be better

            // DE 2: Primary Account Number (PAN) - LLVAR // 1100, 1200, 1220, 1300, 1310, 1320, 1420
            if (paymentMethod is TransactionReference transactionReference) {
                // Original Card Data
                if (transactionReference.OriginalPaymentMethod != null) {
                    IPaymentMethod originalPaymentMethod = transactionReference.OriginalPaymentMethod;
                    PaymentMethodType paymentMethodType = originalPaymentMethod.PaymentMethodType;

                    if (originalPaymentMethod is ICardData cardData) {
                        // DE 2: PAN & DE 14 Expiry
                        request.Set(DataElementId.DE_002, cardData.Number);
                        var month = (cardData.ExpMonth.HasValue) ? cardData.ExpMonth.ToString().PadLeft(2, '0') : string.Empty;
                        var year = (cardData.ExpYear.HasValue) ? cardData.ExpYear.ToString().PadLeft(4, '0').Substring(2, 2) : string.Empty;
                        request.Set(DataElementId.DE_014, year + month);

                        // Data codes
                        dataCode.CardDataInputMode = cardData.ReaderPresent ? DE22_CardDataInputMode.KeyEntry : DE22_CardDataInputMode.Manual;
                        dataCode.CardHolderPresence = cardData.CardPresent ? DE22_CardHolderPresence.CardHolder_Present : DE22_CardHolderPresence.CardHolder_NotPresent;
                        dataCode.CardPresence = cardData.CardPresent ? DE22_CardPresence.CardPresent : DE22_CardPresence.CardNotPresent;

                        if (cardData.OriginalEntryMethod == EntryMethod.Chip) {
                            dataCode.CardDataInputMode = DE22_CardDataInputMode.ContactEmv;
                        }

                        if (cardData.OriginalEntryMethod == EntryMethod.Swipe) {
                            dataCode.CardDataInputMode = DE22_CardDataInputMode.MagStripe;
                        }
                    }
                    else if (originalPaymentMethod is ITrackData track) {
                        if (track is IEncryptable && ((IEncryptable)track).EncryptionData != null) {
                            EncryptionData encryptionData = ((IEncryptable)track).EncryptionData;
                            if (encryptionData.TrackNumber.Equals("1")) {
                                request.Set(DataElementId.DE_045, track.Value);
                            }
                            else if (encryptionData.TrackNumber.Equals("2")) {
                                request.Set(DataElementId.DE_035, track.Value);
                            }
                        }
                        else {
                            // Non original transactions should not be sending track data in its entirety. It needs to be truncated after the expiry date
                            int expiryIndex = track.TrackData.LastIndexOf(track.Expiry) + track.Expiry.Length;
                            if (track.TrackNumber.Equals(TrackNumber.TrackTwo)) {
                                if (expiryIndex < track.TrackData.Length) {
                                    request.Set(DataElementId.DE_035, track.TrackData.Remove(expiryIndex));
                                }
                                else {
                                    request.Set(DataElementId.DE_035, track.TrackData);
                                }
                            }
                            else {
                                if (expiryIndex < track.TrackData.Length) {
                                    request.Set(DataElementId.DE_045, track.TrackData.Remove(expiryIndex));
                                }
                                else {
                                    request.Set(DataElementId.DE_045, track.TrackData);
                                }
                            }
                        }

                        // set data codes
                        if (paymentMethodType.Equals(PaymentMethodType.Credit) || paymentMethodType.Equals(PaymentMethodType.Debit)) {
                            dataCode.CardHolderPresence = DE22_CardHolderPresence.CardHolder_Present;
                            dataCode.CardPresence = DE22_CardPresence.CardPresent;

                            if (track.EntryMethod.Equals(EntryMethod.Swipe)) {
                                dataCode.CardDataInputMode = DE22_CardDataInputMode.MagStripe;
                            }
                            else if (track.EntryMethod.Equals(EntryMethod.Proximity)) {
                                dataCode.CardDataInputMode = DE22_CardDataInputMode.ContactlessMsd;
                            }
                            else {
                                dataCode.CardDataInputMode = DE22_CardDataInputMode.UnalteredTrackData;
                            }
                        }
                    }
                    else if (originalPaymentMethod is GiftCard gift) {
                        // DE 35 / DE 45
                        if (gift.ValueType.Equals("TrackData")) {
                            if (gift.TrackNumber.Equals(TrackNumber.TrackTwo)) {
                                request.Set(DataElementId.DE_035, gift.TrackData);
                            }
                            else {
                                request.Set(DataElementId.DE_045, gift.TrackData);
                            }
                        }
                        else {
                            // DE 2: PAN & DE 14 Expiry
                            request.Set(DataElementId.DE_002, gift.Number);
                        }

                        // set data codes
                        dataCode.CardDataInputMode = DE22_CardDataInputMode.MagStripe;
                        dataCode.CardHolderPresence = DE22_CardHolderPresence.CardHolder_Present;
                        dataCode.CardPresence = DE22_CardPresence.CardPresent;
                    }
                }
            }

            // DE 3: Processing Code - n6 (n2: TRANSACTION TYPE, n2: ACCOUNT TYPE 1, n2: ACCOUNT TYPE 2) // M 1100, 1200, 1220, 1420
            DE3_ProcessingCode processingCode = MapProcessingCode(builder);
            request.Set(DataElementId.DE_003, processingCode);

            // DE 4: Amount, Transaction - n12 // C 1100, 1200, 1220, 1420
            request.Set(DataElementId.DE_004, StringUtils.ToNumeric(transactionAmount, 12));

            // DE 7: Date and Time, Transmission - n10 (MMDDhhmmss) // C
            request.Set(DataElementId.DE_007, DateTime.UtcNow.ToString("MMddhhmmss"));

            // DE 11: System Trace Audit Number (STAN) - n6 // M
            int stan = builder.SystemTraceAuditNumber;
            if (stan == 0 && StanProvider != null) {
                stan = StanProvider.GenerateStan();
            }
            request.Set(DataElementId.DE_011, StringUtils.PadLeft(stan, 6, '0'));

            // DE 12: Date and Time, Transaction - n12 (YYMMDDhhmmss)
            string timestamp = builder.Timestamp;
            if (string.IsNullOrEmpty(timestamp)) {
                timestamp = DateTime.Now.ToString("yyMMddhhmmss");
            }
            request.Set(DataElementId.DE_012, timestamp);

            // DE 15: Date, Settlement - n6 (YYMMDD) // C
            // DE 17: Date, Capture - n4 (MMDD) // C
            if (transactionType.Equals(TransactionType.Capture)) {
                request.Set(DataElementId.DE_017, DateTime.UtcNow.ToString("MMdd"));
            }

            // DE 18: Merchant Type - n4 // C 1100, 1200, 1220, 1300, 1320, 1420 (Same as MCC Code - Add to config since will be same for all transactions)
            request.Set(DataElementId.DE_018, MerchantType);

            // DE 19: Country Code, Acquiring Institution - n3 (ISO 3166) // C Config value perhaps? Same for each message
            //request.Set(DataElementId.DE_019, "840");

            /* DE 22: Point of Service Data Code - an12 //  M 1100, 1200, 1220, 1420 // C 1300, 1320 // O 1500, 1520
                22.1 CARD DATA INPUT CAPABILITY an1 The devices/methods available for card/check data input.
                22.2 CARDHOLDER AUTHENTICATION CAPABILITY an1 The methods available for authenticating the cardholder.
                22.3 CARD CAPTURE CAPABILITY an1 Indicates whether the POS application can retain the card if required to do so.
                22.4 OPERATING ENVIRONMENT an1 Indicates whether the POS application is attended by a clerk and the location of the POS application.
                    22.5 CARDHOLDER PRESENT an1 Indicates whether or not the cardholder is present and if not present then why.
                    22.6 CARD PRESENT an1 Indicates whether the card is present.
                    22.7 CARD DATA INPUT MODE an1 The method used for inputting the card data.
                    22.8 CARDHOLDER AUTHENTICATION METHOD an1 The method used for verifying the cardholder identity.
                22.9 CARDHOLDER AUTHENTICATION ENTITY an1 The entity used for verifying the cardholder identity.
                22.10 CARD DATA OUTPUT CAPABILITY an1 The methods available for updating the card data.
                22.11 TERMINAL OUTPUT CAPABILITY an1 The print/display capabilities of the POS.
                22.12 PIN CAPTURE CAPABILITY an1 Indicates whether the PIN data can be captured and if so the maximum PIN data length that can be captured.
             */
            dataCode.CardDataInputCapability = AcceptorConfig.CardDataInputCapability;
            dataCode.CardHolderAuthenticationCapability = AcceptorConfig.CardHolderAuthenticationCapability;
            dataCode.CardCaptureCapability = AcceptorConfig.CardCaptureCapability;
            dataCode.OperatingEnvironment = AcceptorConfig.OperatingEnvironment;

            if (builder.AuthenticationMethod != null) {
                dataCode.CardHolderAuthenticationMethod = (CardHolderAuthenticationMethod)builder.AuthenticationMethod;
            }
            else {
                dataCode.CardHolderAuthenticationMethod = MapAuthenticationMethod(builder);
            }
            
            dataCode.CardHolderAuthenticationEntity = AcceptorConfig.CardHolderAuthenticationEntity;
            dataCode.CardDataOutputCapability = AcceptorConfig.CardDataOutputCapability;
            dataCode.TerminalOutputCapability = AcceptorConfig.TerminalOutputCapability;
            dataCode.PinCaptureCapability = AcceptorConfig.PinCaptureCapability;
            request.Set(DataElementId.DE_022, dataCode);

            // DE 23: Card Sequence Number - n3 // C 1100, 1120, 1200, 1220, 1420 (Applies to EMV cards if the sequence number is returned from the terminal)
            // DE 24: Function Code - n3 // M
            request.Set(DataElementId.DE_024, MapFunctionCode(builder));

            // DE 25: Message Reason Code - n4 // C 1100, 1120, 1200, 1220, 1300, 1320, 1420, 16XX, 18XX
            DE25_MessageReasonCode? reasonCode = MapMessageReasonCode(builder);
            if (reasonCode != null) {
                request.Set(DataElementId.DE_025, Encoding.UTF8.GetBytes(EnumConverter.GetMapping(Target.NWS, reasonCode)));
            }

            // DE 28: Date, Reconciliation - n6 (YYMMDD)
            if (transactionType.Equals(TransactionType.BatchClose)) {
                request.Set(DataElementId.DE_028, DateTime.UtcNow.ToString("yyMMdd"));
            }

            /* DE 30: Amounts, Original - n24
                30.1 ORIGINAL AMOUNT, TRANSACTION n12 A copy of amount, transaction (DE 4) from the original transaction.
                30.2 ORIGINAL AMOUNT, RECONCILIATION n12 A copy of amount, reconciliation (DE 5) from the original transaction. Since DE 5 is not used, this element will contain all zeros.
             */
            if (paymentMethod is TransactionReference transaction) {
                bool de30Supported = true;
                if (paymentMethod.PaymentMethodType == PaymentMethodType.Gift) {
                    if (transactionType == TransactionType.Reversal || transactionType == TransactionType.Void) {
                        de30Supported = false;
                    }
                }

                if (de30Supported) {
                    if (transaction.OriginalAmount != null) {
                        DE30_OriginalAmounts originalAmounts = new DE30_OriginalAmounts {
                            OriginalTransactionAmount = transaction.OriginalAmount
                        };

                        request.Set(DataElementId.DE_030, originalAmounts);
                    }
                }
            }

            // DE 32: Acquiring Institution Identification Code - LLVAR n.. 11
            // DE 34: Primary Account Number, Extended - LLVAR ns.. 28

            // DE 37: Retrieval Reference Number - anp12

            // DE 38: Approval Code - anp6
            request.Set(DataElementId.DE_038, builder.AuthorizationCode);

            // DE 39: Action Code - n3

            // DE 41: Card Acceptor Terminal Identification Code - ans8
            string companyIdValue = builder.CompanyId;
            if (string.IsNullOrEmpty(companyIdValue)) {
                companyIdValue = CompanyId;
            }
            request.Set(DataElementId.DE_041, StringUtils.PadRight(companyIdValue, 8, ' '));

            // DE 42: Card Acceptor Identification Code - ans15
            request.Set(DataElementId.DE_042, StringUtils.PadRight(TerminalId, 15, ' '));

            /* DE 43: Card Acceptor Name/Location - LLVAR ans.. 99
                43.1 NAME-STREET-CITY ans..83 Name\street\city\
                43.2 POSTAL-CODE ans10
                43.3 REGION ans3 Two letter state/province code for the United States and Canada. Refer to the Heartland Integrator’s Guide.
                43.4 COUNTRY-CODE a3 See A.30.1 ISO 3166-1: Country Codes, p. 809.
             */
            if (AcceptorConfig.Address != null) {
                DE43_CardAcceptorData cardAcceptorData = new DE43_CardAcceptorData {
                    Address = AcceptorConfig.Address
                };
                request.Set(DataElementId.DE_043, cardAcceptorData);
            }

            /* DE 44: Additional Response Data - LLVAR ans.. 99
                44.1 ACTION REASON CODE n4 Contains the reason code for the action. A value of zeros indicates there is no action reason code.
                44.2 TEXT MESSAGE ans..95 Contains the text describing the action.
             */
            // DE 45: Track 1 Data - LLVAR ans.. 76
            /* DE 46: Amounts, Fees - LLLVAR ans..204
                46.1 FEE TYPE CODE n2
                46.2 CURRENCY CODE, FEE n3
                46.3 AMOUNT, FEE x+n8
                46.4 CONVERSION RATE, FEE n8
                46.5 AMOUNT, RECONCILIATION FEE x+n8
                46.6 CURRENCY CODE, RECONCILIATION FEE n3
             */
            /* DE 48: Message Control - LLLVAR ans..999
                48-0 BIT MAP b8 C Specifies which data elements are present.
                48-1 COMMUNICATION DIAGNOSTICS n4 C Data on communication connection.
                48-2 HARDWARE & SOFTWARE CONFIGURATION ans20 C Version information from POS application.
                48-3 LANGUAGE CODE a2 F Language used for display or print.
                48-4 BATCH NUMBER n10 C Current batch.
                48-5 SHIFT NUMBER n3 C Identifies shift for reconciliation and tracking.
                48-6 CLERK ID LVAR an..9 C Identification of clerk operating the terminal.
                48-7 MULTIPLE TRANSACTION CONTROL n9 F Parameters to control multiple related messages.
                48-8 CUSTOMER DATA LLLVAR ns..250 C Data entered by customer or clerk.
                48-9 TRACK 2 FOR SECOND CARD LLVAR ns..37 C Used to specify the second card in a transaction by the Track 2 format.
                48-10 TRACK 1 FOR SECOND CARD LLVAR ans..76 C Used to specify the second card in a transaction by the Track 1 format.
                48-11 CARD TYPE anp4 C Card type.
                48-12 ADMINISTRATIVELY DIRECTED TASK b1 C Notice to or direction for action to be taken by POS application.
                48-13 RFID DATA LLVAR ans..99 C Data received from RFID transponder.
                48-14 PIN ENCRYPTION METHODOLOGY ans2 C Used to identify the type of encryption methodology.
                48-15, 48-32 RESERVED FOR ANSI USE LLVAR ans..99 These are reserved for future use.
                48-33 POS CONFIGURATION LLVAR ans..99 C Values that indicate to the Heartland system capabilities and configuration of the POS application.
                48-34 MESSAGE CONFIGURATION LLVAR ans..99 C Information regarding the POS originating message and the host generated response message.
                48-35 NAME 1 LLVAR ans..99 D
                48-36 NAME 2 LLVAR ans..99 D
                48-37 SECONDARY ACCOUNT NUMBER LLVAR ans..28 C Second Account Number for manually entered transactions requiring 2 account numbers.
                48-38 RESERVED FOR HEARTLAND USE LLVAR ans..99 F
                48-39 PRIOR MESSAGE INFORMATION LLVAR ans..99 C Information regarding the status of the prior message sent by the POS.
                48-40, 48-49 ADDRESS 1 THROUGH ADDRESS 10 LLVAR ans..99 D One or more types of addresses.
                48-50, 48-64 RESERVED FOR HEARTLAND USE LLVAR ans..99 F
             */
            DE48_MessageControl messageControl = MapMessageControl(builder);
            request.Set(DataElementId.DE_048, messageControl);

            // DE 49: Currency Code, Transaction - n3
            // DE 50: Currency Code, Reconciliation - n3

            /* DE 54: Amounts, Additional - LLVAR ans..120
                54.1 ACCOUNT TYPE, ADDITIONAL AMOUNTS n2 Positions 3 and 4 or positions 5 and 6 of the processing code data element.
                54.2 AMOUNT TYPE, ADDITIONAL AMOUNTS n2 Identifies the purpose of the transaction amounts.
                54.3 CURRENCY CODE, ADDITIONAL AMOUNTS n3 Use DE 49 codes.
                54.4 AMOUNT, ADDITIONAL AMOUNTS x + n12 See Use of the Terms Credit and Debit under Table 1-2 Transaction Processing, p. 61.
             */
            if (builder.CashBackAmount != null && transactionType.Equals(TransactionType.Reversal)) {
                DE54_AmountsAdditional amountsAdditional = new DE54_AmountsAdditional();
                if (paymentMethod.PaymentMethodType.Equals(PaymentMethodType.EBT)) {
                    amountsAdditional.Put(DE54_AmountTypeCode.AmountCash, DE3_AccountType.CashBenefitAccount, currencyCode, builder.CashBackAmount);
                    amountsAdditional.Put(DE54_AmountTypeCode.AmountGoodsAndServices, DE3_AccountType.CashBenefitAccount, currencyCode, (transactionAmount - builder.CashBackAmount));
                }
                else if (paymentMethod.PaymentMethodType.Equals(PaymentMethodType.Debit)) {
                    amountsAdditional.Put(DE54_AmountTypeCode.AmountCash, DE3_AccountType.PinDebitAccount, currencyCode, builder.CashBackAmount);
                    amountsAdditional.Put(DE54_AmountTypeCode.AmountGoodsAndServices, DE3_AccountType.PinDebitAccount, currencyCode, (transactionAmount - builder.CashBackAmount));
                }
                request.Set(DataElementId.DE_054, amountsAdditional);
            }

            // DE 55: Integrated Circuit Card (ICC) Data - LLLVAR b..512
            if (!string.IsNullOrEmpty(builder.TagData)) {
                EmvData tagData = EmvUtils.ParseTagData(builder.TagData, EnableLogging);
                if (!string.IsNullOrEmpty(tagData.GetCardSequenceNumber()))
                    request.Set(DataElementId.DE_023, StringUtils.PadLeft(tagData.GetCardSequenceNumber(), 3, '0'));
                request.Set(DataElementId.DE_055, tagData.GetSendBuffer());
            }

            /* DE 56: Original Data Elements - LLVAR n..35
                56.1 Original message type identifier n4
                56.2 Original system trace audit number n6
                56.3 Original date and time, local transaction n12
                56.4 Original acquiring institution identification code LLVAR n..11
             */
            if (paymentMethod is TransactionReference transactReference) {
                // check that we have enough
                if (!string.IsNullOrEmpty(transactReference.MessageTypeIndicator)
                    && !string.IsNullOrEmpty(transactReference.SystemTraceAuditNumber)
                    && !string.IsNullOrEmpty(transactReference.OriginalTransactionTime)) {
                    DE56_OriginalDataElements originalDataElements = new DE56_OriginalDataElements {
                        MessageTypeIdentifier = transactReference.MessageTypeIndicator,
                        SystemTraceAuditNumber = transactReference.SystemTraceAuditNumber,
                        TransactionDateTime = transactReference.OriginalTransactionTime,
                        AcquiringInstitutionId = transactReference.AcquiringInstitutionId
                    };

                    request.Set(DataElementId.DE_056, originalDataElements);
                }
            }
            // DE 58: Authorizing Agent Institution Identification Code - LLVAR n..11

            // DE 59: Transport Data - LLLVAR ans..999
            request.Set(DataElementId.DE_059, builder.TransportData);

            // DE 62: Card Issuer Data - LLLVAR ans..999
            if (!transactionType.Equals(TransactionType.BatchClose)) {
                DE62_CardIssuerData cardIssuerData = MapCardIssuerData(builder);
                request.Set(DataElementId.DE_062, cardIssuerData);
            }

            // DE 63: Product Data - LLLVAR ans…999
            if (builder.ProductData != null) {
                DE63_ProductData productData = builder.ProductData.ToDataElement();
                request.Set(DataElementId.DE_063, productData);
            }

            // DE 72: Data Record - LLLVAR ans..999
            // DE 73: Date, Action - n6 (YYMMDD)
            // DE 96: Key Management Data - LLLVAR b..999
            // DE 97: Amount, Net Reconciliation - x + n16
            // DE 102: Account Identification 1 - LLVAR ans..28
            // DE 103: Check MICR Data (Account Identification 2) - LLVAR ans..28
            // DE 115: eWIC Overflow Data - LLLVAR ansb..999
            // DE 116: eWIC Overflow Data - LLLVAR ansb..999
            // DE 117: eWIC Data - LLLVAR ansb..999
            // DE 123: Reconciliation Totals - LLLVAR ans..999
            if (transactionType.Equals(TransactionType.BatchClose)) {
                DE123_ReconciliationTotals totals = new DE123_ReconciliationTotals();

                int transactionCount = builder.TransactionCount;
                int proprietaryTransactionCount = 0;
                decimal proprietaryTransactionSales = 0;
                int visaTransactionCount = 0;
                decimal visaTransactionSales = 0;
                int masterCardTransactionCount = 0;
                decimal masterCardTransactionSales = 0;
                int otherCardTransactionCount = 0;
                decimal otherCardTransactionSales = 0;
                int creditCardTransactionCount = 0;
                decimal creditCardTransactionSales = 0;
                int debitEBTTransactionCount = 0;
                decimal debitEBTTransactionSales = 0;
                int creditVoidTransactionCount = 0;
                decimal creditVoidTransactionSales = 0;
                int debitEBTVoidTransactionCount = 0;
                decimal debitEBTVoidTransactionSales = 0;
                int creditReturnTransactionCount = 0;
                decimal creditReturnTransactionSales = 0;
                int debitReturnTransactionCount = 0;
                decimal debitReturnTransactionSales = 0;

                // transaction count
                if (transactionCount == 0) {
                    if (BatchProvider != null) {
                        proprietaryTransactionCount = BatchProvider.ProprietaryTransactionCount;
                        proprietaryTransactionSales = BatchProvider.ProprietaryTransactionSales;
                        visaTransactionCount = BatchProvider.VisaTransactionCount;
                        visaTransactionSales = BatchProvider.VisaTransactionSales;
                        masterCardTransactionCount = BatchProvider.MasterCardTransactionCount;
                        masterCardTransactionSales = BatchProvider.MasterCardTransactionSales;
                        otherCardTransactionCount = BatchProvider.OtherCardTransactionCount;
                        otherCardTransactionSales = BatchProvider.OtherCardTransactionSales;
                        creditCardTransactionCount = BatchProvider.CreditCardTransactionCount;
                        creditCardTransactionSales = BatchProvider.CreditCardTransactionSales;
                        debitEBTTransactionCount = BatchProvider.DebitEBTTransactionCount;
                        debitEBTTransactionSales = BatchProvider.DebitEBTTransactionSales;
                        creditVoidTransactionCount = BatchProvider.CreditVoidTransactionCount;
                        creditVoidTransactionSales = BatchProvider.CreditVoidTransactionSales;
                        debitEBTVoidTransactionCount = BatchProvider.DebitEBTVoidTransactionCount;
                        debitEBTVoidTransactionSales = BatchProvider.DebitEBTVoidTransactionSales;
                        creditReturnTransactionCount = BatchProvider.CreditReturnTransactionCount;
                        creditReturnTransactionSales = BatchProvider.CreditReturnTransactionSales;
                        debitReturnTransactionCount = BatchProvider.DebitReturnTransactionCount;
                        debitReturnTransactionSales = BatchProvider.DebitReturnTransactionSales;
                    }
                }

                totals.SetProprietaryTotals(proprietaryTransactionCount, proprietaryTransactionSales);
                totals.SetVisaSaleTotals(visaTransactionCount, visaTransactionSales);
                totals.SetMasterCardSaleTotals(masterCardTransactionCount, masterCardTransactionSales);
                totals.SetOtherCreditSaleTotals(otherCardTransactionCount, otherCardTransactionSales);
                totals.SetCreditSaleTotals(creditCardTransactionCount, creditCardTransactionSales);
                totals.SetDebitSaleTotals(debitEBTTransactionCount, debitEBTTransactionSales);
                totals.SetCreditVoidTotals(creditVoidTransactionCount, creditVoidTransactionSales);
                totals.SetDebitVoidTotals(debitEBTVoidTransactionCount, debitEBTVoidTransactionSales);
                totals.SetCreditRefundTotals(creditReturnTransactionCount, creditReturnTransactionSales);
                totals.SetDebitRefundTotals(debitReturnTransactionCount, debitReturnTransactionSales);

                request.Set(DataElementId.DE_123, totals);
            }

            // DE 124: Sundry Data - LLLVAR ans..999
            // DE 125: Extended Response Data 1 - LLLVAR ans..999
            // DE 126: Extended Response Data 2 - LLLVAR ans..999

            // DE 127: Forwarding Data - LLLVAR ans..999
            if (paymentMethod is TransactionReference transactionReference1) {
                if (transactionReference1.OriginalPaymentMethod != null && transactionReference1.OriginalPaymentMethod is IEncryptable) {
                    EncryptionData encryptionData = ((IEncryptable)transactionReference1.OriginalPaymentMethod).EncryptionData;

                    if (encryptionData != null) {
                        DE127_ForwardingData forwardingData = new DE127_ForwardingData();
                        forwardingData.AddEncryptionData(AcceptorConfig.SupportedEncryptionType, encryptionData.KTB);

                        // check for encrypted cid -- THIS MAY NOT BE VALID FOR FOLLOW ON TRANSACTIONS WHERE THE CVN SHOULD NOT BE STORED
                        //                    if (reference.getOriginalPaymentMethod() instanceof ICardData) {
                        //                        string encryptedCvn = ((ICardData) paymentMethod).getCvn();
                        //                        if (!string.IsNullOrEmpty(encryptedCvn)) {
                        //                            forwardingData.addEncryptionData(encryptionData.getKtb(), encryptedCvn);
                        //                        }
                        //                    }

                        request.Set(DataElementId.DE_127, forwardingData);
                    }
                }
            }

            return SendRequest(request, builder, orgCorr1, orgCorr2);
        }

        private Transaction ProcessAdministrationRequests(AuthorizationBuilder builder)
        {
            byte[] orgCorr1 = new byte[2];
            byte[] orgCorr2 = new byte[8];

            NetworkMessage request = new NetworkMessage();
            // MTI
            string mti = MapMTI(builder);
            request.MessageTypeIndicator = mti;

            // DE 1: Secondary Bitmap - b8 // M
            request.Set(DataElementId.DE_001, new byte[8]); // TODO: This should be better

            // DE 11: System Trace Audit Number (STAN) - n6 // M
            int stan = builder.SystemTraceAuditNumber;
            if (stan == 0 && StanProvider != null) {
                stan = StanProvider.GenerateStan();
            }
            request.Set(DataElementId.DE_011, StringUtils.PadLeft(stan, 6, '0'));

            // DE 12: Date and Time, Transaction - n12 (YYMMDDhhmmss)
            string timestamp = builder.Timestamp;
            if (string.IsNullOrEmpty(timestamp)) {
                timestamp = DateTime.Now.ToString("yyMMddhhmmss");
            }
            request.Set(DataElementId.DE_012, timestamp);

            // DE 24: Function Code - n3 // M
            request.Set(DataElementId.DE_024, MapFunctionCode(builder));

            // DE 41: Card Acceptor Terminal Identification Code - ans8
            string companyIdValue = builder.CompanyId;
            if (string.IsNullOrEmpty(companyIdValue)) {
                companyIdValue = CompanyId;
            }
            request.Set(DataElementId.DE_041, StringUtils.PadRight(companyIdValue, 8, ' '));

            // DE 42: Card Acceptor Identification Code - ans15
            request.Set(DataElementId.DE_042, StringUtils.PadRight(TerminalId, 15, ' '));

            // DE 72: Data Record - LLLVAR ans..999
            if (builder.POSSiteConfigRecord != null) {
                DE72_DataRecord dataRecord = new DE72_DataRecord("SCFG", builder.POSSiteConfigRecord);
                request.Set(DataElementId.DE_072, dataRecord.ToByteArray());
            }

            return SendRequest(request, builder, orgCorr1, orgCorr2);
        }

        public override string SerializeRequest(AuthorizationBuilder builder) {
            throw new UnsupportedTransactionException("NWS does not support hosted payments.");
        }

        private IDeviceMessage BuildMessage(byte[] message, byte[] orgCorr1, byte[] orgCorr2) {
            int messageLength = message.Length + 32;

            // build the header
            NetworkMessageBuilder buffer = new NetworkMessageBuilder()
                    .Append(messageLength, 2) // EH.1: Total Tran Length
                    .Append(Encoding.UTF8.GetBytes(EnumConverter.GetMapping(Target.NWS, NetworkTransactionType.Transaction))) // EH.2: ID (Transaction only - Keep Alive not supported)
                    .Append(0, 2) // EH.3: Reserved
                    .Append((int)MessageType) // EH.4: Type Message
                    .Append((int)characterSet) // EH.5: Character Set
                    .Append(0) // EH.6: Response Code
                    .Append(0) // EH.7: Response Code Origin
                    .Append(0); // EH.8: Processing Flag

            // EH.9: Protocol Type
            if (ProtocolType.Equals(ProtocolType.Async)) {
                if (MessageType.Equals(MessageType.Heartland_POS_8583) || MessageType.Equals(MessageType.Heartland_NTS)) {
                    buffer.Append(0x07);
                }
                else {
                    buffer.Append((int)ProtocolType);
                }
            }
            else {
                buffer.Append((int)ProtocolType);
            }

            // rest of the header
            buffer.Append((int)ConnectionType) // EH.10: Connection Type
                    .Append(NodeIdentification) // EH.11: Node Identification
                    .Append(orgCorr1) // EH.12: Origin Correlation 1 (2 Bytes)
                    .Append(CompanyId) // EH.13: Company ID
                    .Append(orgCorr2) // EH.14: Origin Correlation 2 (8 bytes)
                    .Append(1); // EH.15: Version (0x01)

            // append the 8683 DATA
            buffer.Append(message);

            return new DeviceMessage(buffer.ToArray());
        }

        private Transaction SendRequest<T>(NetworkMessage request, T builder, byte[] orgCorr1, byte[] orgCorr2) where T : TransactionBuilder<Transaction> {
            byte[] sendBuffer = request.BuildMessage();
            //if (EnableLogging) {
                //System.Diagnostics.Debug.WriteLine("Request Breakdown:\r\n" + request.ToString());
            //}
            IDeviceMessage message = BuildMessage(sendBuffer, orgCorr1, orgCorr2);
            TransactionType? transactionType = null;

            try {
                if (builder != null) {
                    transactionType = builder.TransactionType;
                    ForceGatewayTimeout = builder.ForceGatewayTimeout;
                }
                byte[] responseBuffer = Send(message);

                string functionCode = request.GetString(DataElementId.DE_024);
                string messageReasonCode = request.GetString(DataElementId.DE_025);
                string processingCode = request.GetString(DataElementId.DE_003);
                string stan = request.GetString(DataElementId.DE_011);

                PriorMessageInformation priorMessageInformation = new PriorMessageInformation();
                IPaymentMethod paymentMethod = builder?.PaymentMethod;
                if (paymentMethod != null) {
                    DE48_CardType? cardType = MapCardType(paymentMethod);
                    if (cardType != null) {
                        priorMessageInformation.SetCardType(EnumConverter.GetMapping(Target.NWS, cardType));
                    }
                }

                //            priorMessageInformation.setResponseTime(); // TODO: Need to get this from the send message
                priorMessageInformation.FunctionCode = functionCode;
                priorMessageInformation.MessageReasonCode = messageReasonCode;
                priorMessageInformation.MessageTransactionIndicator = request.MessageTypeIndicator;
                priorMessageInformation.ProcessingCode = processingCode;
                priorMessageInformation.SystemTraceAuditNumber = stan;

                Transaction response = MapResponse(responseBuffer, request, builder);
                response.MessageInformation = priorMessageInformation;
                if (BatchProvider != null) {
                    BatchProvider.SetPriorMessageData(priorMessageInformation);
                }                
                return response;
            }
            catch (GatewayTimeoutException exc) {
                if (builder == null || builder.TransactionType.Equals(TransactionType.Reversal)) {
                    throw exc;
                }

                // add the MTI and ProcessingCode from the original transaction to the timeout
                exc.MessageTypeIndicator = request.MessageTypeIndicator;
                exc.ProcessingCode = request.GetString(DataElementId.DE_003);

                decimal? amount = new decimal();
                decimal? cashBackAmount = new decimal();
                IPaymentMethod paymentMethod = null;
                if (builder is AuthorizationBuilder) {
                    amount = ((AuthorizationBuilder)(object)builder).Amount;
                    paymentMethod = builder.PaymentMethod;
                    cashBackAmount = ((AuthorizationBuilder)(object)builder).CashBackAmount;
                }
                else if (builder is ManagementBuilder) {
                    amount = ((ManagementBuilder)(object)builder).Amount;
                    paymentMethod = builder.PaymentMethod;
                }

                // create transaction reference
                TransactionReference reference = new TransactionReference {
                    OriginalAmount = amount,
                    MessageTypeIndicator = request.MessageTypeIndicator,
                    NtsData = new NtsData(FallbackCode.CouldNotCommunicateWithHost, AuthorizerCode.Interchange_Authorized),
                    OriginalProcessingCode = request.GetString(DataElementId.DE_003)
                };
                if (paymentMethod is TransactionReference transactionReference) {
                    reference.OriginalPaymentMethod = transactionReference.OriginalPaymentMethod;
                }
                else {
                    reference.OriginalPaymentMethod = paymentMethod;
                }
                reference.OriginalTransactionTime = request.GetString(DataElementId.DE_012);
                reference.SystemTraceAuditNumber = request.GetString(DataElementId.DE_011);

                // create builder
                ManagementBuilder reversal = new ManagementBuilder(TransactionType.Reversal)
                        .WithPaymentMethod(reference)
                        .WithAmount(amount)
                        .WithCashBackAmount(cashBackAmount);

                // reuse the batch and sequence number
                DE48_MessageControl messageControl = request.GetDataElement<DE48_MessageControl>(DataElementId.DE_048);
                if (messageControl != null) {
                    reversal.WithBatchNumber(messageControl.BatchNumber, messageControl.SequenceNumber);
                }

                for (int i = 0; i < 3; i++) {
                    exc.ReversalCount = i + 1;
                    try {
                        Transaction reversalResponse = this.ManageTransaction(reversal);
                        //exc.GatewayEvents.AddRange(reversalResponse.GatewayEvents);
                        exc.ReversalResponseCode = reversalResponse.ResponseCode;
                        exc.ReversalResponseText = reversalResponse.ResponseMessage;
                        break;
                    }
                    catch (GatewayTimeoutException r_exc) {
                        //exc.GatewayEvents.AddRange(r_exc.GatewayEvents);
                        exc.ReversalResponseCode = r_exc.ResponseCode;
                        exc.ReversalResponseText = r_exc.ResponseMessage;
                    }
                }

                exc.Host = this.currentEndpoint;
                throw exc;
            }
        }

        private Transaction MapResponse<T>(byte[] buffer, NetworkMessage request, T builder) where T : TransactionBuilder<Transaction> {
            Transaction result = new Transaction();
            MessageReader mr = new MessageReader(buffer);

            // parse the header
            NetworkMessageHeader header = NetworkMessageHeader.Parse(mr.ReadBytes(30));
            if (!header.ResponseCode.Equals(NetworkResponseCode.Success)) {
                GatewayException exc = new GatewayException(
                        string.Format("Unexpected response from gateway: {0} {1}", header.ResponseCode.ToString(), header.ResponseCodeOrigin.ToString()),
                        header.ResponseCode.ToString(),
                        header.ResponseCodeOrigin.ToString()) {
                    GatewayEvents = this.events
                };
                throw exc;
            }
            else {
                result.GatewayEvents = this.events;
                result.ResponseCode = "000";
                result.ResponseMessage = header.ResponseCodeOrigin.ToString();

                // parse the message
                if (!header.MessageType.Equals(MessageType.NoMessage)) {
                    string messageTransactionIndicator = mr.ReadString(4);
                    NetworkMessage message = NetworkMessage.Parse(mr.ReadBytes(buffer.Length), Iso8583MessageType.CompleteMessage);
                    message.MessageTypeIndicator = messageTransactionIndicator;

                    // log out the breakdown
                    //if (EnableLogging) {
                    //    System.Diagnostics.Debug.WriteLine("\r\nResponse Breakdown:\r\n" + message.ToString());
                    //}

                    DE44_AdditionalResponseData additionalResponseData = message.GetDataElement<DE44_AdditionalResponseData>(DataElementId.DE_044);
                    DE48_MessageControl messageControl = message.GetDataElement<DE48_MessageControl>(DataElementId.DE_048);
                    DE54_AmountsAdditional additionalAmounts = message.GetDataElement<DE54_AmountsAdditional>(DataElementId.DE_054);
                    DE62_CardIssuerData cardIssuerData = message.GetDataElement<DE62_CardIssuerData>(DataElementId.DE_062);

                    if (cardIssuerData != null) {
                        result.ReferenceNumber = cardIssuerData.Get("IRR");
                        result.TimeResponseFromHeartland = cardIssuerData.Get("HTR");
                        result.AvsResponseCode = cardIssuerData.Get("IAV");
                        result.CvnResponseCode = cardIssuerData.Get("ICV");
                    }

                    if (additionalAmounts != null) {
                        result.AvailableBalance = additionalAmounts.GetAmount(DE3_AccountType.CashCard_CashAccount, DE54_AmountTypeCode.AccountAvailableBalance);
                    }

                    result.AuthorizedAmount = message.GetAmount(DataElementId.DE_004);
                    result.SystemTraceAuditNumber = request.GetString(DataElementId.DE_011);
                    result.HostResponseDate = message.GetDate(DataElementId.DE_012, "yyMMddhhmmss");
                    result.AuthorizationCode = message.GetString(DataElementId.DE_038);
                    string responseCode = message.GetString(DataElementId.DE_039);
                    string responseText = DE39_ActionCodeMethods.GetDescription(responseCode);


                    if (!string.IsNullOrEmpty(responseCode)) {
                        if (additionalResponseData != null) {
                            responseText += string.Format(" - {0}: {1}", additionalResponseData.ActionReasonCode.ToString(), additionalResponseData.TextMessage);
                        }

                        result.ResponseCode = responseCode;
                        result.ResponseMessage = responseText;

                        if (builder != null) {
                            string transactionToken = CheckResponse(responseCode, request, message, builder);
                            result.TransactionToken = transactionToken;
                        }
                    }

                    // EMV response
                    byte[] emvResponse = message.GetByteArray(DataElementId.DE_055);
                    if (emvResponse != null) {
                        EmvData emvData = EmvUtils.ParseTagData(StringUtils.HexFromBytes(emvResponse), EnableLogging);
                        result.EmvIssuerResponse = emvData.GetAcceptedTagData();
                    }

                    if (builder != null) {
                        // transaction reference
                        IPaymentMethod paymentMethod = builder.PaymentMethod;
                        if (paymentMethod != null) {
                            TransactionReference reference = new TransactionReference {
                                AuthCode = result.AuthorizationCode,

                                // original data elements
                                MessageTypeIndicator = request.MessageTypeIndicator,
                                OriginalApprovedAmount = message.GetAmount(DataElementId.DE_004),
                                OriginalProcessingCode = request.GetString(DataElementId.DE_003),
                                SystemTraceAuditNumber = request.GetString(DataElementId.DE_011),
                                OriginalTransactionTime = request.GetString(DataElementId.DE_012),
                                AcquiringInstitutionId = request.GetString(DataElementId.DE_032)
                        };


                            // partial flag
                            if (!String.IsNullOrEmpty(responseCode)) {
                                if (responseCode.Equals("002")) {
                                    reference.PartialApproval = true;
                                }
                                else if (responseCode.Equals("000")) {
                                    string requestAmount = request.GetString(DataElementId.DE_004);
                                    string responseAmount = message.GetString(DataElementId.DE_004);
                                    reference.PartialApproval = !requestAmount.Equals(responseAmount);
                                }
                            }

                            // message control fields
                            if (messageControl != null) {
                                reference.BatchNumber = messageControl.BatchNumber.ToString();
                            }

                            // card issuer data
                            if (cardIssuerData != null) {
                                reference.SetNtsData(cardIssuerData.Get("NTS"));
                            }

                            // authorization builder
                            if (builder is AuthorizationBuilder) {
                                AuthorizationBuilder authBuilder = (AuthorizationBuilder)(object)builder;
                                reference.OriginalAmount = authBuilder.Amount;
                            }

                            // management builder
                            if (builder is ManagementBuilder) {
                                ManagementBuilder managementBuilder = (ManagementBuilder)(object)builder;
                                reference.OriginalAmount = managementBuilder.Amount;
                            }

                            // original payment method
                            if (paymentMethod is TransactionReference originalReference) {
                                reference.OriginalPaymentMethod = originalReference.OriginalPaymentMethod;

                                // check nts specifics
                                if (reference.NtsData == null) {
                                    reference.NtsData = originalReference.NtsData;
                                }

                                // get original amounts
                                if (reference.OriginalAmount == null) {
                                    reference.OriginalAmount = originalReference.OriginalAmount;
                                }
                            }
                            else {
                                reference.OriginalPaymentMethod = paymentMethod;
                            }
                            result.TransactionReference = reference;
                        }

                        // batch summary
                        if (builder.TransactionType.Equals(TransactionType.BatchClose)) {
                            BatchSummary summary = new BatchSummary {
                                ResponseCode = responseCode,
                                ResentTransactions = ResentTransactions,
                                ResentBatchClose = ResentBatch,
                                TransactionToken = result.TransactionToken,
                                SystemTraceAuditNumber = result.SystemTraceAuditNumber
                            };

                            if (messageControl != null) {
                                summary.Id = messageControl.BatchNumber;
                                summary.SequenceNumber = messageControl.SequenceNumber + "";
                            }

                            DE123_ReconciliationTotals reconciliationTotals = message.GetDataElement<DE123_ReconciliationTotals>(DataElementId.DE_123);
                            if (reconciliationTotals != null) {
                                int transactionCount = 0;
                                decimal totalAmount = new decimal(0);
                                foreach (DE123_ReconciliationTotal total in reconciliationTotals.Totals) {
                                    transactionCount += total.TransactionCount;
                                    totalAmount += total.TotalAmount;
                                }

                                summary.TransactionCount = transactionCount;
                                summary.TotalAmount = totalAmount;
                            }
                            result.BatchSummary = summary;
                        }
                    }
                }
            }

            return result;
        }

        private void Validate<T>(T builder) where T : TransactionBuilder<Transaction> {
            IPaymentMethod paymentMethod = builder.PaymentMethod;
            if (paymentMethod is TransactionReference transactionReference) {
                if (transactionReference.OriginalPaymentMethod != null) {
                    paymentMethod = transactionReference.OriginalPaymentMethod;
                }
            }

            TransactionType transactionType = builder.TransactionType;

            if (paymentMethod is GiftCard giftCard) {
                if (giftCard.CardType.Equals("HeartlandGift") && transactionType.Equals(TransactionType.CashOut)) {
                    throw new BuilderException("Cash Out is not allowed for Heartland gift cards.");
                }
            }

            if (paymentMethod is EBT ebtCard) {
                if (!Enum.IsDefined(typeof(EbtCardType),ebtCard.EbtCardType)) {
                    throw new BuilderException("The card type must be specified for EBT transactions.");
                }

                // no refunds on cash benefit cards
                if (ebtCard.EbtCardType == EbtCardType.CashBenefit && transactionType.Equals(TransactionType.Refund)) {
                    throw new UnsupportedTransactionException("Refunds are not allowed for cash benefit cards.");
                }

                // no authorizations for ebt
                if (transactionType.Equals(TransactionType.Auth)) {
                    throw new UnsupportedTransactionException("Authorizations are not allowed for EBT cards.");
                }

                // no manual balance inquiry
                if (transactionType.Equals(TransactionType.Balance) && !(paymentMethod is ITrackData)) {
                    throw new BuilderException("Track data must be used for EBT balance inquiries.");
                }
            }

            // This is currently not working as expected
            //// ReadyLink specific
            //var approvedReadyLinkTransactionTypes = new List<TransactionType>()
            //{
            //    TransactionType.AddValue,
            //    TransactionType.Reversal,
            //    TransactionType.BatchClose
            //};
            //if (paymentMethod is Credit readylinkCredit && readylinkCredit.ReadyLinkCard) {
            //    if (!approvedReadyLinkTransactionTypes.Contains(transactionType)) {
            //        throw new BuilderException($"Visa ReadyLink cards do not support this transaction type: {transactionType}");
            //    }
            //}
            //else if (paymentMethod is Debit readylinkDebit && readylinkDebit.ReadyLinkCard) {
            //    if (!approvedReadyLinkTransactionTypes.Contains(transactionType)) {
            //        throw new BuilderException($"Visa ReadyLink cards do not support this transaction type: {transactionType}");
            //    }
            //}

            // WEX Specific
            if (paymentMethod is Credit credit) {
                if (credit.CardType.Equals("WexFleet")) {
                    if (transactionType.Equals(TransactionType.Refund)) {
                        if (builder.TransactionMatchingData == null) {
                            throw new BuilderException("Transaction mapping data object required for WEX refunds.");
                        }
                        else {
                            TransactionMatchingData tmd = builder.TransactionMatchingData;
                            if (string.IsNullOrEmpty(tmd.OriginalBatchNumber) || string.IsNullOrEmpty(tmd.OriginalDate)) {
                                throw new BuilderException("Transaction Matching Data incomplete. Original batch number and date are required for WEX refunds.");
                            }
                        }
                    }

                    FleetData fleetData = builder.FleetData;
                    if (fleetData == null && !(paymentMethod is CreditTrackData)) {
                        throw new BuilderException("The purchase device sequence number cannot be null for WEX transactions.");
                    }
                    else if ((fleetData != null && fleetData.PurchaseDeviceSequenceNumber == null) && (paymentMethod is CreditTrackData && ((CreditTrackData)paymentMethod).PurchaseDeviceSequenceNumber == null)) {
                        throw new BuilderException("The purchase device sequence number cannot be null for WEX transactions.");
                    }
                }
            }

            // Commenting this out for now, as we currently do not completely support Banknet settlement 
            //// Mastercard Banknet requirements (No manual entry)
            //if (AcceptorConfig.EchoSettlementData == true) {
            //    if (!(paymentMethod is ITrackData)) {
            //        if (paymentMethod is Credit creditTrack) {
            //            if (creditTrack.CardType.StartsWith("MC")) {
            //                throw new UnsupportedTransactionException("Banknet Settlement (Mastercard) requires the magnetic stripe to be read electronically.");
            //            }
            //        }
            //    }
            //}

            if (builder is ManagementBuilder) {
                ManagementBuilder mb = (ManagementBuilder)(object)builder;
                TransactionReference reference = (TransactionReference)mb.PaymentMethod;

                if (transactionType.Equals(TransactionType.BatchClose)) {
                    if (BatchProvider == null) {
                        if (mb.TransactionCount == 0 || mb.TotalCredits == 0 || mb.TotalDebits == 0) {
                            throw new BuilderException("When an IBatchProvider is not present, you must specify transaction count, total debits and total credits when calling batch close.");
                        }

                        if (mb.BatchNumber == 0) {
                            throw new BuilderException("When an IBatchProvider is not present, you must specify a batch and sequence number for a batch close.");
                        }
                    }
                }

                if (transactionType.Equals(TransactionType.Refund)) {
                    if (reference.OriginalPaymentMethod is EBT ebt) {
                        // no refunds on cash benefit cards
                        if (ebt.EbtCardType == EbtCardType.CashBenefit && transactionType.Equals(TransactionType.Refund)) {
                            throw new UnsupportedTransactionException("Refunds are not allowed for cash benefit cards.");
                        }
                    }
                }

                if (transactionType.Equals(TransactionType.Reversal)) {
                    if (string.IsNullOrEmpty(reference.OriginalProcessingCode)) {
                        throw new BuilderException("The original processing code should be specified when performing a reversal.");
                    }

                    // IRR for fleet reversals
                    if (paymentMethod is Credit credit1) {
                        if (credit1.FleetCard && string.IsNullOrEmpty(mb.ReferenceNumber)) {
                            if (reference.NtsData != null && reference.NtsData.FallbackCode.Equals(FallbackCode.None)) {
                                throw new BuilderException("Reference Number is required for fleet voids/reversals.");
                            }
                        }
                    }
                }

                if (transactionType.Equals(TransactionType.Void)) {
                    // IRR for fleet reversals
                    if (paymentMethod is Credit credit1) {
                        if (credit1.FleetCard && string.IsNullOrEmpty(((ManagementBuilder)(object)builder).ReferenceNumber)) {
                            throw new BuilderException("Reference Number is required for fleet voids/reversals.");
                        }
                    }
                }
            }
            else {
                AuthorizationBuilder authBuilder = (AuthorizationBuilder)(object)builder;
                if (paymentMethod.PaymentMethodType.Equals(PaymentMethodType.Debit)) {
                    if (AcceptorConfig.Address == null) {
                        throw new BuilderException("Address is required in acceptor config for Debit/EBT Transactions.");
                    }
                }

                if (paymentMethod is EBT card) {
                    if (card.EbtCardType.Equals(EbtCardType.FoodStamp) && authBuilder.CashBackAmount != null) {
                        throw new BuilderException("Cash back is not allowed for Food Stamp cards.");
                    }
                }
            }
        }

        private string MapMTI<T>(T builder) where T : TransactionBuilder<Transaction> {
            string mtiValue = "1";

            /* MESSAGE CLASS
                0 Reserved for ISO use
                1 Authorization
                2 Financial
                3 File action
                4 Reversal
                5 Reconciliation
                6 Administrative
                7 Fee collection
                8 Network management
                9 Reserved for ISO use
             */
            switch (builder.TransactionType) {
                case TransactionType.Auth:
                case TransactionType.Balance:
                case TransactionType.Verify:
                    mtiValue += "1";
                    break;
                case TransactionType.Activate:
                case TransactionType.AddValue:
                case TransactionType.BenefitWithdrawal:
                case TransactionType.Capture:
                case TransactionType.CashOut:
                case TransactionType.Refund:
                case TransactionType.Sale:
                case TransactionType.Payment:
                case TransactionType.CashAdvance:
                    mtiValue += "2";
                    break;
                case TransactionType.Reversal:
                case TransactionType.Void:
                    mtiValue += "4";
                    break;
                case TransactionType.BatchClose:
                    mtiValue += "5";
                    break;
                case TransactionType.TimeRequest:
                case TransactionType.SiteConfig:
                    mtiValue += "6";
                    break;
                default:
                    mtiValue += "0";
                    break;
            }

            /* MESSAGE FUNCTION
                0 Request
                1 Request response
                2 Advice
                3 Advice response
                4 Notification
                5–9 Reserved for ISO use
             */
            switch (builder.TransactionType) {
                case TransactionType.BatchClose:
                case TransactionType.Capture:
                case TransactionType.Reversal:
                case TransactionType.Void:
                case TransactionType.Payment:
                case TransactionType.SiteConfig:
                    mtiValue += "2";
                    break;
                case TransactionType.Refund:
                    if (builder.PaymentMethod is Debit || builder.PaymentMethod is EBT || builder.PaymentMethod is GiftCard) {
                        mtiValue += "0";
                    }
                    else mtiValue += "2";
                    break;
                default:
                    mtiValue += "0";
                    break;
            }


            /* TRANSACTION ORIGINATOR
                0 POS application
                1 POS application repeat
                2 Heartland system
                3 Heartland system repeat
                4 POS application or Heartland system
                5 Reserved for Heartland use
                6–9 Reserved for ISO use
            */
            switch (builder.TransactionType) {
                case TransactionType.TimeRequest:
                case TransactionType.SiteConfig:
                    mtiValue += "4";
                    break;
                default:
                    mtiValue += "0";
                    break;
            }

            return mtiValue;
        }

        private DE3_ProcessingCode MapProcessingCode<T>(T builder) where T : TransactionBuilder<Transaction> {
            DE3_ProcessingCode processingCode = new DE3_ProcessingCode();

            TransactionType type = builder.TransactionType;
            if (type.Equals(TransactionType.Reversal)) {
                // set the transaction type to the original transaction type
                if (builder.PaymentMethod is TransactionReference reference) {
                    return processingCode.FromByteArray(Encoding.ASCII.GetBytes(reference.OriginalProcessingCode));
                }
                throw new BuilderException("The processing code must be specified when performing a reversal.");
            }

            // check for an original payment method
            IPaymentMethod paymentMethod = builder.PaymentMethod;
            if (paymentMethod is TransactionReference transactionReference) {
                if (transactionReference.OriginalPaymentMethod != null) {
                    paymentMethod = transactionReference.OriginalPaymentMethod;
                }
            }


            switch (type) {
                case TransactionType.Activate: {
                        AuthorizationBuilder authBuilder = (AuthorizationBuilder)(object)builder;
                        if (authBuilder.Amount != null) {
                            processingCode.TransactionType = DE3_TransactionType.Activate;
                        }
                        else {
                            processingCode.TransactionType = DE3_TransactionType.Activate_PreValuedCard;
                        }
                    }
                    break;
                case TransactionType.AddValue: {
                        processingCode.TransactionType = DE3_TransactionType.Deposit;
                        if (paymentMethod is Debit debit) {
                            if (debit.ReadyLinkCard) {
                                processingCode.TransactionType = DE3_TransactionType.LoadValue;
                            }
                        }
                        else if (paymentMethod is Credit readylinkCredit) { 
                            if (readylinkCredit.ReadyLinkCard) {
                                processingCode.TransactionType = DE3_TransactionType.LoadValue;
                            }
                        }
                    }
                    break;
                case TransactionType.Auth: {
                        AuthorizationBuilder authBuilder = (AuthorizationBuilder)(object)builder;
                        bool isVisa = false;
                        if (paymentMethod is Credit credit1)
                            if (credit1.CardType == "Visa")
                                isVisa = true;

                        // Address Verification: $0 amount, Billing Address required (zip code at bare minimum)
                        // Account Verification: $0 amount, supported by Visa only
                        if (authBuilder.Amount.Equals(decimal.Zero) && !authBuilder.AmountEstimated) {
                            if (authBuilder.BillingAddress != null || isVisa == true) {
                                processingCode.TransactionType = DE3_TransactionType.AddressOrAccountVerification;
                                processingCode.ToAccount = DE3_AccountType.Unspecified;
                                processingCode.FromAccount = DE3_AccountType.Unspecified;
                                return processingCode;
                            }
                        }

                        else if(authBuilder.PaymentMethod is eCheck check) {
                            if (check.CheckVerify) {
                                processingCode.TransactionType = DE3_TransactionType.CheckVerification;
                            }
                            else if(check.CheckGuarantee) {
                                processingCode.TransactionType = DE3_TransactionType.CheckGuarantee;
                            }
                        }
                        else {
                            processingCode.TransactionType = DE3_TransactionType.GoodsAndService;
                        }
                    }
                    break;
                case TransactionType.Balance: {
                        processingCode.TransactionType = DE3_TransactionType.BalanceInquiry;
                    }
                    break;
                case TransactionType.BenefitWithdrawal: {
                        processingCode.TransactionType = DE3_TransactionType.Cash;
                    }
                    break;
                case TransactionType.CashOut: {
                        processingCode.TransactionType = DE3_TransactionType.UnloadValue;
                    }
                    break;
                case TransactionType.Refund: {
                        processingCode.TransactionType = DE3_TransactionType.Return;
                    }
                    break;
                case TransactionType.Sale: {
                        AuthorizationBuilder authBuilder = (AuthorizationBuilder)(object)builder;
                        if (authBuilder.CashBackAmount != null) {
                            processingCode.TransactionType = DE3_TransactionType.GoodsAndServiceWithCashDisbursement;
                        }
                        else {
                            processingCode.TransactionType = DE3_TransactionType.GoodsAndService;
                        }
                    }
                    break;
                case TransactionType.Verify: {
                        processingCode.TransactionType = DE3_TransactionType.BalanceInquiry;
                    }
                    break;
                case TransactionType.CashAdvance: {
                        processingCode.TransactionType = DE3_TransactionType.Cash;
                    }
                    break;
                case TransactionType.Payment: {
                        processingCode.TransactionType = DE3_TransactionType.Payment;
                    }
                    break;
                default:
                    processingCode.TransactionType = DE3_TransactionType.GoodsAndService;
                    break;
            }
            
            // setting the accountType
            DE3_AccountType accountType = DE3_AccountType.Unspecified;
            if (paymentMethod is Credit credit) {
                bool isVisa = false;
                if (credit.CardType == "Visa")
                    isVisa = true;

                if (credit.FleetCard) {
                    accountType = DE3_AccountType.FleetAccount;
                }
                else if (credit.PurchaseCard) {
                    accountType = DE3_AccountType.PurchaseAccount;
                }
                else if (credit.ReadyLinkCard) {
                    accountType = DE3_AccountType.PinDebitAccount;
                }
                else if (credit.CardType == "Unknown") {
                    accountType = DE3_AccountType.PinDebitAccount; // Testing through scenarios in certification have shown this to be exclusive to debit. We can update logic if we find otherwise in the future.
                }
                else {
                    if (isVisa) {
                        EmvData tagData = null;
                        TlvData aidTag = null;
                        if (builder is AuthorizationBuilder) {
                            if ((builder as AuthorizationBuilder).TagData != null) {
                                tagData = EmvUtils.ParseTagData((builder as AuthorizationBuilder)?.TagData, EnableLogging);
                                aidTag = tagData.GetTag("9F06");
                            }
                        }
                        else if (builder is ManagementBuilder) {
                            if ((builder as ManagementBuilder).TagData != null) {
                                tagData = EmvUtils.ParseTagData((builder as ManagementBuilder).TagData, EnableLogging);
                                aidTag = tagData.GetTag("9F06");
                            }
                        }

                        if (aidTag == null) {
                            if (tagData != null) {
                                // If the other AID location was null, check the alternate location for a possible value
                                aidTag = tagData.GetTag("4F");
                            }

                            if (aidTag == null) {
                                accountType = DE3_AccountType.CreditAccount;
                            }
                            else {
                                // Visa US Common Debit is not going through Debit rails so we need to force it
                                if (aidTag.GetValue() == "A0000000980840") {
                                    accountType = DE3_AccountType.PinDebitAccount;
                                }
                                else {
                                    accountType = DE3_AccountType.CreditAccount;
                                }
                            }

                        }
                        else {
                            // Visa US Common Debit is not going through Debit rails so we need to force it
                            if (aidTag.GetValue() == "A0000000980840") {
                                accountType = DE3_AccountType.PinDebitAccount;
                            }
                            else  {
                                accountType = DE3_AccountType.CreditAccount;
                            }
                        }
                    }
                    else {
                        accountType = DE3_AccountType.CreditAccount;
                    }
                }
            }
            else if (paymentMethod is Debit) {
                accountType = DE3_AccountType.PinDebitAccount;
            }
            else if (paymentMethod is GiftCard) {
                accountType = DE3_AccountType.CashCardAccount;
            }
            else if (paymentMethod is EBT ebtCard) {
                if (ebtCard.EbtCardType.Equals(EbtCardType.CashBenefit)) {
                    accountType = DE3_AccountType.CashBenefitAccount;
                }
                else {
                    accountType = DE3_AccountType.FoodStampsAccount;
                }
            }
            else if (paymentMethod is eCheck) {
                accountType = DE3_AccountType.CheckingAccount;
            }
            else if (paymentMethod is Ewic) {
                accountType = DE3_AccountType.EWIC;
            }
            
            switch (type) {
                case TransactionType.Activate:
                case TransactionType.AddValue:
                case TransactionType.Refund:
                    processingCode.ToAccount = accountType;
                    processingCode.FromAccount = DE3_AccountType.Unspecified;
                    break;
                case TransactionType.Payment:
                    processingCode.ToAccount = DE3_AccountType.PrivateLabelAccount;
                    processingCode.FromAccount = DE3_AccountType.Unspecified;
                    break;
                default:
                    processingCode.FromAccount = accountType;
                    processingCode.ToAccount = DE3_AccountType.Unspecified;
                    break;
            }

            return processingCode;
        }

        private string MapFunctionCode<T>(T builder) where T : TransactionBuilder<Transaction> {
            TransactionType type = builder.TransactionType;
            TransactionModifier modifier = builder.TransactionModifier;

            switch (type) {
                case TransactionType.Activate:
                case TransactionType.AddValue:
                case TransactionType.BenefitWithdrawal:
                case TransactionType.CashOut:
                case TransactionType.Refund:
                case TransactionType.Sale:
                case TransactionType.CashAdvance:
                case TransactionType.Payment: {
                        return "200";
                    }
                case TransactionType.Capture: {
                        ManagementBuilder managementBuilder = (ManagementBuilder)(object)builder;

                        if (managementBuilder.Amount != null) {
                            if (managementBuilder.PaymentMethod != null && managementBuilder.PaymentMethod is TransactionReference) {
                                TransactionReference reference = (TransactionReference)managementBuilder.PaymentMethod;

                                if (managementBuilder.Amount == reference.OriginalAmount) {
                                    return "201";
                                }
                                return "202";
                            }
                        }
                        return "201";
                    }
                case TransactionType.Auth: {
                        bool isVisa = false;
                        if (builder.PaymentMethod is Credit credit) {
                            if (credit.CardType == "Visa") {
                                isVisa = true;
                            }
                        }

                        if (modifier.Equals(TransactionModifier.Offline)) {
                            return "190";
                        }

                        if (builder is AuthorizationBuilder) {
                            AuthorizationBuilder authBuilder = (AuthorizationBuilder)(object)builder;
                            if (authBuilder.AmountEstimated) {
                                return "101";
                            }
                            else if (authBuilder.Amount.Equals(decimal.Zero) && !authBuilder.AmountEstimated) {
                                {
                                    if (authBuilder.BillingAddress != null || isVisa == true) {
                                        return "181";
                                    }
                                }
                            }
                        }
                        return "100";
                    }
                case TransactionType.Verify:
                case TransactionType.Balance: {
                        return "108";
                    }
                case TransactionType.BatchClose: {
                        ManagementBuilder managementBuilder = (ManagementBuilder)(object)builder;
                        if (managementBuilder.BatchCloseType == default(BatchCloseType) || managementBuilder.BatchCloseType.Equals(BatchCloseType.Forced)) {
                            return "572";
                        }
                        return "570"; // EndOfShift
                    }
                case TransactionType.Reversal: {
                        ManagementBuilder managementBuilder = (ManagementBuilder)(object)builder;

                        if (managementBuilder.Amount != null) {
                            if (managementBuilder.PaymentMethod != null && managementBuilder.PaymentMethod is TransactionReference transactionReference) {
                                TransactionReference reference = (TransactionReference)managementBuilder.PaymentMethod;
                                if (managementBuilder.Amount == transactionReference.OriginalAmount) {
                                    return "400";
                                }
                                return "401";
                            }
                        }
                        return "400";
                    }
                case TransactionType.Void: {
                        ManagementBuilder managementBuilder = (ManagementBuilder)(object)builder;
                        TransactionReference paymentMethod = (TransactionReference)builder.PaymentMethod;
                        bool partial = false;
                        if (paymentMethod != null) {
                            partial = paymentMethod.PartialApproval;
                        }
                        if (managementBuilder.VoidReason == VoidReason.DeviceTimeout && managementBuilder.ForcedReversal == true) {
                            if (partial == true) {
                                return "441";
                            }
                            else {
                                return "444";
                            }
                        }
                        else {
                            return "441";
                        }
                    }
                case TransactionType.TimeRequest: {
                        return "641";
                    }
                case TransactionType.SiteConfig: {
                        return "692";
                    }
                default: {
                        return "000";
                    }
            }
        }

        private DE48_MessageControl MapMessageControl<T>(T builder) where T : TransactionBuilder<Transaction> {
            DE48_MessageControl messageControl = new DE48_MessageControl();
            /* DE 48: Message Control - LLLVAR ans..999
                48-0 BIT MAP b8 C Specifies which data elements are present.
                48-1 COMMUNICATION DIAGNOSTICS n4 C Data on communication connection.
                48-2 HARDWARE & SOFTWARE CONFIGURATION ans20 C Version information from POS application.
                48-3 LANGUAGE CODE a2 F Language used for display or print.
                48-4 BATCH NUMBER n10 C Current batch.
                48-5 SHIFT NUMBER n3 C Identifies shift for reconciliation and tracking.
                48-6 CLERK ID LVAR an..9 C Identification of clerk operating the terminal.
                48-7 MULTIPLE TRANSACTION CONTROL n9 F Parameters to control multiple related messages.
                48-8 CUSTOMER DATA LLLVAR ns..250 C Data entered by customer or clerk.
                48-9 TRACK 2 FOR SECOND CARD LLVAR ns..37 C Used to specify the second card in a transaction by the Track 2 format.
                48-10 TRACK 1 FOR SECOND CARD LLVAR ans..76 C Used to specify the second card in a transaction by the Track 1 format.
                48-11 CARD TYPE anp4 C Card type.
                48-12 ADMINISTRATIVELY DIRECTED TASK b1 C Notice to or direction for action to be taken by POS application.
                48-13 RFID DATA LLVAR ans..99 C Data received from RFID transponder.
                48-14 PIN ENCRYPTION METHODOLOGY ans2 C Used to identify the type of encryption methodology.
                48-15, 48-32 RESERVED FOR ANSI USE LLVAR ans..99 These are reserved for future use.
                48-33 POS CONFIGURATION LLVAR ans..99 C Values that indicate to the Heartland system capabilities and configuration of the POS application.
                48-34 MESSAGE CONFIGURATION LLVAR ans..99 C Information regarding the POS originating message and the host generated response message.
                48-35 NAME 1 LLVAR ans..99 D
                48-36 NAME 2 LLVAR ans..99 D
                48-37 SECONDARY ACCOUNT NUMBER LLVAR ans..28 C Second Account Number for manually entered transactions requiring 2 account numbers.
                48-38 RESERVED FOR HEARTLAND USE LLVAR ans..99 F
                48-39 PRIOR MESSAGE INFORMATION LLVAR ans..99 C Information regarding the status of the prior message sent by the POS.
                48-40, 48-49 ADDRESS 1 THROUGH ADDRESS 10 LLVAR ans..99 D One or more types of addresses.
                48-50, 48-64 RESERVED FOR HEARTLAND USE LLVAR ans..99 F
             */
            // DE48-2 - Hardware/Software Config
            DE48_2_HardwareSoftwareConfig hardwareSoftwareConfig = new DE48_2_HardwareSoftwareConfig {
                HardwareLevel = AcceptorConfig.HardwareLevel,
                SoftwareLevel = AcceptorConfig.SoftwareLevel,
                OperatingSystemLevel = AcceptorConfig.OperatingSystemLevel
            };
            messageControl.HardwareSoftwareConfig = hardwareSoftwareConfig;

            // DE48-4 (Sequence Number & Batch Number)
            if (!builder.TransactionType.Equals(TransactionType.Auth) || !builder.TransactionType.Equals(TransactionType.Balance)) {
                int sequenceNumber = 0;
                if (!builder.TransactionType.Equals(TransactionType.BatchClose)) {
                    sequenceNumber = builder.SequenceNumber;
                    if (sequenceNumber == 0 && BatchProvider != null) {
                        sequenceNumber = BatchProvider.GetSequenceNumber();
                    }
                }
                messageControl.SequenceNumber = sequenceNumber;

                int batchNumber = builder.BatchNumber;
                if (batchNumber == 0 && BatchProvider != null) {
                    batchNumber = BatchProvider.GetBatchNumber();
                }
                messageControl.BatchNumber = batchNumber;
            }

            // DE48-5
            if (builder is AuthorizationBuilder) {
                AuthorizationBuilder authBuilder = (AuthorizationBuilder)(object)builder;
                messageControl.ShiftNumber = authBuilder.ShiftNumber;
            }

            // 48-6 CLERK ID
            if (builder is AuthorizationBuilder) {
                AuthorizationBuilder authBuilder = (AuthorizationBuilder)(object)builder;
                messageControl.ClerkId = authBuilder.ClerkId;
            }

            // DE48-8 Customer Data
            DE48_8_CustomerData customerData = new DE48_8_CustomerData();
            if (builder is AuthorizationBuilder) {
                AuthorizationBuilder authBuilder = (AuthorizationBuilder)(object)builder;

                // postal code
                // Don't send this if the payment method is an echeck
                if (!(builder.PaymentMethod is eCheck)) {
                    if (authBuilder.BillingAddress != null) {
                        Address address = authBuilder.BillingAddress;
                        customerData.Set(DE48_CustomerDataType.PostalCode, address.PostalCode);
                    }
                }

                if (builder.PaymentMethod is eCheck check)
                {
                    customerData.Set(DE48_CustomerDataType.DriverLicense_Number, check.DriversLicenseNumber);
                    customerData.Set(DE48_CustomerDataType.DriverLicense_State_Province, check.DriversLicenseState);
                }
            }

            // fleet data
            if (builder.FleetData != null) {
                FleetData fleetData = builder.FleetData;

                if (builder is AuthorizationBuilder) {
                    if (!string.IsNullOrEmpty((builder as AuthorizationBuilder).TagData)) {
                        customerData.EmvFlag = true;
                    }
                }

                List<string> paddingRequiredForTheseCardTypes = new List<string>()
                {
                    "VoyagerFleet",
                    "MCFleet",
                    "VisaFleet"
                };

                Credit fleetCard = null;
                if (builder.PaymentMethod is Credit) {
                    fleetCard = builder.PaymentMethod as Credit;
                }
                else if (builder.PaymentMethod is TransactionReference) {
                    fleetCard = (builder.PaymentMethod as TransactionReference).OriginalPaymentMethod as Credit;
                }

                if (paddingRequiredForTheseCardTypes.Contains(fleetCard.CardType)) {
                    customerData.Set(DE48_CustomerDataType.Vehicle_Trailer_Number, (fleetData.VehicleNumber != null) ? StringUtils.PadLeft(fleetData.VehicleNumber, 6, '0') : fleetData.VehicleNumber);
                    customerData.Set(DE48_CustomerDataType.DriverId_EmployeeNumber, (fleetData.DriverId != null) ? StringUtils.PadLeft(fleetData.DriverId, 6, '0') : fleetData.DriverId);
                    customerData.Set(DE48_CustomerDataType.Odometer_Hub_Reading, (fleetData.OdometerReading != null) ? StringUtils.PadLeft(fleetData.OdometerReading, 6, '0') : fleetData.OdometerReading);
                }
                else {
                    customerData.Set(DE48_CustomerDataType.Vehicle_Trailer_Number, fleetData.VehicleNumber);
                    customerData.Set(DE48_CustomerDataType.DriverId_EmployeeNumber, fleetData.DriverId);
                    customerData.Set(DE48_CustomerDataType.Odometer_Hub_Reading, fleetData.OdometerReading);
                }

                customerData.Set(DE48_CustomerDataType.UnencryptedIdNumber, fleetData.UserId);
                customerData.Set(DE48_CustomerDataType.VehicleTag, fleetData.VehicleTag);
                customerData.Set(DE48_CustomerDataType.DriverLicense_Number, fleetData.DriversLicenseNumber);
                customerData.Set(DE48_CustomerDataType.TrailerHours_ReferHours, fleetData.TrailerReferHours);
                customerData.Set(DE48_CustomerDataType.EnteredData_Numeric, fleetData.EnteredData);
                customerData.Set(DE48_CustomerDataType.ServicePrompt, fleetData.ServicePrompt);
                customerData.Set(DE48_CustomerDataType.JobNumber, fleetData.JobNumber);
                customerData.Set(DE48_CustomerDataType.Department, fleetData.Department);
                customerData.Set(DE48_CustomerDataType.TripNumber, fleetData.TripNumber);
                customerData.Set(DE48_CustomerDataType.UnitNumber, fleetData.UnitNumber);
            }

            // cvn number
            if (builder.PaymentMethod is ICardData card) {
                IEncryptable encryption = null;
                if (builder.PaymentMethod is IEncryptable) {
                    encryption = (IEncryptable)builder.PaymentMethod;
                }

                if (!string.IsNullOrEmpty(card.Cvn)) {
                    string cvn = card.Cvn;
                    if (encryption != null && encryption.EncryptionData != null) {
                        cvn = StringUtils.PadLeft("", card.Cvn.Length, ' ');
                    }
                    customerData.Set(DE48_CustomerDataType.CardPresentSecurityCode, cvn);
                }
            }

            // gift pin
            if (builder.PaymentMethod is GiftCard giftCard) {
                if (!string.IsNullOrEmpty(giftCard.Pin)) {
                    customerData.Set(DE48_CustomerDataType.CardPresentSecurityCode, giftCard.Pin);
                }
            }

            if (customerData.GetFieldCount() > 0) {
                messageControl.CustomerData = customerData;
                //System.Diagnostics.Debug.WriteLine($"DE48_CustomerData: {customerData}");
            }

            // DE48-11
            // We are checking the AID EMV tag for specific application values, as they should be run with specific card types they won't get caught by the mapping control.
            // We also need to null check the TagData property because ParseTagData does not handle null checking itself.
            TlvData aidTag = null;
            bool maestroCard = false;
            if (builder is AuthorizationBuilder) {
                if ((builder as AuthorizationBuilder).TagData != null) {
                    EmvData tagData = EmvUtils.ParseTagData((builder as AuthorizationBuilder).TagData, EnableLogging);
                    aidTag = tagData.GetTag("9F06");
                }
            }
            else if (builder is ManagementBuilder) {
                if ((builder as ManagementBuilder).TagData != null) {
                    EmvData tagData = EmvUtils.ParseTagData((builder as ManagementBuilder).TagData, EnableLogging);
                    aidTag = tagData.GetTag("9F06");
                }
            }
            // If the tag is still null, check the other potential AID location
            if (aidTag == null) {
                if (builder is AuthorizationBuilder) {
                    if ((builder as AuthorizationBuilder).TagData != null) {
                        EmvData tagData = EmvUtils.ParseTagData((builder as AuthorizationBuilder).TagData, EnableLogging);
                        aidTag = tagData.GetTag("4F");
                    }
                }
                else if (builder is ManagementBuilder) {
                    if ((builder as ManagementBuilder).TagData != null) {
                        EmvData tagData = EmvUtils.ParseTagData((builder as ManagementBuilder).TagData, EnableLogging);
                        aidTag = tagData.GetTag("4F");
                    }
                }
            }

            // If we have a value by this point, we need to check the value to see if we should force PINDEBIT card type
            if (aidTag != null) {
                if (aidTag.GetValue() == "A0000000042203") {
                    maestroCard = true;
                }
            }

            if (maestroCard) {
                messageControl.CardType = DE48_CardType.PINDebitCard;
            }
            else {
                messageControl.CardType = MapCardType(builder.PaymentMethod);
            }

            // DE48-14
            if (builder.PaymentMethod is IPinProtected) {
                DE48_14_PinEncryptionMethodology pinEncryptionMethodology = new DE48_14_PinEncryptionMethodology {
                    KeyManagementDataCode = DE48_KeyManagementDataCode.DerivedUniqueKeyPerTransaction_DUKPT,
                    EncryptionAlgorithmDataCode = DE48_EncryptionAlgorithmDataCode.TripleDES_3Keys
                };
                messageControl.PinEncryptionMethodology = pinEncryptionMethodology;
            }

            // DE48-33
            if (!(builder.PaymentMethod is eCheck)) {
                if (AcceptorConfig.HasPosConfiguration_MessageControl()) {
                    DE48_33_PosConfiguration posConfiguration = new DE48_33_PosConfiguration();
                    posConfiguration.Timezone = posConfiguration.Timezone;
                    posConfiguration.SupportsPartialApproval = AcceptorConfig.SupportsPartialApproval;
                    posConfiguration.SupportsReturnBalance = AcceptorConfig.SupportsReturnBalance;
                    posConfiguration.SupportsCashOver = AcceptorConfig.SupportsCashOver;
                    posConfiguration.MobileDevice = AcceptorConfig.MobileDevice;
                    messageControl.PosConfiguration = posConfiguration;
                }
            }

            // DE48-34 // Message Configuration Fields
            if (AcceptorConfig.HasPosConfiguration_MessageData()) {
                DE48_34_MessageConfiguration messageConfig = new DE48_34_MessageConfiguration()
                {
                    PerformDateCheck = AcceptorConfig.PerformDateCheck,
                    EchoSettlementData = AcceptorConfig.EchoSettlementData,
                    IncludeLoyaltyData = AcceptorConfig.IncludeLoyaltyData,
                };
                messageControl.MessageConfiguration = messageConfig;
            }

            // DE48-39 // Not a follow up message these should be defaults
            PriorMessageInformation priorMessageInformation = new PriorMessageInformation();
            if (builder.PriorMessageInformation != null) {
                priorMessageInformation = builder.PriorMessageInformation;
            }
            else if (BatchProvider != null && BatchProvider.GetPriorMessageData() != null) {
                priorMessageInformation = BatchProvider.GetPriorMessageData();
            }

            DE48_39_PriorMessageInformation pmi = new DE48_39_PriorMessageInformation {
                ResponseTime = priorMessageInformation.ResponseTime,
                CardType = priorMessageInformation.GetCardType(),
                MessageTransactionIndicator = priorMessageInformation.MessageTransactionIndicator,
                ProcessingCode = priorMessageInformation.ProcessingCode,
                Stan = priorMessageInformation.SystemTraceAuditNumber
            };

            // Check authorizations do not support DE 48-39
            if (!(builder.PaymentMethod is eCheck)) {
                messageControl.PriorMessageInformation = pmi;
            }

            // DE48-40 Addresses
            if (builder is AuthorizationBuilder) {
                AuthorizationBuilder authBuilder = (AuthorizationBuilder)(object)builder;
                if (authBuilder.BillingAddress != null) {
                    DE48_Address billing = new DE48_Address {
                        Address = authBuilder.BillingAddress,
                        AddressUsage = DE48_AddressUsage.Billing
                    };
                    if (authBuilder.Amount.Equals(decimal.Zero) && !authBuilder.AmountEstimated) {
                        billing.AddressType = DE48_AddressType.AddressVerification;
                    }
                    else {
                        billing.AddressType = DE48_AddressType.StreetAddress;
                    }
                    messageControl.AddAddress(billing);
                }

                if (authBuilder.ShippingAddress != null) {
                    DE48_Address shipping = new DE48_Address {
                        Address = authBuilder.ShippingAddress,
                        AddressUsage = DE48_AddressUsage.Shipping,
                        AddressType = DE48_AddressType.StreetAddress
                    };
                    messageControl.AddAddress(shipping);
                }
            }
            return messageControl;
        }

        private DE62_CardIssuerData MapCardIssuerData<T>(T builder) where T : TransactionBuilder<Transaction> {
            // DE 62: Card Issuer Data - LLLVAR ans..999
            DE62_CardIssuerData cardIssuerData = new DE62_CardIssuerData();

            // unique device id
            if (!string.IsNullOrEmpty(builder.UniqueDeviceId)) {
                cardIssuerData.Add(DE62_CardIssuerEntryTag.UniqueDeviceId, builder.UniqueDeviceId);
            }
            else if (!string.IsNullOrEmpty(UniqueDeviceId)) {
                cardIssuerData.Add(DE62_CardIssuerEntryTag.UniqueDeviceId, UniqueDeviceId);
            }

            // eWIC
            if (builder.PaymentMethod.PaymentMethodType == PaymentMethodType.Ewic) {
                cardIssuerData.Add(DE62_CardIssuerEntryTag.EwicIssuingEntity, builder.EWICIssuingEntity);
                cardIssuerData.Add(DE62_CardIssuerEntryTag.EwicMerchantId, EWICMerchantId);
            }
            else if (builder.PaymentMethod is eCheck check) {
                if (builder is AuthorizationBuilder)
                {
                    AuthorizationBuilder authBuilder = builder as AuthorizationBuilder;

                    DE62_C00_2_VerificationType verificationType = MapCheckVerificationType(authBuilder);

                    string checkType = EnumConverter.GetMapping(Target.NWS, check.CheckType);
                    string strVerificationType = EnumConverter.GetMapping(Target.NWS, verificationType);

                    cardIssuerData.Add(DE62_CardIssuerEntryTag.CheckInformation, checkType + strVerificationType);

                    DE103_Check_MICR_Data checkData = new DE103_Check_MICR_Data()
                    {
                        AccountNumber = check.AccountNumber,
                        TransitNumber = check.RoutingNumber,
                        SequenceNumber = check.CheckNumber
                    };

                    if (!string.IsNullOrEmpty(authBuilder.RawMICRData)) {
                        cardIssuerData.Add(DE62_CardIssuerEntryTag.CheckExpandedOrRawMICRData, authBuilder.RawMICRData);
                    }
                    else if (checkData.ToByteArray().Length > 28) {
                        cardIssuerData.Add(DE62_CardIssuerEntryTag.CheckExpandedOrRawMICRData, checkData.ToString());
                    }
                }
                
            }

            // wex support
            if (builder.PaymentMethod != null) {
                IPaymentMethod paymentMethod = builder.PaymentMethod;
                if (paymentMethod is TransactionReference transactionReference) {
                    paymentMethod = transactionReference.OriginalPaymentMethod;
                }

                if (paymentMethod is Credit && ((Credit)paymentMethod).CardType.Equals("WexFleet")) {
                    cardIssuerData.Add(DE62_CardIssuerEntryTag.Wex_SpecVersionSupport, "0401");
                    if (builder.TransactionType.Equals(TransactionType.Refund)) {
                        cardIssuerData.Add(DE62_CardIssuerEntryTag.IssuerSpecificTransactionMatchData, builder.TransactionMatchingData.GetElementData());
                    }

                    // purchase device sequence number
                    if (builder.FleetData != null && builder.FleetData.PurchaseDeviceSequenceNumber != null) {
                        cardIssuerData.Add(DE62_CardIssuerEntryTag.Wex_PurchaseDeviceSequenceNumber, builder.FleetData.PurchaseDeviceSequenceNumber);
                    }
                    else if (paymentMethod is CreditTrackData creditTrackData) {
                        cardIssuerData.Add(DE62_CardIssuerEntryTag.FleetCards, "F01", creditTrackData.PurchaseDeviceSequenceNumber);
                    }
                }
            }

            // management builder related
            if (builder is ManagementBuilder) {
                ManagementBuilder mb = (ManagementBuilder)(object)builder;

                // IRR Issuer Reference Number
                if (!string.IsNullOrEmpty(mb.ReferenceNumber)) {
                    cardIssuerData.Add(DE62_CardIssuerEntryTag.RetrievalReferenceNumber, mb.ReferenceNumber);
                }

                // NTE Terminal Error
                if (mb.TransactionType.Equals(TransactionType.BatchClose) && mb.BatchCloseType.Equals(BatchCloseType.Forced)) {
                    cardIssuerData.Add(DE62_CardIssuerEntryTag.TerminalError, "Y");
                }

                if (mb.PaymentMethod is TransactionReference reference) {
                    // NTS Specific Data
                    //if (reference.NtsData != null) {
                    //    cardIssuerData.Add(DE62_CardIssuerEntryTag.NTS_System, reference.NtsData.ToString());
                    //}

                    // original payment method
                    if (reference.OriginalPaymentMethod != null) {
                        if (reference.OriginalPaymentMethod is CreditCardData) {
                            cardIssuerData.Add(DE62_CardIssuerEntryTag.SwipeIndicator, "NSI", "0");
                        }
                        else if (reference.OriginalPaymentMethod is ITrackData track) {
                            string nsiValue = track.TrackNumber.Equals(TrackNumber.TrackTwo) ? "2" : "1";
                            cardIssuerData.Add(DE62_CardIssuerEntryTag.SwipeIndicator, nsiValue);
                        }
                    }
                }
            }
            else {
                AuthorizationBuilder authBuilder = (AuthorizationBuilder)(object)builder;
                if (authBuilder.EmvChipCondition != default(EmvLastChipRead)) {
                    cardIssuerData.Add(DE62_CardIssuerEntryTag.ChipConditionCode, MapChipCondition(authBuilder.EmvChipCondition));
                }
            }

            // catch all
            if (builder.IssuerData != null) {
                Dictionary<DE62_CardIssuerEntryTag, string> issuerData = builder.IssuerData;
                foreach (DE62_CardIssuerEntryTag tag in issuerData.Keys) {
                    cardIssuerData.Add(tag, issuerData[tag]);
                }
            }

            // put if there are entries
            if (cardIssuerData.GetNumEntries() > 0) {
                return cardIssuerData;
            }
            return null;
        }

        private DE62_C00_2_VerificationType MapCheckVerificationType<T>(T builder) where T : TransactionBuilder<Transaction> {
            AuthorizationBuilder authBuilder = builder as AuthorizationBuilder;
            eCheck check = authBuilder.PaymentMethod as eCheck;

            DE62_C00_2_VerificationType verificationType;
            if (string.IsNullOrEmpty(authBuilder.RawMICRData)) {
                verificationType = DE62_C00_2_VerificationType.FormattedMICRData;
                // Check the eCheck for the driver's license first
                if (!string.IsNullOrEmpty(check.DriversLicenseNumber) && !string.IsNullOrEmpty(check.DriversLicenseState)) {
                    verificationType = DE62_C00_2_VerificationType.FormattedMICR_DriversLicense;
                }
            }
            else {
                verificationType = DE62_C00_2_VerificationType.RawMICRData;
                // Check the eCheck for the driver's license first
                if (!string.IsNullOrEmpty(check.DriversLicenseNumber) && !string.IsNullOrEmpty(check.DriversLicenseState)) {
                    verificationType = DE62_C00_2_VerificationType.RawMICR_DriversLicense;
                }
            }

            return verificationType;
        }

        private DE48_CardType? MapCardType(IPaymentMethod paymentMethod) {
            // check to see if the original payment method is set
            if (paymentMethod is TransactionReference transactionReference) {
                if (transactionReference.OriginalPaymentMethod != null) {
                    paymentMethod = transactionReference.OriginalPaymentMethod;
                }
            }

            // evaluate and return
            if (paymentMethod is DebitTrackData) {
                return DE48_CardType.PINDebitCard;
            }
            else if (paymentMethod is Credit card) {
                if (card.CardType.Equals("Amex")) {
                    return DE48_CardType.AmericanExpress;
                }
                else if (card.CardType.Equals("MC")) {
                    return DE48_CardType.Mastercard;
                }
                else if (card.CardType.Equals("MCFleet")) {
                    return DE48_CardType.MastercardFleet;
                }
                else if (card.CardType.Equals("WexFleet")) {
                    return DE48_CardType.WEX;
                }
                else if (card.CardType.Equals("Visa")) {
                    return DE48_CardType.Visa;
                }
                else if (card.CardType.Equals("VisaFleet")) {
                    return DE48_CardType.VisaFleet;
                }
                else if (card.CardType.Equals("VisaReadyLink")) {
                    return DE48_CardType.PINDebitCard; // ReadyLink must have a card type of DB, PIN Debit
                }
                else if (card.CardType.Equals("DinersClub")) {
                    return DE48_CardType.DinersClub;
                }
                else if (card.CardType.Equals("Discover")) {
                    return DE48_CardType.DiscoverCard;
                }
                else if (card.CardType.Equals("Jcb")) {
                    return DE48_CardType.JCB;
                }
                else if (card.CardType.Equals("FleetOneFleet")) {
                    return DE48_CardType.FleetOne;
                }
                else if (card.CardType.Equals("VoyagerFleet")) {
                    return DE48_CardType.Voyager;
                }
                else if (card.CardType.Equals("VisaPurchasing")) {
                    return DE48_CardType.VisaPurchasing;
                }
                else if (card.CardType.Equals("MCPurchasing")) {
                    return DE48_CardType.MastercardPurchasing;
                }
                else if (card.CardType.Equals("FleetCorFuelmanPlusFleet")) {
                    return DE48_CardType.FleetCorFuelmanPlus;
                }
                else if (card.CardType.Equals("FleetCorFleetwideFleet")) {
                    return DE48_CardType.FleetCorFleetwide;
                }
                else if (card.CardType.Equals("FleetCorGasCardFleet")) {
                    return DE48_CardType.FleetCorFleetwide; // This is intentional - GasCard and Fleetwide share a code
                }

            }
            else if (paymentMethod is GiftCard giftCard) {
                if (giftCard.CardType.Equals("ValueLink")) {
                    return DE48_CardType.ValueLinkStoredValue;
                }
                else if (giftCard.CardType.Equals("StoredValue")) {
                    return DE48_CardType.SVSStoredValue;
                }
                else if (giftCard.CardType.Equals("HeartlandGift")) {
                    return DE48_CardType.HeartlandGiftCard_Proprietary;
                }
            }
            else if (paymentMethod is EBT ebtcard) {
                if (ebtcard.EbtCardType.Equals(EbtCardType.FoodStamp)) {
                    return DE48_CardType.EBTFoodStamps;
                }
                return DE48_CardType.EBTCash;
            }
            else if (paymentMethod is Ewic ewic) {
                return DE48_CardType.eWIC;
            }
            return null;
        }

        private string MapChipCondition(EmvLastChipRead chipCondition) {
            switch (chipCondition) {
                case EmvLastChipRead.Successful:
                case EmvLastChipRead.Failed:
                    return "S"; // TODO: this is not correct
                default:
                    return null;
            }
        }

        private CardHolderAuthenticationMethod MapAuthenticationMethod<T>(T builder) where T : TransactionBuilder<Transaction> {
            IPaymentMethod paymentMethod = builder.PaymentMethod;
            PaymentMethodType? paymentMethodType = builder?.PaymentMethod?.PaymentMethodType;

            CardHolderAuthenticationMethod authenticationMethod = CardHolderAuthenticationMethod.NotAuthenticated;


            // Manual card
            if (paymentMethod is ICardData card) {
                if (!string.IsNullOrEmpty(card.Cvn)) {
                    authenticationMethod = CardHolderAuthenticationMethod.OnCard_SecurityCode;
                }

                // Inside, at the counter ("W")
                if (AcceptorConfig.CardDataInputCapability == CardDataInputCapability.ContactlessEmv_ContactEmv_MagStripe_KeyEntry) {
                    if (paymentMethod is Credit credit) {
                        if (credit.FleetCard) {
                            authenticationMethod = CardHolderAuthenticationMethod.ManualSignatureVerification;
                        }
                    }
                }
            }
            
            // Swiped card data
            if (paymentMethod is ITrackData track) {
                // Outside, at the dispenser ("V")
                if (AcceptorConfig.CardDataInputCapability == CardDataInputCapability.ContactlessEmv_ContactEmv_MagStripe) {
                        authenticationMethod = CardHolderAuthenticationMethod.NotAuthenticated;
                }
                // Inside, at the counter ("W")
                else if (AcceptorConfig.CardDataInputCapability == CardDataInputCapability.ContactlessEmv_ContactEmv_MagStripe_KeyEntry) {
                    if (paymentMethodType.Equals(PaymentMethodType.Debit)) {
                        authenticationMethod = CardHolderAuthenticationMethod.PIN;
                    }
                    else if (paymentMethod is Credit credit) {
                        if (credit.FleetCard) {
                            authenticationMethod = CardHolderAuthenticationMethod.ManualSignatureVerification;
                        }
                        else {
                            authenticationMethod = CardHolderAuthenticationMethod.NotAuthenticated;
                        }
                    }
                }
            }
            
            if (paymentMethod is GiftCard giftCard) {
                if (!string.IsNullOrEmpty(giftCard.Pin)) {
                    authenticationMethod = CardHolderAuthenticationMethod.PIN;
                }
                else {
                    authenticationMethod = CardHolderAuthenticationMethod.NotAuthenticated;
                }
            }
            
            if (paymentMethod is IPinProtected pinProtected) {
                string pinBlock = pinProtected.PinBlock;
                if (!string.IsNullOrEmpty(pinBlock)) {
                    authenticationMethod = CardHolderAuthenticationMethod.PIN;
                }
                else {
                    authenticationMethod = CardHolderAuthenticationMethod.NotAuthenticated;
                }
            }

            return authenticationMethod;
        }

        private string CheckResponse<T>(string responseCode, NetworkMessage request, NetworkMessage response, T builder) where T : TransactionBuilder<Transaction> {
            List<string> successCodes = new List<string> {
                "000",
                "002",
                "400",
                "500",
                "501",
                "580"
            };

            decimal amount = response.GetAmount(DataElementId.DE_004);
            TransactionType transactionType = new TransactionType();
            PaymentMethodType paymentMethodType = new PaymentMethodType();
            if (builder != null) {
                transactionType = builder.TransactionType;
                if (builder.PaymentMethod != null) {
                    paymentMethodType = builder.PaymentMethod.PaymentMethodType;
                }
            }

            // report successes
            if (successCodes.Contains(responseCode)) {
                if (builder != null && request.IsDataCollect(paymentMethodType)) {
                    string encodedRequest = null;

                    // check if we need to build the implied data-collect
                    if (transactionType.Equals(TransactionType.Sale)) {
                        NetworkMessage impliedCapture = new NetworkMessage(Iso8583MessageType.CompleteMessage) {
                            MessageTypeIndicator = "1220"
                        };
                        impliedCapture.Set(DataElementId.DE_003, request.GetString(DataElementId.DE_003));
                        impliedCapture.Set(DataElementId.DE_004, request.GetString(DataElementId.DE_004));
                        impliedCapture.Set(DataElementId.DE_007, request.GetString(DataElementId.DE_007));
                        impliedCapture.Set(DataElementId.DE_011, request.GetString(DataElementId.DE_011));
                        impliedCapture.Set(DataElementId.DE_012, request.GetString(DataElementId.DE_012));
                        impliedCapture.Set(DataElementId.DE_017, request.GetString(DataElementId.DE_012).Substring(0, 4));
                        impliedCapture.Set(DataElementId.DE_018, request.GetString(DataElementId.DE_018));
                        impliedCapture.Set(DataElementId.DE_022, request.GetDataElement<DE22_PosDataCode>(DataElementId.DE_022));
                        impliedCapture.Set(DataElementId.DE_024, request.GetString(DataElementId.DE_024));
                        impliedCapture.Set(DataElementId.DE_030, request.GetString(DataElementId.DE_030));
                        impliedCapture.Set(DataElementId.DE_038, response.GetString(DataElementId.DE_038));
                        impliedCapture.Set(DataElementId.DE_041, request.GetString(DataElementId.DE_041));
                        impliedCapture.Set(DataElementId.DE_042, request.GetString(DataElementId.DE_042));
                        impliedCapture.Set(DataElementId.DE_043, request.GetString(DataElementId.DE_043));
                        impliedCapture.Set(DataElementId.DE_048, request.GetDataElement<DE48_MessageControl>(DataElementId.DE_048));

                        // DE_062 Card Issuer Data
                        DE62_CardIssuerData requestIssuerData = request.GetDataElement<DE62_CardIssuerData>(DataElementId.DE_062);
                        if (requestIssuerData == null) {
                            requestIssuerData = new DE62_CardIssuerData();
                        }

                        DE62_CardIssuerData responseIssuerData = response.GetDataElement<DE62_CardIssuerData>(DataElementId.DE_062);
                        if (responseIssuerData != null) {
                            string ntsData = responseIssuerData.Get(DE62_CardIssuerEntryTag.NTS_System);
                            if (ntsData != null) {
                                requestIssuerData.Add(DE62_CardIssuerEntryTag.NTS_System, ntsData);
                            }
                        }

                        // DE_002 / DE_014 - PAN / EXP DATE
                        if (request.Has(DataElementId.DE_002)) {
                            impliedCapture.Set(DataElementId.DE_002, request.GetString(DataElementId.DE_002));
                            impliedCapture.Set(DataElementId.DE_014, request.GetString(DataElementId.DE_014));
                            requestIssuerData.Add(DE62_CardIssuerEntryTag.SwipeIndicator, "NSI", "0");
                        }
                        else if (request.Has(DataElementId.DE_035)) {
                            CreditTrackData track = new CreditTrackData {
                                Value = request.GetString(DataElementId.DE_035)
                            };
                            impliedCapture.Set(DataElementId.DE_002, track.Pan);
                            impliedCapture.Set(DataElementId.DE_014, track.Expiry);
                            requestIssuerData.Add(DE62_CardIssuerEntryTag.SwipeIndicator, "NSI", "2");
                        }
                        else {
                            CreditTrackData track = new CreditTrackData {
                                Value = request.GetString(DataElementId.DE_045)
                            };
                            impliedCapture.Set(DataElementId.DE_002, track.Pan);
                            impliedCapture.Set(DataElementId.DE_014, track.Expiry);
                            requestIssuerData.Add(DE62_CardIssuerEntryTag.SwipeIndicator, "NSI", "1");
                        }
                        impliedCapture.Set(DataElementId.DE_062, requestIssuerData);

                        // DE_025 - MESSAGE REASON CODE
                        if (paymentMethodType != default(PaymentMethodType) && paymentMethodType.Equals(PaymentMethodType.Debit)) {
                            impliedCapture.Set(DataElementId.DE_025, "1379");
                        }
                        else {
                            impliedCapture.Set(DataElementId.DE_025, "1376");
                        }

                        // DE_056 - ORIGINAL TRANSACTION DATA
                        DE56_OriginalDataElements originalDataElements = new DE56_OriginalDataElements {
                            MessageTypeIdentifier = "1200",
                            SystemTraceAuditNumber = request.GetString(DataElementId.DE_011),
                            TransactionDateTime = request.GetString(DataElementId.DE_012)
                        };
                        impliedCapture.Set(DataElementId.DE_056, originalDataElements);

                        encodedRequest = EncodeRequest(impliedCapture);
                        if (BatchProvider != null) {
                            BatchProvider.ReportDataCollect(transactionType, paymentMethodType, amount, encodedRequest);
                        }
                    }
                    else if (!transactionType.Equals(TransactionType.DataCollect)) {
                        encodedRequest = EncodeRequest(request);
                        if (BatchProvider != null) {
                            BatchProvider.ReportDataCollect(transactionType, paymentMethodType, amount, encodedRequest);
                        }
                    }
                    return encodedRequest;
                }

                // if there's a batch provider handle the batch close stuff
                if ((responseCode.Equals("500") || responseCode.Equals("501")) && BatchProvider != null) {
                    BatchProvider.CloseBatch(responseCode.Equals("500"));
                }
                else if (responseCode.Equals("580")) {
                    if (BatchProvider != null) {
                        try {
                            List<string> encodedRequests = BatchProvider.GetEncodedRequests();
                            if (encodedRequests != null) {
                                ResentTransactions = new LinkedList<Transaction>();
                                foreach (string encRequest in encodedRequests) {
                                    try {
                                        NetworkMessage newRequest = DecodeRequest(encRequest);
                                        newRequest.MessageTypeIndicator = "1221";

                                        Transaction resend = SendRequest(newRequest, (TransactionBuilder<Transaction>)null, new byte[2], new byte[8]);
                                        ResentTransactions.AddLast(resend);
                                    }
                                    catch (ApiException) {
                                        /* NOM NOM */
                                        // TODO: this should be reported
                                    }
                                }

                                // resend the batch close
                                request.MessageTypeIndicator = "1521";
                                ResentBatch = SendRequest(request, builder, new byte[2], new byte[8]);
                            }
                        }
                        catch (ApiException) {
                            /* NOM NOM */
                            // TODO: this should be reported
                        }
                    }
                    return EncodeRequest(request);
                }
            }
            return null;
        }

        private string EncodeRequest(NetworkMessage request) {
            var encoded = Convert.ToBase64String(request.BuildMessage());
            if (RequestEncoder == null) {
                RequestEncoder = new PayrollEncoder(CompanyId, TerminalId);
            }
            return RequestEncoder.Encode(encoded);
        }

        private NetworkMessage DecodeRequest(string encodedStr) {
            if (RequestEncoder == null) {
                RequestEncoder = new PayrollEncoder(CompanyId, TerminalId);
            }

            string requestStr = RequestEncoder.Decode(encodedStr);

            byte[] decoded = Convert.FromBase64String(requestStr);
            MessageReader mr = new MessageReader(decoded);

            string mti = mr.ReadString(4);
            byte[] buffer = mr.ReadBytes(decoded.Length);
            NetworkMessage request = NetworkMessage.Parse(buffer, Iso8583MessageType.CompleteMessage);
            request.MessageTypeIndicator = mti;
            return request;
        }

        public Transaction ResubmitTransaction(ResubmitBuilder builder) {
            NetworkMessage request = this.DecodeRequest(builder.TransactionToken);
            switch (builder.TransactionType) {
                case TransactionType.DataCollect:
                    request.MessageTypeIndicator = "1221";
                    break;
                case TransactionType.BatchClose:
                    request.MessageTypeIndicator = "1521";
                    break;
                default:
                    throw new UnsupportedTransactionException("Only data collect or batch close transactions can be resubmitted");
            }

            return SendRequest(request, builder, new byte[2], new byte[8]);
        }

        private DE25_MessageReasonCode? MapMessageReasonCode(ManagementBuilder builder) {
            TransactionReference paymentMethod = (TransactionReference)builder.PaymentMethod;
            IPaymentMethod originalPaymentMethod = null;
            TransactionType transactionType = builder.TransactionType;

            // get the NTS data
            NtsData ntsData = null;
            if (paymentMethod != null) {
                ntsData = paymentMethod.NtsData;
                originalPaymentMethod = paymentMethod.OriginalPaymentMethod;
            }

            // set the fallback and authorizer codes
            FallbackCode? fallbackCode = null;
            AuthorizerCode? authorizerCode = null;
            if (ntsData != null) {
                fallbackCode = ntsData.FallbackCode;
                authorizerCode = ntsData.AuthorizerCode;
            }

            DE25_MessageReasonCode? reasonCode = null;
            if (transactionType.Equals(TransactionType.Capture)) {
                if (authorizerCode != null && authorizerCode.Equals(AuthorizerCode.Voice_Authorized)) {
                    reasonCode = DE25_MessageReasonCode.VoiceCapture;
                }
                else if (fallbackCode != null) {
                    switch (fallbackCode) {
                        case FallbackCode.Received_IssuerTimeout:
                        case FallbackCode.Received_IssuerUnavailable:
                        case FallbackCode.Received_SystemMalfunction:
                            reasonCode = DE25_MessageReasonCode.StandInCapture;
                            break;
                        default:
                            if (originalPaymentMethod is Debit || originalPaymentMethod is EBT) {
                                reasonCode = DE25_MessageReasonCode.PinDebit_EBT_Acknowledgement;
                            }
                            else reasonCode = DE25_MessageReasonCode.AuthCapture;
                            break;
                    }
                }
                else {                    
                    reasonCode = DE25_MessageReasonCode.AuthCapture;
                }
            }
            else if (transactionType.Equals(TransactionType.Void)) {
                bool partial = false;
                if (paymentMethod != null) {
                    partial = paymentMethod.PartialApproval;
                }

                if (fallbackCode != null) {
                    switch (fallbackCode) {
                        case FallbackCode.Received_IssuerTimeout:
                        case FallbackCode.CouldNotCommunicateWithHost:
                        case FallbackCode.Received_IssuerUnavailable:
                            reasonCode = DE25_MessageReasonCode.TimeoutWaitingForResponse_Reversal;
                            break;
                        case FallbackCode.Received_SystemMalfunction:
                            reasonCode = DE25_MessageReasonCode.SystemTimeout_Malfunction;
                            break;
                        default:
                            if (builder.CustomerInitiated) {
                                reasonCode = partial ? DE25_MessageReasonCode.CustomerInitiated_PartialApproval : DE25_MessageReasonCode.CustomerInitiatedVoid;
                            }
                            else if (builder.ForcedReversal) {
                                reasonCode = DE25_MessageReasonCode.FailureToDispense;
                            }
                            else {
                                reasonCode = DE25_MessageReasonCode.MerchantInitiatedVoid;
                            }
                            break;
                    }
                }
                else {
                    if (builder.CustomerInitiated) {
                        reasonCode = partial ? DE25_MessageReasonCode.CustomerInitiated_PartialApproval : DE25_MessageReasonCode.CustomerInitiatedVoid;
                    }
                    else if (builder.ForcedReversal) {
                        reasonCode = DE25_MessageReasonCode.FailureToDispense;
                    }
                    else {
                        reasonCode = DE25_MessageReasonCode.MerchantInitiatedVoid;
                    }
                }
            }
            else if (transactionType.Equals(TransactionType.Reversal)) {
                reasonCode = DE25_MessageReasonCode.TimeoutWaitingForResponse_Reversal;
            }
            else if (transactionType.Equals(TransactionType.Refund) && (originalPaymentMethod is Debit || originalPaymentMethod is EBT)) {
                reasonCode = DE25_MessageReasonCode.PinDebit_EBT_Acknowledgement;
            }

            return reasonCode;
        }

        private void VerifyFleetCardProductLimits(AuthorizationBuilder builder, Credit credit) {
            if (credit.CardType.Equals("VoyagerFleet")) {
                if (builder.ProductData.Count > 6) {
                    throw new BuilderException("Voyager Fleet cards do not allow more than 6 product codes per transaction.");
                }
            }
            else if (credit.CardType.Equals("MCFleet")) {
                if (builder.ProductData.Count > 6) {
                    throw new BuilderException("Mastercard Fleet cards do not allow more than 6 product codes per transaction.");
                }
            }
            else if (credit.CardType.Equals("VisaFleet")) {
                if (builder.ProductData.Count > 6) {
                    throw new BuilderException("Visa Fleet cards do not allow more than 6 product codes per transaction.");
                }
            }
            else if (credit.CardType.Equals("WexFleet")) {
                if (builder.ProductData.Count > 4) {
                    throw new BuilderException("Wex Fleet cards do not allow more than 4 product codes per transaction.");
                }
            }
            else if (credit.CardType.StartsWith("FleetCor")) {
                if (builder.ProductData.Count > 6) {
                    throw new BuilderException("FleetCor cards do not allow more than 6 product codes per transaction.");
                }
            }
        }

        private void VerifyReadyLinkAmountLimits(IPaymentMethod paymentMethod, decimal? amount) {
            // This is not currently working as expected and is breaking our debit tests

            decimal lowerLimit = 20m;
            decimal upperLimit = 750m;

            if (amount == null)
                return;

            if (paymentMethod is Credit credit) {
                if (credit.ReadyLinkCard) {
                    if (amount < lowerLimit) {
                        throw new BuilderException($"Visa ReadyLink transactions must be greater than or equal to the lower limit of { lowerLimit }");
                    }
                    if (amount > upperLimit) {
                        throw new BuilderException($"Visa ReadyLink transactions must be less than or equal to the upper limit of { upperLimit }");
                    }
                }
            }
            else if (paymentMethod is Debit debit) {
                if (debit.ReadyLinkCard) {
                    if (amount < lowerLimit) {
                        throw new BuilderException($"Visa ReadyLink transactions must be greater than or equal to the lower limit of { lowerLimit }");
                    }
                    if (amount > upperLimit) {
                        throw new BuilderException($"Visa ReadyLink transactions must be less than or equal to the upper limit of { upperLimit }");
                    }
                }
            }
        }
    }
}