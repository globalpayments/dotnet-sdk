using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Entities {
    [MapTarget(Target.GP_API)]
    public enum DepositSortProperty {
        [Map(Target.GP_API, "TIME_CREATED")]
        TimeCreated,

        [Map(Target.GP_API, "STATUS")]
        Status,

        [Map(Target.GP_API, "TYPE")]
        Type,
        
        [Map(Target.GP_API, "DEPOSIT_ID")]
        DepositId
    }
}
