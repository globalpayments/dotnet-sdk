using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Entities.Billing {
    public class BillingResponse {
        /// <summary>
        /// Indicates if the action was succesful
        /// </summary>
        internal bool IsSuccessful { get; set; }

        /// <summary>
        /// The response code from the Billing Gateway
        /// </summary>
        public string ResponseCode { get; set; }

        /// <summary>
        /// The response message from the Billing Gateway
        /// </summary>
        public string ResponseMessage { get; set; }
    }
}
