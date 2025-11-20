using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.GpApi;
using GlobalPayments.Api.PaymentMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.GpApi {
    /// <summary>
    /// Test class for verifying CreditSale transactions using different Portico, SecretApiKey, and GpApi credentials/configurations.
    /// </summary>
    [TestClass]
    public class GpApiCreateTokenWithPorticoCredentialTests : BaseGpApiTests {
        // Sample MasterCard and Visa card data for test transactions
        private CreditCardData masterCard;
        private CreditCardData visaCard;

        /// <summary>
        /// Test setup: removes existing configs and registers multiple configurations for different credential scenarios.
        /// Also initializes test card data.
        /// </summary>
        [TestInitialize]
        public void TestInitialize() {

            // Remove any previously registered service config
            ServicesContainer.RemoveConfig();

            // Register service config using "Legacy" Portico credentials
            var legacyPorticoConfig = GpApiConfigSetup("", "", Channel.CardNotPresent);
            legacyPorticoConfig.ServiceUrl = "https://apis-qa.globalpay.com/ucp";
            legacyPorticoConfig.Country = "US";
            legacyPorticoConfig.AccessTokenInfo = new AccessTokenInfo {
                TransactionProcessingAccountName = "accessTokenValidationsecretKey"
            };
            legacyPorticoConfig.PorticoTokenConfig = new PorticoTokenConfig {
                DeviceId = 11753,
                SiteId = 418948,
                LicenseId = 388244,
                Username = "gateway1213846",
                Password = "$Test1234",
            };
            ServicesContainer.ConfigureService(legacyPorticoConfig, "LegacyPorticoConfig");

            // Register service config using SecretApiKey only
            var secretApiKeyConfig = GpApiConfigSetup("", "", Channel.CardNotPresent);
            secretApiKeyConfig.ServiceUrl = "https://apis-qa.globalpay.com/ucp";
            secretApiKeyConfig.Country = "US";
            secretApiKeyConfig.AccessTokenInfo = new AccessTokenInfo {
                TransactionProcessingAccountName = "accessTokenValidationsecretKey"
            };
            secretApiKeyConfig.PorticoTokenConfig = new PorticoTokenConfig {
                SecretApiKey = "skapi_cert_MVISAgC05V8Amnxg2jARLKW-K4ONQeXejrWYCCA_Cw"
            };
            ServicesContainer.ConfigureService(secretApiKeyConfig, "SecretApiKeyConfig");

            // Register service config with both Portico credentials and SecretApiKey
            var fullPorticoConfig = GpApiConfigSetup("", "", Channel.CardNotPresent);
            fullPorticoConfig.ServiceUrl = "https://apis-qa.globalpay.com/ucp";
            fullPorticoConfig.Country = "US";
            fullPorticoConfig.AccessTokenInfo = new AccessTokenInfo {
                TransactionProcessingAccountName = "accessTokenValidationsecretKey"
            };

            fullPorticoConfig.PorticoTokenConfig = new PorticoTokenConfig {
                DeviceId = 11753,
                SiteId = 418948,
                LicenseId = 388244,
                Username = "gateway1213846",
                Password = "$Test1234",
                SecretApiKey = "skapi_cert_MVISAgC05V8Amnxg2jARLKW-K4ONQeXejrWYCCA_Cw"
            };
            ServicesContainer.ConfigureService(fullPorticoConfig, "FullPorticoConfig");

            // Register legacy config with AppId included
            var legacyPorticoAppIdConfig = GpApiConfigSetup("", "", Channel.CardNotPresent);
            legacyPorticoAppIdConfig.AppId = "jYtVGox8yvG6KQwlNHPxbfyDa13kwOGt";
            legacyPorticoAppIdConfig.ServiceUrl = "https://apis-qa.globalpay.com/ucp";
            legacyPorticoAppIdConfig.Country = "US";
            legacyPorticoAppIdConfig.AccessTokenInfo = new AccessTokenInfo {
                TransactionProcessingAccountName = "accessTokenValidationsecretKey"
            };
            legacyPorticoAppIdConfig.PorticoTokenConfig = new PorticoTokenConfig {
                DeviceId = 11753,
                SiteId = 418948,
                LicenseId = 388244,
                Username = "gateway1213846",
                Password = "$Test1234",
            };
            ServicesContainer.ConfigureService(legacyPorticoAppIdConfig, "LegacyPorticoAppIdConfig");

            // Register config with SecretApiKey and AppId
            var secretApiKeyAppIdConfig = GpApiConfigSetup("", "", Channel.CardNotPresent);
            secretApiKeyAppIdConfig.AppId = "jYtVGox8yvG6KQwlNHPxbfyDa13kwOGt";
            secretApiKeyAppIdConfig.ServiceUrl = "https://apis-qa.globalpay.com/ucp";
            secretApiKeyAppIdConfig.Country = "US";
            secretApiKeyAppIdConfig.AccessTokenInfo = new AccessTokenInfo {
                TransactionProcessingAccountName = "accessTokenValidationsecretKey"
            };
            secretApiKeyAppIdConfig.PorticoTokenConfig = new PorticoTokenConfig {
                SecretApiKey = "skapi_cert_MVISAgC05V8Amnxg2jARLKW-K4ONQeXejrWYCCA_Cw"
            };
            ServicesContainer.ConfigureService(secretApiKeyAppIdConfig, "SecretApiKeyAppIdConfig");

            // Register config with Portico credentials, SecretApiKey, and AppId
            var fullPorticoAppIdConfig = GpApiConfigSetup("", "", Channel.CardNotPresent);
            fullPorticoAppIdConfig.AppId = "jYtVGox8yvG6KQwlNHPxbfyDa13kwOGt";
            fullPorticoAppIdConfig.ServiceUrl = "https://apis-qa.globalpay.com/ucp";
            fullPorticoAppIdConfig.Country = "US";
            fullPorticoAppIdConfig.AccessTokenInfo = new AccessTokenInfo {
                TransactionProcessingAccountName = "accessTokenValidationsecretKey"
            };
            fullPorticoAppIdConfig.PorticoTokenConfig = new PorticoTokenConfig {
                DeviceId = 11753,
                SiteId = 418948,
                LicenseId = 388244,
                Username = "gateway1213846",
                Password = "$Test1234",
                SecretApiKey = "skapi_cert_MVISAgC05V8Amnxg2jARLKW-K4ONQeXejrWYCCA_Cw"
            };
            ServicesContainer.ConfigureService(fullPorticoAppIdConfig, "FullPorticoAppIdConfig");

            // Default GpApi config (presumably using AppId and AppKey from base class/test context)
            var gpApiConfig = GpApiConfigSetup(AppId, AppKey, Channel.CardNotPresent);
            ServicesContainer.ConfigureService(gpApiConfig);

            // Register a config with intentionally invalid credentials for negative test scenarios.
            var gpAPiConfigFailingScenarios = GpApiConfigSetup("", "", Channel.CardNotPresent);
            gpAPiConfigFailingScenarios.AppId = "jYtVGox8yvG6KQwlNHPxbfyDa13kwOGt";
            gpAPiConfigFailingScenarios.ServiceUrl = "https://apis-qa.globalpay.com/ucp";
            gpAPiConfigFailingScenarios.Country = "US";
            gpAPiConfigFailingScenarios.AccessTokenInfo = new AccessTokenInfo {
                TransactionProcessingAccountName = "accessTokenValidationsecretKey"
            };
            gpAPiConfigFailingScenarios.PorticoTokenConfig = new PorticoTokenConfig {
                 DeviceId = 11753,
                 SiteId = 418948,
                 LicenseId = 388244,
                 Username = "", // Intentionally left blank for failure scenario
                 Password = "", // Intentionally left blank for failure scenario
                 SecretApiKey = "" // Intentionally left blank for failure scenario
            };
            ServicesContainer.ConfigureService(gpAPiConfigFailingScenarios, "gpAPiConfigFailingScenarios");

            // Initialize sample MasterCard and Visa card data for test usage
            masterCard = new CreditCardData {
                Number = "5546259023665054",
                ExpMonth = 05,
                ExpYear = 2025,
                Cvn = "123",
                CardPresent = false,
                ReaderPresent = false
            };

            visaCard = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = ExpMonth,
                ExpYear = ExpYear,
                Cvn = "123",
                CardPresent = true
            };
        }

        // Should successfully perform a credit sale using legacy Portico credentials.
        [TestMethod]
        public void CreditSale_ShouldReturnsCapturedTransaction_WhenUsingLegacy5Config() {

            var response = masterCard.Charge(12)
                .WithCurrency("USD")
                .Execute("LegacyPorticoConfig");
            Assert.IsNotNull(response);
            Assert.AreEqual(Success, response.ResponseCode);
            AssertTransactionResponse(response, TransactionStatus.Captured);
        }

        // Should successfully perform a credit sale using SecretApiKey only credentials.
        [TestMethod]
        public void CreditSale_ShouldReturnsCapturedTransaction_WhenUsingSecretApiKeyConfig() {

            var response = masterCard.Charge(12)
                .WithCurrency("USD")
                .Execute("SecretApiKeyConfig");
            Assert.IsNotNull(response);
            Assert.AreEqual(Success, response.ResponseCode);
            AssertTransactionResponse(response, TransactionStatus.Captured);
        }

        // Should successfully perform a credit sale using all Portico credentials and SecretApiKey.
        [TestMethod]
        public void CreditSale_ShouldReturnCapturedTransaction_WithAllPorticoConfig() {

            var response = masterCard.Charge(12)
                .WithCurrency("USD")
                .Execute("FullPorticoConfig");
            Assert.IsNotNull(response);
            Assert.AreEqual(Success, response.ResponseCode);
            AssertTransactionResponse(response, TransactionStatus.Captured);
        }

        // Should successfully perform a credit sale using legacy Portico credentials and AppId.
        [TestMethod]
        public void CreditSale_ShouldReturnCapturedTransaction_WhenUsingLegacy5ConfigWithAppId() {

            var response = masterCard.Charge(12)
                .WithCurrency("USD")
                .Execute("LegacyPorticoAppIdConfig");
            Assert.IsNotNull(response);
            Assert.AreEqual(Success, response.ResponseCode);
            AssertTransactionResponse(response, TransactionStatus.Captured);
        }

        // Should successfully perform a credit sale using SecretApiKey and AppId.
        [TestMethod]
        public void CreditSale_ShouldReturnCapturedTransaction_WhenUsingSecretApiKeyConfigWithAppId() {

            var response = masterCard.Charge(12)
                .WithCurrency("USD")
                .Execute("SecretApiKeyAppIdConfig");
            Assert.IsNotNull(response);
            Assert.AreEqual(Success, response.ResponseCode);
            AssertTransactionResponse(response, TransactionStatus.Captured);
        }

        // Should successfully perform a credit sale using all Portico credentials, SecretApiKey and AppId.
        [TestMethod]
        public void CreditSale_ShouldReturnCapturedTransaction_WhenUsingAllPorticoConfigWithAppId() {

            var response = masterCard.Charge(12)
                .WithCurrency("USD")
                .Execute("FullPorticoAppIdConfig");
            Assert.IsNotNull(response);
            Assert.AreEqual(Success, response.ResponseCode);
            AssertTransactionResponse(response, TransactionStatus.Captured);
        }

        // Should successfully perform a credit sale using default GpApi credentials.
        [TestMethod]
        public void CreditSale_ShouldReturnCapturedTransaction_WhenUsingGpApiCredentials() {

            var response = visaCard.Charge(12)
               .WithCurrency("USD")
               .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual(Success, response.ResponseCode);
            AssertTransactionResponse(response, TransactionStatus.Captured);
        }

        // Verifies that a GatewayException is thrown when attempting a credit sale
        // with invalid credentials, and asserts the exception contains the expected
        // response code, message, and error details.
        [TestMethod]
        public void CreditSale_ShouldThrowGatewayException_WhenCredentialsAreInvalid() {

            var ex =  Assert.ThrowsException<GatewayException>(() => {
                var response = masterCard.Charge(12)
               .WithCurrency("USD")
               .Execute("gpAPiConfigFailingScenarios");
            });

            Assert.AreEqual("ACTION_NOT_AUTHORIZED", ex.ResponseCode);
            Assert.AreEqual("40004", ex.ResponseMessage);
            Assert.AreEqual("Status Code: Forbidden - Credentials not recognized to create access token.", ex.Message);
        }

        /// <summary>
        /// Asserts that the transaction response is as expected for successful captured sales.
        /// </summary>
        /// <param name="transaction">Transaction object returned from Charge operation</param>
        /// <param name="transactionStatus">Expected transaction status</param>
        private void AssertTransactionResponse(Transaction transaction, TransactionStatus transactionStatus) {
            Assert.IsNotNull(transaction);
            Assert.AreEqual(Success, transaction.ResponseCode);
            Assert.AreEqual(GetMapping(transactionStatus), transaction.ResponseMessage);
        }
    }
}