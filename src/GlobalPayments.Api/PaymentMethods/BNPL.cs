using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Enums;
using GlobalPayments.Api.PaymentMethods.PaymentInterfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.PaymentMethods
{
    public class BNPL : IPaymentMethod, IAuthable, INotificationData
    {
        public PaymentMethodType PaymentMethodType { get { return PaymentMethodType.BNPL; } }

        public BNPLType BNPLType { get; set; }

        /// <summary>
        /// The endpoint to which the customer should be redirected after a payment has been attempted or
        /// successfully completed on the payment scheme's site.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        /// The endpoint which will receive payment-status messages.
        /// This will include the result of the transaction or any updates to the transaction status.
        /// For certain asynchronous payment methods these notifications may come hours or
        /// days after the initial authorization.
        /// </summary>
        public string StatusUpdateUrl { get; set; }

        /// <summary>
        /// The customer will be redirected back to your notifications.cancel_url in case the transaction is canceled
        /// </summary>
        public string CancelUrl { get; set; }

        public AuthorizationBuilder Authorize(decimal? amount = null, bool isEstimated = false) {
            return new AuthorizationBuilder(TransactionType.Auth, this)
                .WithModifier(TransactionModifier.BuyNowPayLater)
                .WithAmount(amount);
        }
    }
}
