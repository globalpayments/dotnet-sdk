using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Network.Entities {
    [MapTarget(Target.NWS)]
    public enum DE48_KeyManagementDataCode {
        [Map(Target.NWS, "0")]
        NoKey,
        [Map(Target.NWS, "3")]
        DerivedUniqueKeyPerTransaction_DUKPT,
        [Map(Target.NWS, "4")]
        ANSI,
        [Map(Target.NWS, "5")]
        PinPadCharacter
    }
}
