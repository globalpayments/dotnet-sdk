using System;
using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;

namespace GlobalPayments.Api.PaymentMethods
{
    public class Cash : IPaymentMethod, IChargable, IRefundable {
        public PaymentMethodType PaymentMethodType { get { return PaymentMethodType.Cash; } }

        public AuthorizationBuilder Charge(decimal? amount = default(decimal?)) {
            throw new NotImplementedException();
        }

        AuthorizationBuilder IRefundable.Refund(decimal? amount) {
            throw new NotImplementedException();
        }
    }
}
