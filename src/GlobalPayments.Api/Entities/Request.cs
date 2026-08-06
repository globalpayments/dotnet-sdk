using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace GlobalPayments.Api.Entities {
    public class Request {
        public HttpMethod Verb { get; set; } = HttpMethod.Get;
        public string Endpoint { get; set; }
        public string RequestBody { get; set; }
        public Dictionary<string, string> QueryStringParams { get; }

        // Request-scoped masked values. Instance (not static) so concurrent transactions cannot
        // overwrite or null out one another's masking data. Internal because the Request constructor
        // is internal and this is SDK plumbing, not consumer-facing surface.
        internal Dictionary<string, string> MaskedValues { get; set; }

        internal Request() {
            QueryStringParams = new Dictionary<string, string>();
        }

        internal void AddQueryStringParam(string name, string value) {
            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(value)) {
                QueryStringParams.Add(name, value);
            }
        }
    }
}
