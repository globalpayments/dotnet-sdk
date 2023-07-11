using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Entities {
    [MapTarget(Target.GP_API)]
    public enum TransactionStatus {
        [Map(Target.GP_API, "INITIATED")]
        Initiated,

        [Map(Target.GP_API, "AUTHENTICATED")]
        Authenticated,

        [Map(Target.GP_API, "PENDING")]
        Pending,

        [Map(Target.GP_API, "DECLINED")]
        Declined,

        [Map(Target.GP_API, "PREAUTHORIZED")]
        Preauthorized,

        [Map(Target.GP_API, "CAPTURED")]
        Captured,

        [Map(Target.GP_API, "BATCHED")]
        Batched,

        [Map(Target.GP_API, "REVERSED")]
        Reversed,

        [Map(Target.GP_API, "FUNDED")]
        Funded,

        [Map(Target.GP_API, "REJECTED")]
        Rejected
    }
}
