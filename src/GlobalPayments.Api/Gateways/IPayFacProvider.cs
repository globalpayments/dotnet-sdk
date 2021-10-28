using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;

namespace GlobalPayments.Api.Gateways
{
    interface IPayFacProvider {
        Transaction ProcessPayFac(PayFacBuilder builder);
    }
}
