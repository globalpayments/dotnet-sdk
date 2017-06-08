using GlobalPayments.Api.PaymentMethods;

namespace GlobalPayments.Api.Entities {
    public class Customer : RecurringEntity<Customer> {
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public Address Address { get; set; }
        public string HomePhone { get; set; }
        public string WorkPhone { get; set; }
        public string Fax { get; set; }
        public string MobilePhone { get; set; }
        public string Email { get; set; }
        public string Comments { get; set; }
        public string Department { get; set; }
        public string Status { get; set; }

        public RecurringPaymentMethod AddPaymentMethod(string paymentId, IPaymentMethod paymentMethod) {
            var nameOnAccount = string.Format("{0} {1}", FirstName, LastName);
            if (string.IsNullOrWhiteSpace(nameOnAccount))
                nameOnAccount = Company;

            return new RecurringPaymentMethod(paymentMethod) {
                Address = Address,
                CustomerKey = Key,
                Id = paymentId,
                NameOnAccount = nameOnAccount
            };
        }
    }
}
