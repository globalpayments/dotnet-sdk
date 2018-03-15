using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Entities.Payroll {
    public abstract class PayrollEntity {
        internal abstract void FromJson(JsonDoc doc, PayrollEncoder encoder);
    }
}
