using System.Collections.Generic;

namespace GlobalPayments.Api.Entities {
    /// <summary>
    /// Represents the installment term details returned by the GP API,
    /// describing the conditions and parameters of an installment payment plan.
    /// </summary>
    public class Terms {
        /// <summary>
        /// Represents the reference to the installment option being offered.
        /// This field is applicable only if the installment.program is SIP.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The display name of the installment term.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A unique reference identifier for the installment term.
        /// </summary>
        public string Reference { get; set; }

        /// <summary>
        /// The mode of the installment term (e.g., standard, promotional).
        /// </summary>
        public string Mode { get; set; }

        /// <summary>
        /// The number of installment payments in this term.
        /// </summary>
        public string Count { get; set; }

        /// <summary>
        /// The number of grace period payments before the installment plan begins.
        /// </summary>
        public string GracePeriodCount { get; set; }

        /// <summary>
        /// The currency code applicable to the installment term amounts.
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// Indicates the unit of time used for installment payments (e.g., days, months, or years).
        /// </summary>
        public string TimeUnit { get; set; }

        /// <summary>
        /// The percentage cost (interest or fee) applied over the installment term.
        /// </summary>
        public string CostPercentage { get; set; }

        /// <summary>
        /// The total cost of the installment plan, including any fees or interest.
        /// </summary>
        public string TotalPlanCost { get; set; }

        /// <summary>
        /// The amount of each individual installment payment.
        /// </summary>
        public string PlanAmount { get; set; }

        /// <summary>
        /// The list of valid time unit values (e.g., 3, 6, 12) available for this installment term.
        /// </summary>
        public List<int> TimeUnitNumbers { get; set; }

        /// <summary>
        /// The maximum time unit number allowed for this installment term.
        /// </summary>
        public int? MaxTimeUnitNumber { get; set; }

        /// <summary>
        /// The maximum transaction amount eligible for this installment term.
        /// </summary>
        public int? MaxAmount { get; set; }

        /// <summary>
        /// The language code (e.g., "en", "fre") for the terms and conditions presentation.
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// The version of the terms and conditions document accepted by the cardholder.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// The fees associated with this installment term, such as upfront or monthly fees.
        /// </summary>
        public Fees Fees { get; set; }

        /// <summary>
        /// The list of terms and conditions documents applicable to this installment term.
        /// </summary>
        public List<TermsAndConditions> TermsAndConditionsList { get; set; }

        public bool IsEmpty() {
            return string.IsNullOrEmpty(Id) &&
                   string.IsNullOrEmpty(TimeUnit) &&
                   (TimeUnitNumbers == null || TimeUnitNumbers.Count == 0) &&
                   MaxTimeUnitNumber == null;
        }
    }
}
