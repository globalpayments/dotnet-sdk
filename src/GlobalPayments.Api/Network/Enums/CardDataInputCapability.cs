using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Entities {
    [MapTarget(Target.VAPS | Target.Transit | Target.NWS)]
    public enum CardDataInputCapability {
        [Map(Target.VAPS, "0")]
		[Map(Target.Transit, "UNKNOWN")]
        [Map(Target.NWS, "0")]
        Unknown,

        [Map(Target.VAPS, "1")]
		[Map(Target.Transit, "NO_TERMINAL_MANUAL")]
        [Map(Target.NWS, "1")]
        Manual,

        [Map(Target.VAPS, "2")]
		[Map(Target.Transit, "MAGSTRIPE_READ_ONLY")]
        [Map(Target.NWS, "2")]
        MagStripe,

        [Map(Target.VAPS, "3")]
        [Map(Target.NWS, "3")]
        BarCode,

        [Map(Target.VAPS, "4")]
		[Map(Target.Transit, "OCR")]
        [Map(Target.NWS, "4")]
        OCR,

        [Map(Target.VAPS, "5")]
		[Map(Target.Transit, "ICC_CHIP_READ_ONLY")]
        [Map(Target.NWS, "5")]
        ContactEmv,

        [Map(Target.VAPS, "6")]
		[Map(Target.Transit, "KEYED_ENTRY_ONLY")]
        [Map(Target.NWS, "6")]
        KeyEntry,

        [Map(Target.VAPS, "9")]
		[Map(Target.Transit, "ICC_CONTACTLESS_ONLY")]
        [Map(Target.NWS, "9")]
        ContactlessEmv,

        [Map(Target.VAPS, "A")]
		[Map(Target.Transit, "MAGSTRIPE_CONTACTLESS_ONLY")]
        [Map(Target.NWS, "A")]
        ContactlessMsd,

        [Map(Target.VAPS, "B")]
		[Map(Target.Transit, "MAGSTRIPE_KEYED_ENTRY_ONLY")]
        [Map(Target.NWS, "B")]
        MagStripe_KeyEntry,

        [Map(Target.VAPS, "C")]
        [Map(Target.NWS, "C")]
        ContactEmv_MagStripe_KeyEntry,

        [Map(Target.VAPS, "D")]
		[Map(Target.Transit, "MAGSTRIPE_ICC_ONLY")]
        [Map(Target.NWS, "D")]
        ContactEmv_MagStripe,

        [Map(Target.VAPS, "E")]
		[Map(Target.Transit, "ICC_KEYED_ENTRY_ONLY")]
        [Map(Target.NWS, "E")]
        ContactEmv_KeyEntry,

        [Map(Target.VAPS, "F")]
        [Map(Target.NWS, "F")]
        ContactlessMsd_KeyEntry,

        [Map(Target.VAPS, "G")]
        [Map(Target.NWS, "G")]
        ContactlessMsd_MagStripe,

        [Map(Target.VAPS, "H")]
        [Map(Target.NWS, "H")]
        ContactlessMsd_MagStripe_KeyEntry,

        [Map(Target.VAPS, "I")]
        [Map(Target.NWS, "I")]
        ContactEmv_ContactlessMsd,

        [Map(Target.VAPS, "J")]
        [Map(Target.NWS, "J")]
        ContactEmv_ContactlessMsd_KeyEntry,

        [Map(Target.VAPS, "K")]
        [Map(Target.NWS, "K")]
        ContactEmv_ContactlessMsd_MagStripe,

        [Map(Target.VAPS, "L")]
        [Map(Target.NWS, "L")]
        ContactEmv_ContactlessMsd_MagStripe_KeyEntry,

        [Map(Target.VAPS, "M")]
        [Map(Target.NWS, "M")]
        ContactlessEmv_KeyEntry,

        [Map(Target.VAPS, "N")]
        [Map(Target.NWS, "N")]
        ContactlessEmv_MagStripe,

        [Map(Target.VAPS, "O")]
        [Map(Target.NWS, "O")]
        ContactlessEmv_ContactlessMsd,

        [Map(Target.VAPS, "P")]
        [Map(Target.NWS, "P")]
        ContactlessEmv_ContactlessMsd_KeyEntry,

        [Map(Target.VAPS, "Q")]
        [Map(Target.NWS, "Q")]
        ContactlessEmv_ContactlessMsd_MagStripe,

        [Map(Target.VAPS, "R")]
        [Map(Target.NWS, "R")]
        ContactlessEmv_ContactlessMsd_MagStripe_KeyEntry,

        [Map(Target.VAPS, "S")]
        [Map(Target.NWS, "S")]
        [Map(Target.Transit, "MAGSTRIPE_ICC_KEYED_ENTRY_ONLY")]
        ContactlessEmv_MagStripe_KeyEntry,

        [Map(Target.VAPS, "T")]
        [Map(Target.NWS, "T")]
        [Map(Target.Transit, "ICC_CHIP_CONTACT_CONTACTLESS")]
        ContactlessEmv_ContactEmv,

        [Map(Target.VAPS, "U")]
        [Map(Target.NWS, "U")]
        ContactlessEmv_ContactEmv_KeyEntry,

        [Map(Target.VAPS, "V")]
        [Map(Target.NWS, "V")]
        ContactlessEmv_ContactEmv_MagStripe,

        [Map(Target.VAPS, "W")]
        [Map(Target.NWS, "W")]
        ContactlessEmv_ContactEmv_MagStripe_KeyEntry,

        [Map(Target.VAPS, "X")]
        [Map(Target.NWS, "X")]
        ContactlessEmv_ContactEmv_ContactlessMsd,

        [Map(Target.VAPS, "Y")]
        [Map(Target.NWS, "Y")]
        ContactlessEmv_ContactEmv_ContactlessMsd_KeyEntry,

        [Map(Target.VAPS, "Z")]
        [Map(Target.NWS, "Z")]
        ContactlessEmv_ContactEmv_ContactlessMsd_MagStripe,

        [Map(Target.VAPS, "a")]
        [Map(Target.NWS, "a")]
        ContactlessEmv_ContactEmv_ContactlessMsd_MagStripe_KeyEntry
    }
}
