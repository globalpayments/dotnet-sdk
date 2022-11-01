using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.GpEcom {
   
    public class ThreeDSecureAcsClient {
        private string _serviceUrl;
        
        public ThreeDSecureAcsClient(string url) {
            _serviceUrl = url;
        }

        public dynamic Authenticate(string payerAuthRequest, string merchantData = "") {
            HttpClient httpClient = new HttpClient() {
                Timeout = TimeSpan.FromMilliseconds(60000)
            };

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, _serviceUrl);
            HttpResponseMessage response = null;
            try {
                var kvps = new List<KeyValuePair<string, string>>();
                kvps.Add(new KeyValuePair<string, string>("PaReq", payerAuthRequest));
                kvps.Add(new KeyValuePair<string, string>("TermUrl", @"https://www.mywebsite.com/process3dSecure"));
                kvps.Add(new KeyValuePair<string, string>("MD", merchantData));

                request.Content = new FormUrlEncodedContent(kvps);
                response = httpClient.SendAsync(request).Result;
                var rawResponse = response.Content.ReadAsStringAsync().Result;
                if (response.StatusCode != HttpStatusCode.OK) {
                    throw new Exception(rawResponse);
                }

                var authResponse = GetInputValue(rawResponse, "PaRes");
                var md = GetInputValue(rawResponse, "MD");

                return new {
                    pares = authResponse,
                    md
                };
            }
            catch (Exception) {
                throw;
            }
            finally { }
        }

        private string GetInputValue(string raw, string inputValue) {
            var searchString = string.Format("name=\"{0}\" value=\"", inputValue);

            var index = raw.IndexOf(searchString);
            if (index > -1) {
                index = index + searchString.Length;

                var length = raw.IndexOf("\"", index) - index;
                return raw.Substring(index, length);
            }
            return null;
        }
    }
}
