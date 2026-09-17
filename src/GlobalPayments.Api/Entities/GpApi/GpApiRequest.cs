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
        public const string PAYERS_ENDPOINT = "/payers";
        public const string INSTALLMENT_ENDPOINT = "/installments";
        public const string DECRYPT_ENDPOINT = "/decrypt";

        /// <summary>
        /// Builds the GP-API merchant-management collection path.
        /// </summary>
        /// <param name="config">The GP-API configuration containing the optional MMA account id.</param>
        /// <returns>The merchant-management collection path.</returns>
        internal static string GetMerchantManagementUrl(GpApiConfig config) {
            if (!string.IsNullOrEmpty(config?.MerchantManagementAccountID)) {
                return $"{ACCOUNTS_ENDPOINT}/{config.MerchantManagementAccountID}{MERCHANT_MANAGEMENT_ENDPOINT}";
            }

            return MERCHANT_MANAGEMENT_ENDPOINT;
        }

        /// <summary>
        /// Builds an account and merchant URL prefix for the supplied or configured merchant id and MMA account id.
        /// </summary>
        /// <param name="config">The GP-API configuration containing merchant routing values.</param>
        /// <param name="merchantId">Optional explicit merchant id override. If omitted or null, defaults to config.MerchantId.</param>
        /// <returns>The account and merchant URL prefix, or an empty string when no routing values are supplied or configured.</returns>
        internal static string GetMerchantUrl(GpApiConfig config, string merchantId = null) {
            var effectiveMerchantId = !string.IsNullOrEmpty(merchantId) ? merchantId : config?.MerchantId;

            // Standalone account or token-style calls: no merchant routing prefix is needed.
            if (string.IsNullOrEmpty(effectiveMerchantId)) {
                return string.Empty;
            }

            // Multiple-MMA merchant calls: include both MMA and merchant context in the path.
            if (!string.IsNullOrEmpty(config?.MerchantManagementAccountID)) {
                return $"{ACCOUNTS_ENDPOINT}/{config.MerchantManagementAccountID}{MERCHANT_MANAGEMENT_ENDPOINT}/{effectiveMerchantId}";
            }

            // Existing single-MMA merchant calls: preserve the original merchant-scoped path.
            return $"{MERCHANT_MANAGEMENT_ENDPOINT}/{effectiveMerchantId}";
        }
    }
}
