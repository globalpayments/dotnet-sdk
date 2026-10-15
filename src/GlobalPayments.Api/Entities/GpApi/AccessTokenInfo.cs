using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Entities {
    /// <summary>
    /// Contains the access token and scope information returned by GP-API.
    /// </summary>
    public class AccessTokenInfo {
        /// <summary>
        /// The bearer access token returned by GP-API.
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// The type of access token returned by GP-API, typically "Bearer".
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// The identifier of the application that created the access token.
        /// </summary>
        public string AppId { get; set; }
        /// <summary>
        /// The name of the application that created the access token.
        /// </summary>
        public string AppName { get; set; }
        /// <summary>
        /// The UTC timestamp when the access token was created.
        /// </summary>
        public DateTime TimeCreated { get; set; }
        /// <summary>
        /// The access token lifetime in seconds.
        /// </summary>
        public int SecondsToExpire { get; set; }
        /// <summary>
        /// The configured access token expiration interval, when returned by GP-API.
        /// </summary>
        public string IntervalToExpire { get; set; }
        /// <summary>
        /// The email address associated with the application that created the access token.
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// The identifier of the merchant in the access token scope.
        /// </summary>
        public string MerchantId { get; set; }
        /// <summary>
        /// The name of the merchant in the access token scope.
        /// </summary>
        public string MerchantName { get; set; }
        /// <summary>
        /// The accounts and permissions included in the access token scope.
        /// </summary>
        public GpApiAccount[] Accounts { get; set; }
        /// <summary>
        /// The name of the data account in the access token scope.
        /// </summary>
        public string DataAccountName { get; set; }
        /// <summary>
        /// The name of the dispute management account in the access token scope.
        /// </summary>
        public string DisputeManagementAccountName { get; set; }
        /// <summary>
        /// The name of the tokenization account in the access token scope.
        /// </summary>
        public string TokenizationAccountName { get; set; }
        /// <summary>
        /// The name of the transaction processing account in the access token scope.
        /// </summary>
        public string TransactionProcessingAccountName { get; set; }
        /// <summary>
        /// The name of the risk assessment account in the access token scope.
        /// </summary>
        public string RiskAssessmentAccountName { get; set; }
        /// <summary>
        /// The name of the merchant management account in the access token scope.
        /// </summary>
        public string MerchantManagementAccountName { get; set; }
        /// <summary>
        /// The name of the file processing account in the access token scope.
        /// </summary>
        public string FileProcessingAccountName { get; set; }
        /// <summary>
        /// The identifier of the data account in the access token scope.
        /// </summary>
        public string DataAccountID { get; set; }
        /// <summary>
        /// The identifier of the dispute management account in the access token scope.
        /// </summary>
        public string DisputeManagementAccountID { get; set; }
        /// <summary>
        /// The identifier of the tokenization account in the access token scope.
        /// </summary>
        public string TokenizationAccountID { get; set; }
        /// <summary>
        /// The identifier of the transaction processing account in the access token scope.
        /// </summary>
        public string TransactionProcessingAccountID { get; set; }
        /// <summary>
        /// The identifier of the risk assessment account in the access token scope.
        /// </summary>
        public string RiskAssessmentAccountID { get; set; }
        /// <summary>
        /// The identifier of the merchant management account in the access token scope.
        /// </summary>
        public string MerchantManagementAccountID { get; set; }
        /// <summary>
        /// The identifier of the file processing account in the access token scope.
        /// </summary>
        public string FileProcessingAccountID { get; set; }
    }
}
