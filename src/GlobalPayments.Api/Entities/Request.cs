using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace GlobalPayments.Api.Entities {
    internal class Request {
        public HttpMethod Verb { get; set; } = HttpMethod.Get;
        public string Endpoint { get; set; }
        public string RequestBody { get; set; }
        public Dictionary<string, string> QueryStringParams { get; }
        public static Dictionary<string, string> MaskedValues { get; set; }

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
