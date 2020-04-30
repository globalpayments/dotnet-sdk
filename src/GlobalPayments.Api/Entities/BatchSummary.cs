using System;
using System.Collections.Generic;

namespace GlobalPayments.Api.Entities {
    public class BatchSummary {
        public int Id { get; set; }
        public string CloseTransactionId { get; set; }
        public int CloseCount { get; set; }
        public decimal? CreditAmount { get; set; }
        public int CreditCount { get; set; }
        public decimal? DebitAmount { get; set; }
        public int DebitCount { get; set; }
        public string DeviceId { get; set; }
        public string MerchantName { get; set; }
        public DateTime? OpenTime { get; set; }
        public string OpenTransactionId { get; set; }
        public Transaction ResentBatchClose { get; set; }
        public LinkedList<Transaction> ResentTransactions { get; set; }
        public string ResponseCode { get; set; }
        public decimal? ReturnAmount { get; set; }
        public int ReturnCount { get; set; }
        public decimal? SaleAmount { get; set; }
        public int SaleCount { get; set; }
        public string SequenceNumber { get; set; }
        public string SicCode { get; set; }
        public string SiteId { get; set; }
        public string Status { get; set; }
        public decimal? TotalAmount { get; set; }
        public int TransactionCount { get; set; }
        public string TransactionToken { get; set; }
    }
}
