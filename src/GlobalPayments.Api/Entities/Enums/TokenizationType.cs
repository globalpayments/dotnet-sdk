using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Entities.Enums {
    public enum TokenizationType {
        [Map(Target.NWS, "1")]
        GlobalTokenization,
        [Map(Target.NWS, "2")]
        MerchantTokenization
    }
}
