using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Entities {
    [MapTarget(Target.VAPS | Target.Transit | Target.NWS)]
    public enum TerminalOutputCapability {
        [Map(Target.VAPS, "0")]
		[Map(Target.Transit, "UNKNOWN")]
        [Map(Target.NWS, "0")]
        Unknown,

        [Map(Target.VAPS, "1")]
		[Map(Target.Transit, "NONE")]
        [Map(Target.NWS, "1")]
        None,

        [Map(Target.VAPS, "2")]
		[Map(Target.Transit, "PRINT_ONLY")]
        [Map(Target.NWS, "2")]
        Printing,

        [Map(Target.VAPS, "3")]
		[Map(Target.Transit, "DISPLAY_ONLY")]
        [Map(Target.NWS, "3")]
        Display,

        [Map(Target.VAPS, "4")]
		[Map(Target.Transit, "PRINT_AND_DISPLAY")]
        [Map(Target.NWS, "4")]
        Printing_Display,

        [Map(Target.VAPS, "9")]
        [Map(Target.NWS, "9")]
        Coupon_Printing
    }
}
