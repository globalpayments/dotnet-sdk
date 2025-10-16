using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Network.Entities;
using System.Collections.Generic;

namespace GlobalPayments.Api.PaymentMethods {
    internal class TransactionReference : IPaymentMethod {
        internal PaymentMethodType _paymentMethodType;
        internal decimal? _originalAmount;
        public PaymentMethodType PaymentMethodType {
            get {
                if (OriginalPaymentMethod != null) {
                    return OriginalPaymentMethod.PaymentMethodType;
                }
                return _paymentMethodType;
            }
            set { _paymentMethodType = value; }
        }
        public TransactionType OriginalTransactionType { get; set; }
        public string AuthCode { get; set; }
        public string BatchNumber { get; set; }
        public string CardType { get; set; }
        public string OrderId { get; set; }
        public string TransactionId { get; set; }
        public List<FundsAccountDetails> TransfersFundsAccounts { get; set; }
        public string ClientTransactionId { get; set; }
        public string AlternativePaymentType { get; set; }
        public string AcquiringInstitutionId{ get; set; }
        public string MessageTypeIndicator{ get; set; }
        public decimal? OriginalAmount{ 
            get {
                if (OriginalApprovedAmount != null) {
                    return OriginalApprovedAmount; 
                }
                return _originalAmount;
            }
            set {
                _originalAmount = value; 
            } 
        }
        public decimal? OriginalApprovedAmount { get; set; }
        public IPaymentMethod OriginalPaymentMethod{ get; set; }
        public string OriginalProcessingCode{ get; set; }
        public string OriginalTransactionTime{ get; set; }
        public string OriginalUTCTransactionTime { get; set; }
        public bool OriginalAmountEstimated { get; set; }
        public bool PartialApproval { get; set; }
        public int SequenceNumber{ get; set; }
        public string SystemTraceAuditNumber{ get; set; }
        public NtsData NtsData { get; set; }
        public AlternativePaymentResponse AlternativePaymentResponse { get; set; }
        
        public BNPLResponse BNPLResponse;        

        public void SetNtsData(string value) {
            this.NtsData = NtsData.FromString(value);
        }
        public decimal GetOriginalApprovedAmount() {
            if (OriginalApprovedAmount != null) {
                return (decimal)OriginalApprovedAmount;
            }

            return (decimal)OriginalAmount;
        }
    }
}
