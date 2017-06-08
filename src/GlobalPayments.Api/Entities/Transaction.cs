using GlobalPayments.Api.Builders;
using GlobalPayments.Api.PaymentMethods;

namespace GlobalPayments.Api.Entities {
    public class Transaction {
        public decimal? AuthorizedAmount { get; set; }
        public string AuthorizationCode {
            get {
                if (TransactionReference != null)
                    return TransactionReference.AuthCode;
                return null;
            }
            set {
                if (TransactionReference == null)
                    TransactionReference = new TransactionReference();
                TransactionReference.AuthCode = value;
            }
        }
        public decimal? AvailableBalance { get; set; }
        public string AvsResponseCode { get; set; }
        public string AvsResponseMessage { get; set; }
        public decimal? BalanceAmount { get; set; }
        public BatchSummary BatchSummary { get; set; }
        public string CardType { get; set; }
        public string CardLast4 { get; set; }
        public string CavvResponseCode { get; set; }
        public string ClientTransactionId {
            get {
                if (TransactionReference != null)
                    return TransactionReference.ClientTransactionId;
                return null;
            }
            set {
                if (TransactionReference == null)
                    TransactionReference = new TransactionReference();
                TransactionReference.ClientTransactionId = value;
            }
        }
        public string CommercialIndicator { get; set; }
        public string CvnResponseCode { get; set; }
        public string CvnResponseMessage { get; set; }
        public string EmvIssuerResponse { get; set; }
        public string OrderId {
            get {
                if (TransactionReference != null)
                    return TransactionReference.OrderId;
                return null;
            }
            set {
                if (TransactionReference == null)
                    TransactionReference = new TransactionReference();
                TransactionReference.OrderId = value;
            }
        }
        public PaymentMethodType PaymentMethodType {
            get {
                if (TransactionReference != null)
                    return TransactionReference.PaymentMethodType;
                return PaymentMethodType.Credit;
            }
            set {
                if (TransactionReference == null)
                    TransactionReference = new TransactionReference();
                TransactionReference.PaymentMethodType = value;
            }
        }
        public decimal? PointsBalanceAmount { get; set; }
        public string RecurringDataCode { get; set; }
        public string ReferenceNumber { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public string Timestamp { get; set; }
        public string TransactionDescriptor { get; set; }
        public string TransactionId {
            get {
                if (TransactionReference != null)
                    return TransactionReference.TransactionId;
                return null;
            }
            set {
                if (TransactionReference == null)
                    TransactionReference = new TransactionReference();
                TransactionReference.TransactionId = value;
            }
        }
        public string Token { get; set; }

        internal GiftCard GiftCard { get; set; }
        internal TransactionReference TransactionReference { get; set; }

        public static Transaction FromId(string transactionId, PaymentMethodType paymentMethodType = PaymentMethodType.Credit) {
            return new Transaction {
                TransactionReference = new TransactionReference {
                    TransactionId = transactionId,
                    PaymentMethodType = paymentMethodType
                }
            };
        }
        public static Transaction FromId(string transactionId, string orderId, PaymentMethodType paymentMethodType = PaymentMethodType.Credit) {
            return new Transaction {
                TransactionReference = new TransactionReference {
                    TransactionId = transactionId,
                    PaymentMethodType = paymentMethodType,
                    OrderId = orderId
                }
            };
        }

        public ManagementBuilder AdditionalAuth(decimal? amount = null) {
            return new ManagementBuilder(TransactionType.Auth)
                .WithPaymentMethod(TransactionReference)
                .WithAmount(amount);
        }

        public ManagementBuilder Capture(decimal? amount = null) {
            return new ManagementBuilder(TransactionType.Capture)
                .WithPaymentMethod(TransactionReference)
                .WithAmount(amount);
        }

        public ManagementBuilder Edit() {
            return new ManagementBuilder(TransactionType.Edit).WithPaymentMethod(TransactionReference);
        }

        public ManagementBuilder Hold() {
            return new ManagementBuilder(TransactionType.Hold).WithPaymentMethod(TransactionReference);
        }

        public ManagementBuilder Refund(decimal? amount = null) {
            return new ManagementBuilder(TransactionType.Refund)
                .WithPaymentMethod(TransactionReference)
                .WithAmount(amount);
        }

        public ManagementBuilder Release() {
            return new ManagementBuilder(TransactionType.Release).WithPaymentMethod(TransactionReference);
        }

        public ManagementBuilder Reverse(decimal? amount = null) {
            return new ManagementBuilder(TransactionType.Reversal)
                .WithPaymentMethod(TransactionReference)
                .WithAmount(amount);
        }

        public ManagementBuilder Void() {
            return new ManagementBuilder(TransactionType.Void).WithPaymentMethod(TransactionReference);
        }
    }
}
