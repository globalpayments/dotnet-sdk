using System;
using System.IO;
using GlobalPayments.Api.Terminals.Extensions;

namespace GlobalPayments.Api.Terminals.PAX {
    public class CheckSubResponse : PaxDeviceResponse {
        public string CustomerId { get; set; }
        
        internal CheckSubResponse(byte[] buffer)
            : base(buffer, PAX_MSG_ID.T13_RSP_DO_CHECK) {
        }

        protected override void ParseResponse(BinaryReader br) {
            base.ParseResponse(br);

            if (DeviceResponseCode == "000000") {
                HostResponse = new HostResponse(br);
                TransactionType = ((TerminalTransactionType)Int32.Parse(br.ReadToCode(ControlCodes.FS))).ToString().Replace("_", " ");
                AmountResponse = new AmountResponse(br);
                CheckSubResponse = new CheckSubGroup(br);
                TraceResponse = new TraceResponse(br);
                ExtDataResponse = new ExtDataSubGroup(br);
            }

            MapResponse();
        }

        protected override void MapResponse() {
            base.MapResponse();

            // Host Response
            if (HostResponse != null) {
                AuthorizationCode = HostResponse.AuthCode;
            }
        }
    }
}
