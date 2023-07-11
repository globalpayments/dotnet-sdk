using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Utils;
using System;

namespace GlobalPayments.Api.Gateways {
    internal class GeniusConnector : XmlGateway, IPaymentGateway {
        public string MerchantName { get; set; }
        public string MerchantSiteId { get; set; }
        public string MerchantKey { get; set; }
        public string RegisterNumber { get; set; }
        public string TerminalId { get; set; }
        public bool SupportsHostedPayments => false;
        public bool SupportsOpenBanking => false;


        public GeniusConnector() : base() {
        }


        public Transaction ProcessAuthorization(AuthorizationBuilder builder) {
            ElementTree et = new ElementTree();
            IPaymentMethod paymentMethod = builder.PaymentMethod;

            var transaction = et.Element(MapRequestType(builder))
                .Set("xmlns", "http://schemas.merchantwarehouse.com/merchantware/v45/");

            // Credentials
            var credentials = et.SubElement(transaction, "Credentials");
            et.SubElement(credentials, "MerchantName").Text(MerchantName);
            et.SubElement(credentials, "MerchantSiteId").Text(MerchantSiteId);
            et.SubElement(credentials, "MerchantKey").Text(MerchantKey);

            // Payment Data
            var paymentData = et.SubElement(transaction, "PaymentData");
            if (paymentMethod is CreditCardData) {
                CreditCardData card = paymentMethod as CreditCardData;
                if (card.Token != null) {
                    if (card.MobileType != null) {
                        et.SubElement(paymentData, "Source").Text("Wallet");
                        et.SubElement(paymentData, "WalletId", MapWalletId(card.MobileType));
                        et.SubElement(paymentData, "EncryptedPaymentData", card.Token);
                    }
                    else {
                        et.SubElement(paymentData, "Source").Text("Vault");
                        et.SubElement(paymentData, "VaultToken", card.Token);
                    }
                }
                else {
                    et.SubElement(paymentData, "Source").Text("Keyed");
                    et.SubElement(paymentData, "CardNumber", card.Number);
                    et.SubElement(paymentData, "ExpirationDate", card.ShortExpiry);
                    et.SubElement(paymentData, "CardHolder", card.CardHolderName);
                    et.SubElement(paymentData, "CardVerificationValue", card.Cvn);
                }
            }
            else if (paymentMethod is CreditTrackData) {
                et.SubElement(paymentData, "Source").Text("READER");

                CreditTrackData track = paymentMethod as CreditTrackData;
                et.SubElement(paymentData, "TrackData", track.Value);
            }

            // AVS
            et.SubElement(paymentData, "AvsStreetAddress", builder.BillingAddress?.StreetAddress1);
            et.SubElement(paymentData, "AvsZipCode", builder.BillingAddress?.PostalCode);

            // Request
            var request = et.SubElement(transaction, "Request");
            et.SubElement(request, "Amount", builder.Amount.ToCurrencyString());
            et.SubElement(request, "CashbackAmount", builder.CashBackAmount.ToCurrencyString());
            et.SubElement(request, "SurchargeAmount", builder.ConvenienceAmount.ToCurrencyString());
            //et.SubElement(request, "TaxAmount", builder..ToCurrencyString());
            et.SubElement(request, "AuthorizationCode", builder.OfflineAuthCode);

            if (builder.AutoSubstantiation != null) {
                var healthcare = et.SubElement(request, "HealthCareAmountDetails");

                AutoSubstantiation auto = builder.AutoSubstantiation;
                et.SubElement(healthcare, "CopayAmount", auto.CopaySubTotal.ToCurrencyString());
                et.SubElement(healthcare, "ClinicalAmount", auto.ClinicSubTotal.ToCurrencyString());
                et.SubElement(healthcare, "DentalAmount", auto.DentalSubTotal.ToCurrencyString());
                et.SubElement(healthcare, "HealthCareTotalAmount", auto.TotalHealthcareAmount.ToCurrencyString());
                et.SubElement(healthcare, "PrescriptionAmount", auto.PrescriptionSubTotal.ToCurrencyString());
                et.SubElement(healthcare, "VisionAmount", auto.VisionSubTotal.ToCurrencyString());
            }

            et.SubElement(request, "InvoiceNumber", builder.InvoiceNumber);
            et.SubElement(request, "RegisterNumber", RegisterNumber);
            et.SubElement(request, "MerchantTransactionId", builder.ClientTransactionId);
            et.SubElement(request, "CardAcceptorTerminalId", TerminalId);
            et.SubElement(request, "EnablePartialAuthorization", builder.AllowPartialAuth);
            et.SubElement(request, "ForceDuplicate", builder.AllowDuplicates);

            // Level III
            if(builder.CommercialData != null) {
                var invoice = et.SubElement(request, "Invoice");

                CommercialData cd = builder.CommercialData;
                et.SubElement(invoice, "TaxIndicator", MapTaxType(cd.TaxType));
                et.SubElement(invoice, "ProductDescription", cd.Description);
                et.SubElement(invoice, "DiscountAmount", cd.DiscountAmount);
                et.SubElement(invoice, "ShippingAmount", cd.FreightAmount);
                et.SubElement(invoice, "DutyAmount", cd.DutyAmount);
                et.SubElement(invoice, "DestinationPostalCode", cd.DestinationPostalCode);
                et.SubElement(invoice, "DestinationCountryCode", cd.DestinationCountryCode);
                et.SubElement(invoice, "ShipFromPostalCode", cd.OriginPostalCode);

                if (cd.LineItems.Count > 0) {
                    var lineItemsElement = et.SubElement(invoice, "LineItems");

                    foreach (var item in cd.LineItems) {
                        var lineItem = et.SubElement(lineItemsElement, "LineItem");
                        et.SubElement(lineItem, "CommodityCode", item.CommodityCode);
                        et.SubElement(lineItem, "Description", item.Description);
                        et.SubElement(lineItem, "Upc", item.UPC);
                        et.SubElement(lineItem, "Quantity", item.Quantity);
                        et.SubElement(lineItem, "UnitOfMeasure", item.UnitOfMeasure);
                        et.SubElement(lineItem, "UnitCost", item.UnitCost);
                        et.SubElement(lineItem, "DiscountAmount", item.DiscountDetails?.DiscountAmount);
                        et.SubElement(lineItem, "TotalAmount", item.TotalAmount);
                        et.SubElement(lineItem, "TaxAmount", item.TaxAmount);
                        et.SubElement(lineItem, "ExtendedAmount", item.ExtendedAmount);
                        et.SubElement(lineItem, "DebitOrCreditIndicator", item.CreditDebitIndicator.ToString());
                        et.SubElement(lineItem, "NetOrGrossIndicator", item.NetGrossIndicator.ToString());
                    }
                }
            }

            var response = DoTransaction(BuildEnvelope(et, transaction));
            return MapResponse(builder, response);
        }

