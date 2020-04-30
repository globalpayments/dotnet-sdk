using GlobalPayments.Api.Builders;

namespace GlobalPayments.Api.PaymentMethods {
    interface IPrePaid {
        AuthorizationBuilder AddValue(decimal? amount = null);
    }
}
