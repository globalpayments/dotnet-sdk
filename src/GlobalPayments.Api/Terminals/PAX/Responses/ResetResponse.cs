using System;

namespace GlobalPayments.Api.Terminals.PAX {
    public class ResetResponse : PaxDeviceResponse {
        public ResetResponse(byte[] buffer) : base(buffer, PAX_MSG_ID.A17_RSP_RESET) { }
    }
}
