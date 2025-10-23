using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Entities {
    /// <summary>
    /// Represents the supported payment method names for transactions.
    /// </summary>
    [MapTarget(Target.GP_API)]
    public enum PaymentMethodName {
        /// <summary>
        /// Alternative Payment Method (APM).
        /// </summary>
        [Map(Target.GP_API, "APM")]
        APM,

        /// <summary>
        /// Digital wallet payment method.
        /// </summary>
        [Map(Target.GP_API, "DIGITAL WALLET")]
        DigitalWallet,

        /// <summary>
        /// Card payment method.
        /// </summary>
        [Map(Target.GP_API, "CARD")]
        Card,

        /// <summary>
        /// Bank transfer payment method.
        /// </summary>
        [Map(Target.GP_API, "BANK_TRANSFER")]
        BankTransfer,

        /// <summary>
        /// Bank payment method.
        /// </summary>
        [Map(Target.GP_ECOM, "BANK PAYMENT")]
        [Map(Target.GP_API, "BANK_PAYMENT")]
        BankPayment,

        /// <summary>
        /// Buy Now, Pay Later (BNPL) payment method.
        /// </summary>
        [Map(Target.GP_API, "BNPL")]
        BNPL,

        /// <summary>
        /// BLIK payment method.
        /// </summary>
        [Map(Target.GP_API, "BLIK")]
        BLIK
    }
}
