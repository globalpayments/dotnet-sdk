using GlobalPayments.Api.Entities;

namespace GlobalPayments.Api.PaymentMethods {
    internal class TransactionReference : IPaymentMethod {
        public PaymentMethodType PaymentMethodType { get; set; }
        public string AuthCode { get; set; }
        public string BatchNumber { get; set; }
        public string OrderId { get; set; }
        public string TransactionId { get; set; }
        public string ClientTransactionId { get; set; }
        public string AlternativePaymentType { get; set; }
    }
}
