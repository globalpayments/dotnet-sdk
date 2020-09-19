using GlobalPayments.Api.Network.Abstractions;
using GlobalPayments.Api.Utils;
using System.Text;

namespace GlobalPayments.Api.Network.Elements {
    public class DE48_33_PosConfiguration : IDataElement<DE48_33_PosConfiguration> {
        public string Timezone { get; set; }
        public bool? SupportsPartialApproval { get; set; }
        public bool? SupportsReturnBalance { get; set; }
        public bool? SupportsCashOver { get; set; }
        public bool? MobileDevice { get; set; }

        public DE48_33_PosConfiguration FromByteArray(byte[] buffer) {
            StringParser sp = new StringParser(buffer);
            Timezone = sp.ReadString(1);
            SupportsPartialApproval = sp.ReadBoolean("Y");
            SupportsReturnBalance = sp.ReadBoolean("Y");
            SupportsCashOver = sp.ReadBoolean("2");
            MobileDevice = sp.ReadBoolean("Y");
            return this;
        }

        public byte[] ToByteArray() {
            string rvalue = string.IsNullOrEmpty(Timezone) ? " " : Timezone;
            rvalue = string.Concat(rvalue,SupportsPartialApproval == null ? " " : (bool)SupportsPartialApproval ? "Y" : "N");
            rvalue = string.Concat(rvalue,SupportsReturnBalance == null ? " " : (bool)SupportsReturnBalance ? "Y" : "N");
            rvalue = string.Concat(rvalue,SupportsCashOver == null ? " " : (bool)SupportsCashOver ? "0" : "2");
            rvalue =  string.Concat(rvalue,MobileDevice == null ? " " : (bool)MobileDevice ? "Y" : "N");
            return Encoding.ASCII.GetBytes(StringUtils.TrimEnd(rvalue));
        }

        public new string ToString() {
            return Encoding.UTF8.GetString(ToByteArray());
        }
    }
}
