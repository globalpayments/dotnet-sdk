using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.GpApi;
using GlobalPayments.Api.Gateways;
using System.Net.Http;

namespace GlobalPayments.Api.Builders.RequestBuilder.GpApi
{
    internal class GpApiMiCRequestBuilder : IRequestBuilder<string>
    {
        public Request BuildRequest(string rawRequest, GpApiConnector connector) {
            var request = new Request {
                Verb = HttpMethod.Post,
                Endpoint = GpApiRequest.DEVICES,
                RequestBody = rawRequest
            };

            return request;
        }
    }
}
