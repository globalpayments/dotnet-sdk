using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Logging;
using System.Collections.Generic;
using System.Net;

namespace GlobalPayments.Api {
    public abstract class Configuration {
        protected int _timeout = 65000;
        protected Environment _environment = Environment.TEST;

        public Environment Environment { get { return _environment; } set { _environment = value; }  }

        public IRequestLogger RequestLogger { get; set; }

        public IWebProxy WebProxy { get; set; }

        /// <summary>
        /// Gateway service URL
        /// </summary>
        public string ServiceUrl { get; set; }

        /// <summary>
        /// Timeout value for gateway communication (in milliseconds)
        /// </summary>
        public int Timeout { get { return _timeout; } set { _timeout = value; } }

        internal bool Validated { get; private set; }

        internal abstract void ConfigureContainer(ConfiguredServices services);
        public bool EnableLogging { get; set; }
        public bool ForceGatewayTimeout { get; set; }
        public Dictionary<string, string> DynamicHeaders { get; set; }

        internal virtual void Validate() {
            Validated = true;
        }
    }
}
