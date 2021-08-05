using GlobalPayments.Api.Network.Abstractions;
using GlobalPayments.Api.Network.Entities;
using GlobalPayments.Api.Utils;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Network.Elements {
    public class DE48_8_CustomerData : IDataElement<DE48_8_CustomerData> {
        private int fieldCount;
        public Dictionary<DE48_CustomerDataType, string> Fields { get; }
        public bool EmvFlag { get; set; }

        public int GetFieldCount() {
            return Fields.Count;
        }

        public string Get(DE48_CustomerDataType type) {
            if (Fields.ContainsKey(type)) {
                return Fields[type];
            }
            return null;
        }

        public void Set(DE48_CustomerDataType type, string value) {
            if (value != null) {
                Fields[type] = value;
            }
        }

        public DE48_8_CustomerData() {
            Fields = new Dictionary<DE48_CustomerDataType, string>();
        }

        public DE48_8_CustomerData FromByteArray(byte[] buffer) {
            StringParser sp = new StringParser(buffer);
            fieldCount = sp.ReadInt(2);
            for(int i = 0; i < fieldCount; i++) {
                //DE48_CustomerDataType type = sp.ReadStringConstant<DE48_CustomerDataType>(1);
                DE48_CustomerDataType type = EnumConverter.FromMapping<DE48_CustomerDataType>(Target.NWS, sp.ReadString(1));
                string value = sp.ReadToChar('\\');
                Fields[type] = value;
            }
            return this;
        }

        public byte[] ToByteArray() {
            string rvalue = StringUtils.PadLeft(GetFieldCount(), 2, '0');
            foreach(DE48_CustomerDataType type in Fields.Keys) {
                string value = Fields[type];
                rvalue = string.Concat(rvalue, EnumConverter.GetMapping(Target.NWS, type),value,"\\");
            }
            // strip the final '\\'
            rvalue = !EmvFlag ?  StringUtils.TrimEnd(rvalue, "\\") : rvalue;
            return Encoding.ASCII.GetBytes(rvalue);
        }

        public new string ToString() {
            return Encoding.UTF8.GetString(ToByteArray());
        }
    }
}
