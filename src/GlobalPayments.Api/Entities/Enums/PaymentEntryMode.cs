using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Entities {
    [MapTarget(Target.GP_API)]
    public enum PaymentEntryMode {
        [Map(Target.GP_API, "CHIP")]
        Chip,

        [Map(Target.GP_API, "CONTACTLESS_CHIP")]
        ContactlessChip,

        [Map(Target.GP_API, "CONTACTLESS_SWIPE")]
        ContactlessSwipe,

        [Map(Target.GP_API, "ECOM")]
        Ecom,

        [Map(Target.GP_API, "IN_APP")]
        InApp,

        [Map(Target.GP_API, "MANUAL")]
        Manual,

        [Map(Target.GP_API, "MOTO")]
        Moto,

        [Map(Target.GP_API, "SWIPE")]
        Swipe
    }
}