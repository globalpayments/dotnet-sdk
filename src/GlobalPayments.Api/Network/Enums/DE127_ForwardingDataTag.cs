using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Network.Entities {
    [MapTarget(Target.NWS)]
    public enum DE127_ForwardingDataTag {
        [Map(Target.NWS, "E3E")]
        E3_EncryptedData,
        [Map(Target.NWS, "HDR")]
        ForwardedHeaderOnly,
        [Map(Target.NWS, "REQ")]
        ForwardedRequest,
        [Map(Target.NWS, "RSP")]
        ForwardedResponse
    }
}
