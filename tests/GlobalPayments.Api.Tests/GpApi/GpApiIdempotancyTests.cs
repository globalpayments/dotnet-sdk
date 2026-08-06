using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Logging;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Utils.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPayments.Api.Tests.GpApi
{
    [TestClass]
    public class GpApiIdempotancyTests : BaseGpApiTests
    {
        private CreditCardData card;
        private const decimal AMOUNT = 7.8m;
        private const string CURRENCY = "GBP";
        private GpApiConfig gpApiConfig = null;

        [TestInitialize]
        public void TestInitialize()
        {
            ServicesContainer.RemoveConfig();
            gpApiConfig = GpApiConfigSetup(AppId, AppKey, Channel.CardNotPresent);
            //gpApiConfig.RequestLogger = new RequestFileLogger("C://test.txt");
            ServicesContainer.ConfigureService(gpApiConfig);

            card = new CreditCardData
            {
                Number = "4263970000005262",
                ExpMonth = ExpMonth,
                ExpYear = ExpYear,
                Cvn = "123",
                CardPresent = true
            };
        }

        #region AH-2819 Race Condition Reproduction

        // These tests reproduce the thread-safety defect described in AH-2819 / GitHub issue #98.
        // GpApiConnector is a singleton and stages the idempotency key on its shared Headers
        // dictionary before sending. Under concurrent load, one thread can overwrite, strip, or
        // enumerate-while-mutating another thread's key. Because every request below uses a UNIQUE
        // key, the gateway must never see a duplicate and must never return DUPLICATE_ACTION (40039)
        // — so any DUPLICATE_ACTION, or the generic "Error occurred while communicating with gateway"
        // wrapper (the collection-modified crash), is direct evidence that the SDK corrupted a key.
        // Being a race, reproduction is probabilistic; higher concurrency raises the hit rate.

        /// <summary>
        /// Fires many concurrent charges, each with its own unique idempotency key, and asserts that
        /// none fail. With unique keys, a DUPLICATE_ACTION or a communication error can only be caused
        /// by the shared-Headers race swapping or stripping a key between threads.
        /// </summary>
        [TestMethod]
        public void ConcurrentCharges_WithUniqueIdempotencyKeys_ShouldNotCorruptKeys()
        {
            const int concurrentRequests = 20;
            var failures = new System.Collections.Concurrent.ConcurrentBag<string>();

            Parallel.For(0, concurrentRequests,
                new ParallelOptions { MaxDegreeOfParallelism = concurrentRequests },
                i =>
                {
                    var idempotencyKey = Guid.NewGuid().ToString();
                    try
                    {
                        card.Charge(AMOUNT)
                            .WithCurrency(CURRENCY)
                            .WithIdempotencyKey(idempotencyKey)
                            .Execute();
                    }
                    catch (GatewayException ex)
                    {
                        failures.Add($"[{i}] key={idempotencyKey} code={ex.ResponseCode} msg={ex.ResponseMessage} :: {ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        failures.Add($"[{i}] key={idempotencyKey} UNEXPECTED {ex.GetType().Name} :: {ex.Message}");
                    }
                });

            Assert.AreEqual(0, failures.Count,
                $"Idempotency race reproduced: {failures.Count}/{concurrentRequests} unique-key charges failed. " +
                "With unique keys none should fail; failures indicate a corrupted/absent idempotency key.\n" +
                string.Join("\n", failures));
        }

        /// <summary>
        /// High-concurrency, multi-round stress variant to raise the probability of hitting the race.
        /// Each charge uses a unique idempotency key, so any DUPLICATE_ACTION (40039) across the whole
        /// run proves a key was swapped onto another in-flight request by the shared Headers dictionary.
        /// </summary>
        [TestMethod]
        public void ConcurrentCharges_WithUniqueIdempotencyKeys_HighConcurrencyStress()
        {
            const int rounds = 10;
            const int concurrentRequests = 60;

            // Use normal auth (no preset token so SignIn fetches a real token) and bypass the in-source
            // IsFirstTransaction debug fault so real charges occur and the key race can surface.
            ServicesContainer.RemoveConfig();
            ServicesContainer.ConfigureService(gpApiConfig);

            var duplicateActions = new System.Collections.Concurrent.ConcurrentBag<string>();
            var communicationErrors = new System.Collections.Concurrent.ConcurrentBag<string>();
            var otherFailures = new System.Collections.Concurrent.ConcurrentBag<string>();

            for (int round = 0; round < rounds; round++)
            {
                var barrier = new System.Threading.Barrier(concurrentRequests);
                Parallel.For(0, concurrentRequests,
                    new ParallelOptions { MaxDegreeOfParallelism = concurrentRequests },
                    i =>
                    {
                        var idempotencyKey = Guid.NewGuid().ToString();
                        barrier.SignalAndWait(); // release all threads simultaneously to widen the race
                        try
                        {
                            card.Charge(AMOUNT)
                                .WithCurrency(CURRENCY)
                                .WithIdempotencyKey(idempotencyKey)
                                .Execute();
                        }
                        catch (GatewayException ex)
                        {
                            if (ex.ResponseCode == "DUPLICATE_ACTION" || ex.ResponseMessage == "40039")
                            {
                                duplicateActions.Add($"round={round} key={idempotencyKey} :: {ex.Message}");
                            }
                            else if (ex.Message != null && ex.Message.Contains("Error occurred while communicating with gateway"))
                            {
                                communicationErrors.Add($"round={round} key={idempotencyKey} :: {ex.Message}");
                            }
                            else
                            {
                                otherFailures.Add($"round={round} key={idempotencyKey} code={ex.ResponseCode} :: {ex.Message}");
                            }
                        }
                        catch (Exception ex)
                        {
                            otherFailures.Add($"round={round} key={idempotencyKey} UNEXPECTED {ex.GetType().Name} :: {ex.Message}");
                        }
                    });
            }

            var reproduced = duplicateActions.Count + communicationErrors.Count;
            Assert.AreEqual(0, reproduced,
                $"Idempotency race reproduced across {rounds} rounds of {concurrentRequests} unique-key charges. " +
                $"DUPLICATE_ACTION(40039)={duplicateActions.Count} (key swapped onto another request), " +
                $"communication-errors={communicationErrors.Count} (collection-modified crash), " +
                $"other={otherFailures.Count}.\n" +
                "DUPLICATE_ACTION:\n" + string.Join("\n", duplicateActions) + "\n" +
                "COMMUNICATION ERRORS:\n" + string.Join("\n", communicationErrors) + "\n" +
                "OTHER:\n" + string.Join("\n", otherFailures));
        }

        /// <summary>
        /// Reproduces the 401 "token storm" amplifier from AH-2819 / issue #98. The bearer token lives
        /// in the same shared Headers dictionary as the idempotency key, so when the token is invalid
        /// every concurrent caller receives 401 NOT_AUTHENTICATED at once and piles into the
        /// SignIn-and-retry path together. The retry re-enters DoTransactionWithIdempotencyKey, writing
        /// the key to shared Headers a second time and widening the race window. This test starts the
        /// shared connector with a deliberately invalid access token to force that storm; with unique
        /// keys, any DUPLICATE_ACTION (40039) proves a key was swapped during the retry storm.
        /// </summary>
        [TestMethod]
        public void ConcurrentCharges_UnderTokenExpiry401Storm_ShouldNotCorruptKeys()
        {
            const int rounds = 5;
            const int concurrentRequests = 30;

            var duplicateActions = new System.Collections.Concurrent.ConcurrentBag<string>();
            var communicationErrors = new System.Collections.Concurrent.ConcurrentBag<string>();
            var otherFailures = new System.Collections.Concurrent.ConcurrentBag<string>();

            for (int round = 0; round < rounds; round++)
            {
                // Re-create the shared connector with an invalid bearer token before each round so the
                // first attempt of every concurrent request gets a real 401 from the gateway and runs
                // the SignIn-and-retry path together — a fresh "token expiry" storm each round.
                // IsFirstTransaction is false to bypass any local debug fault injection in the connector.
                ServicesContainer.RemoveConfig();
                gpApiConfig.AccessTokenInfo = new AccessTokenInfo
                {
                    Token = "invalid-token-to-force-401-storm"
                };
                ServicesContainer.ConfigureService(gpApiConfig);

                var currentRound = round;
                var barrier = new System.Threading.Barrier(concurrentRequests);

                Parallel.For(0, concurrentRequests,
                    new ParallelOptions { MaxDegreeOfParallelism = concurrentRequests },
                    i =>
                    {
                        var idempotencyKey = Guid.NewGuid().ToString();
                        barrier.SignalAndWait(); // release all threads into the 401 storm simultaneously
                        try
                        {
                            card.Charge(AMOUNT)
                                .WithCurrency(CURRENCY)
                                .WithIdempotencyKey(idempotencyKey)
                                .Execute();
                        }
                        catch (GatewayException ex)
                        {
                            if (ex.ResponseCode == "DUPLICATE_ACTION" || ex.ResponseMessage == "40039")
                            {
                                duplicateActions.Add($"round={currentRound} key={idempotencyKey} :: {ex.Message}");
                            }
                            else if (ex.Message != null && ex.Message.Contains("Error occurred while communicating with gateway"))
                            {
                                communicationErrors.Add($"round={currentRound} key={idempotencyKey} :: {ex.Message}");
                            }
                            else
                            {
                                otherFailures.Add($"round={currentRound} key={idempotencyKey} code={ex.ResponseCode} :: {ex.Message}");
                            }
                        }
                        catch (Exception ex)
                        {
                            otherFailures.Add($"round={currentRound} key={idempotencyKey} UNEXPECTED {ex.GetType().Name} :: {ex.Message}");
                        }
                    });
            }

            var reproduced = duplicateActions.Count + communicationErrors.Count;
            Assert.AreEqual(0, reproduced,
                $"401 token-storm race reproduced across {rounds} rounds of {concurrentRequests} unique-key charges. " +
                $"DUPLICATE_ACTION(40039)={duplicateActions.Count} (key swapped during retry storm), " +
                $"communication-errors={communicationErrors.Count} (collection-modified crash), " +
                $"other={otherFailures.Count}.\n" +
                "DUPLICATE_ACTION:\n" + string.Join("\n", duplicateActions) + "\n" +
                "COMMUNICATION ERRORS:\n" + string.Join("\n", communicationErrors) + "\n" +
                "OTHER:\n" + string.Join("\n", otherFailures));
        }

        /// <summary>
        /// Reproduces AH-2819 using the real-world consumer pattern:
        ///   transactionReference is used as BOTH the idempotency key AND the client transaction ID,
        ///   with EUR currency and an order number — exactly as a typical integration would call it.
        /// Any DUPLICATE_ACTION proves the shared Headers dictionary swapped the key onto a
        /// different thread's request.
        /// </summary>
        [TestMethod]
        public void ConcurrentCharges_WithClientTransactionIdPattern_ShouldNotCorruptKeys()
        {
            const int rounds = 10;
            const int concurrentRequests = 60;

            ServicesContainer.RemoveConfig();
            ServicesContainer.ConfigureService(gpApiConfig);

            var duplicateActions = new System.Collections.Concurrent.ConcurrentBag<string>();
            var communicationErrors = new System.Collections.Concurrent.ConcurrentBag<string>();
            var otherFailures = new System.Collections.Concurrent.ConcurrentBag<string>();

            for (int round = 0; round < rounds; round++)
            {
                var barrier = new System.Threading.Barrier(concurrentRequests);
                Parallel.For(0, concurrentRequests,
                    new ParallelOptions { MaxDegreeOfParallelism = concurrentRequests },
                    i =>
                    {
                        // Mirrors the real consumer pattern: same value for both idempotency key
                        // and client transaction ID, plus EUR currency and an order number.
                        var transactionReference = Guid.NewGuid().ToString();
                        var orderNumber = $"ORD-{round}-{i}-{Guid.NewGuid().ToString("N").Substring(0, 8)}";
                        var amount = AMOUNT;

                        barrier.SignalAndWait(); // release all threads simultaneously to maximise collision
                        try
                        {
                            card.Charge(amount)
                                .WithIdempotencyKey(transactionReference)
                                .WithClientTransactionId(transactionReference)
                                .WithCurrency("EUR")
                                .WithOrderId(orderNumber)
                                .Execute();
                        }
                        catch (GatewayException ex)
                        {
                            if (ex.ResponseCode == "DUPLICATE_ACTION" || ex.ResponseMessage == "40039")
                            {
                                duplicateActions.Add($"round={round} ref={transactionReference} order={orderNumber} :: {ex.Message}");
                            }
                            else if (ex.Message != null && ex.Message.Contains("Error occurred while communicating with gateway"))
                            {
                                communicationErrors.Add($"round={round} ref={transactionReference} :: {ex.Message}");
                            }
                            else
                            {
                                otherFailures.Add($"round={round} ref={transactionReference} code={ex.ResponseCode} :: {ex.Message}");
                            }
                        }
                        catch (Exception ex)
                        {
                            otherFailures.Add($"round={round} ref={transactionReference} UNEXPECTED {ex.GetType().Name} :: {ex.Message}");
                        }
                    });
            }

            var reproduced = duplicateActions.Count + communicationErrors.Count;
            Assert.AreEqual(0, reproduced,
                $"Idempotency race reproduced across {rounds} rounds of {concurrentRequests} unique-key charges " +
                $"(WithClientTransactionId pattern, EUR). " +
                $"DUPLICATE_ACTION(40039)={duplicateActions.Count} (key swapped onto another request), " +
                $"communication-errors={communicationErrors.Count} (collection-modified crash), " +
                $"other={otherFailures.Count}.\n" +
                "DUPLICATE_ACTION:\n" + string.Join("\n", duplicateActions) + "\n" +
                "COMMUNICATION ERRORS:\n" + string.Join("\n", communicationErrors) + "\n" +
                "OTHER:\n" + string.Join("\n", otherFailures));
        }

        /// <summary>
        /// Reproduces the AH-2819 request-log masking race (GitHub issue #98, secondary defect).
        /// A <see cref="IRequestLogger"/> is attached so the request-logging path is exercised, and
        /// every charge carries a PAN + CVV so each request build accumulates masked values. On the
        /// pre-fix code the masked-value state lived on shared static fields (the
        /// <c>ProtectSensitiveData</c> accumulator and the static <c>Request.MaskedValues</c>), so
        /// concurrent builds corrupted one another's collection — surfacing as a raw
        /// <see cref="NullReferenceException"/> or <see cref="InvalidOperationException"/>
        /// ("Operations that change non-concurrent collections must have exclusive access ... corrupted
        /// its state") thrown outside the gateway try/catch during request building. With request-scoped
        /// masking these must never occur, regardless of how many DUPLICATE_ACTION idempotency results
        /// the gateway returns (those are irrelevant here and are ignored).
        /// </summary>
        [TestMethod]
        public void ConcurrentCharges_WithRequestLogger_ShouldNotCorruptMaskedValues()
        {
            const int rounds = 20;
            const int concurrentRequests = 150;

            // Attach a discarding logger so GenerateRequestLog runs (exercising the masking path)
            // without leaking the unmasked request body to console/file.
            ServicesContainer.RemoveConfig();
            gpApiConfig.RequestLogger = new DiscardingRequestLogger();
            ServicesContainer.ConfigureService(gpApiConfig);

            var maskingCrashes = new System.Collections.Concurrent.ConcurrentBag<string>();

            for (int round = 0; round < rounds; round++)
            {
                var barrier = new System.Threading.Barrier(concurrentRequests);
                Parallel.For(0, concurrentRequests,
                    new ParallelOptions { MaxDegreeOfParallelism = concurrentRequests },
                    i =>
                    {
                        barrier.SignalAndWait(); // release all threads simultaneously to widen the masking race
                        try
                        {
                            card.Charge(AMOUNT)
                                .WithCurrency(CURRENCY)
                                .WithIdempotencyKey(Guid.NewGuid().ToString())
                                .Execute();
                        }
                        catch (NullReferenceException ex)
                        {
                            // Read side: Request.MaskedValues nulled by another thread mid-use.
                            maskingCrashes.Add($"round={round} req={i} NullReferenceException :: {ex.Message}\nSTACK:\n{ex.StackTrace}");
                        }
                        catch (InvalidOperationException ex)
                        {
                            // Write side: concurrent mutation of the shared static masking dictionary.
                            maskingCrashes.Add($"round={round} req={i} InvalidOperationException :: {ex.Message}\nSTACK:\n{ex.StackTrace}");
                        }
                        catch (GatewayException)
                        {
                            // DUPLICATE_ACTION and other gateway-level results are irrelevant to the masking race.
                        }
                    });
            }

            Assert.AreEqual(0, maskingCrashes.Count,
                $"Request-log masking race reproduced across {rounds} rounds of {concurrentRequests} concurrent charges: " +
                $"{maskingCrashes.Count} builds crashed while accumulating masked-value state " +
                "(NullReferenceException / non-concurrent-collection corruption). Masked values must be request-scoped.\n" +
                string.Join("\n", maskingCrashes));
        }

        /// <summary>No-op <see cref="IRequestLogger"/> that discards output so concurrent request
        /// building is exercised without writing the unmasked request body anywhere.</summary>
        private sealed class DiscardingRequestLogger : IRequestLogger
        {
            public void RequestSent(string request) { }
            public void ResponseReceived(string response) { }
        }

        #endregion
    }
}
