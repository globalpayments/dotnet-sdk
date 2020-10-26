using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;

namespace GlobalPayments.Api.Builders
{
    public abstract class TransactionBuilder<TResult> : BaseBuilder<TResult> {
        internal TransactionType TransactionType { get; set; }
        internal TransactionModifier TransactionModifier { get; set; }
        internal IPaymentMethod PaymentMethod { get; set; }
        internal bool MultiCapture { get; set; }
        internal DccRateData DccRateData { get; set; }

        public TransactionBuilder(TransactionType type) : base() {
            TransactionType = type;
        }
    }
}
