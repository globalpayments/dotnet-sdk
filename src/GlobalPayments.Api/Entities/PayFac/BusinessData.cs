using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Entities.PayFac
{
    public class BusinessData {
        /// <summary>
        /// The legal name of the business as registered
        /// </summary>
        public string BusinessLegalName { get; set; }
        /// <summary>
        /// This field can be used to provide DBA information on an account. ProPay accounts can be configured to display DBA on cc statements. (Note most banks' CC statements allow for 29 characters)
        /// </summary>
        public string DoingBusinessAs { get; set; }
        /// <summary>
        /// EIN - 9 characters without dashes 
        /// </summary>
        public string EmployerIdentificationNumber { get; set; }

        /// <summary>
        /// Merchant Category Code
        /// </summary>
        public string MerchantCategoryCode { get; set; }

        /// <summary>
        /// The business' website URL
        /// </summary>
        public string WebsiteURL { get; set; }
        /// <summary>
        /// The business' description
        /// </summary>
        public string BusinessDescription { get; set; }
        /// <summary>
        /// The monthly colume of bank card transactions; Value representing the number of pennies in USD, or the number of [currency] without decimals. Defaults to $1000.00 if not sent.
        /// </summary>
        public string MonthlyBankCardVolume { get; set; }
        /// <summary>
        /// The average amount of an individual transaction; Value representing the number of pennies in USD, or the number of [currency] without decimals. Defaults to $300.00 if not sent.
        /// </summary>
        public string AverageTicket { get; set; }
        /// <summary>
        /// The highest transaction amount; Value representing the number of pennies in USD, or the number of [currency] without decimals. Defaults to $300.00 if not sent.
        /// </summary>
        public string HighestTicket { get; set; }

        public string BusinessType { get; set; }
        // The business' address
        public Address BusinessAddress { get; set; }

        public BusinessData() {
            BusinessAddress = new Address();
        }
    }
}
