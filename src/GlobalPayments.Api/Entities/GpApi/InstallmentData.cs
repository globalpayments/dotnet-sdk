using GlobalPayments.Api.Entities.Enums;

namespace GlobalPayments.Api.Entities {
    public class InstallmentData {
        /// <summary>
        /// Indicates the installment payment plan program.
        /// </summary>
        public string Program { get; set; }
        /// <summary>
        /// Indicates the mode of the Installment plan chosen
        /// </summary>
        public string Mode { get; set; }
        /// <summary>
        /// Indicates the total number of payments to be made over the course of the installment payment plan.
        /// </summary>
        public string Count { get; set; }
        /// <summary>
        /// Indicates the grace period before the first payment.
        /// </summary>
        public string GracePeriodCount { get; set; }

        /// <summary>
        /// Installment ID reference (from installment query)
        /// </summary>
        public string Reference { get; set; }

        /// <summary>
        /// Visa installment funding mode (e.g., MERCHANT_FUNDED, CONSUMER_FUNDED)
        /// </summary>
        public FundingMode? FundingMode { get; set; }

        /// <summary>
        /// Visa installment terms
        /// </summary>
        public Terms Terms { get; set; }

        /// <summary>
        /// Eligible plan type for Visa installments (e.g., FULL, LIMITED)
        /// </summary>
        public EligiblePlans? EligiblePlans { get; set; }

        /// <summary>
        /// The unique identifier of the installment record returned by the gateway.
        /// </summary>
        public string Id { get; set; }
    }
}
