using System;
using GlobalPayments.Api.Utils;
using GlobalPayments.Api.Entities.Payroll;
using System.Net.Http;
using System.Net;
using GlobalPayments.Api.Entities;
using System.Text;

namespace GlobalPayments.Api.Gateways {
    internal class PayrollConnector : RestGateway {
        private PayrollEncoder _encoder;

        public string Username { get; internal set; }
        public string Password { get; internal set; }
        public string ApiKey { get; internal set; }
        public string SessionToken { get; internal set; }

        internal PayrollEncoder Encoder {
            get {
                if (_encoder == null) {
                    _encoder = new PayrollEncoder {
                        Username = Username,
                        ApiKey = ApiKey
                    };
                }
                return _encoder;
            }
            set { _encoder = value; }
        }

        public void SignIn() {
            var request = SessionInfo.SignIn(Username, Password, Encoder);

            var response = SendEncryptedRequest<SessionInfo>(request).Results[0];
            if (!string.IsNullOrEmpty(response.ErrorMessage))
                throw new ApiException(response.ErrorMessage);

            SessionToken = response.SessionToken;

            // Build the basic request header
            var credentials = string.Format("{0}|{1}", SessionToken, Username);
            var basicAuth = Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials));
            Headers["Authorization"] = string.Format("Basic {0}", basicAuth);
        }
        public void SignOut() {
            SendEncryptedRequest<SessionInfo>(SessionInfo.SignOut());
        }

        public PayrollResponse<T> SendEncryptedRequest<T>(Func<PayrollEncoder, object[], PayrollRequest> buildRequest, params object[] args) where T : PayrollEntity {
            return SendEncryptedRequest<T>(buildRequest(Encoder, args));
        }

        public PayrollResponse<T> SendEncryptedRequest<T>(PayrollRequest request) where T : PayrollEntity {
            if (!(typeof(T) == typeof(SessionInfo)) && string.IsNullOrEmpty(SessionToken))
                throw new ApiException("Payroll connector is not signed in, please check your configuration.");

            var response = DoTransaction(HttpMethod.Post, request.Endpoint, request.RequestBody);
            return Activator.CreateInstance(typeof(PayrollResponse<T>), new object[] { response, Encoder }) as PayrollResponse<T>;
        }

        protected override string HandleResponse(GatewayResponse response) {
            if (response.StatusCode != HttpStatusCode.OK) {
                var responseMessage = JsonDoc.ParseSingleValue<string>(response.RawResponse, "ResponseMessage");
                throw new GatewayException(string.Format("Status Code: {0} - {1}", response.StatusCode, responseMessage));
            }
            return response.RawResponse;
        }
    }
}
