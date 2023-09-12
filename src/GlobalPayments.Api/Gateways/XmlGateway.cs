using System.Net;
using System.Net.Http;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Logging;

namespace GlobalPayments.Api.Gateways {
    internal abstract class XmlGateway : Gateway {
        public XmlGateway() : base("text/xml") { }

        public virtual string DoTransaction(string request, string endpoint = "") {
            if (Request.MaskedValues != null) {
                MaskedRequestData = Request.MaskedValues;
            }
            var response = SendRequest(HttpMethod.Post, endpoint, request, null, null, true, true);
            if (response.StatusCode != HttpStatusCode.OK) {
                DisposeMaskedValues();
                throw new GatewayException("Unexpected http status code [" + response.StatusCode + "]");
            }
            DisposeMaskedValues();
            return response.RawResponse;
        }

        private void DisposeMaskedValues() {
            Request.MaskedValues = null;
            ProtectSensitiveData.DisposeCollection();
            MaskedRequestData = new System.Collections.Generic.Dictionary<string, string>();            
        }
    }
}
