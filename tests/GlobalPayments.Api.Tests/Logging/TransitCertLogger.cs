using GlobalPayments.Api.Logging;
using System;
using System.Collections.Generic;
using System.IO;

namespace GlobalPayments.Api.Tests.Logging {
    public class TransitCertLogger : IRequestLogger {
        #region Singleton Stuff
        private static TransitCertLogger _instance;
        public static TransitCertLogger GetInstance(string fileName) {
            if (_instance == null) {
                _instance = new TransitCertLogger(fileName);
            }
            return _instance;
        }
        #endregion

        private Dictionary<string, List<string>> _requests;
        private string _fileName;
        private string _firstTransactionId;
        private string _lastTransactionId;
        private string _currentTest;

        public TransitCertLogger(string fileName) {
            _fileName = fileName;
        }

        #region Methods

        public void BeginRun() {
            _requests = new Dictionary<string, List<string>>();
            using (var sw = File.CreateText(_fileName)) {
                sw.WriteLine("Authorization Start Date/Time: {0}", DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss"));
            }
        }

        public void BeginTest(string testName, params string[] args) {
            _currentTest = string.Format(testName, args);
            if (!_requests.ContainsKey(_currentTest)) {
                _requests.Add(_currentTest, new List<string>());
            }
        }

        public void EndRun() {
            using (var sw = File.AppendText(_fileName)) {
                foreach (var key in _requests.Keys) {
                    sw.WriteLine(key);
                    _requests[key].ForEach(p => {
                        sw.WriteLine(p);
                    });
                    sw.WriteLine();
                }

                sw.WriteLine("Authorization End Date/Time: {0}", DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss"));
                sw.WriteLine("First HRN: {0}", _firstTransactionId);
                sw.WriteLine("Last HRN: {0}", _lastTransactionId);
            }
        }

        #endregion

        #region IRequestLogger Implementation

        public void RequestSent(string request) {
            _requests[_currentTest].Add(string.Format("Request: {0}", request));
        }

        public void ResponseReceived(string response) {
            if (_firstTransactionId == null) {
                _firstTransactionId = response;
            }
            else _lastTransactionId = response;

            _requests[_currentTest].Add(string.Format("Response: {0}", response));
        }

        #endregion
    }
}