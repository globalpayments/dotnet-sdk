using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Entities {
    [MapTarget(Target.VAPS | Target.Transit)]
    public enum PinCaptureCapability {
        [Map(Target.VAPS, "0")]
        [Map(Target.Transit, "NOT_SUPPORTED")]
        None,

        [Map(Target.VAPS, "1")]
        [Map(Target.Transit, "UNKNOWN")]
        Unknown,

        [Map(Target.VAPS, "4")]
        [Map(Target.Transit, "4")]
        FourCharacters,

        [Map(Target.VAPS, "5")]
        [Map(Target.Transit, "5")]
        FiveCharacters,

        [Map(Target.VAPS, "6")]
        [Map(Target.Transit, "6")]
        SixCharacters,

        [Map(Target.VAPS, "7")]
        [Map(Target.Transit, "7")]
        SevenCharacters,

        [Map(Target.VAPS, "8")]
        [Map(Target.Transit, "8")]
        EightCharacters,

        [Map(Target.VAPS, "9")]
        [Map(Target.Transit, "9")]
        NineCharacters,

        [Map(Target.VAPS, "A")]
        [Map(Target.Transit, "10")]
        TenCharacters,

        [Map(Target.VAPS, "B")]
        [Map(Target.Transit, "11")]
        ElevenCharacters,

        [Map(Target.VAPS, "C")]
        [Map(Target.Transit, "12")]
        TwelveCharacters
    }
}
