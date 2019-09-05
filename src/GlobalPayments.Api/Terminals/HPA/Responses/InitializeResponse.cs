using System.Linq;
using System.Collections.Generic;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Terminals.HPA.Responses {
    public class InitializeResponse : SipBaseResponse, IInitializeResponse {
        private Dictionary<string, Dictionary<string, string>> _params;

        public string SerialNumber { get; set; }

        public InitializeResponse(byte[] buffer, params string[] messageIds) : base(buffer, messageIds) { }

        internal override void MapResponse(Element response) {
            base.MapResponse(response);

            if (_params == null)
                _params = new Dictionary<string, Dictionary<string, string>>();

            string category = response.GetValue<string>("TableCategory") ?? _params.Keys.Last();
            if (!_params.ContainsKey(category))
                _params.Add(category, new Dictionary<string, string>());

            foreach (Element field in response.GetAll("Field")) {
                var key = field.GetValue<string>("Key");
                var value = field.GetValue<string>("Value");
                _params[category].Add(key, value);
            }
        }

        internal override void FinalizeResponse() {
            base.FinalizeResponse();

            SerialNumber = _params["TERMINAL INFORMATION"]["SERIAL NUMBER"];
        }
    }
}
