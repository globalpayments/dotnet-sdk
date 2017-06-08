using System;
using System.IO;
using System.Text;
using GlobalPayments.Api.Terminals.Extensions;
using GlobalPayments.Api.Terminals.Abstractions;

namespace GlobalPayments.Api.Terminals.PAX {
    internal class CommercialRequest : IRequestSubGroup {
        public string PoNumber { get; set; }
        public string CustomerCode { get; set; }
        public string TaxExempt { get; set; }
        public string TaxExemptId { get; set; }

        public string GetElementString() {
            var sb = new StringBuilder();
            sb.Append(PoNumber);
            sb.Append((char)ControlCodes.US);
            sb.Append(CustomerCode);
            sb.Append((char)ControlCodes.US);
            sb.Append(TaxExempt);
            sb.Append((char)ControlCodes.US);
            sb.Append(TaxExemptId);

            return sb.ToString().TrimEnd((char)ControlCodes.US);
        }
    }

    internal class CommercialResponse : IResponseSubGroup {
        public string PoNumber { get; private set; }
        public string CustomerCode { get; private set; }
        public bool TaxExempt { get; private set; }
        public string TaxExemptId { get; private set; }

        public CommercialResponse(BinaryReader br) {
            var values = br.ReadToCode(ControlCodes.FS);
            if (string.IsNullOrEmpty(values))
                return;

            var data = values.Split((char)ControlCodes.US);
            try {
                this.PoNumber = data[0];
                this.CustomerCode = data[1];
                this.TaxExempt = data[2] == "0" ? false : true;
                this.TaxExemptId = data[3];
            }
            catch (IndexOutOfRangeException) {
            }
        }
    }
}
