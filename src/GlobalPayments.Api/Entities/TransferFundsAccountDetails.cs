using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Entities
{
    public class TransferFundsAccountDetails {
        public string Id { get; set; }
        public string Status { get; set; }
        public string TimeCreated { get; set; }
        public decimal? Amount { get; set; }
        public string Reference { get; set; }
        public string Description { get; set; }

    }
}
