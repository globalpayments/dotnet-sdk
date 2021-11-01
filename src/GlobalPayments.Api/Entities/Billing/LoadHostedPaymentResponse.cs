namespace GlobalPayments.Api.Entities.Billing {
    public class LoadHostedPaymentResponse : BillingResponse {
        /// <summary>
        /// Unique identifier for the hosted payment page
        /// </summary>
        public string PaymentIdentifier { get; set; }
    }
}
