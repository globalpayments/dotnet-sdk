using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Terminals;

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
        PASSIVE
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

    /// <summary>
    /// Configuration for connecting to a payment gateway
    /// </summary>
    public class ServicesConfig {
        // Portico
        /// <summary>
        /// Account's site ID
        /// </summary>
        public int SiteId { get; set; }
        /// <summary>
        /// Account's license ID
        /// </summary>
        public int LicenseId { get; set; }
        /// <summary>
        /// Account's device ID
        /// </summary>
        public int DeviceId { get; set; }
        /// <summary>
        /// Account's username
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// Account's password
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// Integration's developer ID
        /// </summary>
        /// <remarks>
        /// This is provided at the start of an integration's certification.
        /// </remarks>
        public string DeveloperId { get; set; }
        /// <summary>
        /// Integration's version number
        /// </summary>
        /// <remarks>
        /// This is provided at the start of an integration's certification.
        /// </remarks>
        public string VersionNumber { get; set; }
        /// <summary>
        /// Account's secret API key
        /// </summary>
        public string SecretApiKey { get; set; }

        // Realex
        /// <summary>
        /// Account's account ID
        /// </summary>
        public string AccountId { get; set; }
        /// <summary>
        /// Account's merchant ID
        /// </summary>
        public string MerchantId { get; set; }
        /// <summary>
        /// Account's rebate password
        /// </summary>
        public string RebatePassword { get; set; }
        /// <summary>
        /// Account's refund password
        /// </summary>
        public string RefundPassword { get; set; }
        /// <summary>
        /// Account's shared secret
        /// </summary>
        public string SharedSecret { get; set; }
        /// <summary>
        /// Channel for an integration's transactions (e.g. "internet")
        /// </summary>
        public string Channel { get; set; }
        /// <summary>
        /// Hosted Payment Page (HPP) configuration
        /// </summary>
        public HostedPaymentConfig HostedPaymentConfig { get; set; }

        // Device Specific
        /// <summary>
        /// Connection details for physical card reader device
        /// </summary>
        public ConnectionConfig DeviceConnectionConfig { get; set; }

        // Common
        /// <summary>
        /// Gateway service URL
        /// </summary>
        public string ServiceUrl { get; set; }
        /// <summary>
        /// Timeout value for gateway communication (in milliseconds)
        /// </summary>
        public int Timeout { get; set; }

        /// <summary>
        /// Initiate a `ServicesConfig` object
        /// </summary>
        /// <remarks>
        /// Defaults `ServicesConfig.Timeout` to `65000`.
        /// </remarks>
        public ServicesConfig() {
            Timeout = 65000;
        }

        internal void Validate() {
            // portico api key
            if (!string.IsNullOrEmpty(SecretApiKey)) {
                if (SiteId != default(int) || LicenseId != default(int) || DeviceId != default(int) || !string.IsNullOrEmpty(Username) || !string.IsNullOrEmpty(Password)) {
                    throw new ConfigurationException("Configuration contains both secret api key and legacy credentials. These are mutually exclusive.");
                }
            }

            // portico legacy
            if (SiteId != default(int) || LicenseId != default(int) || DeviceId != default(int) || !string.IsNullOrEmpty(Username) || !string.IsNullOrEmpty(Password)) {
                if(!(SiteId != default(int) && LicenseId != default(int) && DeviceId != default(int) && !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password)))
                    throw new ConfigurationException("Site, License, Device, Username and Password should all have a values for this configuration.");
            }

            // realex
            if (!string.IsNullOrEmpty(MerchantId) || !string.IsNullOrEmpty(SharedSecret)) {
                if(string.IsNullOrEmpty(MerchantId))
                    throw new ConfigurationException("MerchantId is required for this configuration.");
                else if (string.IsNullOrEmpty(SharedSecret))
                    throw new ConfigurationException("SharedSecret is required for this configuration.");
            }

            // service url
            if (string.IsNullOrEmpty(ServiceUrl)) {
                throw new ConfigurationException("Service URL could not be determined from the credentials provided. Please specify an endpoint.");
            }
        }
    }
}
