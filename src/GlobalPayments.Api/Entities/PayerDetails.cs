using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Entities
{
    public class PayerDetails
    {       
        public string FirstName { get; set; }        
        public string LastName { get; set; }        
        public string Email { get; set; }       
        public Address BillingAddress{ get; set;}    
        public Address ShippingAddress { get; set; }
    }
}
