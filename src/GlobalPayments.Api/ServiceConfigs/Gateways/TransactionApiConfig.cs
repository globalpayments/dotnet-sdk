using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Gateways;
using GlobalPayments.Api.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace GlobalPayments.Api {
    public class TransactionApiConfig : GatewayConfig {
        public AccessTokenInfo AccessTokenInfo { get; set; }
        public string  Region { get; set; }
        public string AccountCredential { get; set; }
        public string AppSecret { get; set; }
        public Dictionary<string, string> DynamicHeaders { get; set; }
        public TransactionApiConfig() : base(GatewayProvider.TransactionApi) { }

        internal override void ConfigureContainer(ConfiguredServices services) {
            if (string.IsNullOrEmpty(ServiceUrl)) {
                if (Environment.Equals(Entities.Environment.TEST))
                    ServiceUrl = ServiceEndpoints.Transaction_API_TEST;
                else
                    ServiceUrl = ServiceEndpoints.Transaction_API_PRODUCTION;
            }

            var gateway = new TransactionApiConnector {
                ServiceUrl = ServiceUrl,
                Timeout = Timeout,
                AccessToken = AccessTokenInfo?.Token,
                RequestLogger = RequestLogger,
                WebProxy = WebProxy,
                DynamicHeaders = DynamicHeaders,
                Region = Region,
                AccountCredential = AccountCredential,
                AppSecret = AppSecret
            };

            services.GatewayConnector = gateway;
            services.ReportingService = gateway;
        }

        internal override void Validate() {
            base.Validate();
        }
    }
}
