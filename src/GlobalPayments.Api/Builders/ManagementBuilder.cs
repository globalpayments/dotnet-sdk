using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;

namespace GlobalPayments.Api.Builders {
    /// <summary>
    /// Used to follow up transactions for the supported
    /// payment method types.
    /// </summary>
    public class ManagementBuilder : TransactionBuilder<Transaction> {
        internal AlternativePaymentType? AlternativePaymentType { get; set; }
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
        internal string PayerAuthenticationResponse { get; set; }
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

        /// <summary>
        /// Sets the current transaction's amount.
        /// </summary>
        /// <param name="value">The amount</param>
        /// <returns>ManagementBuilder</returns>
        public ManagementBuilder WithAmount(decimal? value) {
            Amount = value;
            return this;
        }

        /// <summary>
        /// Sets the current transaction's authorized amount; where applicable.
        /// </summary>
        /// <param name="value">The authorized amount</param>
        /// <returns>ManagementBuilder</returns>
        public ManagementBuilder WithAuthAmount(decimal? value) {
            AuthAmount = value;
            return this;
        }

        /// <summary>
        /// Sets the Multicapture value as true/false.
        /// </summary>
        /// <returns>ManagementBuilder</returns>
        public ManagementBuilder WithMultiCapture(bool value) {
            MultiCapture = value;
            return this;
        }

        /// <summary>
        /// Sets the currency.
        /// </summary>
        /// <remarks>
        /// The formatting for the supplied value will currently depend on
        /// the configured gateway's requirements.
        /// </remarks>
        /// <param name="value">The currency</param>
        /// <returns>ManagementBuilder</returns>
        public ManagementBuilder WithCurrency(string value) {
            Currency = value;
            return this;
        }

        /// <summary>
        /// Sets the transaction's description.
        /// </summary>
        /// <remarks>
        /// This value is not guaranteed to be sent in the authorization
        /// or settlement process.
        /// </remarks>
        /// <param name="value">The description</param>
        /// <returns>ManagementBuilder</returns>
        public ManagementBuilder WithDescription(string value) {
            Description = value;
            return this;
        }

        /// <summary>
        /// Sets the gratuity amount; where applicable.
        /// </summary>
        /// <remarks>
        /// This value is information only and does not affect
        /// the authorization amount.
        /// </remarks>
        /// <param name="value">The gratuity amount</param>
        /// <returns>ManagementBuilder</returns>
        public ManagementBuilder WithGratuity(decimal? value) {
            Gratuity = value;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public ManagementBuilder WithPayerAuthenticationResponse(string value) {
            PayerAuthenticationResponse = value;
            return this;
        }

        internal ManagementBuilder WithPaymentMethod(IPaymentMethod value) {
            PaymentMethod = value;
            return this;
        }

        /// <summary>
        /// Sets the purchase order number; where applicable.
        /// </summary>
        /// <param name="value">The PO number</param>
        /// <returns>ManagementBuilder</returns>
        public ManagementBuilder WithPoNumber(string value) {
            TransactionModifier = TransactionModifier.LevelII;
            PoNumber = value;
            return this;
        }

        /// <summary>
        /// Sets the reason code for the transaction.
        /// </summary>
        /// <param name="value">The reason code</param>
        /// <returns>ManagementBuilder</returns>
        public ManagementBuilder WithReasonCode(ReasonCode? value) {
            ReasonCode = value;
            return this;
        }

        /// <summary>
        /// Sets the tax amount.
        /// </summary>
        /// <remarks>
        /// Useful for commercial purchase card requests.
        /// </remarks>
        /// <seealso>See `AuthorizationBuilder.WithCommercialRequest`</seealso>
        /// <param name="value">The tax amount</param>
        /// <returns>ManagementBuilder</returns>
        public ManagementBuilder WithTaxAmount(decimal? value) {
            TransactionModifier = TransactionModifier.LevelII;
            TaxAmount = value;
            return this;
        }

        /// <summary>
        /// Sets the tax type.
        /// </summary>
        /// <remarks>
        /// Useful for commercial purchase card requests.
        /// </remarks>
        /// <seealso>See `AuthorizationBuilder.WithCommercialRequest`</seealso>
        /// <param name="value">The tax type</param>
        /// <returns>ManagementBuilder</returns>
        public ManagementBuilder WithTaxType(TaxType value) {
            TransactionModifier = TransactionModifier.LevelII;
            TaxType = value;
            return this;
        }

        internal ManagementBuilder WithModifier(TransactionModifier value) {
            TransactionModifier = value;
            return this;
        }
        public ManagementBuilder WithAlternativePaymentType(AlternativePaymentType value) {
            this.AlternativePaymentType = value;
            return this;
        }

        internal ManagementBuilder(TransactionType type) : base(type) {}

        /// <summary>
        /// Executes the builder against the gateway.
        /// </summary>
        /// <returns>Transaction</returns>
        public override Transaction Execute(string configName = "default") {
            base.Execute(configName);

            var client = ServicesContainer.Instance.GetClient(configName);
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

            Validations.For(TransactionType.VerifySignature)
                .Check(() => PayerAuthenticationResponse).IsNotNull()
                .Check(() => Amount).IsNotNull()
                .Check(() => Currency).IsNotNull()
                .Check(() => OrderId).IsNotNull();
        }
    }
}
