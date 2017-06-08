using System.Net;

namespace GlobalPayments.Api.Gateways
{
    internal class GatewayResponse {
        public HttpStatusCode StatusCode { get; set; }
        public string RawResponse { get; set; }
    }
}
