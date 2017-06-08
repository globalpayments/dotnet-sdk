using System;
using System.IO;
using GlobalPayments.Api.Terminals.Extensions;

namespace GlobalPayments.Api.Terminals.PAX {
    public class GiftResponse : PaxDeviceResponse {
        internal GiftResponse(byte[] buffer)
            : base(buffer, PAX_MSG_ID.T07_RSP_DO_GIFT, PAX_MSG_ID.T09_RSP_DO_LOYALTY) {
        }

        protected override void ParseResponse(BinaryReader br) {
            base.ParseResponse(br);

            if (DeviceResponseCode == "000000") {
                HostResponse = new HostResponse(br);
                TransactionType = br.ReadToCode(ControlCodes.FS);
                AmountResponse = new AmountResponse(br);
                AccountResponse = new AccountResponse(br);
                TraceResponse = new TraceResponse(br);
                ExtDataResponse = new ExtDataSubGroup(br);

                MapResponse();
            }
        }
    }
}
