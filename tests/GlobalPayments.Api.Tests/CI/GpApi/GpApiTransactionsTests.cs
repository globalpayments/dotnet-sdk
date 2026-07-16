using System;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Tests.Utils;
using GlobalPayments.Api.Utils;
using GlobalPayments.Api.Utils.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.CI {
    [TestClass]
    public class GpApiTransactionsTests {
        private const string AppId = "4gPqnGBkppGYvoE5UX9EWQlotTxGUDbs";
        private const string AppKey = "FQyJA5VuEQfcji2M";
        private const decimal Amount = 2.02m;
        private const string Currency = "USD";

        private static CiTestingHarness _harness;
        private CreditCardData _card;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context) {
            _harness = new CiTestingHarness(
                "https://apis.sandbox.globalpay.com/ucp",
                CiTestingHarness.CacheMode.Locked,
                "GpApiTransactionsTests");
        }

        [TestInitialize]
        public void TestInitialize() {
            var now = _harness.GetCurrentTime();
            _card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = now.Month,
                ExpYear = now.Year + 1,
                Cvn = "123",
                CardPresent = true
            };
        }

        private void ConfigureGpApiService() {
            var config = new GpApiConfig {
                AppId = AppId,
                AppKey = AppKey,
                Channel = Channel.CardNotPresent,
                ChallengeNotificationUrl = "https://ensi808o85za.x.pipedream.net/",
                MethodNotificationUrl = "https://ensi808o85za.x.pipedream.net/",
                MerchantContactUrl = "https://enp4qhvjseljg.x.pipedream.net/",
                Country = "US",
                EnableLogging = true,
                RequestLogger = new RequestConsoleLogger(),
                AccessTokenInfo = new AccessTokenInfo {
                    TransactionProcessingAccountName = "transaction_processing",
                    RiskAssessmentAccountName = "EOS_RiskAssessment"
                }
            };
            config.ServiceUrl = _harness.GetTestingUrl();
            ServicesContainer.ConfigureService(config);
        }

        private void AssertTransactionResponse(Transaction transaction, TransactionStatus status) {
            Assert.IsNotNull(transaction);
            Assert.AreEqual("00", transaction.ResponseCode);
            Assert.AreEqual(EnumConverter.GetMapping(Target.GP_API, status), transaction.ResponseMessage);
        }

        [TestMethod]
        public void PostCapture() {
            _harness.SetFunction("GP-API|Transactions|POST Capture");
            ConfigureGpApiService();

            var transaction = _card.Authorize(Amount)
                .WithCurrency(Currency)
                .WithClientTransactionId(_harness.GenerateRandomId("postCapture_auth"))
                .Execute();
            AssertTransactionResponse(transaction, TransactionStatus.Preauthorized);
            
            var capture = transaction.Capture(Amount).Execute();
            AssertTransactionResponse(capture, TransactionStatus.Captured);
        }

        [TestMethod]
        public void PostCharge() {
            _harness.SetFunction("GP-API|Transactions|POST Create");
            ConfigureGpApiService();

            var transaction = _card.Charge(Amount)
                .WithCurrency(Currency)
                .WithClientTransactionId(_harness.GenerateRandomId("postCreate"))
                .Execute();
            AssertTransactionResponse(transaction, TransactionStatus.Captured);
        }
    }
}
