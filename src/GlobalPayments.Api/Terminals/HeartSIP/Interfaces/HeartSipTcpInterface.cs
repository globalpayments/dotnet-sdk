using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Terminals.Messaging;
using GlobalPayments.Api.Terminals.Extensions;
using GlobalPayments.Api.Utils;
using System.Diagnostics;

namespace GlobalPayments.Api.Terminals.HeartSIP.Interfaces {
    internal class HeartSipTcpInterface : IDeviceCommInterface {
        TcpClient _client;
        NetworkStream _stream;
        ITerminalConfiguration _settings;
        AutoResetEvent _await;
        List<byte> message_queue;
        Thread _recieveThread;

        public event MessageSentEventHandler OnMessageSent;
        public event MessageReceivedEventHandler OnMessageReceived;

        public HeartSipTcpInterface(ITerminalConfiguration settings) {
            this._settings = settings;
            this._await = new AutoResetEvent(false);

            OnMessageReceived += MessageReceived;
        }

        private void BeginReceiveThread() {
            new Thread(() => {
                while (true) {
                    try {
                        if (_stream == null)
                            break;

                        if (_stream.DataAvailable) {
                            do {
                                var length = _stream.GetLength();
                                if (length > 0) {
                                    byte[] buffer = new byte[8192];

                                    var incomplete = true;
                                    int offset = 0;
                                    int temp_length = length;
                                    do {
                                        int bytesReceived = _stream.Read(buffer, offset, temp_length);
                                        if (bytesReceived != temp_length) {
                                            offset += bytesReceived;
                                            temp_length -= bytesReceived;
                                            Thread.Sleep(10);
                                        }
                                        else incomplete = false;
                                    } while (incomplete);

                                    var readBuffer = new byte[length];
                                    Array.Copy(buffer, readBuffer, length);

                                    OnMessageReceived?.Invoke(readBuffer);
                                }
                                else break;
                            } while (true);
                        }
                    }
                    catch (Exception exc) {
                        // This never needs to fail
                        var str = exc.Message;
                    }
                    Thread.Sleep(300);
                }
            }).Start();
        }

        public void Connect() {
            if (_client == null) {
                _client = new TcpClient();
                _client.ConnectAsync(_settings.IpAddress, int.Parse(_settings.Port)).Wait(_settings.TimeOut);
                _stream = _client.GetStream();
                _stream.ReadTimeout = 60000;

                BeginReceiveThread(); // start listening
            }
        }

        public void Disconnect() {
            if (_stream != null) {
                _stream.Dispose();
                _stream = null;
            }

            if (_client != null) {
                _client.Dispose();
                _client = null;
            }
            message_queue = null;
        }

        public byte[] Send(IDeviceMessage message) {
            Connect();

            var str_message = message.ToString();
            message_queue = new List<byte>();
            try {
                byte[] buffer = message.GetSendBuffer();
                OnMessageSent?.Invoke(message.ToString().Substring(2));

                if (_stream != null) {
                    _stream.Write(buffer, 0, buffer.Length);
                    _stream.Flush();
                    _await.WaitOne(_settings.TimeOut);
                    if (message_queue.Count == 0)
                        throw new MessageException("Device did not respond within the timeout.");

                    return message_queue.ToArray();
                }
                else throw new MessageException("Device not connected.");
            }
            finally {
                Disconnect();
            }
        }

        private void MessageReceived(byte[] buffer) {
            if (message_queue == null)
                return;

            message_queue.AddRange(buffer);

            var msg = ElementTree.Parse(buffer).Get("SIP");
            var multiMessage = msg.GetValue<int>("MultipleMessage");
            var response = msg.GetValue<string>("Response");
            var text = msg.GetValue<string>("ResponseText");
            if (multiMessage == 0) {
                _await.Set();
            }
            else message_queue.Add((byte)'\r'); // Delimiting character
        }
    }
}
