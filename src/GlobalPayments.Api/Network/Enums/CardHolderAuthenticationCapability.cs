using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Entities {
    [MapTarget(Target.VAPS | Target.Transit)]
    public enum CardHolderAuthenticationCapability {
        [Map(Target.VAPS, "0")]
		[Map(Target.Transit, "NO_CAPABILITY")]
        None,

        [Map(Target.VAPS, "1")]
		[Map(Target.Transit, "PIN_ENTRY")]
        PIN,

        [Map(Target.VAPS, "2")]
		[Map(Target.Transit, "SIGNATURE_ANALYSIS")]
        ElectronicSignature,

        [Map(Target.VAPS, "3")]
        Biometrics,

        [Map(Target.VAPS, "4")]
        Biographic,

        [Map(Target.VAPS, "5")]
        [Map(Target.Transit, "SIGNATURE_ANALYSIS_INOPERATIVE")]
        ElectronicAuthenticationInoperable,

        [Map(Target.VAPS, "6")]
		[Map(Target.Transit, "OTHER")]
        Other,

        [Map(Target.VAPS, "9")]
        OnCardSecurityCode,

        [Map(Target.VAPS, "S")]
        ElectronicAuthentication,

        [Map(Target.Transit, "UNKNOWN")]
        Unknown,

        [Map(Target.Transit, "MPOS_SOFTWARE_BASED_PIN_ENTRY_CAPABILITY")]
        PinOnGlass,
    }
}
