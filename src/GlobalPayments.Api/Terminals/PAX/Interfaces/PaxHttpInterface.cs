using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Terminals.Messaging;

namespace GlobalPayments.Api.Terminals.PAX {
    internal class PaxHttpInterface : IDeviceCommInterface {
        ITerminalConfiguration _settings;
        WebRequest _client;

        public event MessageSentEventHandler OnMessageSent;

        public PaxHttpInterface(ITerminalConfiguration settings) {
            this._settings = settings;
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
                string payload = Convert.ToBase64String(message.GetSendBuffer());

                _client = HttpWebRequest.Create(string.Format("http://{0}:{1}?{2}", _settings.IpAddress, _settings.Port, payload));
                var response = _client.GetResponseAsync().Result;

                var buffer = new List<byte>();
                using (var sr = new StreamReader(response.GetResponseStream())) {
                    var rec_buffer = sr.ReadToEndAsync().Result;
                    foreach (char c in rec_buffer)
                        buffer.Add((byte)c);
                }
                return buffer.ToArray();
            }
            catch (Exception exc) {
                throw new MessageException("Failed to send message. Check inner exception for more details.", exc);
            }
        }
    }
}
