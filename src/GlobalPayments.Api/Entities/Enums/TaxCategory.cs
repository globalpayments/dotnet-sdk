using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Entities {
    public enum TaxCategory {
        [Map(Target.Transit, "SERVICE")]
        Service,

        [Map(Target.Transit, "DUTY")]
        Duty,

        [Map(Target.Transit, "VAT")]
        VAT,

        [Map(Target.Transit, "ALTERNATE")]
        Alternate,

        [Map(Target.Transit, "NATIONAL")]
        National,

        [Map(Target.Transit, "TAX_EXEMPT")]
        TaxExempt
    }
}
