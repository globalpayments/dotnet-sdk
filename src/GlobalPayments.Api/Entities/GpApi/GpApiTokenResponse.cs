using GlobalPayments.Api.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GlobalPayments.Api.Entities {
    internal class GpApiTokenResponse {
        const string DATA_ACCOUNT_NAME_PREFIX = "DAA_";
        const string DISPUTE_MANAGEMENT_ACCOUNT_NAME_PREFIX = "DIA_";
        const string TOKENIZATION_ACCOUNT_NAME_PREFIX = "TKA_";
        const string TRANSACTION_PROCESSING_ACCOUNT_NAME_PREFIX = "TRA_";
        const string RIKS_ASSESSMENT_ACCOUNT_NAME_PREFIX = "RAA_";
        const string MERCHANT_MANAGEMENT_ACCOUNT_NAME_PREFIX = "MMA_";
        const string FILE_PROCESSING_ACCOUNT_NAME_PREFIX = "FPA_";

        internal string Token { get; private set; }
        internal string Type { get; private set; }
        internal string AppId { get; private set; }
        internal string AppName { get; private set; }
        internal DateTime TimeCreated { get; private set; }
        internal int SecondsToExpire { get; private set; }
        internal string Email { get; private set; }
        internal string MerchantId { get; private set; }
        internal string MerchantName { get; private set; }
        internal GpApiAccount[] Accounts { get; private set; }
        internal string DataAccountName { get { return GetAccountName(DATA_ACCOUNT_NAME_PREFIX); } }
        internal string DisputeManagementAccountName { get { return GetAccountName(DISPUTE_MANAGEMENT_ACCOUNT_NAME_PREFIX); } }
        internal string TokenizationAccountName { get { return GetAccountName(TOKENIZATION_ACCOUNT_NAME_PREFIX); } }
        internal string TransactionProcessingAccountName { get { return GetAccountName(TRANSACTION_PROCESSING_ACCOUNT_NAME_PREFIX); } }
        internal string RiskAssessmentAccountName { get { return GetAccountName(RIKS_ASSESSMENT_ACCOUNT_NAME_PREFIX); } }
        internal string MerchantManagementAccountName { get { return GetAccountName(MERCHANT_MANAGEMENT_ACCOUNT_NAME_PREFIX); } }
        internal string FileProcessingAccountName { get { return GetAccountName(FILE_PROCESSING_ACCOUNT_NAME_PREFIX); } }

        internal string DataAccountID { get { return GetAccountID(DATA_ACCOUNT_NAME_PREFIX); } }
        internal string DisputeManagementAccountID { get { return GetAccountID(DISPUTE_MANAGEMENT_ACCOUNT_NAME_PREFIX); } }
        internal string TokenizationAccountID { get { return GetAccountID(TOKENIZATION_ACCOUNT_NAME_PREFIX); } }
        internal string TransactionProcessingAccountID { get { return GetAccountID(TRANSACTION_PROCESSING_ACCOUNT_NAME_PREFIX); } }
        internal string RiskAssessmentAccountID { get { return GetAccountID(RIKS_ASSESSMENT_ACCOUNT_NAME_PREFIX); } }
        internal string MerchantManagementAccountID { get { return GetAccountID(MERCHANT_MANAGEMENT_ACCOUNT_NAME_PREFIX); } }
        internal string FileProcessingAccountID { get { return GetAccountID(FILE_PROCESSING_ACCOUNT_NAME_PREFIX); } }

        private string GetAccountID(string accountPrefix) {
            return Accounts?.Where(a => a.Id.StartsWith(accountPrefix)).Select(a => a.Id).FirstOrDefault();
        }

        private string GetAccountName(string accountPrefix) {
            return Accounts?.Where(a => a.Id.StartsWith(accountPrefix)).Select(a => a.Name).FirstOrDefault();
        }

        public GpApiTokenResponse(string jsonString) {
            JsonDoc doc = JsonDoc.Parse(jsonString);

            MapResponseValues(doc);
        }

        internal virtual void MapResponseValues(JsonDoc doc) {
            Token = doc.GetValue<string>("token");
            Type = doc.GetValue<string>("type");
            AppId = doc.GetValue<string>("app_id");
            AppName = doc.GetValue<string>("app_name");
            TimeCreated = doc.GetValue<DateTime>("time_created");
            SecondsToExpire = doc.GetValue<int>("seconds_to_expire");
            Email = doc.GetValue<string>("email");

            if (doc.Has("scope")) {
                JsonDoc scope = doc.Get("scope");
                MerchantId = scope.GetValue<string>("merchant_id");
                MerchantName = scope.GetValue<string>("merchant_name");
                if (scope.Has("accounts")) {
                    var accounts = new List<GpApiAccount>();
                    foreach (JsonDoc account in scope.GetArray<JsonDoc>("accounts")) {
                        accounts.Add(new GpApiAccount {
                            Id = account.GetValue<string>("id"),
                            Name = account.GetValue<string>("name"),
                        });
                    }
                    Accounts = accounts.ToArray();
                }
            }
        }
    }
}
