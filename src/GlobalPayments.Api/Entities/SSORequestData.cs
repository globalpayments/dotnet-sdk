using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Entities
{
    public class SSORequestData {
        /// <summary>
        /// The ProPay system requires that your single-sign-on originate from the URL originally provided here
        /// </summary>
        public string ReferrerURL { get; set; }
        /// <summary>
        /// The ProPay system requires that your single-sign-on originate from the URL originally provided here. Can supply a range of class c or more restrictive
        /// </summary>
        public string IPAddress { get; set; }
        /// <summary>
        /// The ProPay system requires that your single-sign-on originate from the URL originally provided here. Can supply a range of class c or more restrictive
        /// </summary>
        public string IPSubnetMask { get; set; }
    }
}
