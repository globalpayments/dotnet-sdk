using System.IO;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Terminals.Extensions;

namespace GlobalPayments.Api.Terminals.PAX.Responses
{
    public class AccountInputResponse : PaxTerminalResponse, IAccountInputResponse
    {
        public int EntryMode { get; set; }
        public string Track1Data { get; set; }
        public string Track2Data { get; set; }

        public string Track3Data { get; set; }
        public string PAN { get; set; }
        public string ExpiryDate { get; set; }
        public string QRCode { get; set; }
        public string KSN { get; set; }
        public string AdditionalInfo { get; set; }
        public AccountInputResponse(byte[] buffer)
            : base(buffer, PAX_MSG_ID.A31_RSP_INPUT_ACCOUNT)
        {
        }

        protected override void ParseResponse(BinaryReader br)
        {
            base.ParseResponse(br);
            EntryMode = int.Parse(br.ReadToCode(ControlCodes.FS));
            Track1Data = br.ReadToCode(ControlCodes.FS);
            Track2Data = br.ReadToCode(ControlCodes.FS);
            Track3Data = br.ReadToCode(ControlCodes.FS);
            PAN = br.ReadToCode(ControlCodes.FS);
            ExpiryDate = br.ReadToCode(ControlCodes.FS);
            QRCode = br.ReadToCode(ControlCodes.FS);
            KSN = br.ReadToCode(ControlCodes.FS);
            AdditionalInfo = br.ReadToCode(ControlCodes.FS);
        }
    }
}