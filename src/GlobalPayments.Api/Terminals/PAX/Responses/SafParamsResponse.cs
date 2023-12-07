using System.IO;
using GlobalPayments.Api.Entities.Enums;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Terminals.Extensions;

namespace GlobalPayments.Api.Terminals.PAX.Responses
{
    public class SafParamsResponse : PaxTerminalResponse , ISafParamsResponse
    {
        //private HostResponse hostResponse;
        
        public SafMode SAFMode { get; set; }
        public string StartDateTime { get; set; }
        public string EndDateTime { get; set; }
        public string DurationInDays { get; set; }

        public string MaxNumberOfRecord {  get; set; }
        public string TotalCeilingAmount { get; set; }
        public string CeilingAmountPerCardType { get; set; }
        public string HALOPerCardType {  get; set; }
        public string UploadMode { get; set; }
        public string AutoUploadIntervalTime { get; set; }
        public string DeleteSAFConfirmation {  get; set; }


        public SafParamsResponse(byte[] buffer)
            : base(buffer, PAX_MSG_ID.A79_RSP_GET_SAF_PARAMETERS)
        {
        }

        protected override void ParseResponse(BinaryReader br)
        {
            base.ParseResponse(br);

            SAFMode = (SafMode)int.Parse(br.ReadToCode(ControlCodes.FS));
            StartDateTime = br.ReadToCode(ControlCodes.FS);
            EndDateTime = br.ReadToCode(ControlCodes.FS);
            DurationInDays = br.ReadToCode(ControlCodes.FS);
            MaxNumberOfRecord =br.ReadToCode(ControlCodes.FS);
            TotalCeilingAmount = br.ReadToCode(ControlCodes.FS);
            CeilingAmountPerCardType = br.ReadToCode(ControlCodes.FS);
            HALOPerCardType = br.ReadToCode(ControlCodes.FS);
            UploadMode = br.ReadToCode(ControlCodes.FS);
        }
    }
}
