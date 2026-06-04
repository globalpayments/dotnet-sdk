using GlobalPayments.Api.Utils;
using System;

namespace GlobalPayments.Api.Entities {
    /// <summary>
    /// Alternative payment response data
    /// </summary>
    public class AlternativePaymentResponse {
        /// <summary>
        /// bank account details
        /// </summary>
        public string BankAccount { get; set; }

        /// <summary>
        /// Account holder name of the customer’s account
        /// </summary>
        public string AccountHolderName { get; set; }

        /// <summary>
        /// 2 character ISO country code
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// URL to redirect the customer to - only available in PENDING asynchronous transactions.
        /// Sent there so merchant can redirect consumer to complete an interrupted payment.
        /// </summary>
        public string RedirectUrl { get; set; }

        /// <summary>
        /// This parameter reflects what the customer will see on the proof of payment
        /// (for example, bank statement record and similar). Also known as the payment descriptor
        /// </summary>
        public string PaymentPurpose { get; set; }

        /// <summary>
        /// Alternative payment method identifier (e.g. <c>paypal</c>, <c>sofort</c>, <c>eraty</c>).
        /// </summary>
        public string PaymentMethod { get; set; }

        /// <summary>
        /// Reference assigned by the APM provider for the payment.
        /// </summary>
        public string ProviderReference { get; set; }

        /// <summary>
        /// Name of the APM provider that processed the payment.
        /// </summary>
        public string ProviderName { get; set; }

        /// <summary>
        /// Acknowledgement status returned by the APM provider.
        /// </summary>
        public string Ack { get; set; }

        /// <summary>
        /// Session token returned by the APM provider for follow-up requests.
        /// </summary>
        public string SessionToken { get; set; }

        /// <summary>
        /// Correlation identifier supplied by the APM provider for tracing the request.
        /// </summary>
        public string CorrelationReference { get; set; }

        /// <summary>
        /// API version reference reported by the APM provider.
        /// </summary>
        public string VersionReference { get; set; }

        /// <summary>
        /// Build reference reported by the APM provider.
        /// </summary>
        public string BuildReference { get; set; }

        /// <summary>
        /// Timestamp at which the APM provider created the payment record.
        /// </summary>
        public DateTime? TimeCreatedReference { get; set; }

        /// <summary>
        /// Reference assigned to the underlying APM transaction.
        /// </summary>
        public string TransactionReference { get; set; }

        /// <summary>
        /// Reference for the secure account used by the APM provider, when applicable.
        /// </summary>
        public string SecureAccountReference { get; set; }

        /// <summary>
        /// Reason code returned by the APM provider.
        /// </summary>
        public string ReasonCode { get; set; }

        /// <summary>
        /// Pending reason returned by the APM provider when the payment is awaiting completion.
        /// </summary>
        public string PendingReason { get; set; }

        /// <summary>
        /// Gross amount of the payment as reported by the APM provider.
        /// </summary>
        public decimal? GrossAmount { get; set; }

        /// <summary>
        /// Timestamp at which the APM provider recorded the payment.
        /// </summary>
        public DateTime? PaymentTimeReference { get; set; }

        /// <summary>
        /// Payment type reported by the APM provider (e.g. <c>instant</c>, <c>delayed</c>).
        /// </summary>
        public string PaymentType { get; set; }

        /// <summary>
        /// Current payment status reported by the APM provider.
        /// </summary>
        public string PaymentStatus { get; set; }

        /// <summary>
        /// Transaction type reported by the APM provider.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Protection eligibility flag reported by the APM provider for the payment.
        /// </summary>
        public string ProtectionEligibility { get; set; }

        /// <summary>
        /// Authorization status reported by the APM provider.
        /// </summary>
        public string AuthStatus { get; set; }

        /// <summary>
        /// Authorized amount reported by the APM provider.
        /// </summary>
        public decimal? AuthAmount { get; set; }

        /// <summary>
        /// Authorization acknowledgement returned by the APM provider.
        /// </summary>
        public string AuthAck { get; set; }

        /// <summary>
        /// Correlation identifier for the authorization request.
        /// </summary>
        public string AuthCorrelationReference { get; set; }

        /// <summary>
        /// API version reference for the authorization request.
        /// </summary>
        public string AuthVersionReference { get; set; }

        /// <summary>
        /// Build reference for the authorization request.
        /// </summary>
        public string AuthBuildReference { get; set; }

        /// <summary>
        /// Pending reason for the authorization, when applicable.
        /// </summary>
        public string AuthPendingReason { get; set; }

        /// <summary>
        /// Protection eligibility flag for the authorization.
        /// </summary>
        public string AuthProtectionEligibility { get; set; }

        /// <summary>
        /// Protection eligibility type reported for the authorization.
        /// </summary>
        public string AuthProtectionEligibilityType { get; set; }

        /// <summary>
        /// Reference assigned to the authorization by the APM provider.
        /// </summary>
        public string AuthReference { get; set; }

        /// <summary>
        /// Fee amount charged by the APM provider for the payment.
        /// </summary>
        public decimal? FeeAmount { get; set; }

        /// <summary>
        /// Bank details associated with the payment, when provided.
        /// </summary>
        public Bank Bank { get; set; }

        /// <summary>
        /// The payment category (e.g., BNPL for eRaty).
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// The redirect URL provided directly by the APM provider (e.g., eRaty).
        /// </summary>
        public string ProviderRedirectUrl { get; set; }

        /// <summary>
        /// The payer name returned by the APM provider.
        /// </summary>
        public string ProviderPayerName { get; set; }

        /// <summary>
        /// Installment terms for eRaty (BNPL) transactions.
        /// For eRaty, only <see cref="Terms.TimeUnit"/>, <see cref="Terms.Count"/>,
        /// and <see cref="Terms.Mode"/> are populated. Other <see cref="Terms"/>
        /// properties are used exclusively by the Mexico Installment (VIS) feature.
        /// </summary>
        public Terms Terms { get; set; }
    }
}
