namespace GlobalPayments.Api.Entities {
    /// <summary>
    /// A transaction count and amount within a GP-API batch breakdown.
    /// </summary>
    public class BatchAmountSummary {
        /// <summary>
        /// The transaction count.
        /// </summary>
        public int? Count { get; set; }

        /// <summary>
        /// The amount in major currency units.
        /// </summary>
        public decimal? Amount { get; set; }
    }
}
