using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Terminals.HPA.Responses {
    public class SipSendFileResponse : SipBaseResponse {
        public int MaxDataSize { get; set; }
 
        public SipSendFileResponse(byte[] buffer, params string[] messageIds) : base(buffer, messageIds) {
        }

        internal override void MapResponse(Element response) {
            base.MapResponse(response);

            MaxDataSize = response.GetValue<int>("MaxDataSize");
        }
    }
}
