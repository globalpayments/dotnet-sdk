using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.OnlineBoarding;
using GlobalPayments.Api.Utils;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;

namespace GlobalPayments.Api.Gateways {
    internal class OnlineBoardingConnector : Gateway {
        internal string Portal { get; set; }

        public OnlineBoardingConnector() : base("multipart/form-data;") {
            Timeout = -1;
        }

        private GatewayResponse Authenticate(string portal, string accessToken) {
            var response = SendRequest(HttpMethod.Get, string.Format(@"https://onlineboarding.heartlandpaymentsystems.com/{0}/Invitation/{1}", portal, accessToken));
            if (response.StatusCode == HttpStatusCode.OK) {
                if (response.RequestUrl.Contains(accessToken))
                    throw new GatewayException("Invalid invitation token.");
                return response;
            }
            else throw new GatewayException("Invalid invitation token.");
        }

        private GatewayResponse GetPortalUrl(string portal) {
            var response = SendRequest(HttpMethod.Get, string.Format(@"https://onlineboarding.heartlandpaymentsystems.com/{0}", portal));
            if (response.StatusCode == HttpStatusCode.OK) {
                return response;
            }
            else throw new GatewayException("Invalid portal");
        }

        public MultipartForm ValidateApplication(string url, BoardingApplication application) {
            var content = application.BuildForm();

            var validationResponse = SendRequest(HttpMethod.Post, url + "/ValidateFieldValues", content.ToJson(), null, "application/json");
            if (validationResponse.StatusCode == HttpStatusCode.OK) {
                var validationErrors = application.ProcessValidationResult(validationResponse.RawResponse);
                if (validationErrors.Count > 0) {
                    throw new ValidationException(validationErrors);
                }
                else return content;
            }
            throw new GatewayException("Unable to validate form for submission.");
        }

        public BoardingResponse SendApplication(string invitation, BoardingApplication application = null) {
            if (application == null)
                throw new GatewayException("Application cannot be null.");

            // authorize session with the invitation
            var authSession = invitation != null ? Authenticate(Portal, invitation) :  GetPortalUrl(Portal);

             // validate
             var content = ValidateApplication(authSession.RequestUrl, application);
            
            // use the return URL for the next call
            try {
                var response = SendRequest(authSession.RequestUrl, content.Content);
                return BuildResponse(response);
            }
            catch (GatewayException) {
                throw;
            }
            catch (Exception exc) {
                throw new GatewayException(exc.Message, exc);
            }
        }

        private BoardingResponse BuildResponse(GatewayResponse response, string message = null) {
            if (response.StatusCode == HttpStatusCode.OK) {
                var boardingResponse = new BoardingResponse();

                if (response.RequestUrl.Contains("ThankYou")) {
                    boardingResponse.ApplicationId = int.Parse(response.RequestUrl.Substring(response.RequestUrl.IndexOf("=") + 1));
                    boardingResponse.Message = ParseResponse(response.RawResponse);
                }
                else if(response.RequestUrl.Contains("sign.myhpy.com")) {
                    boardingResponse.SignatureUrl = response.RequestUrl;
                    boardingResponse.Message = "Thank you for your submission.";
                }

                return boardingResponse;
            }
            else throw new GatewayException(message ?? "Unknown application submission error.");
        }

        private string ParseResponse(string raw) {
            Func<string, string> scrubHtml = (input) => {
                var step1 = Regex.Replace(input, @"<[^>]+>|&nbsp;|[\r\n]?", "");
                var step2 = Regex.Replace(step1, @"\s{2,}", " ");
                return step2;
            };


            var sb = new StringBuilder();

            var tags = new string[] { "h1", "div" };
            int start = 0;
            foreach (var tag in tags) {
                var startTag = string.Format("<{0}>", tag);
                var endTag = string.Format("</{0}>", tag);

                start = raw.IndexOf(startTag, start) + startTag.Length;
                var length = raw.IndexOf(endTag, start) - start;

                if (length > 0) {
                    var message = raw.Substring(start, length);
                    sb.AppendLine(scrubHtml(message));
                }
            }

            return sb.ToString();
        }
    }
}
