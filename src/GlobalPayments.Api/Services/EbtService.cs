using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;

namespace GlobalPayments.Api.Services {
    public class EbtService {
        public EbtService(GatewayConfig config, string configName = "default") {
            ServicesContainer.ConfigureService(config, configName);
        }

        public AuthorizationBuilder BalanceInquiry(InquiryType type = InquiryType.FOODSTAMP) {
            return new AuthorizationBuilder(TransactionType.Balance).WithBalanceInquiryType(type).WithAmount(0m);
        }

        public AuthorizationBuilder BenefitWithdrawal(decimal? amount = null) {
            return new AuthorizationBuilder(TransactionType.BenefitWithdrawal).WithAmount(amount).WithCashBack(0m);
        }

        public AuthorizationBuilder Charge(decimal? amount = null) {
            return new AuthorizationBuilder(TransactionType.Sale).WithAmount(amount);
        }

        public AuthorizationBuilder Refund(decimal? amount = null) {
            return new AuthorizationBuilder(TransactionType.Refund)
                .WithAmount(amount)
                .WithPaymentMethod(new TransactionReference {
                    PaymentMethodType = PaymentMethodType.EBT
                });
        }
    }
}
