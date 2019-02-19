using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Terminals
{
    public interface IRequestIdProvider {
        int GetRequestId();
    }
}
