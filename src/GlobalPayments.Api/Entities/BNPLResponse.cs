using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Entities
{
    public class BNPLResponse
    {
       public string ProviderName { get; set; }

        /// <summary>
        /// URL to redirect the customer, sent so merchant can redirect consumer to complete the payment.
        /// </summary>
        public string RedirectUrl { get; set; }
    }
}
