using System;

namespace GlobalPayments.Api.Entities {
    public class TransactionSummary {
        public decimal? Amount { get; set; }
        public string AuthCode { get; set; }
        public decimal? AuthorizedAmount { get; set; }
        public string ClientTransactionId { get; set; }
        public int DeviceId { get; set; }
        public string IssuerResponseCode { get; set; }
        public string IssuerResponseMessage { get; set; }
        public string MaskedCardNumber { get; set; }
        public string OriginalTransactionId { get; set; }
        public string GatewayResponseCode { get; set; }
        public string GatewayResponseMessage { get; set; }
        public string ReferenceNumber { get; set; }
        public string ServiceName { get; set; }
        public decimal? SettlementAmount { get; set; }
        public string Status { get; set; }
        public DateTime TransactionDate { get; set; }
        public string TransactionId { get; set; }
    }
}
