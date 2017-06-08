using System;
using System.IO;
using System.Text;
using GlobalPayments.Api.Terminals.Extensions;
using GlobalPayments.Api.Terminals.Abstractions;

namespace GlobalPayments.Api.Terminals.PAX {
    internal class CheckSubGroup : IRequestSubGroup, IResponseSubGroup {
        public string SaleType { get; set; }
        public string RoutingNumber { get; set; }
        public string AccountNumber { get; set; }
        public string CheckNumber { get; set; }
        public string CheckType { get; set; }
        public string IdType { get; set; }
        public string IdValue { get; set; }
        public string DOB { get; set; }
        public string PhoneNumber { get; set; }
        public string ZipCode { get; set; }

        public CheckSubGroup() { }
        public CheckSubGroup(BinaryReader br) {
            var values = br.ReadToCode(ControlCodes.FS);
            if (string.IsNullOrEmpty(values))
                return;

            var data = values.Split((char)ControlCodes.US);
            try {
                this.SaleType = data[0];
                this.RoutingNumber = data[1];
                this.AccountNumber = data[2];
                this.CheckNumber = data[3];
                this.CheckType = data[4];
                this.IdType = data[5];
                this.IdValue = data[6];
                this.DOB = data[7];
                this.PhoneNumber = data[8];
                this.ZipCode = data[9];
            }
            catch (IndexOutOfRangeException) {
            }
        }

        public string GetElementString() {
            var sb = new StringBuilder();

            sb.Append(SaleType);
            sb.Append((char)ControlCodes.US);
            sb.Append(RoutingNumber);
            sb.Append((char)ControlCodes.US);
            sb.Append(AccountNumber);
            sb.Append((char)ControlCodes.US);
            sb.Append(CheckNumber);
            sb.Append((char)ControlCodes.US);
            sb.Append(CheckType);
            sb.Append((char)ControlCodes.US);
            sb.Append(IdType);
            sb.Append((char)ControlCodes.US);
            sb.Append(IdValue);
            sb.Append((char)ControlCodes.US);
            sb.Append(DOB);
            sb.Append((char)ControlCodes.US);
            sb.Append(PhoneNumber);
            sb.Append((char)ControlCodes.US);
            sb.Append(ZipCode);
            sb.Append((char)ControlCodes.US);

            return sb.ToString().TrimEnd((char)ControlCodes.US);
        }
    }
}
