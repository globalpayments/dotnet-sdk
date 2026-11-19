using System;

namespace GlobalPayments.Api.Entities.Reporting {
    /// <summary
    /// >Represents a GP-API authentication response.
    /// </summary>
    public class AuthenticationSummary {
        /// <summary>
        /// Gets or sets the authentication identifier.
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Gets or sets the creation timestamp.
        /// </summary>
        public DateTime? TimeCreated { get; set; }
        /// <summary>
        /// Gets or sets the last-update timestamp.
        /// </summary>
        public DateTime? TimeLastUpdated { get; set; }
        /// <summary>
        /// Gets or sets the transaction type.
        /// </summary>
        public string TransactionType { get; set; }
        /// <summary>
        /// Gets or sets the authentication lifecycle status.
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// Gets or sets the processing channel.
        /// </summary>
        public string Channel { get; set; }
        /// <summary>
        /// Gets or sets the amount in major currency units.
        /// </summary>
        public decimal? Amount { get; set; }
        /// <summary>
        /// Gets or sets the ISO 4217 currency code.
        /// </summary>
        public string Currency { get; set; }
        /// <summary>
        /// Gets or sets the ISO 3166-1 alpha-2 country code.
        /// </summary>
        public string Country { get; set; }
        /// <summary>
        /// Gets or sets the authentication source.
        /// </summary>
        public string Source { get; set; }
        /// <summary>
        /// Gets or sets the merchant identifier.
        /// </summary>
        public string MerchantId { get; set; }
        /// <summary>
        /// Gets or sets the merchant name.
        /// </summary>
        public string MerchantName { get; set; }
        /// <summary>
        /// Gets or sets the processing account identifier.
        /// </summary>
        public string AccountId { get; set; }
        /// <summary>
        /// Gets or sets the processing account name.
        /// </summary>
        public string AccountName { get; set; }
        /// <summary>
        /// Gets or sets the merchant reference.
        /// </summary>
        public string Reference { get; set; }
        /// <summary>
        /// Gets or sets payment-method response details.
        /// </summary>
        public AuthenticationPaymentMethodSummary PaymentMethod { get; set; }
        /// <summary>
        /// Gets or sets 3DS authentication response details.
        /// </summary>
        public ThreeDSecure ThreeDSecure { get; set; }
        /// <summary>
        /// Gets or sets notification details.
        /// </summary>
        public AuthenticationNotificationsSummary Notifications { get; set; }
        /// <summary>
        /// Gets or sets the identifier of the action that created this authentication.
        /// </summary>
        public string ActionCreateId { get; set; }
        /// <summary>
        /// Gets or sets merchant system identifiers.
        /// </summary>
        public AuthenticationSystemSummary System { get; set; }
        /// <summary>
        /// Gets or sets the action associated with the response.
        /// </summary>
        public Action Action { get; set; }
    }

    /// <summary>
    /// Represents the payment method in an authentication response.
    /// </summary>
    public class AuthenticationPaymentMethodSummary {
        /// <summary>
        /// Gets or sets the payment-method result code.
        /// </summary>
        public string Result { get; set; }
        /// <summary>
        /// Gets or sets the payment-method result message.
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// Gets or sets the payment entry mode.
        /// </summary>
        public string EntryMode { get; set; }
        /// <summary>
        /// Gets or sets the payment-method fingerprint.
        /// </summary>
        public string Fingerprint { get; set; }
        /// <summary>
        /// Gets or sets the fingerprint presence indicator.
        /// </summary>
        public string FingerprintPresenceIndicator { get; set; }
        /// <summary>
        /// Gets or sets the payment-method name.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets card response details.
        /// </summary>
        public AuthenticationCardSummary Card { get; set; }
        /// <summary>
        /// Gets or sets 3DS details associated with the payment method.
        /// </summary>
        public ThreeDSecure ThreeDSecure { get; set; }
    }

    /// <summary>
    /// Represents card response fields associated with an authentication.
    /// </summary>
    public class AuthenticationCardSummary {
        /// <summary>
        /// Gets or sets the card funding type.
        /// </summary>
        public string Funding { get; set; }
        /// <summary>
        /// Gets or sets the card brand.
        /// </summary>
        public string Brand { get; set; }
        /// <summary>
        /// Gets or sets the issuer authorization code.
        /// </summary>
        public string AuthCode { get; set; }
        /// <summary>
        /// Gets or sets the scheme brand reference.
        /// </summary>
        public string BrandReference { get; set; }
        /// <summary>
        /// Gets or sets the first-six/last-four masked card number.
        /// </summary>
        public string MaskedNumberFirst6Last4 { get; set; }
        /// <summary>
        /// Gets or sets the CVV presence indicator.
        /// </summary>
        public string CvvIndicator { get; set; }
        /// <summary>
        /// Gets or sets the CVV verification result.
        /// </summary>
        public string CvvResult { get; set; }
        /// <summary>
        /// Gets or sets the AVS address result.
        /// </summary>
        public string AvsAddressResult { get; set; }
        /// <summary>
        /// Gets or sets the AVS postal-code result.
        /// </summary>
        public string AvsPostalCodeResult { get; set; }
    }

    /// <summary>
    /// Represents merchant system identifiers returned with an authentication.
    /// </summary>
    public class AuthenticationSystemSummary {
        /// <summary>
        /// Gets or sets the merchant identifier from the system object.
        /// </summary>
        public string Mid { get; set; }
        /// <summary>
        /// Gets or sets the terminal identifier.
        /// </summary>
        public string Tid { get; set; }
        /// <summary>
        /// Gets or sets the system name.
        /// </summary>
        public string Name { get; set; }
    }

    /// <summary>
    /// Represents notification fields returned with an authentication.
    /// </summary>
    public class AuthenticationNotificationsSummary {
        /// <summary>
        /// Gets or sets the challenge return URL.
        /// </summary>
        public string ChallengeReturnUrl { get; set; }
    }
}