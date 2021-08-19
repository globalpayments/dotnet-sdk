using GlobalPayments.Api.Utils;

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
    }
}
