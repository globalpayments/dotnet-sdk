using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Entities
{
    public class FundsAccountDetails {
        public string Id { get; set; }
        public string Status { get; set; }
        public string TimeCreated { get; set; }
        public string TimeLastUpdated { get; set; }
        public decimal? Amount { get; set; }
        public string Reference { get; set; }
        public string Description { get; set; }
        public string Currency { get; set; }
        public string PaymentMethodType { get; set; }
        public string PaymentMethodName { get; set; }
        public UserAccount Account { get; set; }
    }
}
