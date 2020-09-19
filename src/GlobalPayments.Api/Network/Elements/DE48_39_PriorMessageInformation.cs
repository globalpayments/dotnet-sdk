using GlobalPayments.Api.Network.Abstractions;
using GlobalPayments.Api.Utils;
using System.Text;

namespace GlobalPayments.Api.Network.Elements {
    public class DE48_39_PriorMessageInformation : IDataElement<DE48_39_PriorMessageInformation> {
        public string ResponseTime { get; set; } = "999";
        public string ConnectTime { get; set; } = "999";
        public string CardType { get; set; } = "    ";
        public string MessageTransactionIndicator { get; set; } = "0000";
        public string ProcessingCode { get; set; } = "000000";
        public string Stan { get; set; } = "000000";

        public DE48_39_PriorMessageInformation FromByteArray(byte[] buffer) {
            StringParser sp = new StringParser(buffer);
            ResponseTime = sp.ReadString(3);
            ConnectTime = sp.ReadString(3);
            CardType = sp.ReadString(4);
            MessageTransactionIndicator = sp.ReadString(4);
            ProcessingCode = sp.ReadString(6);
            Stan = sp.ReadString(6);
            return this;
        }

        public byte[] ToByteArray() {
            string rvalue = string.Concat(ResponseTime,
                ConnectTime,
                CardType,
                MessageTransactionIndicator,
                ProcessingCode,
                Stan);
            return Encoding.ASCII.GetBytes(rvalue);
        }

        public new string ToString() {
            return Encoding.UTF8.GetString(ToByteArray());
        }
    }
}
