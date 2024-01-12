using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Gateways;
using GlobalPayments.Api.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace GlobalPayments.Api {
    public class GpApiConfig : GatewayConfig {
        /// <summary>
        /// GP API app id
        /// </summary>
        public string AppId { get; set; }
        /// <summary>
        /// GP API app key
        /// </summary>
        public string AppKey { get; set; }
        /// <summary>
        /// The time left in seconds before the token expires
        /// </summary>
        public int? SecondsToExpire { get; set; }
        /// <summary>
        /// The time interval set for when the token will expire
        /// </summary>
        public IntervalToExpire? IntervalToExpire { get; set; }
        /// <summary>
        /// Channel
        /// </summary>
        public Channel Channel { get; set; } = Channel.CardNotPresent;
        /// <summary>
        /// Gloabl API language configuration
        /// </summary>
        public Language Language { get; set; } = Language.English;
        /// <summary>
        /// Two letter ISO 3166 country
        /// </summary>
        public string Country { get; set; } = "US";
        /// <summary>
        /// Three letter ISO currency code for use with Card Present device processing
        /// </summary>
        public string DeviceCurrency { get; set; } = "USD";
        /// <summary>
        /// The list of the permissions the integrator want the access token to have.
        /// </summary>
        public string[] Permissions { get; set; }
        /// <summary>
        /// Access token information
        /// </summary>
        public AccessTokenInfo AccessTokenInfo { get; set; }
        /// <summary>
        /// 3DSecure challenge return url
        /// </summary>
        public string ChallengeNotificationUrl { get; set; }
        /// <summary>
        /// 3DSecure method return url
        /// </summary>
        public string MethodNotificationUrl { get; set; }
        /// <summary>
        /// MerchantContactUrl
        /// </summary>
        public string MerchantContactUrl { get; set; }

        public string MerchantId { get; set; }

        public string StatusUrl { get; set; }

        public Dictionary<string, string> DynamicHeaders { get; set; }

        public GpApiConfig() : base(GatewayProvider.GP_API) { }

        internal override void ConfigureContainer(ConfiguredServices services) {
            if (string.IsNullOrEmpty(ServiceUrl)) {
                if (Environment.Equals(Entities.Environment.QA))
                    ServiceUrl = ServiceEndpoints.GP_API_QA;
                else if (Environment.Equals(Entities.Environment.TEST))
                    ServiceUrl = ServiceEndpoints.GP_API_TEST;
                else
                    ServiceUrl = ServiceEndpoints.GP_API_PRODUCTION;
            }

            var gpApiConfig = new GpApiConfig
            {
                AppId = AppId,
                AppKey = AppKey,
                SecondsToExpire = SecondsToExpire,
                IntervalToExpire = IntervalToExpire,
                Channel = Channel,
                Language = Language,
                Country = Country,
                ServiceUrl = ServiceUrl,
                Timeout = Timeout,
                Permissions = Permissions,
                MethodNotificationUrl = MethodNotificationUrl,
                RequestLogger = RequestLogger,
                MerchantContactUrl = MerchantContactUrl,
                ChallengeNotificationUrl = ChallengeNotificationUrl,
                WebProxy = WebProxy,
                DynamicHeaders = DynamicHeaders,
                MerchantId = MerchantId,
                AccessTokenInfo = AccessTokenInfo,
                Environment = Environment,
                StatusUrl = StatusUrl
            };

            var gateway = new GpApiConnector(gpApiConfig);

            gateway.ServiceUrl = ServiceUrl;
            gateway.Environment = Environment;

            services.GatewayConnector = gateway;
            services.ReportingService = gateway;
            services.FraudService = gateway;
            services.FileProcessingService = gateway;
            
            services.SetPayFacProvider(gateway);
            services.SetSecure3dProvider(Secure3dVersion.One, gateway);
            services.SetSecure3dProvider(Secure3dVersion.Two, gateway);
        }

        internal override void Validate() {
            base.Validate();

            if (AccessTokenInfo == null && (AppId == null || AppKey == null))
                throw new ConfigurationException("AccessTokenInfo or AppId and AppKey cannot be null.");
        }
    }
}
