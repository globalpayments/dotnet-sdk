using GlobalPayments.Api.Builders;
using System;
using System.Collections.Generic;

namespace GlobalPayments.Api.Entities {
    public class BatchSummary {
        public int Id { get; set; }
        public string BatchReference { get; set; }
        public string CloseTransactionId { get; set; }

        /// <summary>
        /// The action ID that closed the batch.
        /// </summary>
        public string CloseActionId { get; set; }

        public int CloseCount { get; set; }
        public decimal? CreditAmount { get; set; }
        public int CreditCount { get; set; }
        public decimal? DebitAmount { get; set; }
        public int DebitCount { get; set; }
        public string DeviceId { get; set; }
        public string MerchantName { get; set; }

        /// <summary>
        /// The unique identifier for the merchant.
        /// </summary>
        public string MerchantId { get; set; }

        /// <summary>
        /// The unique identifier for the transaction processing account.
        /// </summary>
        public string AccountId { get; set; }

        /// <summary>
        /// The name of the transaction processing account.
        /// </summary>
        public string AccountName { get; set; }

        /// <summary>
        /// The date and time the batch was opened.
        /// </summary>
        public DateTime? OpenTime { get; set; }

        /// <summary>
        /// The date and time the batch was created.
        /// </summary>
        public DateTime? TimeCreated { get; set; }

        /// <summary>
        /// The date and time the batch was last updated.
        /// </summary>
        public DateTime? TimeLastUpdated { get; set; }

        /// <summary>
        /// The date and time the batch was closed.
        /// </summary>
        public DateTime? TimeClosed { get; set; }

        /// <summary>
        /// The action ID that opened the batch.
        /// </summary>
        public string OpenActionId { get; set; }

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

        /// <summary>
        /// The site reference associated with the batch.
        /// </summary>
        public string SiteReference { get; set; }

        /// <summary>
        /// The device reference associated with the batch.
        /// </summary>
        public string DeviceReference { get; set; }

        public string Status { get; set; }
        public decimal? TotalAmount { get; set; }
        public int TransactionCount { get; set; }
        public string TransactionToken { get; set; }
        public string SystemTraceAuditNumber { get; set; }

        /// <summary>
        /// The currency of the batch transactions.
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// The total gratuity amount for the batch.
        /// </summary>
        public decimal? GratuityAmount { get; set; }
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