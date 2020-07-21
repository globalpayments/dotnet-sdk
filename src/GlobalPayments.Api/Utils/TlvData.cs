using System;
using System.Text;

namespace GlobalPayments.Api.Utils {
    public class TlvData {
        private string tag;
        private string length;
        private string value;
        private string description;

        public string GetTag() {
            return tag;
        }
        public string GetLength() {
            return length;
        }
        public string GetValue() {
            return value;
        }
        public string GetBinaryValue() {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in StringUtils.BytesFromHex(value)) {
                sb.Append(Convert.ToString((b & 0xFF) + 0x100,2).Substring(1));
            }
            return sb.ToString();
        }
        public string GetDescription() {
            return description;
        }

        public string GetFullValue() {
            return string.Format("{0}{1}{2}", tag, length, value);
        }

        public TlvData(string tag, string length, string value) : this(tag, length, value, null) {
            
        }
        public TlvData(string tag, string length, string value, string description) {
            this.tag = tag;
            this.length = length;
            this.value = value;
            this.description = description;
        }
    }
}
