using GlobalPayments.Api.Network.Abstractions;
using GlobalPayments.Api.Network.Entities;
using GlobalPayments.Api.Utils;
using System.Text;

namespace GlobalPayments.Api.Network.Elements {
    public class DE46_FeeAmounts : IDataElement<DE46_FeeAmounts> {
        public FeeType FeeTypeCode { get; set; }
        public Iso4217_CurrencyCode CurrencyCode { get; set; }
        public decimal Amount { get; set; }
        public decimal ConversionRate { get; set; }
        public decimal ReconciliationAmount { get; set; }
        public Iso4217_CurrencyCode ReconciliationCurrencyCode { get; set; }

        public DE46_FeeAmounts FromByteArray(byte[] buffer) {
            StringParser sp = new StringParser(buffer);
            //FeeTypeCode = sp.ReadStringConstant<FeeType>(2);
            //CurrencyCode = sp.ReadStringConstant<Iso4217_CurrencyCode>(3);
            FeeTypeCode = EnumConverter.FromMapping<FeeType>(Target.NWS, sp.ReadString(2));
            CurrencyCode = EnumConverter.FromMapping<Iso4217_CurrencyCode>(Target.NWS, sp.ReadString(3));
            string D1 = sp.ReadString(1); // TODO: We don't know what this is
            Amount = StringUtils.ToAmount(sp.ReadString(8));
            ConversionRate = decimal.Parse(sp.ReadString(8));
            //decimal.TryParse(sp.ReadString(8), out ConversionRate);
            string D2 = sp.ReadString(1); // TODO: We don't know what this is
            ReconciliationAmount = StringUtils.ToAmount(sp.ReadString(8));
            //ReconciliationCurrencyCode = sp.ReadStringConstant<Iso4217_CurrencyCode>(3);
            ReconciliationCurrencyCode = EnumConverter.FromMapping<Iso4217_CurrencyCode>(Target.NWS, sp.ReadString(3));
            return this;
        }

        public byte[] ToByteArray() {
            string rvalue = string.Concat(EnumConverter.GetMapping(Target.NWS, FeeTypeCode),
            EnumConverter.GetMapping(Target.NWS, CurrencyCode),
            "D",
            StringUtils.ToNumeric(Amount, 8),
            StringUtils.ToNumeric(ConversionRate, 8),
            "D",
            StringUtils.ToNumeric(ReconciliationAmount, 8),
            EnumConverter.GetMapping(Target.NWS, ReconciliationCurrencyCode));
            return Encoding.ASCII.GetBytes(rvalue);
        }
    }
}
