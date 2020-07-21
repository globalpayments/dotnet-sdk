using GlobalPayments.Api.Network.Entities;
using GlobalPayments.Api.Utils;
using System.Text;

namespace GlobalPayments.Api.Network.Elements {
    public class DE54_AdditionalAmount {
        public DE3_AccountType AccountType { get; set; }
        public DE54_AmountTypeCode AmountType { get; set; }
        public Iso4217_CurrencyCode CurrencyCode { get; set; }
        public decimal? Amount { get; set; }

        public DE54_AdditionalAmount FromByteArray(byte[] buffer) {
            StringParser sp = new StringParser(buffer);
            AccountType = EnumConverter.FromMapping<DE3_AccountType>(Target.NWS, sp.ReadString(2));
            AmountType = EnumConverter.FromMapping<DE54_AmountTypeCode>(Target.NWS, sp.ReadString(2));
            CurrencyCode = EnumConverter.FromMapping<Iso4217_CurrencyCode>(Target.NWS, sp.ReadString(3));
            sp.ReadString(1);
            Amount = StringUtils.ToAmount(sp.ReadString(12));
            return this;
        }

        public byte[] ToByteArray() {
            string rvalue = string.Concat(EnumConverter.GetMapping(Target.NWS, AccountType),
                EnumConverter.GetMapping(Target.NWS, AmountType),
                EnumConverter.GetMapping(Target.NWS, CurrencyCode),
                "D",
                StringUtils.ToNumeric(Amount, 12));
            return Encoding.ASCII.GetBytes(rvalue);
        }
    }
}
