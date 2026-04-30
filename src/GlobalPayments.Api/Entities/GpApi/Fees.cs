using System.Collections.Generic;

namespace GlobalPayments.Api.Entities {
    /// <summary>
    /// Represents a collection of fee information and related total amounts.
    /// </summary>
    public class Fees {
        /// <summary>
        /// List of fee information details.
        /// </summary>
        public List<FeeInfo> FeeInfo { get; set; }

        /// <summary>
        /// The total amount of all fees.
        /// </summary>
        public string TotalAmount { get; set; }

        /// <summary>
        /// The total amount for subsequent fees.
        /// </summary>
        public string TotalSubsequentAmount { get; set; }

        /// <summary>
        /// The amount for a single subsequent fee.
        /// </summary>
        public string SubsequentAmount { get; set; }

        /// <summary>
        /// The total amount for upfront fees.
        /// </summary>
        public string TotalUpfrontAmount { get; set; }

        /// <summary>
        /// The amount for a single upfront fee.
        /// </summary>
        public string UpfrontAmount { get; set; }
    }
}
