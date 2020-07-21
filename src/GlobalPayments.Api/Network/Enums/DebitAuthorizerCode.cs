using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Network.Entities {
    [MapTarget(Target.NWS)]
    public enum DebitAuthorizerCode {
        [Map(Target.NWS, "00")]
        NonPinDebitCard,
        [Map(Target.NWS, "02")]
        StarSoutheast,
        [Map(Target.NWS, "03")]
        PULSE,
        [Map(Target.NWS, "04")]
        StarCentral,
        [Map(Target.NWS, "06")]
        StarNortheast,
        [Map(Target.NWS, "07")]
        Culiance,
        [Map(Target.NWS, "08")]
        NYCE,
        [Map(Target.NWS, "10")]
        AFFN_Network,
        [Map(Target.NWS, "12")]
        Interlink,
        [Map(Target.NWS, "13")]
        StarWest,
        [Map(Target.NWS, "16")]
        Maestro,
        [Map(Target.NWS, "21")]
        ACCEL,
        [Map(Target.NWS, "33")]
        Quest_JPM_EBT,
        [Map(Target.NWS, "35")]
        AlaskaOption,
        [Map(Target.NWS, "36")]
        Shazam,
        [Map(Target.NWS, "38")]
        VisaReadyLink,
        [Map(Target.NWS, "42")]
        NationalPaymentCard,
        [Map(Target.NWS, "43")]
        RevolutionMoney,
        [Map(Target.NWS, "44")]
        Visa_PIN_POS,
        [Map(Target.NWS, "59")]
        UnknownAuthorizer,
        [Map(Target.NWS, "34")]
        Conduent_EBT
    }
}
