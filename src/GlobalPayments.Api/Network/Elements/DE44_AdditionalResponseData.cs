using GlobalPayments.Api.Network.Abstractions;
using GlobalPayments.Api.Network.Entities;
using GlobalPayments.Api.Utils;
using System.Text;

namespace GlobalPayments.Api.Network.Elements {
    public class DE44_AdditionalResponseData : IDataElement<DE44_AdditionalResponseData> {
        public DE44_ActionReasonCode ActionReasonCode { get; set; }
        public string TextMessage { get; set; }

        public DE44_AdditionalResponseData FromByteArray(byte[] buffer) {
            StringParser sp = new StringParser(buffer);
            //ActionReasonCode = sp.ReadStringConstant<DE44_ActionReasonCode>(4);
            ActionReasonCode = EnumConverter.FromMapping<DE44_ActionReasonCode>(Target.NWS, sp.ReadString(4));
            TextMessage = sp.ReadRemaining();
            return this;
        }

        public byte[] ToByteArray() {
            string rvalue = EnumConverter.GetMapping(Target.NWS, ActionReasonCode);
            if (TextMessage != null) {
                rvalue = string.Concat(rvalue, TextMessage);
            }
            return Encoding.ASCII.GetBytes(rvalue);
        }
    }
}
