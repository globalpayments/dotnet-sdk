using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Entities.Billing {
    public class TokenResponse: BillingResponse {
        public string Token { get; set; }
    }
}
