using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.GpApi;
using GlobalPayments.Api.Gateways;
using System.Net.Http;

namespace GlobalPayments.Api.Builders.RequestBuilder.GpApi
{
    internal class GpApiMiCRequestBuilder : IRequestBuilder<string>
    {
        public Request BuildRequest(string rawRequest, GpApiConnector connector) {
            string merchantUrl = GpApiRequest.GetMerchantUrl(connector.GpApiConfig);
            var request = new Request {
                Verb = HttpMethod.Post,
                Endpoint = $"{merchantUrl}{GpApiRequest.DEVICES}",
                RequestBody = rawRequest
            };

            return request;
        }
    }
}
