using System;
using System.Collections.Generic;
using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities.Billing;
using GlobalPayments.Api.Entities.PayFac;
using GlobalPayments.Api.Gateways.Events;
using GlobalPayments.Api.Network.Entities;
using GlobalPayments.Api.PaymentMethods;

namespace GlobalPayments.Api.Entities {
    /// <summary>
    /// Transaction response.
    /// </summary>
    public class Transaction {
        /// <summary>
        /// The Address used in the transaction.
        /// </summary>
        public Address Address { get; set; }

        /// <summary>
        /// The authorized amount.
        /// </summary>
        public decimal? AuthorizedAmount { get; set; }
        public string AutoSettleFlag { get; set; }

        /// <summary>
        /// The authorization code provided by the issuer.
        /// </summary>
        public string AuthorizationCode {
            get {
                if (TransactionReference != null) {
                    return TransactionReference.AuthCode;
                }
                return null;
            }
            set {
                if (TransactionReference == null) {
                    TransactionReference = new TransactionReference();
                }
                TransactionReference.AuthCode = value;
            }
        }

        /// <summary>
        /// The available balance of the payment method.
        /// </summary>
        public decimal? AvailableBalance { get; set; }

        /// <summary>
        /// The address verification service (AVS) response code.
        /// </summary>
        public string AvsResponseCode { get; set; }

        /// <summary>
        /// The address verification service (AVS) response message.
        /// </summary>
        public string AvsResponseMessage { get; set; }

        /// <summary>
        /// The address verification service (AVS) address response.
        /// </summary>
        public string AvsAddressResponse { get; set; }

        /// <summary>
        /// The balance on the account after the transaction.
        /// </summary>
        public decimal? BalanceAmount { get; set; }

        /// <summary>
        /// Summary of the batch.
        /// </summary>
        public BatchSummary BatchSummary { get; set; }

        public string CardBrandTransactionId { get; set; }

        public AlternativePaymentResponse AlternativePaymentResponse {
            get
            {
                return TransactionReference?.AlternativePaymentResponse;
            }
            set
            {
                if(TransactionReference == null)
                {
                    TransactionReference = new TransactionReference();
                }
                TransactionReference.AlternativePaymentResponse = value;
            }
        }

        /// <summary>
        /// The type of card used in the transaction.
        /// </summary>
        public string CardType { get; set; }

        /// <summary>
        /// The last four digits of the card number used in
        /// the transaction.
        /// </summary>
        public string CardLast4 { get; set; }

        public string FingerPrint { get; set; }

        public string FingerPrintIndicator { get; set; }

        /// <summary>
        /// The card number used in the transaction.
        /// </summary>
        public string CardNumber { get; set; }

        /// <summary>
        /// The card expiry month used in the transaction.
        /// </summary>
        public int? CardExpMonth { get; set; }

        /// <summary>
        /// The card expiry year used in the transaction.
        /// </summary>
        public int? CardExpYear { get; set; }

        /// <summary>
        /// The cardholder name used in the transaction.
        /// </summary>
        public string CardholderName { get; set; }

        /// <summary>
        /// The consumer authentication (3DSecure) verification
        /// value response code.
        /// </summary>
        public string CavvResponseCode { get; set; }

        /// <summary>
        /// The client transaction ID supplied in the request.
        /// </summary>
        public string ClientTransactionId {
            get {
                if (TransactionReference != null) {
                    return TransactionReference.ClientTransactionId;
                }
                return null;
            }
            set {
                if (TransactionReference == null) {
                    TransactionReference = new TransactionReference();
                }
                TransactionReference.ClientTransactionId = value;
            }
        }

        /// <summary>
        /// The fee amount to charge
        /// </summary>
        public decimal ConvenienceFee { get; set; }

        /// <summary>
        /// The CustomerData used in the transaction.
        /// </summary>
        public Customer CustomerData { get; set; }

