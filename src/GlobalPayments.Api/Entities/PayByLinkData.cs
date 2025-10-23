using GlobalPayments.Api.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// Contains entity classes and enumerations related to Pay By Link payment functionality,
/// including data models, configuration options, and status definitions for the GlobalPayments API.
/// </summary>
namespace GlobalPayments.Api.Entities {
    /// <summary>
    /// Represents the data required to create and manage a Pay By Link payment.
    /// </summary>
    public class PayByLinkData {

        /// <summary>
        /// Describes the type of link that will be created (e.g., payment, hosted page, third-party page).
        /// </summary>
        public PayByLinkType? Type { get; set; }

        /// <summary>
        /// Indicates whether the link can be used once or multiple times.
        /// </summary>
        public PaymentMethodUsageMode? UsageMode { get; set; }

        /// <summary>
        /// Specifies the payment methods that are allowed for this Pay By Link.
        /// </summary>
        public PaymentMethodName[] AllowedPaymentMethods { get; set; }

        /// <summary>
        /// The number of times that the link can be used or paid.
        /// </summary>
        public int? UsageLimit { get; set; }

        /// <summary>
        /// Gets or sets the current status of the Pay By Link (e.g., active, closed, expired).
        /// </summary>
        public PayByLinkStatus Status { get; set; }

        /// <summary>
        /// A descriptive name for the link. This will be visible to the customer on the payment page.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Indicates if you want to capture the customer's shipping information on the hosted payment page.
        /// </summary>
        public bool? IsShippable { get; set; }

        /// <summary>
        /// Indicates the cost of shipping when the shippable field is set to true.
        /// </summary>
        public decimal ShippingAmount { get; set; }

        /// <summary>
        /// Indicates the date and time after which the link can no longer be used or paid.
        /// </summary>
        public DateTime? ExpirationDate { get; set; }

        /// <summary>
        /// Images that will be displayed to the customer on the payment page.
        /// </summary>
        public string[] Images { get; set; }

        /// <summary>
        /// The merchant URL that the customer will be redirected to after payment.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        /// The merchant URL (webhook) to notify the merchant of the latest status of the transaction.
        /// </summary>
        public string StatusUpdateUrl { get; set; }

        /// <summary>
        /// The merchant URL that the customer will be redirected to if they choose to cancel.
        /// </summary>
        public string CancelUrl { get; set; }

        /// <summary>
        /// Determines whether Dynamic Currency Conversion (DCC) is activated for the transaction.
        /// </summary>
        public bool? IsDccEnabled { get; set; }

        /// <summary>
        /// Gets or sets the configuration options for the payment method, such as address requirements,
        /// challenge request indicators, exemption status, and storage mode.
        /// </summary>
        public PaymentMethodConfiguration Configuration { get; set; }
    }
}
