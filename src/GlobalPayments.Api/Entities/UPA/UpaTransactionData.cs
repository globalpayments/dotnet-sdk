using System;

namespace GlobalPayments.Api.Entities.UPA {
    public class UpaTransactionData {
        public decimal TotalAmount { get; set; }
        public decimal CashBackAmount { get; set; }
        public DateTime TranDate { get; set; }
        public DateTime TranTime { get; set; }
        public TransactionType TransType { get; set; }
    }
}
