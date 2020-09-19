
namespace GlobalPayments.Api.Network.Entities {
    public enum ProtocolType {
        NotSpecified = 0x00,
        TCP_IP = 0x01,
        UDP_IP = 0x02,
        X25 = 0x03,
        SNA = 0x04,
        Async = 0x05,
        /* TODO:
        For asynchronous protocol two options exist, without link level and with link level support
        (protocol types 5 and 7 respectively). Use protocol type value of 7 when type message value is
        35 or 37. Use protocol type value of 5 when type message value is 01. For other type message
        values consult with your Heartland representative.
        */
        Bisync_3270 = 0x06
    }
}
