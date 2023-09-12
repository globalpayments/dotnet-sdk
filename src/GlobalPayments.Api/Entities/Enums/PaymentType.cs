using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Entities {
    [MapTarget(Target.GP_API)]
    public enum PaymentType {
        [Map(Target.GP_API, "REFUND")]
        Refund,

        [Map(Target.GP_API, "SALE")]
        Sale
    }
}