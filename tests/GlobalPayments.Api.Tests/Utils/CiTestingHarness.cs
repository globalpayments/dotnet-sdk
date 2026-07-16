using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using GlobalPayments.Api.Terminals.Abstractions;
using Newtonsoft.Json;

namespace GlobalPayments.Api.Tests.Utils {
    /// <summary>Record/replay harness for CI tests.</summary>
    public class CiTestingHarness {
        public enum CacheMode { Unlocked, Locked }

        private const string TimestampKey = "_fixedTimestamp";

        private readonly string _proxyBase;
        private readonly string _targetServiceUrl;
        private readonly CacheMode _cacheMode;
        private readonly string _testName;
        private readonly Dictionary<string, string> _generatedIds;
        private readonly long _fixedTimestampMillis;

        private string _language = "dotnet";
        private string _category;
        private string _subcategory;
        private string _currentFunction;
        private List<string> _varPayload;
        private List<string> _varQuery;

        public CiTestingHarness(string targetServiceUrl, CacheMode cacheMode, string testName) {
            var proxyHost = System.Environment.GetEnvironmentVariable("PROXY_ENDPOINT");
            if (string.IsNullOrEmpty(proxyHost)) {
                throw new InvalidOperationException(
                    "PROXY_ENDPOINT environment variable is not set. " +
                    "Set it to the proxy hostname (e.g. PROXY_ENDPOINT=\"your-proxy-host.example.com\").");
            }
            _proxyBase = "https://" + proxyHost + "/proxy";
            _targetServiceUrl = targetServiceUrl;
            _cacheMode = cacheMode;
            _testName = testName;
            SetDefaultVariableFields(targetServiceUrl);

            if (cacheMode == CacheMode.Locked) {
                _generatedIds = LoadIds(true);
            } else {
                _generatedIds = LoadIds(false);
                _generatedIds[TimestampKey] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                    .ToString(CultureInfo.InvariantCulture);
                WriteIds();
            }
            _fixedTimestampMillis = long.Parse(_generatedIds[TimestampKey], CultureInfo.InvariantCulture);
            System.Environment.SetEnvironmentVariable(
                "SDK_TESTING_TIMESTAMP",
                _fixedTimestampMillis.ToString(CultureInfo.InvariantCulture));
        }

        public void SetFunction(string functionPath) {
            if (functionPath == null) {
                _category = _subcategory = _currentFunction = null;
                return;
            }
            var parts = functionPath.Split(new[] { '|' }, 3);
            if (parts.Length == 3) {
                _category = parts[0];
                _subcategory = parts[1];
                _currentFunction = parts[2];
            } else {
                _category = _subcategory = null;
                _currentFunction = functionPath;
            }
        }

        public void SetLanguage(string language) {
            _language = language;
        }

        public void SetVariableFields(List<string> payload, List<string> query = null) {
            _varPayload = payload ?? new List<string>();
            _varQuery = query ?? new List<string>();
        }

        public string GetTestingUrl() {
            var cacheReturns = _cacheMode == CacheMode.Locked ? "true" : "false";
            var targetHost = System.Text.RegularExpressions.Regex.Replace(
                _targetServiceUrl, "^https?://", "https:/");

            var args = "cacheReturns:" + cacheReturns;

            var varsParts = new List<string>();
            foreach (var f in _varPayload) varsParts.Add("payload." + f);
            foreach (var f in _varQuery) varsParts.Add("query." + f);
            if (varsParts.Count > 0) {
                args += ",vars:" + string.Join("|", varsParts);
            }

            if (_language != null && _category != null && _subcategory != null && _currentFunction != null) {
                args += ",language:" + Encode(_language)
                    + ",category:" + Encode(_category)
                    + ",subcategory:" + Encode(_subcategory)
                    + ",function:" + Encode(_currentFunction);
            }

            return _proxyBase + "/(" + args + ")/" + targetHost;
        }

        public DateTime GetCurrentTime() {
            return DateTimeOffset.FromUnixTimeMilliseconds(_fixedTimestampMillis).UtcDateTime;
        }

        public string GenerateRandomId(string key) {
            string existing;
            if (_generatedIds.TryGetValue(key, out existing)) {
                return existing;
            }
            if (_cacheMode == CacheMode.Locked) {
                throw new InvalidOperationException(
                    "Harness is locked but no cached ID found for key: " + key +
                    ". Run tests in Unlocked mode first to generate IDs.");
            }
            var id = new Random().Next(1000000, 99999998).ToString(CultureInfo.InvariantCulture);
            _generatedIds[key] = id;
            WriteIds();
            return id;
        }

        public decimal GenerateRandomDecimal(string key, decimal min, decimal max, int scale) {
            string existing;
            if (_generatedIds.TryGetValue(key, out existing)) {
                return decimal.Parse(existing, CultureInfo.InvariantCulture);
            }
            if (_cacheMode == CacheMode.Locked) {
                throw new InvalidOperationException(
                    "Harness is locked but no cached value found for key: " + key +
                    ". Run tests in Unlocked mode first to generate values.");
            }
            var raw = min + (decimal)new Random().NextDouble() * (max - min);
            var result = Math.Round(raw, scale);
            _generatedIds[key] = result.ToString(CultureInfo.InvariantCulture);
            WriteIds();
            return result;
        }

        public IRequestIdProvider CreateRequestIdProvider(string key) {
            return new HarnessRequestIdProvider(this, key);
        }

        private void SetDefaultVariableFields(string targetServiceUrl) {
            _varPayload = new List<string>();
            _varQuery = new List<string>();
        }

        private static string Encode(string value) {
            return Uri.EscapeDataString(value); // space -> %20
        }

        private string ResourceDir() {
            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            while (dir != null && !File.Exists(Path.Combine(dir.FullName, "GlobalPayments.Api.Tests.csproj"))) {
                dir = dir.Parent;
            }
            var root = dir != null ? dir.FullName : AppContext.BaseDirectory;
            return Path.Combine(root, "CI", "citestingdata");
        }

        private void WriteIds() {
            var dir = ResourceDir();
            Directory.CreateDirectory(dir);
            File.WriteAllText(
                Path.Combine(dir, _testName + ".json"),
                JsonConvert.SerializeObject(_generatedIds, Formatting.Indented));
        }

        private Dictionary<string, string> LoadIds(bool required) {
            var file = Path.Combine(ResourceDir(), _testName + ".json");
            if (!File.Exists(file)) {
                if (required) {
                    throw new InvalidOperationException(
                        "Harness is locked but ID file not found: " + file +
                        ". Run tests in Unlocked mode first to generate the file.");
                }
                return new Dictionary<string, string>();
            }
            var decoded = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(file));
            return decoded ?? new Dictionary<string, string>();
        }

        private class HarnessRequestIdProvider : IRequestIdProvider {
            private readonly CiTestingHarness _harness;
            private readonly string _key;
            private int _counter;

            public HarnessRequestIdProvider(CiTestingHarness harness, string key) {
                _harness = harness;
                _key = key;
            }

            public int GetRequestId() {
                return int.Parse(_harness.GenerateRandomId(_key + "_" + _counter++), CultureInfo.InvariantCulture);
            }
        }
    }
}
