using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Entities {
    /// <summary>
    /// Represents the payer details returned by the gateway for a transaction.
    /// </summary>
    public class PayerDetails {
        /// <summary>
        /// Gateway-assigned unique identifier for the payer (e.g. GP-API <c>payer.id</c>).
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Merchant-defined reference for the payer, returned in the GP-API <c>payer.reference</c> field.
        /// </summary>
        public string Reference { get; set; }

        /// <summary>
        /// Payer's first name.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Payer's last name.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Payer's full name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Payer's mobile phone number in international format.
        /// </summary>
        public string MobilePhone { get; set; }

        /// <summary>
        /// Payer's email address.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Payer's country in ISO 3166-1 alpha-2 format.
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Payer's billing address.
        /// </summary>
        public Address BillingAddress { get; set; }

        /// <summary>
        /// Payer's shipping address.
        /// </summary>
        public Address ShippingAddress { get; set; }

        /// <summary>
        /// Payer's language/locale (e.g. GP-API <c>payer.language</c> = <c>en-GB</c>).
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// How the payer was verified by the wallet (e.g. GP-API <c>payer.verification_type</c> =
        /// <c>MOBILE_PHONE_NUMBER</c>).
        /// </summary>
        public string VerificationType { get; set; }

        /// <summary>
        /// Timestamp the payer record was created, returned in the GP-API <c>payer.time_created</c> field.
        /// </summary>
        public string TimeCreated { get; set; }
    }
}
