using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;

namespace GlobalPayments.Api.Builders
{
    public abstract class TransactionBuilder<TResult> : BaseBuilder<TResult> {
        internal TransactionType TransactionType { get; set; }
        internal TransactionModifier TransactionModifier { get; set; }
        internal IPaymentMethod PaymentMethod { get; set; }

        public TransactionBuilder(TransactionType type, IPaymentMethod paymentMethod = null) : base() {
            TransactionType = type;
            PaymentMethod = paymentMethod;
        }
    }
}
