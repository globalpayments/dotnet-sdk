using System.Collections.Generic;
using System.Net.Http;

namespace GlobalPayments.Api.Entities {
    internal class TransactionApiRequest
    {
        public HttpMethod Verb { get; set; } = HttpMethod.Get;
        public string Endpoint { get; set; }
        public string RequestBody { get; set; }
        public Dictionary<string, string> QueryStringParams { get; }

        internal TransactionApiRequest() {
            QueryStringParams = new Dictionary<string, string>();
        }
 
        internal void AddQueryStringParam(string name, string value) {
            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(value)) {
                QueryStringParams.Add(name, value);
            }
        }
    }
}
