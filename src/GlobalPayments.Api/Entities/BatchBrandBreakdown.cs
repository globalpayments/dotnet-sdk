namespace GlobalPayments.Api.Entities {
    /// <summary>
    /// The transaction totals for a card brand within a GP-API batch.
    /// </summary>
    public class BatchBrandBreakdown {
        /// <summary>
        /// The card brand.
        /// </summary>
        public string Brand { get; set; }

        /// <summary>
        /// The total amount for the card brand in major currency units.
        /// </summary>
        public decimal? Amount { get; set; }

        /// <summary>
        /// The number of transactions for the card brand.
        /// </summary>
        public int? Count { get; set; }

        /// <summary>
        /// The total gratuity amount for the card brand in major currency units.
        /// </summary>
        public decimal? GratuityAmount { get; set; }

        /// <summary>
        /// Sale totals for the card brand.
        /// </summary>
        public BatchAmountSummary Sales { get; set; }

        /// <summary>
        /// Refund totals for the card brand.
        /// </summary>
        public BatchAmountSummary Refunds { get; set; }

        /// <summary>
        /// Funding debit totals for the card brand.
        /// </summary>
        public BatchAmountSummary FundingDebit { get; set; }

        /// <summary>
        /// Funding credit totals for the card brand.
        /// </summary>
        public BatchAmountSummary FundingCredit { get; set; }
    }
}
