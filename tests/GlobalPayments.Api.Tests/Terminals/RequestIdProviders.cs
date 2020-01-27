using System;
using System.IO;
using GlobalPayments.Api.Terminals.Abstractions;

namespace GlobalPayments.Api.Tests.Terminals {
    public class RandomIdProviders : IRequestIdProvider {
        private Random random;

        public RandomIdProviders() {
            random = new Random(DateTime.Now.Millisecond);
        }

        public int GetRequestId() {
            return random.Next(100000, 999999);
        }
    }

    public class IncrementalNumberProviders : IRequestIdProvider {
        private object _fileLock = new object();
        private int _currentNumber = 1000000;
        private string _fileName = @"C:\temp\requestNumber.dat";

        private static IncrementalNumberProviders _instance;
        public static IncrementalNumberProviders GetInstance() {
            if (_instance != null) {
                _instance = new IncrementalNumberProviders();
            }
            return _instance;
        }

        public int CurrentNumber {
            get { return _currentNumber; }
        }

        private IncrementalNumberProviders() {
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
