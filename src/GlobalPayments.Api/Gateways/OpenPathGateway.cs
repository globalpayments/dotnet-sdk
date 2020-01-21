using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace GlobalPayments.Api.Gateways {
    public class OpenPathGateway {
        private string OpenPathApiKey { get; set; }
        private string OpenPathApiUrl { get; set; }
        private long OpenPathTransactionId { get; set; }
        private string PaymentTransactionId { get; set; }
        private AuthorizationBuilder AuthorizationBuilder { get; set; }

        public OpenPathGateway WithOpenPathApiKey(string openPathApiKey) {
            OpenPathApiKey = openPathApiKey;
            return this;
        }

        public OpenPathGateway WithOpenPathApiUrl(string openPathApiUrl) {
            OpenPathApiUrl = openPathApiUrl;
            return this;
        }

        public OpenPathGateway WithPaymentTransactionId(string paymentTransactionId) {
            PaymentTransactionId = paymentTransactionId;
            return this;
        }

        public OpenPathGateway WithAuthorizationBuilder(AuthorizationBuilder authorizationBuilder) {
            AuthorizationBuilder = authorizationBuilder;
            return this;
        }

        public bool IsValidForSideIntegration() {
            return !string.IsNullOrWhiteSpace(OpenPathApiKey) && !string.IsNullOrWhiteSpace(OpenPathApiUrl);
        }

        #region OpenPath Validation
        /// <summary>
        /// Perform OpenPath side integration
        /// Throws exception if transaction is not approved
        /// </summary>
        /// <returns></returns>
        public OpenPathResponse Process() {
            if (string.IsNullOrWhiteSpace(OpenPathApiKey))
                throw new BuilderException("OpenPath Api Key cannot be null or empty");
            else if (string.IsNullOrWhiteSpace(OpenPathApiUrl))
                throw new BuilderException("OpenPath Api Url cannot be null or empty");
            else if (AuthorizationBuilder == null)
                throw new BuilderException("Cannot process OpenPath integration, AuthorizationBuilder is null");

            var openPathTransaction = new OpenPathTransaction().MapData(AuthorizationBuilder);
            openPathTransaction.OpenPathApiKey = OpenPathApiKey;
            var result = SendRequest(JsonConvert.SerializeObject(openPathTransaction), OpenPathApiUrl);

            string additionalInformation = string.Join(",", result.Results);
            switch (result.Status) {
                case OpenPathStatusType.Declined:
                    throw new BuilderException($"Transaction declined by OpenPath: { result.Message}{System.Environment.NewLine}{additionalInformation}");
                case OpenPathStatusType.Error:
                    throw new BuilderException($"{ result.Message}{System.Environment.NewLine}{additionalInformation}");
                case OpenPathStatusType.Rejected:
                    throw new BuilderException($"Transaction rejected by OpenPath: { result.Message}{System.Environment.NewLine}{additionalInformation}");
                case OpenPathStatusType.Queued:
                    throw new BuilderException($"Transaction has been put to queue by OpenPath: { result.Message}{System.Environment.NewLine}{additionalInformation}");
                case OpenPathStatusType.Approved:
                case OpenPathStatusType.Processed:
                case OpenPathStatusType.BouncedBack:
                    OpenPathTransactionId = result.TransactionId;
                    break;
            }
            return result;
        }
        #endregion

        #region Sends the transaction Id to OpenPath
        public void SaveTransactionId() {
            if (!string.IsNullOrWhiteSpace(OpenPathApiKey) &&
                !string.IsNullOrWhiteSpace(OpenPathApiUrl) &&
                !string.IsNullOrWhiteSpace(PaymentTransactionId) &&
                OpenPathTransactionId != 0) {
                SendRequest(
                    JsonConvert.SerializeObject(
                        new {
                            PaymentTransactionId,
                            OpenPathTransactionId
                        }
                    ),
                    $"{OpenPathApiUrl}/updatetransactionid"
                );
            }
        }
        #endregion

        private OpenPathResponse SendRequest(string jsonContent, string url) {

            var result = new OpenPathResponse();

            // using (var client = new HttpClient() { Timeout = TimeSpan.FromMilliseconds(30000) })
            using (var client = new HttpClient()) { 
                using (var request = new HttpRequestMessage(HttpMethod.Post, url)) {
                    using (var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json")) {

                        request.Content = stringContent;

                        using (var response = client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).Result) {
                            response.EnsureSuccessStatusCode();
                            var responseJson = response.Content.ReadAsStringAsync().Result;
                            result = JsonConvert.DeserializeObject<OpenPathResponse>(responseJson);
                        }

                    }
                }
            }

            return result;

        }

    }
}
