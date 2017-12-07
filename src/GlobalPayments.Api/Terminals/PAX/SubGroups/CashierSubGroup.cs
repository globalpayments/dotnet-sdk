using System;
using System.IO;
using System.Text;
using GlobalPayments.Api.Terminals.Extensions;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Terminals.PAX {
    internal class CashierSubGroup : IRequestSubGroup, IResponseSubGroup {
        public string ClerkId { get; set; }
        public string ShiftId { get; set; }

        public CashierSubGroup() { }
        public CashierSubGroup(BinaryReader br) {
            var values = br.ReadToCode(ControlCodes.FS);
            if (string.IsNullOrEmpty(values))
                return;

            var data = values.Split((char)ControlCodes.US);
            try {
                ClerkId = data[0];
                ShiftId = data[1];
            }
            catch (IndexOutOfRangeException exc) {
                EventLogger.Instance.Error(exc.Message);
            }
        }

        public string GetElementString() {
            var sb = new StringBuilder();
            sb.Append(ClerkId);
            sb.Append((char)ControlCodes.US);
            sb.Append(ShiftId);

            return sb.ToString().TrimEnd((char)ControlCodes.US);
        }
    }
}
