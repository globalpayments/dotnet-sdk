﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using GlobalPayments.Api.Utils;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Terminals.Messaging;
using System.Collections.Generic;
using GlobalPayments.Api.Entities;
using System.Threading;
using System.Threading.Tasks;
//using Serilog;

namespace GlobalPayments.Api.Terminals.INGENICO {
    internal class IngenicoTcpInterface : IDeviceCommInterface {
        private TcpClient _client;
        private NetworkStream _stream;
        private ITerminalConfiguration _settings;
        private TcpListenerEx _listener;
        private Socket _server;
        private List<IPAddress> _ipAddresses = new List<IPAddress>();
        private BroadcastMessage _broadcastMessage;
        private byte[] termResponse;
        private Thread dataReceiving;

        public event MessageSentEventHandler OnMessageSent;
        public event BroadcastMessageEventHandler OnBroadcastMessage;

        public IngenicoTcpInterface(ITerminalConfiguration settings) {
            _settings = settings;
            _client = new TcpClient();
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
                }
                else {
                    throw new ConfigurationException("Server already started.");
                }
            }
            catch (Exception ex) {
                throw new Exception(ex.Message);
            }
        }

        public void Disconnect() {
            try {
                if (_listener.Active) {

                    // Closing and disposing current clients
                    _client.Close();
                    _client.Dispose();

                    // Stopping server listening
                    _listener.Stop();

                    _ipAddresses.Clear();
                }
            }
            catch (Exception ex) {
                throw new Exception(ex.Message);
            }

        }

        public byte[] Send(IDeviceMessage message) {

            byte[] buffer = message.GetSendBuffer();
            termResponse = null;

            try {
                // Validate if server is starting
                if (!_listener.Active) {
                    throw new ConfigurationException("Server is not running.");
                }


                if (_ipAddresses.Count > 0) {
                    _stream.WriteAsync(buffer, 0, buffer.Length).Wait();
                    // Should be move to Finally block before deployment
                    OnMessageSent?.Invoke(Encoding.UTF8.GetString(RemoveHeader(buffer)));

                    while (termResponse == null) {
                        Thread.Sleep(100);
                        if (termResponse != null) {
                            return termResponse;
                        }
                    }
                    return null;
                }
                else
                    throw new ConfigurationException("No terminal connected to server.");
            }
            catch (Exception ex) {
                throw new Exception(ex.Message);
            }
        }

        #region Interface private Methods
        private void InitializeServer() {
            if (_listener == null) {
                int _port = INGENICO_GLOBALS.IP_PORT; // Default port.
                if (!string.IsNullOrWhiteSpace(_settings.Port)) {
                    if (!int.TryParse(_settings.Port, out _port))
                        throw new ConfigurationException("Invalid port number.");
                }

                _listener = new TcpListenerEx(IPAddress.Any, _port);

                // Set timeout for client to send data.
                _server = _listener.Server;
                _server.ReceiveTimeout = _settings.Timeout;
            }
            else {
                throw new ConfigurationException("Server already initialize.");
            }
        }

        private void AcceptingClient() {
            _client = _listener.AcceptTcpClient();
            _stream = _client.GetStream();
            _ipAddresses.Add(((IPEndPoint)_client.Client.RemoteEndPoint).Address);

            // Start thread for handling keep alive request.
            if (dataReceiving == null || dataReceiving.ThreadState != ThreadState.Running) {
                dataReceiving = new Thread(new ThreadStart(AnalyzeReceivedData));
                dataReceiving.Start();
            }
        }

        private bool isBroadcast(byte[] terminalResponse) {
            return Encoding.UTF8.GetString(terminalResponse).Contains(INGENICO_GLOBALS.BROADCAST);
        }

        private bool isCancel(byte[] buffer) {
            return Encoding.UTF8.GetString(buffer).Contains(INGENICO_GLOBALS.CANCEL);
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
            }
            else {
                return null;
            }
        }

        private void AnalyzeReceivedData() {
            try {
                var headerBuffer = new byte[2];
                while (_stream.CanRead && _listener.Active && _client.Connected) {

                    _stream.ReadAsync(headerBuffer, 0, headerBuffer.Length).Wait();

                    var dataLength = Task.Run(() => TerminalUtilities.HeaderLength(headerBuffer)).Result;
                    byte[] dataBuffer = new byte[dataLength + 2];

                    // Read data
                    _stream.ReadAsync(dataBuffer, 0, dataBuffer.Length).Wait();

                    if (isBroadcast(dataBuffer)) {
                        _broadcastMessage = new BroadcastMessage(dataBuffer);
                        OnBroadcastMessage?.Invoke(_broadcastMessage.Code, _broadcastMessage.Message);
                    }
                    else if (isKeepAlive(dataBuffer) && INGENICO_GLOBALS.KeepAlive) {
                        var keepAliveRep = KeepAliveResponse(dataBuffer);
                        _stream.WriteAsync(keepAliveRep, 0, keepAliveRep.Length).Wait();
                    }
                    else { // Receiving request response data.
                        termResponse = dataBuffer;
                    }
                    headerBuffer = new byte[2];
                }
            }
            catch (Exception ex) {
                if (_stream.CanRead && _listener.Active && _client.Connected) {
                    throw new ApiException("Unable to get data from terminal. " + ex.Message);
                }
            }
        }
        #endregion
    }
}