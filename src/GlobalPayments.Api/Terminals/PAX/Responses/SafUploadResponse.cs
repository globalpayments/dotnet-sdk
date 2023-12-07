using System.IO;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Terminals.Extensions;

namespace GlobalPayments.Api.Terminals.PAX.Responses
{
    public class SafUploadResponse : PaxTerminalResponse, ISafUploadResponse
    {
        public string TotalCount { get; set; }
        public string TotalAmount { get; set; }
        public string TimeStamp { get; set; }
        public string UploadedCount { get; set; }
        public string UploadedAmount { get; set; }
        public string FailedCount { get; set; }
        public string FailedTotal { get; set; }
        public string TorInfo { get; set; }
        public SafUploadResponse(byte[] buffer)
            : base(buffer, PAX_MSG_ID.B09_RSP_SAF_UPLOAD)
        {
        }

        protected override void ParseResponse(BinaryReader br)
        {
            base.ParseResponse(br);
            TotalCount = br.ReadToCode(ControlCodes.FS);
            TotalAmount = br.ReadToCode(ControlCodes.FS);
            TimeStamp = br.ReadToCode(ControlCodes.FS);
            UploadedCount = br.ReadToCode(ControlCodes.FS);
            UploadedAmount = br.ReadToCode(ControlCodes.FS);
            FailedCount = br.ReadToCode(ControlCodes.FS);
            FailedTotal = br.ReadToCode(ControlCodes.FS);
            TorInfo = br.ReadToCode(ControlCodes.FS);
        }
    }
}