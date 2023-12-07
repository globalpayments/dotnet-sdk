using System.IO;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Terminals.Extensions;

namespace GlobalPayments.Api.Terminals.PAX.Responses
{
    public class SafSummaryReport : PaxTerminalResponse, ISafSummaryReport
    {
        public string TotalCount { get; set; }
        public string TotalAmount { get; set; }
        public SafSummaryReport(byte[] buffer)
            : base(buffer, PAX_MSG_ID.R11_RSP_SAF_SUMMARY_REPORT)
        {
        }

        protected override void ParseResponse(BinaryReader br)
        {
            base.ParseResponse(br);
            TotalCount = br.ReadToCode(ControlCodes.FS);
            TotalAmount = br.ReadToCode(ControlCodes.FS);
        }
    }
}