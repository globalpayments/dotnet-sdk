using System;

namespace GlobalPayments.Api.Logging {
    public interface IRequestLogger {
        void RequestSent(string request);
        void ResponseReceived(string response);
    }
}
