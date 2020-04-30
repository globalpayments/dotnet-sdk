using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Services;

namespace GlobalPayments.Api.PaymentMethods
{
    /// <summary>
    /// Use credit or eCheck/ACH as a recurring payment method.
    /// </summary>
    public class RecurringPaymentMethod : RecurringEntity<RecurringPaymentMethod>, IPaymentMethod, IChargable, IAuthable, IVerifiable, IRefundable, ISecure3d {
        /// <summary>
        /// The address associated with the payment method account.
        /// </summary>
        public Address Address { get; set; }

        /// <summary>
        /// The payment method's commercial indicator (Level II/III).
        /// </summary>
        public string CommercialIndicator { get; internal set; }

        /// <summary>
        /// The identifier of the payment method's customer.
        /// </summary>
        public string CustomerKey { get; set; }

        /// <summary>
        /// The payment method's expiration date.
        /// </summary>
        public string ExpirationDate { get; set; }

        /// <summary>
        /// The name on the payment method account.
        /// </summary>
        public string NameOnAccount { get; set; }

        /// <summary>
        /// The underlying payment method.
        /// </summary>
        public IPaymentMethod PaymentMethod { get; set; }

        /// <summary>
        /// Set to `PaymentMethodType.Recurring` for internal methods.
        /// </summary>
        public PaymentMethodType PaymentMethodType { get { return PaymentMethodType.Recurring; } }

        /// <summary>
        /// The payment method type, Credit Card vs ACH.
        /// </summary>
        public string PaymentType { get; internal set; }

        /// <summary>
        /// Indicates if the payment method is the default/preferred
        /// method for the customer.
        /// </summary>
        public bool PreferredPayment { get; set; }

        /// <summary>
        /// The payment method status
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// The payment method's tax type
        /// </summary>
        public string TaxType { get; set; }

        public ThreeDSecure ThreeDSecure { get; set; }

        public RecurringPaymentMethod() : this(null, null) { }
        internal RecurringPaymentMethod(IPaymentMethod paymentMethod) {
            PaymentMethod = paymentMethod;
        }
        public RecurringPaymentMethod(string customerId, string paymentId) {
            CustomerKey = customerId;
            Key = paymentId;
            PaymentType = "Credit Card"; // set default
        }

        /// <summary>
        /// Creates an authorization against the payment method.
        /// </summary>
        /// <param name="amount">The amount of the transaction</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder Authorize(decimal? amount = null, bool isEstimated = false) {
            return new AuthorizationBuilder(TransactionType.Auth, this)
                .WithAmount(amount)
                .WithOneTimePayment(true)
                .WithAmountEstimated(isEstimated);
        }

        /// <summary>
        /// Creates a charge (sale) against the payment method.
        /// </summary>
        /// <param name="amount">The amount of the transaction</param>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder Charge(decimal? amount = null) {
            return new AuthorizationBuilder(TransactionType.Sale, this).WithAmount(amount).WithOneTimePayment(true);
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
        /// Verifies the payment method with the issuer.
        /// </summary>
        /// <returns>AuthorizationBuilder</returns>
        public AuthorizationBuilder Verify() {
            return new AuthorizationBuilder(TransactionType.Verify, this);
        }

        /// <summary>
        /// Creates a recurring schedule using the payment method.
        /// </summary>
        /// <param name="schedule">The schedule's identifier</param>
        /// <returns>Schedule</returns>
        public Schedule AddSchedule(string scheduleId) {
            return new Schedule(CustomerKey, Key) {
                Id = scheduleId
            };
        }
    }
}
