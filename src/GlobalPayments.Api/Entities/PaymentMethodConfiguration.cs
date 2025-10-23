using GlobalPayments.Api.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Entities {
    /// <summary>
    /// Represents the configuration options for a payment method, including address requirements,
    /// challenge request indicators, exemption status, and storage mode.
    /// </summary>
    public class PaymentMethodConfiguration {

        /// <summary>
        /// Gets or sets the challenge request indicator for 3D Secure authentication.
        /// </summary>
        public ChallengeRequestIndicator? ChallengeRequestIndicator { get; set; }

        /// <summary>
        /// Gets or sets the exemption status for Strong Customer Authentication (SCA).
        /// </summary>
        public ExemptStatus ExemptStatus { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the billing address is required.
        /// </summary>
        public bool IsBillingAddressRequired { get; set; }

        /// <summary>
        /// Indicates whether the shipping address is required on the hosted payment page.
        /// </summary>
        public bool? IsShippingAddressEnabled { get; set; }

        /// <summary>
        /// Indicates whether the shipping address can be changed by the customer on the PayPal review page.
        /// </summary>
        public bool? IsAddressOverrideAllowed { get; set; }

        /// <summary>
        /// Indicates whether to store the card as part of a transaction.
        /// </summary>
        public StorageMode StorageMode { get; set; }
    }
}
