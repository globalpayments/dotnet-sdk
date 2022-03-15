using System.Collections.Generic;
using GlobalPayments.Api.Entities.Billing;
using GlobalPayments.Api.Entities.Enums;
using GlobalPayments.Api.PaymentMethods;

namespace GlobalPayments.Api.Entities {
    /// <summary>
    /// Data collection to supplement a hosted payment page.
    /// </summary>
    public class HostedPaymentData {
        /// <summary>
        /// A value indicating to the issuer that the shipping and billing addresses
        /// are expected to be the same. Used as a fraud prevention.
        /// </summary>
        public bool? AddressesMatch { get; set; }

        /// <summary>
        /// Value used to determine the challenge request preference for 3DS2
        /// </summary>
        public ChallengeRequestIndicator ChallengeRequestIndicator { get; set; }

        /// <summary>
        /// The customer's email address
        /// </summary>
        public string CustomerEmail { get; set; }

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
        /// The customer's mobile phone number
        /// </summary>
        public string CustomerPhoneMobile { get; set; }

        /// <summary>
        /// A Country value representing the Country.
        /// </summary>
        public string CustomerCountry { get; set; }

        /// <summary>
        /// A StatusUpdateUrl is representing after the transaction
        /// Where the status response will come like SUCCESS/PENDING
        ///.
        /// </summary>
        public string TransactionStatusUrl { get; set; }

        /// <summary>
        /// A CancelUrl is representing during the payment
        /// Where the transaction cancels to .
        /// </summary>
        public string CancelUrl { get; set; }

        /// <summary>
        /// A ReturnUrl is representing after the payment
        /// Where the transaction return to.
        /// </summary>
        public string MerchantResponseUrl { get; set; }

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
        /// The customer's Address
        /// </summary>
        public Address CustomerAddress { get; set; }

        /// <summary>
        /// Allows customer information to be editable on hosted page if supported
        /// </summary>
        public bool CustomerIsEditable { get; set; }

        /// <summary>
        /// Type of hosted page payment
        /// </summary>
        public HostedPaymentType HostedPaymentType { get; set; }

        /// <summary>
        /// List of bills for hosted pages
        /// </summary>
        public IEnumerable<Bill> Bills { get; set; }

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
