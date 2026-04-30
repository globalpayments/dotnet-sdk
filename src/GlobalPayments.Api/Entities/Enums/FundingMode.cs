using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Entities.Enums {
    /// <summary>
    /// Specifies the funding mode for a transaction or account.
    /// </summary>
    public enum FundingMode {
        /// <summary>
        /// The transaction or account is funded entirely by the merchant.
        /// </summary>
        [Map(Target.GP_API, "MERCHANT_FUNDED")]
        MERCHANT_FUNDED,

        /// <summary>
        /// The transaction or account is funded entirely by the consumer.
        /// </summary>
        [Map(Target.GP_API, "CONSUMER_FUNDED")]
        CONSUMER_FUNDED,

        /// <summary>
        /// The transaction or account is funded by a combination of both merchant and consumer contributions.
        /// </summary>
        [Map(Target.GP_API, "HYBRID_FUNDED")]
        HYBRID_FUNDED,

        /// <summary>
        /// A bilateral funding arrangement between two parties, typically merchant and a financial institution.
        /// </summary>
        [Map(Target.GP_API, "BILATERAL")]
        BILATERAL,

        /// <summary>
        /// Represents any funding mode, used when no specific mode restriction applies.
        /// </summary>
        [Map(Target.GP_API, "ANY")]
        ANY
    }
}
