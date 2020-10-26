using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Terminals.Ingenico.Requests;
using GlobalPayments.Api.Terminals.Messaging;
using GlobalPayments.Api.Utils;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GlobalPayments.Api.Terminals.Ingenico {
    internal class IngenicoTcpInterface : IDeviceCommInterface {
        private volatile TcpClient _client;
        private NetworkStream _stream;
        private ITerminalConfiguration _settings;
        private TcpListenerEx _listener;
        private Socket _server;
        private List<IPAddress> _ipAddresses = new List<IPAddress>();
        private BroadcastMessage _broadcastMessage;
        private byte[] _termResponse;
        private Thread _dataReceiving;
        private bool _isKeepAlive;
        private bool _isKeepAliveRunning;
        private Exception _receivingException;
        private bool _isResponseNeeded;
        private volatile bool _readData;
        private volatile bool _disposable;
        private volatile bool _bufferSend;

        public event MessageSentEventHandler OnMessageSent;
        public event BroadcastMessageEventHandler OnBroadcastMessage;
        public event PayAtTableRequestEventHandler OnPayAtTableRequest;

        public IngenicoTcpInterface(ITerminalConfiguration settings) {
            _settings = settings;
            _client = new TcpClient(); ;
            _ipAddresses = new List<IPAddress>();

            InitializeServer();

            // Start listening to port.
            Connect();

            // Accepting client connected to port.
            AcceptingClient();
        }

        public void Connect() {
            try {
                if (!_listener.Active) {
                    _listener.Start();
                } else {
                    throw new ConfigurationException("Server already started.");
                }
            } catch (SocketException ex) {
                throw new ConfigurationException(ex.Message);
            }
        }

        public void Disconnect() {
            try {
                if (_listener.Active) {
                    _readData = false;
                    _stream.Close();

                    // Closing and disposing current clients
                    if (!_settings.ConnectionMode.Equals(ConnectionModes.PAY_AT_TABLE)) {
                        while (true) {
                            if (_disposable) {
                                _client.Close();
                                _client.Dispose();
                                break;
                            }
                        }
                    } else {
                        _client.Close();
                        _client.Dispose();
                    }

                    // Stopping server listening
                    _listener.Stop();
                    _ipAddresses.Clear();
                }
            } catch (Exception ex) {
                throw new ConfigurationException(ex.Message);
            }
        }

        public byte[] Send(IDeviceMessage message) {
            byte[] buffer = message.GetSendBuffer();
            _termResponse = null;
            _bufferSend = false;
            _isResponseNeeded = true;

            try {
                // Validate if server is starting
                if (!_listener.Active) {
                    throw new ConfigurationException("Server is not running.");
                }

                // Validate keep alive for setting of timeout during Transaction
                _stream.ReadTimeout = _settings.Timeout;

                if (_ipAddresses.Count > 0 || _client.Connected) {
                    _stream.WriteAsync(buffer, 0, buffer.Length).Wait();
                    _bufferSend = true;

                    if (_settings.ConnectionMode.Equals(ConnectionModes.PAY_AT_TABLE)) {
                        string data = Encoding.UTF8.GetString(buffer);
                        OnMessageSent?.Invoke(data.Substring(1, data.Length - 3));

                        return null;
                    }

                    OnMessageSent?.Invoke(Encoding.UTF8.GetString(RemoveHeader(buffer)));
                    while (_termResponse == null) {
                        Thread.Sleep(100);
                        if (_receivingException != null) {
                            Exception ex = _receivingException;
                            _receivingException = null;
                            throw ex;
                        }

                        if (_termResponse != null) {
                            // Remove timeout for stream  read
                            if (!_isKeepAlive) {
                                _stream.ReadTimeout = -1;
                            }

                            _isResponseNeeded = false;
                            _receivingException = null;
                        }
                    }

                    return _termResponse;
                } else {
                    throw new ConfigurationException("No terminal connected to server.");
                }
            } catch (Exception ex) {
                throw new ApiException(ex.Message);
            }
        }

        #region Interface private Methods
        private void InitializeServer() {
            try {
                if (_listener == null) {
                    int _port = INGENICO_GLOBALS.IP_PORT; // Default port.
                    if (!string.IsNullOrWhiteSpace(_settings.Port)) {
                        if (!int.TryParse(_settings.Port, out _port)) {
                            throw new ConfigurationException("Invalid port number.");
                        }
                    }

                    _listener = new TcpListenerEx(IPAddress.Any, _port);

                    // Set timeout for client to send data.
                    _server = _listener.Server;

                    // Initialize keep Alive value to false.
                    _isKeepAlive = false;
                    _isKeepAliveRunning = false;

                    _readData = true;
                    _disposable = false;
                } else {
                    throw new ConfigurationException("Server already initialize.");
                }
            } catch (Exception e) {
                throw new ConfigurationException(e.Message);
            }
        }

        private void AcceptingClient() {
            try {
                _client = _listener.AcceptTcpClient();
                _stream = _client.GetStream();
                _ipAddresses.Add(((IPEndPoint)_client.Client.RemoteEndPoint).Address);

                if (_settings.ConnectionMode != ConnectionModes.PAY_AT_TABLE) {
                    if (_settings.Timeout <= 0) {
                        _settings.Timeout = Timeout.Infinite;
                    }

                    _stream.ReadTimeout = _settings.Timeout;
                }
                
                // Start thread for handling keep alive request.
                if (_dataReceiving == null || _dataReceiving.ThreadState != ThreadState.Running) {
                    _dataReceiving = new Thread(new ThreadStart(AnalyzeReceivedData));
                    _dataReceiving.Start();
                }
            } catch (Exception e) {
                throw new ConfigurationException(e.Message);
            }
        }

        private bool isBroadcast(byte[] terminalResponse) {
            return Encoding.UTF8.GetString(terminalResponse).Contains(INGENICO_GLOBALS.BROADCAST);
        }

        private bool isKeepAlive(byte[] buffer) {
            return Encoding.UTF8.GetString(buffer).Contains(INGENICO_GLOBALS.TID_CODE);
        }

        private byte[] RemoveHeader(byte[] buffer) {
            return buffer.SubArray(2, buffer.Length - 2);
        }

        private byte[] KeepAliveResponse(byte[] buffer) {
            if (buffer.Length > 0) {
                var tIdIndex = Encoding.ASCII.GetString(buffer, 0, buffer.Length).IndexOf(INGENICO_GLOBALS.TID_CODE);
                var tId = Encoding.ASCII.GetString(buffer, tIdIndex + 10, 8);

                var respData = INGENICO_GLOBALS.KEEP_ALIVE_RESPONSE.FormatWith(tId);
                respData = TerminalUtilities.CalculateHeader(Encoding.ASCII.GetBytes(respData)) + respData;
                return Encoding.ASCII.GetBytes(respData);
            } else {
                return null;
            }
        }

        private async void AnalyzeReceivedData() {
            try {
                var headerBuffer = new byte[2];
                while (_readData) {
                    // Pay@Table functionalities handling.
                    if (_settings.ConnectionMode.Equals(ConnectionModes.PAY_AT_TABLE)) {
                        byte[] buffer = new byte[8192];
                        int bytesReceived = _stream.ReadAsync(buffer, 0, buffer.Length).Result;

                        if (bytesReceived > 0) {
                            byte[] readBuffer = buffer.SubArray(0, bytesReceived);
                            string raw = Encoding.UTF8.GetString(readBuffer);
                            string dataETX = raw.Substring(1, raw.Length - 2);

                            byte[] bufferLRC = TerminalUtilities.CalculateLRC(raw.Substring(raw.Length - 1));
                            string receivedLRC = BitConverter.ToString(bufferLRC);

                            byte[] calculateLRC = TerminalUtilities.CalculateLRC(dataETX);
                            string calculatedLRC = BitConverter.ToString(calculateLRC);

                            if (receivedLRC.Equals(calculatedLRC)) {
                                string data = raw.Substring(1, raw.Length - 3);
                                byte[] patRequest = Encoding.UTF8.GetBytes(data);
                                OnPayAtTableRequest?.Invoke(new PATRequest(patRequest));
                            }
                        }
                    } else {
                        // Standard functionalities handling
                        _stream.Read(headerBuffer, 0, headerBuffer.Length);

                        if (!_readData) {
                            throw new Exception();
                        }

                        int dataLength = await Task.Run(() => TerminalUtilities.HeaderLength(headerBuffer));
                        if (dataLength > 0) {
                            byte[] dataBuffer = new byte[dataLength];

                            var incomplete = true;
                            int offset = 0;
                            int tempLength = dataLength;

                            do {

                                // Read data
                                int bytesReceived = _stream.Read(dataBuffer, offset, tempLength);
                                if (!_readData) {
                                    throw new Exception();
                                }
                                if (bytesReceived != tempLength) {
                                    offset += bytesReceived;
                                    tempLength -= bytesReceived;
                                } else {
                                    incomplete = false;
                                }
                            } while (incomplete);

                            var readBuffer = new byte[dataLength];
                            Array.Copy(dataBuffer, readBuffer, dataLength);

                            if (isBroadcast(readBuffer)) {
                                _broadcastMessage = new BroadcastMessage(readBuffer);
                                OnBroadcastMessage?.Invoke(_broadcastMessage.Code, _broadcastMessage.Message);
                            } else if (isKeepAlive(readBuffer) && INGENICO_GLOBALS.KeepAlive) {

                                _isKeepAlive = true;

                                if (_isKeepAlive && !_isKeepAliveRunning) {
                                    _stream.ReadTimeout = _settings.Timeout;
                                    _isKeepAliveRunning = true;
                                }

                                var keepAliveRep = KeepAliveResponse(readBuffer);
                                _stream.WriteAsync(keepAliveRep, 0, keepAliveRep.Length).Wait();
                            } else { // Receiving request response data.
                                _termResponse = readBuffer;
                            }
                        } else {
                            _receivingException = new ApiException("No data received.");
                        }
                    }
                }
            } catch (Exception ex) {
                if (_settings.ConnectionMode != ConnectionModes.PAY_AT_TABLE) {
                    if (_isResponseNeeded || _isKeepAlive) {
                        _receivingException = ex;
                        _stream.ReadTimeout = -1;
                        _isKeepAlive = false;
                    }

                    if (_readData) {
                        AnalyzeReceivedData();
                    } else {
                        _disposable = true;
                    }
                }
            }
        }
        #endregion
    }
}
