using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Enums;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Utils;
using GlobalPayments.Api.Utils.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.GpApi {
    [TestClass]
    public class GpApiBatchCloseWithoutIdTest : BaseGpApiTests {
        private CreditCardData card;

        private const string Currency = "GBP";
        private const decimal Amount = 100m;

        // Multi-merchant (MMA-scoped) app: token scope is PAR_..., requires MerchantId on the config.
        private const string MultiMerchantAppId = "wuqol13QQJu0vcjbdN9892cl5IcyJgPU";
        private const string MultiMerchantAppKey = "jdOJNxsGbE0nLeTx";
        private const string MultiMerchantTargetMerchantId = "MER_3abc0724f49c40e59a61309ae6d37dfd";
        private const string MultiMerchantAccountName = "test_account_CungkardAk";

        // Standalone merchant app A: token scope is MER_..., no MerchantId required.
        // Used inline (configured as default within the test method).
        private const string StandaloneMerchantInlineAppId = "UDVB5ngQEEn6wPLA9gJZMj25Uw6B9wXcXbd9XTu7tEq9pbUg";
        private const string StandaloneMerchantInlineAppKey = "UICgLDNtzLXK7L2p8AGcRfaf9PzvYPCuVGJxQoWjqUCr2BERs80xNBuSDe9uB9G7";
        private const string StandaloneMerchantInlineAccountName = "test_account_QxiWq2X9sg";

        // Standalone merchant app B: registered as a named config in TestInitialize.
        private const string StandaloneMerchantNamedAppId = "naL0LWpbHEHJSw9aSmap1u3W1rqusAA4587WzEBikVG4zbEK";
        private const string StandaloneMerchantNamedAppKey = "LeC5h0BuYfVnvTe3b0732AHG2HGtxptNNq9RfoQcnP0sozfKkYyLvNtiQf5md3BB";

        // Named GP API config keys.
        private const string MultiMerchantNamedConfig = "MultiMerchantNamedConfig";
        private const string StandaloneMerchantNamedConfig = "StandaloneMerchantNamedConfig";

        [TestInitialize]
        public void TestInitialize() {
            ServicesContainer.RemoveConfig();

            var multiMerchantConfig = GpApiConfigSetup(MultiMerchantAppId, MultiMerchantAppKey, Channel.CardPresent);
            multiMerchantConfig.MerchantId = MultiMerchantTargetMerchantId;
            multiMerchantConfig.Country = "US";
            multiMerchantConfig.AccessTokenInfo = new AccessTokenInfo {
                TransactionProcessingAccountName = MultiMerchantAccountName
            };

            ServicesContainer.ConfigureService(multiMerchantConfig);
            ServicesContainer.ConfigureService(multiMerchantConfig, MultiMerchantNamedConfig);

            var standaloneNamedConfig = new GpApiConfig {
                AppId = StandaloneMerchantNamedAppId,
                AppKey = StandaloneMerchantNamedAppKey,
                Channel = Channel.CardPresent,
                RequestLogger = new RequestConsoleLogger(),
                EnableLogging = true,
                Country = "US",
                AccessTokenInfo = new AccessTokenInfo {
                    TransactionProcessingAccountName = MultiMerchantAccountName
                }
            };
            ServicesContainer.ConfigureService(standaloneNamedConfig, StandaloneMerchantNamedConfig);

            card = new CreditCardData {
                Number = "5425230000004415",
                ExpMonth = 12,
                ExpYear = 26,
                Cvn = "123",
                CardPresent = true
            };
        }

        #region Positive Tests - Multi-Merchant (MMA app + MerchantId)

        /// <summary>
        /// Multi-merchant default config: close batch without a batch ID using only the configured
        /// account context. Request body must contain only <c>account_name</c>.
        /// </summary>
        [TestMethod]
        public void CloseBatch_WithoutBatchId_MultiMerchant_DefaultConfig_AccountOnly() {
            var multiMerchantConfig = new GpApiConfig {
                AppId = MultiMerchantAppId,
                AppKey = MultiMerchantAppKey,
                Channel = Channel.CardPresent,
                RequestLogger = new RequestConsoleLogger(),
                EnableLogging = true,
                MerchantId = MultiMerchantTargetMerchantId,
                Country = "US",
                AccessTokenInfo = new AccessTokenInfo {
                    TransactionProcessingAccountName = StandaloneMerchantInlineAccountName
                }
            };

            ServicesContainer.ConfigureService(multiMerchantConfig);

            const decimal chargeAmount = 4.35m;

            var transaction = card.Charge(chargeAmount).
                WithCurrency("USD").
                Execute();

            AssertTransactionResponse(transaction, TransactionStatus.Captured);

            WaitForGpApiReplication();

            var batchSummary = BatchService.CloseBatch();
            AssertBatchCloseResponse(batchSummary, chargeAmount);
        }

        /// <summary>
        /// Multi-merchant default config: close batch without a batch ID, scoped by currency and
        /// payment methods. Request body must contain
        /// <c>account_name + channel + currency + country + payment_methods</c>.
        /// </summary>
        [TestMethod]
        public void CloseBatch_WithoutBatchId_MultiMerchant_DefaultConfig_WithCurrencyAndPaymentMethods() {
            var transaction = card.Charge(Amount)
                .WithCurrency(Currency)
                .Execute();
            AssertTransactionResponse(transaction, TransactionStatus.Captured);

            WaitForGpApiReplication();

            var batchSummary = BatchService.CloseBatch(
                Currency, new PaymentMethodName[] { PaymentMethodName.Card });
            AssertBatchCloseResponse(batchSummary, Amount);
        }

        /// <summary>
        /// Multi-merchant named config: close batch without a batch ID, scoped by currency and payment
        /// methods, routed through a named GP API configuration.
        /// </summary>
        [TestMethod]
        public void CloseBatch_WithoutBatchId_MultiMerchant_NamedConfig_WithCurrencyAndPaymentMethods() {
            var transaction = card.Charge(Amount)
                .WithCurrency(Currency)
                .Execute(MultiMerchantNamedConfig);
            AssertTransactionResponse(transaction, TransactionStatus.Captured);

            WaitForGpApiReplication();

            var batchSummary = BatchService.CloseBatch(
                Currency,
                new PaymentMethodName[] { PaymentMethodName.Card },
                MultiMerchantNamedConfig);
            AssertBatchCloseResponse(batchSummary, Amount);
        }

        #endregion

        #region Positive Tests - Standalone Merchant (no MerchantId on config)

        /// <summary>
        /// Standalone merchant default config (no MerchantId): close batch without a batch ID using
        /// only the configured account context. Request body must contain only <c>account_name</c>.
        /// </summary>
        [TestMethod]
        public void CloseBatch_WithoutBatchId_StandaloneMerchant_DefaultConfig_AccountOnly() {
            var standaloneInlineConfig = new GpApiConfig {
                AppId = StandaloneMerchantInlineAppId,
                AppKey = StandaloneMerchantInlineAppKey,
                Channel = Channel.CardPresent,
                RequestLogger = new RequestConsoleLogger(),
                EnableLogging = true,
                Country = "US",
                AccessTokenInfo = new AccessTokenInfo {
                    TransactionProcessingAccountName = StandaloneMerchantInlineAccountName
                }
            };

            ServicesContainer.ConfigureService(standaloneInlineConfig);
            var transaction = card.Charge(Amount)
                .WithCurrency("USD")
                .Execute();
            AssertTransactionResponse(transaction, TransactionStatus.Captured);

            WaitForGpApiReplication();

            var batchSummary = BatchService.CloseBatch();
            AssertBatchCloseResponse(batchSummary, Amount);
        }

        /// <summary>
        /// Standalone merchant named config (no MerchantId): close batch without a batch ID, scoped by
        /// currency and payment methods, routed through a named GP API configuration.
        /// </summary>
        [TestMethod]
        public void CloseBatch_WithoutBatchId_StandaloneMerchant_NamedConfig_WithCurrencyAndPaymentMethods() {
            var transaction = card.Charge(Amount)
                .WithCurrency(Currency)
                .Execute(StandaloneMerchantNamedConfig);
            AssertTransactionResponse(transaction, TransactionStatus.Captured);

            WaitForGpApiReplication();

            var batchSummary = BatchService.CloseBatch(
                Currency,
                new PaymentMethodName[] { PaymentMethodName.Card },
                StandaloneMerchantNamedConfig);
            AssertBatchCloseResponse(batchSummary, Amount);
        }

        #endregion

        #region Negative Tests

        /// <summary>
        /// Multi-merchant default config: gateway returns CONFIGURATION_DOES_NOT_EXIST (40041) when
        /// the close-batch currency does not match the original transaction currency.
        /// </summary>
        [TestMethod]
        public void CloseBatch_WithoutBatchId_MultiMerchant_DefaultConfig_CurrencyMismatch_ThrowsGatewayException() {
            var transaction = card.Charge(Amount)
                .WithCurrency(Currency)
                .Execute();
            AssertTransactionResponse(transaction, TransactionStatus.Captured);

            WaitForGpApiReplication();

            var ex = Assert.ThrowsException<GatewayException>(() => {
                BatchService.CloseBatch(
                    "USD", new PaymentMethodName[] { PaymentMethodName.Card });
            });

            Assert.IsNotNull(ex.ResponseMessage);
        }

        /// <summary>
        /// Multi-merchant named config: gateway returns CONFIGURATION_DOES_NOT_EXIST (40041) when
        /// the close-batch currency does not match the original transaction currency.
        /// </summary>
        [TestMethod]
        public void CloseBatch_WithoutBatchId_MultiMerchant_NamedConfig_CurrencyMismatch_ThrowsGatewayException() {
            var transaction = card.Charge(Amount)
                .WithCurrency(Currency)
                .Execute(MultiMerchantNamedConfig);
            AssertTransactionResponse(transaction, TransactionStatus.Captured);

            WaitForGpApiReplication();

            var ex = Assert.ThrowsException<GatewayException>(() => {
                BatchService.CloseBatch(
                    "USD",
                    new PaymentMethodName[] { PaymentMethodName.Card },
                    MultiMerchantNamedConfig);
            });

            Assert.IsNotNull(ex.ResponseMessage);
        }

        /// <summary>
        /// Gateway raises GatewayException when the supplied currency is not configured for any account.
        /// </summary>
        [TestMethod]
        public void CloseBatch_WithoutBatchId_InvalidCurrency_ThrowsGatewayException()
        {
            Assert.ThrowsException<GatewayException>(() =>
                BatchService.CloseBatch("SGD",
                    new PaymentMethodName[] { PaymentMethodName.Card },
                    MultiMerchantNamedConfig));
        }

        #endregion

        /// <summary>
        /// Asserts the batch close response has the expected status and amounts.
        /// </summary>
        private static void AssertBatchCloseResponse(BatchSummary batchSummary, decimal amount) {
            Assert.IsNotNull(batchSummary);
            Assert.AreEqual(Closed, batchSummary.Status);
            Assert.IsTrue(batchSummary.TransactionCount >= 1);
            Assert.IsTrue(batchSummary.TotalAmount >= amount);
        }

        /// <summary>
        /// Asserts the transaction response is successful with the expected status.
        /// </summary>
        private void AssertTransactionResponse(Transaction transaction, TransactionStatus transactionStatus) {
            Assert.IsNotNull(transaction);
            Assert.AreEqual("00", transaction.ResponseCode);
            Assert.AreEqual(GetMapping(transactionStatus), transaction.ResponseMessage);
        }
    }
}
