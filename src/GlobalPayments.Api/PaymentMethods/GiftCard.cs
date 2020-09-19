using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.PaymentMethods {
    /// <summary>
    /// Use gift/loyaly/stored value account as a payment method.
    /// </summary>
    public class GiftCard : IPaymentMethod, IPrePaid, IBalanceable, IReversable, IChargable, IAuthable, IRefundable {
        private string _token;
        private string _trackData;
        private string _number;
        private string _alias;

        /// <summary>
        /// The gift card's alias.
        /// </summary>
        public string Alias {
            get {
                return _alias;
            }
            set {
                _alias = value;
                ValueType = "Alias";
            }
        }

        /// <summary>
        /// The gift card's card number.
        /// </summary>
        public string Number {
            get {
                return _number;
            }
            set {
                if (string.IsNullOrEmpty(Value)) {
                    SetValue(value);
                }
                else {
                    _number = value;
                    ValueType = "CardNbr";
                }
            }
        }

        /// <summary>
        /// Set to `PaymentMethodType.Gift` for internal methods.
        /// </summary>
        public PaymentMethodType PaymentMethodType { get { return PaymentMethodType.Gift; } }

        /// <summary>
        /// The gift card's PIN.
        /// </summary>
        public string Pin { get; set; }

        /// <summary>
        /// The token representing the gift card.
        /// </summary>
        public string Token {
            get {
                return _token;
            }
            set {
                _token = value;
                ValueType = "TokenValue";
            }
        }

        /// <summary>
        /// The gift card's track data
        /// </summary>
        public string TrackData {
            get {
                return _trackData;
            }
            set {
                if (string.IsNullOrEmpty(Value)) {
                    SetValue(value);
                }
                else {
                    _trackData = value;
                    ValueType = "TrackData";
                }                
            }
        }

        internal string Value { get; set; }
        internal string ValueType { get; private set; }
        internal TrackNumber TrackNumber { get; set; }
        internal string Pan { get; set; }
        internal string Expiry { get; set; }
        internal string CardType { get; set; }

        /// <summary>
        /// Activates an existing gift card.
        /// </summary>
        /// <param name="amount">The amount of the transaction</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder Activate(decimal? amount = null) {
            return new AuthorizationBuilder(TransactionType.Activate, this).WithAmount(amount);
        }

        /// <summary>
        /// Adds an alias to to an existing gift card.
        /// </summary>
        /// <param name="phoneNumber">The phone number to add as an alias</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder AddAlias(string phoneNumber) {
            return new AuthorizationBuilder(TransactionType.Alias, this).WithAlias(AliasAction.ADD, phoneNumber);
        }

        /// <summary>
        /// Adds value to to an activated gift card.
        /// </summary>
        /// <param name="amount">The amount of the transaction</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder AddValue(decimal? amount = null) {
            return new AuthorizationBuilder(TransactionType.AddValue, this).WithAmount(amount);
        }

        public AuthorizationBuilder Authorize(decimal? amount = null, bool isEstimate = false)
        {
            return new AuthorizationBuilder(TransactionType.Auth, this)
                    .WithAmount(amount)
                    .WithAmountEstimated(isEstimate);
        }

        /// <summary>
        /// Completes a balance inquiry (lookup) on an activated gift card.
        /// </summary>
        /// <param name="inquiry">The type of inquiry to make</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder BalanceInquiry(InquiryType? inquiry = null) {
            return new AuthorizationBuilder(TransactionType.Balance, this);
        }

        public AuthorizationBuilder CashOut()
        {
            return new AuthorizationBuilder(TransactionType.CashOut, this);
        }

        /// <summary>
        /// Creates a charge (sale) transaction against an activated gift card.
        /// </summary>
        /// <param name="amount">The amount of the transaction</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder Charge(decimal? amount = null) {
            return new AuthorizationBuilder(TransactionType.Sale, this).WithAmount(amount);
        }

        /// <summary>
        /// Creates a gift card with an alias.
        /// </summary>
        /// <exception cref="GatewayException">
        /// Thrown when the gift card cannot be created.
        /// </exception>
        /// <param name="phoneNumber">The phone number to be used as the alias</param>
        /// <returns>GiftCard</returns>
        public static GiftCard Create(string phoneNumber)
        {
            var card = new GiftCard { };

            var response = new AuthorizationBuilder(TransactionType.Alias, card)
                .WithAlias(AliasAction.CREATE, phoneNumber)
                .Execute();

            // if success return a card
            if (response.ResponseCode == "00")
            {
                return response.GiftCard;
            }
            else throw new GatewayException("Failed to create gift card.", response.ResponseCode, response.ResponseMessage);
        }

        /// <summary>
        /// Deactivates a gift card.
        /// </summary>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder Deactivate() {
            return new AuthorizationBuilder(TransactionType.Deactivate, this);
        }

        public AuthorizationBuilder Refund(decimal? amount = null) {
            return new AuthorizationBuilder(TransactionType.Refund, this)
                    .WithAmount(amount);
        }

        /// <summary>
        /// Removes an alias from an existing gift card.
        /// </summary>
        /// <param name="phoneNumber">The phone number alias to remove</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder RemoveAlias(string phoneNumber) {
            return new AuthorizationBuilder(TransactionType.Alias, this).WithAlias(AliasAction.DELETE, phoneNumber);
        }

        /// <summary>
        /// Replaces an existing gift card with a new one,
        /// transferring the balance from the old card to
        /// the new card in the process.
        /// </summary>
        /// <param name="newCard">The replacement gift card</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder ReplaceWith(GiftCard newCard) {
            return new AuthorizationBuilder(TransactionType.Replace, this).WithReplacementCard(newCard);
        }

        /// <summary>
        /// Reverses a previous charge against an activated gift card.
        /// </summary>
        /// <param name="amount">The amount of the transaction</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder Reverse(decimal? amount = null) {
            return new AuthorizationBuilder(TransactionType.Reversal, this).WithAmount(amount);
        }

        /// <summary>
        /// Adds rewards points to an activated gift card.
        /// </summary>
        /// <param name="amount">The amount of the transaction</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder Rewards(decimal? amount = null) {
            return new AuthorizationBuilder(TransactionType.Reward, this).WithAmount(amount);
        }

        public void SetValue(string value) {
            Value = value;

            CardUtils.ParseTrackData(this);
            if (string.IsNullOrEmpty(TrackData)) {
                Number = value;
                Pan = value;
            }
            CardType = CardUtils.MapCardType(Pan);
        }
    }
}
