using System;
using System.IO;
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
            var timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff") + "Z";
            AppendText(
                "-----------------------------------------------------------------------------\n" +
                "[" + timestamp + "] Request: " + request + "\n"
            );
        }

        public void ResponseReceived(string response) {
            var timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff") + "Z";
            AppendText(
                "[" + timestamp + "] Response: " + response.TrimEnd('\r', '\n') + "\n" +
                "-----------------------------------------------------------------------------"
            );
        }

        public void AppendText(string format, params string[] args) {
            try {
                lock (_lock) {
                    if (args == null || args.Length == 0)
                        Console.WriteLine(format);
                    else
                        Console.WriteLine(format, args);
                }
            }
            catch (Exception ex) { /* NOM NOM */ }
        }
    }
}
