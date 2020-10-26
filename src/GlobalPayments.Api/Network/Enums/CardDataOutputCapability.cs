using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Entities {
    [MapTarget(Target.VAPS | Target.Transit)]
    public enum CardDataOutputCapability {
        [Map(Target.VAPS, "0")]
		[Map(Target.Transit, "OTHER")]
        Unknown,

        [Map(Target.VAPS, "1")]
		[Map(Target.Transit, "NONE")]
        None,

        [Map(Target.VAPS, "2")]
		[Map(Target.Transit, "MAGNETIC_STRIPE_WRITE")]
        MagStripe_Write,

        [Map(Target.VAPS, "3")]
		[Map(Target.Transit, "ICC")]
        ICC
    }
}
