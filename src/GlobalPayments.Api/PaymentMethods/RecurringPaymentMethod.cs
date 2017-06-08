using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Services;

namespace GlobalPayments.Api.PaymentMethods
{
    public class RecurringPaymentMethod : RecurringEntity<RecurringPaymentMethod>, IPaymentMethod, IChargable, IAuthable, IVerifiable, IRefundable {
        public Address Address { get; set; }
        public string CommercialIndicator { get; internal set; }
        public string CustomerKey { get; set; }
        public string ExpirationDate { get; set; }
        public string NameOnAccount { get; set; }
        private IPaymentMethod paymentMethod;
        public IPaymentMethod PaymentMethod {
            get { return paymentMethod; }
            set {
                var client = ServicesContainer.Instance.GetRecurringClient();
                if (client.SupportsUpdatePaymentDetails)
                    paymentMethod = value;
                else throw new UnsupportedTransactionException();
            }
        }
        public PaymentMethodType PaymentMethodType { get { return PaymentMethodType.Recurring; } }
        public string PaymentType { get; internal set; }
        public bool PreferredPayment { get; set; }
        public string Status { get; set; }
        public string TaxType { get; set; }

        public RecurringPaymentMethod() : this(null, null) { }
        internal RecurringPaymentMethod(IPaymentMethod paymentMethod) {
            this.paymentMethod = paymentMethod;
        }
        public RecurringPaymentMethod(string customerId, string paymentId) {
            CustomerKey = customerId;
            Key = paymentId;
            PaymentType = "Credit Card"; // set default
        }

        public AuthorizationBuilder Authorize(decimal? amount = null) {
            return new AuthorizationBuilder(TransactionType.Auth, this).WithAmount(amount).WithOneTimePayment(true);
        }

        public AuthorizationBuilder Charge(decimal? amount = null) {
            return new AuthorizationBuilder(TransactionType.Sale, this).WithAmount(amount).WithOneTimePayment(true);
        }

        public AuthorizationBuilder Refund(decimal? amount = null) {
            return new AuthorizationBuilder(TransactionType.Refund, this).WithAmount(amount);
        }

        public AuthorizationBuilder Verify() {
            return new AuthorizationBuilder(TransactionType.Verify, this);
        }

        public Schedule AddSchedule(string sheduleId) {
            return new Schedule(CustomerKey, Key) {
                Id = sheduleId
            };
        }
    }
}
