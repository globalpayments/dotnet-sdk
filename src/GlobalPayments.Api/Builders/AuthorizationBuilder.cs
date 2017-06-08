using System;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;

namespace GlobalPayments.Api.Builders {
    public class AuthorizationBuilder : TransactionBuilder<Transaction> {
        internal string Alias { get; set; }
        internal AliasAction? AliasAction { get; set; }
        internal bool AllowDuplicates { get; set; }
        internal bool AllowPartialAuth { get; set; }
        internal decimal? Amount { get; set; }
        internal decimal? AuthAmount { get; set; }
        internal InquiryType? BalanceInquiryType { get; set; }
        internal Address BillingAddress { get; set; }
        internal decimal? CashBackAmount { get; set; }
        internal string ClientTransactionId { get; set; }
        internal string Currency { get; set; }
        internal string CustomerId { get; set; }
        internal string CustomerIpAddress { get; set; }
        internal string Cvn { get; set; }
        internal string Description { get; set; }
        internal string DynamicDescriptor { get; set; }
        internal EcommerceInfo EcommerceInfo { get; set; }
        internal decimal? Gratuity { get; set; }
        internal HostedPaymentData HostedPaymentData { get; set; }
        internal string InvoiceNumber { get; set; }
        internal bool Level2Request { get; set; }
        internal string OfflineAuthCode { get; set; }
        internal bool OneTimePayment { get; set; }
        internal string OrderId { get; set; }
        internal string ProductId { get; set; }
        internal RecurringSequence? RecurringSequence { get; set; }
        internal RecurringType? RecurringType { get; set; }
        internal bool RequestMultiUseToken { get; set; }
        internal GiftCard ReplacementCard { get; set; }
        internal string ScheduleId { get; set; }
        internal Address ShippingAddress { get; set; }
        internal string Timestamp { get; set; }

        public AuthorizationBuilder WithAddress(Address value, AddressType type = AddressType.Billing) {
            value.Type = type; // set the address type
            if (type == AddressType.Billing)
                BillingAddress = value;
            else ShippingAddress = value;
            return this;
        }

        internal AuthorizationBuilder WithAlias(AliasAction action, string value) {
            Alias = value;
            AliasAction = action;
            return this;
        }

        public AuthorizationBuilder WithAllowDuplicates(bool value) {
            AllowDuplicates = value;
            return this;
        }

        public AuthorizationBuilder WithAllowPartialAuth(bool value) {
            AllowPartialAuth = value;
            return this;
        }

        public AuthorizationBuilder WithAmount(decimal? value) {
            Amount = value;
            return this;
        }

        public AuthorizationBuilder WithAuthAmount(decimal? value) {
            AuthAmount = value;
            return this;
        }

        internal AuthorizationBuilder WithBalanceInquiryType(InquiryType? value) {
            BalanceInquiryType = value;
            return this;
        }

        public AuthorizationBuilder WithCashBack(decimal? value) {
            CashBackAmount = value;
            TransactionModifier = TransactionModifier.CashBack;
            return this;
        }

        public AuthorizationBuilder WithClientTransactionId(string value) {
            if (TransactionType == TransactionType.Reversal || TransactionType == TransactionType.Refund) {
                if (PaymentMethod is TransactionReference) {
                    ((TransactionReference)PaymentMethod).ClientTransactionId = value;
                }
                else {
                    PaymentMethod = new TransactionReference {
                        ClientTransactionId = value
                    };
                }
            }
            else ClientTransactionId = value;
            return this;
        }

        public AuthorizationBuilder WithCurrency(string value) {
            Currency = value;
            return this;
        }

        public AuthorizationBuilder WithCustomerId(string value) {
            CustomerId = value;
            return this;
        }

        public AuthorizationBuilder WithCustomerIpAddress(string value) {
            CustomerIpAddress = value;
            return this;
        }

        /// <summary>
        /// Sets the CVN value for recurring payments; where applicable.
        /// </summary>
        /// <param name="value">Cvn value to use in the request</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithCvn(string value) {
            Cvn = value;
            return this;
        }

        public AuthorizationBuilder WithDescription(string value) {
            Description = value;
            return this;
        }

        public AuthorizationBuilder WithDynamicDescriptor(string value) {
            DynamicDescriptor = value;
            return this;
        }

        public AuthorizationBuilder WithEcommerceInfo(EcommerceInfo value) {
            EcommerceInfo = value;
            return this;
        }

        public AuthorizationBuilder WithGratuity(decimal? value) {
            Gratuity = value;
            return this;
        }

