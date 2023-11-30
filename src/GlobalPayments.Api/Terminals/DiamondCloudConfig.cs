using GlobalPayments.Api.Entities;

namespace GlobalPayments.Api.Terminals {
    public class DiamondCloudConfig : ConnectionConfig {
        public string statusUrl { get; set; }
        public string IsvID { get; set; }
        public string SecretKey { get; set; }
        public string Region { get; set; }
        public string PosID { get; set; }
        internal override void ConfigureContainer(ConfiguredServices services) {
            if (string.IsNullOrEmpty(ServiceUrl)) {
                ServiceUrl = (Environment == Environment.PRODUCTION) ?
                    (Region == Entities.Enums.Region.EU.ToString() ?
                    ServiceEndpoints.DIAMOND_CLOUD_PROD_EU : ServiceEndpoints.DIAMOND_CLOUD_PROD) : ServiceEndpoints.DIAMOND_CLOUD_TEST;
            }

            Region = this.Region ?? Entities.Enums.Region.US.ToString();
            base.ConfigureContainer(services);
            
        }

        internal override void Validate() {
            base.Validate();

            if (ConnectionMode == ConnectionModes.DIAMOND_CLOUD) {
                if (string.IsNullOrEmpty(IsvID) || string.IsNullOrEmpty(SecretKey)) {
                    throw new ConfigurationException("ISV ID and secretKey is required for " + ConnectionModes.DIAMOND_CLOUD.ToString());
                }
            }
        }
    }
}
