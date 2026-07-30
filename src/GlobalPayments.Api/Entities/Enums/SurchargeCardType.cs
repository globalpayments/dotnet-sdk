using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Entities.Enums {
    /// <summary>
    /// Represents the card types to which a Pay By Link surcharge can be applied.
    /// </summary>
    [MapTarget(Target.GP_API)]
    public enum SurchargeCardType {
        /// <summary>
        /// Surcharge applied to debit cards.
        /// </summary>
        [Map(Target.GP_API, "DEBIT")]
        DEBIT,

        /// <summary>
        /// Surcharge applied to credit cards.
        /// </summary>
        [Map(Target.GP_API, "CREDIT")]
        CREDIT,

        /// <summary>
        /// Surcharge applied to commercial cards.
        /// </summary>
        [Map(Target.GP_API, "COMMERCIAL")]
        COMMERCIAL
    }
}
