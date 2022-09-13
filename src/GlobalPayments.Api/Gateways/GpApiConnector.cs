using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Mapping;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Utils;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Reflection;

namespace GlobalPayments.Api.Gateways {
    internal partial class GpApiConnector : RestGateway, IPaymentGateway, IReportingService, ISecure3dProvider {
        private const string IDEMPOTENCY_HEADER = "x-gp-idempotency";        
        public GpApiConfig GpApiConfig { get; set; }

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
                    Headers["Authorization"] = $"Bearer {_AccessToken}";
                }
            }
        }       

        public bool SupportsHostedPayments { get { return true; } }

        public string SerializeRequest(AuthorizationBuilder builder) {
            throw new NotImplementedException();
        }

        public GpApiConnector(GpApiConfig gpApiConfig)
        {   
            GpApiConfig = gpApiConfig;
            Timeout = gpApiConfig.Timeout;            
            RequestLogger = gpApiConfig.RequestLogger;            
            WebProxy = gpApiConfig.WebProxy;

            // Set required api version header
            Headers["X-GP-Version"] = "2021-03-22";
            Headers["Accept"] = "application/json";
            Headers["Accept-Encoding"] = "gzip";
            Headers["x-gp-sdk"] = "net;version=" + getReleaseVersion();

            DynamicHeaders = gpApiConfig.DynamicHeaders;
        }
               

        //Get the SDK release version
        private string getReleaseVersion()
        {
            try
            {
                return Assembly.Load(new AssemblyName("GlobalPayments.Api"))?.GetName()?.Version?.ToString();
            }
            catch(Exception ex)
            {
                return string.Empty;
            }
        }

        public void SignIn() {
            AccessTokenInfo accessTokenInfo = GpApiConfig.AccessTokenInfo;

            if (accessTokenInfo != null && !string.IsNullOrEmpty(accessTokenInfo.Token)) {
                AccessToken = accessTokenInfo.Token;
                return;
            }
                var response = GetAccessToken();

                AccessToken = response.Token;

            if (accessTokenInfo == null)
            {
                accessTokenInfo = new AccessTokenInfo();
            }

            if (string.IsNullOrEmpty(accessTokenInfo.DataAccountName)) {
                accessTokenInfo.DataAccountName = response.DataAccountName;
            }
            if (string.IsNullOrEmpty(accessTokenInfo.DisputeManagementAccountName)) {
                accessTokenInfo.DisputeManagementAccountName = response.DisputeManagementAccountName;
            }
            if (string.IsNullOrEmpty(accessTokenInfo.TokenizationAccountName)) {
                accessTokenInfo.TokenizationAccountName = response.TokenizationAccountName;
            }
            if (string.IsNullOrEmpty(accessTokenInfo.TransactionProcessingAccountName)) {
                accessTokenInfo.TransactionProcessingAccountName = response.TransactionProcessingAccountName;
            }

            GpApiConfig.AccessTokenInfo = accessTokenInfo;
        }

        public void SignOut() {
            //SendEncryptedRequest<SessionInfo>(SessionInfo.SignOut());
        }

        public GpApiTokenResponse GetAccessToken() {
            AccessToken = null;

            GpApiRequest request = GpApiSessionInfo.SignIn(GpApiConfig.AppId, GpApiConfig.AppKey, GpApiConfig.SecondsToExpire, GpApiConfig.IntervalToExpire, GpApiConfig.Permissions);

            string response = base.DoTransaction(request.Verb, request.Endpoint, request.RequestBody);

            return Activator.CreateInstance(typeof(GpApiTokenResponse), new object[] { response }) as GpApiTokenResponse;
        }

        private string DoTransactionWithIdempotencyKey(HttpMethod verb, string endpoint, string data = null, Dictionary<string, string> queryStringParams = null, string idempotencyKey = null) {
            if (!string.IsNullOrEmpty(idempotencyKey)) {
                Headers[IDEMPOTENCY_HEADER] = idempotencyKey;
            }
            try {
                return base.DoTransaction(verb, endpoint, data, queryStringParams);
            }
            finally {
                Headers.Remove(IDEMPOTENCY_HEADER);
            }
        }

        public string DoTransaction(HttpMethod verb, string endpoint, string data = null, Dictionary<string, string> queryStringParams = null, string idempotencyKey = null) {
            if (string.IsNullOrEmpty(AccessToken)) {
                SignIn();
            }
            try {
                return DoTransactionWithIdempotencyKey(verb, endpoint, data, queryStringParams, idempotencyKey);
            }
            catch (GatewayException ex) {
                if (ex.ResponseCode == "NOT_AUTHENTICATED" && !string.IsNullOrEmpty(GpApiConfig.AppId) && !string.IsNullOrEmpty(GpApiConfig.AppKey)) {
                    SignIn();
                    return DoTransactionWithIdempotencyKey(verb, endpoint, data, queryStringParams, idempotencyKey);
                }
                throw ex;
            }
        }

        protected override string HandleResponse(GatewayResponse response) {
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.NoContent) {
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

        public Transaction ProcessAuthorization(AuthorizationBuilder builder) {
            if (string.IsNullOrEmpty(AccessToken)) {
                SignIn();
            }

            GpApiRequest request = GpApiAuthorizationRequestBuilder.BuildRequest(builder, this);

            if (request != null) {
                var response = DoTransaction(request.Verb, request.Endpoint, request.RequestBody, request.QueryStringParams, builder.IdempotencyKey);
                if (builder.PaymentMethod is AlternativePaymentMethod) {
                    return GpApiMapping.MapResponseAPM(response);
                }
                return GpApiMapping.MapResponse(response);
            }
            return null;
        }

        public Transaction ManageTransaction(ManagementBuilder builder) {
            if (string.IsNullOrEmpty(AccessToken)) {
                SignIn();
            }

            GpApiRequest request = GpApiManagementRequestBuilder.BuildRequest(builder, this);

            if (request != null) {
                var response = DoTransaction(request.Verb, request.Endpoint, request.RequestBody, request.QueryStringParams, builder.IdempotencyKey);
                if (builder.PaymentMethod is TransactionReference && builder.PaymentMethod.PaymentMethodType == PaymentMethodType.APM) {
                    return GpApiMapping.MapResponseAPM(response);
                }
                return GpApiMapping.MapResponse(response);
            }
            return null;
        }

        public T ProcessReport<T>(ReportBuilder<T> builder) where T : class {
            if (string.IsNullOrEmpty(AccessToken)) {
                SignIn();
            }

            GpApiRequest request = GpApiReportRequestBuilder.BuildRequest(builder, this);

            if (request != null) {
                var response = DoTransaction(request.Verb, request.Endpoint, request.RequestBody, request.QueryStringParams);

                return GpApiMapping.MapReportResponse<T>(response, builder.ReportType);
            }
            return null;
        }

        public Secure3dVersion Version { get { return Secure3dVersion.Any; } }

        public Transaction ProcessSecure3d(Secure3dBuilder builder) {
            if (string.IsNullOrEmpty(AccessToken)) {
                SignIn();
            }

            var request = GpApiSecure3DRequestBuilder.BuildRequest(builder, this);

            if (request != null) {
                var response = DoTransaction(request.Verb, request.Endpoint, request.RequestBody, request.QueryStringParams, builder.IdempotencyKey);

                return GpApiMapping.Map3DSecureData(response);
            }
            return null;
        }
    }
}
