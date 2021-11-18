using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Terminals.Abstractions;

namespace GlobalPayments.Api.Terminals.Builders {
    public class TerminalAuthBuilder : TerminalBuilder<TerminalAuthBuilder> {
        internal Address Address { get; set; }
        internal bool AllowDuplicates { get; set; }
        internal decimal? Amount { get; set; }
        internal string AuthCode {
            get {
                if (PaymentMethod is TransactionReference)
                    return (PaymentMethod as TransactionReference).AuthCode;
                return null;
            }
        }
        internal AutoSubstantiation AutoSubstantiation { get; set; }
        internal decimal? CashBackAmount { get; set; }
        internal string ClientTransactionId { get; set; }
        internal CurrencyType? Currency { get; set; }
        internal string CustomerCode { get; set; }
        internal decimal? Gratuity { get; set; }
        internal string InvoiceNumber { get; set; }
        internal string PoNumber { get; set; }
        internal bool RequestMultiUseToken { get; set; }
        internal bool SignatureCapture { get; set; }
        internal decimal TaxAmount { get; set; }
        internal string TaxExempt { get; set; }
        internal string TaxExemptId { get; set; }
        internal int TerminalRefNumber { get; set; }
        internal int? ClerkId { get; set; }
        internal string LineItemLeft { get; set; }
        internal string LineItemRight { get; set; }
        internal string TransactionId {
            get {
                if (PaymentMethod is TransactionReference)
                    return (PaymentMethod as TransactionReference).TransactionId;
                return null;
            }
        }

        public TerminalAuthBuilder WithLineItemLeft(string lineItemLeft) {
            LineItemLeft = lineItemLeft;
            return this;
        }

        public TerminalAuthBuilder WithLineItemRight(string lineItemRight) {
            LineItemRight = lineItemRight;
            return this;
        }

        public TerminalAuthBuilder WithAddress(Address address) {
            Address = address;
            return this;
        }

        public TerminalAuthBuilder WithEcrId(int ecrId) {
            EcrId = ecrId;
            return this;
        }

        public TerminalAuthBuilder WithAllowDuplicates(bool allowDuplicates) {
            AllowDuplicates = allowDuplicates;
            return this;
        }
        public TerminalAuthBuilder WithAmount(decimal? amount) {
            Amount = amount;
            return this;
        }
        public TerminalAuthBuilder WithAuthCode(string value) {
            if (PaymentMethod == null || !(PaymentMethod is TransactionReference))
                PaymentMethod = new TransactionReference();
            (PaymentMethod as TransactionReference).AuthCode = value;
            return this;
        }

        /// <summary>
        /// Sets the auto subtantiation values for the transaction.
        /// </summary>
        /// <param name="value">The auto substantiation object</param>
        /// <returns>TerminalAuthBuilder</returns>
        public TerminalAuthBuilder WithAutoSubstantiation(AutoSubstantiation value) {
            AutoSubstantiation = value;
            return this;
        }
        public TerminalAuthBuilder WithCashBack(decimal? amount) {
            CashBackAmount = amount;
            return this;
        }
        public TerminalAuthBuilder WithClientTransactionId(string value) {
            ClientTransactionId = value;
            return this;
        }
        public TerminalAuthBuilder WithCurrency(CurrencyType? value) {
            Currency = value;
            return this;
        }
        public TerminalAuthBuilder WithCustomerCode(string customerCode) {
            CustomerCode = customerCode;
            return this;
        }
        public TerminalAuthBuilder WithGratuity(decimal? gratuity) {
            Gratuity = gratuity;
            return this;
        }
        public TerminalAuthBuilder WithInvoiceNumber(string invoiceNumber) {
            this.InvoiceNumber = invoiceNumber;
            return this;
        }
        public TerminalAuthBuilder WithPaymentMethod(IPaymentMethod method) {
            PaymentMethod = method;
            return this;
        }
        public TerminalAuthBuilder WithPoNumber(string poNumber) {
            PoNumber = poNumber;
            return this;
        }
        public TerminalAuthBuilder WithRequestMultiUseToken(bool requestMultiUseToken) {
            RequestMultiUseToken = requestMultiUseToken;
            return this;
        }
        public TerminalAuthBuilder WithSignatureCapture(bool signatureCapture) {
            SignatureCapture = signatureCapture;
            return this;
        }
        public TerminalAuthBuilder WithTaxAmount(decimal taxAmount) {
            TaxAmount = taxAmount;
            return this;
        }

        public TerminalAuthBuilder WithClerkId(short clerkId) {
            ClerkId = clerkId;
            return this;
        }

        public TerminalAuthBuilder WithTerminalRefNumber(int terminalRefNumber) {
            TerminalRefNumber = terminalRefNumber;
            return this;
        }

        public TerminalAuthBuilder WithTaxType(TaxType taxType, string taxExemptId = null) {
            TaxExempt = taxType == TaxType.TAXEXEMPT ? "1" : "0";
            TaxExemptId = taxExemptId;
            return this;
        }
        public TerminalAuthBuilder WithToken(string value) {
            if (PaymentMethod == null || !(PaymentMethod is CreditCardData))
                PaymentMethod = new CreditCardData();
            (PaymentMethod as CreditCardData).Token = value;
            return this;
        }
        public TerminalAuthBuilder WithTransactionId(string value) {
            if (PaymentMethod == null || !(PaymentMethod is TransactionReference))
                PaymentMethod = new TransactionReference();
            (PaymentMethod as TransactionReference).TransactionId = value;
            return this;
        }

        internal TerminalAuthBuilder(TransactionType type, PaymentMethodType paymentType) : base(type, paymentType) {
        }

        public override ITerminalResponse Execute(string configName = "default") {
            base.Execute(configName);

            var device = ServicesContainer.Instance.GetDeviceController(configName);
            return device.ProcessTransaction(this);
        }

        public override byte[] Serialize(string configName = "default") {
            base.Execute();

            var device = ServicesContainer.Instance.GetDeviceController(configName);
            return device.SerializeRequest(this);
        }

        protected override void SetupValidations() {
            Validations.For(TransactionType.Sale | TransactionType.Auth).Check(() => Amount).IsNotNull();
            Validations.For(TransactionType.Refund).Check(() => Amount).IsNotNull();
            Validations.For(TransactionType.Refund)
                .With(PaymentMethodType.Credit)
                .When(() => TransactionId).IsNotNull()
                .Check(() => AuthCode).IsNotNull();
            Validations.For(PaymentMethodType.Gift).Check(() => Currency).IsNotNull();
            Validations.For(TransactionType.AddValue).Check(() => Amount).IsNotNull();

            Validations.For(PaymentMethodType.EBT).With(TransactionType.Balance)
                .When(() => Currency).IsNotNull()
                .Check(() => Currency).DoesNotEqual(CurrencyType.VOUCHER);
            Validations.For(TransactionType.BenefitWithdrawal)
                .When(() => Currency).IsNotNull()
                .Check(() => Currency).Equals(CurrencyType.CASH_BENEFITS);
            Validations.For(PaymentMethodType.EBT).With(TransactionType.Refund).Check(() => AllowDuplicates).Equals(false);
            Validations.For(PaymentMethodType.EBT).With(TransactionType.BenefitWithdrawal).Check(() => AllowDuplicates).Equals(false);
        }
    }
}
