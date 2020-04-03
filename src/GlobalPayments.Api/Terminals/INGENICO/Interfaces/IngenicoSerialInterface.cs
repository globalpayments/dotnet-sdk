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

namespace GlobalPayments.Api.Terminals.Ingenico {
    internal class IngenicoSerialInterface : IDeviceCommInterface {
        public event MessageSentEventHandler OnMessageSent;
        public event BroadcastMessageEventHandler OnBroadcastMessage;
        public event MessageReceivedEventHandler OnMessageReceived;

        ITerminalConfiguration _settings;

        private SerialPort _serial;
        private bool _complete = false;
        private bool _isResult = false;
        private bool _isAcknowledge = false;
        private bool _broadcast = false;
        private bool _isXML = false;
        private string _buffer = string.Empty;
        private string _appendReport = string.Empty;
        private List<byte> _messageResponse;

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
                    _serial.DataReceived += new SerialDataReceivedEventHandler(Serial_DataReceived);
                    _serial.Open();
                }
                else {
                    throw new MessageException("Serial Port is already open.");
                }
            }
        }

        private void Serial_DataReceived(object sender, SerialDataReceivedEventArgs e) {
            SerialPort serial = (SerialPort)sender;
            do {
                Thread.Sleep(0100);
                _buffer = serial.ReadExisting();

                if (!string.IsNullOrEmpty(_buffer)) {
                    _serial.ReadTimeout = _settings.Timeout;

                    if (_buffer.Equals(INGENICO_RESP.ACKNOWLEDGE)) {
                        _isAcknowledge = true;
                        break;
                    }
                    else if (_buffer.Equals(INGENICO_RESP.ENQUIRY)) {
                        _serial.Write(BitConverter.GetBytes((char)ControlCodes.ACK), 0, 1);
                        break;
                    }
                    else if (_buffer.Contains(INGENICO_GLOBALS.BROADCAST)) {
                        _broadcast = true;
                        break;
                    }
                    else if (_buffer.Equals(INGENICO_RESP.NOTACKNOWLEDGE)) {
                        _isAcknowledge = false;
                        break;
                    }
                    else if (INGENICO_RESP.XML.Any(_buffer.Contains)) {
                        _isXML = true;
                        break;
                    }
                    else {
                        if (!_buffer.Contains(INGENICO_GLOBALS.BROADCAST) && !_buffer.Contains(INGENICO_RESP.INVALID)
                            && !INGENICO_RESP.XML.Any(_buffer.Contains) && !_buffer.Contains(INGENICO_RESP.ENDOFTXN)
                            && !_buffer.Contains(INGENICO_RESP.NOTACKNOWLEDGE)) {
                            _isResult = true;
                        }
                        break;
                    }
                }
            } while (true);
        }

        public void Disconnect() {
            _serial.Close();
            _serial?.Dispose();
            _serial = null;
            _buffer = string.Empty;
            _appendReport = string.Empty;
            _isResult = false;
            _complete = false;
            _isAcknowledge = false;
            _broadcast = false;
            _isXML = false;
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

                    if (_serial == null) {
                        return false;
                    }

                    do {
                        _serial.Write(BitConverter.GetBytes((char)ControlCodes.ENQ), 0, 1);
                        if (_isAcknowledge) {
                            do {
                                byte[] msg = message.GetSendBuffer();
                                foreach (byte b in msg) {
                                    byte[] _b = new byte[] { b };
                                    _serial.Write(_b, 0, 1);
                                }

                                if (_isAcknowledge) {
                                    _serial.Write(BitConverter.GetBytes((char)ControlCodes.EOT), 0, 1);
                                    break;
                                }
                            } while (true);

                            do {
                                if (_broadcast) {
                                    byte[] bMsg = Encoding.ASCII.GetBytes(_buffer);
                                    BroadcastMessage broadcastMsg = new BroadcastMessage(bMsg);
                                    OnBroadcastMessage?.Invoke(broadcastMsg.Code, broadcastMsg.Message);
                                    _broadcast = false;
                                }

                                if (_isXML) {
                                    do {
                                        _appendReport += _buffer;
                                        if (_appendReport.Contains(INGENICO_RESP.ENDXML)) {
                                            string xmlData = _appendReport.Substring(1, _appendReport.Length - 3);
                                            if (MessageReceived(xmlData)) {
                                                _serial.Write(BitConverter.GetBytes((char)ControlCodes.ACK), 0, 1);
                                                _complete = true;
                                            }
                                        }
                                        Thread.Sleep(0500);
                                    } while (!_complete);
                                }

                                if (_isResult) {
                                    string check = Encoding.UTF8.GetString(message.GetSendBuffer());
                                    if (_buffer.Contains(check.Substring(0, 2))) {
                                        do {
                                            string rData = _buffer.Substring(1, _buffer.Length - 3);
                                            _buffer = _buffer.Substring(1, _buffer.Length - 3);
                                            bool validateLRC = ValidateResponseLRC(rData, _buffer);
                                            if (validateLRC) {
                                                if (MessageReceived(rData)) {
                                                    _serial.Write(BitConverter.GetBytes((char)ControlCodes.ACK), 0, 1);
                                                    _complete = true;
                                                }
                                            }
                                        } while (!_complete);
                                    }
                                }
                                if (_complete) {
                                    break;
                                }
                            } while (true);
                            break;
                        }
                        else {
                            Thread.Sleep(1000);
                            _serial.Write(BitConverter.GetBytes((char)ControlCodes.EOT), 0, 1);
                            enquiryCount++;

                            if (enquiryCount.Equals(3)) {
                                throw new MessageException("Terminal did not respond in Enquiry for three (3) times. Send aborted.");
                            }
                        }
                    } while (true);

                    return _complete;
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
                    Task<bool> task = WriteMessage(message);
                    if (!task.Wait(_settings.Timeout)) {
                        throw new MessageException("Terminal did not response within timeout.");
                    }
                    string test = Encoding.ASCII.GetString(_messageResponse.ToArray());
                    return _messageResponse.ToArray();
                }
                else {
                    throw new MessageException("Terminal not connected.");
                }
            }
            finally {
                if (_serial != null) {
                    Disconnect();
                }
            }
        }

        private bool MessageReceived(string messageData) {
            if (_messageResponse == null) {
                return false;
            }
            foreach (char b in messageData) {
                _messageResponse.Add((byte)b);
            }
            OnMessageReceived?.Invoke(Encoding.UTF8.GetString(_messageResponse.ToArray()));
            return true;
        }
    }
}