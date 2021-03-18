using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Gateways;
using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api {
    public class TransitConfig : GatewayConfig {
        public string DeveloperId { get; set; }
        public string DeviceId { get;set; }
        public string MerchantId { get; set; }
        public string TransactionKey { get; set; }
        
        public TransitConfig() : base(GatewayProvider.TransIT) { }

        internal override void ConfigureContainer(ConfiguredServices services) {
            base.ConfigureContainer(services);

            if (string.IsNullOrEmpty(ServiceUrl)) {
                if (Environment.Equals(Entities.Environment.TEST)) {
                    ServiceUrl = ServiceEndpoints.TRANSIT_MULTIPASS_TEST;
                }
                else ServiceUrl = ServiceEndpoints.TRANSIT_MULTIPASS_PRODUCTION;
            }

            var gateway = new TransitConnector() {
                AcceptorConfig = AcceptorConfig,
                DeveloperId = DeveloperId,
                DeviceId = DeviceId,
                MerchantId = MerchantId,
                TransactionKey = TransactionKey,
                ServiceUrl = ServiceUrl,
                Timeout = Timeout,
                RequestLogger = RequestLogger,
                WebProxy = WebProxy
            };

            services.GatewayConnector = gateway;
        }

        internal override void Validate() {
            base.Validate();

            if (AcceptorConfig == null) {
                throw new ConfigurationException("You must provide a valid AcceptorConfig.");
            }
            else {
                AcceptorConfig.Validate(Target.Transit);
            }

            if (string.IsNullOrEmpty(DeviceId)) {
                throw new ConfigurationException("DeviceId cannot be null.");
            }

            if (string.IsNullOrEmpty(MerchantId)) {
                throw new ConfigurationException("MerchantId cannot be null.");
            }

            if (string.IsNullOrEmpty(TransactionKey)) {
                throw new ConfigurationException("TransactionKey cannot be null. Use TransitService.GenerateTransactionKey(...) to generate a transaction key for the config.");
            }
        }
    }
}
