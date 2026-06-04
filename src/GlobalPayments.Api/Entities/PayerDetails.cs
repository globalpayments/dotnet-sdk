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
    }
}
