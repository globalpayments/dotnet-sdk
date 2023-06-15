using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Entities {
    [MapTarget(Target.GP_API)]
    public enum Language {
        [Map(Target.GP_API, "EN")]
        English,

        [Map(Target.GP_API, "ES")]
        Spanish
    }
}