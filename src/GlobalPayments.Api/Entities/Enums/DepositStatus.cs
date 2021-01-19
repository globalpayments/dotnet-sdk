using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Entities {
    [MapTarget(Target.GP_API)]
    public enum DepositStatus {
        [Map(Target.GP_API, "FUNDED")]
        Funded,

        [Map(Target.GP_API, "SPLIT_FUNDING")]
        SplitFunding,

        [Map(Target.GP_API, "DELAYED")]
        Delayed,

        [Map(Target.GP_API, "RESERVED")]
        Reserved,

        [Map(Target.GP_API, "IRREG")]
        Irregular,

        [Map(Target.GP_API, "RELEASED")]
        Released
    }
}
