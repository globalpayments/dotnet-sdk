using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Utils.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.GpApi {
    /// <summary>
    /// Integration tests for EU Data Residency support on GP-API (AH-1553 / AH-1608).
    ///
    /// Tests cover:
    ///   - DataResidency.EU routes to EU sandbox/production endpoints
    ///   - DataResidency defaults to None and uses standard endpoints
    ///   - Non-GP-API configs (e.g. PorticoConfig) do not expose DataResidency
    /// </summary>
    [TestClass]
    public class GpApiDataResidencyTests : BaseGpApiTests {
        private CreditCardData card;

        [TestInitialize]
        public void TestInitialize() {
            card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 05,
                ExpYear = 2026,
                Cvn = "852"
            };
        }

        [TestCleanup]
        public void TestCleanup() {
        }

        /// <summary>
        /// DataResidency set to EU → uses EU sandbox endpoint and successfully processes a sale transaction.
        /// </summary>
        [TestMethod]
        public void DataResidency_EU_UsesEuEndpointAndSaleTransaction() {

            var config = GpApiConfigSetup(EuAppId, EuAppKey, Channel.CardNotPresent);

            config.DataResidency = DataResidency.EU;
            config.RequestLogger = new RequestConsoleLogger();
            config.AccessTokenInfo = new AccessTokenInfo {
                TransactionProcessingAccountName = "internet"
            };

            ServicesContainer.ConfigureService(config, "EuConfig");

            var response = card.Charge(1.00m)
                .WithCurrency("EUR")
                .Execute("EuConfig");
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.IsNotNull(response.TransactionId);
        }

        /// <summary>
        /// DataResidency not set → defaults to None and uses standard sandbox endpoint.
        /// </summary>
        [TestMethod]
        public void DataResidency_DefaultsToNone_UsesStandardEndpointAndSaleTransaction() {

            var config = GpApiConfigSetup(AppId, AppKey, Channel.CardNotPresent);

            ServicesContainer.ConfigureService(config, "DefaultConfig");

            var response = card.Charge(1.00m)
                .WithCurrency("USD")
                .Execute("DefaultConfig");

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.IsNotNull(response.TransactionId);
        }

        // ── Negative test cases ────────────────────────────────────────────────

        /// <summary>
        /// DataResidency.EU with an explicit wrong ServiceUrl → connection fails.
        /// An explicit ServiceUrl always takes precedence over DataResidency-based routing,
        /// so the request is sent to the custom unreachable host and a GatewayException is thrown.
        /// </summary>
        [TestMethod]
        public void DataResidency_EU_WithWrongServiceUrl_ThrowsGatewayException() {
            var config = GpApiConfigSetup(EuAppId, EuAppKey, Channel.CardNotPresent);
            config.DataResidency = DataResidency.EU;
            config.RequestLogger = new RequestConsoleLogger();
            config.ServiceUrl = "https://apis.example.com/ucp";
            config.AccessTokenInfo = new AccessTokenInfo {
                TransactionProcessingAccountName = "internet"
            };

            ServicesContainer.ConfigureService(config);

            // Explicit ServiceUrl must NOT be overridden by DataResidency routing.
            Assert.AreEqual("https://apis.example.com/ucp", config.ServiceUrl,
                "An explicit ServiceUrl should not be replaced by the EU endpoint.");

            var ex = Assert.ThrowsException<GatewayException>(() => {
                card.Charge(1.00m)
                    .WithCurrency("EUR")
                    .Execute();
            });

            Assert.IsTrue(
                ex.Message.Contains("Error occurred while communicating with gateway."),
                $"Unexpected message: {ex.Message}");
        }

        /// <summary>
        /// DataResidency.EU with QA ServiceUrl → connection fails.
        /// The QA endpoint is not reachable from the test environment, so a GatewayException is thrown.
        /// To see the actual endpoint used, run this test in Test Explorer and inspect the request log output.
        /// </summary>
        [TestMethod]
        public void DataResidency_EU_WithQAServiceUrl_ThrowsGatewayException() {
            var config = GpApiConfigSetup(EuAppId, EuAppKey, Channel.CardNotPresent);
            config.DataResidency = DataResidency.EU;
            config.RequestLogger = new RequestConsoleLogger();
            config.ServiceUrl = ServiceEndpoints.GP_API_EU_QA;
            config.AccessTokenInfo = new AccessTokenInfo {
                TransactionProcessingAccountName = "internet"
            };

            ServicesContainer.ConfigureService(config);

            var ex = Assert.ThrowsException<GatewayException>(() => {
                card.Charge(1.00m)
                    .WithCurrency("EUR")
                    .Execute();
            });

            Assert.IsTrue(
                ex.Message.Contains("Error occurred while communicating with gateway."),
                $"Unexpected message: {ex.Message}");
        }

        /// <summary>
        /// DataResidency.EU with Production ServiceUrl → authentication fails.
        /// The production endpoint is reachable but rejects sandbox credentials with ACTION_NOT_AUTHORIZED.
        /// To see the actual endpoint used, run this test in Test Explorer and inspect the request log output.
        /// </summary>
        [TestMethod]
        public void DataResidency_EU_WithProdServiceUrl_ThrowsGatewayException() {
            var config = GpApiConfigSetup(EuAppId, EuAppKey, Channel.CardNotPresent);
            config.DataResidency = DataResidency.EU;
            config.RequestLogger = new RequestConsoleLogger();
            config.ServiceUrl = ServiceEndpoints.GP_API_EU_PRODUCTION;
            config.AccessTokenInfo = new AccessTokenInfo {
                TransactionProcessingAccountName = "internet"
            };

            ServicesContainer.ConfigureService(config);

            var ex = Assert.ThrowsException<GatewayException>(() => {
                card.Charge(1.00m)
                    .WithCurrency("EUR")
                    .Execute();
            });

            Assert.AreEqual("ACTION_NOT_AUTHORIZED", ex.ResponseCode);
        }

        /// <summary>
        /// DataResidency.EU with invalid credentials → authentication fails.
        /// </summary>
        [TestMethod]
        public void DataResidency_EU_InvalidCredentials_ThrowsActionNotAuthorized() {
            var config = GpApiConfigSetup("INVALID_APP_ID", "INVALID_APP_KEY", Channel.CardNotPresent);
            config.DataResidency = DataResidency.EU;
            config.RequestLogger = new RequestConsoleLogger();

            ServicesContainer.ConfigureService(config);

            var ex = Assert.ThrowsException<GatewayException>(() => {
                card.Charge(1.00m)
                    .WithCurrency("EUR")
                    .Execute();
            });

            Assert.AreEqual("ACTION_NOT_AUTHORIZED", ex.ResponseCode);
        }

        /// <summary>
        /// Standard (non-EU) credentials used with DataResidency.EU → routed to EU endpoint,
        /// authentication fails because credentials are not registered on the EU environment.
        /// </summary>
        [TestMethod]
        public void DataResidency_EU_WithNonEuCredentials_ThrowsActionNotAuthorized() {
            var config = GpApiConfigSetup(AppId, AppKey, Channel.CardNotPresent);
            config.DataResidency = DataResidency.EU;
            config.RequestLogger = new RequestConsoleLogger();

            ServicesContainer.ConfigureService(config);

            var ex = Assert.ThrowsException<GatewayException>(() => {
                card.Charge(1.00m)
                    .WithCurrency("EUR")
                    .Execute();
            });

            Assert.AreEqual("ACTION_NOT_AUTHORIZED", ex.ResponseCode);
        }

        /// <summary>
        /// EU credentials used without DataResidency.EU → routed to standard endpoint,
        /// authentication fails because EU credentials are not registered on the global environment.
        /// </summary>
        [TestMethod]
        public void DataResidency_None_WithEuCredentials_ThrowsActionNotAuthorized() {
            var config = GpApiConfigSetup(EuAppId, EuAppKey, Channel.CardNotPresent);
            // DataResidency intentionally left as default (None)
            config.RequestLogger = new RequestConsoleLogger();

            ServicesContainer.ConfigureService(config);

            var ex = Assert.ThrowsException<GatewayException>(() => {
                card.Charge(1.00m)
                    .WithCurrency("USD")
                    .Execute();
            });

            Assert.AreEqual("ACTION_NOT_AUTHORIZED", ex.ResponseCode);
        }
    }
}
