using GlobalPayments.Api.Builders;

namespace GlobalPayments.Api.PaymentMethods {
    interface IRefundable {
        AuthorizationBuilder Refund(decimal? amount = null);
    }
}
