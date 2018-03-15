using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;

namespace GlobalPayments.Api.Services {
    public class CreditService {
        public CreditService(GatewayConfig config, string configName = "default") {
            ServicesContainer.ConfigureService(config, configName);
        }

        public AuthorizationBuilder Authorize(decimal? amount = null) {
            return new AuthorizationBuilder(TransactionType.Auth).WithAmount(amount);
        }

        public ManagementBuilder Capture(string transactionId) {
            return new ManagementBuilder(TransactionType.Capture)
                .WithPaymentMethod(new TransactionReference {
                    PaymentMethodType = PaymentMethodType.Credit,
                    TransactionId = transactionId
                });
        }

        public AuthorizationBuilder Charge(decimal? amount = null) {
            return new AuthorizationBuilder(TransactionType.Sale).WithAmount(amount);
        }

        public ManagementBuilder Edit(string transactionId = null) {
            return new ManagementBuilder(TransactionType.Edit)
                .WithPaymentMethod(new TransactionReference {
                    PaymentMethodType = PaymentMethodType.Credit,
                    TransactionId = transactionId
                });
        }

        public AuthorizationBuilder Refund(decimal? amount = null) {
            return new AuthorizationBuilder(TransactionType.Refund)
                .WithAmount(amount)
                .WithPaymentMethod(new TransactionReference {
                    PaymentMethodType = PaymentMethodType.Credit
                });
        }

        public AuthorizationBuilder Reverse(decimal? amount = null) {
            return new AuthorizationBuilder(TransactionType.Reversal)
                .WithAmount(amount)
                .WithPaymentMethod(new TransactionReference {
                    PaymentMethodType = PaymentMethodType.Credit
                });
        }

        public AuthorizationBuilder Verify() {
            return new AuthorizationBuilder(TransactionType.Verify);
        }

        public ManagementBuilder Void(string transactionId = null) {
            return new ManagementBuilder(TransactionType.Void)
                .WithPaymentMethod(new TransactionReference {
                    PaymentMethodType = PaymentMethodType.Credit,
                    TransactionId = transactionId
                });
        }
    }
}
