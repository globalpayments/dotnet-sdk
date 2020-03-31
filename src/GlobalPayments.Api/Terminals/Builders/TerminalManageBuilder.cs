using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Terminals.Ingenico;

namespace GlobalPayments.Api.Terminals.Builders {
    public class TerminalManageBuilder : TerminalBuilder<TerminalManageBuilder> {
        internal decimal? Amount { get; set; }
        internal string ClientTransactionId { get; set; }
        internal CurrencyType? Currency { get; set; }
        internal decimal? Gratuity { get; set; }
        internal string TransactionId {
            get {
                if (PaymentMethod is TransactionReference) {
                    return (PaymentMethod as TransactionReference).TransactionId;
                }
                return null;
            }
        }
        internal string CurrencyCode { get; set; }
        internal PaymentMode PaymentMode { get; set; }
        internal string AuthCode {
            get {
                if (PaymentMethod is TransactionReference)
                    return (PaymentMethod as TransactionReference).AuthCode;
                return null;
            }
        }

        /// <summary>
        /// Sets the currency code for the transaction.
        /// </summary>
        /// <param name="value">Currency Code</param>
        /// <returns></returns>
        public TerminalManageBuilder WithCurrencyCode(string value) {
            CurrencyCode = value;
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

        /// <summary>
        /// Sets the authorization code for the transaction.
        /// </summary>
        /// <param name="value">Authorization Code</param>
        /// <returns></returns>
        public TerminalManageBuilder WithAuthCode(string value) {
            if (PaymentMethod == null || !(PaymentMethod is TransactionReference)) {
                PaymentMethod = new TransactionReference();
            }
            (PaymentMethod as TransactionReference).AuthCode = value;
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
            Validations.For(TransactionType.Capture).When(() => AuthCode).IsNull().Check(() => TransactionId).IsNotNull();
            Validations.For(TransactionType.Void).When(() => ClientTransactionId).IsNull().Check(() => TransactionId).IsNotNull();
            Validations.For(PaymentMethodType.Gift).Check(() => Currency).IsNotNull();
        }
    }
}
