using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Entities.Billing {
    public class LoadSecurePayResponse: BillingResponse {
        public string PaymentIdentifier { get; set; }
    }
}
