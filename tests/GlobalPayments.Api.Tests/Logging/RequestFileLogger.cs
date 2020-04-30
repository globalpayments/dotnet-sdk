using System;
using System.IO;
using System.Threading.Tasks;
using GlobalPayments.Api.Logging;

namespace GlobalPayments.Api.Tests.Logging {
    public class RequestFileLogger : IRequestLogger {
        private object _fileLock = new object();
        private string _fileName;

        public RequestFileLogger(string fileName, string initializationString = null, params string[] args) {
            try {
                _fileName = fileName;
                if (!File.Exists(_fileName)) {
                    using (var sw = File.CreateText(_fileName)) {
                        if (!string.IsNullOrEmpty(initializationString)) {
                            AppendText(initializationString, args);
                        }
                    }
                }
            }
            catch (Exception) { /* NOM NOM */ }
        }

        public void RequestSent(string request) {
            Task.Run(() => {
                AppendText("Response: {0}", request);
            });
        }

        public void ResponseReceived(string response) {
            Task.Run(() => {
                AppendText("Response: {0}", response.TrimEnd('\r', '\n'));
            });
        }

        public void AppendText(string format, params string[] args) {
            try {
                lock (_fileLock) {
                    using (var sw = File.AppendText(_fileName)) {
                        sw.WriteLine(format, args);
                    }
                }
            }
            catch (Exception) { /* NOM NOM */ }
        }
    }
}
