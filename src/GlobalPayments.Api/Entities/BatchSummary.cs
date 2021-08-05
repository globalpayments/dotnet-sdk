using GlobalPayments.Api.Builders;
using System;
using System.Collections.Generic;

namespace GlobalPayments.Api.Entities {
    public class BatchSummary {
        public int Id { get; set; }
        public string BatchReference { get; set; }
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
        public string SystemTraceAuditNumber { get; set; }
        public bool IsBalanced {
            get {
                if (ResponseCode != null) {
                    return ResponseCode.Equals("500");
                }
                return false;
            }
            set { }
        }

        public BatchSummary ResubmitTransactions(List<String> transactionTokens, String configName) {
            if(!ResponseCode.Equals("580")) {
                throw new BuilderException("Batch recovery has not been requested for this batch.");
            }

            // resubmit the tokens
            LinkedList<Transaction> responses = new LinkedList<Transaction>();
            foreach(string token in transactionTokens) {
                Transaction response = new ResubmitBuilder(TransactionType.DataCollect)
                        .WithTransactionToken(token)
                        .Execute(configName);
                responses.AddLast(response);
            }
            ResentTransactions = responses;

            // resubmit the batch summary
            Transaction batchResponse = new ResubmitBuilder(TransactionType.BatchClose)
                    .WithTransactionToken(TransactionToken)
                    .Execute(configName);
            ResentBatchClose = batchResponse;
            ResponseCode = batchResponse.ResponseCode;
            return this;
        }
    }
}