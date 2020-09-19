
namespace GlobalPayments.Api.Network.Entities {
    public enum NetworkResponseCodeOrigin {
        Default= 0x00,
        FrontEndProcess= 0x01,
        BackEndProcess= 0x02,
        InternalProcess= 0x03,
        AuthorizationHost= 0x04
    }
}
