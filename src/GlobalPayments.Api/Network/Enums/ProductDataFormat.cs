using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Network.Entities {
    [MapTarget(Target.NWS)]
    public enum ProductDataFormat {
        [Map(Target.NWS, "0")]
        HeartlandStandardFormat,
        [Map(Target.NWS, "1")]
        ANSI_X9_TG23_Format,
        [Map(Target.NWS, "2")]
        Heartland_ProductCoupon_Format
    }
}
