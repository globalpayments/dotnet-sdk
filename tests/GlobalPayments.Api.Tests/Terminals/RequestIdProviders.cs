using System;
using System.IO;
using GlobalPayments.Api.Terminals.Abstractions;

namespace GlobalPayments.Api.Tests.Terminals {
    public class RandomIdProvider : IRequestIdProvider {
        private Random random;

        public RandomIdProvider() {
            random = new Random(DateTime.Now.Millisecond);
        }

        public int GetRequestId() {
            return random.Next(100000, 999999);
        }
    }

    public class IncrementalNumberProvider : IRequestIdProvider {
        private object _fileLock = new object();
        private int _currentNumber = 1000000;
        private string _fileName = @"C:\temp\requestNumber.dat";

        private static IncrementalNumberProvider _instance;
        public static IncrementalNumberProvider GetInstance() {
            if (_instance == null) {
                _instance = new IncrementalNumberProvider();
            }
            return _instance;
        }

        public int CurrentNumber {
            get { return _currentNumber; }
        }

        private IncrementalNumberProvider() {
            lock (_fileLock) {
                try {
                    using (var sr = new StreamReader(_fileName)) {
                        string savedValue = sr.ReadLine();
                        if (!string.IsNullOrEmpty(savedValue)) {
                            _currentNumber = int.Parse(savedValue);
                        }
                        else SaveCurrentNumber();
                    }
                }
                catch (IOException) {
                    SaveCurrentNumber();
                }
            }
        }

        public int GetRequestId() {
            lock (_fileLock) {
                if (_currentNumber == 999999) {
                    _currentNumber = 100000;
                }
                else _currentNumber += 1;
                SaveCurrentNumber();
            }
            return _currentNumber;
        }

        private void SaveCurrentNumber() {
            try {
                using (var sw = new StreamWriter(_fileName)) {
                    sw.WriteLine(_currentNumber);
                    sw.Flush();
                }
            }
            catch (IOException) { /* NOM NOM */ }
        }
    }
}
