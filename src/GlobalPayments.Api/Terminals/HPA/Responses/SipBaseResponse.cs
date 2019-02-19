using GlobalPayments.Api.Utils;
using GlobalPayments.Api.Terminals.Extensions;
using GlobalPayments.Api.Entities;
using System.Collections.Generic;
using System.Linq;

namespace GlobalPayments.Api.Terminals.HPA.Responses {
    public class SipBaseResponse : TerminalResponse {
        private string _response;

        public string EcrId { get; set; }
        public string SipId { get; set; }

        public SipBaseResponse(byte[] buffer, params string[] messageIds) {
            _response = string.Empty;
            foreach (byte b in buffer)
                _response += (char)b;

            var messages = _response.Split('\r');
            foreach (var message in messages) {
                if (string.IsNullOrEmpty(message)) 
                    continue;

                var root = ElementTree.Parse(message).Get("SIP");
                Command = root.GetValue<string>("Response");
                if (Command != null && !messageIds.ToList().Contains(Command)) {
                    throw new MessageException("Excpected {0} but recieved {1}".FormatWith(string.Join(", ", messageIds), Command));
                }

                Version = root.GetValue<string>("Version");
                EcrId = root.GetValue<string>("ECRId");
                SipId = root.GetValue<string>("SIPId");
                Status = root.GetValue<string>("MultipleMessage");
                DeviceResponseCode = NormalizeResponse(root.GetValue<string>("Result"));
                DeviceResponseText = root.GetValue<string>("ResultText");

                MapResponse(root);
            }
            FinalizeResponse();
        }

        internal virtual void MapResponse(Element response) { }
        internal virtual void FinalizeResponse() { }

        protected string NormalizeResponse(string response) {
            var acceptedCodes = new List<string> { "0", "85" };
            if (acceptedCodes.Contains(response))
                return "00";
            return response;
        }

        public override string ToString() {
            return _response;
        }
    }
}
