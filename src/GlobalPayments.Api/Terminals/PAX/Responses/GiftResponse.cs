using System;
using System.IO;
using GlobalPayments.Api.Terminals.Extensions;

namespace GlobalPayments.Api.Terminals.PAX {
    public class GiftResponse : PaxTerminalResponse {
        public GiftResponse(byte[] buffer)
            : base(buffer, PAX_MSG_ID.T07_RSP_DO_GIFT, PAX_MSG_ID.T09_RSP_DO_LOYALTY) {
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
    }
}
