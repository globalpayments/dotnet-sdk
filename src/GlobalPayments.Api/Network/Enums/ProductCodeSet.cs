using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Network.Entities {
    [MapTarget(Target.NWS)]
    public enum ProductCodeSet {
        [Map(Target.NWS, "0")]
        Heartland,
        [Map(Target.NWS, "1")]
        ClientSpecific,
        [Map(Target.NWS, "2")]
        IssuerSpecific,
        [Map(Target.NWS, "3")]
        Conexxus_3_Digit,
        [Map(Target.NWS, "4")]
        Conexxus_6_Digit,
        [Map(Target.NWS, "5")]
        UCC_12,
        [Map(Target.NWS, "6")]
        GTIN,
        [Map(Target.NWS, "7")]
        Mixed,
        [Map(Target.NWS, "8")]
        ClientSpecificAddendum_1,
        [Map(Target.NWS, "9")]
        ClientSpecificAddendum_2
    }
}
