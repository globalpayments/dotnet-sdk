using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Gateways
{
    interface IPayFacProvider {
        Transaction ProcessPayFac(PayFacBuilder builder);
    }
}
