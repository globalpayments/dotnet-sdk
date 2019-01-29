using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Utils;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Terminals.Messaging;

namespace GlobalPayments.Api.Terminals.PAX {
    internal class PaxTcpInterface : IDeviceCommInterface {
        TcpClient _client;
        NetworkStream _stream;
        ITerminalConfiguration _settings;
        AutoResetEvent _await;
        int _nakCount = 0;
        
        public event MessageSentEventHandler OnMessageSent;

        public PaxTcpInterface(ITerminalConfiguration settings) {
            _settings = settings;
            _await = new AutoResetEvent(false);
        }

        public void Connect() { 
            if (_client == null) {
                _client = new TcpClient();
                _client.ConnectAsync(_settings.IpAddress, int.Parse(_settings.Port)).Wait(_settings.Timeout);
                _stream = _client.GetStream();
                _stream.ReadTimeout = _settings.Timeout;
            }
        }

        public void Disconnect() {
            _stream?.Dispose();
            _stream = null;

            _client?.Dispose();
            _client = null;
        }

        public byte[] Send(IDeviceMessage message) {
            Connect();

            using (_stream) {
                byte[] buffer = message.GetSendBuffer();

                try {
                    OnMessageSent?.Invoke(message.ToString());
                    for (int i = 0; i < 3; i++) {
                        _stream.WriteAsync(buffer, 0, buffer.Length).Wait();

                        var rvalue = _stream.GetTerminalResponseAsync();
                        if (rvalue != null) {
                            byte lrc = rvalue[rvalue.Length - 1]; // Should be the LRC
                            if (lrc != TerminalUtilities.CalculateLRC(rvalue)) {
                                SendControlCode(ControlCodes.NAK);
                            }
                            else {
                                SendControlCode(ControlCodes.ACK);
                                return rvalue;
                            }
                        }
                    }
                    throw new MessageException("Terminal did not respond in the given timeout.");
                }
                catch (Exception exc) {
                    throw new MessageException(exc.Message, exc);
                }
                finally {
                    Disconnect();
                }
            }
        }

        private void SendControlCode(ControlCodes code) {
            if (code != ControlCodes.NAK) {
                _nakCount = 0;
                _stream.Write(new byte[] { (byte)code }, 0, 1);
            }
            else if (++_nakCount == 3)
                SendControlCode(ControlCodes.EOT);
        }
    }
}
