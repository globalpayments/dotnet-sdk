using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using GlobalPayments.Api;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Logging;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.GpApi {
    /// <summary>
    /// Regression coverage for the concurrency defect where the shared, singleton
    /// GpApiConnector mutated its Headers dictionary per request (x-gp-idempotency and
    /// Authorization). Under concurrent load this let callers overwrite or drop one
    /// another's idempotency key, so the gateway could not deduplicate and duplicate
    /// charges were possible; it could also throw "collection was modified" from the
    /// header enumeration during a token refresh.
    ///
    /// All tests run fully offline against a local loopback listener so the exact
    /// ProcessAuthorization -> DoTransaction -> DoTransactionWithIdempotencyKey ->
    /// SendRequest path is exercised without touching the sandbox.
    /// </summary>
    [TestClass]
    public class GpApiConcurrencyTest {
        private const string ConfigName = "gpapi-concurrency-test";
        private const string IdempotencyHeader = "x-gp-idempotency";

        private const string SuccessBody =
            "{\"id\":\"TRN_TEST\",\"status\":\"CAPTURED\",\"amount\":\"9810\",\"currency\":\"USD\"," +
            "\"action\":{\"result_code\":\"SUCCESS\",\"type\":\"AUTHORIZE\"}}";

        // Parseable by GpApiConnector.HandleResponse -> ResponseCode "NOT_AUTHENTICATED",
        // which drives DoTransaction's re-authentication + retry branch.
        private const string NotAuthenticatedBody =
            "{\"error_code\":\"NOT_AUTHENTICATED\",\"detailed_error_code\":\"40001\"," +
            "\"detailed_error_description\":\"Invalid access token\"}";

        private HttpListener _listener;
        private string _baseUrl;

        // Idempotency header values observed on requests that were answered with 200.
        private readonly ConcurrentBag<string> _receivedKeys = new ConcurrentBag<string>();
        // Number of times each idempotency key has reached the server (used to fail-once).
        private readonly ConcurrentDictionary<string, int> _hitCounts = new ConcurrentDictionary<string, int>();

        // Per-test hook deciding the HTTP status for a request, given its idempotency key
        // and how many times that key has now been seen. Defaults to always-200.
        private Func<string, int, int> _statusFor = (key, hitCount) => (int)HttpStatusCode.OK;

        [TestCleanup]
        public void TearDown() {
            try {
                if (_listener != null && _listener.IsListening) {
                    _listener.Stop();
                }
                _listener?.Close();
            }
            catch {
                /* listener already torn down */
            }
        }

        [TestMethod]
        public void ConcurrentCharges_EachRequestKeepsItsOwnIdempotencyKey() {
            // Verifies the core anti-double-charge invariant: under heavy concurrency every
            // request carries its OWN idempotency key, none is dropped, none is duplicated
            // onto another caller's request.
            StartServer(GpApiConfig());

            const int rounds = 10;
            const int concurrentCallsPerRound = 50;

            var sentKeys = new List<string>();
            var failures = new ConcurrentQueue<Exception>();

            for (int round = 0; round < rounds; round++) {
                var roundKeys = Enumerable.Range(0, concurrentCallsPerRound)
                    .Select(_ => Guid.NewGuid().ToString())
                    .ToArray();
                sentKeys.AddRange(roundKeys);
                ChargeConcurrently(roundKeys, failures);
            }

            AssertNoFailures(failures);
            var received = _receivedKeys.ToList();
            Assert.IsFalse(received.Contains("<MISSING>"),
                "A request was sent with no x-gp-idempotency header (key dropped by a concurrent caller).");
            CollectionAssert.AreEquivalent(sentKeys, received,
                "The idempotency keys the server received do not match those sent; keys were overwritten or dropped across threads.");
        }

        [TestMethod]
        public void ConcurrentChargesWithForcedReauth_KeepIdempotencyKeysAndDoNotThrow() {
            // Forces ~1-in-4 requests to 401 on first sight, driving concurrent re-authentication
            // (the AccessToken setter writes Authorization onto the shared Headers) WHILE other
            // threads snapshot Headers in SendRequest. This exercises the _headerLock write path.
            // The 401'd request retries with the SAME idempotency key, which is now "seen", so the
            // retry succeeds. Each key must still be delivered exactly once with its own header.
            _statusFor = (key, hitCount) =>
                (hitCount == 1 && FailOnFirstHit(key))
                    ? (int)HttpStatusCode.Unauthorized
                    : (int)HttpStatusCode.OK;

            StartServer(GpApiConfig());

            const int rounds = 8;
            const int concurrentCallsPerRound = 50;

            var sentKeys = new List<string>();
            var failures = new ConcurrentQueue<Exception>();

            for (int round = 0; round < rounds; round++) {
                var roundKeys = Enumerable.Range(0, concurrentCallsPerRound)
                    .Select(_ => Guid.NewGuid().ToString())
                    .ToArray();
                sentKeys.AddRange(roundKeys);
                ChargeConcurrently(roundKeys, failures);
            }

            AssertNoFailures(failures);
            var received = _receivedKeys.ToList();
            Assert.IsFalse(received.Contains("<MISSING>"),
                "A request was sent with no x-gp-idempotency header during concurrent re-auth.");
            // Only the successful (200) deliveries were recorded; each key should appear exactly once.
            CollectionAssert.AreEquivalent(sentKeys, received,
                "Idempotency keys were lost or duplicated while Authorization was being rewritten concurrently.");
        }

        [TestMethod]
        public void ConcurrentChargesWithProductionLogging_DoNotThrow() {
            // With logging enabled in PRODUCTION the connector builds a masked request log, which
            // reads MaskedRequestData and drives ProtectSensitiveData / MaskedValueCollection.
            // Before the fix that shared masking state corrupted under concurrency and threw. This
            // asserts the path is now crash-safe under load.
            var config = GpApiConfig();
            config.Environment = Entities.Environment.PRODUCTION;
            config.EnableLogging = true;
            config.RequestLogger = new NoOpRequestLogger();

            StartServer(config);

            const int rounds = 8;
            const int concurrentCallsPerRound = 50;

            var failures = new ConcurrentQueue<Exception>();
            for (int round = 0; round < rounds; round++) {
                var roundKeys = Enumerable.Range(0, concurrentCallsPerRound)
                    .Select(_ => Guid.NewGuid().ToString())
                    .ToArray();
                ChargeConcurrently(roundKeys, failures);
            }

            AssertNoFailures(failures);
        }

        // ---- helpers ------------------------------------------------------------------

        private static void ChargeConcurrently(IEnumerable<string> keys, ConcurrentQueue<Exception> failures) {
            Parallel.ForEach(keys, key => {
                try {
                    var card = new CreditCardData {
                        Number = "4263970000005262",
                        ExpMonth = 12,
                        ExpYear = DateTime.Now.Year + 1,
                        Cvn = "131",
                        CardHolderName = "James Mason"
                    };

                    card.Charge(98.10m)
                        .WithCurrency("USD")
                        .WithIdempotencyKey(key)
                        .Execute(ConfigName);
                }
                catch (Exception ex) {
                    failures.Enqueue(ex);
                }
            });
        }

        private static void AssertNoFailures(ConcurrentQueue<Exception> failures) {
            Assert.IsTrue(failures.IsEmpty,
                "Concurrent Execute() calls threw: " +
                string.Join(" | ", failures.Select(e => $"{e.GetType().Name}: {e.Message}")));
        }

        // Deterministic ~25% selection so the test does not depend on Random.
        private static bool FailOnFirstHit(string key) {
            unchecked {
                int hash = 17;
                foreach (var c in key) {
                    hash = hash * 31 + c;
                }
                return (hash & 3) == 0;
            }
        }

        private GpApiConfig GpApiConfig() {
            return new GpApiConfig {
                AppId = "test-app-id",
                AppKey = "test-app-key",
                Channel = Channel.CardNotPresent,
                Country = "US",
                ServiceUrl = _baseUrl,
                MethodNotificationUrl = "https://example.com/method",
                ChallengeNotificationUrl = "https://example.com/challenge",
                MerchantContactUrl = "https://example.com/contact",
                // A preset token short-circuits the network sign-in (see GpApiConnector.SignIn),
                // while still exercising the Authorization write on the shared Headers.
                AccessTokenInfo = new AccessTokenInfo {
                    Token = "preset-access-token",
                    TransactionProcessingAccountName = "transaction_processing"
                }
            };
        }

        private void StartServer(GpApiConfig config) {
            var port = GetFreeTcpPort();
            _baseUrl = $"http://localhost:{port}";
            config.ServiceUrl = _baseUrl;

            _listener = new HttpListener();
            _listener.Prefixes.Add($"http://localhost:{port}/");
            _listener.Start();
            PumpRequests(_listener);

            ServicesContainer.ConfigureService(config, ConfigName);
        }

        private void PumpRequests(HttpListener listener) {
            Task.Run(async () => {
                while (listener.IsListening) {
                    HttpListenerContext context;
                    try {
                        context = await listener.GetContextAsync();
                    }
                    catch {
                        break; // listener stopped
                    }
                    _ = Task.Run(() => HandleRequest(context));
                }
            });
        }

        private void HandleRequest(HttpListenerContext context) {
            try {
                var key = context.Request.Headers[IdempotencyHeader];
                var normalizedKey = string.IsNullOrEmpty(key) ? "<MISSING>" : key;
                var hitCount = _hitCounts.AddOrUpdate(normalizedKey, 1, (_, prev) => prev + 1);

                var status = _statusFor(normalizedKey, hitCount);
                if (status == (int)HttpStatusCode.OK) {
                    // Record the header only for delivered (accepted) requests.
                    _receivedKeys.Add(normalizedKey);
                }

                var body = status == (int)HttpStatusCode.OK ? SuccessBody : NotAuthenticatedBody;
                var buffer = Encoding.UTF8.GetBytes(body);
                context.Response.StatusCode = status;
                context.Response.ContentType = "application/json";
                context.Response.ContentLength64 = buffer.Length;
                context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                context.Response.OutputStream.Close();
            }
            catch {
                /* client went away; nothing to do */
            }
        }

        private static int GetFreeTcpPort() {
            var probe = new TcpListener(IPAddress.Loopback, 0);
            probe.Start();
            var port = ((IPEndPoint)probe.LocalEndpoint).Port;
            probe.Stop();
            return port;
        }

        private sealed class NoOpRequestLogger : IRequestLogger {
            public void RequestSent(string request) { }
            public void ResponseReceived(string response) { }
        }
    }
}
