using GlobalPayments.Api.Utils;
using System;

namespace GlobalPayments.Api.Entities {
    /// <summary>
    /// Alternative payment response data
    /// </summary>
    public class AlternativePaymentResponse
    {
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
        /// 
        /// </summary>
        public string PaymentMethod { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ProviderReference { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ProviderName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Ack { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string SessionToken { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string CorrelationReference { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string VersionReference { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string BuildReference { get; set; }

        public DateTime? TimeCreatedReference { get; set; }
        public string TransactionReference { get; set; }
        public string SecureAccountReference { get; set; }
        public string ReasonCode { get; set; }
        public string PendingReason { get; set; }
        public decimal? GrossAmount { get; set; }
        public DateTime? PaymentTimeReference { get; set; }
        public string PaymentType { get; set; }
        public string PaymentStatus { get; set; }
        public string Type { get; set; }
        public string ProtectionEligibility { get; set; }
        public string AuthStatus { get; set; }
        public decimal? AuthAmount { get; set; }
        public string AuthAck { get; set; }
        public string AuthCorrelationReference { get; set; }
        public string AuthVersionReference { get; set; }
        public string AuthBuildReference { get; set; }
        public string AuthPendingReason { get; set; }
        public string AuthProtectionEligibility { get; set; }
        public string AuthProtectionEligibilityType { get; set; }
        public string AuthReference { get; set; }
        public decimal? FeeAmount { get; set; }
    }
}
