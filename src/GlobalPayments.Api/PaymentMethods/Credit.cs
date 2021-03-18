using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;

namespace GlobalPayments.Api.PaymentMethods {
    /// <summary>
    /// Use credit as a payment method.
    /// </summary>
    public abstract class Credit : IPaymentMethod, IEncryptable, ITokenizable, IChargable, IAuthable, IRefundable, IReversable, IVerifiable, IPrePaid, IBalanceable, ISecure3d {
        /// <summary>
        /// The name of the issuing Bank
        /// </summary>
        public string BankName { get; set; }

        /// <summary>
        /// The card type of the manual entry data.
        /// </summary>
        /// <remarks>
        /// Default value is `"Unknown"`.
        /// </remarks>
        public string CardType { get; set; }

        /// <summary>
        /// The card's encryption data; where applicable.
        /// </summary>
        public EncryptionData EncryptionData { get; set; }

        /// <summary>
        /// Set to `PaymentMethodType.Credit` for internal methods.
        /// </summary>
        public PaymentMethodType PaymentMethodType { get { return PaymentMethodType.Credit; } }

        /// <summary>
        /// Secure 3d Data attached to the card
        /// </summary>
        public ThreeDSecure ThreeDSecure { get; set; }

        /// <summary>
        /// A token value representing the card.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// A MobileType value representing the Google/Apple.
        /// </summary>
        public string MobileType { get; set; }

        public bool FleetCard { get; set; }

        public bool PurchaseCard { get; set; }

        public Credit() {
            CardType = "Unknown";
        }

        /// <summary>
        /// Creates an authorization against the payment method.
        /// </summary>
        /// <param name="amount">The amount of the transaction</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder Authorize(decimal? amount = null, bool isEstimated = false) {
            return new AuthorizationBuilder(TransactionType.Auth, this)
                .WithAmount(amount ?? ThreeDSecure?.Amount)
                .WithCurrency(ThreeDSecure?.Currency)
                .WithOrderId(ThreeDSecure?.OrderId)
                .WithAmountEstimated(isEstimated);
        }

        /// <summary>
        /// Creates a charge (sale) against the payment method.
        /// </summary>
        /// <param name="amount">The amount of the transaction</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder Charge(decimal? amount = null) {
           return new AuthorizationBuilder(TransactionType.Sale, this)
                .WithAmount(amount ?? ThreeDSecure?.Amount)
                .WithCurrency(ThreeDSecure?.Currency)
                .WithOrderId(ThreeDSecure?.OrderId);
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
        public string Tokenize(string configName = "default") {
            return Tokenize(true, configName);
        }
        public string Tokenize(bool verifyCard, string configName = "default") {
            TransactionType type = verifyCard ? TransactionType.Verify : TransactionType.Tokenize;

            var response =  new AuthorizationBuilder(type, this)
                .WithRequestMultiUseToken(verifyCard)
                .WithTokenUsageMode(TokenUsageMode.Multiple)
                .Execute(configName);
            return response.Token;
        }

        /// <summary>
        /// Detokenizes payment method
        /// </summary>
        /// <param name="configName"></param>
        /// <returns></returns>
        ITokenizable ITokenizable.Detokenize(string configName) {
            if (string.IsNullOrEmpty(Token)) {
                throw new BuilderException("Token cannot be null");
            }

            var transaction = new ManagementBuilder(TransactionType.Detokenize)
                .WithPaymentMethod(this)
                .Execute(configName);

            CardType = transaction.CardType;

            return this;
        }

        /// <summary>
        /// Updates the token expiry date with the values proced to the card object
        /// </summary>
        /// <returns>boolean value indcating success/failure</returns>
        public bool UpdateTokenExpiry(string configName = "default") {
            if (string.IsNullOrEmpty(Token)) {
                throw new BuilderException("Token cannot be null");
            }

            try {
                new ManagementBuilder(TransactionType.TokenUpdate)
                    .WithPaymentMethod(this)
                    .Execute(configName);
                return true;
            }
            catch (ApiException) {
                return false;
            }
        }

        /// <summary>
        /// Deletes the token associated with the current card object
        /// </summary>
        /// <returns>boolean value indicating success/failure</returns>
        public bool DeleteToken(string configName = "default") {
            if (string.IsNullOrEmpty(Token)) {
                throw new BuilderException("Token cannot be null");
            }

            try {
                new ManagementBuilder(TransactionType.TokenDelete)
                    .WithPaymentMethod(this)
                    .Execute(configName);
                return true;
            }
            catch (ApiException) {
                return false;
            }
        }
    }
}
