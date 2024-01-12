using System;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Gateways;

namespace GlobalPayments.Api {
    public class PorticoConfig : GatewayConfig {
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

        /// <summary>
        /// A unique device id to to be passed with each transaction
        /// </summary>
        public string UniqueDeviceId { get; set; }

        /// <summary>
        /// ProPay CertificationStr Value
        /// </summary>
        public string CertificationStr { get; set; }

        /// <summary>
        /// ProPay TerminalID Value
        /// </summary>
        public string TerminalID { get; set; }

        /// <summary>
        /// ProPay X509 Certificate Location
        /// </summary>
        public string X509CertificatePath { get; set; }

        /// <summary>
        /// ProPay X509 Certificate Base64 String (Optional: Can be used instead of X509CertificatePath)
        /// </summary>
        public string X509CertificateBase64String { get; set; }
        /// <summary>
        /// Name and Version of the SDK used for integration, where applicable.  Expected for users of the Heartland SDK.
        /// </summary>
        public string SDKNameVersion { get; set; }

        /// <summary>
        /// If true (default), use the US ProPay endpoints.
        /// If false, use the Canadian ProPay endpoints
        /// </summary>
        public bool ProPayUS { get; set; } = true;
        public bool? IsSafDataSupported { get; set; } = null;
        private string PayPlanEndpoint {
            get {
                if (
                    (!String.IsNullOrEmpty(SecretApiKey) && SecretApiKey.ToLower().Contains("cert")) ||
                    (String.IsNullOrEmpty(SecretApiKey) && Environment == Entities.Environment.TEST)
                ) {
                    return "/Portico.PayPlan.v2/";
                }
                return "/PayPlan.v2/";
            }
        }

        public PorticoConfig() : base(GatewayProvider.Portico) {
        }

        internal override void ConfigureContainer(ConfiguredServices services) {
            base.ConfigureContainer(services);

            if (string.IsNullOrEmpty(ServiceUrl)) {
                if (Environment.Equals(Entities.Environment.TEST)) {
                    ServiceUrl = ServiceEndpoints.PORTICO_TEST;
                }
                else ServiceUrl = ServiceEndpoints.PORTICO_PRODUCTION;
            }

            var gateway = new PorticoConnector {
                SiteId = SiteId,
                LicenseId = LicenseId,
                DeviceId = DeviceId,
                Username = Username,
                Password = Password,
                SecretApiKey = SecretApiKey,
                DeveloperId = DeveloperId,
                VersionNumber = VersionNumber,
                Timeout = Timeout,
                ServiceUrl = ServiceUrl + "/Hps.Exchange.PosGateway/PosGatewayService.asmx",
                UniqueDeviceId = UniqueDeviceId,
                RequestLogger = RequestLogger,
                WebProxy = WebProxy,
                SDKNameVersion = SDKNameVersion,
                IsSafDataSupported = IsSafDataSupported,
            };
            services.GatewayConnector = gateway;

            // no data connector
            if (string.IsNullOrEmpty(DataClientId)) {
                services.ReportingService = gateway;
            }

            var payplan = new PayPlanConnector {
                SecretApiKey = SecretApiKey,
                Timeout = Timeout,
                ServiceUrl = ServiceUrl + PayPlanEndpoint,
                RequestLogger = RequestLogger,
                WebProxy = WebProxy
            };
            services.RecurringConnector = payplan;

            // ProPay Connector
            if (CertificationStr != null && CertificationStr.Length > 0) {
                if (Environment == Entities.Environment.TEST) {
                    ServiceUrl = ProPayUS ? ServiceEndpoints.PROPAY_TEST : ServiceEndpoints.PROPAY_TEST_CANADIAN;
                }
                else {
                    ServiceUrl = ProPayUS ? ServiceEndpoints.PROPAY_PRODUCTION : ServiceEndpoints.PROPAY_PRODUCTION_CANADIAN;
                }

                var payFac = new ProPayConnector() {
                    CertStr = CertificationStr,
                    TermID = TerminalID,
                    Timeout = Timeout,
                    ServiceUrl = ServiceUrl,
                    X509CertPath = X509CertificatePath,
                    X509Base64String = X509CertificateBase64String,
                    RequestLogger = RequestLogger,
                    WebProxy = WebProxy
                };

                services.PayFacProvider = payFac;
            }
        }

        internal override void Validate() {
            base.Validate();

            // portico api key
            if (!string.IsNullOrEmpty(SecretApiKey)) {
                if (SiteId != default(int) || LicenseId != default(int) || DeviceId != default(int) || !string.IsNullOrEmpty(Username) || !string.IsNullOrEmpty(Password)) {
                    throw new ConfigurationException("Configuration contains both secret api key and legacy credentials. These are mutually exclusive.");
                }
            }

            // portico legacy
            if (SiteId != default(int) || LicenseId != default(int) || DeviceId != default(int) || !string.IsNullOrEmpty(Username) || !string.IsNullOrEmpty(Password)) {
                if (!(SiteId != default(int) && LicenseId != default(int) && DeviceId != default(int) && !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password)))
                    throw new ConfigurationException("Site, License, Device, Username and Password should all have a values for this configuration.");
            }
        }
    }
}
