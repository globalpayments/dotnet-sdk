using System;
using System.IO;
using System.Threading.Tasks;
using GlobalPayments.Api.Logging;

namespace GlobalPayments.Api.Utils.Logging {
    public class RequestConsoleLogger : IRequestLogger {
        private object _lock = new object();

        public RequestConsoleLogger(string initializationString = null, params string[] args) {
            try {
                if (!string.IsNullOrEmpty(initializationString)) {
                    AppendText(initializationString, args);
                }
            }
            catch (Exception ex) { /* NOM NOM */ }
        }

        public void RequestSent(string request) {
            Task.Run(() => {
                AppendText("Request: {0}", request);
            });
        }

        public void ResponseReceived(string response) {
            Task.Run(() => {
                AppendText("Response: {0}", response.TrimEnd('\r', '\n'));
            });
        }

        public void AppendText(string format, params string[] args) {
            try {
                lock (_lock) {
                    Console.WriteLine(format, args);
                }
            }
            catch (Exception ex) { /* NOM NOM */ }
        }
    }
}
