using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Network.Entities{
    [MapTarget(Target.NWS)]
    public enum CardHolderAuthenticationMethod {
        [Map(Target.NWS, "0")]
        NotAuthenticated,
        [Map(Target.NWS, "1")]
        PIN,
        [Map(Target.NWS, "2")]
        ElectronicSignature,
        [Map(Target.NWS, "3")]
        Biometrics,
        [Map(Target.NWS, "4")]
        Biographic,
        [Map(Target.NWS, "5")]
        ManualSignatureVerification,
        [Map(Target.NWS, "6")]
        Other,
        [Map(Target.NWS, "9")]
        OnCard_SecurityCode,
        [Map(Target.NWS, "S")]
        Authenticated,
        [Map(Target.NWS, "T")]
        AuthenticationAttempted,
        [Map(Target.NWS, "U")]
        AuthenticationFailed
    }
}
