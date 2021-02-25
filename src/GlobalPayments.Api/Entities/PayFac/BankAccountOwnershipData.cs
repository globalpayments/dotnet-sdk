using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Entities.PayFac
{
    public class BankAccountOwnershipData {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Address OwnerAddress { get; set; }
        public string PhoneNumber { get; set; }

        public BankAccountOwnershipData() {
            OwnerAddress = new Address();
        }
    }
}
