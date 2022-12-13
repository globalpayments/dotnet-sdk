using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Entities.PayFac
{
    public class BankAccountData {
        /// <summary>
        /// ISO 3166 standard 3-character country code
        /// </summary>
        public string AccountCountryCode { get; set; }
        /// <summary>
        /// Merchant/Individual Name
        /// </summary>
        public string AccountName { get; set; }
        /// <summary>
        /// Financial Institution account number
        /// </summary>
        public string AccountNumber { get; set; }
        /// <summary>
        /// Valid values are: Personal and Business
        /// </summary>
        public string AccountOwnershipType { get; set; }
        /// <summary>
        /// Valid Values are:
        /// C - Checking
        /// S - Savings
        /// G - General Ledger
        /// </summary>
        public string AccountType { get; set; }
        /// <summary>
        /// Name of financial institution
        /// </summary>
        public string BankName { get; set; }
        /// <summary>
        /// Financial institution routing number. Must be a valid ACH routing number.
        /// </summary>
        public string RoutingNumber { get; set; }

        /// <summary>
        /// The account holder's name. This is required if payment method is a bank account.
        /// </summary>
        public string AccountHolderName { get; set; }

        public Address BankAddress { get; set; }
    }
}
