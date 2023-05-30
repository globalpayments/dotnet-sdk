using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Gateways
{
    public interface IFraudCheckService
    {
        T ProcessFraud<T>(FraudBuilder<T> builder) where T : class;
    }
}
