using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;

namespace GlobalPayments.Api.Services {
    public class GiftService {
        public GiftService(GatewayConfig config, string configName = "default") {
            ServicesContainer.ConfigureService(config, configName);
        }

        public AuthorizationBuilder Activate(decimal? amount = null) {
            return new AuthorizationBuilder(TransactionType.Activate).WithAmount(amount);
        }

        public AuthorizationBuilder AddValue(decimal? amount = null) {
            return new AuthorizationBuilder(TransactionType.AddValue).WithAmount(amount);
        }

        public AuthorizationBuilder AddAlias(string phoneNumber) {
            return new AuthorizationBuilder(TransactionType.Alias).WithAlias(AliasAction.ADD, phoneNumber);
        }

        public AuthorizationBuilder BalanceInquiry(InquiryType? inquiry = null) {
            return new AuthorizationBuilder(TransactionType.Balance);
        }

        public AuthorizationBuilder Charge(decimal? amount = null) {
            return new AuthorizationBuilder(TransactionType.Sale).WithAmount(amount);
        }

        public GiftCard Create(string phoneNumber) {
            return GiftCard.Create(phoneNumber);
        }

        public AuthorizationBuilder Deactivate() {
            return new AuthorizationBuilder(TransactionType.Deactivate);
        }

        public AuthorizationBuilder RemoveAlias(string phoneNumber) {
            return new AuthorizationBuilder(TransactionType.Alias).WithAlias(AliasAction.DELETE, phoneNumber);
        }

        public AuthorizationBuilder ReplaceWith(GiftCard newCard) {
            return new AuthorizationBuilder(TransactionType.Replace).WithReplacementCard(newCard);
        }

        public AuthorizationBuilder Reverse(decimal? amount = null) {
            return new AuthorizationBuilder(TransactionType.Reversal)
                .WithAmount(amount)
                .WithPaymentMethod(new TransactionReference {
                    PaymentMethodType = PaymentMethodType.Gift
                });
        }

        public AuthorizationBuilder Rewards(decimal? amount = null) {
            return new AuthorizationBuilder(TransactionType.Reward).WithAmount(amount);
        }

        public ManagementBuilder Void(string transactionId) {
            return new ManagementBuilder(TransactionType.Void).WithPaymentMethod(new TransactionReference {
                TransactionId = transactionId,
                PaymentMethodType = PaymentMethodType.Gift
            });
        }
    }
}
