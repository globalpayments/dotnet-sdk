using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;

namespace GlobalPayments.Api.Gateways {
    internal interface IPaymentGateway {
        Transaction ProcessAuthorization(AuthorizationBuilder builder);
        Transaction ManageTransaction(ManagementBuilder builder);
        string SerializeRequest(AuthorizationBuilder builder);
        bool SupportsHostedPayments { get; }
        bool SupportsOpenBanking { get; }
    }
}
