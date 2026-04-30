using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Entities.Enums {
    /// <summary>
    /// Specifies the eligibility plan type for a transaction or account.
    /// </summary>
    public enum EligiblePlans {
        /// <summary>
        /// The account or transaction is eligible for a full plan, with no restrictions on available features or benefits.
        /// </summary>
        [Map(Target.GP_API, "FULL")]
        FULL,

        /// <summary>
        /// The account or transaction is eligible for a limited plan, with a restricted set of features or benefits.
        /// </summary>
        [Map(Target.GP_API, "LIMITED")]
        LIMITED
    }
}
