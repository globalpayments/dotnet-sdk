using System;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Terminals;

namespace GlobalPayments.Api {
    /// <summary>
    /// Configuration for connecting to a payment gateway
    /// </summary>
    public class ServicesConfig {
        /// <summary>
        /// Connection details for your processing gateway
        /// </summary>
        public GatewayConfig GatewayConfig { get; set; }

        // Device Specific
        /// <summary>
        /// Connection details for physical card reader device
        /// </summary>
        public ConnectionConfig DeviceConnectionConfig { get; set; }

        /// Reservation Service Config
        /// <summary>
        /// Connection details for the reservation service
        /// </summary>
        public TableServiceConfig TableServiceConfig { get; set; }

        public PayrollConfig PayrollConfig { get; set; }

        public BoardingConfig BoardingConfig { get; set; }

        public int Timeout {
            set {
                if(GatewayConfig != null)
                    GatewayConfig.Timeout = value;
                if (DeviceConnectionConfig != null)
                    DeviceConnectionConfig.Timeout = value;
                if (TableServiceConfig != null)
                    TableServiceConfig.Timeout = value;
                if (PayrollConfig != null)
                    PayrollConfig.Timeout = value;
            }
        }

        internal void Validate() {
            GatewayConfig?.Validate();
            DeviceConnectionConfig?.Validate();
            TableServiceConfig?.Validate();
            PayrollConfig?.Validate();
        }
    }
}
