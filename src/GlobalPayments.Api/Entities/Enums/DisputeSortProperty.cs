using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Entities {
    [MapTarget(Target.GP_API)]
    public enum DisputeSortProperty {
        [Map(Target.GP_API, "id")]
        Id,

        [Map(Target.GP_API, "arn")]
        ARN,

        [Map(Target.GP_API, "brand")]
        Brand,

        [Map(Target.GP_API, "status")]
        Status,

        [Map(Target.GP_API, "stage")]
        Stage,

        [Map(Target.GP_API, "from_stage_time_created")]
        FromStageTimeCreated,

        [Map(Target.GP_API, "to_stage_time_created")]
        ToStageTimeCreated,

        [Map(Target.GP_API, "adjustment_funding")]
        AdjustmentFunding,

        [Map(Target.GP_API, "from_adjustment_time_created")]
        FromAdjustmentTimeCreated,

        [Map(Target.GP_API, "to_adjustment_time_created")]
        ToAdjustmentTimeCreated
    }
}
