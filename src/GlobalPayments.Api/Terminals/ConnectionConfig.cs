﻿using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Terminals.PAX;
using GlobalPayments.Api.Terminals.HPA;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Terminals.Genius;
using GlobalPayments.Api.Terminals.UPA;
using GlobalPayments.Api.Terminals.Genius.ServiceConfigs;
using GlobalPayments.Api.Terminals.Diamond;

namespace GlobalPayments.Api.Terminals {
    public enum ConnectionModes {
        SERIAL,
        TCP_IP,
        SSL_TCP,
        HTTP,		
        MIC,
        MEET_IN_THE_CLOUD,       
        DIAMOND_CLOUD
    }

    public enum BaudRate {
        r38400 = 38400,
        r57600 = 57600,
        r19200 = 19200,
        r115200 = 115200
    }

    public enum Parity {
        None = 0,
        Odd,
        Even,
    }
    public enum StopBits {
        One = 1,
        Two
    }
    public enum DataBits {
        Seven = 7,
        Eight = 8
    }

    public interface ITerminalConfiguration {
        ConnectionModes ConnectionMode { get; set; }
        DeviceType DeviceType { get; set; }
        IRequestIdProvider RequestIdProvider { get; set; }

        // Ethernet
        string IpAddress { get; set; }
        string Port { get; set; }

        // Serial
        BaudRate BaudRate { get; set; }
        Parity Parity { get; set; }
        StopBits StopBits { get; set; }
        DataBits DataBits { get; set; }

        // Timeout
        int Timeout { get; set; }

        // Associated Gateway
        GatewayConfig GatewayConfig { get; set; }
    }

    public class ConnectionConfig : Configuration, ITerminalConfiguration {
        public DeviceType DeviceType { get; set; }
        public ConnectionModes ConnectionMode { get; set; }
        public BaudRate BaudRate { get; set; }
        public Parity Parity { get; set; }
        public StopBits StopBits { get; set; }
        public DataBits DataBits { get; set; }
        public string IpAddress { get; set; }
        public string Port { get; set; }
        public IRequestIdProvider RequestIdProvider { get; set; }
        public GatewayConfig GatewayConfig { get; set; }
        public MitcConfig GeniusMitcConfig { get; set; }

        public ConnectionConfig() {
            Timeout = -1;
        }

        internal override void ConfigureContainer(ConfiguredServices services) {
            switch (DeviceType) {
                case DeviceType.PAX_DEVICE:                
                    services.DeviceController = new PaxController(this);
                    break;
                case DeviceType.HPA_ISC250:
                case DeviceType.HPA_LANE3000:
                    services.DeviceController = new HpaController(this);
                    break;
                //case DeviceType.GENIUS:
                //services.DeviceController = new GeniusController(this);
                //break;
                case DeviceType.UPA_DEVICE:                
                    services.DeviceController = new UpaController(this);
                    break;
                case DeviceType.PAX_ARIES8:
                case DeviceType.PAX_A80:
                case DeviceType.PAX_A35:
                case DeviceType.PAX_A920:
                case DeviceType.PAX_A77:
                case DeviceType.NEXGO_N5:
                    services.DeviceController = new DiamondController(this as DiamondCloudConfig);
                    break;
                case DeviceType.GENIUS_VERIFONE_P400:
                    services.DeviceController = new GeniusController(this);
                    break;
                default:
                    break;
            }
        }

        internal override void Validate() {
            base.Validate();

            if (ConnectionMode == ConnectionModes.TCP_IP || ConnectionMode == ConnectionModes.HTTP) {
                if (string.IsNullOrEmpty(IpAddress))
                    throw new ApiException("IpAddress is required for TCP or HTTP communication modes.");
                if(string.IsNullOrEmpty(Port))
                    throw new ApiException("Port is required for TCP or HTTP communication modes.");
            }
            else if(ConnectionMode == ConnectionModes.MEET_IN_THE_CLOUD)
            {
                if(this.GeniusMitcConfig== null)
                {
                    throw new ConfigurationException("meetInTheCloudConfig object is required for this connection method");
                }
            }
        }
    }
}