        public NtsData NTSData {
            get {
                if (TransactionReference != null) {
                    return TransactionReference.NtsData;
                }
                return null;
            }
            set {
                if (TransactionReference == null) {
                    TransactionReference = new TransactionReference();
                }
                TransactionReference.NtsData = value;
            }
        }
        public string MessageTypeIndicator {
            get {
                if (TransactionReference != null) {
                    return TransactionReference.MessageTypeIndicator;
                }
                return null;
            }
            set {
                if (TransactionReference == null) {
                    TransactionReference = new TransactionReference();
                }
                TransactionReference.MessageTypeIndicator = value;
            }
        }
        public string SystemTraceAuditNumber {
            get {
                if (TransactionReference != null) {
                    return TransactionReference.SystemTraceAuditNumber;
                }
                return null;
            }
            set {
                if (TransactionReference == null) {
                    TransactionReference = new TransactionReference();
                }
                TransactionReference.SystemTraceAuditNumber = value;
            }
        }
        public string OriginalTransactionTime {
            get {
                if (TransactionReference != null) {
                    return TransactionReference.OriginalTransactionTime;
                }
                return null;
            }
            set {
                if (TransactionReference == null) {
                    TransactionReference = new TransactionReference();
                }
                TransactionReference.OriginalTransactionTime = value;

            }
        }
        /// <summary>
        /// The commercial indicator for Level II/III.
        /// </summary>
        public string CommercialIndicator { get; set; }

        /// <summary>
        /// The card verification number (CVN) response code.
        /// </summary>
        public string CvnResponseCode { get; set; }

        /// <summary>
        /// The card verification number (CVN) response message.
        /// </summary>
        public string CvnResponseMessage { get; set; }

        public DccRateData DccRateData { get; set; }

        public DebitMac DebitMac { get; set; }

