using System;
using System.IO;
using GlobalPayments.Api.Terminals.Extensions;

namespace GlobalPayments.Api.Terminals.PAX {
    public class DebitResponse : PaxTerminalResponse {
        internal DebitResponse(byte[] buffer) : base(buffer, PAX_MSG_ID.T03_RSP_DO_DEBIT) { }

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

            // Host Response
            if (HostResponse != null) {
                AuthorizationCode = HostResponse.AuthCode;
            }

            // Account Response
            if (AccountResponse != null) {
                PaymentType = AccountResponse.CardType.ToString();
            }

            // Account Response
            if (AmountResponse != null) {
                TransactionAmount = AmountResponse.ApprovedAmount;
                AmountDue = AmountResponse.AmountDue;
            }
        }
    }
}