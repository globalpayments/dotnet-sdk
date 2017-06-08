using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;

namespace GlobalPayments.Api.Gateways {
    internal interface IPaymentGateway {
        Transaction ProcesAuthorization(AuthorizationBuilder builder);
        Transaction ManageTransaction(ManagementBuilder builder);
        T ProcessReport<T>(ReportBuilder<T> builder) where T : class;
        string SerializeRequest(AuthorizationBuilder builder);
        bool SupportsHostedPayments { get; }
    }
}
