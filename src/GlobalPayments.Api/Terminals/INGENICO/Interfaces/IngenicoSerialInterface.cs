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

namespace GlobalPayments.Api.Terminals.INGENICO {
    internal class IngenicoSerialInterface : IDeviceCommInterface {
        public event MessageSentEventHandler OnMessageSent;
        public event BroadcastMessageEventHandler OnBroadcastMessage;

        ITerminalConfiguration _settings;

        private SerialPort _serial;
        private bool complete = false,
                     isResult = false,
                     isAcknowledge = false,
                     broadcast = false,
                     isXML = false;
        private string buffer = string.Empty;
        private string appendReport = string.Empty;
        private List<byte> messageResponse;

        public IngenicoSerialInterface(ITerminalConfiguration settings) {
            this._settings = settings;
        }

        public void Connect() {
            if (_serial == null) {
                _serial = new SerialPort() {
                    PortName = "COM{0}".FormatWith(_settings.Port),
                    BaudRate = (int)_settings.BaudRate,
                    DataBits = (int)_settings.DataBits,
                    StopBits = (System.IO.Ports.StopBits)_settings.StopBits,
                    Parity = (System.IO.Ports.Parity)_settings.Parity,
                    Handshake = (Handshake)_settings.Handshake,
                    RtsEnable = true,
                    DtrEnable = true,
                    ReadTimeout = _settings.Timeout
                };

                if (!_serial.IsOpen) {
                    _serial.DataReceived += new SerialDataReceivedEventHandler(_serial_DataReceived);
                    _serial.Open();
                } else throw new MessageException("Serial Port is already open.");
            }
        }

        private void _serial_DataReceived(object sender, SerialDataReceivedEventArgs e) {
            SerialPort serial = (SerialPort)sender;
            do {
                Thread.Sleep(0100);
                buffer = serial.ReadExisting();

                if (!string.IsNullOrEmpty(buffer)) {
                    _serial.ReadTimeout = _settings.Timeout;

                    if (buffer.Equals(INGENICO_RESP.ACKNOWLEDGE)) {
                        isAcknowledge = true;
                        break;
                    } else if (buffer.Equals(INGENICO_RESP.ENQUIRY)) {
                        _serial.Write(BitConverter.GetBytes((char)ControlCodes.ACK), 0, 1);
                        break;
                    } else if (buffer.Contains(INGENICO_GLOBALS.BROADCAST)) {
                        broadcast = true;
                        break;
                    } else if (buffer.Equals(INGENICO_RESP.NOTACKNOWLEDGE)) {
                        isAcknowledge = false;
                        break;
                    } else if (INGENICO_RESP.XML.Any(buffer.Contains)) {
                        isXML = true;
                        break;
                    } else {
                        if (!buffer.Contains(INGENICO_GLOBALS.BROADCAST) && !buffer.Contains(INGENICO_RESP.INVALID)
                            && !INGENICO_RESP.XML.Any(buffer.Contains) && !buffer.Contains(INGENICO_RESP.ENDOFTXN)
                            && !buffer.Contains(INGENICO_RESP.NOTACKNOWLEDGE))
                            isResult = true;
                        break;
                    }
                }
            } while (true);
        }

        public void Disconnect() {
            _serial?.Dispose();
            _serial.Close();
            _serial = null;
            buffer = string.Empty;
            appendReport = string.Empty;
            isResult = false;
            complete = false;
            isAcknowledge = false;
            broadcast = false;
            isXML = false;
        }

        private bool ValidateResponseLRC(string calculate, string actual) {
            bool response = false;

            byte[] calculateLRC = TerminalUtilities.CalculateLRC(calculate);
            byte[] actualLRC = TerminalUtilities.CalculateLRC(actual);

            if (BitConverter.ToString(actualLRC) == BitConverter.ToString(calculateLRC))
                response = true;

            return response;
        }

        private async Task<bool> WriteMessage(IDeviceMessage message) {
            return await Task.Run(() => {
                try {
                    int enquiryCount = 0;
                    messageResponse = new List<byte>();

                    if (_serial == null)
                        return false;

                    do {
                        _serial.Write(BitConverter.GetBytes((char)ControlCodes.ENQ), 0, 1);
                        if (isAcknowledge) {
                            do {
                                byte[] msg = message.GetSendBuffer();
                                foreach (byte b in msg) {
                                    byte[] _b = new byte[] { b };
                                    _serial.Write(_b, 0, 1);
                                }

                                if (isAcknowledge) {
                                    _serial.Write(BitConverter.GetBytes((char)ControlCodes.EOT), 0, 1);
                                    break;
                                }
                            } while (true);

                            do {
                                if (broadcast) {
                                    byte[] bMsg = Encoding.ASCII.GetBytes(buffer);
                                    BroadcastMessage broadcastMsg = new BroadcastMessage(bMsg);
                                    OnBroadcastMessage?.Invoke(broadcastMsg.Code, broadcastMsg.Message);
                                    broadcast = false;
                                }

                                if (isXML) {
                                    do {
                                        appendReport += buffer;
                                        if (appendReport.Contains(INGENICO_RESP.ENDXML)) {
                                            string xmlData = appendReport.Substring(1, appendReport.Length - 3);
                                            if (MessageReceived(xmlData)) {
                                                _serial.Write(BitConverter.GetBytes((char)ControlCodes.ACK), 0, 1);
                                                complete = true;
                                            }
                                        }
                                        Thread.Sleep(0500);
                                    } while (!complete);
                                }

                                if (isResult) {
                                    string check = Encoding.UTF8.GetString(message.GetSendBuffer());
                                    if (buffer.Contains(check.Substring(0, 2))) {
                                        do {
                                            string rData = buffer.Substring(1, buffer.Length - 3);
                                            buffer = buffer.Substring(1, buffer.Length - 3);
                                            bool validateLRC = ValidateResponseLRC(rData, buffer);
                                            if (validateLRC) {
                                                if (MessageReceived(rData)) {
                                                    _serial.Write(BitConverter.GetBytes((char)ControlCodes.ACK), 0, 1);
                                                    complete = true;
                                                }
                                            }
                                        } while (!complete);
                                    }
                                }
                                if (complete)
                                    break;
                            } while (true);
                            break;
                        } else {
                            Thread.Sleep(1000);
                            _serial.Write(BitConverter.GetBytes((char)ControlCodes.EOT), 0, 1);
                            enquiryCount++;

                            if (enquiryCount.Equals(3)) {
                                throw new MessageException("Terminal did not respond in Enquiry for three (3) times. Send aborted.");
                            }
                        }
                    } while (true);

                    return complete;
                }
                catch (MessageException e) {
                    throw new MessageException(e.Message);
                }
            });
        }

        public byte[] Send(IDeviceMessage message) {
            Connect();
            try {
                if (_serial != null) {
                    string bufferSend = Encoding.ASCII.GetString(message.GetSendBuffer());
                    OnMessageSent?.Invoke(bufferSend.Substring(1, bufferSend.Length - 3));
                    var task = WriteMessage(message);
                    if (!task.Wait(_settings.Timeout)) {
                        throw new MessageException("Terminal did not response within timeout.");
                    }
                    string test = Encoding.ASCII.GetString(messageResponse.ToArray());
                    return messageResponse.ToArray();
                } else throw new MessageException("Terminal not connected.");
            }
            finally {
                if (_serial != null) {
                    Disconnect();
                }
            }
        }

        private bool MessageReceived(string messageData) {
            if (messageResponse == null)
                return false;
            foreach (char b in messageData)
                messageResponse.Add((byte)b);
            return true;
        }
    }
}