using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;

namespace GlobalPayments.Api.PaymentMethods {
    public abstract class Credit : IPaymentMethod, IEncryptable, ITokenizable, IChargable, IAuthable, IRefundable, IReversable, IVerifiable, IPrePayable, IBalanceable {
        public EncryptionData EncryptionData { get; set; }
        public PaymentMethodType PaymentMethodType { get { return PaymentMethodType.Credit; } }
        public string Token { get; set; }

        public AuthorizationBuilder Authorize(decimal? amount = null) {
            return new AuthorizationBuilder(TransactionType.Auth, this).WithAmount(amount);
        }

        public AuthorizationBuilder Charge(decimal? amount = null) {
            return new AuthorizationBuilder(TransactionType.Sale, this).WithAmount(amount);
        }

        public AuthorizationBuilder AddValue(decimal? amount = null) {
            return new AuthorizationBuilder(TransactionType.AddValue, this).WithAmount(amount);
        }

        public AuthorizationBuilder BalanceInquiry(InquiryType? inquiry = null) {
            return new AuthorizationBuilder(TransactionType.Balance, this).WithBalanceInquiryType(inquiry);
        }

        public AuthorizationBuilder Refund(decimal? amount = null) {
            return new AuthorizationBuilder(TransactionType.Refund, this).WithAmount(amount);
        }

        public AuthorizationBuilder Reverse(decimal? amount = null) {
            return new AuthorizationBuilder(TransactionType.Reversal, this).WithAmount(amount);
        }

        public AuthorizationBuilder Verify() {
            return new AuthorizationBuilder(TransactionType.Verify, this);
        }

        public string Tokenize() {
            var response =  new AuthorizationBuilder(TransactionType.Verify, this).WithRequestMultiUseToken(true).Execute();
            return response.Token;
        }
    }

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
        public bool CardPresent { get; set; }
        public string Cvn {
            get { return cvn; }
            set {
                if (!string.IsNullOrEmpty(value)) {
                    cvn = value;
                    CvnPresenceIndicator = CvnPresenceIndicator.Present;
                }
            }
        }
        public string CardHolderName { get; set; }
        public CvnPresenceIndicator CvnPresenceIndicator { get; set; }
        public string Number { get; set; }
        public int? ExpMonth { get; set; }
        public int? ExpYear { get; set; }
        internal string ShortExpiry {
            get {
                var month = (ExpMonth.HasValue) ? ExpMonth.ToString().PadLeft(2, '0') : string.Empty;
                var year = (ExpYear.HasValue) ? ExpYear.ToString().PadLeft(4, '0').Substring(2, 2) : string.Empty;
                return month + year;
            }
        }
        public bool ReaderPresent { get; set; }

        public CreditCardData() {
            CardPresent = false;
            ReaderPresent = false;
            CvnPresenceIndicator = CvnPresenceIndicator.NotRequested;
        }
    }

    public class CreditTrackData : Credit, ITrackData {
        public EntryMethod EntryMethod { get; set; }
        public string Value { get; set; }
    }
}
