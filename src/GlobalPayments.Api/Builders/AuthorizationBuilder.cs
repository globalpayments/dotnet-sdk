using System;
using System.Collections.Generic;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Billing;
using GlobalPayments.Api.Entities.TransactionApi.Request;
using GlobalPayments.Api.Entities.Enums;
using GlobalPayments.Api.Network.Elements;
using GlobalPayments.Api.Network.Entities;
using GlobalPayments.Api.PaymentMethods;
using System.Reflection;
using System.Linq;

namespace GlobalPayments.Api.Builders {
    /// <summary>
    /// Used to create charges, verifies, etc. for the supported
    /// payment method types.
    /// </summary>
    public class AuthorizationBuilder : TransactionBuilder<Transaction> {
        /// <summary>
        /// Gets or sets the installment payment data for the transaction.
        /// </summary>
        internal InstallmentData InstallmentData { get; set; }
        /// <summary>
        /// Gets or sets the account type for the transaction; see <see cref="GlobalPayments.Api.Entities.Enums.AccountType"/> for supported values.
        /// </summary>
        internal AccountType? AccountType { get; set; }
        /// <summary>
        /// Gets or sets the gift-card alias used in alias operations.
        /// </summary>
        internal string Alias { get; set; }
        /// <summary>
        /// Gets or sets the alias action to perform (add or delete).
        /// </summary>
        internal AliasAction? AliasAction { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the gateway's duplicate-transaction check is bypassed.
        /// </summary>
        internal bool AllowDuplicates { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether partial authorizations are accepted.
        /// </summary>
        internal bool AllowPartialAuth { get; set; }
        /// <summary>
        /// Gets or sets the transaction amount.
        /// </summary>
        internal decimal? Amount { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the supplied amount is an estimate.
        /// </summary>
        internal bool? AmountEstimated { get; set; }
        /// <summary>
        /// Gets or sets the portion of the transaction amount that represents tax.
        /// </summary>
        internal decimal? AmountTaxed { get; set; }
        /// <summary>
        /// Gets or sets the pre-authorized amount used for incremental or completion requests.
        /// </summary>
        internal decimal? AuthAmount { get; set; }
        /// <summary>
        /// Gets or sets the auto-substantiation data for healthcare FSA/HSA card transactions.
        /// </summary>
        internal AutoSubstantiation AutoSubstantiation { get; set; }
        /// <summary>
        /// Gets or sets the balance inquiry type for EBT or gift card balance checks.
        /// </summary>
        internal InquiryType? BalanceInquiryType { get; set; }
        /// <summary>
        /// Gets or sets the billing address for the cardholder.
        /// </summary>
        internal Address BillingAddress { get; set; }
        /// <summary>
        /// Gets or sets the bills to be paid in a BillPay transaction.
        /// </summary>
        internal IEnumerable<Bill> Bills { get; set; }
        /// <summary>
        /// Gets or sets the customer associated with the transaction.
        /// </summary>
        internal Customer Customer { get; set; }
        /// <summary>
        /// Gets or sets the card-brand transaction ID used to link subsequent card-on-file transactions to the original.
        /// </summary>
        internal string CardBrandTransactionId { get; set; }
        /// <summary>
        /// Gets or sets the cash-back amount for debit or EBT cash-back transactions.
        /// </summary>
        internal decimal? CashBackAmount { get; set; }
        /// <summary>
        /// Gets or sets the client-assigned transaction ID used for timeout recovery and idempotency.
        /// </summary>
        internal string ClientTransactionId { get; set; }
        /// <summary>
        /// Gets or sets the Level II/III commercial card data for the transaction.
        /// </summary>
        internal CommercialData CommercialData { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this is a commercial purchasing card request.
        /// </summary>
        internal bool CommercialRequest { get; set; }
        /// <summary>
        /// Gets or sets the ISO 4217 currency code for the transaction (e.g. "USD").
        /// </summary>
        internal string Currency { get; set; }
        /// <summary>
        /// Gets or sets the merchant-assigned customer identifier.
        /// </summary>
        internal string CustomerId { get; set; }
        /// <summary>
        /// Gets or sets enriched customer profile data to include with the transaction.
        /// </summary>
        internal Customer CustomerData { get; set; }
        /// <summary>
        /// Gets or sets custom key/value string arrays to pass to the gateway.
        /// </summary>
        internal List<string[]> CustomData { get; set; }
        /// <summary>
        /// Gets or sets the cardholder's IP address for fraud-screening purposes.
        /// </summary>
        internal string CustomerIpAddress { get; set; }
        /// <summary>
        /// Gets or sets the card verification number (CVN/CVV2) for recurring or card-not-present transactions.
        /// </summary>
        internal string Cvn { get; set; }
        /// <summary>
        /// Gets or sets a free-text description to associate with the transaction.
        /// </summary>
        internal string Description { get; set; }
        /// <summary>
        /// Gets or sets the Cybersource Decision Manager fraud-screening data.
        /// </summary>
        internal DecisionManager DecisionManager { get; set; }
        /// <summary>
        /// Gets or sets the dynamic descriptor displayed on the cardholder's statement.
        /// </summary>
        internal string DynamicDescriptor { get; set; }
        /// <summary>
        /// Gets or sets the eCommerce-specific data (3D Secure, direct market) for the transaction.
        /// </summary>
        internal EcommerceInfo EcommerceInfo { get; set; }
        /// <summary>
        /// Gets or sets the EMV fallback condition when a chip read fails and mag-stripe fallback is required.
        /// </summary>
        internal EmvFallbackCondition? EmvFallbackCondition { get; set; }
        /// <summary>
        /// Gets or sets the outcome of the last EMV chip read attempt prior to mag-stripe fallback.
        /// </summary>
        internal EmvLastChipRead? EmvLastChipRead { get; set; }
        /// <summary>
        /// Gets or sets the fraud-filter mode to apply to the transaction.
        /// </summary>
        internal FraudFilterMode? FraudFilterMode { get; set; }
        /// <summary>
        /// Gets or sets the collection of fraud rules to evaluate during transaction processing.
        /// </summary>
        internal FraudRuleCollection FraudRules { get; set; }
        /// <summary>
        /// Gets or sets the gratuity (tip) amount; informational only, does not affect the authorization total.
        /// </summary>
        internal decimal? Gratuity { get; set; }
        /// <summary>
        /// Gets or sets the convenience fee amount added to the transaction.
        /// </summary>
        internal decimal? ConvenienceAmount { get; set; }
        /// <summary>
        /// Gets or sets the shipping fee amount included in the transaction.
        /// </summary>
        internal decimal? ShippingAmt { get; set; }
        /// <summary>
        /// Gets or sets the shipping discount amount applied to the transaction.
        /// </summary>
        internal decimal? ShippingDiscount { get; set; }
        /// <summary>
        /// Gets or sets order details (line items, tax, shipping) for Level III card-not-present transactions.
        /// </summary>
        internal OrderDetails OrderDetails { get; set; }
        /// <summary>
        /// Gets or sets the hosted payment page (HPP) data for Realex/GP-ECOM integrations.
        /// </summary>
        internal HostedPaymentData HostedPaymentData { get; set; }
        /// <summary>
        /// Gets or sets the idempotency key to prevent duplicate processing of the same request.
        /// </summary>
        internal string IdempotencyKey { get; set; }
        /// <summary>
        /// Gets or sets the invoice number associated with the transaction.
        /// </summary>
        internal string InvoiceNumber { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether Level II purchasing card data is present.
        /// </summary>
        internal bool Level2Request { get; set; }
        /// <summary>
        /// Gets or sets the lodging data for hotel or hospitality transactions (Portico gateway).
        /// </summary>
        internal LodgingData LodgingData { get; set; }
        /// <summary>
        /// Gets or sets the message authentication code (MAC) for network-secured terminal transactions.
        /// </summary>
        internal string MessageAuthenticationCode { get; set; }
        /// <summary>
        /// Gets or sets miscellaneous product line items associated with the transaction.
        /// </summary>
        internal List<Product> MiscProductData { get; set; }
        /// <summary>
        /// Gets or sets the offline authorization code obtained by calling the issuing bank directly.
        /// </summary>
        internal string OfflineAuthCode { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this is a one-time payment against a recurring profile.
        /// </summary>
        internal bool OneTimePayment { get; set; }
        /// <summary>
        /// Gets or sets the order identifier for the transaction.
        /// </summary>
        internal string OrderId { get; set; }
        /// <summary>
        /// Gets or sets the EMV payment application version string from the chip card.
        /// </summary>
        internal string PaymentApplicationVersion { get; set; }
        /// <summary>
        /// Gets or sets the payment method usage mode (single-use or multi-use token).
        /// </summary>
        internal PaymentMethodUsageMode? PaymentMethodUsageMode { get; set; }
        /// <summary>
        /// Gets or sets the cardholder's home phone number.
        /// </summary>
        public PhoneNumber HomePhone { get; set; }
        /// <summary>
        /// Gets or sets the cardholder's work phone number.
        /// </summary>
        public PhoneNumber WorkPhone { get; set; }
        /// <summary>
        /// Gets or sets the phone number at the shipping destination.
        /// </summary>
        public PhoneNumber ShippingPhone { get; set; }
        /// <summary>
        /// Gets or sets the remittance reference type for B2B payment identification.
        /// </summary>
        public RemittanceReferenceType? RemittanceReferenceType { get; set; }
        /// <summary>
        /// Gets or sets the remittance reference value that corresponds to <see cref="RemittanceReferenceType"/>.
        /// </summary>
        public string RemittanceReferenceValue { get; set; }
        /// <summary>
        /// Gets or sets the cardholder's mobile phone number.
        /// </summary>
        public PhoneNumber MobilePhone { get; set; }
        /// <summary>
        /// Gets or sets the POS sequence number for Canadian debit transactions.
        /// </summary>
        internal string PosSequenceNumber { get; set; }
        /// <summary>
        /// Gets or sets the merchant product identifier associated with the transaction.
        /// </summary>
        internal string ProductId { get; set; }
        /// <summary>
        /// Gets or sets the position in the recurring payment sequence (first, subsequent, or last).
        /// </summary>
        internal RecurringSequence? RecurringSequence { get; set; }
        /// <summary>
        /// Gets or sets the recurring payment type (fixed or variable amount).
        /// </summary>
        internal RecurringType? RecurringType { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether multi-use tokenization (card storage) should be requested.
        /// </summary>
        internal bool RequestMultiUseToken { get; set; }
        /// <summary>
        /// Indicates the storage mode for the payment method.
        /// </summary>
        internal StorageMode? StorageMode { get; set; }
        /// <summary>
        /// Gets or sets the replacement gift card for gift card replacement transactions.
        /// </summary>
        internal GiftCard ReplacementCard { get; set; }
        /// <summary>
        /// Gets or sets the reason code explaining why a reversal transaction is being performed.
        /// </summary>
        internal ReversalReasonCode? ReversalReasonCode { get; set; }
        /// <summary>
        /// Gets or sets the recurring schedule ID that links this transaction to a stored schedule.
        /// </summary>
        internal string ScheduleId { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the token should be shared across the merchant group.
        /// </summary>
        internal bool? ShareTokenWithGroup { get; set; } = null;
        /// <summary>
        /// Gets or sets the shipping destination address for the order.
        /// </summary>
        internal Address ShippingAddress { get; set; }
        /// <summary>
        /// Gets or sets the stored credential data for card-on-file or recurring transactions.
        /// </summary>
        internal StoredCredential StoredCredential { get; set; }
        /// <summary>
        /// Gets or sets gateway-specific supplementary data as a keyed dictionary of string arrays.
        /// </summary>
        internal Dictionary<string, List<string[]>> SupplementaryData { get; set; }
        /// <summary>
        /// Gets or sets the surcharge amount added on top of the base transaction total.
        /// </summary>
        internal decimal? SurchargeAmount { get; set; }
        /// <summary>
        /// Gets or sets the EMV tag data for chip card (ICC) transactions.
        /// </summary>
        internal string TagData { get; set; }
        /// <summary>
        /// Gets or sets the transaction timestamp in the format required by the gateway.
        /// </summary>
        internal string Timestamp { get; set; }
        /// <summary>
        /// Gets or sets the fee amount for fee-based or convenience-fee transactions.
        /// </summary>
        internal decimal FeeAmount { get; set; }
        /// <summary>
        /// Gets or sets the type of fee applied to the transaction (e.g. surcharge, convenience).
        /// </summary>
        internal FeeType FeeType { get; set; }
        /// <summary>
        /// Gets or sets the shift number for terminal-based clerk and shift tracking.
        /// </summary>
        internal string ShiftNumber { get; set; }
        /// <summary>
        /// Gets or sets the clerk identifier for terminal-based clerk tracking.
        /// </summary>
        internal string ClerkId { get; set; }
        /// <summary>
        /// Gets or sets the transport-layer data forwarded directly to the network (used in network gateway integrations).
        /// </summary>
        internal string TransportData { get; set; }
        /// <summary>
        /// Gets or sets the cardholder authentication method used at the point of interaction.
        /// </summary>
        internal CardHolderAuthenticationMethod? AuthenticationMethod { get; set; }
        /// <summary>
        /// Gets or sets the POS site configuration record for network-based transactions.
        /// </summary>
        internal RecordDataEntry POSSiteConfigRecord { get; set; }
        /// <summary>
        /// Gets or sets the customer identifier used for ACH/check transactions.
        /// </summary>
        internal string CheckCustomerId { get; set; }
        /// <summary>
        /// Gets or sets the raw MICR data read from the bottom of a physical check.
        /// </summary>
        internal string RawMICRData { get; set; }
        /// <summary>
        /// Gets or sets who initiated the stored credential transaction (merchant or cardholder).
        /// </summary>
        internal StoredCredentialInitiator? TransactionInitiator { get; set; }
        /// <summary>
        /// Gets or sets the card-on-file category indicator classifying the type of subsequent transaction.
        /// </summary>
        internal string CategoryIndicator {  get; set; }
        /// <summary>
        /// Gets or sets the BNPL (Buy Now Pay Later) shipping method for the order.
        /// </summary>
        internal BNPLShippingMethod BNPLShippingMethod {get;set;}
        /// <summary>
        /// Gets or sets a value indicating whether sensitive fields in the gateway response should be masked.
        /// </summary>
        internal bool MaskedDataResponse { get; set; }
        /// <summary>
        /// Gets or sets the card types to block from being used in this transaction.
        /// </summary>
        internal BlockedCardType CardTypesBlocking { get; set; }
        /// <summary>
        /// Gets or sets the merchant category used to classify the business for processing rules.
        /// </summary>
        internal MerchantCategory? MerchantCategory { get; set; }
        /// <summary>
        /// Gets or sets the credit or debit indicator used for surcharge calculation.
        /// </summary>
        internal CreditDebitIndicator? CreditDebitIndicator { get; set; }
        /// <summary>
        /// Gets a value indicating whether any EMV fallback data (condition, last chip read, or app version) has been populated.
        /// </summary>
        internal bool HasEmvFallbackData {
            get {
                return (EmvFallbackCondition != null || EmvLastChipRead != null || !string.IsNullOrEmpty(PaymentApplicationVersion));
            }
        }
        /// <summary>
        /// Gets or sets the EMV chip condition recorded during the chip read attempt.
        /// </summary>
        internal EmvLastChipRead EmvChipCondition { get; set; }
        /// <summary>
        /// Gets or sets the date the payment method was last registered or updated with the gateway.
        /// </summary>
        internal DateTime LastRegisteredDate { get; set; }
        /// <summary>
        /// Gets or sets the estimated shipping date for the order.
        /// </summary>
        internal DateTime ShippingDate { get; set; }
        /// <summary>
        /// Gets or sets supplemental transaction data forwarded to the Transaction API.
        /// </summary>
        internal TransactionData TransactionData { get; set; }
        /// <summary>
        /// Gets or sets the estimated number of transactions for a multi-capture authorization sequence.
        /// </summary>
        internal int? EstimatedNumberTransaction { get; set; }

        /// <summary>
        /// Gets or sets the cardholder presence indicator at the point of interaction.
        /// </summary>
        internal DE22_CardHolderPresence CardHolderPresence { get; set; }
        /// <summary>
        /// Sets the supplemental transaction data forwarded to the Transaction API; where applicable.
        /// </summary>
        /// <param name="value">The transaction data to attach to the request.</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithTransactionData(TransactionData value) {
            TransactionData = value;
            return this;
        }

        /// <summary>
        /// Sets the estimated shipping date for the order; where applicable.
        /// </summary>
        /// <param name="value">The estimated shipping date.</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithShippingDate(DateTime value)  {
            ShippingDate = value;
            return this;
        }
        /// <summary>
        /// Indicates the type of account provided; see the associated Type enumerations for specific values supported.
        /// </summary>
        /// <param name="value">AccountType</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithAccountType(AccountType value) {
            AccountType = value;
            return this;
        }

        /// <summary>
        /// Sets an address value; where applicable.
        /// </summary>
        /// <remarks>
        /// Currently supports billing and shipping addresses.
        /// </remarks>
        /// <param name="value">The desired address information</param>
        /// <param name="type">The desired address type</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithAddress(Address value, AddressType type = AddressType.Billing) {
            value.Type = type; // set the address type
            if (type == AddressType.Billing)
                BillingAddress = value;
            else ShippingAddress = value;
            return this;
        }

        /// <summary>
        /// Sets the gift-card alias and the action to perform on it (add or delete); where applicable.
        /// </summary>
        /// <param name="action">The alias action to perform.</param>
        /// <param name="value">The alias string to assign to the gift card.</param>
        /// <returns>AuthorizationBuilder</returns>
        internal AuthorizationBuilder WithAlias(AliasAction action, string value) {
            Alias = value;
            AliasAction = action;
            return this;
        }

        /// <summary>
        /// Allows duplicate transactions by skipping the
        /// gateway's duplicate checking.
        /// </summary>
        /// <param name="value">The duplicate skip flag</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithAllowDuplicates(bool value) {
            AllowDuplicates = value;
            return this;
        }

        /// <summary>
        /// Allows partial authorizations to occur.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="value">The allow partial flag</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithAllowPartialAuth(bool value) {
            AllowPartialAuth = value;
            return this;
        }

        /// <summary>
        /// Sets the transaction's amount
        /// </summary>
        /// <param name="value">The amount</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithAmount(decimal? value) {
            Amount = value;
            return this;
        }

        /// <summary>
        /// Sets a flag indicating whether the supplied amount is an estimate; where applicable.
        /// </summary>
        /// <param name="value"><c>true</c> if the amount is an estimate; otherwise <c>false</c>.</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithAmountEstimated(bool value) {
            AmountEstimated = value;
            return this;
        }

