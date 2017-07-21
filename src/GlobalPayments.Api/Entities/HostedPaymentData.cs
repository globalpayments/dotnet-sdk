using System.Collections.Generic;

namespace GlobalPayments.Api.Entities {
    /// <summary>
    /// Data collection to supplement a hosted payment page.
    /// </summary>
    public class HostedPaymentData {
        /// <summary>
        /// Indicates if the customer is known and has an account.
        /// </summary>
        public bool? CustomerExists { get; set; }

        /// <summary>
        /// The identifier for the customer.
        /// </summary>
        public string CustomerKey { get; set; }

        /// <summary>
        /// The customer's number.
        /// </summary>
        public string CustomerNumber { get; set; }

        /// <summary>
        /// Indicates if the customer should be prompted to store their card.
        /// </summary>
        public bool? OfferToSaveCard { get; set; }

        /// <summary>
        /// The identifier for the customer's desired payment method.
        /// </summary>
        public string PaymentKey { get; set; }

        /// <summary>
        /// The product ID.
        /// </summary>
        public string ProductId { get; set; }

        /// <summary>
        /// Supplementary data that can be supplied at the descretion
        /// of the merchant/application.
        /// </summary>
        public Dictionary<string, string> SupplementaryData { get; set; }

        public HostedPaymentData() {
            SupplementaryData = new Dictionary<string, string>();
        }
    }
}
