using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;

namespace GlobalPayments.Api.Entities {
    public class InstallmentData {
        /// <summary>
        /// Indicates the installment payment plan program.
        /// </summary>
        public string Program { get; set; }
        /// <summary>
        /// Indicates he mode of the Installment plan choosen
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
    }
}
