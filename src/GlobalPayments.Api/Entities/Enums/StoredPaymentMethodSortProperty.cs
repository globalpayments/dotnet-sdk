using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Entities {
    [MapTarget(Target.GP_API)]
    public enum StoredPaymentMethodSortProperty {
        [Map(Target.GP_API, "TIME_CREATED")]
        TimeCreated
    }
}
