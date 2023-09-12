using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Entities {
    [MapTarget(Target.GP_API)]
    public enum DisputeStatus {
        [Map(Target.GP_API, "UNDER_REVIEW")]
        UnderReview,

        [Map(Target.GP_API, "WITH_MERCHANT")]
        WithMerchant,

        [Map(Target.GP_API, "CLOSED")]
        Closed,

        //Only for Settlement disputes
        [Map(Target.GP_API, "FUNDED")]
        Funded
    }
}
