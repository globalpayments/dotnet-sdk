using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Gateways {
    internal class TransitConnector : XmlGateway, IPaymentGateway, ISecure3dProvider {
        public AcceptorConfig AcceptorConfig { get; set; }
        public string DeviceId { get; set; }
        public string DeveloperId { get; set; }
        public string MerchantId { get; set; }
        public string TransactionKey { get; set; }
        public Secure3dVersion Version { get; set; }
        public bool SupportsHostedPayments => false;
        public bool SupportsOpenBanking => false;


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
            var request = new TransitRequestBuilder(MapTransactionType(builder))
                .Set("developerID", DeveloperId)
                .Set("deviceID", DeviceId)
                .Set("transactionKey", TransactionKey)
                .Set("transactionAmount", builder.Amount.ToCurrencyString(true))
                .Set("tokenRequired", builder.RequestMultiUseToken ? "Y" : "N")
                .Set("externalReferenceID", builder.ClientTransactionId);

            if (builder.RequestMultiUseToken) {
                request.Set("cardOnFile", builder.RequestMultiUseToken ? "Y" : "N");
            }

            var cardDataSource = MapCardDataSource(builder);
            request.Set("cardDataSource", cardDataSource);
            if (builder.PaymentMethod is ICardData card) {
                string cardNumber = card.Number;
                string cardDataInputMode = "ELECTRONIC_COMMERCE_NO_SECURITY_CHANNEL_ENCRYPTED_SET_WITHOUT_CARDHOLDER_CERTIFICATE";
                if (card.CardType.Equals("Amex") && !string.IsNullOrEmpty(card.Cvn)) {
                    cardDataInputMode = "MANUALLY_ENTERED_WITH_KEYED_CID_AMEX_JCB";
                } else if (AcceptorConfig.OperatingEnvironment == OperatingEnvironment.OnPremises_CardAcceptor_Attended) {
                    cardDataInputMode = "KEY_ENTERED_INPUT";
                }

                if (card is ITokenizable token && token.Token != null) {
                    cardNumber = token.Token;
                }

                if (builder.StoredCredential != null && builder.StoredCredential.Initiator == StoredCredentialInitiator.Merchant) {
                    cardDataInputMode = "MERCHANT_INITIATED_TRANSACTION_CARD_CREDENTIAL_STORED_ON_FILE";
                    request.Set("cardOnFileTransactionIdentifier", builder.StoredCredential.SchemeId);
                }

                var cardholderPresentDetail = card.CardPresent ? "CARDHOLDER_PRESENT" : "CARDHOLDER_NOT_PRESENT_ELECTRONIC_COMMERCE";
                if (cardDataSource == "MAIL") {
                    cardholderPresentDetail = "CARDHOLDER_NOT_PRESENT_MAIL_TRANSACTION";
                } else if (cardDataSource == "PHONE") {
                    cardholderPresentDetail = "CARDHOLDER_NOT_PRESENT_PHONE_TRANSACTION";
                }

                request.Set("cardNumber", cardNumber)
                    .Set("expirationDate", card.ShortExpiry)
                    .Set("cvv2", card.Cvn)
                    .Set("cardPresentDetail", card.CardPresent ? "CARD_PRESENT" : "CARD_NOT_PRESENT")
                    .Set("cardholderPresentDetail", cardholderPresentDetail)
                    .Set("cardDataInputMode", cardDataInputMode)
                    .Set("cardholderAuthenticationMethod", "NOT_AUTHENTICATED")
                    .Set("authorizationIndicator", builder.AmountEstimated ? "PREAUTH" : "FINAL");
            }
            else if (builder.PaymentMethod is ITrackData track) {
                request.Set(track.TrackNumber.Equals(TrackNumber.TrackTwo) ? "track2Data" : "track1Data", track.TrackData);
                request.Set("cardPresentDetail", "CARD_PRESENT")
                    .Set("cardholderPresentDetail", "CARDHOLDER_PRESENT")
                    .Set("cardDataInputMode", "MAGNETIC_STRIPE_READER_INPUT")
                    .Set("cardholderAuthenticationMethod", "NOT_AUTHENTICATED");

                if (builder.HasEmvFallbackData) {
                    request.Set("emvFallbackCondition", EnumConverter.GetMapping(Target.Transit, builder.EmvFallbackCondition))
                        .Set("lastChipRead", EnumConverter.GetMapping(Target.Transit, builder.EmvLastChipRead))
                        .Set("paymentAppVersion", builder.PaymentApplicationVersion ?? "unspecified");
                }
            }

            // AVS
            if (builder.BillingAddress != null) {
                request.Set("addressLine1", builder.BillingAddress.StreetAddress1)
                    .Set("zip", builder.BillingAddress.PostalCode);
            }

            // PIN Debit
            if (builder.PaymentMethod is IPinProtected pinProtected && !string.IsNullOrEmpty(pinProtected.PinBlock)) {
                request.Set("pin", pinProtected.PinBlock.Substring(0, 16))
                    .Set("pinKsn", pinProtected.PinBlock.Substring(16));
            }

            if (builder.PaymentMethod is Credit pm && pm.CardType.Equals("Discover") && (cardDataSource.Equals("INTERNET"))) {
                request.Set("registeredUserIndicator", builder.LastRegisteredDate != default(DateTime) ? "YES" : "NO");
                request.Set("lastRegisteredChangeDate", builder.LastRegisteredDate != default(DateTime) ? builder.LastRegisteredDate.ToString("MM/dd/yyyy") : "00/00/0000");
            }

            if (builder.Gratuity != null) {
                request.Set("tip", builder.Gratuity.ToCurrencyString());
            }

            #region 3DS 1/2
            if (builder.PaymentMethod is ISecure3d secure && secure.ThreeDSecure != null) {
                if (secure.ThreeDSecure.Version.Equals(Secure3dVersion.One)) {
                    request.Set("programProtocol", "1");
                }
                else {
                    request.Set("programProtocol", "2")
                        .Set("directoryServerTransactionID", secure.ThreeDSecure.DirectoryServerTransactionId);
                }

                request.Set("eciIndicator", secure.ThreeDSecure.Eci)
                    .Set("secureCode", secure.ThreeDSecure.SecureCode)
                    .Set("digitalPaymentCryptogram", secure.ThreeDSecure.AuthenticationValue)
                    .Set("securityProtocol", secure.ThreeDSecure.AuthenticationType)
                    .Set("ucafCollectionIndicator", EnumConverter.GetMapping(Target.Transit, secure.ThreeDSecure.UCAFIndicator));

            }
            #endregion

            #region Commercial Card Requests
            if (builder.CommercialData != null) {
                var cd = builder.CommercialData;

                if (cd.CommercialIndicator.Equals(CommercialIndicator.Level_II)) {
                    request.Set("commercialCardLevel", "LEVEL2");
                }
                else {
                    request.Set("commercialCardLevel", "LEVEL3");
                    request.SetProductDetails(cd.LineItems);
                }

                request.Set("salesTax", cd.TaxAmount.ToCurrencyString())
                    .Set("chargeDescriptor", cd.Description)
                    .Set("customerRefID", cd.CustomerReferenceId)
                    .Set("purchaseOrder", cd.PoNumber)
                    .Set("shipToZip", cd.DestinationPostalCode)
                    .Set("shipFromZip", cd.OriginPostalCode)
                    .Set("supplierReferenceNumber", cd.SupplierReferenceNumber)
                    .Set("customerVATNumber", cd.CustomerVAT_Number)
                    .Set("summaryCommodityCode", cd.SummaryCommodityCode)
                    .Set("shippingCharges", cd.FreightAmount.ToCurrencyString())
                    .Set("dutyCharges", cd.DutyAmount.ToCurrencyString())
                    .Set("destinationCountryCode", cd.DestinationCountryCode)
                    .Set("vatInvoice", cd.VAT_InvoiceNumber)
                    .Set("orderDate", cd.OrderDate?.ToString("dd/MM/yyyy"))
                    .SetAdditionalTaxDetails(cd.AdditionalTaxDetails);
            }
            #endregion

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
            var request = new TransitRequestBuilder(MapTransactionType(builder))
                .Set("developerID", DeveloperId)
                .Set("deviceID", DeviceId)
                .Set("transactionKey", TransactionKey)
                .Set("transactionAmount", builder.Amount.ToCurrencyString(true))
                .Set("tip", builder.Gratuity.ToCurrencyString())
                .Set("transactionID", builder.TransactionId)
                .Set("isPartialShipment", builder.MultiCapture ? "Y" : null)
                .SetPartialShipmentData(builder.MultiCaptureSequence, builder.MultiCapturePaymentCount)
                .Set("externalReferenceID", builder.ClientTransactionId)
                .Set("voidReason", EnumConverter.GetMapping(Target.Transit, builder.VoidReason));

            string response = DoTransaction(request.BuildRequest(builder));
            return MapResponse(builder, response);
        }

        public string SerializeRequest(AuthorizationBuilder builder) {
            throw new UnsupportedTransactionException();
        }

        public Transaction ProcessSecure3d(Secure3dBuilder builder) {
            throw new NotImplementedException();
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
                Token = root.GetValue<string>("token"),
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
                CardBrandTransactionId = root.GetValue<string>("cardTransactionIdentifier"),
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

            // batch response
            if (root.Has("batchInfo")) {
                Element batchInfo = root.Get("batchInfo");

                trans.BatchSummary = new BatchSummary {
                    ResponseCode = responseCode,
                    SicCode = batchInfo.GetValue<string>("SICCODE"),
                    SaleCount = batchInfo.GetValue<int>("saleCount"),
                    SaleAmount = batchInfo.GetValue<decimal>("saleAmount"),
                    ReturnCount = batchInfo.GetValue<int>("returnCount"),
                    ReturnAmount = batchInfo.GetValue<decimal>("returnAmount")
                };
            }

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
                case TransactionType.BatchClose:
                    return "BatchClose";
                case TransactionType.Reversal:
                case TransactionType.Void:
                    return "Void";
                case TransactionType.Verify:
                    return "CardAuthentication";
                case TransactionType.Tokenize:
                    return "GetOnusToken";
                case TransactionType.Refund:
                    return "Return";
                default:
                    throw new UnsupportedTransactionException();
            }
        }

        private string MapCardDataSource(AuthorizationBuilder builder) {
            IPaymentMethod paymentMethod = builder.PaymentMethod;
            EcommerceInfo ecommerceInfo = builder.EcommerceInfo;

            if (paymentMethod is ICardData card) {
                if (card.ReaderPresent && card.CardPresent) {
                    return "MANUAL";
                }

                if (ecommerceInfo == null) {
                    return "INTERNET";
                }

                switch (ecommerceInfo.Channel) {
                    case EcommerceChannel.ECOM:
                        return "INTERNET";
                    case EcommerceChannel.MOTO:
                        return "PHONE|MAIL";
                    case EcommerceChannel.MAIL:
                        return "MAIL";
                    case EcommerceChannel.PHONE:
                        return "PHONE";
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

        public string CreateManifest()
        {
            var dateFormatString = DateTime.Now.ToString("MMddyyyy");

            var plainText = StringUtils.PadRight(MerchantId, 20, ' ') +
                            StringUtils.PadRight(DeviceId, 24, ' ') +
                            "000000000000" +
                            StringUtils.PadRight(dateFormatString, 8, ' ');

            var tempTransactionKey = TransactionKey.Substring(0, 16);

            var byteArray = Encoding.UTF8.GetBytes(tempTransactionKey);
            var encrypted = new byte[0];

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Padding = PaddingMode.None;
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.BlockSize = 128;
                aesAlg.KeySize = 128;
                aesAlg.Key = byteArray;
                aesAlg.IV = byteArray;
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            var encryptedData = BitConverter.ToString(encrypted).ToLower().Replace("-", string.Empty);

            var hashKey = HashHmac(TransactionKey, TransactionKey);

            return hashKey.Substring(0, 4) + encryptedData + hashKey.Substring(hashKey.Length - 4, 4); ;
        }

        private string HashHmac(string message, string secret)
        {
            Encoding encoding = Encoding.UTF8;
            using (HMACMD5 hmac = new HMACMD5(encoding.GetBytes(secret)))
            {
                var msg = encoding.GetBytes(message);
                var hash = hmac.ComputeHash(msg);
                return BitConverter.ToString(hash).ToLower().Replace("-", string.Empty);
            }
        }
    }
}
