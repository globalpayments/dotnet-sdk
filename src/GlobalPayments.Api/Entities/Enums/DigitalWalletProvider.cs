using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Entities.Enums {
    /// <summary>
    /// Represents the digital wallet providers that can be enabled on a Hosted Payment Page.
    /// </summary>
    [MapTarget(Target.GP_API)]
    public enum DigitalWalletProvider {
        /// <summary>
        /// Apple Pay digital wallet.
        /// </summary>
        [Map(Target.GP_API, "APPLEPAY")]
        APPLEPAY,

        /// <summary>
        /// Google Pay digital wallet.
        /// </summary>
        [Map(Target.GP_API, "GOOGLEPAY")]
        GOOGLEPAY,

        /// <summary>
        /// Click to Pay digital wallet.
        /// </summary>
        [Map(Target.GP_API, "CLICK_TO_PAY")]
        CLICK_TO_PAY
    }
}
