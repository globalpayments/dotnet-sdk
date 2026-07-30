using GlobalPayments.Api.Entities.Enums;

namespace GlobalPayments.Api.Entities {
    /// <summary>
    /// Represents a surcharge applied to a specific card type for a Pay By Link order.
    /// </summary>
    public class Surcharge {
        /// <summary>
        /// The card type to which the surcharge applies (e.g., debit, credit, commercial).
        /// </summary>
        public SurchargeCardType? CardType { get; set; }

        /// <summary>
        /// The surcharge amount applied to the associated card type.
        /// </summary>
        public decimal? Amount { get; set; }
    }
}
