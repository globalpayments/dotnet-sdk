using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Entities {
    public enum TokenUsageMode {
        [Map(Target.GP_API, "SINGLE")]
        Single,

        [Map(Target.GP_API, "MULTIPLE")]
        Multiple,
    }
}
