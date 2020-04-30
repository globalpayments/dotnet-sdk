using GlobalPayments.Api.Builders;

namespace GlobalPayments.Api.PaymentMethods {
    interface IEditable {
        ManagementBuilder Edit(decimal? amount = null);
    }
}
