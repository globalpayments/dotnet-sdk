﻿using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Billing;
using GlobalPayments.Api.Entities.Enums;
using GlobalPayments.Api.Entities.PayFac;
using GlobalPayments.Api.Network.Entities;
using GlobalPayments.Api.PaymentMethods;
using System.Collections.Generic;

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
        internal string BatchReference { get; set; }
        internal IEnumerable<Bill> Bills { get; set; }
        internal string CardType {
            get
            {
                if (PaymentMethod is TransactionReference)
                {
                    return ((TransactionReference)PaymentMethod).CardType;
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
        internal CommercialData CommercialData { get; set; }
        internal decimal? ConvenienceAmount { get; set; }
        internal string Currency { get; set; }
        internal string CustomerId { get; set; }
        internal string CustomerIpAddress { get; set; }
        internal IEnumerable<DisputeDocument> DisputeDocuments { get; set; }
        internal string DisputeId { get; set; }
        internal string DynamicDescriptor { get; set; }
        internal eCheck BankTransferDetails { get; set; }
        internal DccRateData DccRateData { get; set; }
        internal EcommerceInfo EcommerceInfo { get; set; }
        internal decimal? Gratuity { get; set; }
        internal string IdempotencyKey { get; set; }
        internal string InvoiceNumber { get; set; }
        internal LodgingData LodgingData { get; set; }
        internal int? MultiCapturePaymentCount { get; set; }
        internal int? MultiCaptureSequence { get; set; }
        internal string EntryClass { get; set; }
        internal string PaymentPurposeCode { get; set; }
        internal string SoftDescriptor { get; set; }
        internal string OrderId {
            get {
                if (PaymentMethod is TransactionReference) {
                    return ((TransactionReference)PaymentMethod).OrderId;
                }
                return null;
            }
        }
        internal string PayerAuthenticationResponse { get; set; }
        internal ReasonCode? ReasonCode { get; set; }
        internal Dictionary<string, List<string[]>> SupplementaryData { get; set; }
        internal decimal? SurchargeAmount { get; set; }
        internal string TransactionId {
            get {
                if (PaymentMethod is TransactionReference) {
                    return ((TransactionReference)PaymentMethod).TransactionId;
                }
                return null;
            }
        }        
        internal string ClerkId { get; set; }
        internal int TransactionCount { get; set; }
        internal decimal TotalCredits { get; set; }
        internal decimal TotalDebits { get; set; }
        internal string ReferenceNumber { get; set; }
        internal BatchCloseType BatchCloseType { get; set; }
        internal decimal? CashBackAmount { get; set; }
        internal bool ForcedReversal { get; set; }
        internal bool CustomerInitiated { get; set; }
        internal string TransportData { get; set; }
        internal string Timestamp { get; set; }
        internal VoidReason? VoidReason { get; set; }
        internal bool AllowDuplicates { get; set; }
        internal CardHolderAuthenticationMethod? AuthenticationMethod { get; set; }
        internal bool GenerateReceipt { get; set; }
        internal string TagData { get; set; }
        internal PaymentMethodUsageMode? PaymentMethodUsageMode { get; set; }
        internal string Reference { get; set; }
        internal FundsData FundsData { get; set; }

        //internal string EWICIssuingEntity { get; set; }
        //internal CustomerData AuthorizationCustomerData { get; set; }

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

        public ManagementBuilder WithClerkId(string value) {
            ClerkId = value;
            return this;
        }
        /// <summary>
        /// Sets the current transaction's entry class where applicable.
        /// </summary>
        /// <param name="value">entry class</param>
        /// <returns>ManagementBuilder</returns>
        public ManagementBuilder WithEntryClass(string value) {
            EntryClass = value;
            return this;
        }

        /// <summary>
        /// Sets the current transaction's payment purpose code where applicable.
        /// </summary>
        /// <param name="value">payment purpose code</param>
        /// <returns>ManagementBuilder</returns>
        public ManagementBuilder WithPaymentPurposeCode(string value) {
            PaymentPurposeCode = value;
            return this;
        }

        /// <summary>
        /// Sets the current batch reference
        /// </summary>
        /// <param name="value">The batch reference</param>
        /// <returns></returns>
        public ManagementBuilder WithBatchReference(string value) {
            BatchReference = value;
            return this;
        }

        /// <summary>
        /// Adds the bills to the transaction, where applicable
        /// </summary>
        /// <param name="values">The transaction's bills</param>
        /// <returns>AuthorizationBuilder</returns>
        public ManagementBuilder WithBills(params Bill[] values) {
            Bills = values;
            return this;
        }

        /// <summary>
        /// Sets the Multicapture value as true/false.
        /// </summary>
        /// <returns>ManagementBuilder</returns>
        //public ManagementBuilder WithMultiCapture(bool value) {
        //    MultiCapture = value;
        //    return this;
        //}

        public ManagementBuilder WithMultiCapture(int sequence = 1, int paymentCount = 1) {
            MultiCapture = true;
            MultiCaptureSequence = sequence;
            MultiCapturePaymentCount = paymentCount;

            return this;
        }

        public ManagementBuilder WithCommercialData(CommercialData data) {
            CommercialData = data;
            if (data.CommercialIndicator.Equals(CommercialIndicator.Level_II)) {
                TransactionModifier = TransactionModifier.Level_II;
            }
            else { TransactionModifier = TransactionModifier.Level_III; }
            return this;
        }

        /// <summary>
        /// Sets the Convenience amount; where applicable.
        /// </summary>
        /// <param name="value">The Convenience amount</param>
        /// <returns>AuthorizationBuilder</returns>
        public ManagementBuilder WithConvenienceAmount(decimal? value) {
            ConvenienceAmount = value;
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
        /// Sets the customer ID; where applicable.
        /// </summary>
        /// <remarks>
        /// This is an application/merchant generated value.
        /// </remarks>
        /// <param name="value">The customer ID</param>
        /// <returns>ManagementBuilder</returns>
        public ManagementBuilder WithCustomerId(string value) {
            CustomerId = value;
            return this;
        }

        ///// <summary>
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

        public ManagementBuilder WithPayByLinkData(PayByLinkData payByLinkData) {
            PayByLinkData = payByLinkData;            
            return this;
        }

        /// <summary>
        /// Sets the dispute documents
        /// </summary>
        /// <param name="value">The dispute documents</param>
        /// <returns>ManagementBuilder</returns>
        public ManagementBuilder WithDisputeDocuments(IEnumerable<DisputeDocument> value) {
            DisputeDocuments = value;
            return this;
        }

        /// <summary>
        /// Sets the dispute id
        /// </summary>
        /// <param name="value">The dispute id</param>
        /// <returns>ManagementBuilder</returns>
        public ManagementBuilder WithDisputeId(string value) {
            DisputeId = value;
            return this;
        }

        /// <summary>
        /// Set the election check information
        /// </summary>
        /// <remarks>
        /// This value is sent during the authorization process and is displayed
        /// in the consumer's account.
        /// </remarks>
        /// <param name="value">eCheck</param>
        /// <returns>AuthorizationBuilder</returns>
        public ManagementBuilder WithBankTransferDetails(eCheck value) {
            BankTransferDetails = value;
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
        public ManagementBuilder WithDynamicDescriptor(string value) {
            DynamicDescriptor = value;
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
        /// Field submitted in the request that is used to ensure idempotency is maintained within the action
        /// </summary>
        /// <param name="value">The idempotency key</param>
        /// <returns>ManagementBuilder</returns>
        public ManagementBuilder WithIdempotencyKey(string value) {
            IdempotencyKey = value;
            return this;
        }

        /// <summary>
        /// Sets the invoice number; where applicable.
        /// </summary>
        /// <param name="value">The invoice number</param>
        /// <returns>ManagementnBuilder</returns>
        public ManagementBuilder WithInvoiceNumber(string value) {
            InvoiceNumber = value;
            return this;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="value">string</param>
        /// <returns></returns>
        public ManagementBuilder WithPayerAuthenticationResponse(string value) {
            PayerAuthenticationResponse = value;
            return this;
        }

        internal ManagementBuilder WithPaymentMethod(IPaymentMethod value) {
            PaymentMethod = value;
            return this;
        }

        public ManagementBuilder WithPaymentLinkId(string value) {
            PaymentLinkId = value;
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
        ///
        /// </summary>
        /// <param name="type"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public ManagementBuilder WithSupplementaryData(string type, params string[] values) {
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
        public ManagementBuilder WithSurchargeAmount(decimal value) {
            SurchargeAmount = value;
            return this;
        }

        internal ManagementBuilder WithModifier(TransactionModifier value) {
            TransactionModifier = value;
            return this;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public ManagementBuilder WithAlternativePaymentType(AlternativePaymentType value) {
            AlternativePaymentType = value;
            return this;
        }
        public ManagementBuilder WithCashBackAmount(decimal? value) {
            CashBackAmount = value;
            return this;
        }
        public ManagementBuilder WithBatchNumber(int batchNumber, int sequenceNumber = 0) {
            BatchNumber = batchNumber;
            SequenceNumber = sequenceNumber;
            return this;
        }
        public ManagementBuilder WithBatchCloseType(BatchCloseType value) {
            BatchCloseType = value;
            return this;
        }
        public ManagementBuilder WithBatchTotals(int transactionCount, decimal totalDebits, decimal totalCredits) {
            TransactionCount = transactionCount;
            TotalDebits = totalDebits;
            TotalCredits = totalCredits;

            return this;
        }
        public ManagementBuilder WithTransportData(string value) {
            TransportData = value;
            return this;
        }
        public ManagementBuilder WithTimestamp(string value) {
            Timestamp = value;
            return this;
        }
        public ManagementBuilder WithReferenceNumber(string value) {
            ReferenceNumber = value;
            return this;
        }

        /// <summary>
        /// Allows duplicate transactions by skipping the
        /// gateway's duplicate checking.
        /// </summary>
        /// <param name="value">The duplicate skip flag</param>
        /// <returns>ManagementBuilder</returns>
        public ManagementBuilder WithAllowDuplicates(bool value) {
            AllowDuplicates = value;
            return this;
        }

        /// <summary>
        /// Lodging data information for Portico implementation
        /// </summary>
        /// <param name="value">The lodging data</param>
        /// <returns>AuthorizationBuilder</returns>
        public ManagementBuilder WithLodgingData(LodgingData value) {
            LodgingData = value;
            return this;
        }

        public ManagementBuilder WithVoidReason(VoidReason? value) {
            VoidReason = value;
            return this;
        }
        
        public ManagementBuilder WithReference(string value) {
            Reference = value;
            return this;
        }

        public ManagementBuilder WithFundsData(FundsData value) {
            FundsData = value;
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
            if (client.SupportsOpenBanking &&
                PaymentMethod is TransactionReference &&
            PaymentMethod.PaymentMethodType == PaymentMethodType.BankPayment) {
            var obClient = ServicesContainer.Instance.GetOpenBanking(configName);
                if (obClient != client) {
                    return obClient.ManageOpenBanking(this);
                }
            }
            return client.ManageTransaction(this);
        }

        protected override void SetupValidations() {

            
            #region ENUM VALIDATION WITH FLAG ATTRIBUTE     
            /// TO ADD
            #endregion


            Validations.For(TransactionType.Capture)
                .Check(() => PaymentMethod).IsNotNull()
                .Check(() => TransactionId).IsNotNull()
                .Check(() => VoidReason).IsNull();

            Validations.For(TransactionType.Edit)
                .Check(() => PaymentMethod).IsNotNull()
                .Check(() => VoidReason).IsNull();

            Validations.For(TransactionType.Hold)
                .Check(() => PaymentMethod).IsNotNull()
                .Check(() => TransactionId).IsNotNull()
                .Check(() => VoidReason).IsNull();

            Validations.For(TransactionType.Release)
                .Check(() => PaymentMethod).IsNotNull()
                .Check(() => TransactionId).IsNotNull()
                .Check(() => VoidReason).IsNull();

            Validations.For(TransactionType.Reauth)
                .Check(() => TransactionId).IsNotNull();

            // TODO: Need level validations
            //Validations.For(TransactionType.Edit).With(TransactionModifier.Level_II)
            //    .Check(() => TaxType).IsNotNull();

            Validations.For(TransactionType.Refund)
                .When(() => Amount).IsNotNull()
                .Check(() => Currency).IsNotNull()
                .Check(() => VoidReason).IsNull();

            Validations.For(TransactionType.VerifySignature)
                .Check(() => PayerAuthenticationResponse).IsNotNull()
                .Check(() => Amount).IsNotNull()
                .Check(() => Currency).IsNotNull()
                .Check(() => OrderId).IsNotNull()
                .Check(() => VoidReason).IsNull();

            Validations.For(TransactionType.TokenUpdate)
                .Check(() => PaymentMethod).IsNotNull()
                .Check(() => PaymentMethod).Is<ITokenizable>();

            Validations.For(TransactionType.TokenUpdate)
                .Check(() => PaymentMethod).IsNotNull()
                .Check(() => PaymentMethod).Is<ITokenizable>();

            Validations.For(TransactionType.TokenUpdate)
                .Check(() => PaymentMethod).Is<CreditCardData>();

            Validations.For(TransactionType.PayByLinkUpdate)
               .Check(() => PayByLinkData).IsNotNull()
               .Check(() => Amount).IsNotNull()
               .Check(() => PayByLinkData).PropertyOf(nameof(PayByLinkData.UsageMode)).IsNotNull()
               .Check(() => PayByLinkData).PropertyOf(nameof(PayByLinkData.UsageLimit)).IsNotNull()
               .Check(() => PayByLinkData).PropertyOf(nameof(PayByLinkData.Type)).IsNotNull();
       
            Validations.For(TransactionType.SplitFunds)
                .Check(() => FundsData).IsNotNull()
                .Check(() => Amount).IsNotNull();

            Validations.For(TransactionType.TokenUpdate)
                .Check(() => VoidReason).IsNull();

            Validations.For(TransactionType.TokenDelete)
                .Check(() => VoidReason).IsNull();
        }
        public ManagementBuilder WithForcedReversal(bool value) {
            ForcedReversal = value;
            return this;
        }
        public ManagementBuilder WithProductData(ProductData value) {
            ProductData = value;
            return this;
        }
        public ManagementBuilder WithFleetData(FleetData value) {
            FleetData = value;
            return this;
        }
        public ManagementBuilder WithCustomerInitiated(bool value) {
            CustomerInitiated = value;
            return this;
        }
        public ManagementBuilder WithForceGatewayTimeout(bool value) {
            ForceGatewayTimeout = value;
            return this;
        }

        public ManagementBuilder WithAuthenticatioNMethod(CardHolderAuthenticationMethod value) {
            AuthenticationMethod = value;
            return this;
        }

        public ManagementBuilder WithTagData(string value) {
            TagData = value;
            return this;
        }

        public ManagementBuilder WithEWICIssuingEntity(string value) {
            EWICIssuingEntity = value;
            return this;
        }

        public ManagementBuilder WithDccRateData(DccRateData value){
            DccRateData = value;
            return this;
        }

        public ManagementBuilder WithPaymentMethodUsageMode(PaymentMethodUsageMode value) {
            PaymentMethodUsageMode = value;
            return this;
        }

        public ManagementBuilder WithEcommerceInfo(EcommerceInfo value) {
            EcommerceInfo = value;
            return this;
        }
        /// <summary>
        /// Generate receipt
        /// </summary>
        /// <param name="value">generate receipt</param>
        /// <returns>ManagementBuilder</returns>
        public ManagementBuilder WithGenerateReceipt(bool value) {
            GenerateReceipt = value;
            return this;
        }
        public ManagementBuilder WithSoftDescriptor(string value) {
            SoftDescriptor = value;
            return this;
        }
    }
}
