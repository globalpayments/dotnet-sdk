using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Entities {
    public enum PaymentMethodUsageMode {        
        /// <summary>
        /// The payment method token is temporary; removed once a transaction is executed or after a short period.
        /// Required when using Hosted Fields or Drop-in UI integration types.
        /// </summary>
        [Map(Target.GP_API, "SINGLE")]
        Single,

        /// <summary>
        /// The payment method token is permanent and can be used to create many transactions.
        /// </summary>
        [Map(Target.GP_API, "MULTIPLE")]
        Multiple,

        /// <summary>
        /// When transacting with a token, instructs the gateway to use the card number (PAN/FPAN)
        /// when both the card number and the network token are available.
        /// </summary>
        [Map(Target.GP_API, "USE_CARD_NUMBER")]
        UseCardNumber,

        /// <summary>
        /// When transacting with a token, instructs the gateway to use the network token
        /// instead of the card number when both are available.
        /// </summary>
        [Map(Target.GP_API, "USE_NETWORK_TOKEN")]
        UseNetworkToken

    }
}
