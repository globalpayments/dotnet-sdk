using GlobalPayments.Api.Entities;

namespace GlobalPayments.Api {
    public class PayrollConfig : Configuration {
        public string Username { get; set; }
        public string Password { get; set; }
        public string ApiKey { get; set; }

        internal override void ConfigureContainer(ConfiguredServices services) {
            services.PayrollConnector = new Gateways.PayrollConnector {
                Username = Username,
                Password = Password,
                ApiKey = ApiKey,
                ServiceUrl = ServiceUrl,
                Timeout = Timeout
            };
        }

        internal override void Validate() {
            base.Validate();

            if (Username == null || Password == null || ApiKey == null)
                throw new ConfigurationException("Username, Password, and ApiKey cannot be null.");
        }
    }
}
