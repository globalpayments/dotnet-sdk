using System;
using System.IO;
using System.Text;
using GlobalPayments.Api.Terminals.Extensions;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Terminals.PAX {
    internal class AvsRequest : IRequestSubGroup {
        public string ZipCode { get; set; }
        public string Address { get; set; }

        public string GetElementString() {
            var sb = new StringBuilder();
            sb.Append(ZipCode);
            sb.Append((char)ControlCodes.US);
            sb.Append(Address);

            return sb.ToString().TrimEnd((char)ControlCodes.US);
        }
    }

    internal class AvsResponse : IResponseSubGroup {
        public string AvsResponseCode { get; private set; }
        public string AvsResponseMessage { get; private set; }

        public AvsResponse(BinaryReader br) {
            var values = br.ReadToCode(ControlCodes.FS);
            if (string.IsNullOrEmpty(values))
                return;

            var data = values.Split((char)ControlCodes.US);
            try {
                AvsResponseCode = data[0];
                AvsResponseMessage = data[1];
            }
            catch (IndexOutOfRangeException exc) {
                EventLogger.Instance.Error(exc.Message);
            }
        }
    }
}
