using System;
using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;

namespace GlobalPayments.Api.PaymentMethods
{
    public class GiftCard : IPaymentMethod, IPrePayable, IBalanceable, IReversable, IChargable {
        public string Alias {
            get {
                return Value;
            }
            set {
                Value = value;
                ValueType = "Alias";
            }
        }
        public string Number {
            get {
                return Value;
            }
            set {
                Value = value;
                ValueType = "CardNbr";
            }
        }
        public PaymentMethodType PaymentMethodType { get { return PaymentMethodType.Gift; } }
        public string Pin { get; set; }
        public string Token {
            get {
                return Value;
            }
            set {
                Value = value;
                ValueType = "TokenValue";
            }
        }
        public string TrackData {
            get {
                return Value;
            }
            set {
                Value = value;
                ValueType = "TrackData";
            }
        }
        internal string Value { get; set; }
        internal string ValueType { get; private set; }
        
        public AuthorizationBuilder AddAlias(string phoneNumber) {
            return new AuthorizationBuilder(TransactionType.Alias, this).WithAlias(AliasAction.ADD, phoneNumber);
        }

        public AuthorizationBuilder Activate(decimal? amount = null) {
            return new AuthorizationBuilder(TransactionType.Activate, this).WithAmount(amount);
        }

        public AuthorizationBuilder AddValue(decimal? amount = null) {
            return new AuthorizationBuilder(TransactionType.AddValue, this).WithAmount(amount);
        }

        public AuthorizationBuilder BalanceInquiry(InquiryType? inquiry = null) {
            return new AuthorizationBuilder(TransactionType.Balance, this);
        }

        public AuthorizationBuilder Charge(decimal? amount = null) {
            return new AuthorizationBuilder(TransactionType.Sale, this).WithAmount(amount);
        }

        public AuthorizationBuilder Deactivate() {
            return new AuthorizationBuilder(TransactionType.Deactivate, this);
        }

        public AuthorizationBuilder RemoveAlias(string phoneNumber) {
            return new AuthorizationBuilder(TransactionType.Alias, this).WithAlias(AliasAction.DELETE, phoneNumber);
        }

        public AuthorizationBuilder ReplaceWith(GiftCard newCard) {
            return new AuthorizationBuilder(TransactionType.Replace, this).WithReplacementCard(newCard);
        }

        public AuthorizationBuilder Reverse(decimal? amount = null) {
            return new AuthorizationBuilder(TransactionType.Reversal, this).WithAmount(amount);
        }

        public AuthorizationBuilder Rewards(decimal? amount = null) {
            return new AuthorizationBuilder(TransactionType.Reward, this).WithAmount(amount);
        }

        public static GiftCard Create(string phoneNumber) {
            var card = new GiftCard { };

            var response = new AuthorizationBuilder(TransactionType.Alias, card)
                .WithAlias(AliasAction.CREATE, phoneNumber)
                .Execute();

            // if success return a card
            if (response.ResponseCode == "00") {
                return response.GiftCard;
            }
            else throw new GatewayException("Failed to create gift card.", response.ResponseCode, response.ResponseMessage);
        }
    }
}
