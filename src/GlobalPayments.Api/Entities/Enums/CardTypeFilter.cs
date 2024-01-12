using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Entities.Enums {
    public enum CardTypeFilter {
        [Map(Target.UPA, "GIFT")]
        GIFT,
        [Map(Target.UPA, "VISA")]
        VISA,
        [Map(Target.UPA, "MC")]
        MC,
        [Map(Target.UPA, "AMEX")]
        AMEX,
        [Map(Target.UPA, "DISCOVER")]
        DISCOVER
    }
}
