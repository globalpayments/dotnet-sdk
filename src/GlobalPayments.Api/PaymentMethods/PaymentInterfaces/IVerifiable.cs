using GlobalPayments.Api.Builders;

namespace GlobalPayments.Api.PaymentMethods {
    interface IVerifiable {
        AuthorizationBuilder Verify();
    }
}
