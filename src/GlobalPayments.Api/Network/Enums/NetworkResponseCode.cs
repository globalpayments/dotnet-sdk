
namespace GlobalPayments.Api.Network.Entities {
    public enum NetworkResponseCode {
        Success= 0x00,
        FailedConnection= 0x01,
        Timeout= 0x02,
        FormatError_Originator= 0x03,
        StoreAndForward= 0x04,
        UnsupportedTransaction= 0x05,
        UnsupportedServiceProvider= 0x06,
        FormatError_ServiceProvider= 0x07

    }
}
