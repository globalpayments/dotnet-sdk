using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Gateways;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Services {
    public class GpApiService {
        public static AccessTokenInfo GenerateTransactionKey(GpApiConfig gpApiConfig) {
            GpApiConnector connector = new GpApiConnector(gpApiConfig);

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
