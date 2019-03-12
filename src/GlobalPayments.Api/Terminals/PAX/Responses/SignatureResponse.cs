using System.IO;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Terminals.Extensions;

namespace GlobalPayments.Api.Terminals.PAX {
    public class SignatureResponse : PaxTerminalResponse, ISignatureResponse {
        public int TotalLength { get; set; }
        public int ResponseLegth { get; set; }

        public SignatureResponse(byte[] response) : base(response, PAX_MSG_ID.A09_RSP_GET_SIGNATURE, PAX_MSG_ID.A21_RSP_DO_SIGNATURE) { }

        protected override void ParseResponse(BinaryReader br) {
            base.ParseResponse(br);

            if (DeviceResponseCode == "000000" && Command == PAX_MSG_ID.A09_RSP_GET_SIGNATURE) {
                TotalLength = int.Parse(br.ReadToCode(ControlCodes.FS));
                ResponseLegth = int.Parse(br.ReadToCode(ControlCodes.FS));

                var signatureData = br.ReadToCode(ControlCodes.ETX);
                SignatureData = TerminalUtilities.BuildSignatureImage(signatureData);
            }
        }
    }
}
