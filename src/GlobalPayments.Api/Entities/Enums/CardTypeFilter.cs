using GlobalPayments.Api.Utils;
using System;

namespace GlobalPayments.Api.Entities.Enums {
    /// <summary>
    /// Defines card type filters used to specify which card types are accepted or processed.
    /// This enumeration supports bitwise combination of its member values using the Flags attribute.
    /// </summary>
    [Flags]
    public enum CardTypeFilter {
        /// <summary>
        /// Gift card type filter.
        /// </summary>
        [Map(Target.UPA, "GIFT")]
        GIFT = 0x01,
        
        /// <summary>
        /// Visa card type filter.
        /// </summary>
        [Map(Target.UPA, "VISA")]
        VISA = 0x02,
        
        /// <summary>
        /// MasterCard type filter.
        /// </summary>
        [Map(Target.UPA, "MC")]
        MC = 0x04,
        
        /// <summary>
        /// American Express card type filter.
        /// </summary>
        [Map(Target.UPA, "AMEX")]
        AMEX = 0x08,
        
        /// <summary>
        /// Discover card type filter.
        /// </summary>
        [Map(Target.UPA, "DISCOVER")]
        DISCOVER = 0x10
    }
}
