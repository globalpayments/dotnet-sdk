using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Entities {
    [MapTarget(Target.GP_API)]
    public enum Channel {
        [Map(Target.GP_API, "CP")]
        CardPresent,

        [Map(Target.GP_API, "CNP")]
        CardNotPresent
    }
}
