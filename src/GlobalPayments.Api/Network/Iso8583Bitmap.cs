using GlobalPayments.Api.Network.Entities;
using GlobalPayments.Api.Utils;
using System;
using System.Text;

namespace GlobalPayments.Api.Network {
    public class Iso8583Bitmap {
        private string binaryValue;
        private int offset;
        private int currIndex = -1;

        //private ReverseIntEnumMap<DataElementId> dataElementMap;

        public Iso8583Bitmap(byte[] bytes) : this(bytes, 0) { }

        public Iso8583Bitmap(byte[] bytes, int offset) {
            this.offset = offset;

            StringBuilder sb = new StringBuilder();
            foreach (byte b in bytes) {
                sb.Append(Convert.ToString((b & 0xFF) + 0x100,2).Substring(1));
            }

            binaryValue = sb.ToString();
            //dataElementMap = new ReverseIntEnumMap<DataElementId>();
        }

        public bool IsPresent(DataElementId element) {
            return binaryValue[(int.Parse(EnumConverter.GetMapping(Target.NWS, element)) - offset)] == '1';
        }

        public DataElementId GetNextDataElement() {
            // get the next set value
            char value;
            do {
                // return null if end of string
                if (++currIndex >= binaryValue.Length) {
                    return 0;
                }

                value = binaryValue[(currIndex)];
            }
            while (value == '0');

            // return the enum value
            //return dataElementMap.Get(currIndex + offset);
            return (DataElementId)(currIndex + offset);
        }

        internal void SetDataElement(DataElementId element) {
            StringBuilder sb = new StringBuilder(binaryValue);
            sb[element.GetHashCode() - offset] = '1';
            binaryValue = sb.ToString();
        }

        public string ToBinaryString() {
            return binaryValue;
        }

        public string ToHexString() {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < binaryValue.Length; i += 8) {
                int decim = Convert.ToInt32(binaryValue.Substring(i, 8), 2);
                string hexValue = Convert.ToString(decim, 16);
                sb.Append(StringUtils.PadLeft(hexValue, 2, '0'));
            }

            return sb.ToString();
        }

        public byte[] ToByteArray() {
            string s = ToHexString();

            byte[] b = new byte[s.Length / 2];
            for (int i = 0; i < b.Length; i++) {
                int index = i * 2;
                int v = Convert.ToInt32(s.Substring(index, 2), 16);
                b[i] = (byte)v;
            }
            return b;
        }
    }
}
