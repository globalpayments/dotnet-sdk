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
        internal decimal? TaxAmount { get; set; }
        internal string TaxExempt { get; set; }
        internal string TaxExemptId { get; set; }
        internal string InvoiceNumber { get; set; }
        internal int? ProcessCPC { get; set; }

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

        public TerminalManageBuilder WithTaxAmount(decimal taxAmount) {
            TaxAmount = taxAmount;
            return this;
        }
        public TerminalManageBuilder WithTaxType(TaxType taxType, string taxExemptId = null) {
            TaxExempt = taxType == TaxType.TAXEXEMPT ? "1" : "0";
            TaxExemptId = taxExemptId;
            return this;
        }

        public TerminalManageBuilder WithInvoiceNumber(string invoiceNumber) {
            this.InvoiceNumber = invoiceNumber;
            return this;
        }

        public TerminalManageBuilder WithProcessCPC(int value) {
            ProcessCPC = value;
            return this;
        }

        public TerminalManageBuilder WithTransactionId(string value) {
            if (PaymentMethod == null || !(PaymentMethod is TransactionReference))
                PaymentMethod = new TransactionReference();
            (PaymentMethod as TransactionReference).TransactionId = value;
            return this;
        }

        public TerminalManageBuilder WithTransactionModifier(TransactionModifier modifier) {
            TransactionModifier = modifier;
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
          
            #region ENUM VALIDATION WITH FLAG ATTRIBUTE          

            Validations.For(PaymentMethodType.Gift).Check(() => Currency).IsNotNull();

            #endregion


            Validations.For(TransactionType.Capture).Check(() => TransactionId).IsNotNull();
            Validations.For(TransactionType.Void).When(() => ClientTransactionId).IsNull().Check(() => TransactionId).IsNotNull();
            
        }
    }
}
