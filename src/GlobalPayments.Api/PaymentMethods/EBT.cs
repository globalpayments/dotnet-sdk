using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Builders;

namespace GlobalPayments.Api.PaymentMethods {
    /// <summary>
    /// Use EBT as a payment method.
    /// </summary>
    public abstract class EBT : IPaymentMethod, IBalanceable, IChargable, IRefundable, IPinProtected {
        /// <summary>
        /// Set to `PaymentMethodType.EBT` for internal methods.
        /// </summary>
        public EbtCardType EbtCardType { get; set; }
        public PaymentMethodType PaymentMethodType { get { return PaymentMethodType.EBT; } }
        public string PinBlock { get; set; }
        public string CardHolderName { get; set; }
        public AuthorizationBuilder BalanceInquiry(InquiryType? inquiry = InquiryType.FOODSTAMP) {
            return new AuthorizationBuilder(TransactionType.Balance, this).WithBalanceInquiryType(inquiry).WithAmount(0m);
        }

        public AuthorizationBuilder BenefitWithdrawal(decimal? amount = null) {
            return new AuthorizationBuilder(TransactionType.BenefitWithdrawal, this).WithAmount(amount).WithCashBack(0m);
        }

        public AuthorizationBuilder Charge(decimal? amount = null) {
            return new AuthorizationBuilder(TransactionType.Sale, this).WithAmount(amount);
        }

        public AuthorizationBuilder Refund(decimal? amount = null) {
            return new AuthorizationBuilder(TransactionType.Refund, this).WithAmount(amount);
        }
        public AuthorizationBuilder Authorize(decimal? amount = null, bool isEstimated = true) {
            return new AuthorizationBuilder(TransactionType.Auth, this)
                    .WithAmount(amount)
                    .WithAmountEstimated(true);
        }
    }
}
