using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Network.Entities {
    [MapTarget(Target.NWS)]
    public enum DE48_AddressUsage {
        [Map(Target.NWS, "0")]
        Business,
        [Map(Target.NWS, "1")]
        Home,
        [Map(Target.NWS, "2")]
        Fax,
        [Map(Target.NWS, "3")]
        Cellular,
        [Map(Target.NWS, "4")]
        Billing,
        [Map(Target.NWS, "5")]
        Shipping,
        [Map(Target.NWS, "6")]
        CustomerServiceCenter,
        [Map(Target.NWS, "7")]
        Daytime,
        [Map(Target.NWS, "8")]
        Evening,
        [Map(Target.NWS, "9")]
        PreviousHome,
        [Map(Target.NWS, "A")]
        Merchant
    }
}
