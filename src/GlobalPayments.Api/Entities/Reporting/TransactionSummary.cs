using GlobalPayments.Api.Entities.Billing;
using System;
using System.Collections.Generic;

namespace GlobalPayments.Api.Entities {
    /// <summary>
    /// Transaction-level report data
    /// </summary>
    public class TransactionSummary {
        public string AccountDataSource { get; set; }

        public decimal? AdjustmentAmount { get; set; }

        public string AdjustmentCurrency { get; set; }
        public string AdjustmentReason { get; set; }

        public AltPaymentData AltPaymentData { get; set; }

        /// <summary>
        /// The originally requested authorization amount.
        /// </summary>
        public decimal? Amount { get; set; }

        //public unknown AttachmentInfo { get; set; }

        public string Application { get; set; }

        public string AquirerReferenceNumber { get; set; }

        public List<AuthorizationRecord> AuthorizationRecords { get; set; }

        /// <summary>
        /// The authorized amount.
        /// </summary>
        public decimal? AuthorizedAmount { get; set; }

        /// <summary>
        /// The authorization code provided by the issuer.
        /// </summary>

        public string AuthCode { get; set; }

        public string AvsResponseCode { get; set; }

        public DateTime? BatchCloseDate { get; set; }

        public string BatchId { get; set; }

        public string BatchSequenceNumber { get; set; }

        public Address BillingAddress { get; set; }
        public List<Bill> BillTransactions { get; set; }

        public string BrandReference { get; set; }

        public decimal? CaptureAmount { get; set; }

        public string CardHolderFirstName { get; set; }

        public string CardHolderLastName { get; set; }

        public string CardHolderName { get; set; }

        public string CardSwiped { get; set; }

        public string CardType { get; set; }

        public string CavvResponseCode { get; set; }

        public string Channel { get; set; }

        public CheckData CheckData { get; set; }

        public string ClerkId { get; set; }

        /// <summary>
        /// The client transaction ID sent in the authorization request.
        /// </summary>
        public string ClientTransactionId { get; set; }

        public string CompanyName { get; set; }

        /// <summary>
        /// The originally requested convenience amount.
        /// </summary>
        public decimal? ConvenienceAmount { get; set; }

        public string Currency { get; set; }

        public string CustomerFirstName { get; set; }

        public string CustomerId { get; set; }

        public string CustomerLastName { get; set; }

        public string CvnResponseCode { get; set; }

        public bool DebtRepaymentIndicator { get; set; }

        public decimal? DepositAmount { get; set; }

        public string DepositCurrency { get; set; }

        public DateTime? DepositDate { get; set; }

        public string DepositReference { get; set; }

        public string DepositType { get; set; }

        public string Description { get; set; }

        /// <summary>
        /// The device ID where the transaction was ran; where applicable.
        /// </summary>
        public int DeviceId { get; set; }

        public string EciIndicator { get; set; }

        public string EmvChipCondition { get; set; }

        public string EntryMode { get; set; }
        public decimal FeeAmount { get; set; }

        public string FraudRuleInfo { get; set; }

        public bool FullyCaptured { get; set; }

        public decimal? CashBackAmount { get; set; }
        public decimal? GratuityAmount { get; set; }

        public bool HasEcomPaymentData { get; set; }

        public bool HasEmvTags { get; set; }

        public string InvoiceNumber { get; set; }

        /// <summary>
        /// The original response code from the issuer.
        /// </summary>
        public string IssuerResponseCode { get; set; }

        /// <summary>
        /// The original response message from the issuer.
        /// </summary>
        public string IssuerResponseMessage { get; set; }

        public string IssuerTransactionId { get; set; }

        /// <summary>
        /// The original response code from the gateway.
        /// </summary>
        public string GatewayResponseCode { get; set; }

        /// <summary>
        /// The original response message from the gateway.
        /// </summary>
        public string GatewayResponseMessage { get; set; }

        public string GiftCurrency { get; set; }

        public LodgingData LodgingData { get; set; }

        public string MaskedAlias { get; set; }

        /// <summary>
        /// The authorized card number, masked.
        /// </summary>
        public string MaskedCardNumber { get; set; }

        public string MerchantCategory { get; set; }

        public string MerchantDbaName { get; set; }

        public string MerchantHierarchy { get; set; }

        public string MerchantId { get; set; }

        public string MerchantName { get; set; }

        public string MerchantNumber { get; set; }
        public string MerchantInvoiceNumber { get; set; }
        public string MerchantPONumber { get; set; }
        public string MerchantTransactionDescription { get; set; }
        public string MerchantTransactionID { get; set; }
        public decimal NetAmount { get; set; }
        public decimal NetFeeAmount { get; set; }

        public bool OneTimePayment { get; set; }

        public string OrderId { get; set; }

        /// <summary>
        /// The gateway transaction ID of the authorization request.
        /// </summary>
        public string OriginalTransactionId { get; set; }

        public string PaymentMethodKey { get; set; }

        public string PaymentType { get; set; }
        public Customer PayorData { get; set; }

        public string PoNumber { get; set; }

        public string RecurringDataCode { get; set; }

        /// <summary>
        /// The reference number provided by the issuer.
        /// </summary>
        public string ReferenceNumber { get; set; }

        public int? RepeatCount { get; set; }

        public DateTime? ResponseDate { get; set; }

        public string ScheduleId { get; set; }

        public string SchemeReferenceData { get; set; }

        /// <summary>
        /// The transaction type.
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// The settled from the authorization.
        /// </summary>
        public decimal? SettlementAmount { get; set; }

        /// <summary>
        /// The originally requested shipping amount.
        /// </summary>
        public decimal? ShippingAmount { get; set; }

        public string SiteTrace { get; set; }

        /// <summary>
        /// The transaction status.
        /// </summary>
        public string Status { get; set; }

        public decimal? SurchargeAmount { get; set; }

        public decimal? TaxAmount { get; set; }

        public string TaxType { get; set; }

        public string TerminalId { get; set; }

        public string TerminalRefNumber { get; set; }

        public string TokenPanLastFour { get; set; }

        /// <summary>
        /// The date/time of the original transaction.
        /// </summary>
        public DateTime? TransactionDate { get; set; }

        public DateTime? TransactionLocalDate { get; set; }

        public string TransactionDescriptor { get; set; }

        public string TransactionStatus { get; set; }

        /// <summary>
        /// The gateway transaction ID of the transaction.
        /// </summary>
        public string TransactionId { get; set; }

        public string UniqueDeviceId { get; set; }

        public string Username { get; set; }

        public string TransactionType { get; set; }

        public string CardEntryMethod { get; set; }

        public decimal? AmountDue { get; set; }

        public bool HostTimeout { get; set; }

        public string Country { get; set; }

        public string Xid { get; set; }

        public string AccountNumberLast4 { get; set; }

        public string AccountType { get; set; }

        public AlternativePaymentResponse AlternativePaymentResponse { get; set; }
    }
}
