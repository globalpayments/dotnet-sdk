using GlobalPayments.Api.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Entities.PayFac
{
    public class UserPersonalData {
        /// <summary>
        /// Merchant/Individual first name
        /// </summary>
        public string FirstName { get; set; }
        /// <summary>
        /// Merchant/Individual middle initial
        /// </summary>
        public string MiddleInitial { get; set; }
        /// <summary>
        /// Merchant/Individual lane name
        /// </summary>
        public string LastName { get; set; }
        /// <summary>
        /// Merchant/Individual first name
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// Merchant/Individual date of birth. Must be in 'mm-dd-yyyy' format. Individual must be 18+ to obtain an account. The value 01-01-1981 will give a successul response. All others will return a Status 66 (Failed KYC).
        /// </summary>
        public string DateOfBirth { get; set; }
        /// <summary>
        /// Merchant/Individual social security number. Must be 9 characters without dashes. Required for USA when using personal validation. If business validated, do not pass!
        /// </summary>
        public string SSN { get; set; }
        /// <summary>
        /// Merchant/Individual email address. Must be unique in ProPay system. ProPay's system will send automated emails to the email address on file unless NotificationEmail is provided. This value is truncated beyond 55 characters.
        /// </summary>
        public string SourceEmail { get; set; }
        /// <summary>
        /// Merchant/Individual day phone number. For USA, CAN, NZL, and AUS value must be 10 characters
        /// </summary>
        public string DayPhone { get; set; }
        /// <summary>
        /// Merchant/Individual evening phone number. For USA, CAN, NZL, and AUS value must be 10 characters
        /// </summary>
        public string EveningPhone { get; set; }
        /// <summary>
        /// Communication email address. ProPay's system will send automated emails to the email address on file rather than the source email
        /// </summary>
        public string NotificationEmail { get; set; }
        /// <summary>
        /// Required to specify the currency in which funds should be held, if other than USD. An affiliation must be granted permission to create accounts in currencies other than USD. ISO 4217 standard 3 character currency code.
        /// </summary>
        public string CurrencyCode { get; set; }
        /// <summary>
        /// One of the previously assigned merchant tiers. If not provided, will default to cheapest available tier.
        /// </summary>
        public string Tier { get; set; }
        /// <summary>
        /// This is a partner's own unique identifier. Typically used as the distributor or consultant ID
        /// </summary>
        public string ExternalID { get; set; }
        /// <summary>
        ///  Numeric value which will give a user access to ProPay's IVR system. Can also be used to reset password
        /// </summary>
        public string PhonePIN { get; set; }
        /// <summary>
        /// ProPay account username. Must be unique in ProPay system. Username defaults to <sourceEmail> if userId is not provided
        /// </summary>
        public string UserID { get; set; }

        public string IpSignup { get; set; }
        public Boolean USCitizen { get; set; }
        public Boolean BOAttestation { get; set; }
        public string TermsAcceptanceIP { get; set; }
        public string TermsAcceptanceTimeStamp { get; set; }
        public string TermsVersion { get; set; }

        /// <summary>
        /// Merchant/Individual address
        /// </summary>
        public Address UserAddress { get; set; }
        /// <summary>
        /// Business physical address
        /// </summary>
        public Address MailingAddress { get; set; }

        /// <summary>
        /// The legal business name of the merchant being boarded.
        /// </summary>
        public string LegalName { get; set; }       

        /// <summary>
        /// The merchant's DBA (Doing Business As) name or the alternate name the merchant may be known as.
        /// </summary>
        public string DBA { get; set; }

        /// <summary>
        /// A four-digit number used to classify the merchant into an industry or market segment.
        /// </summary>
        public int MerchantCategoryCode { get; set; }

        /// <summary>
        /// The merchant's business website URL
        /// </summary>
        public string Website { get; set; }

        public UserType? Type { get; set; }

        public string NotificationStatusUrl { get; set; }

        /// <summary>
        /// The merchants tax identification number. For example, in the US the (EIN) Employer Identification Number would be used.
        /// </summary>
        public string TaxIdReference { get; set; }

        public UserPersonalData() {
            UserAddress = new Address();
            MailingAddress = new Address();
        }
    }
}
