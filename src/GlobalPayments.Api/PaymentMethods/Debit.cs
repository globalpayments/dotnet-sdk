using System;
using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;

namespace GlobalPayments.Api.PaymentMethods
{
    public abstract class Debit : IPaymentMethod, IPrePayable, IRefundable, IReversable, IChargable, IEncryptable, IPinProtected {
        public EncryptionData EncryptionData { get; set; }
        public PaymentMethodType PaymentMethodType { get { return PaymentMethodType.Debit; } }
        public string PinBlock { get; set; }

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
    public class DebitTrackData : Debit, ITrackData {
        public EntryMethod EntryMethod { get; set; }
        public string Value { get; set; }
    }
}
