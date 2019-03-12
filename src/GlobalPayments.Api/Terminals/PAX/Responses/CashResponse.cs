using System;
using System.IO;
using GlobalPayments.Api.Terminals.Extensions;

namespace GlobalPayments.Api.Terminals.PAX {
    public class CashResponse : PaxTerminalResponse {
        private HostResponse hostResponse;
        private string transactionType;
        private AmountResponse amountResponse;
        private TraceResponse traceResponse;
        private ExtDataSubGroup extDataResponse;

        internal CashResponse(byte[] buffer)
            : base(buffer, PAX_MSG_ID.T11_RSP_DO_CASH) {
        }

        protected override void ParseResponse(BinaryReader br) {
            base.ParseResponse(br);

            if (acceptedCodes.Contains(DeviceResponseCode)) {
                hostResponse = new HostResponse(br);
                transactionType = ((TerminalTransactionType)Int32.Parse(br.ReadToCode(ControlCodes.FS))).ToString().Replace("_", " ");
                amountResponse = new AmountResponse(br);
                traceResponse = new TraceResponse(br);
                extDataResponse = new ExtDataSubGroup(br);
            }
        }
    }
}
