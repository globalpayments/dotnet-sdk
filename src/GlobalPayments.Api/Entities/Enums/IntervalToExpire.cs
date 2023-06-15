using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Entities {
    [MapTarget(Target.GP_API)]
    public enum IntervalToExpire {
        [Map(Target.GP_API, "WEEK")]
        WEEK,

        [Map(Target.GP_API, "DAY")]
        DAY,

        [Map(Target.GP_API, "12_HOURS")]
        TWELVE_HOURS,

        [Map(Target.GP_API, "6_HOURS")]
        SIX_HOURS,

        [Map(Target.GP_API, "3_HOURS")]
        THREE_HOURS,

        [Map(Target.GP_API, "1_HOUR")]
        ONE_HOUR,

        [Map(Target.GP_API, "30_MINUTES")]
        THIRTY_MINUTES,

        [Map(Target.GP_API, "10_MINUTES")]
        TEN_MINUTES,

        [Map(Target.GP_API, "5_MINUTES")]
        FIVE_MINUTES
    }
}