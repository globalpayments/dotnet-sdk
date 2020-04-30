using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Entities {
    public enum UCAFIndicator {
        [Map(Target.Transit, "0")]
        NotSupported,

        [Map(Target.Transit, "1")]
        MerchantOnly,

        [Map(Target.Transit, "2")]
        FullyAuthenticated,

        [Map(Target.Transit, "5")]
        IssuerRiskBased,

        [Map(Target.Transit, "6")]
        MerchantRiskBased,

        [Map(Target.Transit, "7")]
        PartialShipmentIncremental
    }
}
