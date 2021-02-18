using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Entities.Billing {
    internal class Credentials {
        public string ApiKey { get; set; }
        public string MerchantName { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
    }
}
