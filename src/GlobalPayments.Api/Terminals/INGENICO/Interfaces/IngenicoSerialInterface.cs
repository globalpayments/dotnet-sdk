using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Terminals.INGENICO.Interfaces;
using GlobalPayments.Api.Terminals.Messaging;
using GlobalPayments.Api.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GlobalPayments.Api.Terminals.Ingenico {
    internal class IngenicoSerialInterface : IDeviceCommInterface {
        private ITerminalConfiguration _settings;

        private object _lock;
        private bool _isTransComplete;

        private string _buffer;
        private List<byte> _messageResponse;

        public event MessageSentEventHandler OnMessageSent;
        public event BroadcastMessageEventHandler OnBroadcastMessage;
        public event PayAtTableRequestEventHandler OnPayAtTableRequest; // not required for this connection mode

        public IngenicoSerialInterface(ITerminalConfiguration settings) {
            _settings = settings;
            Connect();
        }

        public void Connect() {
            if (_settings == null) {
                throw new ConfigurationException("Please create connection between device and serial port.");
            }

            if (_settings.Timeout <= 0) {
                _settings.Timeout = Timeout.Infinite;
            }

            try {
                var param = new SerialPort.SerialParameters() {
                    portName = "COM{0}".FormatWith(_settings.Port),
                    baudrate = (int)_settings.BaudRate,
                    byteSize = 8,
                    parityBit = (int)_settings.Parity,
                    stopBit = 0,
                    timeout = new SerialPort.Timeout() {
                        readIntervalTimeout = 0,
                        readTotalTimeoutConstant = 0,
                        writeTotalTimeoutConstant = 0
                    },
                    flowControl = 0
                };

                SerialPort.setSerialParameters(param);
                var param2 = SerialPort.getSerialParameters();

                if (!SerialPort.isOpen()) {
                    SerialPort.initialize();
                    int code = SerialPort.open();

                    if (code != 0) {
                        throw new ConfigurationException(GetWinErrMsg(code));
                    }

                    _lock = new object();
                }
            } catch (Exception e) {
                throw new ConfigurationException(e.Message);
            }
        }

        public void Disconnect() {
            SerialPort.close();
        }

        public byte[] Send(IDeviceMessage message) {
            try {
                _messageResponse = new List<byte>();

                OnMessageSent?.Invoke(RemoveControlCodes(message.GetSendBuffer()));
                Thread.Sleep(250);

                var writeMessage = WriteMessage(message);

                lock (_lock) {
                    while (!_isTransComplete) {
                        if (!Monitor.Wait(_lock, _settings.Timeout)) {
                            throw new ApiException("Terminal did not respond within timeout.");
                        }
                    }
                }

                return _messageResponse.ToArray();
            } catch (Exception e) {
                throw new ApiException(e.Message);
            }
        }

        #region Internal Methods
        private void SendRequest(IDeviceMessage message) {
            do {
                SerialPort.write(Encoding.ASCII.GetString(new byte[] { 0x05 }), 1);
                SerialPort.read(ref _buffer, 1);

                if (_buffer.Contains((char)0x06)) {
                    _buffer = string.Empty;

                    lock (_lock) {
                        Monitor.Pulse(_lock);
                    }

                    Thread.Sleep(_settings.TimeDelay);
                    SerialPort.write(Encoding.ASCII.GetString(message.GetSendBuffer()),
                        message.GetSendBuffer().Length);
                    SerialPort.read(ref _buffer, 1);

                    if (_buffer.Contains((char)0x06)) {
                        _buffer = string.Empty;

                        lock (_lock) {
                            Monitor.Pulse(_lock);
                        }

                        Thread.Sleep(_settings.TimeDelay);
                        SerialPort.write(Encoding.ASCII.GetString(new byte[] { 0x04 }), 1);
                        break;
                    }
                }

            } while (true);
        }

        private void ReceivedMessage() {
            try {
                do {
                    SerialPort.read(ref _buffer, 1);

                    if (_buffer.Contains((char)0x05)) {
                        _buffer = string.Empty;

                        lock (_lock) {
                            Monitor.Pulse(_lock);
                        }

                        Thread.Sleep(_settings.TimeDelay);
                        SerialPort.write(Encoding.ASCII.GetString(new byte[] { 0x06 }), 1);
                        string response = string.Empty;

                        do {
                            SerialPort.read(ref _buffer, 1);
                            response += _buffer;
                            _buffer = string.Empty;

                            lock (_lock) {
                                Monitor.Pulse(_lock);
                            }

                            if (response.Contains((char)0x03)) {
                                byte lrc = TerminalUtilities.CalculateLRC(
                                    Encoding.ASCII.GetBytes(response));

                                SerialPort.read(ref _buffer, 1);

                                string receivedLRC = _buffer.ToHexString();
                                string sLRC = lrc.ToString("X2");

                                lock (_lock) {
                                    Monitor.Pulse(_lock);
                                }

                                Thread.Sleep(_settings.TimeDelay);

                                if (receivedLRC.Equals(sLRC)) {
                                    SerialPort.write(Encoding.ASCII.GetString(new byte[] { 0x06 }), 1);
                                    break;
                                } else {
                                    SerialPort.write(Encoding.ASCII.GetString(new byte[] { 0x15 }), 1);
                                }

                                _buffer = string.Empty;
                            }
                        } while (true);

                        if (response.Contains(INGENICO_GLOBALS.BROADCAST)) {
                            byte[] bMsg = Encoding.GetEncoding(28591).GetBytes(response);
                            BroadcastMessage broadcastMsg = new BroadcastMessage(bMsg);
                            OnBroadcastMessage?.Invoke(broadcastMsg.Code, broadcastMsg.Message);
                        } else {
                            byte[] byteArr = Encoding.ASCII.GetBytes(response);
                            MessageReceived(RemoveControlCodes(byteArr));
                            _isTransComplete = true;

                            lock (_lock) {
                                Monitor.Pulse(_lock);
                            }
                        }

                        SerialPort.read(ref _buffer, 1);

                        if (!_buffer.Contains((char)0x04)) {
                            Thread.Sleep(_settings.TimeDelay);

                            SerialPort.write(Encoding.ASCII.GetString(new byte[] { 0x15 }), 1);
                        }
                    }
                } while (!_isTransComplete);

                _isTransComplete = false;
            } catch (Exception e) {
                throw new ApiException(e.Message);
            }
        }

        private async Task WriteMessage(IDeviceMessage message) {
            await Task.Run(() => {
                try {
                    SendRequest(message);
                    ReceivedMessage();
                } catch (Exception e) {
                    throw new ApiException(e.Message);
                }
            });
        }

        private string RemoveControlCodes(byte[] data) {
            string response = string.Empty;

            byte stx = (byte)ControlCodes.STX;
            byte etx = (byte)ControlCodes.ETX;
            byte ack = (byte)ControlCodes.ACK;
            byte nak = (byte)ControlCodes.NAK;
            byte eot = (byte)ControlCodes.EOT;

            var length = data.Length;
            if (data[data.Length - 1] != etx) {
                length--;
            }

            for (int i = 0; i < length; i++) {
                if (data[i] == stx ||
                    data[i] == etx ||
                    data[i] == ack ||
                    data[i] == nak ||
                    data[i] == eot) {
                    continue;
                }

                response += (char)data[i];
            }

            return response;
        }

        private void MessageReceived(string messageData) {
            foreach (char b in messageData) {
                _messageResponse.Add((byte)b);
            }
        }

        public static string GetWinErrMsg(int errorCode) {
            if (errorCode <= 499) {
                switch (errorCode) {
                    case 1: return "[" + errorCode.ToString() + "] Incorrect function.";
                    case 2: return "[" + errorCode.ToString() + "] The system cannot find the file specified.";
                    case 3: return "[" + errorCode.ToString() + "] The system cannot find the path specified.";
                    case 4: return "[" + errorCode.ToString() + "] The system cannot open the file.";
                    case 5: return "[" + errorCode.ToString() + "] Access is denied.";
                    case 6: return "[" + errorCode.ToString() + "] The handle is invalid.";
                    case 87: return "[" + errorCode.ToString() + "] The parameter is incorrect.";
                    case 170: return "[" + errorCode.ToString() + "] The requested resource is in use.";

                    default:
                        return "[" + errorCode.ToString() + "] Unhandled Error Code. See https://docs.microsoft.com/en-us/windows/win32/debug/system-error-codes--0-499-";
                }
            }

            if (errorCode >= 500 && errorCode <= 999) {
                switch (errorCode) {
                    default:
                        return "[" + errorCode.ToString() + "] Unhandled Error Code. See https://docs.microsoft.com/en-us/windows/win32/debug/system-error-codes--500-999-";
                }
            }

            if (errorCode >= 1000 && errorCode <= 1299) {
                switch (errorCode) {
                    default:
                        return "[" + errorCode.ToString() + "] Unhandled Error Code. See https://docs.microsoft.com/en-us/windows/win32/debug/system-error-codes--1000-1299-";
                }
            }

            if (errorCode >= 1300 && errorCode <= 1699) {
                switch (errorCode) {
                    default:
                        return "[" + errorCode.ToString() + "] Unhandled Error Code. See https://docs.microsoft.com/en-us/windows/win32/debug/system-error-codes--1300-1699-";
                }
            }

            if (errorCode >= 1700 && errorCode <= 3999) {
                switch (errorCode) {
                    default:
                        return "[" + errorCode.ToString() + "] Unhandled Error Code. See https://docs.microsoft.com/en-us/windows/win32/debug/system-error-codes--1700-3999-";
                }
            }

            if (errorCode >= 4000 && errorCode <= 5999) {
                switch (errorCode) {
                    default:
                        return "[" + errorCode.ToString() + "] Unhandled Error Code. See https://docs.microsoft.com/en-us/windows/win32/debug/system-error-codes--4000-5999-";
                }
            }

            if (errorCode >= 6000 && errorCode <= 8199) {
                switch (errorCode) {
                    default:
                        return "[" + errorCode.ToString() + "] Unhandled Error Code. See https://docs.microsoft.com/en-us/windows/win32/debug/system-error-codes--6000-8199-";
                }
            }

            if (errorCode >= 8200 && errorCode <= 8999) {
                switch (errorCode) {
                    default:
                        return "[" + errorCode.ToString() + "] Unhandled Error Code. See https://docs.microsoft.com/en-us/windows/win32/debug/system-error-codes--8200-8999-";
                }
            }

            if (errorCode >= 9000 && errorCode <= 11999) {
                switch (errorCode) {
                    default:
                        return "[" + errorCode.ToString() + "] Unhandled Error Code. See https://docs.microsoft.com/en-us/windows/win32/debug/system-error-codes--9000-11999-";
                }
            }

            if (errorCode >= 12000 && errorCode <= 15999) {
                switch (errorCode) {
                    default:
                        return "[" + errorCode.ToString() + "] Unhandled Error Code. See https://docs.microsoft.com/en-us/windows/win32/debug/system-error-codes--12000-15999-";
                }
            }

            return "[" + errorCode.ToString() + "] Unhandled Error Code.";
        }
        #endregion
    }
}