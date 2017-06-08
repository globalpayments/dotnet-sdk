using System;
using System.IO;
using System.Text;
using GlobalPayments.Api.Terminals.Extensions;
using GlobalPayments.Api.Terminals.Abstractions;

namespace GlobalPayments.Api.Terminals.PAX {
    internal class TraceRequest : IRequestSubGroup {
        public string ReferenceNumber { get; set; }
        public string InvoiceNumber { get; set; }
        public string AuthCode { get; set; }
        public string TransactionNumber { get; set; }
        public string TimeStamp { get; set; }

        public string GetElementString() {
            var sb = new StringBuilder();
            sb.Append(ReferenceNumber);
            sb.Append((char)ControlCodes.US);
            sb.Append(InvoiceNumber);
            sb.Append((char)ControlCodes.US);
            sb.Append(AuthCode);
            sb.Append((char)ControlCodes.US);
            sb.Append(TransactionNumber);
            sb.Append((char)ControlCodes.US);
            sb.Append(TimeStamp);

            return sb.ToString().TrimEnd((char)ControlCodes.US);
        }
    }

    internal class TraceResponse : IResponseSubGroup {
        public string TransactionNumber { get; private set; }
        public string ReferenceNumber { get; private set; }
        public string TimeStamp { get; private set; }

        public TraceResponse(BinaryReader br) {
            var values = br.ReadToCode(ControlCodes.FS);
            if (string.IsNullOrEmpty(values))
                return;

            var data = values.Split((char)ControlCodes.US);
            try {
                this.TransactionNumber = data[0];
                this.ReferenceNumber = data[1];
                this.TimeStamp = data[2];
            }
            catch (IndexOutOfRangeException) {
            }
        }
    }
}