        /// <summary>
        /// Additional hosted payment specific information for Realex HPP implementation.
        /// </summary>
        /// <param name="value">HostedPaymentData</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithHostedPaymentData(HostedPaymentData value) {
            var client = ServicesContainer.Instance.GetClient();
            if (client.SupportsHostedPayments) {
                HostedPaymentData = value;
                return this;
            }
            throw new UnsupportedTransactionException("You current gateway does not support hosted payments.");
        }

        public AuthorizationBuilder WithInvoiceNumber(string value) {
            InvoiceNumber = value;
            return this;
        }

        public AuthorizationBuilder WithCommercialRequest(bool value) {
            Level2Request = value;
            return this;
        }

        public AuthorizationBuilder WithOfflineAuthCode(string value) {
            OfflineAuthCode = value;
            TransactionModifier = TransactionModifier.Offline;
            return this;
        }

        public AuthorizationBuilder WithOneTimePayment(bool value) {
            OneTimePayment = value;
            TransactionModifier = TransactionModifier.Recurring;
            return this;
        }

        public AuthorizationBuilder WithOrderId(string value) {
            OrderId = value;
            return this;
        }

        public AuthorizationBuilder WithPaymentMethod(IPaymentMethod value) {
            PaymentMethod = value;
            if (value is EBTCardData && ((EBTCardData)value).SerialNumber != null)
                TransactionModifier = TransactionModifier.Voucher;
            return this;
        }

        public AuthorizationBuilder WithProductId(string value) {
            ProductId = value;
            return this;
        }

        /// <summary>
        /// Sets the Recurring Info for Realex based recurring payments; where applicable.
        /// </summary>
        /// <param name="type">The value can be 'fixed' or 'variable' depending on whether the amount will change for each transaction.</param>
        /// <param name="sequence">Indicates where in the recurring sequence the transaction occurs. Must be 'first' for the first transaction for this card, 'subsequent' for transactions after that, and 'last' for the final transaction of the set.</param>
        /// <returns></returns>
        public AuthorizationBuilder WithRecurringInfo(RecurringType type, RecurringSequence sequence) {
            RecurringSequence = sequence;
            RecurringType = type;
            return this;
        }

        public AuthorizationBuilder WithRequestMultiUseToken(bool value) {
            RequestMultiUseToken = value;
            return this;
        }

        internal AuthorizationBuilder WithReplacementCard(GiftCard value) {
            ReplacementCard = value;
            return this;
        }

        public AuthorizationBuilder WithScheduleId(string value) {
            ScheduleId = value;
            return this;
        }

        public AuthorizationBuilder WithTransactionId(string value) {
            if (PaymentMethod is TransactionReference) {
                ((TransactionReference)PaymentMethod).TransactionId = value;
            }
            else {
                PaymentMethod = new TransactionReference {
                    TransactionId = value
                };
            }
            return this;
        }

        internal AuthorizationBuilder WithModifier(TransactionModifier value) {
            TransactionModifier = value;
            return this;
        }

        public AuthorizationBuilder WithTimestamp(string value) {
            Timestamp = value;
            return this;
        }


        internal AuthorizationBuilder(TransactionType type, IPaymentMethod payment = null) : base(type, payment) {}

        public override Transaction Execute() {
            base.Execute();

            var client = ServicesContainer.Instance.GetClient();
            return client.ProcesAuthorization(this);
        }

        public string Serialize() {
            TransactionModifier = TransactionModifier.HostedRequest;
            base.Execute();

            var client = ServicesContainer.Instance.GetClient();
            if (client.SupportsHostedPayments) {
                return client.SerializeRequest(this);
            }
            throw new UnsupportedTransactionException("You current gateway does not support hosted payments.");
        }

        protected override void SetupValidations() {
            Validations.For(TransactionType.Auth | TransactionType.Sale | TransactionType.Refund | TransactionType.AddValue)
                .With(TransactionModifier.None)
                .Check(() => Amount).IsNotNull()
                .Check(() => Currency).IsNotNull()
                .Check(() => PaymentMethod).IsNotNull();

            Validations.For(TransactionType.Auth | TransactionType.Sale)
                .With(TransactionModifier.HostedRequest)
                .Check(() => Amount).IsNotNull()
                .Check(() => Currency).IsNotNull();

            Validations.For(TransactionType.Verify)
                .With(TransactionModifier.HostedRequest)
                .Check(() => Currency).IsNotNull()
                .Check(() => Amount).IsNull();

            Validations.For(TransactionType.Auth | TransactionType.Sale)
                .With(TransactionModifier.Offline)
                .Check(() => Amount).IsNotNull()
                .Check(() => Currency).IsNotNull()
                .Check(() => OfflineAuthCode).IsNotNull();

            Validations.For(TransactionType.BenefitWithdrawal).With(TransactionModifier.CashBack)
                .Check(() => Amount).IsNotNull()
                .Check(() => Currency).IsNotNull()
                .Check(() => PaymentMethod).IsNotNull();

            Validations.For(TransactionType.Balance).Check(() => PaymentMethod).IsNotNull();

            Validations.For(TransactionType.Alias)
                .Check(() => AliasAction).IsNotNull()
                .Check(() => Alias).IsNotNull();

            Validations.For(TransactionType.Replace).Check(() => ReplacementCard).IsNotNull();

            Validations.For(PaymentMethodType.ACH).Check(() => BillingAddress).IsNotNull();
        }
    }
}
