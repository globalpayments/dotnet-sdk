using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using GlobalPayments.Api.Entities;
using System.Threading.Tasks;
using GlobalPayments.Api.Logging;
using System.Net;
using Newtonsoft.Json;
using GlobalPayments.Api.Utils;
using System.Xml.Linq;

namespace GlobalPayments.Api.Gateways {
    internal abstract class Gateway {
        private string _contentType;

        //public bool EnableLogging { get; set; }
        public IRequestLogger RequestLogger { get; set; }
        public IWebProxy WebProxy { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public int Timeout { get; set; }
        public string ServiceUrl { get; set; }

        public Dictionary<string, string> DynamicHeaders;
       
        public Entities.Environment Environment { get; set; }

        public Dictionary<string, string> MaskedRequestData;

        public Gateway(string contentType) {
            Headers = new Dictionary<string, string>();
            _contentType = contentType;
            DynamicHeaders = new Dictionary<string, string>();
            MaskedRequestData = new Dictionary<string, string>();
        }

        private string GenerateRequestLog(HttpRequestMessage request, bool isXml = false) {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{request.Method.ToString()} {request.RequestUri}");
            foreach (var header in request.Headers) {
                sb.AppendLine($"{header.Key}: {string.Join(", ", header.Value)}");
            }
            if (request.Content != null) {
                foreach (var header in request.Content.Headers) {
                    sb.AppendLine($"{header.Key}: {string.Join(", ", header.Value)}");
                }
                if (MaskedRequestData.Count > 0 && Environment == Entities.Environment.PRODUCTION) {
                    var data = request.Content.ReadAsStringAsync().Result;
                    sb.AppendLine(MaskSensitiveData(data, isXml));
                }
                else { sb.AppendLine(request.Content.ReadAsStringAsync().Result); }
                
            }
            return sb.ToString();
        }
                

        protected GatewayResponse SendRequest(HttpMethod verb, string endpoint, string data = null, Dictionary<string, string> queryStringParams = null, string contentType = null, bool isCharSet = true, bool isXml = false) {
            HttpClient httpClient = new HttpClient(HttpClientHandlerBuilder.Build(WebProxy)) {
                Timeout = TimeSpan.FromMilliseconds(Timeout)

            };

            var queryString = BuildQueryString(queryStringParams);
            HttpRequestMessage request = new HttpRequestMessage(verb, ServiceUrl + endpoint + queryString);
            foreach (var item in Headers) {
                request.Headers.Add(item.Key, item.Value);
            }

            if(DynamicHeaders != null) {
                foreach (var item in DynamicHeaders)
                {
                    request.Headers.Add(item.Key, item.Value);
                }
            }

            HttpResponseMessage response = null;
            try {
                if (verb != HttpMethod.Get && data != null) {
                    if (isCharSet) {
                        request.Content = new StringContent(data, Encoding.UTF8, contentType ?? _contentType);
                    }
                    else {
                        var content = new StringContent(data, Encoding.UTF8, "application/json");
                        content.Headers.ContentType.CharSet = "";
                        request.Content = content;
                    }
                }

                RequestLogger?.RequestSent(GenerateRequestLog(request, isXml));
                
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
            finally {         
            }
        }

        protected async Task<GatewayResponse> SendRequestAsync(string endpoint, MultipartFormDataContent content) {
            HttpClient httpClient = new HttpClient(HttpClientHandlerBuilder.Build(WebProxy)) {
                Timeout = TimeSpan.FromMilliseconds(Timeout)
            };

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, ServiceUrl + endpoint);
            HttpResponseMessage response = null;
            try {
                request.Content = content;

                RequestLogger?.RequestSent(GenerateRequestLog(request));

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
                request.Content = content;

                RequestLogger?.RequestSent(GenerateRequestLog(request));

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
            if (queryStringParams == null || queryStringParams.Count == 0)
                return string.Empty;
            return string.Format("?{0}", string.Join("&", queryStringParams.Select(kvp => string.Format("{0}={1}", Uri.EscapeDataString(kvp.Key), Uri.EscapeDataString(kvp.Value)))));
        }

        private string MaskSensitiveData(string data, bool isXml = false)
        {
            if (isXml) {
                var xml = XDocument.Parse(data);
                data = JsonConvert.SerializeXNode(xml);
            }

            var dataParsed = JsonDoc.Parse(data);
            foreach (var maskedItem in MaskedRequestData) {
                var key = maskedItem.Key;
                var parts = key.Split('.');
                var value = maskedItem.Value;
                if (parts.Length > 1) {
                    var cont = 0;
                    JsonDoc valueToReplace = new JsonDoc();
                    for (int i = 0; i < parts.Length; i++) {
                        if (i < parts.Length - 1) {
                            var list = parts[i].Split(';');                            
                            if (cont == 0) {                                
                                valueToReplace = list.Length > 1 ? dataParsed?.GetArray<JsonDoc>(list[0]).FirstOrDefault() : dataParsed.Has(parts[i]) ?  dataParsed?.Get(parts[i]) : null;
                                cont++;
                            }
                            else {
                                valueToReplace = list.Length > 1 ? valueToReplace?.GetArray<JsonDoc>(list[0]).FirstOrDefault() : valueToReplace.Has(parts[i]) ?  valueToReplace?.Get(parts[i]) : null;
                            }
                        }
                        else {
                            if (valueToReplace != null && valueToReplace.Has(parts[i])) {
                                valueToReplace?.Remove(parts[i]);
                                valueToReplace?.Set(parts[i], value);
                            }
                        }
                    }
                }
                else {
                    if (dataParsed != null && dataParsed.Has(key)) {
                        dataParsed?.Remove(key);
                        dataParsed?.Set(key, value);
                    }
                }
            }

            if (isXml)  {
                var des = JsonConvert.DeserializeXNode(dataParsed.ToString());
                return des.ToString();
            }          
            
            return dataParsed.ToString();            
        }
    }
}
