using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;

namespace GlobalPayments.Api.Terminals.Builders {
    public abstract class TerminalBuilder<T> : TransactionBuilder<TerminalResponse> where T : TerminalBuilder<T> {
        internal PaymentMethodType PaymentMethodType { get; set; }
        internal int ReferenceNumber { get; set; }

        public T WithReferenceNumber(int value) {
            ReferenceNumber = value;
            return this as T;
        }
        internal TerminalBuilder(TransactionType type, PaymentMethodType paymentType) : base(type) {
            PaymentMethodType = paymentType;
        }
    }
}
