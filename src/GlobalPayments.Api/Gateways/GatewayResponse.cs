using System.Net;

namespace GlobalPayments.Api.Gateways {
    internal class GatewayResponse {
        public string RawResponse { get; set; }
        public string RequestUrl { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }
}
