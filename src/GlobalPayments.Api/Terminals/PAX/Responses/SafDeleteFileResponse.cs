using System.IO;
using GlobalPayments.Api.Entities.Enums;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Terminals.Extensions;

namespace GlobalPayments.Api.Terminals.PAX.Responses
{
    public class SafDeleteFileResponse : PaxTerminalResponse, ISafDeleteFileResponse
    {
        public string TotalCount { get; set; }
        public string TorInfo { get; set; }

        public SafDeleteFileResponse(byte[] buffer)
            : base(buffer, PAX_MSG_ID.B11_RSP_DELETE_SAF_FILE)
        {
        }

        protected override void ParseResponse(BinaryReader br)
        {
            base.ParseResponse(br);
            TotalCount = br.ReadToCode(ControlCodes.FS);
            TorInfo = br.ReadToCode(ControlCodes.FS);
        }
    }
}