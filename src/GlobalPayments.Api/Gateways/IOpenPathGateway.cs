using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Gateways {
    public interface IOpenPathGateway {
        string OpenPathApiKey { get; set; }
        string OpenPathApiUrl { get; set; }
    }
}
