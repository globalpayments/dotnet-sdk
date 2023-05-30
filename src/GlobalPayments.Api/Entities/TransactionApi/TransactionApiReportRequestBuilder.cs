using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Gateways;
using System.Net.Http;

namespace GlobalPayments.Api.Entities {
    internal class TransactionApiReportRequestBuilder {
        internal static TransactionApiRequest BuildRequest<T>(ReportBuilder<T> builder, TransactionApiConnector gateway) where T : class {
            return new TransactionApiRequest {
                Verb = HttpMethod.Get,
                Endpoint = $"",
            };
        }
    }
}
