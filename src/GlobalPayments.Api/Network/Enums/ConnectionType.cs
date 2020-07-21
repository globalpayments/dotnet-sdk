
namespace GlobalPayments.Api.Network.Entities {
    public enum ConnectionType {
        NotSpecified = 0x00,
        Service800 = 0x01,
        LeasedLine = 0x02,
        Connect950 = 0x03,
        DirectDial = 0x04,
        VSAT = 0x05,
        ISDN = 0x06,
        eCommerce = 0x07,
        FrameRelay = 0x08,
        FixedWireless = 0x09,
        MobileWireless = 0x0A,
        BlackBox_Presidia = 0x19,
        TNS_Internet = 0x30,
        Datawire_Internet = 0x31,
        Echosat = 0x32,
        Accel = 0x33,
        MobileWireless_2 = 0x34,
        Tech_Pilot = 0x35,
        Hughes_VSAT_Broadband = 0x36,
        Hughes_DSL_Broadband = 0x37,
        EchoSat_Smartlink = 0x41,
        MPLS = 0x42,
        SSL_Gateway = 0x43,
        Native_SSL = 0x44,
        Sagenet = 0x45,
        Cybera = 0x46
    }
}
