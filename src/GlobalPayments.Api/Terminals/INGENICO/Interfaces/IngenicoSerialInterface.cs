using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Terminals.Messaging;
using GlobalPayments.Api.Utils;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Management;

namespace GlobalPayments.Api.Terminals.Ingenico {
    internal class IngenicoSerialInterface : IDeviceCommInterface {
        private SerialPort serialPort;
        private ITerminalConfiguration settings;

        private bool transComplete;
        private bool isResult;
        private bool isAcknowledge;
        private bool isBroadcast;
        private bool isXML;

        private List<byte> messageResponse;
        private string bufferReceived = string.Empty;
        private StringBuilder report = new StringBuilder();

        public event MessageSentEventHandler OnMessageSent;
        public event BroadcastMessageEventHandler OnBroadcastMessage;

        public IngenicoSerialInterface(ITerminalConfiguration settings) {
            this.settings = settings;
            Connect();
        }

        public void Connect() {
            if (serialPort == null) {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(INGENICO_GLOBALS.MGMT_SCOPE, INGENICO_GLOBALS.MGMT_QUERY);

                string manufacturer = string.Empty;
                foreach (ManagementObject mgmtObject in searcher.Get()) {
                    manufacturer = mgmtObject["Manufacturer"].ToString().ToLower();
                    if (manufacturer.Equals("ingenico")) {
                        string caption = mgmtObject["Caption"].ToString();
                        string portName = "COM{0}".FormatWith(settings.Port);
                        if (caption.Equals(portName)) {
                            serialPort = new SerialPort() {
                                PortName = portName,
                                BaudRate = (int)settings.BaudRate,
                                DataBits = (int)settings.DataBits,
                                StopBits = settings.StopBits,
                                Parity = settings.Parity,
                                Handshake = settings.Handshake,
                                RtsEnable = true,
                                DtrEnable = true,
                                ReadTimeout = settings.Timeout
                            };
                        }
                    }
                }

                if (serialPort == null) {
                    throw new ConfigurationException("Can't connect to the terminal. Port not found.");
                }

                if (!serialPort.IsOpen) {
                    serialPort.DataReceived += new SerialDataReceivedEventHandler(Serial_DataReceived);
                    serialPort.Open();
                }
            } else {
                throw new ConfigurationException("Serial port is already open.");
            }
        }

        private void Serial_DataReceived(object sender, SerialDataReceivedEventArgs e) {
            SerialPort serial = (SerialPort)sender;
            do {
                Thread.Sleep(0100);
                bufferReceived = serial.ReadExisting();

                if (!string.IsNullOrEmpty(bufferReceived)) {
                    serialPort.ReadTimeout = settings.Timeout;

                    if (bufferReceived.Equals(INGENICO_RESP.ACKNOWLEDGE)) {
                        isAcknowledge = true;
                        break;
                    } else if (bufferReceived.Equals(INGENICO_RESP.ENQUIRY)) {
                        serialPort.Write(BitConverter.GetBytes((char)ControlCodes.ACK), 0, 1);
                        break;
                    } else if (bufferReceived.Contains(INGENICO_GLOBALS.BROADCAST)) {
                        isBroadcast = true;
                        break;
                    } else if (INGENICO_RESP.XML.Any(bufferReceived.Contains)) {
                        isXML = true;
                        break;
                    } else if (!bufferReceived.Contains(INGENICO_RESP.INVALID)
                        && !bufferReceived.Contains(INGENICO_GLOBALS.BROADCAST)
                        && !INGENICO_RESP.XML.Any(bufferReceived.Contains)) {
                        isResult = true;
                        break;
                    }
                }
            } while (true);
        }

        public void Disconnect() {
            serialPort.Close();
            serialPort?.Dispose();
            serialPort = null;
        }

        private bool ValidateResponseLRC(string calculate, string actual) {
            bool response = false;

            byte[] calculateLRC = TerminalUtilities.CalculateLRC(calculate);
            byte[] actualLRC = TerminalUtilities.CalculateLRC(actual);

            if (BitConverter.ToString(actualLRC) == BitConverter.ToString(calculateLRC)) {
                response = true;
            }

            return response;
        }

