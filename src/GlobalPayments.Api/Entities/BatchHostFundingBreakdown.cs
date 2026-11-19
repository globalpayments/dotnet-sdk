namespace GlobalPayments.Api.Entities {
    /// <summary>
    /// Sale and refund totals for a host funding direction.
    /// </summary>
    public class BatchHostFundingBreakdown {
        /// <summary>
        /// Sale totals.
        /// </summary>
        public BatchAmountSummary Sales { get; set; }

        /// <summary>
        /// Refund totals.
        /// </summary>
        public BatchAmountSummary Refunds { get; set; }
    }
}
