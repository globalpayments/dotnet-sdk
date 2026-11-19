namespace GlobalPayments.Api.Entities {
    /// <summary>
    /// The transaction totals grouped by processing host within a GP-API batch.
    /// </summary>
    public class BatchHostBreakdown {
        /// <summary>
        /// The name of the processing host.
        /// </summary>
        public string MerchantName { get; set; }

        /// <summary>
        /// The host reference.
        /// </summary>
        public string Reference { get; set; }

        /// <summary>
        /// The total number of transactions processed by the host.
        /// </summary>
        public int? Count { get; set; }

        /// <summary>
        /// The total amount processed by the host in major currency units.
        /// </summary>
        public decimal? Amount { get; set; }

        /// <summary>
        /// Funding debit totals for the host.
        /// </summary>
        public BatchHostFundingBreakdown FundingDebit { get; set; }

        /// <summary>
        /// Funding credit totals for the host.
        /// </summary>
        public BatchHostFundingBreakdown FundingCredit { get; set; }
    }
}
