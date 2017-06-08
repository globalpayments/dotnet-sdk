using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;

namespace GlobalPayments.Api.Builders {
    public class ManagementBuilder : TransactionBuilder<Transaction> {
        internal decimal? Amount { get; set; }
        internal decimal? AuthAmount { get; set; }
        internal string AuthorizationCode {
            get {
                if (PaymentMethod is TransactionReference) {
                    return ((TransactionReference)PaymentMethod).AuthCode;
                }
                return null;
            }
        }
        internal string ClientTransactionId {
            get {
                if (PaymentMethod is TransactionReference) {
                    return ((TransactionReference)PaymentMethod).ClientTransactionId;
                }
                return null;
            }
        }
        internal string Currency { get; set; }
        internal string Description { get; set; }
        internal decimal? Gratuity { get; set; }
        internal string OrderId {
            get {
                if (PaymentMethod is TransactionReference) {
                    return ((TransactionReference)PaymentMethod).OrderId;
                }
                return null;
            }
        }
        internal string PoNumber { get; set; }
        internal ReasonCode? ReasonCode { get; set;}
        internal decimal? TaxAmount { get; set; }
        internal TaxType? TaxType { get; set; }
        internal string TransactionId {
            get {
                if (PaymentMethod is TransactionReference) {
                    return ((TransactionReference)PaymentMethod).TransactionId;
                }
                return null;
            }
        }

        public ManagementBuilder WithAmount(decimal? value) {
            Amount = value;
            return this;
        }

        public ManagementBuilder WithAuthAmount(decimal? value) {
            AuthAmount = value;
            return this;
        }

        public ManagementBuilder WithCurrency(string value) {
            Currency = value;
            return this;
        }

        public ManagementBuilder WithDescription(string value) {
            Description = value;
            return this;
        }

        public ManagementBuilder WithGratuity(decimal? value) {
            Gratuity = value;
            return this;
        }

        internal ManagementBuilder WithPaymentMethod(IPaymentMethod value) {
            PaymentMethod = value;
            return this;
        }

        public ManagementBuilder WithPoNumber(string value) {
            TransactionModifier = TransactionModifier.LevelII;
            PoNumber = value;
            return this;
        }

        public ManagementBuilder WithReasonCode(ReasonCode? value) {
            ReasonCode = value;
            return this;
        }

        public ManagementBuilder WithTaxAmount(decimal? value) {
            TransactionModifier = TransactionModifier.LevelII;
            TaxAmount = value;
            return this;
        }

        public ManagementBuilder WithTaxType(TaxType value) {
            TransactionModifier = TransactionModifier.LevelII;
            TaxType = value;
            return this;
        }

        internal ManagementBuilder WithModifier(TransactionModifier value) {
            TransactionModifier = value;
            return this;
        }

        internal ManagementBuilder(TransactionType type) : base(type) {}

        public override Transaction Execute() {
            base.Execute();

            var client = ServicesContainer.Instance.GetClient();
            return client.ManageTransaction(this);
        }

        protected override void SetupValidations() {
            Validations.For(TransactionType.Capture | TransactionType.Edit | TransactionType.Hold | TransactionType.Release)
                .Check(() => TransactionId).IsNotNull();

            Validations.For(TransactionType.Edit).With(TransactionModifier.LevelII)
                .Check(() => TaxType).IsNotNull();

            Validations.For(TransactionType.Refund)
                .When(() => Amount).IsNotNull()
                .Check(() => Currency).IsNotNull();
        }
    }
}
