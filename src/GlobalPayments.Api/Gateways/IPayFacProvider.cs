using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Gateways
{
    interface IPayFacProvider {
        T ProcessPayFac<T>(PayFacBuilder<T> builder) where T : class;
        T ProcessBoardingUser<T>(PayFacBuilder<T> builder) where T : class;
        bool HasBuiltInMerchantManagementService { get; }
    }
}
