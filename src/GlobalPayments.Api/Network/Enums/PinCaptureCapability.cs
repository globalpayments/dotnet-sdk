using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Entities {
    [MapTarget(Target.VAPS | Target.Transit | Target.NWS)]
    public enum PinCaptureCapability {
        [Map(Target.VAPS, "0")]
        [Map(Target.Transit, "NOT_SUPPORTED")]
        [Map(Target.NWS, "0")]
        None,

        [Map(Target.VAPS, "1")]
        [Map(Target.Transit, "UNKNOWN")]
        [Map(Target.NWS, "1")]
        Unknown,

        [Map(Target.VAPS, "4")]
        [Map(Target.Transit, "4")]
        [Map(Target.NWS, "4")]
        FourCharacters,

        [Map(Target.VAPS, "5")]
        [Map(Target.Transit, "5")]
        [Map(Target.NWS, "5")]
        FiveCharacters,

        [Map(Target.VAPS, "6")]
        [Map(Target.Transit, "6")]
        [Map(Target.NWS, "6")]
        SixCharacters,

        [Map(Target.VAPS, "7")]
        [Map(Target.Transit, "7")]
        [Map(Target.NWS, "7")]
        SevenCharacters,

        [Map(Target.VAPS, "8")]
        [Map(Target.Transit, "8")]
        [Map(Target.NWS, "8")]
        EightCharacters,

        [Map(Target.VAPS, "9")]
        [Map(Target.Transit, "9")]
        [Map(Target.NWS, "9")]
        NineCharacters,

        [Map(Target.VAPS, "A")]
        [Map(Target.Transit, "10")]
        [Map(Target.NWS, "A")]
        TenCharacters,

        [Map(Target.VAPS, "B")]
        [Map(Target.Transit, "11")]
        [Map(Target.NWS, "B")]
        ElevenCharacters,

        [Map(Target.VAPS, "C")]
        [Map(Target.Transit, "12")]
        [Map(Target.NWS, "C")]
        TwelveCharacters
    }
}
