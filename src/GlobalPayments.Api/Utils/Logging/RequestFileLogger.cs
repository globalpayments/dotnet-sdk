using System;
using System.IO;
using GlobalPayments.Api.Logging;

namespace GlobalPayments.Api.Utils.Logging {
    public class RequestFileLogger : IRequestLogger {
        private object _fileLock = new object();
        private string _fileName;

        public RequestFileLogger(string fileName, string initializationString = null, params string[] args) {
            try {
                _fileName = fileName;
                string directory = Path.GetDirectoryName(_fileName);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                if (!File.Exists(_fileName)) {
                    using (var sw = File.CreateText(_fileName)) {
                        if (!string.IsNullOrEmpty(initializationString)) {
                            AppendText(initializationString, args);
                        }
                    }
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
                lock (_fileLock) {
                    using (var sw = File.AppendText(_fileName)) {
                        if (args == null || args.Length == 0)
                            sw.WriteLine(format);
                        else
                            sw.WriteLine(format, args);
                    }
                }
            }
            catch (Exception ex) { /* NOM NOM */ }
        }
    }
}
