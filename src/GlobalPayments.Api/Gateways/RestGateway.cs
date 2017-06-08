using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Gateways {
    internal abstract class RestGateway : Gateway {
        public RestGateway() : base("application/json") {}

        public virtual string DoTransaction(HttpMethod verb, string endpoint, string data = null, Dictionary<string, string> queryStringParams = null) {
            var response = SendRequest(verb, endpoint, data, queryStringParams);
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.NoContent) {
                var parsed = JsonDoc.Parse(response.RawResponse);
                var error = parsed.Get("error") ?? parsed;
                throw new GatewayException(string.Format("Status Code: {0} - {1}", response.StatusCode, error.GetValue<string>("message")));
            }
            return response.RawResponse;
        }
    }
}
