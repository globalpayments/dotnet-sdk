using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Entities
{
    public class CardIssuerResponse
    {
        /// <summary>
        /// The result code of the AVS check from the card issuer.
        /// </summary>
        public string AvsResult { get; set; }

        /// <summary>
        /// Result code from the card issuer.
        /// </summary>
        public string Result { get; set; }

        /// <summary>
        /// The result code of the CVV check from the card issuer.
        /// </summary>
        public string CvvResult { get; set; }

        /// <summary>
        /// The result code of the AVS address check from the card issuer.
        /// </summary>
        public string AvsAddressResult { get; set; }

        /// <summary>
        /// The result of the AVS postal code check from the card issuer.
        /// </summary>
        public string AvsPostalCodeResult { get; set; }
    }
}
