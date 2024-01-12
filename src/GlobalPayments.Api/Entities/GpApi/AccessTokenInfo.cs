using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Entities
{
    public class AccessTokenInfo
    {
        public string Token { get; set; }
        public string DataAccountName { get; set; }
        public string DisputeManagementAccountName { get; set; }
        public string TokenizationAccountName { get; set; }
        public string TransactionProcessingAccountName { get; set; }
        public string RiskAssessmentAccountName { get; set; }
        public string MerchantManagementAccountName { get; set; }
        public string FileProcessingAccountName { get; set; }
        public string DataAccountID { get; set; }
        public string DisputeManagementAccountID { get; set; }
        public string TokenizationAccountID { get; set; }
        public string TransactionProcessingAccountID { get; set; }
        public string RiskAssessmentAccountID { get; set; }
        public string MerchantManagementAccountID { get; set; }
        public string FileProcessingAccountID { get; set; }
    }
}
