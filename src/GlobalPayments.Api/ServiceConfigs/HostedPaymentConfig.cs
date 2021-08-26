using GlobalPayments.Api.Entities;

namespace GlobalPayments.Api {
    /// <summary>
    /// Options when specifying HPP versions.
    /// Useful with `HostedPaymentConfig`.
    /// </summary>
    public static class HppVersion {
        /// <summary>
        /// HPP Version 1
        /// </summary>
        public const string VERSION_1 = "1";
        /// <summary>
        /// HPP Version 2
        /// </summary>
        public const string VERSION_2 = "2";
    }

    /// <summary>
    /// Specify how the fraud filter should operate
    /// </summary>
    public enum FraudFilterMode {
        /// <summary>
        /// Fraud filter will behave as configured in RealControl
        /// </summary>
        NONE,
        /// <summary>
        /// Disables the fraud filter
        /// </summary>
        OFF,
        /// <summary>
        /// Sets the fraud filter to passive mode
        /// </summary>
        PASSIVE,
        /// <summary>
        /// Sets the fraud filter to active mode
        /// </summary>
        ACTIVE
    }

    /// <summary>
    /// Hosted Payment Page (HPP) configuration
    /// </summary>
    /// <remarks>
    /// This configuration is used when constructing HPP requests to be used by
    /// a client library (JS, iOS, Android).
    /// </remarks>
    public class HostedPaymentConfig {
        /// <summary>
        /// Allow card to be stored within the HPP
        /// </summary>
        public bool? CardStorageEnabled { get; set; }
        /// <summary>
        /// Allow Dynamic Currency Conversion (DCC) to be available
        /// </summary>
        public bool? DynamicCurrencyConversionEnabled { get; set; }
        /// <summary>
        /// Allow a consumer's previously stored cards to be shown
        /// </summary>
        public bool? DisplaySavedCards { get; set; }
        /// <summary>
        /// Manner in which the fraud filter should operate
        /// </summary>
        public FraudFilterMode FraudFilterMode { get; set; }
        /// <summary>
        /// Fraud filter rules collection
        /// </summary>
        public FraudRuleCollection FraudFilterRules { get; set; }
        /// <summary>
        /// The desired language for the HPP
        /// </summary>
        public string Language { get; set; }
        /// <summary>
        /// Text for the HPP's submit button
        /// </summary>
        public string PaymentButtonText { get; set; }
        /// <summary>
        /// URL to receive `POST` data of the HPP's result
        /// </summary>
        public string ResponseUrl { get; set; }
        /// <summary>
        /// Denotes if Transaction Stability Score (TSS) should be active
        /// </summary>
        public bool? RequestTransactionStabilityScore { get; set; }
        /// <summary>
        /// Specify HPP version
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// iFrame Optimisation - dimensions
        /// </summary>
        public string PostDimensions { get; set; }
        /// <summary>
        /// iFrame Optimisation - response
        /// </summary>
        public string PostResponse { get; set; }
    }
}
