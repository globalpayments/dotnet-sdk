using GlobalPayments.Api.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Entities.Reporting
{
    public class MerchantAccountSummary
    {
        public string Id { get; set; }

        public MerchantAccountType? Type { get; set; }

        public string Name { get; set; }

        public MerchantAccountStatus? Status { get; set; }

        public List<Channel> Channels { get; set; }

        public List<string> Permissions { get; set; }

        public List<string> Countries { get; set; }

        public List<string> Currencies { get; set; }

        public List<PaymentMethodName> PaymentMethods { get; set; }

        public List<string> Configurations { get; set; }

        public List<Address> Addresses { get; set; }

        /// <summary>
        /// The structured configurations returned by GP-API. Legacy string configurations remain in Configurations.
        /// </summary>
        public List<AccountConfiguration> AccountConfigurations { get; set; }

        /// <summary>
        /// The email address associated with the account.
        /// </summary>
        public string EmailId { get; set; }

        /// <summary>
        /// The merchant that owns the account.
        /// </summary>
        public string MerchantId { get; set; }

        /// <summary>
        /// The merchant's display name.
        /// </summary>
        public string MerchantName { get; set; }

        /// <summary>
        /// The account creation timestamp supplied by GP-API.
        /// </summary>
        public string TimeCreated { get; set; }

        /// <summary>
        /// The last update timestamp supplied by GP-API.
        /// </summary>
        public string TimeLastUpdated { get; set; }

        /// <summary>
        /// The replacement reason returned in the documented card_replacment_reason field.
        /// </summary>
        public string CardReplacementReason { get; set; }

        /// <summary>
        /// The account's enabled and disabled capabilities.
        /// </summary>
        public List<string> Capabilities { get; set; }

        /// <summary>
        /// The processing platform associated with the account.
        /// </summary>
        public AccountPlatform Platform { get; set; }

        /// <summary>
        /// The fund management account's balances, limits, and processing statistics.
        /// </summary>
        public AccountProcessingStatistics PaymentProcessingStatistics { get; set; }

        /// <summary>
        /// Metadata for the account action executed by GP-API.
        /// </summary>
        public Action Action { get; set; }
    }
}
