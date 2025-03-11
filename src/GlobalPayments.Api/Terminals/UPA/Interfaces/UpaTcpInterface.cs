using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Terminals.Messaging;
using GlobalPayments.Api.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPayments.Api.Terminals.UPA {
    internal class UpaTcpInterface : IDeviceCommInterface {
        TcpClient _client;
        NetworkStream _stream;
        ITerminalConfiguration _settings;
        int _connectionCount = 0;
        public event MessageSentEventHandler OnMessageSent;
        public event MessageReceivedEventHandler OnMessageReceived;

        public UpaTcpInterface(ITerminalConfiguration settings) {
            _settings = settings;
        }

        public void Connect() {
            int connectionTimestamp = Int32.Parse(DateTime.Now.ToString("mmssfff"));

            if (_client == null) {
                try {
                    _client = new TcpClient();
                    _client.ConnectAsync(_settings.IpAddress, int.Parse(_settings.Port)).Wait(_settings.Timeout);
                }
                catch(Exception exc) {
                    throw new MessageException(exc.Message);
                }
                if (Int32.Parse(DateTime.Now.ToString("mmssfff")) > connectionTimestamp + _settings.Timeout) {
                    throw new MessageException("Connection not established within the specified timeout.");
                }
                try {
                    _stream = _client.GetStream();
                    _stream.ReadTimeout = _settings.Timeout;
                }
                catch (Exception ex) {
                    throw new MessageException(ex.Message);
                }
            }
        }

        public void Disconnect() {
            try {
                try {
                    _stream?.Dispose();
                    _stream = null;
                }
                catch { }
                _client?.Dispose();
                _client = null;
            }
            catch { }
        }

        public byte[] Send(IDeviceMessage message) {
            byte[] buffer = message.GetSendBuffer();
            bool readyReceived = false;
            byte[] responseMessage = null;
            int resCount = 0;
            try {
                Connect();

                var task = _stream.WriteAsync(buffer, 0, buffer.Length);
                
                if (!task.Wait(_settings.Timeout)) {
                    throw new MessageException("Terminal did not respond in the given timeout.");
                }
                try {
                    OnMessageSent?.Invoke(message.ToString());
                }
                catch { }
                do {
                    List<byte[]> responses = GetTerminalResponses(_stream);
                    if (responses != null) {
                        if (responses.Count > 1) {
                            resCount = responses.Count;
                        }
                        foreach (byte[] messageBytes in responses) {
                            string msgValue = GetResponseMessageType(messageBytes);
                            try {
                                OnMessageReceived?.Invoke(msgValue);
                            }
                            catch(Exception exc) {
                                throw new MessageException(exc.Message, exc);
                            }
                            switch (msgValue) {
                                case UpaMessageType.Ack:
                                case UpaMessageType.Nak:
                                case UpaMessageType.TimeOut:
                                    break;
                                case UpaMessageType.Busy:
                                    throw new Exception("Device is Busy.");
                                case UpaMessageType.Ready:
                                    readyReceived = true;
                                    break;
                                case UpaMessageType.Msg:
                                    responseMessage = TrimResponse(messageBytes);
                                    if (IsNonReadyResponse(responseMessage)) {
                                        readyReceived = true; // since reboot doesn't return READY
                                    }
                                    SendAckMessageToDevice();
                                    break;
                                default:
                                    throw new Exception("Message field value is unknown in API response.");
                            }
                        }
                    }
                } while (!readyReceived);

                return responseMessage;
            }
            catch (Exception exc) {
                _settings.LogManagementProvider?.ResponseReceived(exc.Message);
                throw new MessageException(exc.Message, exc);
            }
            finally {
                _settings.LogManagementProvider?.RequestSent(GenerateRequestLog(_settings.IpAddress, _settings.Port, _settings.Timeout.ToString(), Encoding.UTF8.GetString(buffer)));
                if (responseMessage != null) {
                    _settings.LogManagementProvider?.ResponseReceived(Encoding.UTF8.GetString(responseMessage));
                }
                Disconnect();
            }
        }

        private int ReadWithTimeout(Stream stream, byte[] buffer) {
            var readTask = stream.ReadAsync(buffer, 0, buffer.Length);
            if (Task.WhenAny(readTask, Task.Delay(_settings.Timeout)).Result == readTask) {
                return readTask.Result;
            }
            else {
                throw new MessageException("Terminal stream read did not respond in the given timeout.");
            }
        }

        private List<byte[]> GetTerminalResponses(NetworkStream stream) {
            byte[] buffer = new byte[32768];
            
            int bytesReceived = ReadWithTimeout(stream, buffer);

            List<byte[]> responses = new List<byte[]>();
            List<byte> tempBuffer = new List<byte>();
            bool inMessage = false;
            if (bytesReceived > 0) {
                byte[] readBuffer = new byte[bytesReceived];
                Array.Copy(buffer, readBuffer, bytesReceived);

                ControlCodes code = (ControlCodes)readBuffer[0];
               if (code == ControlCodes.STX) {
                    for (int i = 0; i < bytesReceived; i++) {
                        byte b = buffer[i];
                        if (b == (byte)ControlCodes.STX) {
                            tempBuffer.Clear();
                            inMessage = true;
                        }
                        else if (b == (byte)ControlCodes.ETX) {
                            if (inMessage) {
                                responses.Add(tempBuffer.ToArray());
                                tempBuffer.Clear();
                                inMessage = false;
                            }
                        }
                        else if (b != 0x0A && inMessage) {
                            tempBuffer.Add(b);
                        }
                    }
                    return responses;
                }
                else throw new MessageException(string.Format("Unknown message received: {0}", code));
            }
            return null;
        }

        private byte[] TrimResponse(byte[] value) {
            return System.Text.Encoding.UTF8.GetBytes(
                System.Text.Encoding.UTF8.GetString(value)
                    .TrimStart((char)ControlCodes.STX, (char)ControlCodes.LF)
                    .TrimEnd((char)ControlCodes.LF, (char)ControlCodes.ETX));
        }

        private string GetResponseMessageType(byte[] response) {
            var jsonObject = System.Text.Encoding.UTF8.GetString(TrimResponse(response));
            var jsonDoc = JsonDoc.Parse(jsonObject);
            return jsonDoc.GetValue<string>("message");
        }

        private void SendAckMessageToDevice() {
            var doc = new JsonDoc();
            doc.Set("message", UpaMessageType.Ack);

            var message = TerminalUtilities.BuildUpaRequest(doc.ToString());
            var ackBuffer = message.GetSendBuffer();

            _stream.Write(ackBuffer, 0, ackBuffer.Length);
            try
            {
                OnMessageSent?.Invoke(message.ToString());
            }
            catch { }
        }

        private bool IsNonReadyResponse(byte[] responseMessage) {
            var responseMessageString = System.Text.Encoding.UTF8.GetString(responseMessage);
            var response = JsonDoc.Parse(responseMessageString);
            var data = response.Get("data");
            if (data == null) {
                return false;
            }
            var cmdResult = data.Get("cmdResult");
            return (
                (data.GetValue<string>("response") == UpaTransType.SetParam && cmdResult.GetValue<string>("result") == "Success") ||
                    data.GetValue<string>("response") == UpaTransType.Reboot ||
                   data.GetValue<string>("response") == UpaTransType.Restart || (cmdResult != null && cmdResult.GetValue<string>("result") == "Failed"));
        }

        private string GenerateRequestLog(string ipAddress, string port, string timeout, string request) {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Ip: {ipAddress}");
            sb.AppendLine($"port: {port}");
            sb.AppendLine($"timeout: {timeout}");
            sb.AppendLine(request);

            return sb.ToString();
        }
    }
}
