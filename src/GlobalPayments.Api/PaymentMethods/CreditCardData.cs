using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Utils;
using System;

namespace GlobalPayments.Api.PaymentMethods {
    /// <summary>
    /// Use credit tokens or manual entry data as a payment method.
    /// </summary>
    public class CreditCardData : Credit, ICardData {
        private string cvn;

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
        private string _number;
        public string Number {
            get { return _number; }
            set {
                _number = value;
                try {
                    CardType = CardUtils.MapCardType(_number);
                    FleetCard = CardUtils.IsFleet(CardType, _number);
                    PurchaseCard = CardUtils.IsPurchase(CardType, _number);
                    ReadyLinkCard = CardUtils.IsReadyLink(CardType, _number);
                }
                catch (Exception) {
                    CardType = "Unknown";
                }
            }
        }

       public ManualEntryMethod? EntryMethod { get; set; }

        public EntryMethod? OriginalEntryMethod { get; set; }

        /// <summary>
        /// The card's expiration month.
        /// </summary>
        public int? ExpMonth { get; set; }

        internal int? _expYear;

        /// <summary>
        /// The card's expiration year.
        /// </summary>
        public int? ExpYear {
            get { return _expYear; }
            set {
                if (value.HasValue && (int)Math.Floor(Math.Log10(value.Value)) + 1 == 2) {
                    _expYear = value + 2000;
                } else {
                    _expYear = value;
                }
            }
        }

        public string ShortExpiry {
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
        public string Cardutils { get; private set; }

        public CreditCardData(string token = null) : base() {
            Token = token;
            CardPresent = false;
            ReaderPresent = false;
            CvnPresenceIndicator = CvnPresenceIndicator.NotRequested;
        }

        public AuthorizationBuilder GetDccRate(DccRateType dccRateType = DccRateType.None, DccProcessor dccProcessor = DccProcessor.None) {
            DccRateData dccRateData = new DccRateData {
                DccRateType = dccRateType,
                DccProcessor = dccProcessor
            };

            return new AuthorizationBuilder(TransactionType.DccRateLookup, this).WithDccRateData(dccRateData);
        }
        
        public bool HasInAppPaymentData()
        {
            return (!string.IsNullOrEmpty(this.Token) && !string.IsNullOrEmpty(this.MobileType));
        }
    }
}
