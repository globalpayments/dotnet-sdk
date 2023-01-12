using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.PaymentMethods.PaymentInterfaces
{
    public interface INotificationData
    {
        /// <summary>
        /// A ReturnUrl is representing after the payment
        /// Where the transaction return to.
        /// </summary>
        string ReturnUrl { get; set; }

        /// <summary>
        /// A StatusUpdateUrl is representing after the transaction
        /// Where the status response will come like SUCCESS/PENDING
        ///.
        /// </summary>
        string StatusUpdateUrl { get; set; }

        /// <summary>
        /// A CancelUrl is representing during the payment
        /// Where the transaction cancels to .
        /// </summary>
        string CancelUrl { get; set; }
    }
}
