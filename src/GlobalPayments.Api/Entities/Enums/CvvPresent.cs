using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Entities {
    /// <summary>
    /// Indicates whether CVV was provided in the transaction.
    /// </summary>
    [MapTarget(Target.GP_API)]
    public enum CvvPresent {
        /// <summary>
        /// CVV is present.
        /// </summary>
        [Map(Target.GP_API, "YES")]
        YES,

        /// <summary>
        /// CVV is not present.
        /// </summary>
        [Map(Target.GP_API, "NO")]
        NO
    }
}
