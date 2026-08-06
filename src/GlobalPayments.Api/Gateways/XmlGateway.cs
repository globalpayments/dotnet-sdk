using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Logging;

namespace GlobalPayments.Api.Gateways {
    internal abstract class XmlGateway : Gateway {
        public XmlGateway() : base("text/xml") { }

        public virtual string DoTransaction(string request, string endpoint = "", IDictionary<string, string> maskedValues = null) {
            var response = SendRequest(HttpMethod.Post, endpoint, request, null, null, true, true, maskedValues : maskedValues);
            if (response.StatusCode != HttpStatusCode.OK) {
                throw new GatewayException("Unexpected http status code [" + response.StatusCode + "]");
            }
            return response.RawResponse;
        }
    }
}
