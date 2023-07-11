using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Entities {
    [MapTarget(Target.GP_API)]
    public enum SortDirection {
        [Map(Target.GP_API, "ASC")]
        Ascending,

        [Map(Target.GP_API, "DESC")]
        Descending
    }
}
