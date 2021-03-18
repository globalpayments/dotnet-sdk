using System.Net;
using System.Net.Http;

namespace GlobalPayments.Api.Gateways {
    internal class HttpClientHandlerBuilder {
        internal static HttpClientHandler Build(IWebProxy webProxy = null) {
            var handler = new HttpClientHandler {
                AutomaticDecompression = DecompressionMethods.GZip,
            };

            if (webProxy != null) {
                handler.UseProxy = true;
                handler.Proxy = webProxy;
            }

            return handler;
        }
    }
}
