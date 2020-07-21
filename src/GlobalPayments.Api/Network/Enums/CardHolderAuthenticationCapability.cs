using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Entities {
    [MapTarget(Target.VAPS | Target.Transit | Target.NWS)]
    public enum CardHolderAuthenticationCapability {
        [Map(Target.VAPS, "0")]
		[Map(Target.Transit, "NO_CAPABILITY")]
        [Map(Target.NWS, "0")]
        None,

        [Map(Target.VAPS, "1")]
		[Map(Target.Transit, "PIN_ENTRY")]
        [Map(Target.NWS, "1")]
        PIN,

        [Map(Target.VAPS, "2")]
		[Map(Target.Transit, "SIGNATURE_ANALYSIS")]
        [Map(Target.NWS, "2")]
        ElectronicSignature,

        [Map(Target.VAPS, "3")]
        [Map(Target.NWS, "3")]
        Biometrics,

        [Map(Target.VAPS, "4")]
        [Map(Target.NWS, "4")]
        Biographic,

        [Map(Target.VAPS, "5")]
        [Map(Target.NWS, "5")]
        [Map(Target.Transit, "SIGNATURE_ANALYSIS_INOPERATIVE")]
        ElectronicAuthenticationInoperable,

        [Map(Target.VAPS, "6")]
        [Map(Target.NWS, "6")]
        [Map(Target.Transit, "OTHER")]
        Other,

        [Map(Target.VAPS, "9")]
        [Map(Target.NWS, "9")]
        OnCardSecurityCode,

        [Map(Target.VAPS, "S")]
        [Map(Target.NWS, "S")]
        ElectronicAuthentication,

        [Map(Target.Transit, "UNKNOWN")]
        Unknown,

        [Map(Target.Transit, "MPOS_SOFTWARE_BASED_PIN_ENTRY_CAPABILITY")]
        PinOnGlass,
    }
}
