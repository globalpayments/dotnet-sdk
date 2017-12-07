using System;
using System.Text;
using GlobalPayments.Api.Terminals.Abstractions;

namespace GlobalPayments.Api.Terminals {
    public class DeviceMessage : IDeviceMessage {
        byte[] _buffer;

        public DeviceMessage(byte[] buffer) {
            this._buffer = buffer;
        }

        public byte[] GetSendBuffer() {
            return _buffer;
        }

        public override string ToString() {
            var sb = new StringBuilder();
            foreach (byte b in _buffer) {
                if (Enum.IsDefined(typeof(ControlCodes), b)) {
                    var code = (ControlCodes)b;
                    sb.Append(string.Format("[{0}]", code.ToString()));
                }
                else sb.Append((char)b);
            }

            return sb.ToString();
        }
    }
}
