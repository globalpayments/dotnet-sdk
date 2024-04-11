using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Terminals.Messaging;
using GlobalPayments.Api.Utils;
using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using log4net;

namespace GlobalPayments.Api.Terminals.UPA
{
    internal class UpaTcpInterface : IDeviceCommInterface
    {

        private readonly ITerminalConfiguration _settings;
        private readonly ILog _logger = LogManager.GetLogger(typeof(UpaTcpInterface));
        private readonly CancellationTokenSource _tokenSource;

        public event MessageSentEventHandler OnMessageSent;
        public event MessageReceivedEventHandler OnMessageReceived;

        public UpaTcpInterface(ITerminalConfiguration settings)
        {
            _settings = settings;
            _tokenSource = new CancellationTokenSource();
        }

        public void Connect()
        {

        }

        public void Disconnect()
        {
            _tokenSource.Cancel();
        }

        public byte[] Send(IDeviceMessage deviceMessage)
        {
            var token = _tokenSource.Token;

            var requestId = deviceMessage.GetRequestBuilder().GetValue<JsonDoc>("data")?.GetValue<string>("requestId");

            var tcs = new TaskCompletionSource<bool>();

            var serverIsBusy = false;

            byte[] responseMessage = null;

            var buffer = deviceMessage.GetSendBuffer();

            var client = new TcpClientAsync
            {
                IpAddress = IPAddress.Parse(_settings.IpAddress),
                Port = int.Parse(_settings.Port),
                AutoReconnect = false,
                ConnectedCallback = async (c, isReconnected) =>
                {
                    await c.Send(new ArraySegment<byte>(buffer, 0, buffer.Length), token);

                    OnMessageSent?.Invoke(deviceMessage.ToString());

                    while (true)
                    {
                        await c.StreamBuffer.WaitAsync(token);
                        if (c.IsClosing)
                        {
                            break;
                        }
                    }
                },
                ReceivedCallback = async (c, count) =>
                {
                    var bytes = await c.StreamBuffer.DequeueAsync(count, token);

                    var rvalue = bytes.GetTerminalResponseAsync();
                    if (rvalue != null)
                    {
                        var msgValue = GetResponseMessageType(rvalue);
                        OnMessageSent?.Invoke($"Server Response: {msgValue}");
                        switch (msgValue)
                        {
                            case UpaMessageType.Ack:
                                break;
                            case UpaMessageType.Nak:
                                break;
                            case UpaMessageType.Ready:
                                if (serverIsBusy)
                                {
                                    await c.Send(new ArraySegment<byte>(buffer, 0, buffer.Length), token);
                                    OnMessageSent?.Invoke("Resending Request...");
                                    serverIsBusy = false;
                                }
                                break;
                            case UpaMessageType.Busy:
                                serverIsBusy = true;
                                break;
                            case UpaMessageType.TimeOut:
                                break;
                            case UpaMessageType.Msg:
                                responseMessage = TrimResponse(rvalue);
                                OnMessageSent?.Invoke($"Sending {UpaMessageType.Ack}...");
                                await SendAckMessageToDevice(c);
                                break;
                            case UpaMessageType.Error:
                                var errorJson = $@"{{""id"": ""{requestId}"",""status"": ""Error"",""action"": {{""result_code"": ""Unexpected error from terminal.""}}}}";
                                responseMessage = Encoding.UTF8.GetBytes(errorJson);
                                OnMessageSent?.Invoke($"Sending {UpaMessageType.Ack}...");
                                await SendAckMessageToDevice(c);
                                break;
                            default:
                                throw new Exception("Message field value is unknown in API response.");
                        }
                    }

                    if (responseMessage != null)
                    {
                        OnMessageReceived?.Invoke(Encoding.UTF8.GetString(responseMessage));
                        c.Disconnect();
                    }
                },
                ClosedCallback = (c, r) => { tcs.SetResult(true); }
            };

            client.Message += ClientOnMessage();

            var t = Task.WhenAny(client.RunAsync(), tcs.Task);

            t.Wait(token);

            client.Message -= ClientOnMessage();

            client.Dispose();

            return responseMessage;

            EventHandler<MessageEventArgs> ClientOnMessage()
            {
                return (s, a) =>
                {
                    if (a.Exception == null)
                    {
                        _logger.Debug($"Tcp Client: {a.Message}");
                    }
                    else
                    {
                        _logger.Error($"Tcp Client: {a.Message}", a.Exception);
                    }
                };
            }
        }

        private static byte[] TrimResponse(byte[] value)
        {
            var json = Encoding.UTF8.GetString(value)
                .TrimStart((char)ControlCodes.STX, (char)ControlCodes.LF)
                .TrimEnd((char)ControlCodes.LF, (char)ControlCodes.ETX);

            return Encoding.UTF8.GetBytes(json);
        }

        private string GetResponseMessageType(byte[] response)
        {
            var jsonObject = Encoding.UTF8.GetString(TrimResponse(response));
            try
            {
                var jsonDoc = JsonDoc.Parse(jsonObject);
                return jsonDoc.GetValue<string>("message");
            }
            catch (Exception ex)
            {
                _logger.Error($"{ex.Message} : Response - {jsonObject}");
                return UpaMessageType.Error;
            }
        }

        private static Task SendAckMessageToDevice(TcpClientAsync c)
        {
            var doc = new JsonDoc();
            doc.Set("message", UpaMessageType.Ack);

            var message = TerminalUtilities.BuildUpaRequest(doc.ToString());
            var ackBuffer = message.GetSendBuffer();

            return c.Send(new ArraySegment<byte>(ackBuffer, 0, ackBuffer.Length));
        }

        private static bool IsNonReadyResponse(byte[] responseMessage)
        {
            var responseMessageString = Encoding.UTF8.GetString(responseMessage);
            var response = JsonDoc.Parse(responseMessageString);
            var data = response.Get("data");
            if (data == null)
            {
                return false;
            }

            var cmdResult = data.Get("cmdResult");
            return (
                data.GetValue<string>("response") == UpaTransType.Reboot ||
                (cmdResult != null && cmdResult.GetValue<string>("result") == "Failed")
            );
        }
    }
}
