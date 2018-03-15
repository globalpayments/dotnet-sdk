using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Gateways;
using System;

namespace GlobalPayments.Api {
    public class TableServiceConfig : Configuration {
        public TableServiceProviders? TableServiceProvider { get; set; }

        internal override void ConfigureContainer(ConfiguredServices services) {
            if (TableServiceProvider == TableServiceProviders.FreshTxt) {
                services.TableServiceConnector = new TableServiceConnector {
                    ServiceUrl = "https://www.freshtxt.com/api31/",
                    Timeout = Timeout
                };
            }
        }

        internal override void Validate() {
            base.Validate();

            if (!TableServiceProvider.HasValue)
                throw new ConfigurationException("A Table Service Provider must be specified.");
        }
    }
}
