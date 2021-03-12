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
        private SerialPort _serialPort;
        private ITerminalConfiguration _settings;

        private bool _transComplete;
        private bool _isResult;
        private bool _isAcknowledge;
        private bool _isBroadcast;
        private bool _isXML;

        private List<byte> _messageResponse;
        private string _bufferReceived = string.Empty;
        private StringBuilder _report = new StringBuilder();

        public event MessageSentEventHandler OnMessageSent;
        public event BroadcastMessageEventHandler OnBroadcastMessage;
        public event PayAtTableRequestEventHandler OnPayAtTableRequest;

        public IngenicoSerialInterface(ITerminalConfiguration settings) {
            this._settings = settings;
            Connect();
        }

        public void Connect() {
            if (_serialPort == null) {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(INGENICO_GLOBALS.MGMT_SCOPE, INGENICO_GLOBALS.MGMT_QUERY);

                string manufacturer = string.Empty;
                foreach (ManagementObject mgmtObject in searcher.Get()) {
                    manufacturer = mgmtObject["Manufacturer"].ToString().ToLower();
                    if (manufacturer.Equals("ingenico")) {
                        string caption = mgmtObject["Caption"].ToString();
                        string portName = "COM{0}".FormatWith(_settings.Port);
                        if (caption.Equals(portName)) {
                            _serialPort = new SerialPort() {
                                PortName = portName,
                                BaudRate = (int)_settings.BaudRate,
                                DataBits = (int)_settings.DataBits,
                                StopBits = _settings.StopBits,
                                Parity = _settings.Parity,
                                Handshake = _settings.Handshake,
                                RtsEnable = true,
                                DtrEnable = true,
                                ReadTimeout = _settings.Timeout
                            };
                        }
                    }
                }

                if (_serialPort == null) {
                    throw new ConfigurationException("Can't connect to the terminal. Port not found.");
                }

                if (!_serialPort.IsOpen) {
                    _serialPort.DataReceived += new SerialDataReceivedEventHandler(Serial_DataReceived);
                    _serialPort.Open();
                }
            } else {
                throw new ConfigurationException("Serial port is already open.");
            }
        }

        private void Serial_DataReceived(object sender, SerialDataReceivedEventArgs e) {
            SerialPort serial = (SerialPort)sender;
            do {
                Thread.Sleep(0100);
                _bufferReceived = serial.ReadExisting();

                if (!string.IsNullOrEmpty(_bufferReceived)) {
                    _serialPort.ReadTimeout = _settings.Timeout;

                    if (_bufferReceived.Equals(INGENICO_RESP.ACKNOWLEDGE)) {
                        _isAcknowledge = true;
                        break;
                    } else if (_bufferReceived.Equals(INGENICO_RESP.ENQUIRY)) {
                        _serialPort.Write(BitConverter.GetBytes((char)ControlCodes.ACK), 0, 1);
                        break;
                    } else if (_bufferReceived.Contains(INGENICO_GLOBALS.BROADCAST)) {
                        _isBroadcast = true;
                        break;
                    } else if (INGENICO_RESP.XML.Any(_bufferReceived.Contains)) {
                        _isXML = true;
                        break;
                    } else if (!_bufferReceived.Contains(INGENICO_RESP.INVALID)
                        && !_bufferReceived.Contains(INGENICO_GLOBALS.BROADCAST)
                        && !INGENICO_RESP.XML.Any(_bufferReceived.Contains)) {
                        _isResult = true;
                        break;
                    }
                }
            } while (true);
        }

        public void Disconnect() {
            _serialPort.Close();
            _serialPort?.Dispose();
            _serialPort = null;
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
                    _messageResponse = new List<byte>();

                    if (_serialPort == null) {
                        return false;
                    }

                    do {
                        _serialPort.Write(BitConverter.GetBytes((char)ControlCodes.ENQ), 0, 1);
                        if (!_isAcknowledge) {
                            Thread.Sleep(1000);
                            _serialPort.Write(BitConverter.GetBytes((char)ControlCodes.EOT), 0, 1);
                            enquiryCount++;

                            if (enquiryCount.Equals(3)) {
                                throw new MessageException("Terminal did not respond in Enquiry for three (3) times. Send aborted.");
                            }
                        } else {
                            do {
                                byte[] msg = message.GetSendBuffer();
                                foreach (byte b in msg) {
                                    byte[] _b = new byte[] { b };
                                    _serialPort.Write(_b, 0, 1);
                                }

                                if (_isAcknowledge) {
                                    _serialPort.Write(BitConverter.GetBytes((char)ControlCodes.EOT), 0, 1);
                                    _isAcknowledge = false;
                                    break;
                                }
                            } while (true);

                            do {
                                Thread.Sleep(100);
                                if (_isBroadcast) {
                                    byte[] bMsg = Encoding.ASCII.GetBytes(_bufferReceived);
                                    BroadcastMessage broadcastMsg = new BroadcastMessage(bMsg);
                                    OnBroadcastMessage?.Invoke(broadcastMsg.Code, broadcastMsg.Message);
                                    _isBroadcast = false;
                                }

                                if (_isXML) {
                                    do {
                                        _report.Append(_bufferReceived);
                                        if (_report.ToString().Contains(INGENICO_RESP.ENDXML)) {
                                            string xmlData = _report.ToString().Substring(1, _report.ToString().Length - 3);
                                            if (MessageReceived(xmlData)) {
                                                _serialPort.Write(BitConverter.GetBytes((char)ControlCodes.ACK), 0, 1);
                                                _isXML = false;
                                                _transComplete = true;
                                            }
                                        }
                                        Thread.Sleep(0500);
                                    } while (!_transComplete);
                                }

                                if (_isResult) {
                                    string check = Encoding.UTF8.GetString(message.GetSendBuffer());
                                    if (_bufferReceived.Contains(check.Substring(0, 2))) {
                                        do {
                                            string rData = _bufferReceived.Substring(1, _bufferReceived.Length - 3);
                                            _bufferReceived = _bufferReceived.Substring(1, _bufferReceived.Length - 3);
                                            bool validateLRC = ValidateResponseLRC(rData, _bufferReceived);
                                            if (validateLRC) {
                                                if (MessageReceived(rData)) {
                                                    _serialPort.Write(BitConverter.GetBytes((char)ControlCodes.ACK), 0, 1);
                                                    _isResult = false;
                                                    _transComplete = true;
                                                }
                                            }
                                        } while (!_transComplete);
                                    }
                                }
                            } while (!_transComplete);

                            return _transComplete;
                        }
                    } while (true);
                } catch (MessageException e) {
                    throw new MessageException(e.Message);
                }
            });
        }

        public byte[] Send(IDeviceMessage message) {
            try {
                if (_serialPort != null) {
                    string bufferSend = Encoding.ASCII.GetString(message.GetSendBuffer());
                    OnMessageSent?.Invoke(bufferSend.Substring(1, bufferSend.Length - 3));
                    Task<bool> task = WriteMessage(message);
                    if (!task.Wait(_settings.Timeout)) {
                        throw new MessageException("Terminal did not response within timeout.");
                    }

                    return _messageResponse.ToArray();
                } else {
                    throw new MessageException("Terminal not connected.");
                }
            } finally {
                _transComplete = false;
            }
        }

        private bool MessageReceived(string messageData) {
            if (_messageResponse == null) {
                return false;
            }

            foreach (char b in messageData) {
                _messageResponse.Add((byte)b);
            }

            return true;
        }
    }
}