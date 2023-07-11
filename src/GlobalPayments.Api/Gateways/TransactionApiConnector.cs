using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Mapping;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Utils;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace GlobalPayments.Api.Gateways {
    internal partial class TransactionApiConnector : RestGateway, IPaymentGateway, IReportingService {

        private string _AccessToken;
        public string AccessToken {
            get {
                return _AccessToken;
            }
            internal set {
                _AccessToken = value;
                if (string.IsNullOrEmpty(_AccessToken)) {
                    Headers.Remove("Authorization");
                }
                else {
                    Headers["Authorization"] = $"AuthToken {_AccessToken}";
                }
            }
        }
        public string Region { get; set; }
        public string AccountCredential { get; set; }
        public string AppSecret { get; set; }
        public bool SupportsHostedPayments => true;
        public bool SupportsOpenBanking => false;


        internal TransactionApiConnector() {
            // Set required api version header
            Headers["X-GP-Api-Key"] = "qeG6EWZOiAwk4jsiHzsh2BN8VkN2rdAs";
            Headers["X-GP-Version"] = "2021-04-08";
        }


        #region Interface Implementations
        public Transaction ProcessAuthorization(AuthorizationBuilder builder) {
            SignIn();
            TransactionApiRequest request = TransactionApiAuthorizationRequestBuilder.BuildRequest(builder, this);

            if (request != null)
            {
                var response = DoTransaction(request.Verb, request.Endpoint, request.RequestBody, request.QueryStringParams);
                return TransactionApiMapping.MapResponse(response, builder.TransactionType);
            }
            return null;
        }

        public Transaction ManageTransaction(ManagementBuilder builder) {
            SignIn();
            TransactionApiRequest request = TransactionApiManagementRequestBuilder.BuildRequest(builder, this);

            if (request != null) {
                var response = DoTransaction(request.Verb, request.Endpoint, request.RequestBody, request.QueryStringParams);
                return TransactionApiMapping.MapResponse(response, builder.TransactionType,((TransactionReference)builder.PaymentMethod).OriginalTransactionType);
            }
            return null;
        }

        public T ProcessReport<T>(ReportBuilder<T> builder) where T : class {
            SignIn();
            TransactionApiRequest request = TransactionApiReportRequestBuilder.BuildRequest(builder, this);

            if (request != null) {
                var response = DoTransaction(request.Verb, request.Endpoint, request.RequestBody, request.QueryStringParams);
                return TransactionApiMapping.MapReportResponse<T>(response, builder.ReportType);
            }
            return null;
        }

        public string SerializeRequest(AuthorizationBuilder builder) {
            throw new NotImplementedException();
        }
        #endregion

        public void SignIn() {
                AccessToken = GetAccessToken();
        }

        public void SignOut() { }

        public string GetAccessToken() {
            AccessToken = null;
            AccessToken = new TransactionApiSessionInfo().SignIn(Region, AccountCredential, AppSecret);
            return AccessToken;
        }

        public string DoTransaction(HttpMethod verb, string endpoint, string data = null, Dictionary<string, string> queryStringParams = null) {
            if (string.IsNullOrEmpty(AccessToken)) {
                SignIn();
            }
            try {
                return base.DoTransaction(verb, endpoint, data, queryStringParams, false);
            }
            catch (GatewayException ex) {
                throw ex;
            }
        }

        protected override string HandleResponse(GatewayResponse response) {
            if (response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent && response.StatusCode != HttpStatusCode.OK
                && response.StatusCode != (HttpStatusCode)TransactionApiStatusCode.PartiallyApproved) {
                var parsed = JsonDoc.Parse(response.RawResponse);
                if (parsed.Has("error_code")) {
                    string errorCode = parsed.GetValue<string>("error_code");
                    string detailedErrorCode = parsed.GetValue<string>("detailed_error_code");
                    string detailedErrorDescription = parsed.GetValue<string>("detailed_error_description");

                    throw new GatewayException($"Status Code: {response.StatusCode} - {detailedErrorDescription}", errorCode, detailedErrorCode);
                }
                throw new GatewayException($"Status Code: {response.StatusCode}", responseMessage: response.RawResponse);
            }
            return response.RawResponse;
        }
    }
}
    