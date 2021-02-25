using GlobalPayments.Api.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Entities.PayFac
{
    public class AccountPermissions {
        /// <summary>
        /// Account permitted to load funds via ACH. Valid values are: Y and N
        /// </summary>
        public bool? ACHIn { get; set; }

        /// <summary>
        /// Account balance allowed to be pushed to on-file DDA. Affects automatic sweeps. Valid values are: Y and N
        /// </summary>
        public bool? ACHOut { get; set; }

        /// <summary>
        /// Valid values are: Y and N
        /// </summary>
        public bool? CCProcessing { get; set; }

        /// <summary>
        /// Valid values are: Y and N
        /// </summary>
        public bool? ProPayIn { get; set; }

        /// <summary>
        /// Valid values are: Y and N
        /// </summary>
        public bool? ProPayOut { get; set; }

        /// <summary>
        /// Valid values between 0 and 999999999. Expressed as number of pennies in USD or number of account's currency without decimals
        /// </summary>
        public string CreditCardMonthLimit { get; set; }

        /// <summary>
        /// Valid values between 0 and 999999999. Expressed as number of pennies in USD or number of account's currency without decimals
        /// </summary>
        public string CreditCardTransactionLimit { get; set; }

        /// <summary>
        /// Used to updated status of ProPay account. Note: the ONLY value that will allow an account to process transactions is "ReadyToProcess"
        /// </summary>
        public ProPayAccountStatus MerchantOverallStatus { get; set; }

        /// <summary>
        /// Valid values are Y and N. Please work with ProPay for more information about soft limits feature
        /// </summary>
        public bool? SoftLimitEnabled { get; set; }

        /// <summary>
        /// Valid values are Y and N. Please work with ProPay for more information about soft limits feature
        /// </summary>
        public bool? ACHPaymentSoftLimitEnabled { get; set; }

        /// <summary>
        /// Valid values between 0 and 499. Please work with ProPay for more information about soft limits feature
        /// </summary>
        public string SoftLimitACHOffPercent { get; set; }

        /// <summary>
        /// Valid values between 0 and 499. Please work with ProPay for more information about soft limits feature
        /// </summary>
        public string ACHPaymentACHOffPercent { get; set; }
    }
}
