using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;

namespace GlobalPayments.Api.PaymentMethods {
    /// <summary>
    /// Use credit as a payment method.
    /// </summary>
    public abstract class Credit : IPaymentMethod, IEncryptable, ITokenizable, IChargable, IAuthable, IRefundable, IReversable, IVerifiable, IPrePayable, IBalanceable {
        /// <summary>
        /// The card's encryption data; where applicable.
        /// </summary>
        public EncryptionData EncryptionData { get; set; }

        /// <summary>
        /// Set to `PaymentMethodType.Credit` for internal methods.
        /// </summary>
        public PaymentMethodType PaymentMethodType { get { return PaymentMethodType.Credit; } }

        /// <summary>
        /// A token value representing the card.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Creates an authorization against the payment method.
        /// </summary>
        /// <param name="amount">The amount of the transaction</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder Authorize(decimal? amount = null) {
            return new AuthorizationBuilder(TransactionType.Auth, this).WithAmount(amount);
        }

        /// <summary>
        /// Creates a charge (sale) against the payment method.
        /// </summary>
        /// <param name="amount">The amount of the transaction</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder Charge(decimal? amount = null) {
            return new AuthorizationBuilder(TransactionType.Sale, this).WithAmount(amount);
        }

        /// <summary>
        /// Adds value to to a payment method.
        /// </summary>
        /// <param name="amount">The amount of the transaction</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder AddValue(decimal? amount = null) {
            return new AuthorizationBuilder(TransactionType.AddValue, this).WithAmount(amount);
        }

        /// <summary>
        /// Completes a balance inquiry (lookup) on the payment method.
        /// </summary>
        /// <param name="inquiry">The type of inquiry to make</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder BalanceInquiry(InquiryType? inquiry = null) {
            return new AuthorizationBuilder(TransactionType.Balance, this).WithBalanceInquiryType(inquiry);
        }

        /// <summary>
        /// Refunds the payment method.
        /// </summary>
        /// <param name="amount">The amount of the transaction</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder Refund(decimal? amount = null) {
            return new AuthorizationBuilder(TransactionType.Refund, this).WithAmount(amount);
        }

        /// <summary>
        /// Reverses a previous transaction against the payment method.
        /// </summary>
        /// <param name="amount">The amount of the transaction</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder Reverse(decimal? amount = null) {
            return new AuthorizationBuilder(TransactionType.Reversal, this).WithAmount(amount);
        }

        /// <summary>
        /// Verifies the payment method with the issuer.
        /// </summary>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder Verify() {
            return new AuthorizationBuilder(TransactionType.Verify, this);
        }

        /// <summary>
        /// Tokenizes the payment method, verifying the payment method
        /// with the issuer in the process.
        /// </summary>
        /// <returns>AuthorizationBuilder</returns>
        public string Tokenize() {
            var response =  new AuthorizationBuilder(TransactionType.Verify, this).WithRequestMultiUseToken(true).Execute();
            return response.Token;
        }
    }

    /// <summary>
    /// Use credit tokens or manual entry data as a payment method.
    /// </summary>
    public class CreditCardData : Credit, ICardData {
        private static readonly Regex AmexRegex = new Regex(@"^3[47][0-9]{13}$", RegexOptions.None);
        private static readonly Regex MasterCardRegex = new Regex(@"^5[1-5][0-9]{14}$", RegexOptions.None);
        private static readonly Regex VisaRegex = new Regex(@"^4[0-9]{12}(?:[0-9]{3})?$", RegexOptions.None);
        private static readonly Regex DinersClubRegex = new Regex(@"^3(?:0[0-5]|[68][0-9])[0-9]{11}$", RegexOptions.None);
        private static readonly Regex RouteClubRegex = new Regex(@"^(2014|2149)", RegexOptions.None);
        private static readonly Regex DiscoverRegex = new Regex(@"^6(?:011|5[0-9]{2})[0-9]{12}$", RegexOptions.None);
        private static readonly Regex JcbRegex = new Regex(@"^(?:2131|1800|35\d{3})\d{11}$", RegexOptions.None);

        private Dictionary<string, Regex> regexHash;
        private Dictionary<string, Regex> RegexHash {
            get {
                if (regexHash == null) {
                    regexHash = new Dictionary<string, Regex>();
                    regexHash.Add("Amex", AmexRegex);
                    regexHash.Add("MC", MasterCardRegex);
                    regexHash.Add("Visa", VisaRegex);
                    regexHash.Add("DinersClub", DinersClubRegex);
                    regexHash.Add("EnRoute", RouteClubRegex);
                    regexHash.Add("Discover", DiscoverRegex);
                    regexHash.Add("Jcb", JcbRegex);
                }

                return regexHash;
            }

            set {
                regexHash = value;
            }
        }

        private string cvn;

        /// <summary>
        /// The card type of the manual entry data.
        /// </summary>
        /// <remarks>
        /// Default value is `"Unknown"`.
        /// </remarks>
        public string CardType {
            get {
                string cardType = "Unknown";

                try {
                    string cardNum = Number.Replace(" ", string.Empty).Replace("-", string.Empty);
                    foreach (string cardTypeName in RegexHash.Keys) {
                        if (RegexHash[cardTypeName].IsMatch(cardNum)) {
                            cardType = cardTypeName;
                            break;
                        }
                    }
                }
                catch (Exception) { /* NOM NOM */ }

                return cardType;
            }
        }

        /// <summary>
        /// Indicates if the card is present with the merchant at time of payment.
        /// </summary>
        /// <remarks>
        /// Default value is `false`.
        /// </remarks>
        public bool CardPresent { get; set; }

        /// <summary>
        /// The card's card verification number (CVN).
        /// </summary>
        /// <remarks>
        /// When set, `CreditCardData.CvnPresenceIndicator` is set to
        /// `CvnPresenceIndicator.Present`.
        /// </remarks>
        public string Cvn {
            get { return cvn; }
            set {
                if (!string.IsNullOrEmpty(value)) {
                    cvn = value;
                    CvnPresenceIndicator = CvnPresenceIndicator.Present;
                }
            }
        }

        /// <summary>
        /// The name on the front of the card.
        /// </summary>
        public string CardHolderName { get; set; }

        /// <summary>
        /// Indicates card verification number (CVN) presence.
        /// </summary>
        /// <remarks>
        /// Default value is `CvnPresenceIndicator.NotRequested`.
        /// </remarks>
        public CvnPresenceIndicator CvnPresenceIndicator { get; set; }

        /// <summary>
        /// The card's number.
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// The card's expiration month.
        /// </summary>
        public int? ExpMonth { get; set; }

        /// <summary>
        /// The card's expiration year.
        /// </summary>
        public int? ExpYear { get; set; }

        internal string ShortExpiry {
            get {
                var month = (ExpMonth.HasValue) ? ExpMonth.ToString().PadLeft(2, '0') : string.Empty;
                var year = (ExpYear.HasValue) ? ExpYear.ToString().PadLeft(4, '0').Substring(2, 2) : string.Empty;
                return month + year;
            }
        }

        /// <summary>
        /// Indicates if a card reader was used when accepting the card data.
        /// </summary>
        /// <remarks>
        /// Default value is `false`.
        /// </remarks>
        public bool ReaderPresent { get; set; }

        public CreditCardData() {
            CardPresent = false;
            ReaderPresent = false;
            CvnPresenceIndicator = CvnPresenceIndicator.NotRequested;
        }
    }

    /// <summary>
    /// Use credit track data as a payment method.
    /// </summary>
    public class CreditTrackData : Credit, ITrackData {
        /// <summary>
        /// Indicates how the card's track data was obtained.
        /// </summary>
        public EntryMethod EntryMethod { get; set; }

        /// <summary>
        /// The card's track data.
        /// </summary>
        public string Value { get; set; }
    }
}
