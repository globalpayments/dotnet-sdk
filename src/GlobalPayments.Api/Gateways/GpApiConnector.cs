using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Builders.RequestBuilder.GpApi;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Gateways.Interfaces;
using GlobalPayments.Api.Logging;
using GlobalPayments.Api.Mapping;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Utils;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Reflection;

namespace GlobalPayments.Api.Gateways {
    internal partial class GpApiConnector : RestGateway, IPaymentGateway, IReportingService, ISecure3dProvider, IPayFacProvider, IFraudCheckService, IDeviceCloudService, IFileProcessingService {
        private const string IDEMPOTENCY_HEADER = "x-gp-idempotency";

        private string _AccessToken;
        private string ReleaseVersion {
            get {
                try {
                    return Assembly.Load(new AssemblyName("GlobalPayments.Api"))?.GetName()?.Version?.ToString();
                }
                catch { return string.Empty; }
            }
        }

        public GpApiConfig GpApiConfig { get; set; }
        public Secure3dVersion Version { get { return Secure3dVersion.Any; } }

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
        public bool HasBuiltInMerchantManagementService => true;
        public bool SupportsHostedPayments { get { return true; } }
        public bool SupportsOpenBanking => true;


        public GpApiConnector(GpApiConfig gpApiConfig) {
            GpApiConfig = gpApiConfig;
            Timeout = gpApiConfig.Timeout;
            RequestLogger = gpApiConfig.RequestLogger;
            WebProxy = gpApiConfig.WebProxy;

            // Set required api version header
            Headers["X-GP-Version"] = "2021-03-22";
            Headers["Accept"] = "application/json";
            Headers["Accept-Encoding"] = "gzip";
            Headers["x-gp-sdk"] = "net;version=" + ReleaseVersion;

            DynamicHeaders = gpApiConfig.DynamicHeaders;
        }


        #region Interface Implementations
       

        public string SerializeRequest(AuthorizationBuilder builder) {
            throw new NotImplementedException();
        }
        
        
        public T ProcessPayFac<T>(PayFacBuilder<T> builder) where T : class {
            throw new UnsupportedTransactionException($"Method {this.GetType().GetMethod("ProcessPayFac")} not supported");
        }       

        public string ProcessPassThrough(JsonDoc rawRequest) {
            if (string.IsNullOrEmpty(AccessToken)) {
                SignIn();
            }

            Request request = new GpApiMiCRequestBuilder().BuildRequest(rawRequest.ToString(), this);

            if (request != null) {
                return DoTransaction(request.Verb, request.Endpoint, request.RequestBody, request.QueryStringParams);
            }
            return null;
        }
        #endregion

        public FileProcessor ProcessFileUpload(FileProcessingBuilder builder) {
            if (string.IsNullOrEmpty(AccessToken)) {
                SignIn();
            }
            var request = new GpApiFileProcessingRequestBuilder().BuildRequest(builder, this);
            if (request != null) {
                var response = DoTransaction(request.Verb, request.Endpoint, request.RequestBody, request.QueryStringParams);               
                return GpApiMapping.MapFileProcessingResponse(response);
            }
            return null;            
        }


        public void SignIn() {
            AccessTokenInfo accessTokenInfo = GpApiConfig.AccessTokenInfo;

            if (accessTokenInfo != null && !string.IsNullOrEmpty(accessTokenInfo.Token)) {
                AccessToken = accessTokenInfo.Token;
                return;
            }
            var response = GetAccessToken();

            AccessToken = response.Token;

            if (accessTokenInfo == null) {
                accessTokenInfo = new AccessTokenInfo();
            }

            if (string.IsNullOrEmpty(accessTokenInfo.DataAccountID) &&
                string.IsNullOrEmpty(accessTokenInfo.DataAccountName)) {
                accessTokenInfo.DataAccountID = response.DataAccountID;
            }
            if (string.IsNullOrEmpty(accessTokenInfo.TokenizationAccountName) &&
                string.IsNullOrEmpty(accessTokenInfo.TokenizationAccountID)) {
                accessTokenInfo.TokenizationAccountID = response.TokenizationAccountID;
            }
            if (string.IsNullOrEmpty(accessTokenInfo.DisputeManagementAccountName) &&
                string.IsNullOrEmpty(accessTokenInfo.DisputeManagementAccountID)) {
                accessTokenInfo.DisputeManagementAccountID = response.DisputeManagementAccountID;
            }

            if (string.IsNullOrEmpty(accessTokenInfo.TransactionProcessingAccountName) &&
                string.IsNullOrEmpty(accessTokenInfo.TransactionProcessingAccountID)) {
                accessTokenInfo.TransactionProcessingAccountID = response.TransactionProcessingAccountID;
            }
            if (string.IsNullOrEmpty(accessTokenInfo.RiskAssessmentAccountName) &&
                string.IsNullOrEmpty(accessTokenInfo.RiskAssessmentAccountID)) {
                accessTokenInfo.RiskAssessmentAccountID = response.RiskAssessmentAccountID;
            }
            if (string.IsNullOrEmpty(accessTokenInfo.MerchantManagementAccountName) &&
               string.IsNullOrEmpty(accessTokenInfo.MerchantManagementAccountID)) {
                accessTokenInfo.MerchantManagementAccountID = response.MerchantManagementAccountID;
            }
            if (string.IsNullOrEmpty(accessTokenInfo.FileProcessingAccountName) &&
               string.IsNullOrEmpty(accessTokenInfo.FileProcessingAccountID))
            {
                accessTokenInfo.FileProcessingAccountID = response.FileProcessingAccountID;
            }

            GpApiConfig.AccessTokenInfo = accessTokenInfo;
        }

