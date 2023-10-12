using System;
using System.Net.Sockets;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Utils;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Terminals.Messaging;
using Newtonsoft.Json.Linq;
using System.Text;

namespace GlobalPayments.Api.Terminals.PAX {
    internal class PaxTcpInterface : IDeviceCommInterface {
        TcpClient _client;
        NetworkStream _stream;
        ITerminalConfiguration _settings;
        int _nakCount = 0;
        int _connectionCount = 0;
        
        public event MessageSentEventHandler OnMessageSent;

        public event MessageReceivedEventHandler OnMessageReceived;

        public PaxTcpInterface(ITerminalConfiguration settings) {
            _settings = settings;
        }

        public void Connect() { 
            if (_client == null) {
                _client = new TcpClient();
                _client.ConnectAsync(_settings.IpAddress, int.Parse(_settings.Port)).Wait(_settings.Timeout);
                _stream = _client.GetStream();
                _stream.ReadTimeout = _settings.Timeout;
            }
            _connectionCount++;
        }

        public void Disconnect() {
            if (_connectionCount > 0) {
                _connectionCount--;
            }
            if (_connectionCount == 0) {
                _stream?.Dispose();
                _stream = null;

                _client?.Dispose();
                _client = null;
            }
        }

        public byte[] Send(IDeviceMessage message) {
            byte[] buffer = message.GetSendBuffer();
            byte[] rvalue = null;

            try
            {
                Connect();
            
                for (int i = 0; i < 3; i++) {
                    _stream.WriteAsync(buffer, 0, buffer.Length).Wait();

                    rvalue = _stream.GetTerminalResponseAsync();
                    if (rvalue != null) {
                        byte lrc = rvalue[rvalue.Length - 1]; // Should be the LRC
                        Array.Resize(ref rvalue, rvalue.Length - 1);
                        if (lrc != TerminalUtilities.CalculateLRC(rvalue)) {
                            SendControlCode(ControlCodes.NAK);
                        }
                        else {
                            SendControlCode(ControlCodes.ACK);
                            Array.Resize(ref rvalue, rvalue.Length + 1);
                            rvalue[rvalue.Length - 1] = lrc;
                            return rvalue;
                        }
                    }
                    else {
                        // Reset the connection before the next attempt
                        Disconnect();
                        Connect();
                    }
                }
                throw new MessageException("Terminal did not respond in the given timeout.");
            }
            catch (Exception exc) {
                throw new MessageException(exc.Message, exc);
            }
            finally {
                OnMessageSent?.Invoke(message.ToString());
                if (rvalue != null)
                {
                    OnMessageReceived?.Invoke(Encoding.UTF8.GetString(rvalue, 0, rvalue.Length));
                }
                else
                {
                    OnMessageReceived?.Invoke("Terminal did not respond");
                }
                Disconnect();
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
