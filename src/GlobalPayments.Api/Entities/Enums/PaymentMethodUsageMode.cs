using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Entities {
    public enum PaymentMethodUsageMode {        
        [Map(Target.GP_API, "SINGLE")]
        Single,

        [Map(Target.GP_API, "MULTIPLE")]
        Multiple
    }
}
