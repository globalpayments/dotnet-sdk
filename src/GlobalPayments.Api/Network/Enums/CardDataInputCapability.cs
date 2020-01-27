using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Entities {
    [MapTarget(Target.VAPS | Target.Transit)]
    public enum CardDataInputCapability {
        [Map(Target.VAPS, "0")]
		[Map(Target.Transit, "UNKNOWN")]
        Unknown,

        [Map(Target.VAPS, "1")]
		[Map(Target.Transit, "NO_TERMINAL_MANUAL")]
        Manual,

        [Map(Target.VAPS, "2")]
		[Map(Target.Transit, "MAGSTRIPE_READ_ONLY")]
        MagStripe,

        [Map(Target.VAPS, "3")]
        BarCode,

        [Map(Target.VAPS, "4")]
		[Map(Target.Transit, "OCR")]
        OCR,

        [Map(Target.VAPS, "5")]
		[Map(Target.Transit, "ICC_CHIP_READ_ONLY")]
        ContactEmv,

        [Map(Target.VAPS, "6")]
		[Map(Target.Transit, "KEYED_ENTRY_ONLY")]
        KeyEntry,

        [Map(Target.VAPS, "9")]
		[Map(Target.Transit, "ICC_CONTACTLESS_ONLY")]
        ContactlessEmv,

        [Map(Target.VAPS, "A")]
		[Map(Target.Transit, "MAGSTRIPE_CONTACTLESS_ONLY")]
        ContactlessMsd,

        [Map(Target.VAPS, "B")]
		[Map(Target.Transit, "MAGSTRIPE_KEYED_ENTRY_ONLY")]
        MagStripe_KeyEntry,

        [Map(Target.VAPS, "C")]
        ContactEmv_MagStripe_KeyEntry,

        [Map(Target.VAPS, "D")]
		[Map(Target.Transit, "MAGSTRIPE_ICC_ONLY")]
        ContactEmv_MagStripe,

        [Map(Target.VAPS, "E")]
		[Map(Target.Transit, "ICC_KEYED_ENTRY_ONLY")]
        ContactEmv_KeyEntry,

        [Map(Target.VAPS, "F")]
        ContactlessMsd_KeyEntry,

        [Map(Target.VAPS, "G")]
        ContactlessMsd_MagStripe,

        [Map(Target.VAPS, "H")]
        ContactlessMsd_MagStripe_KeyEntry,

        [Map(Target.VAPS, "I")]
        ContactEmv_ContactlessMsd,

        [Map(Target.VAPS, "J")]
        ContactEmv_ContactlessMsd_KeyEntry,

        [Map(Target.VAPS, "K")]
        ContactEmv_ContactlessMsd_MagStripe,

        [Map(Target.VAPS, "L")]
        ContactEmv_ContactlessMsd_MagStripe_KeyEntry,

        [Map(Target.VAPS, "M")]
        ContactlessEmv_KeyEntry,

        [Map(Target.VAPS, "N")]
        ContactlessEmv_MagStripe,

        [Map(Target.VAPS, "O")]
        ContactlessEmv_ContactlessMsd,

        [Map(Target.VAPS, "P")]
        ContactlessEmv_ContactlessMsd_KeyEntry,

        [Map(Target.VAPS, "Q")]
        ContactlessEmv_ContactlessMsd_MagStripe,

        [Map(Target.VAPS, "R")]
        ContactlessEmv_ContactlessMsd_MagStripe_KeyEntry,

        [Map(Target.VAPS, "S")]
		[Map(Target.Transit, "MAGSTRIPE_ICC_KEYED_ENTRY_ONLY")]
        ContactlessEmv_MagStripe_KeyEntry,

        [Map(Target.VAPS, "T")]
		[Map(Target.Transit, "ICC_CHIP_CONTACT_CONTACTLESS")]
        ContactlessEmv_ContactEmv,

        [Map(Target.VAPS, "U")]
        ContactlessEmv_ContactEmv_KeyEntry,

        [Map(Target.VAPS, "V")]
        ContactlessEmv_ContactEmv_MagStripe,

        [Map(Target.VAPS, "W")]
        ContactlessEmv_ContactEmv_MagStripe_KeyEntry,

        [Map(Target.VAPS, "X")]
        ContactlessEmv_ContactEmv_ContactlessMsd,

        [Map(Target.VAPS, "Y")]
        ContactlessEmv_ContactEmv_ContactlessMsd_KeyEntry,

        [Map(Target.VAPS, "Z")]
        ContactlessEmv_ContactEmv_ContactlessMsd_MagStripe,

        [Map(Target.VAPS, "a")]
        ContactlessEmv_ContactEmv_ContactlessMsd_MagStripe_KeyEntry
    }
}
