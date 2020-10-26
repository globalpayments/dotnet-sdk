using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Entities {
    [MapTarget(Target.VAPS | Target.Transit)]
    public enum TerminalOutputCapability {
        [Map(Target.VAPS, "0")]
		[Map(Target.Transit, "UNKNOWN")]
        Unknown,

        [Map(Target.VAPS, "1")]
		[Map(Target.Transit, "NONE")]
        None,

        [Map(Target.VAPS, "2")]
		[Map(Target.Transit, "PRINT_ONLY")]
        Printing,

        [Map(Target.VAPS, "3")]
		[Map(Target.Transit, "DISPLAY_ONLY")]
        Display,

        [Map(Target.VAPS, "4")]
		[Map(Target.Transit, "PRINT_AND_DISPLAY")]
        Printing_Display,

        [Map(Target.VAPS, "9")]
        Coupon_Printing
    }
}
