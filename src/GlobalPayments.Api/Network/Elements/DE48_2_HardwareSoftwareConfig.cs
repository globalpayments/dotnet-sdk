using GlobalPayments.Api.Network.Abstractions;
using GlobalPayments.Api.Utils;
using System.Text;

namespace GlobalPayments.Api.Network.Elements {
    public class DE48_2_HardwareSoftwareConfig : IDataElement<DE48_2_HardwareSoftwareConfig> {
        public string HardwareLevel { get; set; }
        public string SoftwareLevel { get; set; }
        public string OperatingSystemLevel { get; set; }

        public DE48_2_HardwareSoftwareConfig FromByteArray(byte[] buffer) {
            StringParser sp = new StringParser(buffer);
            HardwareLevel = sp.ReadString(4);
            SoftwareLevel = sp.ReadString(8);
            OperatingSystemLevel = sp.ReadString(8);
            return this;
        }

        public byte[] ToByteArray() {
            if (string.IsNullOrEmpty(HardwareLevel) && string.IsNullOrEmpty(SoftwareLevel) && string.IsNullOrEmpty(OperatingSystemLevel)) {
                return null;
            }
            string rvalue = string.IsNullOrEmpty(HardwareLevel) ? "    " : StringUtils.PadRight(HardwareLevel, 4, ' ');
            rvalue = string.Concat(rvalue,string.IsNullOrEmpty(SoftwareLevel) ? "        " : StringUtils.PadRight(SoftwareLevel, 8, ' '));
            rvalue = string.Concat(rvalue,string.IsNullOrEmpty(OperatingSystemLevel) ? "        " : StringUtils.PadRight(OperatingSystemLevel, 8, ' '));
            return Encoding.ASCII.GetBytes(rvalue);
        }

        public new string ToString() {
            return Encoding.UTF8.GetString(ToByteArray());
        }
    }
}
