using System;
using System.Text;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Terminals {
    public class DeviceMessage<T> : DeviceMessage where T : IRawRequestBuilder {
        private T _document;

        public DeviceMessage(T doc, byte[] buffer) : base(buffer) {
            _document = doc;
        }

        public override IRawRequestBuilder GetRequestBuilder() {
            return _document;
        }
        public override T GetRequestField<T>(string keyName) {
            return _document.GetValue<T>(keyName);
        }
    }

    public class DeviceMessage : IDeviceMessage {
        byte[] _buffer;

        public bool AwaitResponse { get; set; }
        public bool KeepAlive { get; set; }
        
        public DeviceMessage(byte[] buffer) {
            _buffer = buffer;
        }

        public byte[] GetSendBuffer() {
            return _buffer;
        }

        public virtual IRawRequestBuilder GetRequestBuilder() {
            return null;
        }
        public virtual T GetRequestField<T>(string keyName) {
            return default(T);
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
