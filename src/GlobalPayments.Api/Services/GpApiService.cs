using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Gateways;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Services {
    public class GpApiService {
        public static AccessTokenInfo GenerateTransactionKey(Entities.Environment environment, string appId, string appKey, int? secondsToExpire = null, IntervalToExpire? intervalToExpire = null, string[] permissions = null) {
            var connector = new GpApiConnector {
                AppId = appId,
                AppKey = appKey,
                SecondsToExpire = secondsToExpire,
                IntervalToExpire = intervalToExpire,
                ServiceUrl = environment.Equals(Entities.Environment.PRODUCTION) ? ServiceEndpoints.GP_API_PRODUCTION : ServiceEndpoints.GP_API_TEST,
                Timeout = 10000,
                Permissions = permissions,
            };

            var data = connector.GetAccessToken();

            return new AccessTokenInfo {
                Token = data.Token,
                DataAccountName = data.DataAccountName,
                DisputeManagementAccountName = data.DisputeManagementAccountName,
                TokenizationAccountName = data.TokenizationAccountName,
                TransactionProcessingAccountName = data.TransactionProcessingAccountName,
            };
        }
    }
}
