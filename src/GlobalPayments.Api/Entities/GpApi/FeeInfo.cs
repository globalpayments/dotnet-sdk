namespace GlobalPayments.Api.Entities {
    /// <summary>
    /// Represents fee-related information associated with a transaction or account.
    /// This class captures the type of fee, a percentage-based component, and a
    /// flat (fixed) monetary component.
    /// </summary>
    public class FeeInfo {
        /// <summary>
        /// The fee type identifier or description.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Interest or percentage rate component of the fee, expressed as a decimal string.
        /// </summary>
        public string InterestRate { get; set; }

        /// <summary>
        /// Flat (fixed) monetary amount component of the fee, expressed as a decimal string.
        /// </summary>
        public string FlatAmount { get; set; }
    }
}
