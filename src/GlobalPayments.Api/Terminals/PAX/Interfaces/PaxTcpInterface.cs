using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Terminals.Abstractions;

namespace GlobalPayments.Api.Terminals.PAX {
    delegate void MessageReceivedEventHandler(byte[] message);
    delegate void ControlCodeReceivedEventHandler(ControlCodes code);

    internal class PaxTcpInterface : IDeviceCommInterface {
        TcpClient _client;
        NetworkStream _stream;
        ITerminalConfiguration _settings;
        AutoResetEvent _await;
        int _nakCount = 0;
        
        public event MessageSentEventHandler OnMessageSent;
        public event MessageReceivedEventHandler OnMessageReceived;
        public event ControlCodeReceivedEventHandler OnControlCodeReceived;

        public PaxTcpInterface(ITerminalConfiguration settings) {
            this._settings = settings;
            this._await = new AutoResetEvent(false);

            new Thread(() => {
                while (true) {
                    try{
                        if (_stream.DataAvailable) {
                            byte[] buffer = new byte[4096];
                            int bytesReceived = _stream.Read(buffer, 0, buffer.Length);

                            byte[] readBuffer = new byte[bytesReceived];
                            Array.Copy(buffer, readBuffer, bytesReceived);

                            var queue = new Queue<byte>(readBuffer);
                            while (queue.Count > 0) {
                                var code = (ControlCodes)queue.Dequeue();
                                if (code == ControlCodes.ACK || code == ControlCodes.NAK || code == ControlCodes.EOT) {
                                    if (OnControlCodeReceived != null)
                                        OnControlCodeReceived(code);
                                }
                                else if (code == ControlCodes.STX) {
                                    var rec_buffer = new List<byte>() { (byte)code };
                                    while (true) {
                                        rec_buffer.Add(queue.Dequeue());
                                        if (rec_buffer[rec_buffer.Count - 1] == (byte)ControlCodes.ETX)
                                            break;
                                    }

                                    byte lrc = queue.Dequeue(); // Should be the LRC
                                    if (lrc != TerminalUtilities.CalculateLRC(rec_buffer.ToArray())) {
                                        SendControlCode(ControlCodes.NAK);
                                    }
                                    else {
                                        rec_buffer.Add(lrc);

                                        SendControlCode(ControlCodes.ACK);
                                        if (OnMessageReceived != null)
                                            OnMessageReceived(rec_buffer.ToArray());
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception exc) {
                            // This never needs to fail
                    }
                    Thread.Sleep(300);
                }
            }).Start();
        }

        public void Connect() {
            _client = new TcpClient();
            // TODO: Fix this later
            //_client.ConnectAsync(_settings.IpAddress, int.Parse(_settings.Port));
            _stream = _client.GetStream();
        }

        public void Disconnect() {
            if (_stream != null)
                _stream.Dispose();

            if (_client != null)
                _client.Dispose();
        }

        public byte[] Send(IDeviceMessage message) {
            byte[] buffer = message.GetSendBuffer();

            ControlCodes? code = null;
            OnControlCodeReceived += (rec_code) => {
                code = rec_code;
                _await.Set();
            };

            byte[] rvalue = null;
            OnMessageReceived += (rec_buffer) => {
                rvalue = rec_buffer;
                _await.Set();
            };

            for (int i = 0; i < 3; i++) {
                _stream.Write(buffer, 0, buffer.Length);
                if (OnMessageSent != null)
                    OnMessageSent(message.ToString());
                _await.WaitOne(1000);

                if (!code.HasValue)
                    throw new MessageException("Terminal did not respond in the given timeout.");

                if (code == ControlCodes.NAK) continue;
                else if (code == ControlCodes.EOT) throw new MessageException("Terminal returned EOT for the current message.");
                else if (code == ControlCodes.ACK) {
                    _await.WaitOne(_settings.TimeOut);
                    break;
                }
                else throw new MessageException(string.Format("Unknown message received: {0}", code));
            }
            return rvalue;
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
