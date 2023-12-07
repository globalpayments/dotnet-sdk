using System.Collections.Generic;
using System.Net.Http;

namespace GlobalPayments.Api.Entities.GpApi {
    internal class GpApiRequest {
        public const string ACCESS_TOKEN_ENDPOINT = "/accesstoken";
        public const string TRANSACTION_ENDPOINT = "/transactions";
        public const string PAYMENT_METHODS_ENDPOINT = "/payment-methods";
        public const string VERIFICATIONS_ENDPOINT = "/verifications";
        public const string DEPOSITS_ENDPOINT = "/settlement/deposits";
        public const string DISPUTES_ENDPOINT = "/disputes";
        public const string CAPTURES_ENDPOINT = "/capture";
        public const string SETTLEMENT_DISPUTES_ENDPOINT = "/settlement/disputes";
        public const string SETTLEMENT_TRANSACTIONS_ENDPOINT = "/settlement/transactions";
        public const string AUTHENTICATIONS_ENDPOINT = "/authentications";
        public const string BATCHES_ENDPOINT = "/batches";
        public const string ACTIONS_ENDPOINT = "/actions";
        public const string MERCHANT_MANAGEMENT_ENDPOINT = "/merchants";
        public const string DCC_ENDPOINT = "/currency-conversions";
        public const string PAYBYLINK_ENDPOINT = "/links";
        public const string RISK_ASSESSMENTS = "/risk-assessments";
        public const string ACCOUNTS_ENDPOINT = "/accounts";
        public const string TRANSFER_ENDPOINT = "/transfers";
        public const string DEVICES = "/devices";
        public const string FILE_PROCESSING = "/files";
    }
}
