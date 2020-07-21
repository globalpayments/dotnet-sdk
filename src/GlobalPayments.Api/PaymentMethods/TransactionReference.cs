using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Network.Entities;

namespace GlobalPayments.Api.PaymentMethods {
    internal class TransactionReference : IPaymentMethod {
        public PaymentMethodType PaymentMethodType {
            get {
                if (OriginalPaymentMethod != null) {
                    return OriginalPaymentMethod.PaymentMethodType;
                }
                return PaymentMethodType;
            }
            set { }
        }
        public string AuthCode { get; set; }
        public string BatchNumber { get; set; }
        public string OrderId { get; set; }
        public string TransactionId { get; set; }
        public string ClientTransactionId { get; set; }
        public string AlternativePaymentType { get; set; }
        public string AcquiringInstitutionId{ get; set; }
        public string MessageTypeIndicator{ get; set; }
        public decimal? OriginalAmount{ get; set; }
        public IPaymentMethod OriginalPaymentMethod{ get; set; }
        public string OriginalProcessingCode{ get; set; }
        public string OriginalTransactionTime{ get; set; }
        public int SequenceNumber{ get; set; }
        public string SystemTraceAuditNumber{ get; set; }
        public NtsData NtsData { get; set; }
        public void SetNtsData(string value) {
            this.NtsData = NtsData.FromString(value);
        }
    }
}
