using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Gateways;

namespace GlobalPayments.Api {
    public abstract class GatewayConfig : Configuration {
        public AcceptorConfig AcceptorConfig { get; set; }
        protected GatewayProvider GatewayProvider { get; set; }
        /// <summary>
        /// Determines wether to use the data reporting service or not
        /// </summary>
        public bool UseDataReportingService { get; set; } = false;

        #region GP Data Services
        /// <summary>
        /// Client Id for Global Payments Data Services
        /// </summary>
        public string DataClientId { get; set; }

        /// <summary>
        /// Client Secret for Global Payments Data Services
        /// </summary>
        public string DataClientSecret { get; set; }

        /// <summary>
        /// The UserId for the Global Payment Data Services
        /// </summary>
        public string DataClientUserId { get; set; }

        /// <summary>
        /// The Url of the Global Data Service
        /// </summary>
        public string DataClientSeviceUrl { get; set; }
        #endregion

        internal GatewayConfig(GatewayProvider provider) {
            GatewayProvider = provider;
        }

        internal override void ConfigureContainer(ConfiguredServices services) {
            var reportingService = new DataServicesConnector {
                ClientId = DataClientId,
                ClientSecret = DataClientSecret,
                UserId = DataClientUserId,
                ServiceUrl = DataClientSeviceUrl ?? "https://globalpay-test.apigee.net/apis/reporting/",
                Timeout = Timeout
            };
            services.ReportingService = reportingService;
        }

        internal override void Validate() {
            base.Validate();

            // data client
            if (!string.IsNullOrEmpty(DataClientId) || !string.IsNullOrEmpty(DataClientSecret)) {
                if (string.IsNullOrEmpty(DataClientId) || string.IsNullOrEmpty(DataClientSecret)) {
                    throw new ConfigurationException("Both \"DataClientID\" and \"DataClientSecret\" are required for data client services.");
                }
                if (string.IsNullOrEmpty(DataClientUserId)) {
                    throw new ConfigurationException("DataClientUserId required for data client services.");
                }
            }
        }
    }
}
