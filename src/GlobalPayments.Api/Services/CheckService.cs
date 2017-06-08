using System;
using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;

namespace GlobalPayments.Api.Services {
    public class CheckService {
        public CheckService(ServicesConfig config) {
            ServicesContainer.Configure(config);
        }

        // Recurring


        // Charge
        public AuthorizationBuilder Charge(decimal? amount = null) {
            return new AuthorizationBuilder(TransactionType.Sale).WithAmount(amount);
        }

        // Void
        public ManagementBuilder Void(string transactionId) {
            return new ManagementBuilder(TransactionType.Void)
                .WithPaymentMethod(new TransactionReference {
                    PaymentMethodType = PaymentMethodType.ACH,
                    TransactionId = transactionId
                });
        }
    }
}
