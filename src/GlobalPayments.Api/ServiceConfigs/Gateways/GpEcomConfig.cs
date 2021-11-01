﻿using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Gateways;

namespace GlobalPayments.Api {
    public class GpEcomConfig : GatewayConfig {
        #region Gateway
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
        #endregion

        #region Secure 3D
        public string ChallengeNotificationUrl { get; set; }

        public string MerchantContactUrl { get; set; }

        public string MethodNotificationUrl { get; set; }

        public Secure3dVersion? Secure3dVersion { get; set; }
        #endregion

        public GpEcomConfig() : base(GatewayProvider.GP_Ecom) {
        }

        internal override void ConfigureContainer(ConfiguredServices services) {
            base.ConfigureContainer(services);

            if (string.IsNullOrEmpty(ServiceUrl)) {
                if (Environment.Equals(Entities.Environment.TEST)) {
                    ServiceUrl = ServiceEndpoints.GLOBAL_ECOM_TEST;
                }
                else ServiceUrl = ServiceEndpoints.GLOBAL_ECOM_PRODUCTION;
            }

            var gateway = new GpEcomConnector {
                AccountId = AccountId,
                Channel = Channel,
                MerchantId = MerchantId,
                RebatePassword = RebatePassword,
                RefundPassword = RefundPassword,
                SharedSecret = SharedSecret,
                Timeout = Timeout,
                ServiceUrl = ServiceUrl,
                HostedPaymentConfig = HostedPaymentConfig,
                RequestLogger = RequestLogger,
                WebProxy = WebProxy
            };
            services.GatewayConnector = gateway;
            services.RecurringConnector = gateway;

            // set reporting gateway
            if (!UseDataReportingService) {
                services.ReportingService = gateway;
            }

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
                    Timeout = Timeout,
                    RequestLogger = RequestLogger,
                    WebProxy = WebProxy
                    //secure3d2.EnableLogging = EnableLogging
                };

                services.SetSecure3dProvider(Entities.Secure3dVersion.Two, secure3d2);
            }
        }

        internal override void Validate() {
            base.Validate();

            if (string.IsNullOrEmpty(MerchantId)) {
                throw new ConfigurationException("MerchantId is required for this configuration.");
            }
            else if (string.IsNullOrEmpty(SharedSecret)) {
                throw new ConfigurationException("SharedSecret is required for this configuration.");
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
    }
}
