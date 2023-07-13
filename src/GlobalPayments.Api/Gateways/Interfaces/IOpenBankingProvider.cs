using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Gateways
{
    public interface IOpenBankingProvider
    {
        bool SupportsHostedPayments { get; }
        Transaction ProcessOpenBanking(AuthorizationBuilder builder);
        Transaction ManageOpenBanking(ManagementBuilder builder);
    }
}
