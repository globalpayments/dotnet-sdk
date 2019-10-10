using System;
using System.IO;
using GlobalPayments.Api.Terminals.Extensions;

namespace GlobalPayments.Api.Terminals.PAX {
    public class EbtResponse : PaxTerminalResponse {
        public string AvsResultCode { get; set; }
        public string AvsResultText { get; set; }

        // TODO: CVV Response Code
        // TODO: CVV Response Text
        // TODO: Authorized Amount
        public string CardType { get; set; }
        // TODO: Available Balance

        public EbtResponse(byte[] buffer)
            : base(buffer, PAX_MSG_ID.T05_RSP_DO_EBT) {
        }

        protected override void ParseResponse(BinaryReader br) {
            base.ParseResponse(br);

            if (acceptedCodes.Contains(DeviceResponseCode)) {
                HostResponse = new HostResponse(br);
                TransactionType = ((TerminalTransactionType)Int32.Parse(br.ReadToCode(ControlCodes.FS))).ToString().Replace("_", " ");
                AmountResponse = new AmountResponse(br);
                AccountResponse = new AccountResponse(br);
                TraceResponse = new TraceResponse(br);
                ExtDataResponse = new ExtDataSubGroup(br);

                MapResponse();
            }
        }

        protected override void MapResponse() {
            base.MapResponse();
        }
    }
}
