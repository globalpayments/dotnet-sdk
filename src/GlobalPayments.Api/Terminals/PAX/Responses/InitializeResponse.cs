using System.IO;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Terminals.Extensions;

namespace GlobalPayments.Api.Terminals.PAX {
    public class InitializeResponse : PaxTerminalResponse, IInitializeResponse {
        public string SerialNumber { get; set; }

        public InitializeResponse(byte[] buffer)
            : base(buffer, PAX_MSG_ID.A01_RSP_INITIALIZE) {
        }

        protected override void ParseResponse(BinaryReader br) {
            base.ParseResponse(br);
            SerialNumber = br.ReadToCode(ControlCodes.ETX);
        }
    }
}
