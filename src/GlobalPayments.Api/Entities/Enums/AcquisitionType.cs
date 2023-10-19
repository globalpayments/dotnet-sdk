using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Entities.Enums {
    public enum AcquisitionType {
        [Map(Target.UPA, "None")]
        None,
        [Map(Target.UPA, "Contact")]
        Contact,
        [Map(Target.UPA, "Contactless")]
        Contactless,
        [Map(Target.UPA, "Swipe")]
        Swipe,
        [Map(Target.UPA, "Manual")]
        Manual,
        [Map(Target.UPA, "Scan")]
        Scan,
        [Map(Target.UPA, "Insert")]
        Insert,
        [Map(Target.UPA, "Tap")]
        Tap
    }
}