        public Transaction ManageTransaction(ManagementBuilder builder) {
            var et = new ElementTree();
            TransactionType transactionType = builder.TransactionType;

            var transaction = et.Element(MapRequestType(builder))
                .Set("xmlns", "http://schemas.merchantwarehouse.com/merchantware/v45/");

            // Credentials
            var credentials = et.SubElement(transaction, "Credentials");
            et.SubElement(credentials, "MerchantName").Text(MerchantName);
            et.SubElement(credentials, "MerchantSiteId").Text(MerchantSiteId);
            et.SubElement(credentials, "MerchantKey").Text(MerchantKey);

            // Payment Data
            if (transactionType.Equals(TransactionType.Refund)) {
                var paymentData = et.SubElement(transaction, "PaymentData");

                et.SubElement(paymentData, "Source").Text("PreviousTransaction");
                et.SubElement(paymentData, "Token", builder.TransactionId);
            }

            // Request
            var request = et.SubElement(transaction, "Request");
            if (!transactionType.Equals(TransactionType.Refund)) {
                et.SubElement(request, "Token", builder.TransactionId);
            }
            et.SubElement(request, "Amount", builder.Amount.ToCurrencyString());
            et.SubElement(request, "InvoiceNumber", builder.InvoiceNumber);
            et.SubElement(request, "RegisterNumber", RegisterNumber);
            et.SubElement(request, "MerchantTransactionId", builder.ClientTransactionId);
            et.SubElement(request, "CardAcceptorTerminalId", TerminalId);

            if (transactionType.Equals(TransactionType.TokenDelete) || transactionType.Equals(TransactionType.TokenUpdate)) {
                var card = builder.PaymentMethod as CreditCardData;

                et.SubElement(request, "VaultToken", card.Token);
                if (transactionType.Equals(TransactionType.TokenUpdate)) {
                    et.SubElement(request, "ExpirationDate", card.ShortExpiry);
                }
            }

            var response = DoTransaction(BuildEnvelope(et, transaction));
            return MapResponse(builder, response);
        }

