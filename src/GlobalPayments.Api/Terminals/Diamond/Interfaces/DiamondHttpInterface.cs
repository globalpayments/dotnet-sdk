using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Terminals.Messaging;
using GlobalPayments.Api.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;

namespace GlobalPayments.Api.Terminals.Diamond.Interfaces {
    internal class DiamondHttpInterface : IDeviceCommInterface {
        private DiamondCloudConfig _settings;
        protected Dictionary<string, string> Headers { get; set; }
        protected string ContentType = "application/json";
        private string AuthorizationId;

        public event MessageSentEventHandler OnMessageSent;
        public event MessageReceivedEventHandler OnMessageReceived;

        public DiamondHttpInterface(ITerminalConfiguration settings) {
            _settings = settings as DiamondCloudConfig;
        }

        public void Connect() {
            char[] stringArray = _settings.IsvID.ToCharArray();
            Array.Reverse(stringArray);
            string reversedStr = new string(stringArray);
            var data = reversedStr + AuthorizationId;
            var authorizationToken = GenerationUtils.HMACSHA256Hash(data, string.Concat(System.Linq.Enumerable.Repeat(_settings.SecretKey, 7)));            
            if (Headers == null) {
                Headers = new Dictionary<string, string>();
            }
            Headers["Authorization"] = $"Bearer {authorizationToken}";
        }

        public void Disconnect() {
            // TODO: Implement disconnect() method.
        }

        public byte[] Send(IDeviceMessage message) {
            OnMessageSent?.Invoke(message.ToString());

            var buffer = Encoding.UTF8.GetString(message.GetSendBuffer());
            var queryParams = JsonDoc.Parse(buffer).Get("queryParams") ?? null;
            if (string.IsNullOrEmpty(queryParams.GetValue<string>("cloud_id"))) {
                AuthorizationId = queryParams.GetValue<string>("POS_ID");
            }
            else {
                AuthorizationId = queryParams.GetValue<string>("cloud_id");
            }

            Connect();
            var queryString = BuildQueryString(queryParams);
            var data = JsonDoc.Parse(buffer).Get("body") ?? null;            
            var verbData =JsonDoc.Parse(buffer).Get("verb") ?? null;
            if (verbData == null) {
                throw new GatewayException("Payment type not supported!");
            }
            var verb = new HttpMethod(verbData.GetValue<string>("Method")) ?? HttpMethod.Post;
            Dictionary<string, string> mandatoryHeaders = new Dictionary<string, string>();           

            try {
                var url = _settings.ServiceUrl + JsonDoc.Parse(buffer).GetValue<string>("endpoint") + queryString;

                HttpClient httpClient = new HttpClient();

                HttpRequestMessage request = new HttpRequestMessage(verb, url);
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                foreach (var header in Headers) {
                    request.Headers.Add(header.Key, header.Value);
                }

                if (data != null) {
                    var content = new StringContent(data.ToString(), Encoding.UTF8, "application/json");
                    request.Content = content;
                }

                var response = httpClient.SendAsync(request); 
                if(response.Result.StatusCode != HttpStatusCode.OK) {
                    throw new ApiException($"ERROR: status code {response.Result.StatusCode.ToString()}");
                }
                var body = response.Result.Content.ReadAsStringAsync().Result;
                if (JsonDoc.IsJson(body)) {
                    var parsed = JsonDoc.Parse(body);
                    if (parsed.Has("status") && parsed.GetValue<string>("status") == "error") {
                        throw new GatewayException($"Status Code: {parsed.GetValue<string>("code")} - {parsed.GetValue<string>("message")}");
                    }
                    return Encoding.UTF8.GetBytes(body);
                }
                else {
                    return Encoding.UTF8.GetBytes(body);
                }                
            }
            catch (Exception e) {   
                throw new GatewayException($"Device {_settings.DeviceType} error: {e.Message}");
            }
        }

        private string BuildQueryString(JsonDoc queryStringParams = null) {
            if (queryStringParams == null) {
                return "";
            }
            List<string> query = new List<string>();
            foreach (var param in queryStringParams.Keys) {
                query.Add($"{param}={queryStringParams.GetValue<string>(param)}");
            }

            return string.Format("?{0}", string.Join("&", query.Select(x => x)));
        }

        private Dictionary<string, string> PrepareHeaders(Dictionary<string, string> mandatoryHeaders) {
            Headers = Headers.Concat(mandatoryHeaders).ToDictionary(x => x.Key, x => x.Value);
            return Headers;
        }
    }
}
