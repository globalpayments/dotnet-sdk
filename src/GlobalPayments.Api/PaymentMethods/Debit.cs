using System;
using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;

namespace GlobalPayments.Api.PaymentMethods
{
    /// <summary>
    /// Use PIN debit as a payment method.
    /// </summary>
    public abstract class Debit : IPaymentMethod, IPrePaid, IRefundable, IReversable, IChargable, IEncryptable, IPinProtected {
        public string CardType { get; set; }

        public EncryptionData EncryptionData { get; set; }

        /// <summary>
        /// Set to `PaymentMethodType.Debit` for internal methods.
        /// </summary>
        public PaymentMethodType PaymentMethodType { get { return PaymentMethodType.Debit; } }

        public string PinBlock { get; set; }

        public Debit() {
            CardType = "Unknown";
        }

        public AuthorizationBuilder AddValue(decimal? amount = null) {
            return new AuthorizationBuilder(TransactionType.AddValue, this).WithAmount(amount);
        }

        public AuthorizationBuilder Charge(decimal? amount = null) {
            return new AuthorizationBuilder(TransactionType.Sale, this).WithAmount(amount);
        }

        public AuthorizationBuilder Refund(decimal? amount = null) {
            return new AuthorizationBuilder(TransactionType.Refund, this).WithAmount(amount);
        }

        public AuthorizationBuilder Reverse(decimal? amount = null) {
            return new AuthorizationBuilder(TransactionType.Reversal, this).WithAmount(amount);
        }
    }
}
