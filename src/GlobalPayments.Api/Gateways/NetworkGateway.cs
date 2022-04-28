using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Gateways.Events;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Terminals.Extensions;
using GlobalPayments.Api.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;

namespace GlobalPayments.Api.Gateways {
    public class NetworkGateway {
        TcpClient client;
        SslStream sslStream;
        private int connectionFaults = 0;
        public string PrimaryEndpoint { get; set; }
        public int PrimaryPort { get; set; }
        public string SecondaryEndpoint { get; set; }
        public int SecondaryPort { get; set; }
        internal string currentEndpoint;
        public bool EnableLogging { get; set; }
        public bool ForceGatewayTimeout { get; set; }
        public int Timeout { get; set; }
        private string connectorName = "NetworkGateway";
        protected List<IGatewayEvent> events;      
        
        // establish connection
        private void Connect(string endpoint, int port) {
            currentEndpoint = endpoint.Equals(PrimaryEndpoint) ? "primary" : "secondary";

            // create the connection event
            ConnectionEvent connectionEvent = new ConnectionEvent(connectorName) {
                Endpoint = endpoint,
                Port = port.ToString(),
                Host = currentEndpoint,
                ConnectionAttempts = connectionFaults
            };
            if (client == null) {
                try {
                    connectionEvent.ConnectionStarted = DateTime.UtcNow;
                    try {
                        client = new TcpClient();
                        client.ConnectAsync(endpoint, port).Wait();                        
                        connectionEvent.SslNavigation = true;
                    }
                    catch (Exception) {
                        connectionEvent.SslNavigation = false;
                    }
                    if (client != null && client.Connected) {
                        // connection completed
                        connectionEvent.ConnectionCompleted = DateTime.UtcNow;
                        sslStream = new SslStream(
                            client.GetStream(),
                            false);
                        sslStream.AuthenticateAsClientAsync(endpoint, null, System.Security.Authentication.SslProtocols.Tls12, false).Wait(30000);
                        connectionFaults = 0;
                    }
                    else {
                        // connection fail over
                        connectionEvent.ConnectionFailOver = DateTime.UtcNow;
                        //events.add(connectionEvent);
                        if (connectionFaults++ != 3) {
                            if (endpoint.Equals(PrimaryEndpoint) && SecondaryEndpoint != null) {
                                client = null;
                                Connect(SecondaryEndpoint, SecondaryPort);
                            }
                            else {
                                client = null;
                                Connect(PrimaryEndpoint, PrimaryPort);
                            }
                        }
                        else {
                            throw new IOException("Failed to connect to primary or secondary processing endpoints.");
                        }
                    }
                }
                catch (Exception exc) {
                    GatewayException gatewayException = new GatewayException("Error occurred while communicating with gateway.", exc);
                    //gatewayException.setGatewayEvents(events);
                    throw gatewayException;
                }
            }
        }

        // close connection
        private void Disconnect() {
            try {
                if (sslStream != null) {
                    sslStream.Dispose();
                    sslStream = null;
                }
                if (client != null) {
                    client.Dispose();
                }
                client = null;
            }
            catch (IOException) {
                // eat the close exception
            }
        }

        internal byte[] Send(IDeviceMessage message) {
            byte[] buffer = message.GetSendBuffer();
            bool timedOut = false;
            Connect(PrimaryEndpoint, PrimaryPort);
            try {
                for (int i = 0; i < 2; i++) {
                    DateTime requestSent = DateTime.UtcNow;
                    if (client != null && client.Connected && sslStream.IsAuthenticated) {
                        sslStream.Write(buffer, 0, buffer.Length);
                        sslStream.Flush();
                    }
                    byte[] rvalue = GetGatewayResponse();
                    if (rvalue != null && !ForceGatewayTimeout) {
                        return rvalue;
                    }
                    // did not get a response, switch endpoints and try again
                    timedOut = true;
                    if (!currentEndpoint.Equals("secondary") && !string.IsNullOrEmpty(SecondaryEndpoint) && i < 1) {
                        Disconnect();
                        Connect(SecondaryEndpoint, SecondaryPort);
                    }
                }
                throw new GatewayTimeoutException();
            }
            catch (GatewayTimeoutException exc) {
                exc.GatewayEvents = events;
                throw exc;
            }
            catch (Exception exc) {
                if (timedOut) {
                    GatewayTimeoutException gatewayException = new GatewayTimeoutException(exc) {
                        GatewayEvents = events
                    };
                    throw gatewayException;
                }
                else {
                    GatewayException gatewayException = new GatewayException("Failed to connect to primary or secondary processing endpoints.", exc);
                    throw gatewayException;
                }
            }
            finally {
                Disconnect();
                // remove the force timeout
                if (ForceGatewayTimeout) {
                    ForceGatewayTimeout = false;
                }
            }
        }

        internal byte[] GetGatewayResponse() {
            int? bytesReceived = 0;
            byte[] buffer = new byte[2048];            
            int position = 0;            
            int messageLength = 0;
            var t = DateTime.Now;
            int length;
            do {
                if (messageLength == 0) {
                    byte[] lengthBuffer = new byte[2];
                    length = sslStream.ReadAsync(lengthBuffer, 0, lengthBuffer.Length).Result;
                    if (length == 2) {
                        messageLength = BitConverterX.ToInt16(lengthBuffer, 0) - 2;
                    }
                }
                if (messageLength != 0) {
                    int currLength = sslStream.ReadAsync(buffer, position, messageLength).Result;
                    if (currLength == messageLength) {
                        bytesReceived = messageLength;
                    }
                    else {
                        position += currLength;
                    }
                }
            } while ((DateTime.Now - t).TotalMilliseconds <= 20000 && bytesReceived == 0);
            if (bytesReceived > 0) {
                byte[] rec_buffer = new byte[(int)bytesReceived];
                Array.Copy(buffer, 0, rec_buffer, 0, (int)bytesReceived);
                return rec_buffer;
            }
            throw new GatewayTimeoutException();
        }        
    }
}