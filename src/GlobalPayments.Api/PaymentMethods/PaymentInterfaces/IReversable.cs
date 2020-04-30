using GlobalPayments.Api.Builders;

namespace GlobalPayments.Api.PaymentMethods {
    interface IReversable {
        AuthorizationBuilder Reverse(decimal? amount = null);
    }
}