        /// <summary>
        /// Sets the portion of the transaction amount that represents tax; where applicable.
        /// </summary>
        /// <param name="value">The tax amount included in the total transaction amount.</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithAmountTaxed(decimal? value) {
            AmountTaxed = value;
            return this;
        }


        /// <summary>
        /// Sets the transaction's authorization amount; where applicable.
        /// </summary>
        /// <remarks>
        /// This is a specialized field. In most cases,
        /// `Authorization.WithAmount` should be used.
        /// </remarks>
        /// <param name="value">The authorization amount</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithAuthAmount(decimal? value) {
            AuthAmount = value;
            return this;
        }

        /// <summary>
        /// Sets the auto subtantiation values for the transaction.
        /// </summary>
        /// <param name="value">The auto substantiation object</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithAutoSubstantiation(AutoSubstantiation value) {
            AutoSubstantiation = value;
            return this;
        }

        /// <summary>
        /// Sets the Multicapture value as true/false.
        /// Sets the possible number of transactions
        /// </summary>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithMultiCapture(bool value = true, int? estimatedNumTrans = null) {
            MultiCapture = value;
            EstimatedNumberTransaction = estimatedNumTrans;
            return this;
        }

        /// <summary>
        /// Sets the balance inquiry type for EBT or gift card balance check transactions; where applicable.
        /// </summary>
        /// <param name="value">The type of balance inquiry to perform.</param>
        /// <returns>AuthorizationBuilder</returns>
        internal AuthorizationBuilder WithBalanceInquiryType(InquiryType? value) {
            BalanceInquiryType = value;
            return this;
        }

        /// <summary>
        /// Adds the bills to the transaction, where applicable
        /// </summary>
        /// <param name="values">The transaction's bills</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithBills(params Bill[] values) {
            Bills = values;
            return this;
        }

        /// <summary>
        /// Sets the customer, where applicable
        /// </summary>
        /// <param name="value">The transaction's customer</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithCustomer(Customer value) {
            Customer = value;
            return this;
        }

        /// <summary>
        /// Sets card brand storage (card-on-file) data indicating who initiated the transaction and linking it to the original brand transaction ID.
        /// </summary>
        /// <param name="transactionInitiator">Who initiated the transaction (merchant or cardholder).</param>
        /// <param name="cardBrandTransactionId">The card brand transaction ID from the original stored credential authorization.</param>
        /// <param name="categoryIndicator">The card-on-file category indicator classifying the type of subsequent transaction.</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithCardBrandStorage(
            StoredCredentialInitiator transactionInitiator,
            string cardBrandTransactionId = null,
            string categoryIndicator = null
        ) {
            TransactionInitiator = transactionInitiator;
            CardBrandTransactionId = cardBrandTransactionId;
            CategoryIndicator = categoryIndicator;
            return this;
        }

        /// <summary>
        /// Sets the cash back amount.
        /// </summary>
        /// <remarks>
        /// This is a specialized field for debit or EBT transactions.
        /// </remarks>
        /// <param name="value">The desired cash back amount</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithCashBack(decimal? value) {
            CashBackAmount = value;
            TransactionModifier = TransactionModifier.CashBack;
            return this;
        }

        /// <summary>
        /// Sets the client transaction ID.
        /// </summary>
        /// <remarks>
        /// This is an application derived value that can be used to identify a
        /// transaction in case a gateway transaction ID is not returned, e.g.
        /// in cases of timeouts.
        ///
        /// The supplied value should be unique to the configured merchant or
        /// terminal account.
        /// </remarks>
        /// <param name="value">The client transaction ID</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithClientTransactionId(string value) {
            if (TransactionType == TransactionType.Reversal || TransactionType == TransactionType.Refund) {
                if (PaymentMethod is TransactionReference) {
                    ((TransactionReference)PaymentMethod).ClientTransactionId = value;
                }
                else if(PaymentMethod is eCheck) {
                    ClientTransactionId = value;
                }
                else if(PaymentMethod is ICardData) {
                    ClientTransactionId = value;
                }
                else if (PaymentMethod is Credit) {
                    ClientTransactionId = value;
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

        /// <summary>
        /// Sets the transaction's currency; where applicable.
        /// </summary>
        /// <remarks>
        /// The formatting for the supplied value will currently depend on
        /// the configured gateway's requirements.
        /// </remarks>
        /// <param name="value">The currency</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithCurrency(string value) {
            Currency = value;
            return this;
        }

        /// <summary>
        /// Sets the fraud filter mode and optional fraud rule collection to apply during transaction processing.
        /// </summary>
        /// <param name="fraudFilter">The fraud filter mode (e.g. active, passive, off).</param>
        /// <param name="fraudRules">An optional collection of specific fraud rules to apply.</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithFraudFilter(FraudFilterMode fraudFilter, FraudRuleCollection fraudRules = null) {
            FraudFilterMode = fraudFilter;
            if(fraudRules != null)
                FraudRules = fraudRules;
            return this;
        }

        /// <summary>
        /// Appends a row of custom key/value string data to be passed to the gateway; where applicable.
        /// </summary>
        /// <param name="values">One or more string values forming a single custom data row.</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithCustomData(params string[] values) {
            if (CustomData == null) {
                CustomData = new List<string[]>();
            }
            CustomData.Add(values);

            return this;
        }

        /// <summary>
        /// Sets enriched customer profile data to include with the transaction; where applicable.
        /// </summary>
        /// <param name="value">The customer object containing profile details.</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithCustomerData(Customer value) {
            CustomerData = value;
            return this;
        }

        /// <summary>
        /// Sets the customer ID; where applicable.
        /// </summary>
        /// <remarks>
        /// This is an application/merchant generated value.
        /// </remarks>
        /// <param name="value">The customer ID</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithCustomerId(string value) {
            CustomerId = value;
            return this;
        }

        /// <summary>
        /// Sets the customer's IP address; where applicable.
        /// </summary>
        /// <remarks>
        /// This value should be obtained during the payment process.
        /// </remarks>
        /// <param name="value">The customer's IP address</param>
        /// <returns>AuthorizationBuilder</returns>
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

        /// <summary>
        /// Sets the POS site configuration record for network-based transactions; where applicable.
        /// </summary>
        /// <param name="value">The POS site configuration record data entry.</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithPOSSiteConfigRecord(RecordDataEntry value) {
            POSSiteConfigRecord = value;
            return this;
        }

        /// <summary>
        /// Sets the Dynamic Currency Conversion (DCC) rate data for the transaction; where applicable.
        /// </summary>
        /// <param name="value">The DCC rate data containing exchange rate and currency information.</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithDccRateData(DccRateData value) {
            DccRateData = value;
            return this;
        }

        /// <summary>
        /// Sets the Cybersource Decision Manager fraud-screening data for the transaction; where applicable.
        /// </summary>
        /// <param name="value">The Decision Manager data used for fraud analysis.</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithDecisionManager(DecisionManager value) {
            DecisionManager = value;
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
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithDescription(string value) {
            Description = value;
            return this;
        }

        /// <summary>
        /// Sets the transaction's dynamic descriptor.
        /// </summary>
        /// <remarks>
        /// This value is sent during the authorization process and is displayed
        /// in the consumer's account.
        /// </remarks>
        /// <param name="value">The dynamic descriptor</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithDynamicDescriptor(string value) {
            DynamicDescriptor = value;
            return this;
        }

        /// <summary>
        /// Sets eCommerce specific data; where applicable.
        /// </summary>
        /// <remarks>
        /// This can include:
        ///
        ///   - Consumer authentication (3DSecure) data
        ///   - Direct market data
        /// </remarks>
        /// <param name="value">The eCommerce data</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithEcommerceInfo(EcommerceInfo value) {
            EcommerceInfo = value;
            return this;
        }

        /// <summary>
        /// This must be provided when the POS was not able to successfully communicate to the chip card and was required to fall back to a magnetic stripe read on an EMV capable terminal.
        /// </summary>
        /// <remarks>
        /// The values can indicate multiple factors:
        ///
        ///   - The EMV chip read failed
        ///   - Did the previous attempt fail
        /// </remarks>
        /// <param name="value">EmvChipCondition</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithEmvFallbackData(EmvFallbackCondition condition, EmvLastChipRead lastRead, string appVersion = null) {
            EmvFallbackCondition = condition;
            EmvLastChipRead = lastRead;
            PaymentApplicationVersion = appVersion;

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
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithGratuity(decimal? value) {
            Gratuity = value;
            return this;
        }

        /// <summary>
        /// Sets the Convenience amount; where applicable.
        /// </summary>
        /// <param name="value">The Convenience amount</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithConvenienceAmount(decimal? value) {
            ConvenienceAmount = value;
            return this;
        }

        /// <summary>
        /// Sets the Shipping amount; where applicable.
        /// </summary>
        /// <param name="value">The Shipping amount</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithShippingAmt(decimal? value) {
            ShippingAmt = value;
            return this;
        }

        /// <summary>
        /// Set the request shippingDiscount; where applicable.
        /// </summary>
        /// <param name="value">The shippingDiscount</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithShippingDiscount(decimal? value) {
            ShippingDiscount = value;
            return this;
        }

        /// <summary>
        /// Sets the order details (line items, tax, and shipping) for Level III card-not-present transactions; where applicable.
        /// </summary>
        /// <param name="value">The OrderDetails</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithOrderDetails(OrderDetails value) {
            OrderDetails = value;
            return this;
        }

        /// <summary>
        /// Additional hosted payment specific information for Realex HPP implementation.
        /// </summary>
        /// <param name="value">The hosted payment data</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithHostedPaymentData(HostedPaymentData value) {
            HostedPaymentData = value;
            return this;
        }

        /// <summary>
        /// Field submitted in the request that is used to ensure idempotency is maintained within the action
        /// </summary>
        /// <param name="value">The idempotency key</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithIdempotencyKey(string value) {
            IdempotencyKey = value;
            return this;
        }

        /// <summary>
        /// Sets the invoice number; where applicable.
        /// </summary>
        /// <param name="value">The invoice number</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithInvoiceNumber(string value) {
            InvoiceNumber = value;
            return this;
        }

        /// <summary>
        /// Sets the commercial purchasing card flag for the transaction; where applicable.
        /// </summary>
        /// <remarks>
        /// When set to <c>true</c>, the gateway will return prompts for Level II/III data when applicable.
        /// </remarks>
        /// <param name="value">The commercial request flag.</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithCommercialRequest(bool value) {
            CommercialRequest = value;
            return this;
        }

        /// <summary>
        /// Sets the Level II/III commercial card data for the transaction; where applicable.
        /// </summary>
        /// <param name="value">The commercial data (tax amounts, line items, purchase order number, etc.).</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithCommercialData(CommercialData value) {
            CommercialData = value;
            return this;
        }

        /// <summary>
        /// Sets the date the payment method was last registered or updated with the gateway; where applicable.
        /// </summary>
        /// <param name="value">The date the card or token was last registered.</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithLastRegisteredDate(DateTime value) {
            LastRegisteredDate = value;
            return this;
        }

        /// <summary>
        /// Sets the commercial request flag; where applicable.
        /// </summary>
        /// <remarks>
        /// This flag indicates commercial purchase cards are accepted/expected.
        /// The application should inspect the transaction response and pass the
        /// appropriate Level II data when necessary.
        /// </remarks>
        /// <param name="value">The commercial request flag</param>
        /// <returns>AuthorizationBuilder</returns>
        //public AuthorizationBuilder WithCommercialRequest(bool value) {
        //    Level2Request = value;
        //    return this;
        //}

        /// <summary>
        /// Sets the message authentication code; where applicable.
        /// </summary>
        /// <param name="value">A special block of encrypted data added to every transaction when it is sent from the payment terminal to the payment processor.</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithMessageAuthenticationCode(string value) {
            MessageAuthenticationCode = value;
            return this;
        }

        /// <summary>
        /// Sets the list of miscellaneous product line items to associate with the transaction; where applicable.
        /// </summary>
        /// <param name="values">The list of product objects describing each line item.</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithMiscProductData(List<Product> values) {
            MiscProductData = values;
            return this;
        }

        /// <summary>
        /// Sets the offline authorization code; where applicable.
        /// </summary>
        /// <remarks>
        /// The merchant is required to supply this value as obtained when
        /// calling the issuing bank for the authorization.
        /// </remarks>
        /// <param name="value">The offline authorization code</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithOfflineAuthCode(string value) {
            OfflineAuthCode = value;
            TransactionModifier = TransactionModifier.Offline;
            return this;
        }

        /// <summary>
        /// Sets the one-time payment flag; where applicable.
        /// </summary>
        /// <remarks>
        /// This is only useful when using recurring payment profiles for
        /// one-time payments that are not a part of a recurring schedule.
        /// </remarks>
        /// <param name="value">The one-time flag</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithOneTimePayment(bool value) {
            OneTimePayment = value;
            TransactionModifier = TransactionModifier.Recurring;
            return this;
        }

        /// <summary>
        /// Sets the transaction's order ID; where applicable.
        /// </summary>
        /// <param name="value">The order ID</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithOrderId(string value) {
            OrderId = value;
            return this;
        }

        /// <summary>
        /// Sets the EMV payment application version string read from the chip card; where applicable.
        /// </summary>
        /// <param name="value">The application version number from the chip card's application data.</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithPaymentApplicationVersion(string value) {
            PaymentApplicationVersion = value;
            return this;
        }

        /// <summary>
        /// Sets the payment method usage mode to differentiate single-use from multi-use token requests; where applicable.
        /// </summary>
        /// <param name="value">The payment method usage mode (single-use or multi-use).</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithPaymentMethodUsageMode(PaymentMethodUsageMode? value) {
            PaymentMethodUsageMode = value;
            return this;
        }

        /// <summary>
        /// Sets the transaction's payment method.
        /// </summary>
        /// <param name="value">The payment method</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithPaymentMethod(IPaymentMethod value) {
            PaymentMethod = value;
            if (value is EBTCardData && ((EBTCardData)value).SerialNumber != null)
                TransactionModifier = TransactionModifier.Voucher;
            if(value is CreditCardData  && ((CreditCardData)value).MobileType !=null)
                TransactionModifier = TransactionModifier.EncryptedMobile;
            return this;
        }

        /// <summary>
        /// Sets the POS Sequence Number; where applicable.
        /// </summary>
        /// <param name="value">POS sequence number for Canadian Debit transactions.</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithPosSequenceNumber(string value) {
            PosSequenceNumber = value;
            return this;
        }

        /// <summary>
        /// Sets the transaction's product ID; where applicable.
        /// </summary>
        /// <param name="value">The product ID</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithProductId(string value) {
            ProductId = value;
            return this;
        }

        /// <summary>
        /// Sets the Recurring Info for Realex based recurring payments;
        /// where applicable.
        /// </summary>
        /// <param name="type">
        /// The value can be 'fixed' or 'variable' depending on whether
        /// the amount will change for each transaction.
        /// </param>
        /// <param name="sequence">
        /// Indicates where in the recurring sequence the transaction
        /// occurs. Must be 'first' for the first transaction for this
        /// card, 'subsequent' for transactions after that, and 'last'
        /// for the final transaction of the set.
        /// </param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithRecurringInfo(RecurringType type, RecurringSequence sequence) {
            RecurringSequence = sequence;
            RecurringType = type;
            return this;
        }

        /// <summary>
        /// Requests multi-use tokenization / card storage.
        /// </summary>
        /// <remarks>
        /// This will depend on a successful transaction. If there was a failure
        /// or decline, the multi-use tokenization / card storage will not be
        /// successful.
        /// </remarks>
        /// <param name="value">The request flag</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithRequestMultiUseToken(bool value) {
            RequestMultiUseToken = value;
            return this;
        }

        /// <summary>
        /// Indicates the storage mode for the payment method; where applicable.
        /// </summary>
        /// <param name="value">The storage mode (e.g. <see cref="StorageMode.StoreOnSuccess"/>) to apply.</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithStorageMode(StorageMode value) {
            StorageMode = value;
            return this;
        }

        /// <summary>
        /// Sets the replacement gift card for a gift card replacement transaction; where applicable.
        /// </summary>
        /// <param name="value">The new gift card to assign as the replacement card.</param>
        /// <returns>AuthorizationBuilder</returns>
        internal AuthorizationBuilder WithReplacementCard(GiftCard value) {
            ReplacementCard = value;
            return this;
        }

        /// <summary>
        /// Sets the reason code explaining why a reversal is being performed; where applicable.
        /// </summary>
        /// <param name="value">The reversal reason code.</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithReversalReasonCode(ReversalReasonCode value) {
            ReversalReasonCode = value;
            return this;
        }

        /// <summary>
        /// Sets the Pay-by-Link data for link-based payment requests; where applicable.
        /// </summary>
        /// <param name="payByLinkData">The Pay-by-Link configuration data.</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithPayByLinkData(PayByLinkData payByLinkData) {
            PayByLinkData = payByLinkData;
            return this;
        }

        /// <summary>
        /// Sets the payment link ID used to associate the transaction with an existing Pay-by-Link resource; where applicable.
        /// </summary>
        /// <param name="paymentLinkId">The unique identifier of the payment link.</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithPaymentLinkId(string paymentLinkId) {
            PaymentLinkId = paymentLinkId;
            return this;
        }

        /// <summary>
        /// Sets the schedule ID associated with the transaction; where applicable.
        /// </summary>
        /// <remarks>
        /// This is specific to transactions against recurring profiles that are
        /// a part of a recurring schedule.
        /// </remarks>
        /// <param name="value">The schedule ID</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithScheduleId(string value) {
            ScheduleId = value;
            return this;
        }

        /// <summary>
        /// Sets the ShareTokenWithGroup value to be used for updating the BillPay token.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithShareTokenWithGroup(bool? value) {
            ShareTokenWithGroup = value;
            return this;
        }

        /// <summary>
        /// Sets the stored credential data for card-on-file or recurring transactions; where applicable.
        /// </summary>
        /// <param name="value">The stored credential details (initiator, type, sequence, and card brand transaction ID).</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithStoredCredential(StoredCredential value) {
            StoredCredential = value;
            return this;
        }

        /// <summary>
        /// Appends gateway-specific supplementary data as a typed key/value collection; where applicable.
        /// </summary>
        /// <param name="type">The supplementary data type key recognized by the gateway.</param>
        /// <param name="values">One or more string values associated with the type.</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithSupplementaryData(string type, params string[] values) {
            // create the dictionary
            if (SupplementaryData == null) {
                SupplementaryData = new Dictionary<string, List<string[]>>();
            }

            // add the key
            if (!SupplementaryData.ContainsKey(type)) {
                SupplementaryData.Add(type, new List<string[]>());
            }

            // add the values to it
            SupplementaryData[type].Add(values);

            return this;
        }

        /// <summary>
        /// Sets the surcharge amount for the transaction.
        /// </summary>
        public AuthorizationBuilder WithSurchargeAmount(decimal value) {
            SurchargeAmount = value;
            return this;
        }

        /// <summary>
        /// Sets the related gateway transaction ID; where applicable.
        /// </summary>
        /// <remarks>
        /// This value is used to associated a previous transaction with the
        /// current transaction.
        /// </remarks>
        /// <param name="value">The gateway transaction ID</param>
        /// <returns>AuthorizationBuilder</returns>
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

        /// <summary>
        /// Sets the transaction modifier to adjust authorization behavior (e.g. hosted request, offline, cash back); where applicable.
        /// </summary>
        /// <param name="value">The transaction modifier to apply.</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithModifier(TransactionModifier value) {
            TransactionModifier = value;
            return this;
        }

        /// <summary>
        /// Sets the EMV tag data to be sent along with an EMV transaction.
        /// </summary>
        /// <param name="value">the EMV tag data</param>
        /// <returns>Authorization Builder</returns>
        public AuthorizationBuilder WithTagData(string value) {
            TagData = value;
            return this;
        }

        /// <summary>
        /// Sets the timestamp; where applicable.
        /// </summary>
        /// <param name="value">The transaction's timestamp</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithTimestamp(string value) {
            Timestamp = value;
            return this;
        }


        /// <summary>
        /// Lodging data information for Portico
        /// </summary>
        /// <param name="value">The lodging data</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithLodgingData(LodgingData value) {
            LodgingData = value;
            return this;
        }

        /// <summary>
        /// Sets the surcharge amount; where applicable.
        /// </summary>
        /// <param name="value">The surcharge amount</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithSurchargeAmount(decimal? value, CreditDebitIndicator? creditDebitIndicator = null) {
            SurchargeAmount = value;
            CreditDebitIndicator = creditDebitIndicator;
            return this;
        }

        /// <summary>
        /// Sets a flag indicating whether sensitive fields in the gateway response should be masked; where applicable.
        /// </summary>
        /// <param name="value"><c>true</c> to enable masking of sensitive response data; otherwise <c>false</c>.</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithMaskedDataResponse(bool value) {
            MaskedDataResponse = value;
            return this;
        }

        /// <summary>
        /// Sets the card types to block from being used in this transaction; where applicable.
        /// </summary>
        /// <param name="cardTypesBlocking">A <see cref="BlockedCardType"/> value specifying which card brands to reject.</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithBlockedCardType(BlockedCardType cardTypesBlocking) {
            var hasNulls = cardTypesBlocking.GetType().GetTypeInfo().DeclaredFields.All(p => p.GetValue(cardTypesBlocking) == null);
            if (hasNulls) {            
                throw new BuilderException("No properties set on the object");
            }
            CardTypesBlocking = cardTypesBlocking;

            return this;
        }

        /// <summary>
        /// Sets the merchant category to classify the business type for processing rules; where applicable.
        /// </summary>
        /// <param name="value">The merchant category indicator.</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithMerchantCategory(MerchantCategory value) {
            MerchantCategory = value;
            return this;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="AuthorizationBuilder"/> with the specified transaction type and optional payment method.
        /// </summary>
        /// <param name="type">The type of transaction to build (e.g. <see cref="TransactionType.Sale"/>, <see cref="TransactionType.Auth"/>).</param>
        /// <param name="payment">The optional payment method to associate with the transaction.</param>
        internal AuthorizationBuilder(TransactionType type, IPaymentMethod payment = null) : base(type) {
            WithPaymentMethod(payment);
        }

        /// <summary>
        /// Executes the authorization builder against the gateway.
        /// </summary>
        /// <returns>Transaction</returns>
        public override Transaction Execute(string configName = "default") {
            base.Execute(configName);

            var client = ServicesContainer.Instance.GetClient(configName);
            if (client.SupportsOpenBanking && PaymentMethod is BankPayment) {
            var obClient = ServicesContainer.Instance.GetOpenBanking(configName);
                if (obClient != null && (obClient != client)) {
                    return obClient.ProcessOpenBanking(this);
                }
            }
            return client.ProcessAuthorization(this);
        }

        /// <summary>
        /// Serializes an authorization builder for hosted payment page requests.
        /// </summary>
        /// <remarks>
        /// Requires the gateway and account support hosted payment pages.
        /// </remarks>
        /// <returns>string</returns>
        public string Serialize(string configName = "default") {
            TransactionModifier = TransactionModifier.HostedRequest;
            base.Execute();

            var client = ServicesContainer.Instance.GetClient(configName);
            if (client.SupportsHostedPayments) {
                return client.SerializeRequest(this);
            }
            throw new UnsupportedTransactionException("You current gateway does not support hosted payments.");
        }

        protected override void SetupValidations() {
           
            #region ENUM VALIDATION WITH FLAG ATTRIBUTE            

            Validations.For(PaymentMethodType.Debit | PaymentMethodType.Credit)
                .When(() => HasEmvFallbackData).IsTrue()
                .Check(() => TagData).IsNull();

            Validations.For(PaymentMethodType.Debit | PaymentMethodType.Credit)
                .When(() => TagData).IsNotNull()
                .Check(() => HasEmvFallbackData).IsFalse();

            Validations.For(PaymentMethodType.Recurring)
                .Check(() => ShippingAmt).IsNull();

            Validations.For(PaymentMethodType.Debit)
                .When(() => ReversalReasonCode).IsNotNull()
                .Check(() => TransactionType).Equals(TransactionType.Reversal);

            #endregion



            Validations.For(TransactionType.Auth)
                .With(TransactionModifier.None)
                .Check(() => Amount).IsNotNull()
                .Check(() => Currency).IsNotNull()
                .Check(() => PaymentMethod).IsNotNull();

            Validations.For(TransactionType.Sale)
                .With(TransactionModifier.None)
                .Check(() => Amount).IsNotNull()
                .Check(() => Currency).IsNotNull()
                .Check(() => PaymentMethod).IsNotNull();

            Validations.For(TransactionType.Refund)
                .With(TransactionModifier.None)
                .Check(() => Amount).IsNotNull()
                .Check(() => Currency).IsNotNull()
                .Check(() => PaymentMethod).IsNotNull();

            Validations.For(TransactionType.AddValue)
                .With(TransactionModifier.None)
                .Check(() => Amount).IsNotNull()
                .Check(() => Currency).IsNotNull()
                .Check(() => PaymentMethod).IsNotNull();

            Validations.For(TransactionType.Auth)
                .With(TransactionModifier.HostedRequest)
                .Check(() => Amount).IsNotNull()
                .Check(() => Currency).IsNotNull();

            Validations.For(TransactionType.Sale)
                .With(TransactionModifier.HostedRequest)
                .Check(() => Amount).IsNotNull()
                .Check(() => Currency).IsNotNull();

            Validations.For(TransactionType.Verify)
                .With(TransactionModifier.HostedRequest)
                .Check(() => Currency).IsNotNull();

            Validations.For(TransactionType.Auth)
                .With(TransactionModifier.Offline)
                .Check(() => Amount).IsNotNull()
                .Check(() => Currency).IsNotNull()
                .Check(() => OfflineAuthCode).IsNotNull();

            Validations.For(TransactionType.Sale)
                .With(TransactionModifier.Offline)
                .Check(() => Amount).IsNotNull()
                .Check(() => Currency).IsNotNull()
                .Check(() => OfflineAuthCode).IsNotNull();

            Validations.For(TransactionType.BenefitWithdrawal)
                .With(TransactionModifier.CashBack)
                .Check(() => Amount).IsNotNull()
                .Check(() => Currency).IsNotNull()
                .Check(() => PaymentMethod).IsNotNull();

            Validations.For(TransactionType.Balance)
                .Check(() => PaymentMethod).IsNotNull();

            Validations.For(TransactionType.Alias)
                .Check(() => AliasAction).IsNotNull()
                .Check(() => Alias).IsNotNull();

            Validations.For(TransactionType.Replace)
                .Check(() => ReplacementCard).IsNotNull();           

            Validations.For(TransactionType.Auth)
                .With(TransactionModifier.EncryptedMobile)
                .Check(() => PaymentMethod).IsNotNull();

            Validations.For(TransactionType.Sale)
                .With(TransactionModifier.EncryptedMobile)
                .Check(() => PaymentMethod).IsNotNull();          

            Validations.For(TransactionType.Sale)
                .With(TransactionModifier.AlternativePaymentMethod)
                .Check(() => PaymentMethod).IsNotNull()
                .Check(() => Amount).IsNotNull()
                .Check(() => Currency).IsNotNull()                
                .Check(() => PaymentMethod).PropertyOf(nameof(AlternativePaymentMethod.StatusUpdateUrl)).IsNotNull()
                .Check(() => PaymentMethod).PropertyOf(nameof(AlternativePaymentMethod.ReturnUrl)).IsNotNull()
                .Check(() => PaymentMethod).PropertyOf(nameof(AlternativePaymentMethod.AccountHolderName)).IsNotNull();

            Validations.For(TransactionType.Auth)
                .With(TransactionModifier.AlternativePaymentMethod)
                .Check(() => PaymentMethod).IsNotNull()
                .Check(() => Amount).IsNotNull()
                .Check(() => Currency).IsNotNull()
                .Check(() => PaymentMethod).PropertyOf(nameof(AlternativePaymentMethod.StatusUpdateUrl)).IsNotNull()
                .Check(() => PaymentMethod).PropertyOf(nameof(AlternativePaymentMethod.ReturnUrl)).IsNotNull()
                .Check(() => PaymentMethod).PropertyOf(nameof(AlternativePaymentMethod.AccountHolderName)).IsNotNull();


        }

        /// <summary>
        /// Sets the phone number for the cardholder's specified phone type (home, work, shipping, or mobile); where applicable.
        /// </summary>
        /// <param name="phoneCountryCode">The ITU-T E.164 country calling code (e.g. "1" for US).</param>
        /// <param name="number">The subscriber phone number (without the country calling code).</param>
        /// <param name="type">The type of phone number being set.</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithPhoneNumber(string phoneCountryCode, string number, PhoneNumberType type) {
            var phoneNumber = new PhoneNumber {
                CountryCode = phoneCountryCode,
                Number = number
            };
            switch (type) {
                case PhoneNumberType.Home:
                    HomePhone = phoneNumber;
                    break;
                case PhoneNumberType.Work:
                    WorkPhone = phoneNumber;
                    break;
                case PhoneNumberType.Shipping:
                    ShippingPhone = phoneNumber;
                    break;
                case PhoneNumberType.Mobile:
                    MobilePhone = phoneNumber;
                    break;
                default:
                    break;
            }
            return this;
        }

        /// <summary>
        /// Forces a gateway timeout for testing or retry scenarios; where applicable.
        /// </summary>
        /// <param name="value"><c>true</c> to force a gateway timeout; otherwise <c>false</c>.</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithForceGatewayTimeout(bool value) {
            ForceGatewayTimeout = value;
            return this;
        }

        /// <summary>
        /// Sets a fee to be applied on top of the transaction amount; where applicable.
        /// </summary>
        /// <param name="feeType">The type of fee (e.g. surcharge, convenience).</param>
        /// <param name="feeAmount">The fee amount to add to the transaction.</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithFee(FeeType feeType, decimal feeAmount) {
            FeeType = feeType;
            FeeAmount = feeAmount;

            return this;
        }

        /// <summary>
        /// Sets the unique device identifier for the terminal or POS device initiating the transaction; where applicable.
        /// </summary>
        /// <param name="value">The unique device ID string assigned to the terminal.</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithUniqueDeviceId(string value) {
            UniqueDeviceId = value;
            return this;
        }

        /// <summary>
        /// Sets the clerk identifier for terminal-based clerk tracking; where applicable.
        /// </summary>
        /// <param name="value">The clerk ID string assigned to the POS operator.</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithClerkId(string value) {
            ClerkId = value;
            return this;
        }

        /// <summary>
        /// Sets the shift number for terminal-based shift tracking; where applicable.
        /// </summary>
        /// <param name="value">The shift number assigned to the current terminal session.</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithShiftNumber(string value) {
            ShiftNumber = value;
            return this;
        }

        /// <summary>
        /// Sets the transport-layer data forwarded directly to the network; used in network gateway integrations.
        /// </summary>
        /// <param name="value">The raw transport data string.</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithTransportData(string value) {
            TransportData = value;
            return this;
        }

        /// <summary>
        /// Sets the batch number for the transaction; where applicable.
        /// </summary>
        /// <param name="value">The batch number assigned by the terminal or POS.</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithBatchNumber(int value) {
            BatchNumber = value;
            return this;
        }

        /// <summary>
        /// Sets the batch number and sequence number for the transaction; where applicable.
        /// </summary>
        /// <param name="batchNumber">The batch number assigned by the terminal or POS.</param>
        /// <param name="sequenceNumber">The sequence number of the transaction within the batch.</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithBatchNumber(int batchNumber, int sequenceNumber) {
            BatchNumber = batchNumber;
            SequenceNumber = sequenceNumber;
            return this;
        }

        /// <summary>
        /// Sets the company ID associated with the transaction; where applicable.
        /// </summary>
        /// <param name="value">The company identifier string.</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithCompanyId(string value) {
            CompanyId = value;
            return this;
        }

        /// <summary>
        /// Sets the fleet card data for fleet-specific purchasing card transactions; where applicable.
        /// </summary>
        /// <param name="value">The fleet data containing driver, vehicle, and purchase details.</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithFleetData(FleetData value) {
            FleetData = value;
            return this;
        }

        /// <summary>
        /// [Obsolete] Sets the full issuer data dictionary for the transaction. Use <see cref="WithFleetData"/> on <c>TransactionBuilder</c> instead.
        /// </summary>
        /// <param name="value">A dictionary mapping card issuer entry tags to their values.</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithIssuerData(Dictionary<DE62_CardIssuerEntryTag, string> value){
            IssuerData = value;
            return this;
        }

        /// <summary>
        /// Appends a single issuer data entry (tag/value pair) to the card issuer data collection; where applicable.
        /// </summary>
        /// <param name="tag">The DE62 card issuer entry tag identifying the data field.</param>
        /// <param name="value">The string value associated with the tag.</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithIssuerData(DE62_CardIssuerEntryTag tag, string value) {
            if (IssuerData == null) {
                IssuerData = new Dictionary<DE62_CardIssuerEntryTag, string>();
            }
            IssuerData.Add(tag, value);
            return this;
        }

        /// <summary>
        /// Sets the system trace audit number (STAN) for the transaction; where applicable.
        /// </summary>
        /// <param name="value">The system trace audit number assigned by the terminal or host.</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithSystemTraceAuditNumber(int value) {
            SystemTraceAuditNumber = value;
            return this;
        }        

        /// <summary>
        /// Sets the transaction matching data used to associate this transaction with a previous one; where applicable.
        /// </summary>
        /// <param name="value">The transaction matching data containing identifiers from the original transaction.</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithTransactionMatchingData(TransactionMatchingData value) {
            TransactionMatchingData = value;
            return this;
        }

        /// <summary>
        /// Sets the EMV chip condition recorded when a chip read was attempted but fell back to mag-stripe; where applicable.
        /// </summary>
        /// <param name="value">The last chip read result (e.g. successful, failed, no chip).</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithChipCondition(EmvLastChipRead value) {
            EmvChipCondition = value;
            return this;
        }

        /// <summary>
        /// Sets the product data for fuel, fleet, or itemized purchase transactions; where applicable.
        /// </summary>
        /// <param name="value">The product data containing item details and quantities.</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithProductData(ProductData value) {
            ProductData = value;
            return this;
        }

        /// <summary>
        /// Sets the Electronic WIC (EWIC) data for EBT Women, Infants and Children transactions; where applicable.
        /// </summary>
        /// <param name="eWicData">The EWIC data containing item UPCs and quantities.</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithEWICData(EWICData eWicData) {
            EwicData = eWicData;
            return this;
        }

        /// <summary>
        /// Sets the cardholder authentication method used at the point of interaction; where applicable.
        /// </summary>
        /// <param name="value">The cardholder authentication method (e.g. PIN, signature, unattended).</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithAuthenticationMethod(CardHolderAuthenticationMethod value) {
            AuthenticationMethod = value;
            return this;
        }

        /// <summary>
        /// Sets the EWIC issuing entity identifier for Electronic WIC transactions; where applicable.
        /// </summary>
        /// <param name="value">The WIC issuing entity identifier string.</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithEWICIssuingEntity(string value) {
            EWICIssuingEntity = value;
            return this;
        }

        /// <summary>
        /// Sets the customer identifier for ACH/check transactions; where applicable.
        /// </summary>
        /// <param name="value">The customer ID used to identify the check account holder.</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithCheckCustomerId(string value) {
            CheckCustomerId = value;
            return this;
        }

        /// <summary>
        /// Sets the raw MICR data read from the bottom of a physical check; where applicable.
        /// </summary>
        /// <param name="value">The raw MICR string containing routing, account, and check numbers.</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithRawMICRData(string value) {
            RawMICRData = value;
            return this;
        }

        
        /// <summary>
        /// Sets the remittance reference type and value for B2B payment identification; where applicable.
        /// </summary>
        /// <param name="remittanceReferenceType">The reference type classifying the remittance information.</param>
        /// <param name="remittanceReferenceValue">The remittance reference value (e.g. invoice number, purchase order number).</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithRemittanceReference(RemittanceReferenceType remittanceReferenceType, string remittanceReferenceValue) {
            RemittanceReferenceType = remittanceReferenceType;
            RemittanceReferenceValue = remittanceReferenceValue;
            return this;
        }

        /// <summary>
        /// Sets the BNPL (Buy Now Pay Later) shipping method for the order; where applicable.
        /// </summary>
        /// <param name="value">The BNPL shipping method to apply to the order.</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithBNPLShippingMethod(BNPLShippingMethod value) {
            if (!(PaymentMethod is BNPL)) {
                throw new ArgumentException("The selected payment method doesn't support this property!");
            }

            BNPLShippingMethod = value;
            return this;
        }

        /// <summary>
        /// Sets the installment payment data for the transaction; where applicable.
        /// </summary>
        /// <param name="installmentData">The installment payment data to attach to the request.</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithInstallmentData(InstallmentData installmentData) {
            InstallmentData = installmentData;
            return this;
        }

        /// <summary>
        /// Sets the cardholder presence indicator at the point of interaction; where applicable.
        /// </summary>
        /// <param name="value">The DE22 cardholder presence indicator value.</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithCardHolderPresence(DE22_CardHolderPresence value) {
            CardHolderPresence = value;
            return this;
        }
    }
}