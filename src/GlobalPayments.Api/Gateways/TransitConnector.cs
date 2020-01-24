using System;
using System.Collections.Generic;
using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Gateways {
    internal class TransitRequest {
        private string _root;
        private Dictionary<string, string> _values;

        private string this[string key] {
            get {
                if (_values.ContainsKey(key)) {
                    return _values[key];
                }
                return null;
            }
        }

        public TransitRequest(string root) {
            _root = root;
            _values = new Dictionary<string, string>();
        }

        public TransitRequest Set(string key, string value) {
            if (value != null) {
                _values.Add(key, value);
            }
            return this;
        }

        public string BuildRequest<T>(T builder) where T : TransactionBuilder<Transaction> {
            var et = new ElementTree();

            Element transaction = et.Element(_root);
            foreach (var element in BuildRequestMap(builder)) {
                et.SubElement(transaction, element, this[element]);
            }

            return et.ToString(transaction);
        }

        private LinkedList<string> BuildRequestMap<T>(T builder) where T : TransactionBuilder<Transaction> {
            IPaymentMethod paymentMethod = builder.PaymentMethod;

            switch (builder.TransactionType) {
                case TransactionType.Auth:
                case TransactionType.Sale: {
                        if (paymentMethod is Debit) {
                            return BuildList("deviceID|transactionKey|manifest|cardDataSource|transactionAmount|tip|salesTax|taxType|taxAmount|taxRate|currencyCode|track2Data|track3Data|emulatedTrackData|emvTags|emvFallbackCondition|lastChipRead|paymentAppVersion|emcContactlessToContactChip|pin|pinKsn|secureCode|paymentAccountReference|panReferenceIdentifier|nfcTags|ksn|transactionMID|externalReferenceID|operatorID|orderNumber|cardOnFile|merchantReportID|encryptionType|tokenRequired|healthCareAccountType|prescriptionAmount|visionAmount|dentalAmount|clinicAmount|isQualifiedIIAS|rxNumber|couponID|providerID|providerToken|locationID|notifyEmailID|customerCode|firstName|lastName|transTotalDiscountAmount|transDiscountName|transDiscountAmount|transDiscountPercentage|priority|stackable|productCode|productName|price|quantity|measurementUnit|productDiscountName|productDiscountAmount|productDiscountPercentage|productDiscountType|priority|stackable|productTaxName|productTaxAmount|productTaxPercentage|productTaxType|productVariation|modifierName|modifierValue|modifierPrice|productNotes|softDescriptor|developerID|registeredUserIndicator|lastRegisteredChangeDate|laneID|authorizationIndicator|terminalCapability|terminalOperatingEnvironment|cardholderAuthenticationMethod|terminalAuthenticationCapability|terminalOutputCapability|maxPinLength|terminalCardCaptureCapability|cardholderPresentDetail|cardPresentDetail|cardDataInputMode|cardholderAuthenticationEntity|cardDataOutputCapability|splitTenderPayment|splitTenderID|splitTenderConsolidatedReceipt|noIndividualTransactionReceipt");
                        }
                        return BuildList("deviceID|transactionKey|manifest|cardDataSource|transactionAmount|tip|salesTax|taxType|taxAmount|taxRate|taxCategory|shippingCharges|dutyCharges|surcharge|additionalAmountType|additionalAmount|additionalAmountSign|currencyCode|cardNumber|expirationDate|cvv2|track1Data|track2Data|track3Data|emulatedTrackData|cardHolderName|secureCode|securityProtocol|ucafCollectionIndicator|paymentAccountReference|panReferenceIdentifier|eciIndicator|cardOnFileTransactionIdentifier|emvTags|pin|pinKsn|emvFallbackCondition|lastChipRead|paymentAppVersion|emvContactlessToContactChip|nfcTags|walletSource|checkOutID|addressLine1|zip|transactionMID|externalReferenceID|operatorID|orderNumber|cardOnFile|merchantReportID|encryptionType|ksn|tokenRequired|healthCareAccountType|prescriptionAmount|visionAmount|dentalAmount|clinicAmount|isQualifiedIIAS|rxNumber|couponID|providerID|providerToken|locationID|notifyEmailID|orderID|customerCode|firstName|lastName|customerPhone|transTotalDiscountAmount|transDiscountName|transDiscountAmount|transDiscountPercentage|priority|stackable|productCode|productName|price|quantity|measurementUnit|productDiscountName|productDiscountAmount|productDicsountPercentage|productDiscountType|priority|stackable|productTaxName|productTaxAmount|productTaxPercentage|productTaxType|productVariation|modifierName|modifierValue|modifierPrice|productNotes|productDiscountIndicator|productCommodityCode|alternateTaxID|creditIndicator|orderNotes|orderServiceTimestamp|commercialCardLevel|purchaseOrder|chargeDescriptor|customerVATNumber|customerRefID|orderDate|summaryCommodityCode|vatInvoice|chargeDescriptor2|chargeDescriptor3|chargeDescriptor4|supplierReferenceNumber|shipFromZip|shipToZip|destinationCountryCode|orderID|tokenRequesterID|softDescriptor|terminalCapability|terminalOperatingEnvironment|cardholderAuthenticationMethod|terminalAuthenticationCapability|terminalOutputCapability|maxPinLength|terminalCardCaptureCapability|cardholderPresentDetail|cardPresentDetail|cardDataInputMode|cardholderAuthenticationEntity|cardDataOutputCapability|mPosAcceptanceDeviceType|developerID|paymentFacilitatorIdentifier|paymentFacilitatorName|subMerchantIdentifier|subMerchantName|subMerchantCountryCode|subMerchantStateCode|subMerchantCity|subMerchantPostalCode|subMerchantEmailId|subMerchantPhone|isoIdentifier|isRecurring|billingType|paymentCount|currentPaymentCount|isoIdentifier|registeredUserIndicator|lastRegisteredChangeDate|laneID|authorizationIndicator|splitTenderPayment|splitTenderID|splitTenderConsolidatedReceipt|noIndividualTransactionReceipt");
                    }
                case TransactionType.Balance:
                        return BuildList("deviceID|transactionKey|manifest|cardDataSource|currencyCode|track1Data|track2Data|track3Data|emulatedTrackData|cardNumber|expirationDate|cvv2|cardHolderName|secureCode|securityProtocol|ucafCollectionIndicator|paymentAccountReference|panReferenceIdentifier|eciIndicator|cardOnFileTransactionIdentifier|nfcTags|walletSource|checkOutID|dtvv|addressLine1|zip|transactionMID|externalReferenceID|operatorID|orderNumber|cardOnFile|encryptionType|ksn|tokenRequired|customerCode|firstName|lastName|customerPhone|tokenRequesterID|softDescriptor|developerID|laneID|terminalCapability|terminalOperatingEnvironment|cardholderAuthenticationMethod|terminalAuthenticationCapability|terminalOutputCapability|maxPinLength|terminalCardCaptureCapability|cardholderPresentDetail|cardPresentDetail|cardDataInputMode|cardholderAuthenticationEntity|cardDataOutputCapability|mPosAcceptanceDeviceType");
                case TransactionType.Capture:
                        return BuildList("deviceID|transactionKey|manifest|transactionAmount|tip|salesTax|taxType|taxAmount|taxRate|taxCategory|shippingCharges|dutyCharges|surcharge|additionalAmountType|additionalAmount|additionalAmountSign|transactionID|externalReferenceID|operatorID|isPartialShipment|currentPaymentSequenceNumber|totalPaymentCount|softDescriptor|merchantReportID|customerCode|firstName|lastName|transTotalDiscountAmount|transDiscountName|transDiscountAmount|transDiscountPercentage|priority|stackable|productCode|productName|price|quantity|measurementUnit|productDiscountName|productDiscountAmount|productDicsountPercentage|productDiscountType|priority|stackable|productTaxName|productTaxAmount|productTaxPercentage|productTaxType|productVariation|modifierName|modifierValue|modiferPrice|productNotes|productDiscountIndicator|productCommodityCode|alternateTaxID|creditIndicator|orderNotes|orderServiceTimestamp|commercialCardLevel|purchaseOrder|chargeDescriptor|customerVATNumber|customerRefID|orderDate|summaryCommodityCode|vatInvoice|chargeDescriptor2|chargeDescriptor3|chargeDescriptor4|supplierReferenceNumber|shipFromZip|shipToZip|destinationCountryCode|developerID|paymentFacilitatorIdentifier|paymentFacilitatorName|subMerchantIdentifier|subMerchantName|subMerchantCountryCode|subMerchantStateCode|subMerchantCity|subMerchantPostalCode|subMerchantEmailId|subMerchantPhone");
                case TransactionType.Verify: {
                        /* This is the list for the zero dollar authorization */
                        return BuildList("deviceID|transactionKey|manifest|cardDataSource|currencyCode|track1Data|track2Data|track3Data|emulatedTrackData|cardNumber|expirationDate|cvv2|cardHolderName|secureCode|securityProtocol|ucafCollectionIndicator|paymentAccountReference|panReferenceIdentifier|eciIndicator|nfcTags|walletSource|checkOutID|addressLine1|zip|transactionMID|externalReferenceID|operatorID|orderNumber|cardOnFile|merchantReportID|encryptionType|ksn|tokenRequired|customerCode|firstName|lastName|tokenRequesterID|softDescriptor|developerID|laneID|terminalCapability|terminalOperatingEnvironment|cardholderAuthenticationMethod|terminalAuthenticationCapability|terminalOutputCapability|maxPinLength|terminalCardCaptureCapability|cardholderPresentDetail|cardPresentDetail|cardDataInputMode|cardholderAuthenticationEntity|cardDataOutputCapability|mPosAcceptanceDeviceType");

                        /* This is the list for the card AVS and CVV check */
                        //return BuildList("deviceID|transactionKey|manifest|cardDataSource|emulatedTrackData|cardNumber|expirationDate|cvv2|walletSource|checkOutID|cardHolderName|secureCode|securityProtocol|ucafCollectionIndicator|paymentAccountReference|panReferenceIdentifier|eciIndicator|nfcTags|addressLine1|zip|externalReferenceID|operatorID|orderNumber|cardOnFile|merchantReportID|encryptionType|ksn|tokenRequired|customerCode|firstName|lastName|tokenRequesterID|softDescriptor|developerID|laneID|terminalCapability|terminalOperatingEnvironment|cardholderAuthenticationMethod|terminalAuthenticationCapability|terminalOutputCapability|maxPinLength|terminalCardCaptureCapability|cardholderPresentDetail|cardPresentDetail|cardDataInputMode|cardholderAuthenticationEntity|cardDataOutputCapability|mPosAcceptanceDeviceType");
                    }
                default: throw new UnsupportedTransactionException();
            }
        }

        private LinkedList<string> BuildList(string str) {
            var list = new LinkedList<string>();

            string[] values = str.Split('|');
            foreach (string value in values) {
                if (!string.IsNullOrEmpty(value)) {
                    list.AddLast(value);
                }
            }

            return list;
        }
    }

    internal class TransitConnector : XmlGateway, IPaymentGateway {
        public AcceptorConfig AcceptorConfig { get; set; }
        public string DeviceId { get; set; }
        public string DeveloperId { get; set; }
        public string MerchantId { get; set; }
        public string TransactionKey { get; set; }

        public bool SupportsHostedPayments { get { return false; } }

        public TransitConnector() {
        }

        public string GenerateKey(string userId, string password) {
            var et = new ElementTree();

            var root = et.Element("GenerateKey");
            et.SubElement(root, "mid").Text(MerchantId);
            et.SubElement(root, "userID").Text(userId);
            et.SubElement(root, "password").Text(password);
            et.SubElement(root, "transactionKey", TransactionKey);

            string rawResponse = DoTransaction(et.ToString(root));

            var response = ElementTree.Parse(rawResponse).Get("GenerateKeyResponse");
            if (response.GetValue<string>("status").Equals("PASS")) {
                TransactionKey = response.GetValue<string>("transactionKey");
                return TransactionKey;
            }
            else {
                string responseCode = response.GetValue<string>("responseCode");
                string responseMessage = response.GetValue<string>("responseMessage");
                throw new GatewayException("Failed to generate transaction key for the given credentials", responseCode, responseMessage);
            }
        }
        private string GenerateManifest(decimal amount, string timestamp) {
            throw new NotImplementedException();
        }

        public Transaction ProcessAuthorization(AuthorizationBuilder builder) {
            var request = new TransitRequest(MapTransactionType(builder))
                .Set("developerID", DeveloperId)
                .Set("deviceID", DeviceId)
                .Set("transactionKey", TransactionKey)
                .Set("transactionAmount", builder.Amount.ToCurrencyString());

            request.Set("cardDataSource", MapCardDataSource(builder));
            if (builder.PaymentMethod is ICardData card) {
                request.Set("cardNumber", card.Number)
                    .Set("expirationDate", card.ShortExpiry)
                    .Set("cvv2", card.Cvn)
                    .Set("cardPresentDetail", card.CardPresent ? "CARD_PRESENT" : "CARD_NOT_PRESENT")
                    .Set("cardholderPresentDetail", card.CardPresent ? "CARDHOLDER_PRESENT" : "CARDHOLDER_NOT_PRESENT_ELECTRONIC_COMMERCE")
                    .Set("cardDataInputMode", card.CardPresent ? "KEY_ENTERED_INPUT" : "PAN_ENTRY_ELECTRONIC_COMMERCE_INCLUDING_REMOTE_CHIP")
                    .Set("cardholderAuthenticationMethod", "UNKNOWN");
            }
            else if (builder.PaymentMethod is ITrackData track) {
                request.Set(track.TrackNumber.Equals(TrackNumber.TrackTwo) ? "track2Data" : "track1Data", track.TrackData);
                request.Set("cardPresentDetail", "CARD_PRESENT")
                    .Set("cardholderPresentDetail", "CARDHOLDER_PRESENT")
                    .Set("cardDataInputMode", "MAGNETIC_STRIPE_READER_INPUT")
                    .Set("cardholderAuthenticationMethod", "UNKNOWN");

                if (builder.HasEmvFallbackData) {
                    request.Set("emvFallbackCondition", EnumConverter.GetMapping(Target.Transit, builder.EmvFallbackCondition))
                        .Set("lastChipRead", EnumConverter.GetMapping(Target.Transit, builder.EmvLastChipRead))
                        .Set("paymentAppVersion", builder.PaymentApplicationVersion ?? "unspecified");
                }
            }

            // PIN Debit
            if (builder.PaymentMethod is IPinProtected pinProtected) {
                request.Set("pin", pinProtected.PinBlock.Substring(0, 16))
                    .Set("pinKsn", pinProtected.PinBlock.Substring(16));
            }

            // Acceptor Config
            request.Set("terminalCapability", EnumConverter.GetMapping(Target.Transit, AcceptorConfig.CardDataInputCapability))
                .Set("terminalCardCaptureCapability", AcceptorConfig.CardCaptureCapability ? "CARD_CAPTURE_CAPABILITY" : "NO_CAPABILITY")
                .Set("terminalOperatingEnvironment", EnumConverter.GetMapping(Target.Transit, AcceptorConfig.OperatingEnvironment))
                .Set("cardholderAuthenticationEntity", EnumConverter.GetMapping(Target.Transit, AcceptorConfig.CardHolderAuthenticationEntity))
                .Set("cardDataOutputCapability", EnumConverter.GetMapping(Target.Transit, AcceptorConfig.CardDataOutputCapability))
                .Set("terminalAuthenticationCapability", EnumConverter.GetMapping(Target.Transit, AcceptorConfig.CardHolderAuthenticationCapability))
                .Set("terminalOutputCapability", EnumConverter.GetMapping(Target.Transit, AcceptorConfig.TerminalOutputCapability))
                .Set("maxPinLength", EnumConverter.GetMapping(Target.Transit, AcceptorConfig.PinCaptureCapability));

            string response = DoTransaction(request.BuildRequest(builder));
            return MapResponse(builder, response);
        }

        public Transaction ManageTransaction(ManagementBuilder builder) {
            var request = new TransitRequest(MapTransactionType(builder))
                .Set("developerID", DeveloperId)
                .Set("deviceID", DeviceId)
                .Set("transactionKey", TransactionKey)
                .Set("transactionAmount", builder.Amount.ToCurrencyString())
                .Set("tip", builder.Gratuity.ToCurrencyString())
                .Set("transactionID", builder.TransactionId)
                .Set("isPartialShipment", builder.MultiCapture ? "Y" : null);

            string response = DoTransaction(request.BuildRequest(builder));
            return MapResponse(builder, response);
        }

        public string SerializeRequest(AuthorizationBuilder builder) {
            throw new UnsupportedTransactionException();
        }

        private Transaction MapResponse<T>(T builder, string rawResponse) where T : TransactionBuilder<Transaction> {
            string rootName = "{0}Response".FormatWith(MapTransactionType(builder));

            var root = ElementTree.Parse(rawResponse).Get(rootName);
            string status = root.GetValue<string>("status");
            string responseCode = NormalizeResponse(root.GetValue<string>("responseCode"));
            string responseMessage = root.GetValue<string>("responseMessage");

            if (!status.Equals("PASS")) {
                throw new GatewayException(
                    "Unexpected Gateway Response: {0} - {1}".FormatWith(responseCode, responseMessage),
                    responseCode,
                    responseMessage
                );
            }

            Transaction trans = new Transaction {
                ResponseCode = responseCode,
                ResponseMessage = responseMessage,
                AuthorizationCode = root.GetValue<string>("authCode"),
                // hostResponseCode
                // hostReferenceNumber,
                // taskID,
                TransactionId = root.GetValue<string>("transactionID"),
                Timestamp = root.GetValue<string>("transactionTimestamp"),
                // transactionAmount
                AuthorizedAmount = root.GetValue<decimal>("processedAmount"),
                // totalAmount
                // tip
                // salesTax
                // orderNumber
                // externalReferenceID
                AvsResponseCode = root.GetValue<string>("addressVerificationCode"),
                CvnResponseCode = root.GetValue<string>("cvvVerificationCode"),
                // cardHolderVerificationCode
                CardType = root.GetValue<string>("cardType"),
                CardLast4 = root.GetValue<string>("maskedCardNumber"),
                // token
                // expirationDate
                // accountUpdaterResponseCode
                CommercialIndicator = root.GetValue<string>("commercialCard"),
                // cavvResponseCode
                // ucafCollectionIndicator
                // paymentAccountReference
                // panReferenceIdentifier
                BalanceAmount = root.GetValue<decimal>("balanceAmount"),
                // fcsID
                // transactionIntegrityClassification
                // aci
                // cardTransactionIdentifier
                // discountAmount
                // discountType
                // discountValue
                // firstName
                // lastName
                // prescriptionAmount
                // visionAmount
                // dentalAmount
                // clinicAmount
                // customerReceipt
                // merchantReceipt
                // consolidatedCustomerReceipt
                // consolidatedMerchantReceipt
                // pan
                // panExpirationDate
                // tokenAssuranceLevel
                // maskedPAN
                // tokenAccRangeStatus
                // splitTenderID
                // additionalAmountAndAccountType
            };

            return trans;
        }

        private string MapTransactionType<T>(T builder) where T : TransactionBuilder<Transaction> {
            switch (builder.TransactionType) {
                case TransactionType.Auth:
                case TransactionType.Capture:
                    return builder.TransactionType.ToString();
                case TransactionType.Sale: {
                        if (builder.PaymentMethod is Debit) {
                            return "DebitSale";
                        }
                        return builder.TransactionType.ToString();
                    }
                case TransactionType.Balance:
                    return "BalanceInquiry";
                case TransactionType.Verify:
                    return "CardAuthentication";
                default:
                    throw new UnsupportedTransactionException();
            }
        }
        private string MapCardDataSource(AuthorizationBuilder builder) {
            IPaymentMethod paymentMethod = builder.PaymentMethod;

            if (paymentMethod is ICardData card) {
                if (card.ReaderPresent) {
                    return card.CardPresent ? "MANUAL" : "PHONE|MAIL";
                }
                else {
                    return card.CardPresent ? "MANUAL" : "INTERNET";
                }
            }
            else if (paymentMethod is ITrackData track) {
                if (builder.TagData != null) {
                    return track.EntryMethod.Equals(EntryMethod.Swipe) ? "EMV" : "EMV_CONTACTLESS";
                }
                else if (builder.HasEmvFallbackData) {
                    return "FALLBACK_SWIPE";
                }
                return "SWIPE";
            }

            throw new UnsupportedTransactionException();
        }

        private string NormalizeResponse(string input) {
            if (input.Equals("A0000") | input.Equals("A0014") | input.Equals("A3200")) {
                return "00";
            }
            else if (input.Equals("A0002") | input.Equals("A3207")) {
                return "10";
            }
            else return input;
        }
    }
}
