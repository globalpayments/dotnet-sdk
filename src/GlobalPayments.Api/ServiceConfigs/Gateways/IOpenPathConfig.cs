using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.ServiceConfigs.Gateways
{
    public interface IOpenPathConfig {
        /// <summary>
        /// The OpenPath Api key for integration with OpenPath platform
        /// </summary>
        string OpenPathApiKey { get; set; }

        /// <summary>
        /// The OpenPath Api key for integration with OpenPath platform
        /// </summary>
        string OpenPathApiUrl { get; set; }
    }
}
