using GlobalPayments.Api.Builders;

namespace GlobalPayments.Api.PaymentMethods {
    interface IAuthable {
        AuthorizationBuilder Authorize(decimal? amount = null, bool isEstimated = false);
    }
}
