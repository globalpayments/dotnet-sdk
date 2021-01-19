using System;
using System.Net;

namespace GlobalPayments.Api.Tests.Logging {
    public class CustomWebProxy : IWebProxy {
        private readonly string _uri;

        public CustomWebProxy(string uri, ICredentials credentials = null) {
            _uri = uri;
            if (credentials != null) {
                Credentials = credentials;
            }
        }

        public ICredentials Credentials { get; set; }

        public Uri GetProxy(Uri destination) {
            return new Uri(_uri);
        }

        public bool IsBypassed(Uri host) {
            return false;
        }
    }
}
