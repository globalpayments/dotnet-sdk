using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Entities.Enums {
    /// <summary>
    /// Represents the types of Pay By Link options available in the payment system.
    /// </summary>
    public enum PayByLinkType {
        /// <summary>
        /// Standard payment link.
        /// </summary>
        PAYMENT,

        /// <summary>
        /// Hosted payment page link.
        /// </summary>
        HOSTED_PAYMENT_PAGE,

        /// <summary>
        /// Third-party payment page link.
        /// </summary>
        THIRD_PARTY_PAGE,

        /// <summary>
        /// Link for exchanging application credentials.
        /// </summary>
        EXCHANGE_APP_CREDENTIALS
    }
}
