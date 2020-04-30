using GlobalPayments.Api.Builders;

namespace GlobalPayments.Api.PaymentMethods {
    interface IChargable {
        AuthorizationBuilder Charge(decimal? amount = null);
    }
}
