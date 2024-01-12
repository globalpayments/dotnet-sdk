using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Gateways;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Terminals.Messaging;
using GlobalPayments.Api.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPayments.Api.Terminals.Genius.Interfaces {
    internal class GeniusHttpInterface : IDeviceCommInterface {
        private ITerminalConfiguration _settings;
        private GeniusConfig _gatewayConfig;

        public event MessageSentEventHandler OnMessageSent;

        public event MessageReceivedEventHandler OnMessageReceived;

        public GeniusHttpInterface(ITerminalConfiguration settings) {
            _settings = settings;
            _gatewayConfig = settings.GatewayConfig as GeniusConfig;
        }

        public void Connect() {
            // not required for this connection mode
        }

        public void Disconnect() {
            // not required for this connection mode
        }

        public byte[] Send(IDeviceMessage message) {
            OnMessageSent?.Invoke(message.ToString());

            try {
                return Task.Run(async () => {
                    GatewayResponse serviceResponse = await StageTransactionAsync(message);
                    if (serviceResponse.StatusCode == HttpStatusCode.OK) {
                        var root = ElementTree.Parse(serviceResponse.RawResponse).Get("CreateTransactionResponse");

                        var errors = root.GetAll("Message");
                        if (errors.Length > 0) {
                            var sb = new StringBuilder();
                            foreach (var error in root.GetAll("Message")) {
                                sb.AppendLine(error.GetValue<string>("Information"));
                            }
                            throw new MessageException(sb.ToString());
                        }

                        string transportKey = root.GetValue<string>("TransportKey");
                        string validationKey = root.GetValue<string>("ValidationKey");

                        return await InitializeTransactionAsync(transportKey);
                    }
                    else throw new MessageException(serviceResponse.StatusCode.ToString());
                }).Result;
            }
            catch (Exception exc) {
                throw new MessageException("Failed to send message. Check inner exception for more details.", exc);
            }
        }

        private async Task<GatewayResponse> StageTransactionAsync(IDeviceMessage message) {
            try {
                string payload = Encoding.UTF8.GetString(message.GetSendBuffer());

                string url = ServiceEndpoints.GENIUS_TERMINAL_TEST;
                if (_gatewayConfig.Environment.Equals(Entities.Environment.PRODUCTION)) {
                    url = ServiceEndpoints.GENIUS_TERMINAL_PRODUCTION;
                }

                HttpClient httpClient = new HttpClient {
                    Timeout = TimeSpan.FromMilliseconds(_settings.Timeout)
                };

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Headers.Add("SOAPAction", "http://transport.merchantware.net/v4/CreateTransaction");

                HttpResponseMessage response = null;
                try {
                    request.Content = new StringContent(payload, Encoding.UTF8, "text/xml");
                    response = await httpClient.SendAsync(request);
                    return new GatewayResponse {
                        StatusCode = response.StatusCode,
                        RequestUrl = response.RequestMessage.RequestUri.ToString(),
                        RawResponse = response.Content.ReadAsStringAsync().Result
                    };
                }
                catch (Exception exc) {
                    throw new GatewayException("Error occurred while communicating with gateway.", exc);
                }
            }
            catch (Exception exc) {
                throw new MessageException("Failed to send message. Check inner exception for more details.", exc);
            }
        }

        private async Task<byte[]> InitializeTransactionAsync(string transportKey) {
            WebRequest client = HttpWebRequest.Create(string.Format("http://{0}:{1}?TransportKey={2}&Format=XML", _settings.IpAddress, _settings.Port, transportKey));

            var response = await client.GetResponseAsync();

            var buffer = new List<byte>();
            using (var sr = new StreamReader(response.GetResponseStream())) {
                var rec_buffer = await sr.ReadToEndAsync();
                foreach (char c in rec_buffer) {
                    buffer.Add((byte)c);
                }
            }
            return buffer.ToArray();
        }
    }
}
