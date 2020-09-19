using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Network.Entities {
    [MapTarget(Target.NWS)]
    public enum FallbackCode {
        [Map(Target.NWS, "00")]
        None,
        [Map(Target.NWS, "08")]
        CreditAdjustment,
        [Map(Target.NWS, "30")]
        Referral,
        [Map(Target.NWS, "48")]
        Received_SystemMalfunction,
        [Map(Target.NWS, "68")]
        CouldNotCommunicateWithHost,
        [Map(Target.NWS, "88")]
        Received_IssuerTimeout,
        [Map(Target.NWS, "98")]
        Received_IssuerUnavailable
    }
}
