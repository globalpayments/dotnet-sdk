using GlobalPayments.Api.Entities;

namespace GlobalPayments.Api.PaymentMethods {
    public interface IPaymentMethod {
        PaymentMethodType PaymentMethodType { get; } 
    }
}
