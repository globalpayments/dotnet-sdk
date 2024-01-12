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
        internal AccountType? AccountType { get; set; }
        internal string Alias { get; set; }
        internal AliasAction? AliasAction { get; set; }
        internal bool AllowDuplicates { get; set; }
        internal bool AllowPartialAuth { get; set; }
        internal decimal? Amount { get; set; }
        internal bool AmountEstimated { get; set; }
        internal decimal? AmountTaxed { get; set; }
        internal decimal? AuthAmount { get; set; }
        internal AutoSubstantiation AutoSubstantiation { get; set; }
        internal InquiryType? BalanceInquiryType { get; set; }
        internal Address BillingAddress { get; set; }
        internal IEnumerable<Bill> Bills { get; set; }
        internal Customer Customer { get; set; }
        internal string CardBrandTransactionId { get; set; }
        internal decimal? CashBackAmount { get; set; }
        internal string ClientTransactionId { get; set; }
        internal CommercialData CommercialData { get; set; }
        internal bool CommercialRequest { get; set; }
        internal string Currency { get; set; }
        internal string CustomerId { get; set; }
        internal Customer CustomerData { get; set; }
        internal List<string[]> CustomData { get; set; }
        internal string CustomerIpAddress { get; set; }
        internal string Cvn { get; set; }
        internal string Description { get; set; }
        internal DecisionManager DecisionManager { get; set; }
        internal string DynamicDescriptor { get; set; }
        internal EcommerceInfo EcommerceInfo { get; set; }
        internal EmvFallbackCondition? EmvFallbackCondition { get; set; }
        internal EmvLastChipRead? EmvLastChipRead { get; set; }
        internal FraudFilterMode? FraudFilterMode { get; set; }
        internal FraudRuleCollection FraudRules { get; set; }
        internal decimal? Gratuity { get; set; }
        internal decimal? ConvenienceAmount { get; set; }
        internal decimal? ShippingAmt { get; set; }
        internal decimal? ShippingDiscount { get; set; }
        internal OrderDetails OrderDetails { get; set; }
        internal HostedPaymentData HostedPaymentData { get; set; }
        internal string IdempotencyKey { get; set; }
        internal string InvoiceNumber { get; set; }
        internal bool Level2Request { get; set; }
        internal LodgingData LodgingData { get; set; }
        internal string MessageAuthenticationCode { get; set; }
        internal List<Product> MiscProductData { get; set; }
        internal string OfflineAuthCode { get; set; }
        internal bool OneTimePayment { get; set; }
        internal string OrderId { get; set; }
        internal string PaymentApplicationVersion { get; set; }
        internal PaymentMethodUsageMode? PaymentMethodUsageMode { get; set; }
        public PhoneNumber HomePhone { get; set; }
        public PhoneNumber WorkPhone { get; set; }
        public PhoneNumber ShippingPhone { get; set; }
        public RemittanceReferenceType? RemittanceReferenceType { get; set; }
        public string RemittanceReferenceValue { get; set; }
        public PhoneNumber MobilePhone { get; set; }
        internal string PosSequenceNumber { get; set; }
        internal string ProductId { get; set; }
        internal RecurringSequence? RecurringSequence { get; set; }
        internal RecurringType? RecurringType { get; set; }
        internal bool RequestMultiUseToken { get; set; }
        internal GiftCard ReplacementCard { get; set; }
        internal ReversalReasonCode? ReversalReasonCode { get; set; }
        internal string ScheduleId { get; set; }
        internal bool? ShareTokenWithGroup { get; set; } = null;
        internal Address ShippingAddress { get; set; }
        internal StoredCredential StoredCredential { get; set; }
        internal Dictionary<string, List<string[]>> SupplementaryData { get; set; }
        internal decimal? SurchargeAmount { get; set; }
        internal string TagData { get; set; }
        internal string Timestamp { get; set; }
        internal decimal FeeAmount { get; set; }
        internal FeeType FeeType { get; set; }
        internal string ShiftNumber { get; set; }
        internal string ClerkId { get; set; }
        internal string TransportData { get; set; }
        internal CardHolderAuthenticationMethod? AuthenticationMethod { get; set; }
        internal RecordDataEntry POSSiteConfigRecord { get; set; }
        internal string CheckCustomerId { get; set; }
        internal string RawMICRData { get; set; }
        internal StoredCredentialInitiator? TransactionInitiator { get; set; }
        internal BNPLShippingMethod BNPLShippingMethod {get;set;}
        internal bool MaskedDataResponse { get; set; }
        internal BlockedCardType CardTypesBlocking { get; set; }
        internal MerchantCategory? MerchantCategory { get; set; }
        internal bool HasEmvFallbackData {
            get {
                return (EmvFallbackCondition != null || EmvLastChipRead != null || !string.IsNullOrEmpty(PaymentApplicationVersion));
            }
        }
        internal EmvLastChipRead EmvChipCondition { get; set; }
        internal DateTime LastRegisteredDate { get; set; }
        internal DateTime ShippingDate { get; set; }
        internal TransactionData TransactionData { get; set; }

        public AuthorizationBuilder WithTransactionData(TransactionData value) {
            TransactionData = value;
            return this;
        }

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

        public AuthorizationBuilder WithAmountEstimated(bool value) {
            AmountEstimated = value;
            return this;
        }

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
        /// </summary>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder WithMultiCapture(bool value = true) {
            MultiCapture = value;
            return this;
        }

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

        public AuthorizationBuilder WithCardBrandStorage(StoredCredentialInitiator transactionInitiator, string value = null) {
            TransactionInitiator = transactionInitiator;
            CardBrandTransactionId = value;
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

        public AuthorizationBuilder WithFraudFilter(FraudFilterMode fraudFilter, FraudRuleCollection fraudRules = null) {
            FraudFilterMode = fraudFilter;
            if(fraudRules != null)
                FraudRules = fraudRules;
            return this;
        }

        public AuthorizationBuilder WithCustomData(params string[] values) {
            if (CustomData == null) {
                CustomData = new List<string[]>();
            }
            CustomData.Add(values);

            return this;
        }

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

        public AuthorizationBuilder WithPOSSiteConfigRecord(RecordDataEntry value) {
            POSSiteConfigRecord = value;
            return this;
        }

        public AuthorizationBuilder WithDccRateData(DccRateData value) {
            DccRateData = value;
            return this;
        }

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
        /// 
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

        public AuthorizationBuilder WithCommercialRequest(bool value) {
            CommercialRequest = value;
            return this;
        }

        public AuthorizationBuilder WithCommercialData(CommercialData value) {
            CommercialData = value;
            return this;
        }

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

        public AuthorizationBuilder WithPaymentApplicationVersion(string value) {
            PaymentApplicationVersion = value;
            return this;
        }

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

        internal AuthorizationBuilder WithReplacementCard(GiftCard value) {
            ReplacementCard = value;
            return this;
        }

        public AuthorizationBuilder WithReversalReasonCode(ReversalReasonCode value) {
            ReversalReasonCode = value;
            return this;
        }

        public AuthorizationBuilder WithPayByLinkData(PayByLinkData payByLinkData) {
            PayByLinkData = payByLinkData;
            return this;
        }

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

        public AuthorizationBuilder WithStoredCredential(StoredCredential value) {
            StoredCredential = value;
            return this;
        }

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
        public AuthorizationBuilder WithSurchargeAmount(decimal? value) {
            SurchargeAmount = value;
            return this;
        }

        public AuthorizationBuilder WithMaskedDataResponse(bool value) {
            MaskedDataResponse = value;
            return this;
        }

        public AuthorizationBuilder WithBlockedCardType(BlockedCardType cardTypesBlocking) {
            var hasNulls = cardTypesBlocking.GetType().GetProperties().All(p => p.GetValue(cardTypesBlocking) == null);
            if (hasNulls) {            
                throw new BuilderException("No properties set on the object");
            }
            CardTypesBlocking = cardTypesBlocking;

            return this;
        }

        public AuthorizationBuilder WithMerchantCategory(MerchantCategory value) {
            MerchantCategory = value;
            return this;
        }

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
        /// 
        /// </summary>
        /// <param name="phoneCountryCode"></param>
        /// <param name="number"></param>
        /// <param name="type"></param>
        /// <returns></returns>
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

        public AuthorizationBuilder WithForceGatewayTimeout(bool value) {
            ForceGatewayTimeout = value;
            return this;
        }

        public AuthorizationBuilder WithFee(FeeType feeType, decimal feeAmount) {
            FeeType = feeType;
            FeeAmount = feeAmount;

            return this;
        }

        public AuthorizationBuilder WithUniqueDeviceId(string value) {
            UniqueDeviceId = value;
            return this;
        }

        public AuthorizationBuilder WithClerkId(string value) {
            ClerkId = value;
            return this;
        }

        public AuthorizationBuilder WithShiftNumber(string value) {
            ShiftNumber = value;
            return this;
        }

        public AuthorizationBuilder WithTransportData(string value) {
            TransportData = value;
            return this;
        }

        public AuthorizationBuilder WithBatchNumber(int value) {
            BatchNumber = value;
            return this;
        }

        public AuthorizationBuilder WithBatchNumber(int batchNumber, int sequenceNumber) {
            BatchNumber = batchNumber;
            SequenceNumber = sequenceNumber;
            return this;
        }

        public AuthorizationBuilder WithCompanyId(string value) {
            CompanyId = value;
            return this;
        }

        public AuthorizationBuilder WithFleetData(FleetData value) {
            FleetData = value;
            return this;
        }

        public AuthorizationBuilder WithIssuerData(Dictionary<DE62_CardIssuerEntryTag, string> value) {
            IssuerData = value;
            return this;
        }

        public AuthorizationBuilder WithSystemTraceAuditNumber(int value) {
            SystemTraceAuditNumber = value;
            return this;
        }        

        public AuthorizationBuilder WithTransactionMatchingData(TransactionMatchingData value) {
            TransactionMatchingData = value;
            return this;
        }

        public AuthorizationBuilder WithChipCondition(EmvLastChipRead value) {
            EmvChipCondition = value;
            return this;
        }

        public AuthorizationBuilder WithProductData(ProductData value) {
            ProductData = value;
            return this;
        }

        public AuthorizationBuilder WithEWICData(EWICData eWicData) {
            EwicData = eWicData;
            return this;
        }

        public AuthorizationBuilder WithAuthenticationMethod(CardHolderAuthenticationMethod value) {
            AuthenticationMethod = value;
            return this;
        }

        public AuthorizationBuilder WithEWICIssuingEntity(string value) {
            EWICIssuingEntity = value;
            return this;
        }

        public AuthorizationBuilder WithCheckCustomerId(string value) {
            CheckCustomerId = value;
            return this;
        }

        public AuthorizationBuilder WithRawMICRData(string value) {
            RawMICRData = value;
            return this;
        }

        
        public AuthorizationBuilder WithRemittanceReference(RemittanceReferenceType remittanceReferenceType, string remittanceReferenceValue) {
            RemittanceReferenceType = remittanceReferenceType;
            RemittanceReferenceValue = remittanceReferenceValue;
            return this;
        }

        public AuthorizationBuilder WithBNPLShippingMethod(BNPLShippingMethod value) {
            if (!(PaymentMethod is BNPL)) {
                throw new ArgumentException("The selected payment method doesn't support this property!");
            }

            BNPLShippingMethod = value;
            return this;
        }
    }
}