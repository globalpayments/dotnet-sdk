using GlobalPayments.Api.PaymentMethods;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Entities.PayFac
{
    public class RenewAccountData {
        /// <summary>
        /// Supplying a value will change the account's tier under the affiliation upon renewal
        /// If not passed, the tier will not be changed
        /// </summary>
        public string Tier { get; set; }

        /// <summary>
        /// Credit Card details
        /// </summary>
        public CreditCardData CreditCard { get; set; }

        /// <summary>
        /// The US zip code of the credit card. 5 or 9 digits without a dash for US cards. Omit for internation credit cards.
        /// </summary>
        public string ZipCode { get; set; }

        /// <summary>
        /// User to pay for an account via ACH and monthly renewal. Financial institution account number.
        /// *Required if using ACH to pay renewal fee
        /// </summary>
        public string PaymentBankAccountNumber { get; set; }

        /// <summary>
        /// Used to pay for an account via ACH and monthly renewal. Financial institution account number.
        /// *Required if using ACH to pay renewal fee
        /// </summary>
        public string PaymentBankRoutingNumber { get; set; }

        /// <summary>
        /// Used to pay for an account via ACH and monthly renewal. Valid values are: Checking and Savings
        /// </summary>
        public string PaymentBankAccountType { get; set; }

        public RenewAccountData() {
            CreditCard = new CreditCardData();
        }
    }
}
