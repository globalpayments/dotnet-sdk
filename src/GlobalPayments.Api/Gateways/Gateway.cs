using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using GlobalPayments.Api.Entities;
using System.Threading.Tasks;
using System.Diagnostics;
using GlobalPayments.Api.Logging;
using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Gateways {
    internal abstract class Gateway {
        private string _contentType;

        //public bool EnableLogging { get; set; }
        public IRequestLogger RequestLogger { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public int Timeout { get; set; }
        public string ServiceUrl { get; set; }

        public Gateway(string contentType) {
            Headers = new Dictionary<string, string>();
            _contentType = contentType;
        }

        protected GatewayResponse SendRequest(HttpMethod verb, string endpoint, string data = null, Dictionary<string, string> queryStringParams = null, string contentType = null) {
            HttpClient httpClient = new HttpClient {
                Timeout = TimeSpan.FromMilliseconds(Timeout)
            };

            var queryString = BuildQueryString(queryStringParams);
            HttpRequestMessage request = new HttpRequestMessage(verb, ServiceUrl + endpoint + queryString);
            foreach (var item in Headers) {
                request.Headers.Add(item.Key, item.Value);
            }

            HttpResponseMessage response = null;
            try {
                if (verb != HttpMethod.Get && data != null) {
                    request.Content = new StringContent(data, Encoding.UTF8, contentType ?? _contentType);
                    RequestLogger?.RequestSent(data);                    
                }
                response = httpClient.SendAsync(request).Result;

                string rawResponse = response.Content.ReadAsStringAsync().Result;
                RequestLogger?.ResponseReceived(rawResponse);

                return new GatewayResponse {
                    StatusCode = response.StatusCode,
                    RequestUrl = response.RequestMessage.RequestUri.ToString(),
                    RawResponse = rawResponse
                };
            }
            catch (Exception exc) {
                throw new GatewayException("Error occurred while communicating with gateway.", exc);
            }
            finally { }
        }

        protected async Task<GatewayResponse> SendRequestAsync(string endpoint, MultipartFormDataContent content) {
            HttpClient httpClient = new HttpClient {
                Timeout = TimeSpan.FromMilliseconds(Timeout)
            };

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, ServiceUrl + endpoint);
            HttpResponseMessage response = null;
            try {
                RequestLogger?.RequestSent(content.ToString());

                request.Content = content;
                response = await httpClient.SendAsync(request);

                string rawResponse = response.Content.ReadAsStringAsync().Result;
                RequestLogger?.ResponseReceived(rawResponse);

                return new GatewayResponse {
                    StatusCode = response.StatusCode,
                    RequestUrl = response.RequestMessage.RequestUri.ToString(),
                    RawResponse = rawResponse
                };
            }
            catch (Exception exc) {
                throw new GatewayException("Error occurred while communicating with gateway.", exc);
            }
            finally { }
        }
        protected GatewayResponse SendRequest(string endpoint, MultipartFormDataContent content) {
            HttpClient httpClient = new HttpClient {
                Timeout = TimeSpan.FromMilliseconds(Timeout)
            };

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, ServiceUrl + endpoint);
            HttpResponseMessage response = null;
            try {
                RequestLogger?.RequestSent(content.ToString());

                request.Content = content;
                response = httpClient.SendAsync(request).Result;

                string rawResponse = response.Content.ReadAsStringAsync().Result;
                RequestLogger?.ResponseReceived(rawResponse);

                return new GatewayResponse {
                    StatusCode = response.StatusCode,
                    RequestUrl = response.RequestMessage.RequestUri.ToString(),
                    RawResponse = rawResponse
                };
            }
            catch (Exception exc) {
                throw new GatewayException("Error occurred while communicating with gateway.", exc);
            }
            finally { }
        }

        private string BuildQueryString(Dictionary<string, string> queryStringParams) {
            if (queryStringParams == null)
                return string.Empty;
            return string.Format("?{0}", string.Join("&", queryStringParams.Select(kvp => string.Format("{0}={1}", Uri.EscapeDataString(kvp.Key), Uri.EscapeDataString(kvp.Value)))));
        }
    }
}
