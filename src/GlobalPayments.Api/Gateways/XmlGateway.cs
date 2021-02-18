using System.Net;
using System.Net.Http;
using GlobalPayments.Api.Entities;

namespace GlobalPayments.Api.Gateways {
    internal abstract class XmlGateway : Gateway {
        public XmlGateway() : base("text/xml") { }

        public virtual string DoTransaction(string request, string endpoint = "") {
            var response = SendRequest(HttpMethod.Post, endpoint, request);
            if (response.StatusCode != HttpStatusCode.OK) {
                throw new GatewayException("Unexpected http status code [" + response.StatusCode + "]");
            }
            return response.RawResponse;
        }
    }
}
