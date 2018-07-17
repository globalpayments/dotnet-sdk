using System.Collections.Generic;
using GlobalPayments.Api.PaymentMethods;

namespace GlobalPayments.Api.Entities {
    /// <summary>
    /// Data collection to supplement a hosted payment page.
    /// </summary>
    public class HostedPaymentData: AlternatePaymentMethod {
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

        /// <summary>
        /// The customer's FirstName.
        /// </summary>
        public string CustomerFirstName { get; set; }

        /// <summary>
        /// The customer's Lastname.
        /// </summary>
        public string CustomerLastName { get; set; }

        /// <summary>
        /// The Alternative Payment Type is an Array which store
        /// Different types of the PaymentMethods Available.
        /// </summary>
        public AlternativePaymentType[] PresetPaymentMethods { get; set; }

        public HostedPaymentData() {
            SupplementaryData = new Dictionary<string, string>();
        }
    }
}
