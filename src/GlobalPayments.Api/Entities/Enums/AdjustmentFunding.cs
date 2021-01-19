using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Entities {
    [MapTarget(Target.GP_API)]
    public enum AdjustmentFunding {
        [Map(Target.GP_API, "CREDIT")]
        Credit,

        [Map(Target.GP_API, "DEBIT")]
        Debit,
    }
}
