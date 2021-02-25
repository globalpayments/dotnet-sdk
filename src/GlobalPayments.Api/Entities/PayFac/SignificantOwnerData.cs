using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Entities.PayFac
{
    public class SignificantOwnerData {
        /// <summary>
        /// Seller's authorized Signer First Name. By default Merchant's First name is saved.
        /// </summary>
        public string AuthorizedSignerFirstName { get; set; }
        /// <summary>
        /// Seller's Authorized Signer Last Name. By default Merchant's Last name is saved.
        /// </summary>
        public string AuthorizedSignerLastName { get; set; }
        /// <summary>
        /// This field contains the Seller's Authorized Signer Title
        /// </summary>
        public string AuthorizedSignerTitle { get; set; }
        /// <summary>
        /// Seller's Authorized Signer owner information
        /// </summary>
        public OwnersData SignificantOwner { get; set; }

        public SignificantOwnerData() {
            SignificantOwner = new OwnersData();
        }
    }
}
