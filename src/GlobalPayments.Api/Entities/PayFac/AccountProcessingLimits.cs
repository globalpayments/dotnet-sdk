namespace GlobalPayments.Api.Entities.PayFac {
    /// <summary>
    /// GP-API account processing limits. Amounts use major units (100.00 is sent as "10000"). Percentages are unscaled; null values are omitted from an update.
    /// </summary>
    public class AccountProcessingLimits {
        /// <summary>
        /// The permitted negative balance in major units.
        /// </summary>
        public decimal? AllowedNegativeBalanceAmount { get; set; }

        /// <summary>
        /// The maximum card transaction amount in major units.
        /// </summary>
        public decimal? CardTransactionLimitAmount { get; set; }

        /// <summary>
        /// The monthly card processing limit in major units.
        /// </summary>
        public decimal? CardMonthlyLimitAmount { get; set; }

        /// <summary>
        /// The maximum bank transfer amount in major units.
        /// </summary>
        public decimal? BankTransferTransactionLimitAmount { get; set; }

        /// <summary>
        /// The monthly bank transfer processing limit in major units.
        /// </summary>
        public decimal? BankTransferMonthlyLimitAmount { get; set; }

        /// <summary>
        /// The card processing percentage at which payouts are disabled.
        /// </summary>
        public decimal? DisablePayoutCardLimitPercentage { get; set; }

        /// <summary>
        /// The bank transfer processing percentage at which payouts are disabled.
        /// </summary>
        public decimal? DisablePayoutBankTransferLimitPercentage { get; set; }
    }
}
