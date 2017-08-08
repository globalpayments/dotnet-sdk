using GlobalPayments.Api.Entities;

namespace GlobalPayments.Api.Terminals {
    public enum ConnectionModes {
        SERIAL,
        TCP_IP,
        SSL_TCP,
        HTTP
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
        
        // Ethernet
        string IpAddress { get; set; }
        string Port { get; set; }

        // Serial
        BaudRate BaudRate { get; set; }
        Parity Parity { get; set; }
        StopBits StopBits { get; set; }
        DataBits DataBits { get; set; }

        int TimeOut { get; set; }

        void Validate();
    }

    public class ConnectionConfig : ITerminalConfiguration {
        public DeviceType DeviceType { get; set; }
        public ConnectionModes ConnectionMode { get; set; }
        public BaudRate BaudRate { get; set; }
        public Parity Parity { get; set; }
        public StopBits StopBits { get; set; }
        public DataBits DataBits { get; set; }
        public int TimeOut { get; set; }
        public string IpAddress { get; set; }
        public string Port { get; set; }
        //public int? DeviceId { get; set; }
        //public int? SiteId { get; set; }
        //public int? LicenseId { get; set; }
        //public string UserName { get; set; }
        //public string Password { get; set; }
        //public string Url { get; set; }

        public ConnectionConfig() {
            this.TimeOut = -1;
        }

        public void Validate() {
            if (ConnectionMode == ConnectionModes.TCP_IP || ConnectionMode == ConnectionModes.HTTP) {
                if (string.IsNullOrEmpty(IpAddress))
                    throw new ApiException("IpAddress is required for TCP or HTTP communication modes.");
                if(string.IsNullOrEmpty(Port))
                    throw new ApiException("Port is required for TCP or HTTP communication modes.");
            }
        }
    }
}
