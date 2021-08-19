using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Gateways;

namespace GlobalPayments.Api {
    public class BillPayConfig : Configuration {
        public string ApiKey { get; set; }
        public string MerchantName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        /// <summary>
        /// Indicates if the merchant hosts Bill Records in the BillPay system
        /// </summary>
        public bool UseBillRecordlookup { get; set; }

        internal override void ConfigureContainer(ConfiguredServices services) {
            var gateway = new BillPayProvider() {
                Credentials = new Entities.Billing.Credentials {
                    UserName = Username,
                    Password = Password,
                    ApiKey = ApiKey,
                    MerchantName = MerchantName
                },
                ServiceUrl = ServiceUrl ?? ServiceEndpoints.BILLPAY_PRODUCTION,
                Timeout = Timeout,
                IsBillDataHosted = UseBillRecordlookup,
                RequestLogger = RequestLogger,
                WebProxy = WebProxy
            };

            services.GatewayConnector = gateway;
            services.BillingProvider = gateway;
            services.RecurringConnector = gateway;
            services.ReportingService = gateway;
        }

        internal override void Validate() {
            base.Validate();

            if ((string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password)) && string.IsNullOrWhiteSpace(ApiKey)) {
                throw new ConfigurationException("Login Credentials or an API Key is required.");
            }

            if ((!string.IsNullOrWhiteSpace(Username) || !string.IsNullOrWhiteSpace(Password)) && !string.IsNullOrWhiteSpace(ApiKey)) {
                throw new ConfigurationException("Cannot provide both Login Credentials and an API key.");
            }

            if (string.IsNullOrWhiteSpace(ApiKey)) {
                if (string.IsNullOrEmpty(Username)) {
                    throw new ConfigurationException("UserName is missing.");
                }

                if (Username.Trim().Length > 50) {
                    throw new ConfigurationException("UserName must be 50 characters or less.");
                }

                if (string.IsNullOrEmpty(Password)) {
                    throw new ConfigurationException("Password is missing.");
                }

                if (Password.Trim().Length > 50) {
                    throw new ConfigurationException("Password must be 50 characters or less.");
                }
            }

            if (string.IsNullOrWhiteSpace(MerchantName)) {
                throw new ConfigurationException("MerchantName is required");
            }

            #region Validate endpoint

            if (ServiceUrl != ServiceEndpoints.BILLPAY_CERTIFICATION
                && ServiceUrl != ServiceEndpoints.BILLPAY_PRODUCTION
                && ServiceUrl != ServiceEndpoints.BILLPAY_TEST
                && !ServiceUrl.Contains("localhost"))
            {
                throw new ConfigurationException("Please use one of the pre-defined BillPay service urls.");
            }

            #endregion
        }
    }
}
