using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Utils;
using System;

namespace GlobalPayments.Api.Terminals.HeartSIP.Responses {
    public class SipSignatureResponse : SipBaseResponse, ISignatureResponse {
        public SipSignatureResponse(byte[] response, params string[] messageIds) : base(response, messageIds) { }

        internal override void MapResponse(Element response) {
            base.MapResponse(response);

            if (DeviceResponseCode == "00") {
                var attachmentData = response.GetValue<string>("AttachmentData");
                SignatureData = Convert.FromBase64String(attachmentData);
            }
        }
    }
}
