using GlobalPayments.Api.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Entities.Reporting
{
    public class MerchantSummary
    {
        /// <summary>
        /// A unique identifier for the object created by Global Payments. The first 3 characters identifies the resource an id relates to.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The label to identify the merchant
        /// </summary>
        public string Name { get; set; }

   
        public UserStatus? Status { get; set; }

       
        public List<UserLinks> Links { get; set; }
    }
}