        private async Task<bool> WriteMessage(IDeviceMessage message) {
            return await Task.Run(() => {
                try {
                    int enquiryCount = 0;
                    messageResponse = new List<byte>();

                    if (serialPort == null) {
                        return false;
                    }

                    do {
                        serialPort.Write(BitConverter.GetBytes((char)ControlCodes.ENQ), 0, 1);
                        if (!isAcknowledge) {
                            Thread.Sleep(1000);
                            serialPort.Write(BitConverter.GetBytes((char)ControlCodes.EOT), 0, 1);
                            enquiryCount++;

                            if (enquiryCount.Equals(3)) {
                                throw new MessageException("Terminal did not respond in Enquiry for three (3) times. Send aborted.");
                            }
                        } else {
                            do {
                                byte[] msg = message.GetSendBuffer();
                                foreach (byte b in msg) {
                                    byte[] _b = new byte[] { b };
                                    serialPort.Write(_b, 0, 1);
                                }

                                if (isAcknowledge) {
                                    serialPort.Write(BitConverter.GetBytes((char)ControlCodes.EOT), 0, 1);
                                    isAcknowledge = false;
                                    break;
                                }
                            } while (true);

                            do {
                                Thread.Sleep(100);
                                if (isBroadcast) {
                                    byte[] bMsg = Encoding.ASCII.GetBytes(bufferReceived);
                                    BroadcastMessage broadcastMsg = new BroadcastMessage(bMsg);
                                    OnBroadcastMessage?.Invoke(broadcastMsg.Code, broadcastMsg.Message);
                                    isBroadcast = false;
                                }

                                if (isXML) {
                                    do {
                                        report.Append(bufferReceived);
                                        if (report.ToString().Contains(INGENICO_RESP.ENDXML)) {
                                            string xmlData = report.ToString().Substring(1, report.ToString().Length - 3);
                                            if (MessageReceived(xmlData)) {
                                                serialPort.Write(BitConverter.GetBytes((char)ControlCodes.ACK), 0, 1);
                                                isXML = false;
                                                transComplete = true;
                                            }
                                        }
                                        Thread.Sleep(0500);
                                    } while (!transComplete);
                                }

                                if (isResult) {
                                    string check = Encoding.UTF8.GetString(message.GetSendBuffer());
                                    if (bufferReceived.Contains(check.Substring(0, 2))) {
                                        do {
                                            string rData = bufferReceived.Substring(1, bufferReceived.Length - 3);
                                            bufferReceived = bufferReceived.Substring(1, bufferReceived.Length - 3);
                                            bool validateLRC = ValidateResponseLRC(rData, bufferReceived);
                                            if (validateLRC) {
                                                if (MessageReceived(rData)) {
                                                    serialPort.Write(BitConverter.GetBytes((char)ControlCodes.ACK), 0, 1);
                                                    isResult = false;
                                                    transComplete = true;
                                                }
                                            }
                                        } while (!transComplete);
                                    }
                                }
                            } while (!transComplete);

                            return transComplete;
                        }
                    } while (true);
                } catch (MessageException e) {
                    throw new MessageException(e.Message);
                }
            });
        }

        public byte[] Send(IDeviceMessage message) {
            try {
                if (serialPort != null) {
                    string bufferSend = Encoding.ASCII.GetString(message.GetSendBuffer());
                    OnMessageSent?.Invoke(bufferSend.Substring(1, bufferSend.Length - 3));
                    Task<bool> task = WriteMessage(message);
                    if (!task.Wait(settings.Timeout)) {
                        throw new MessageException("Terminal did not response within timeout.");
                    }

                    return messageResponse.ToArray();
                } else {
                    throw new MessageException("Terminal not connected.");
                }
            } finally {
                transComplete = false;
            }
        }

        private bool MessageReceived(string messageData) {
            if (messageResponse == null) {
                return false;
            }

            foreach (char b in messageData) {
                messageResponse.Add((byte)b);
            }

            return true;
        }
    }
}