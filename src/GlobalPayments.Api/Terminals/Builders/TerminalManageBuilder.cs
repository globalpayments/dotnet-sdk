using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;

namespace GlobalPayments.Api.Terminals.Builders {
    public class TerminalManageBuilder : TerminalBuilder<TerminalManageBuilder> {
        internal decimal? Amount { get; set; }
        internal CurrencyType? Currency { get; set; }
        internal decimal? Gratuity { get; set; }
        internal string TransactionId {
            get {
                if (PaymentMethod is TransactionReference)
                    return (PaymentMethod as TransactionReference).TransactionId;
                return null;
            }
        }

        public TerminalManageBuilder WithAmount(decimal? amount) {
            Amount = amount;
            return this;
        }
        public TerminalManageBuilder WithCurrency(CurrencyType? value) {
            Currency = value;
            return this;
        }
        public TerminalManageBuilder WithGratuity(decimal? amount) {
            Gratuity = amount;
            return this;
        }
        public TerminalManageBuilder WithTransactionId(string value) {
            if (PaymentMethod == null || !(PaymentMethod is TransactionReference))
                PaymentMethod = new TransactionReference();
            (PaymentMethod as TransactionReference).TransactionId = value;
            return this;
        }

        internal TerminalManageBuilder(TransactionType type, PaymentMethodType paymentType) : base(type, paymentType) {
        }

        public override TerminalResponse Execute() {
            base.Execute();

            var device = ServicesContainer.Instance.GetDeviceController();
            return device.ManageTransaction(this);
        }

        protected override void SetupValidations() {
            Validations.For(TransactionType.Capture | TransactionType.Void).Check(() => TransactionId).IsNotNull();
            Validations.For(PaymentMethodType.Gift).Check(() => Currency).IsNotNull();
        }
    }
}
