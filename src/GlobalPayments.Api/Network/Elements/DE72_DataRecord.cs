using GlobalPayments.Api.Network.Abstractions;
using GlobalPayments.Api.Utils;
using System.Text;

namespace GlobalPayments.Api.Network.Elements {
    public class DE72_DataRecord : IDataElement<DE72_DataRecord> {
        public DE72_DataRecord FromByteArray(byte[] buffer) {
            StringParser sp = new StringParser(buffer);
            // TODO: Parse out this element
            return this;
        }

        public byte[] ToByteArray() {
            string rvalue = "";
            return Encoding.ASCII.GetBytes(rvalue);
        }

        public new string ToString() {
            return Encoding.UTF8.GetString(ToByteArray());
        }
    }
}
