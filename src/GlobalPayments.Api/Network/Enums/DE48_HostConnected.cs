using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Network.Entities {
    [MapTarget(Target.NWS)]
    public enum DE48_HostConnected {
        [Map(Target.NWS, "0")]
        Unavailable,
        [Map(Target.NWS, "1")]
        PrimaryHost,
        [Map(Target.NWS, "2")]
        SecondaryHost

    }
}
