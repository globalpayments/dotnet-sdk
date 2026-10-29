namespace GlobalPayments.Api.Entities.Reporting {
    /// <summary>
    /// Fund management account statistics. Gateway minor-unit amounts are converted to major units using the SDK's two-decimal account amount convention; percentages are unscaled.
    /// </summary>
    public class AccountProcessingStatistics {
        /// <summary>
        /// The maximum card transaction amount.
        /// </summary>
        public decimal? CardTransactionLimitAmount { get; set; }
        /// <summary>
        /// The monthly card processing limit.
        /// </summary>
        public decimal? CardMonthlyLimitAmount { get; set; }
        /// <summary>
        /// The maximum bank transfer transaction amount.
        /// </summary>
        public decimal? BankTransferPerTransactionLimitAmount { get; set; }
        /// <summary>
        /// The monthly bank transfer processing limit.
        /// </summary>
        public decimal? BankTransferMonthlyLimitAmount { get; set; }
        /// <summary>
        /// The bank transfer amount processed this month.
        /// </summary>
        public decimal? BankTransferMonthToDateProcessedAmount { get; set; }
        /// <summary>
        /// The card amount processed this month.
        /// </summary>
        public decimal? CardMonthToDateProcessedAmount { get; set; }
        /// <summary>
        /// The balance available for transfer or payout.
        /// </summary>
        public decimal? AvailableBalanceAmount { get; set; }
        /// <summary>
        /// The balance awaiting funding.
        /// </summary>
        public decimal? PendingBalanceAmount { get; set; }
        /// <summary>
        /// The balance held in reserve.
        /// </summary>
        public decimal? ReserveBalanceAmount { get; set; }
        /// <summary>
        /// The permitted negative balance.
        /// </summary>
        public decimal? AllowedNegativeBalanceAmount { get; set; }
        /// <summary>
        /// The card percentage at which payouts are disabled.
        /// </summary>
        public decimal? DisablePayoutCardLimitPercentage { get; set; }
        /// <summary>
        /// The bank transfer percentage at which payouts are disabled.
        /// </summary>
        public decimal? DisablePayoutBankTransferLimitPercentage { get; set; }
        /// <summary>
        /// The card processing percentage for the current month.
        /// </summary>
        public decimal? CardMonthToDateProcessedPercentage { get; set; }
        /// <summary>
        /// The bank transfer processing percentage for the current month.
        /// </summary>
        public decimal? BankTransferMonthToDateProcessedPercentage { get; set; }
    }
}