        /// <summary>
        /// The EMV response from the issuer.
        /// </summary>
        public string EmvIssuerResponse { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public FraudResponse FraudResponse;
        /// <summary>
        /// The host response date
        /// </summary>
        public DateTime? HostResponseDate { get; set; }

        /// <summary>
        /// The Auto settle Flag which comes in response
        /// </summary>
        public bool MultiCapture { get { return (MultiCapturePaymentCount != null && MultiCapturePaymentCount != null); } }

        public int? MultiCapturePaymentCount { get; set; }

        public int? MultiCaptureSequence { get; set; }

        /// <summary>
        /// The order ID supplied in the request.
        /// </summary>
        public string OrderId {
            get {
                if (TransactionReference != null) {
                    return TransactionReference.OrderId;
                }
                return null;
            }
            set {
                if (TransactionReference == null) {
                    TransactionReference = new TransactionReference();
                }
                TransactionReference.OrderId = value;
            }
        }

        /// <summary>
        /// The type of payment made in the request.
        /// </summary>
        public PaymentMethodType PaymentMethodType {
            get {
                if (TransactionReference != null) {
                    return TransactionReference.PaymentMethodType;
                }
                return PaymentMethodType.Credit;
            }
            set {
                if (TransactionReference == null) {
                    TransactionReference = new TransactionReference();
                }
                TransactionReference.PaymentMethodType = value;
            }
        }

        /// <summary>
        /// The remaining points on the account after the transaction.
        /// </summary>
        public decimal? PointsBalanceAmount { get; set; }

        /// <summary>
        /// The recurring profile data code.
        /// </summary>
        public string RecurringDataCode { get; set; }

        /// <summary>
        /// The reference number provided by the issuer.
        /// </summary>
        public string ReferenceNumber { get; set; }

        /// <summary>
        /// The original response code from the issuer/gateway.
        /// </summary>
        public string ResponseCode { get; set; }

        /// <summary>
        /// The original response message from the issuer/gateway.
        /// </summary>
        public string ResponseMessage { get; set; }

        /// <summary>
        /// A catch all for additional fields not mapped to a specific transaction properties.
        /// </summary>
        public Dictionary<string, string> ResponseValues { get; set; }

        public string SchemeId { get; set; }

        internal ThreeDSecure ThreeDSecure { get; set; }

        /// <summary>
        /// The response from ProPay
        /// </summary>
        public PayFacResponseData PayFacData;

        /// <summary>
        /// The timestamp of the transaction.
        /// </summary>
        public string Timestamp { get; set; }

        /// <summary>
        /// The transaction descriptor.
        /// </summary>
        public string TransactionDescriptor { get; set; }

        /// <summary>
        /// The result of an administrative Time Request
        /// </summary>
        public string TimeResponseFromHeartland { get; set; }

        /// <summary>
        /// The gateway transaction ID of the transaction.
        /// </summary>
        public string TransactionId {
            get {
                if (TransactionReference != null) {
                    return TransactionReference.TransactionId;
                }
                return null;
            }
            set {
                if (TransactionReference == null) {
                    TransactionReference = new TransactionReference();
                }
                TransactionReference.TransactionId = value;
            }
        }
        public string ProcessingCode {
            get {
                if (TransactionReference != null) {
                    return TransactionReference.OriginalProcessingCode;
                }
                return null;
            }
            set {
                if (TransactionReference == null) {
                    TransactionReference = new TransactionReference();
                }
                TransactionReference.OriginalProcessingCode = value;
            }
        }

        /// <summary>
        /// The payment token returned in the transaction.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// The token data returned for the specific token
        /// </summary>
        public TokenData TokenData { get; set; }
        

        internal GiftCard GiftCard { get; set; }

        internal TransactionReference TransactionReference { get; set; }
        public PriorMessageInformation MessageInformation { get; set; }
        public string TransactionToken { get; set; }
        public List<IGatewayEvent> GatewayEvents { get; set; }

        /// <summary>
        /// Creates an additional authorization against the original transaction.
        /// </summary>
        /// <param name="amount">The additional amount to authorize</param>
        public ManagementBuilder AdditionalAuth(decimal? amount = null) {
            return new ManagementBuilder(TransactionType.Auth)
                .WithPaymentMethod(TransactionReference)
                .WithAmount(amount);
        }

        public ManagementBuilder Cancel(decimal? amount = null) {
            return new ManagementBuilder(TransactionType.Void)
                    .WithPaymentMethod(TransactionReference)
                    .WithCustomerInitiated(true)
                    .WithAmount(amount);
        }

        /// <summary>
        /// Captures the original transaction.
        /// </summary>
        /// <param name="amount">The amount to capture</param>
        public ManagementBuilder Capture(decimal? amount = null) {
            var builder = new ManagementBuilder(TransactionType.Capture)
                .WithPaymentMethod(TransactionReference)
                .WithAmount(amount);

            if (MultiCapture) {
                builder.WithMultiCapture(MultiCaptureSequence.Value, MultiCapturePaymentCount.Value);
            }
            return builder;
        }

        /// <summary>
        /// Edits the original transaction.
        /// </summary>
        public ManagementBuilder Edit() {
            return new ManagementBuilder(TransactionType.Edit).WithPaymentMethod(TransactionReference);
        }

        /// <summary>
        /// Places the original transaction on hold.
        /// </summary>
        public ManagementBuilder Hold() {
            return new ManagementBuilder(TransactionType.Hold).WithPaymentMethod(TransactionReference);
        }

        public ManagementBuilder PreAuthCompletion(decimal? amount = null) {
            return new ManagementBuilder(TransactionType.PreAuthCompletion)
                    .WithPaymentMethod(TransactionReference)
                    .WithAmount(amount);
        }

        /// <summary>
        /// Refunds/returns the original transaction.
        /// </summary>
        /// <param name="amount">The amount to refund/return</param>
        public ManagementBuilder Refund(decimal? amount = null) {
            return new ManagementBuilder(TransactionType.Refund)
                .WithPaymentMethod(TransactionReference)
                .WithAmount(amount);
        }

        /// <summary>
        /// Releases the original transaction from a hold.
        /// </summary>
        public ManagementBuilder Release() {
            return new ManagementBuilder(TransactionType.Release).WithPaymentMethod(TransactionReference);
        }

        /// <summary>
        /// Reverses the original transaction.
        /// </summary>
        /// <param name="amount">The original authorization amount</param>
        public ManagementBuilder Reverse(decimal? amount = null) {
            return new ManagementBuilder(TransactionType.Reversal)
                .WithPaymentMethod(TransactionReference)
                .WithAmount(amount);
        }

        /// <summary>
        /// Reauthorize the original transaction.
        /// </summary>
        /// <param name="amount">The original authorization amount</param>
        public ManagementBuilder Reauthorize(decimal? amount = null) {
            return new ManagementBuilder(TransactionType.Reauth)
                .WithPaymentMethod(TransactionReference)
                .WithAmount(amount ?? BalanceAmount);
        }

        /// <summary>
        /// Voids the original transaction.
        /// </summary>
        public ManagementBuilder Void(VoidReason? reason = null, decimal? amount = null, bool force = false) {
            return new ManagementBuilder(TransactionType.Void)
                .WithPaymentMethod(TransactionReference)
                .WithVoidReason(reason)
                .WithAmount(amount)
                .WithForcedReversal(force);
        }

        /// <summary>
        /// Confirm an original transaction. For now it is used for the APM transactions with PayPal
        /// </summary>
        public ManagementBuilder Confirm(decimal? amount = null)
        {
            return new ManagementBuilder(TransactionType.Confirm)
                .WithPaymentMethod(this.TransactionReference)
                .WithAmount(amount);
        }

        public ManagementBuilder Increment(decimal? amount = null) {
            return new ManagementBuilder(TransactionType.Increment).WithAmount(amount).WithPaymentMethod(TransactionReference);
        }

        /// <summary>
        /// Creates a `Transaction` object from a stored transaction ID.
        /// </summary>
        /// <remarks>
        /// Used to expose management requests on the original transaction
        /// at a later date/time.
        /// </remarks>
        /// <param name="transactionId">The original transaction ID</param>
        /// <param name="paymentMethodType">
        /// The original payment method type. Defaults to `PaymentMethodType.Credit`.
        /// </param>
        public static Transaction FromId(string transactionId, PaymentMethodType paymentMethodType = PaymentMethodType.Credit)
        {
            return new Transaction
            {
                TransactionReference = new TransactionReference
                {
                    TransactionId = transactionId,
                    PaymentMethodType = paymentMethodType
                }
            };
        }

        /// <summary>
        /// Creates a `Transaction` object from a stored transaction ID.
        /// </summary>
        /// <remarks>
        /// Used to expose management requests on the original transaction
        /// at a later date/time.
        /// </remarks>
        /// <param name="transactionId">The original transaction ID</param>
        /// <param name="transactionId">The original transaction's order ID</param>
        /// <param name="paymentMethodType">
        /// The original payment method type. Defaults to `PaymentMethodType.Credit`.
        /// </param>
        public static Transaction FromId(string transactionId, string orderId, PaymentMethodType paymentMethodType = PaymentMethodType.Credit)
        {
            return new Transaction
            {
                TransactionReference = new TransactionReference
                {
                    TransactionId = transactionId,
                    PaymentMethodType = paymentMethodType,
                    OrderId = orderId
                }
            };
        }

        public static Transaction FromNetwork(decimal? amount, string authCode, NtsData originalNtsCode, IPaymentMethod originalPaymentMethod, string messageTypeIndicator = null, string stan = null, string originalTransactionTime = null, string originalProcessingCode = null, string acquirerId = null)
        {
            TransactionReference reference = new TransactionReference();
            reference.OriginalAmount = amount;
            reference.AcquiringInstitutionId = acquirerId;
            reference.AuthCode = authCode;
            reference.MessageTypeIndicator = messageTypeIndicator;
            reference.NtsData = originalNtsCode;
            reference.OriginalPaymentMethod = originalPaymentMethod;
            reference.OriginalTransactionTime = originalTransactionTime;
            reference.SystemTraceAuditNumber = stan;
            reference.OriginalProcessingCode = originalProcessingCode;

            Transaction trans = new Transaction();
            trans.TransactionReference = reference;

            return trans;
        }
        public List<CheckResponseErrorMessage> CheckResponseErrorMessages { get; set; }
    }
}