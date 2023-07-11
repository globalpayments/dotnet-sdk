using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Entities {
    public enum StoredCredentialType {
        OneOff,
        [Map(Target.GP_API, "INSTALLMENT")]
        Installment,
        [Map(Target.GP_API, "RECURRING")]
        Recurring,
        [Map(Target.GP_API, "UNSCHEDULED")]
        Unscheduled,
        [Map(Target.GP_API, "SUBSCRIPTION")]
        Subscription,
        [Map(Target.GP_API, "MAINTAIN_PAYMENT_METHOD")]
        MaintainPaymentMethod,
        [Map(Target.GP_API, "MAINTAIN_PAYMENT_VERIFICATION")]
        MaintainPaymentVerification
    }
}
