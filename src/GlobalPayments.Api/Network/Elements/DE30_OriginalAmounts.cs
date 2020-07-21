using GlobalPayments.Api.Network.Abstractions;
using GlobalPayments.Api.Utils;
using System.Text;

namespace GlobalPayments.Api.Network.Elements {
    public class DE30_OriginalAmounts : IDataElement<DE30_OriginalAmounts> {
        public decimal? OriginalTransactionAmount { get; set; }
        public decimal OriginalReconciliationAmount { get; set; }

        public DE30_OriginalAmounts FromByteArray(byte[] buffer) {
            StringParser sp = new StringParser(buffer);
            OriginalTransactionAmount = StringUtils.ToAmount(sp.ReadString(12));
            OriginalReconciliationAmount = StringUtils.ToAmount(sp.ReadString(12));
            return this;
        }

        public byte[] ToByteArray() {
            string rvalue = string.Concat(StringUtils.PadLeft(StringUtils.ToNumeric(OriginalTransactionAmount), 12, '0'),
                    StringUtils.PadLeft(StringUtils.ToNumeric(OriginalReconciliationAmount), 12, '0'));
            return Encoding.ASCII.GetBytes(rvalue);
        }

        public new string ToString() {
            return Encoding.UTF8.GetString(ToByteArray());
        }
    }
}
