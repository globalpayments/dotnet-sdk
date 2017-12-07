using System;
using System.IO;
using System.Text;
using GlobalPayments.Api.Terminals.Extensions;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Terminals.PAX {
    internal class AmountRequest : IRequestSubGroup {
        public string TransactionAmount { get; set; }
        public string TipAmount { get; set; }
        public string CashBackAmount { get; set; }
        public string MerchantFee { get; set; }
        public string TaxAmount { get; set; }
        public string FuelAmount { get; set; }

        public string GetElementString() {
            var sb = new StringBuilder();
            sb.Append(TransactionAmount);
            sb.Append((char)ControlCodes.US);
            sb.Append(TipAmount);
            sb.Append((char)ControlCodes.US);
            sb.Append(CashBackAmount);
            sb.Append((char)ControlCodes.US);
            sb.Append(MerchantFee);
            sb.Append((char)ControlCodes.US);
            sb.Append(TaxAmount);
            sb.Append((char)ControlCodes.US);
            sb.Append(FuelAmount);

            return sb.ToString().TrimEnd((char)ControlCodes.US);
        }
    }

    internal class AmountResponse : IResponseSubGroup {
        public decimal? ApprovedAmount { get; private set; }
        public decimal? AmountDue { get; private set; }
        public decimal? TipAmount { get; private set; }
        public decimal? CashBackAmount { get; private set; }
        public decimal? MerchantFee { get; private set; }
        public decimal? TaxAmount { get; private set; }
        public decimal? Balance1 { get; private set; }
        public decimal? Balance2 { get; private set; }

        public AmountResponse(BinaryReader br) {
            var values = br.ReadToCode(ControlCodes.FS);
            if (string.IsNullOrEmpty(values))
                return;

            var data = values.Split((char)ControlCodes.US);
            try {
                this.ApprovedAmount = data[0].ToAmount();
                this.AmountDue = data[1].ToAmount();
                this.TipAmount = data[2].ToAmount();
                this.CashBackAmount = data[3].ToAmount();
                this.MerchantFee = data[4].ToAmount();
                this.TaxAmount = data[5].ToAmount();
                this.Balance1 = data[6].ToAmount();
                this.Balance2 = data[7].ToAmount();
            }
            catch (IndexOutOfRangeException exc) {
                EventLogger.Instance.Error(exc.Message);
            }
        }
    }
}
