using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Network.Entities {
    [MapTarget(Target.NWS)]
    public enum ServiceLevel {
        [Map(Target.NWS, "F")]
        FullServe,
        [Map(Target.NWS, "N")]
        MiniServe,
        [Map(Target.NWS, "O")]
        Other_NonFuel,
        [Map(Target.NWS, "S")]
        SelfServe,
        [Map(Target.NWS, "X")]
        MaxiServe
    }
}
