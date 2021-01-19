using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Entities {
    public enum StoredCredentialReason {
        [Map(Target.GP_API, "INCREMENTAL")]
        Incremental,
        [Map(Target.GP_API, "RESUBMISSION")]
        Resubmission,
        [Map(Target.GP_API, "REAUTHORIZATION")]
        Reauthorization,
        [Map(Target.GP_API, "DELAYED")]
        Delayed,
        [Map(Target.GP_API, "NO_SHOW")]
        NoShow
    }
}
