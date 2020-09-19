using GlobalPayments.Api.Network.Abstractions;
using GlobalPayments.Api.Utils;
using System.Text;

namespace GlobalPayments.Api.Network.Elements {
    public class DE103_Check_MICR_Data : IDataElement<DE103_Check_MICR_Data> {
        public string TransitNumber { get; set; }
        public string AccountNumber { get; set; }
        public string SequenceNumber { get; set; }

        public DE103_Check_MICR_Data FromByteArray(byte[] buffer) {
            StringParser sp = new StringParser(buffer);
            TransitNumber = sp.ReadToChar('\\');
            AccountNumber = sp.ReadToChar('\\');
            SequenceNumber = sp.ReadRemaining();
            return this;
        }

        public byte[] ToByteArray() {
          string rvalue = TransitNumber;
          if (!string.IsNullOrEmpty(AccountNumber)) {
              rvalue = string.Concat(rvalue,"\\" + AccountNumber);
          }
          if (!string.IsNullOrEmpty(SequenceNumber)) {
              rvalue = string.Concat(rvalue,"\\" + SequenceNumber);
          }          
          return Encoding.ASCII.GetBytes(rvalue);
        }

        public new string ToString() {
            return Encoding.UTF8.GetString(ToByteArray());
        }
    }
}
