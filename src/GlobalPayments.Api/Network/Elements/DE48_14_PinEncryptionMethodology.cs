using GlobalPayments.Api.Network.Abstractions;
using GlobalPayments.Api.Network.Entities;
using GlobalPayments.Api.Utils;
using System.Text;

namespace GlobalPayments.Api.Network.Elements {
    public class DE48_14_PinEncryptionMethodology : IDataElement<DE48_14_PinEncryptionMethodology> {
        public DE48_KeyManagementDataCode KeyManagementDataCode { get; set; }
        public DE48_EncryptionAlgorithmDataCode EncryptionAlgorithmDataCode { get; set; }

        public DE48_14_PinEncryptionMethodology FromByteArray(byte[] buffer) {
            StringParser sp = new StringParser(buffer);
            //KeyManagementDataCode = sp.ReadStringConstant<DE48_KeyManagementDataCode>(1);
            //EncryptionAlgorithmDataCode = sp.ReadStringConstant<DE48_EncryptionAlgorithmDataCode>(1);
            KeyManagementDataCode = EnumConverter.FromMapping<DE48_KeyManagementDataCode>(Target.NWS, sp.ReadString(1));
            EncryptionAlgorithmDataCode = EnumConverter.FromMapping<DE48_EncryptionAlgorithmDataCode>(Target.NWS, sp.ReadString(1));
            return this;
        }

        public byte[] ToByteArray() {
            string rvalue = string.Concat(EnumConverter.GetMapping(Target.NWS, KeyManagementDataCode),
                    EnumConverter.GetMapping(Target.NWS, EncryptionAlgorithmDataCode));
            return Encoding.ASCII.GetBytes(rvalue);
        }
    }
}
