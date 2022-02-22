using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Entities.PayFac
{
    public class PayFacResponseData {
        public string AccountNumber { get; set; }
        public string RecAccountNum { get; set; }
        public string Password { get; set; }
        public string Amount { get; set; }
        public string TransNum { get; set; }
        public string Pending { get; set; }
        public string SecondaryAmount { get; set; }
        public string SecondaryTransNum { get; set; }
        public string SourceEmail { get; set; }
        public string AuthToken { get; set; }
        public List<BeneficialOwnerDataResult> BeneficialOwnerDataResults { get; set; }

        // Account information
        public string AccountStatus { get; set; }
        public Address PhysicalAddress { get; set; }
        public string Affiliation { get; set; }
        public string APIReady { get; set; }
        public string CurrencyCode { get; set; }
        public string Expiration { get; set; }
        public string SignupDate { get; set; }
        public string Tier { get; set; }
        public string VisaCheckoutMerchantID { get; set; }
        public string CreditCardTransactionLimit { get; set; }
        public string CreditCardMonthLimit { get; set; }
        public string ACHPaymentPerTranLimit { get; set; }
        public string ACHPaymentMonthLimit { get; set; }
        public string CreditCardMonthlyVolume { get; set; }
        public string ACHPaymentMonthlyVolume { get; set; }
        public string ReserveBalance { get; set; }
        public string MasterPassCheckoutMerchantID { get; set; }

        // Enhanced Account Details
        public UserPersonalData PersonalData { get; set; }
        public Address HomeAddress { get; set; }
        public Address MailAddress { get; set; }
        public BusinessData BusinessData { get; set; }
        public AccountPermissions AccountLimits { get; set; }
        public string AvailableBalance { get; set; }
        public string PendingBalance { get; set; }
        public AccountBalanceResponseData AccountBalance { get; set; }
        public BankAccountData PrimaryBankAccountData { get; set; }
        public BankAccountData SecondaryBankAccountData { get; set; }
        public GrossBillingInformation GrossBillingInformation { get; set; }

        // Account balance
        public string PendingAmount { get; set; }
        public string ReserveAmount { get; set; }
        public AccountBalanceResponseData ACHOut { get; set; }
        public AccountBalanceResponseData FlashFunds { get; set; }

        // Portico data

        /// <summary>
        /// A unique transaction ID assigned by the payment facilitator
        /// </summary>
        public string TransactionId { get; set; }
        /// <summary>
        /// A unique sub-merchant account ID assigned by the payment facilitator
        /// </summary>
        public string TransactionNumber { get; set; }
    }

    public class BeneficialOwnerDataResult {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Status { get; set; }
    }

    public class AccountBalanceResponseData {
        public string Enabled { get; set; }
        public string LimitRemaining { get; set; }
        public string TransferFee { get; set; }
        public string FeeType { get; set; }
        public string AccountLastFour { get; set; }
    }
}
