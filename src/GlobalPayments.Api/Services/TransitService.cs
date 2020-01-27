using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Gateways;

namespace GlobalPayments.Api.Services {
    public class TransitService {
        public static string GenerateTransactionKey(Environment environment, string merchantId, string userId, string password, string transactionKey = null) {
            var connector = new TransitConnector {
                MerchantId = merchantId,
                TransactionKey = transactionKey,
                ServiceUrl = environment.Equals(Environment.PRODUCTION) ? ServiceEndpoints.TRANSIT_MULTIPASS_PRODUCTION: ServiceEndpoints.TRANSIT_MULTIPASS_TEST,
                Timeout = 10000
            };

            return connector.GenerateKey(userId, password);
        }
    }
}
