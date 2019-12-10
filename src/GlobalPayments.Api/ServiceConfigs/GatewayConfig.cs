using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Gateways;
using System;

namespace GlobalPayments.Api {
    public class GatewayConfig : Configuration {
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

        /// <summary>
        /// A unique device id to to be passed with each transaction
        /// </summary>
        public string UniqueDeviceId { get; set; }

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

        public string ChallengeNotificationUrl { get; set; }

        public string MerchantContactUrl { get; set; }

        public string MethodNotificationUrl { get; set; }

        public Secure3dVersion? Secure3dVersion { get; set; }

        /// <summary>
        /// The OpenPath Api key for side integration in OpenPath platform
        /// </summary>
        public string OpenPathApiKey { get; set; }

        /// <summary>
        /// Optional: The OpenPath Api URL where the data will be posted for side integration in OpenPath platform
        /// </summary>
        public string OpenPathApiUrl { get; set; } = "https://api-app.openpath.io/v1/globalpayments";


        internal override void ConfigureContainer(ConfiguredServices services) {
            if (!string.IsNullOrEmpty(MerchantId)) {
                if (string.IsNullOrEmpty(ServiceUrl)) {
                    if (Environment.Equals(Entities.Environment.TEST)) {
                        ServiceUrl = ServiceEndpoints.GLOBAL_ECOM_TEST;
                    }
                    else ServiceUrl = ServiceEndpoints.GLOBAL_ECOM_PRODUCTION;
                }

                var gateway = new RealexConnector {
                    AccountId = AccountId,
                    Channel = Channel,
                    MerchantId = MerchantId,
                    RebatePassword = RebatePassword,
                    RefundPassword = RefundPassword,
                    SharedSecret = SharedSecret,
                    Timeout = Timeout,
                    ServiceUrl = ServiceUrl,
                    HostedPaymentConfig = HostedPaymentConfig,
                    OpenPathApiKey = OpenPathApiKey,
                    OpenPathApiUrl = OpenPathApiUrl
                };
                services.GatewayConnector = gateway;
                services.RecurringConnector = gateway;

                var reportingService = new DataServicesConnector {
                    ClientId = DataClientId,
                    ClientSecret = DataClientSecret,
                    UserId = DataClientUserId,
                    ServiceUrl = DataClientSeviceUrl ?? "https://globalpay-test.apigee.net/apis/reporting/",
                    Timeout = Timeout
                };
                services.ReportingService = reportingService;

                // set default
                if (Secure3dVersion == null) {
                    Secure3dVersion = Entities.Secure3dVersion.One;
                }

                // secure 3d v1
                if (Secure3dVersion.Equals(Entities.Secure3dVersion.One) || Secure3dVersion.Equals(Entities.Secure3dVersion.Any)) {
                    services.SetSecure3dProvider(Entities.Secure3dVersion.One, gateway);
                }

                // secure 3d v2
                if (Secure3dVersion.Equals(Entities.Secure3dVersion.Two) || Secure3dVersion.Equals(Entities.Secure3dVersion.Any)) {
                    Gp3DSProvider secure3d2 = new Gp3DSProvider {
                        MerchantId = MerchantId,
                        AccountId = AccountId,
                        SharedSecret = SharedSecret,
                        ServiceUrl = Environment.Equals(Entities.Environment.TEST) ? ServiceEndpoints.THREE_DS_AUTH_TEST : ServiceEndpoints.THREE_DS_AUTH_PRODUCTION,
                        MerchantContactUrl = MerchantContactUrl,
                        MethodNotificationUrl = MethodNotificationUrl,
                        ChallengeNotificationUrl = ChallengeNotificationUrl,
                        Timeout = Timeout
                        //secure3d2.EnableLogging = EnableLogging
                    };  

                    services.SetSecure3dProvider(Entities.Secure3dVersion.Two, secure3d2);
                }
            }
            else {
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
                    OpenPathApiKey = OpenPathApiKey,
                    OpenPathApiUrl = OpenPathApiUrl
                };
                services.GatewayConnector = gateway;

                if (!string.IsNullOrEmpty(DataClientId)) {
                    services.ReportingService = new DataServicesConnector {
                        ClientId = DataClientId,
                        ClientSecret = DataClientSecret,
                        UserId = DataClientUserId,
                        ServiceUrl = DataClientSeviceUrl ?? "https://globalpay-test.apigee.net/apis/reporting/",
                        Timeout = Timeout
                    };
                }
                else services.ReportingService = gateway;

                var payplan = new PayPlanConnector {
                    SecretApiKey = SecretApiKey,
                    Timeout = Timeout,
                    ServiceUrl = ServiceUrl + getPayPlanEndpoint(SecretApiKey)
                };
                services.RecurringConnector = payplan;
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

            // realex
            if (!string.IsNullOrEmpty(MerchantId) || !string.IsNullOrEmpty(SharedSecret)) {
                if (string.IsNullOrEmpty(MerchantId))
                    throw new ConfigurationException("MerchantId is required for this configuration.");
                else if (string.IsNullOrEmpty(SharedSecret))
                    throw new ConfigurationException("SharedSecret is required for this configuration.");
            }

            // data client
            if (!string.IsNullOrEmpty(DataClientId) || !string.IsNullOrEmpty(DataClientSecret)) {
                if (string.IsNullOrEmpty(DataClientId) || string.IsNullOrEmpty(DataClientSecret))
                    throw new ConfigurationException("Both \"DataClientID\" and \"DataClientSecret\" are required for data client services.");
                if (string.IsNullOrEmpty(DataClientUserId))
                    throw new ConfigurationException("DataClientUserId required for data client services.");
            }

            // secure 3d
            if (Secure3dVersion != null) {
                // ensure we have the fields we need
                if (Secure3dVersion.Equals(Entities.Secure3dVersion.Two) || Secure3dVersion.Equals(Entities.Secure3dVersion.Any)) {
                    if (string.IsNullOrEmpty(ChallengeNotificationUrl)) {
                        throw new ConfigurationException("The challenge notification URL is required for 3DS v2 processing.");
                    }

                    if (string.IsNullOrEmpty(MethodNotificationUrl)) {
                        throw new ConfigurationException("The method notification URL is required for 3DS v2 processing.");
                    }
                }
            }
        }

        private string getPayPlanEndpoint(string key) {
            if (string.IsNullOrEmpty(key) || key.ToLower().Contains("cert")) {
                return "/Portico.PayPlan.v2/";
            }
            return "/PayPlan.v2/";
        }
    }
}
