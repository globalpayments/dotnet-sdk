using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Gateways;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Gateways
{
    internal interface IRequestBuilder<T> {
        Request BuildRequest(T builder, GpApiConnector gateway);
    }
}
