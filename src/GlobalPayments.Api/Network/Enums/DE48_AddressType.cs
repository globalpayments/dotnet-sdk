using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Network.Entities {
    [MapTarget(Target.NWS)]
    public enum DE48_AddressType {
        [Map(Target.NWS, "0")]
        StreetAddress,
        [Map(Target.NWS, "1")]
        AddressVerification,
        [Map(Target.NWS, "2")]
        PhoneNumber,
        [Map(Target.NWS, "3")]
        Email,
        [Map(Target.NWS, "4")]
        PrefixedUrl,
        [Map(Target.NWS, "5")]
        AddressVerification_Numeric
    }
}
