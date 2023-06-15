using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Entities {
    [MapTarget(Target.GP_API)]
    public enum StoredPaymentMethodStatus {
        [Map(Target.GP_API, "ACTIVE")]
        Active
    }
}
