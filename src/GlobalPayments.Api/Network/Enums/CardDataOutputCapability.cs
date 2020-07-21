using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Entities {
    [MapTarget(Target.VAPS | Target.Transit | Target.NWS)]
    public enum CardDataOutputCapability {
        [Map(Target.VAPS, "0")]
		[Map(Target.Transit, "OTHER")]
        [Map(Target.NWS, "0")]
        Unknown,

        [Map(Target.VAPS, "1")]
		[Map(Target.Transit, "NONE")]
        [Map(Target.NWS, "1")]
        None,

        [Map(Target.VAPS, "2")]
		[Map(Target.Transit, "MAGNETIC_STRIPE_WRITE")]
        [Map(Target.NWS, "2")]
        MagStripe_Write,

        [Map(Target.VAPS, "3")]
		[Map(Target.Transit, "ICC")]
        [Map(Target.NWS, "3")]
        ICC
    }
}
