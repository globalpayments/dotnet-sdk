using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Network.Entities {
    [MapTarget(Target.NWS)]
    public enum DE22_CardPresence {
        [Map(Target.NWS, "0")]
        CardNotPresent,
        [Map(Target.NWS, "1")]
        CardPresent,
        [Map(Target.NWS, "8")]
        CardOnFile
    }
}