        public void SignOut() {
            //SendEncryptedRequest<SessionInfo>(SessionInfo.SignOut());
        }

        public GpApiTokenResponse GetAccessToken() {
            AccessToken = null;

            Request request = GpApiSessionInfo.SignIn(GpApiConfig.AppId, GpApiConfig.AppKey, GpApiConfig.SecondsToExpire, GpApiConfig.IntervalToExpire, GpApiConfig.Permissions);

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
            try
            {
                if (Request.MaskedValues != null) {
                    MaskedRequestData = Request.MaskedValues;
                }
                return DoTransactionWithIdempotencyKey(verb, endpoint, data, queryStringParams, idempotencyKey);
            }
            catch (GatewayException ex)
            {
                if (ex.ResponseCode == "NOT_AUTHENTICATED" && !string.IsNullOrEmpty(GpApiConfig.AppId) && !string.IsNullOrEmpty(GpApiConfig.AppKey))
                {
                    SignIn();
                    return DoTransactionWithIdempotencyKey(verb, endpoint, data, queryStringParams, idempotencyKey);
                }
                throw ex;
            }
            finally {
                Request.MaskedValues = null;
                ProtectSensitiveData.DisposeCollection();
                MaskedRequestData = new Dictionary<string, string>();
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

            Request request = new GpApiAuthorizationRequestBuilder().BuildRequest(builder, this);

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

            Request request = new Builders.RequestBuilder.GpApi.GpApiManagementRequestBuilder().BuildRequest(builder, this);

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

            Request request = new GpApiReportRequestBuilder<T>().BuildRequest(builder, this);

            if (request != null) {
                var response = DoTransaction(request.Verb, request.Endpoint, request.RequestBody, request.QueryStringParams);

                return GpApiMapping.MapReportResponse<T>(response, builder.ReportType);
            }
            return null;
        }       

        public Transaction ProcessSecure3d(Secure3dBuilder builder) {
            if (string.IsNullOrEmpty(AccessToken)) {
                SignIn();
            }

            var request = new GpApiSecureRequestBuilder<ThreeDSecure>().BuildRequest(builder, this);

            if (request != null) {
                if (Request.MaskedValues != null) {
                    MaskedRequestData = Request.MaskedValues;
                }

                var response = DoTransaction(request.Verb, request.Endpoint, request.RequestBody, request.QueryStringParams, builder.IdempotencyKey);

                return GpApiMapping.Map3DSecureData(response);
            }
            return null;
        }

        public T ProcessBoardingUser<T>(PayFacBuilder<T> builder) where T : class 
        {
            T result = Activator.CreateInstance<T>();
            
            if (string.IsNullOrEmpty(AccessToken)) {
                SignIn();
            }
            Request request = new GpApiPayFacRequestBuilder<T>().BuildRequest(builder, this);

            if (request != null){
                var response = DoTransaction(request.Verb, request.Endpoint, request.RequestBody, request.QueryStringParams, builder.IdempotencyKey); 
                return GpApiMapping.MapMerchantEndpointResponse<T>(response);
            }
            return result;
        }
        
        public T ProcessFraud<T>(FraudBuilder<T> builder) where T : class
        {
            T result = Activator.CreateInstance<T>();
            if (string.IsNullOrEmpty(AccessToken)) {
                SignIn();
            }
            Request request = new GpApiSecureRequestBuilder<T>().BuildRequest(builder, this);

            if (request != null) {
                var response = DoTransaction(request.Verb, request.Endpoint, request.RequestBody, request.QueryStringParams, builder.IdempotencyKey);
                return GpApiMapping.MapRiskAssessmentResponse<T>(response);
            }
            return result;
        }      
    }
}
