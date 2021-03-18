using System;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Gateways;

namespace GlobalPayments.Api {
    public class GeniusConfig : GatewayConfig {
        public string ClerkId { get; set; }

        public string MerchantName { get; set; }

        public string MerchantSiteId { get; set; }

        public string MerchantKey { get; set; }

        public string RegisterNumber { get; set; }

        public string DBA { get; set; }

        public string TerminalId { get; set; }

        public GeniusConfig() : base(GatewayProvider.Genius) { }

        internal override void ConfigureContainer(ConfiguredServices services) {
            base.ConfigureContainer(services);

            if (string.IsNullOrEmpty(ServiceUrl)) {
                if (Environment.Equals(Entities.Environment.TEST)) {
                    ServiceUrl = ServiceEndpoints.GENIUS_API_TEST;
                }
                else ServiceUrl = ServiceEndpoints.GENIUS_API_PRODUCTION;
            }

            var gateway = new GeniusConnector() {
                MerchantName = MerchantName,
                MerchantSiteId = MerchantSiteId,
                MerchantKey = MerchantKey,
                RegisterNumber = RegisterNumber,
                TerminalId = TerminalId,
                Timeout = Timeout,
                ServiceUrl = ServiceUrl,
                RequestLogger = RequestLogger,
                WebProxy = WebProxy
            };

            services.GatewayConnector = gateway;
        }

        internal override void Validate() {
            base.Validate();

            if (string.IsNullOrEmpty(MerchantSiteId)) {
                throw new ConfigurationException("MerchantSiteId is required for this configuration.");
            }
            else if (string.IsNullOrEmpty(MerchantName)) {
                throw new ConfigurationException("MerchantName is required for this configuration.");
            }
            else if (string.IsNullOrEmpty(MerchantKey)) {
                throw new ConfigurationException("MerchantKey is required for this configuration.");
            }
        }
    }
}
