using System.Collections.Generic;
using GlobalPayments.Api.PaymentMethods;

namespace GlobalPayments.Api.Entities {
    /// <summary>
    /// A customer resource.
    /// </summary>
    /// <remarks>
    /// Mostly used in recurring scenarios.
    /// </remarks>
    public class Customer : RecurringEntity<Customer> {
        /// <summary>
        /// Customer's title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Customer's first name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Customer's last name
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Customer's middle name
        /// </summary>
        public string MiddleName { get; set; }

        /// <summary>
        /// Customer's buiness name
        /// </summary>
        public string BusinessName { get; set; }

        /// <summary>
        /// Customer's company
        /// </summary>
        public string Company { get; set; }

        public string CustomerPassword { get; set; }

        public string DateOfBirth { get; set; }

        public string DomainName { get; set; }

        public string DeviceFingerPrint { get; set; }

        /// <summary>
        /// Customer's address
        /// </summary>
        public Address Address { get; set; }

        /// <summary>
        /// Customer's home phone number
        /// </summary>
        public string HomePhone { get; set; }

        /// <summary>
        /// Customer's work phone number
        /// </summary>
        public string WorkPhone { get; set; }

        /// <summary>
        /// Customer's fax phone number
        /// </summary>
        public string Fax { get; set; }

        /// <summary>
        /// Customer's mobile phone number
        /// </summary>
        public string MobilePhone { get; set; }

        /// <summary>
        /// Customer's email address
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Customer comments
        /// </summary>
        public string Comments { get; set; }

        /// <summary>
        /// Customer's department within its organization
        /// </summary>
        public string Department { get; set; }

        /// <summary>
        /// Customer resource's status
        /// </summary>
        public string Status { get; set; }
        
        public PhoneNumber Phone { get; set; }
   
        public List<CustomerDocument> Documents { get; set; }

        /// <summary>
        /// Customer's existing payment methods
        /// </summary>
        public List<RecurringPaymentMethod> PaymentMethods { get; set; }

        public RecurringPaymentMethod AddPaymentMethod(string paymentId, IPaymentMethod paymentMethod)
        {
            return AddPaymentMethod(paymentId, paymentMethod, null);
        }

        /// <summary>
        /// Adds a payment method to the customer
        /// </summary>
        /// <param name="paymentId">
        /// An application derived ID for the payment method
        /// </param>
        /// <param name="paymentMethod">
        /// The payment method
        /// </param>
        /// <returns>RecurringPaymentMethod</returns>
        public RecurringPaymentMethod AddPaymentMethod(string paymentId, IPaymentMethod paymentMethod, StoredCredential storedCredential) {
            var nameOnAccount = string.Format("{0} {1}", FirstName, LastName);
            if (string.IsNullOrWhiteSpace(nameOnAccount))
                nameOnAccount = Company;

            if(PaymentMethods == null) {
                PaymentMethods = new List<RecurringPaymentMethod>();
            }
            PaymentMethods.Add(new RecurringPaymentMethod() { 
                Id = paymentId,
                PaymentMethod = paymentMethod});

            return new RecurringPaymentMethod(paymentMethod) {
                Address = Address,
                CustomerKey = Key,
                Id = paymentId,
                NameOnAccount = nameOnAccount,
                StoredCredential = storedCredential,               
            };
        }
    }
}
