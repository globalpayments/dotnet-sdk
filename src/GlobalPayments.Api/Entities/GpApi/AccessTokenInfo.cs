using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Entities {
    public class AccessTokenInfo {
        public string Token { get; set; }
        public string DataAccountName { get; set; }
        public string DisputeManagementAccountName { get; set; }
        public string TokenizationAccountName { get; set; }
        public string TransactionProcessingAccountName { get; set; }
    }
}
