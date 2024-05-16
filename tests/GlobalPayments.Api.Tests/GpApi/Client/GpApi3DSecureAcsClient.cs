using GlobalPayments.Api.Entities;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;

namespace GlobalPayments.Api.Tests.GpApi.Client
{
    /// <summary>
    /// This 3DS ACS client mocks the ACS Authentication Simulator used for testing purposes
    /// </summary>
    internal class GpApi3DSecureAcsClient {
        private string _redirectUrl;
        public GpApi3DSecureAcsClient(string redirectUrl) {
            _redirectUrl = redirectUrl;
        }

        private string SubmitFormData(string formUrl, List<KeyValuePair<string, string>> formData) {
            HttpClient httpClient = new HttpClient()
            {
                Timeout = TimeSpan.FromMilliseconds(6000)
            };

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, formUrl);
            HttpResponseMessage response = null;
            try {
                request.Content = new FormUrlEncodedContent(formData);
                response = httpClient.SendAsync(request).Result;
                var rawResponse = response.Content.ReadAsStringAsync().Result;

                if (response.StatusCode != HttpStatusCode.OK) {
                    throw new Exception(rawResponse);
                }

                return rawResponse;
            }
            catch (Exception) {
                throw;
            }
            finally {
            }
        }

        private string GetFormAction(string rawHtml, string formName) {
            var searchString = $"name=\"{formName}\" action=\"";

            var index = rawHtml.IndexOf(searchString, comparisonType: StringComparison.InvariantCultureIgnoreCase);
            if (index > -1) {
                index = index + searchString.Length;
                var length = rawHtml.IndexOf("\"", index) - index;
                return rawHtml.Substring(index, length);
            }

            return null;
        }

        private string GetInputValue(string rawHtml, string inputName) {
            var searchString = $"name=\"{inputName}\" value=\"";

            var index = rawHtml.IndexOf(searchString, comparisonType: StringComparison.InvariantCultureIgnoreCase);
            if (index > -1) {
                index = index + searchString.Length;
                var length = rawHtml.IndexOf("\"", index) - index;
                return rawHtml.Substring(index, length);
            }

            return null;
        }

        /// <summary>
        /// Performs ACS authentication for 3DS v1
        /// </summary>
        /// <param name="secureEcom"></param>
        /// <param name="paRes"></param>
        /// <param name="authenticationResultCode"></param>
        /// <returns></returns>
        protected internal string Authenticate_v1(ThreeDSecure secureEcom, out string paRes,
            AuthenticationResultCode authenticationResultCode = 0)
        {
            // Step 1
            var formData = new List<KeyValuePair<string, string>>();
            formData.Add(
                new KeyValuePair<string, string>(secureEcom.MessageType, secureEcom.PayerAuthenticationRequest));
            formData.Add(new KeyValuePair<string, string>(secureEcom.SessionDataFieldName,
                secureEcom.ServerTransactionId));
            formData.Add(new KeyValuePair<string, string>("TermUrl", secureEcom.ChallengeReturnUrl));
            formData.Add(new KeyValuePair<string, string>("AuthenticationResultCode",
                authenticationResultCode.ToString("D")));
            string rawResponse = SubmitFormData(secureEcom.IssuerAcsUrl, formData);

            paRes = GetInputValue(rawResponse, "PaRes");

            // Step 2
            formData = new List<KeyValuePair<string, string>>();
            formData.Add(new KeyValuePair<string, string>("MD", GetInputValue(rawResponse, "MD")));
            formData.Add(new KeyValuePair<string, string>("PaRes", paRes));
            rawResponse = SubmitFormData(GetFormAction(rawResponse, "PAResForm"), formData);

            return rawResponse;
        }

        /// <summary>
        /// Performs ACS authentication for 3DS v2
        /// </summary>
        /// <param name="secureEcom"></param>
        /// <returns></returns>
        protected internal string Authenticate_v2(ThreeDSecure secureEcom) {
            // Step 1
            var formData = new List<KeyValuePair<string, string>>();
            formData.Add(new KeyValuePair<string, string>(secureEcom.MessageType, secureEcom.PayerAuthenticationRequest));
            //formData.Add(new KeyValuePair<string, string>(secureEcom.SessionDataFieldName, secureEcom.ServerTransactionId));
            string rawResponse = SubmitFormData(secureEcom.IssuerAcsUrl, formData);

            // Step 2
            formData = new List<KeyValuePair<string, string>>();
            formData.Add(new KeyValuePair<string, string>("get-status-type", "true"));
            do {
                rawResponse = SubmitFormData(secureEcom.IssuerAcsUrl, formData);
                Thread.Sleep(2000);
            } while (rawResponse.Trim() == "IN_PROGRESS");

            // Step 3
            formData = new List<KeyValuePair<string, string>>();
            rawResponse = SubmitFormData(secureEcom.IssuerAcsUrl, formData);

            // Step 4
            formData = new List<KeyValuePair<string, string>>();
            formData.Add(new KeyValuePair<string, string>("cres", GetInputValue(rawResponse, "cres")));
            rawResponse = SubmitFormData(GetFormAction(rawResponse, "ResForm"), formData);

            return rawResponse;
        }
    }
}
