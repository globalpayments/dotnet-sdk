using GlobalPayments.Api.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Terminals.HPA.Responses {
    public class HeartBeatResponse : SipBaseResponse {
        public string TransactionTime { get; set; }

        public HeartBeatResponse(byte[] buffer, params string[] messageIds) : base(buffer, messageIds) { }

        internal override void MapResponse(Element response) {
            base.MapResponse(response);
            DeviceResponseCode = NormalizeResponse(response.GetValue<string>("Result"));
            DeviceResponseText = response.GetValue<string>("ResultText");
            TransactionTime = response.GetValue<string>("TransactionTime");
        }
    }
}
