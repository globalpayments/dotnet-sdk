using GlobalPayments.Api.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GlobalPayments.Api.Entities {
    internal class GpApiTokenResponse {
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
        internal string DataAccountName { get { return GetAccountName("DAA_"); } }
        internal string DisputeManagementAccountName { get { return GetAccountName("DIA_"); } }
        internal string TokenizationAccountName { get { return GetAccountName("TKA_"); } }
        internal string TransactionProcessingAccountName { get { return GetAccountName("TRA_"); } }

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
