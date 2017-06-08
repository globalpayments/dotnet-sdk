using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Terminals;

namespace GlobalPayments.Api {
    public static class HppVersion {
        public const string VERSION_1 = "1";
        public const string VERSION_2 = "2";
    }

    public enum FraudFilterMode {
        OFF,
        ACTIVE,
        PASSIVE
    }

    public class HostedPaymentConfig {
        public bool CardStorageEnabled { get; set; }
        public bool DirectCurrencyConversionEnabled { get; set; }
        public bool DisplaySavedCards { get; set; }
        public FraudFilterMode FraudFilterMode { get; set; }
        public string Language { get; set; }
        public string PaymentButtonText { get; set; }
        public string ResponseUrl { get; set; }
        public bool RequestTransactionStabilityScore { get; set; }
        public string Version { get; set; }
    }

    public class ServicesConfig {
        // Portico
        public int SiteId { get; set; }
        public int LicenseId { get; set; }
        public int DeviceId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string DeveloperId { get; set; }
        public string VersionNumber { get; set; }
        public string SecretApiKey { get; set; }

        // Realex
        public string AccountId { get; set; }
        public string MerchantId { get; set; }
        public string RebatePassword { get; set; }
        public string RefundPassword { get; set; }
        public string SharedSecret { get; set; }
        public string Channel { get; set; }
        public HostedPaymentConfig HostedPaymentConfig { get; set; }

        // Device Specific
        public ConnectionConfig DeviceConnectionConfig { get; set; }

        // Common
        public string ServiceUrl { get; set; }
        public int Timeout { get; set; }

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
