using System;
using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;

namespace GlobalPayments.Api.Services {
    public class DebitService {
        public DebitService(ServicesConfig config) {
            ServicesContainer.Configure(config);
        }

        public AuthorizationBuilder Charge(decimal? amount = null) {
            return new AuthorizationBuilder(TransactionType.Sale).WithAmount(amount);
        }

        public AuthorizationBuilder Refund(decimal? amount = null) {
            return new AuthorizationBuilder(TransactionType.Refund)
                .WithAmount(amount)
                .WithPaymentMethod(new TransactionReference {
                    PaymentMethodType = PaymentMethodType.Debit
                });
        }

        public AuthorizationBuilder Reverse(decimal? amount = null) {
            return new AuthorizationBuilder(TransactionType.Reversal)
                .WithAmount(amount)
                .WithPaymentMethod(new TransactionReference {
                    PaymentMethodType = PaymentMethodType.Debit
                });
        }
    }
}
