using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Terminals.Abstractions;

namespace GlobalPayments.Api.Terminals.Builders {
    public class TerminalManageBuilder : TerminalBuilder<TerminalManageBuilder> {
        internal decimal? Amount { get; set; }
        internal string ClientTransactionId { get; set; }
        internal CurrencyType? Currency { get; set; }
        internal decimal? Gratuity { get; set; }
        public string TerminalRefNumber { get; set; }
        internal string TransactionId {
            get {
                if (PaymentMethod is TransactionReference)
                    return (PaymentMethod as TransactionReference).TransactionId;
                return null;
            }
        }

        public TerminalManageBuilder WithTerminalRefNumber(string terminalRefNumber) {
            TerminalRefNumber = terminalRefNumber;
            return this;
        }

        public TerminalManageBuilder WithEcrId(int ecrId) {
            EcrId = ecrId;
            return this;
        }

        public TerminalManageBuilder WithAmount(decimal? amount) {
            Amount = amount;
            return this;
        }
        public TerminalManageBuilder WithClientTransactionId(string value) {
            ClientTransactionId = value;
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

        public override ITerminalResponse Execute(string configName = "default") {
            base.Execute(configName);

            var device = ServicesContainer.Instance.GetDeviceController(configName);
            return device.ManageTransaction(this);
        }

        public override byte[] Serialize(string configName = "default") {
            base.Execute();

            var device = ServicesContainer.Instance.GetDeviceController(configName);
            return device.SerializeRequest(this);
        }

        protected override void SetupValidations() {
            Validations.For(TransactionType.Capture).Check(() => TransactionId).IsNotNull();
            Validations.For(TransactionType.Void).When(() => ClientTransactionId).IsNull().Check(() => TransactionId).IsNotNull();
            Validations.For(PaymentMethodType.Gift).Check(() => Currency).IsNotNull();
        }
    }
}