        public string SerializeRequest(AuthorizationBuilder builder) {
            throw new UnsupportedTransactionException();
        }

        private string BuildEnvelope(ElementTree et, Element transaction) {
            var envelope = et.Element("soap:Envelope");
            var body = et.SubElement(envelope, "soap:Body");

            body.Append(transaction);

            return et.ToString(envelope);
        }

        private string MapRequestType<T>(T builder) where T : TransactionBuilder<Transaction> {
            TransactionType transType = builder.TransactionType;

            switch (transType) {
                case TransactionType.Auth: {
                        if (builder.TransactionModifier.Equals(TransactionModifier.Offline)) {
                            return "ForceCapture";
                        }
                        return "Authorize";
                    }
                case TransactionType.BatchClose: {
                        return "SettleBatch";
                    }
                case TransactionType.Capture: {
                        return "Capture";
                    }
                case TransactionType.Edit: {
                        return "AdjustTip";
                    }
                //AttachSignature
                //FindBoardedCard
                case TransactionType.Refund: {
                        return "Refund";
                    }
                case TransactionType.Sale: {
                        return "Sale";
                    }
                case TransactionType.TokenDelete: {
                        return "UnboardCard";
                    }
                case TransactionType.TokenUpdate: {
                        return "UpdateBoardedCard";
                    }
                case TransactionType.Verify: {
                        return "BoardCard";
                    }
                case TransactionType.Void: {
                        return "Void";
                    }
                default: { throw new UnsupportedTransactionException(); }
            }
        }

        private string MapWalletId(string mobileType) {
            switch (mobileType) {
                case "apple-pay": {
                        return "ApplePay";
                    }
                default: { return "Unknown"; }
            }
        }

        private string MapTaxType(TaxType type) {
            switch (type) {
                case TaxType.NOTUSED: {
                        return "NotProvided";
                    }
                case TaxType.SALESTAX: {
                        return "Provided";
                    }
                case TaxType.TAXEXEMPT: {
                        return "Exempt";
                    }
                default: {
                        return "UNKNOWN";
                    }
            }
        }

        private Transaction MapResponse<T>(T builder, string rawResponse) where T : TransactionBuilder<Transaction> {
            var root = ElementTree.Parse(rawResponse).Get(MapRequestType(builder) + "Response");

            // check response
            string errorCode = root.GetValue<string>("ErrorCode");
            string errorMessage = root.GetValue<string>("ErrorMessage");
            if (!string.IsNullOrEmpty(errorMessage)) {
                throw new GatewayException(
                    string.Format("Unexpected Gateway Response: {0} - {1}", errorCode, errorMessage),
                    errorCode, 
                    errorMessage
                );
            }

            var response = new Transaction {
                ResponseCode = "00",
                ResponseMessage = root.GetValue<string>("ApprovalStatus"),
                TransactionId = root.GetValue<string>("Token"),
                AuthorizationCode = root.GetValue<string>("AuthorizationCode"),
                HostResponseDate = root.GetValue<DateTime>("TransactionDate"),
                AuthorizedAmount = root.GetValue<string>("Amount").ToAmount(),
                AvailableBalance = root.GetValue<string>("RemainingCardBalance")?.ToAmount(),
                //MaskedCardNumber = root.GetValue<string>("CardNumber"),
                //CardHolder
                CardType = root.GetValue<string>("CardType"),
                //FsaCard
                //ReaderEntryMode
                AvsResponseCode = root.GetValue<string>("AvsResponse"),
                CvnResponseCode = root.GetValue<string>("CvResponse"),
                //ExtraData
                //FraudScoring
                //DebitTraceNumber
                //Rfmiq
                //Invoice
                Token = root.GetValue<string>("VaultToken")
            };

            if (root.Has("BatchStatus")) {
                var summary = new BatchSummary {
                    Status = root.GetValue<string>("BatchStatus"),
                    TotalAmount = root.GetValue<decimal>("BatchAmount"),
                    TransactionCount = root.GetValue<int>("TransactionCount")
                };
                response.BatchSummary = summary;
            }

            return response;
        }
    }
}
