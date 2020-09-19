using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Network.Entities {
    [MapTarget(Target.NWS)]
    public enum DE22_CardDataInputMode {
        [Map(Target.NWS, "0")]
        Unspecified,
        [Map(Target.NWS, "1")]
        Manual,
        [Map(Target.NWS, "2")]
        MagStripe,
        [Map(Target.NWS, "3")]
        BarCode,
        [Map(Target.NWS, "4")]
        OCR,
        [Map(Target.NWS, "5")]
        MagStripe_Fallback,
        [Map(Target.NWS, "6")]
        KeyEntry,
        [Map(Target.NWS, "A")]
        ContactlessMsd,
        [Map(Target.NWS, "B")]
        UnalteredTrackData,
        [Map(Target.NWS, "C")]
        ContactEmv,
        [Map(Target.NWS, "D")]
        ContactlessEmv,
        [Map(Target.NWS, "S")]
        Credential_On_File,
        [Map(Target.NWS, "T")]
        ECommerce,
        [Map(Target.NWS, "U")]
        Secure_ECommerce
    }
}
