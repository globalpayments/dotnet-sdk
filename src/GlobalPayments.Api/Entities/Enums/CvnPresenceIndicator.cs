using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Entities {
    /// <summary>
    /// Indicates CVN presence at time of payment.
    /// </summary>
    public enum CvnPresenceIndicator {
        /// <summary>
        /// Indicates CVN was present.
        /// </summary>
        [Map(Target.GP_API, "PRESENT")]
        Present = 1,

        /// <summary>
        /// Indicates CVN was present but illegible.
        /// </summary>
        [Map(Target.GP_API, "ILLEGIBLE")]
        Illegible,

        /// <summary>
        /// Indicates CVN was not present.
        /// </summary>
        [Map(Target.GP_API, "NOT_PRESENT")]
        NotOnCard,

        /// <summary>
        /// Indicates CVN was not requested.
        /// </summary>
        NotRequested
    }
}
