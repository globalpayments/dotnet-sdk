using GlobalPayments.Api.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Entities
{
    public class PayByLinkData
    {
        /// <summary>
        /// Describes the type of link that will be created.
        /// </summary>
        public PayByLinkType? Type { get; set; }

        /// <summary>
        /// Indicates whether the link can be used once or multiple times
        /// </summary>
        public PaymentMethodUsageMode? UsageMode { get; set; }

        public PaymentMethodName[] AllowedPaymentMethods { get; set; }

        /// <summary>
        /// The number of the times that the link can be used or paid.
        /// </summary>
        public int? UsageLimit { get; set; }

        public PayByLinkStatus Status { get; set; }

        /// <summary>
        /// A descriptive name for the link. This will be visible to the customer on the payment page.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Indicates if you want to capture the customers shipping information on the hosted payment page.
        /// If you enable this field you can also set an optional shipping fee in the shipping_amount.
        /// </summary>
        public bool? IsShippable { get; set; }

        /// <summary>
        /// Indicates the cost of shipping when the shippable field is set to YES.
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
        /// The merchant URL that the customer will be redirected to.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        /// The merchant URL (webhook) to notify the merchant of the latest status of the transaction
        /// </summary>
        public string StatusUpdateUrl { get; set; }

        /// <summary>
        /// The merchant URL that the customer will be redirected to if they chose to cancel
        /// </summary>
        public string CancelUrl { get; set; }
    }
}
