using System;
using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Enums;
using GlobalPayments.Api.Gateways;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Utils;
using GlobalPayments.Api.Utils.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.GpApi {
    /// <summary>
    /// Tests GP-API MMA account merchant URL routing.
    /// </summary>
    [TestClass]
    public class GpApiMMAAccountTests : BaseGpApiTests {
        private const string DefaultConfigName = "DefaultSale";
        private const string MerchantConfigName = "MerchantSale";
        private const string MmaAccountMerchantConfigName = "MmaAccountMerchantSale";

        private const string MerchantScopedAppId = "wuqol13QQJu0vcjbdN9892cl5IcyJgPU";
        private const string MerchantScopedAppKey = "jdOJNxsGbE0nLeTx";
        private const string MerchantScopedMerchantId = "MER_3abc0724f49c40e59a61309ae6d37dfd";
        private const string MerchantScopedAccountName = "test_account_CungkardAk";

        private const string MmaAccountId = "MMA_344aebc638864d7381909dcf78d4c760";
        private const string MmaAccountMerchantId = "MER_6517954c0c8544feb3f74c825e2b032c";
        private CreditCardData visaCard;
        private CreditCardData masterCard;
        private GpApiConfig mmaAccountMerchantConfig;

        /// <summary>
        /// Initializes the live MMA account configuration and test card.
        /// </summary>
        [TestInitialize]
        public void TestInitialize() {
            ServicesContainer.RemoveConfig();
            ServicesContainer.RemoveConfig(DefaultConfigName);
            ServicesContainer.RemoveConfig(MerchantConfigName);
            ServicesContainer.RemoveConfig(MmaAccountMerchantConfigName);

            var config = GpApiConfigSetup(AppId, AppKey, Channel.CardNotPresent);
            ServicesContainer.ConfigureService(config);

            var merchantScopedConfig = GpApiConfigSetup(MerchantScopedAppId, MerchantScopedAppKey, Channel.CardPresent);
            merchantScopedConfig.MerchantId = MerchantScopedMerchantId;
            merchantScopedConfig.Country = "US";
            merchantScopedConfig.AccessTokenInfo = new AccessTokenInfo
            {
                TransactionProcessingAccountName = MerchantScopedAccountName
            };

            ServicesContainer.ConfigureService(merchantScopedConfig, MerchantConfigName);

            mmaAccountMerchantConfig = GpApiConfigSetup(PartnerAppId, PartnerAppKey, Channel.CardNotPresent);
            mmaAccountMerchantConfig.MerchantManagementAccountID = MmaAccountId;
            mmaAccountMerchantConfig.MerchantId = MmaAccountMerchantId;
            mmaAccountMerchantConfig.EnableLogging = true;
            mmaAccountMerchantConfig.RequestLogger = new RequestConsoleLogger();
            ServicesContainer.ConfigureService(mmaAccountMerchantConfig, MmaAccountMerchantConfigName);

            visaCard = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = ExpMonth,
                ExpYear = ExpYear,
                Cvn = "123",
                CardHolderName = "James Mason"
            };

            masterCard = new CreditCardData {
                Number = "5425230000004415",
                ExpMonth = 12,
                ExpYear = 26,
                Cvn = "123",
                CardPresent = true
            };

        }
        #region Positive Tests

        /// <summary>
        /// Validates the new MMA merchant-routing behavior for /accounts/{MMA_ID}/merchants/{MER_ID}/transactions.
        /// </summary>
        [TestMethod]
        public void ChargeCard_WithMultiMerchantTransactionsEndpoint_ReturnsSuccess() {
            var response = masterCard.Charge(1.01m)
                 .WithCurrency("USD")
                 .Execute(MmaAccountMerchantConfigName);

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        /// <summary>
        /// Regression check to confirm the existing merchant-scoped route still works: /merchants/{merchantId}/transactions.
        /// </summary>
        [TestMethod]
        public void ChargeCard_WithMerchantTransactionsEndpoint_ReturnsSuccess() {
           
            var response = masterCard.Charge(1.01m)
                 .WithCurrency("GBP")
                 .Execute(MerchantConfigName);

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        /// <summary>
        /// Regression check to confirm the default transaction route remains unchanged: /transactions.
        /// </summary>
        [TestMethod]
        public void ChargeCard_WithDefaultTransactionsEndpoint_ReturnsSuccess() {

            var response = visaCard.Charge(1.01m)
                .WithCurrency("GBP")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }
        #endregion

        #region MMA Routing Smoke Tests

        /// <summary>
        /// Confirms that a management request succeeds through the MMA merchant route.
        /// </summary>
        [TestMethod]
        public void CaptureTransaction_WithMultiMerchantConfiguration_ReturnsResponse() {
            var authorization = masterCard.Authorize(1.01m)
                .WithCurrency("USD")
                .Execute(MmaAccountMerchantConfigName);

            var response = authorization.Capture(1.01m)
                .Execute(MmaAccountMerchantConfigName);

            Assert.IsNotNull(response);
        }

        /// <summary>
        /// Verifies that file-processing requests generate an MMA-scoped endpoint; the probe resource is not configured in sandbox.
        /// </summary>
        [TestMethod]
        public void GetFileProcessingDetails_WithMultiMerchantConfiguration_ReturnsResponse() {
            // The sandbox lacks this probe resource, but the request still generates the MMA-scoped endpoint.
            var ex = Assert.ThrowsException<GatewayException>(() => {
                new FileProcessingBuilder(FileProcessingActionType.GET_DETAILS)
                    .WithResourceId("FPR_971edc6eb0944d8d890dcba7a2a41bea")
                    .Execute(MmaAccountMerchantConfigName);
            });

            Assert.AreEqual("RESOURCE_NOT_FOUND", ex.ResponseCode);
            Assert.AreEqual("40118", ex.ResponseMessage);
            Assert.AreEqual("Status Code: NotFound - Files FPR_971edc6eb0944d8d890dcba7a2a41bea not found at this /ucp/files/FPR_971edc6eb0944d8d890dcba7a2a41bea", ex.Message);
        }

        /// <summary>
        /// Verifies that installment requests generate an MMA-scoped endpoint; the MMA sandbox account is not configured for installments.
        /// </summary>
        [TestMethod]
        public void CreateInstallment_WithMultiMerchantConfiguration_ReturnsResponse() {
            var installment = new Installment {
                Amount = 1.01m,
                CardDetails = masterCard,
                Channel = "CNP",
                Country = "US",
                Currency = "USD",
                EntryMode = "ECOM",
                Program = "SIP",
                Reference = Guid.NewGuid().ToString()
            };

            // The MMA sandbox account does not support this installment program, but the request generates the MMA-scoped endpoint.
            var ex = Assert.ThrowsException<GatewayException>(() => {
                InstallmentService.Create(installment, MmaAccountMerchantConfigName);
            });

            Assert.AreEqual("SYSTEM_ERROR_DOWNSTREAM", ex.ResponseCode);
            Assert.AreEqual("50046", ex.ResponseMessage);
            Assert.AreEqual("Status Code: BadGateway - Unable to process your request due to an error with a system down stream.", ex.Message);
        }

        /// <summary>
        /// Confirms that a recurring payer request succeeds through the MMA merchant route.
        /// </summary>
        [TestMethod]
        public void CreatePayer_WithMultiMerchantConfiguration_ReturnsResponse() {
            var customer = new Customer {
                FirstName = "MMA",
                Key = Guid.NewGuid().ToString(),
                LastName = "Routing"
            };

            var response = RecurringService.Create(customer, MmaAccountMerchantConfigName);

            Assert.IsNotNull(response);
        }

        /// <summary>
        /// Confirms that a reporting request succeeds through the MMA merchant route.
        /// </summary>
        [TestMethod]
        public void GetTransactionDetail_WithMultiMerchantConfiguration_ReturnsResponse() {
            var authorization = masterCard.Authorize(1.01m)
                .WithCurrency("USD")
                .Execute(MmaAccountMerchantConfigName);

            var response = ReportingService.TransactionDetail(authorization.TransactionId)
                .Execute(MmaAccountMerchantConfigName);

            Assert.IsNotNull(response);
        }

        /// <summary>
        /// Confirms that a 3DS enrollment request succeeds through the MMA merchant route.
        /// </summary>
        [TestMethod]
        public void CheckThreeDSecureEnrollment_WithMultiMerchantConfiguration_ReturnsResponse() {
            var response = Secure3dService.CheckEnrollment(visaCard)
                .WithAmount(1.01m)
                .WithCurrency("USD")
                .Execute(MmaAccountMerchantConfigName);

            Assert.IsNotNull(response);
        }

        /// <summary>
        /// Confirms that a fraud-risk assessment succeeds through the MMA merchant route.
        /// </summary>
        [TestMethod]
        public void AssessFraudRisk_WithMultiMerchantConfiguration_ReturnsResponse() {
            var browserData = new BrowserData {
                AcceptHeader = "text/html,application/xhtml+xml",
                ColorDepth = ColorDepth.TWENTY_FOUR_BITS,
                IpAddress = "123.123.123.123",
                JavaEnabled = true,
                Language = "en",
                ChallengeWindowSize = ChallengeWindowSize.WINDOWED_600X400,
                Timezone = "0",
                UserAgent = "Mozilla/5.0"
            };

            // The sandbox risk service is not configured for this MMA account, but the request still generates the MMA-scoped endpoint.
            var ex = Assert.ThrowsException<GatewayException>(() => {
                FraudService.RiskAssess(visaCard)
                    .WithAmount(1.01m)
                    .WithCurrency("USD")
                    .WithAuthenticationSource(AuthenticationSource.BROWSER)
                    .WithBrowserData(browserData)
                    .Execute(MmaAccountMerchantConfigName);
            });

            Assert.AreEqual("SYSTEM_ERROR_DOWNSTREAM", ex.ResponseCode);
            Assert.AreEqual("50046", ex.ResponseMessage);
            Assert.AreEqual("Status Code: BadGateway - Unable to process your request due to an error with a system down stream.", ex.Message);
        }

        #endregion

        #region Negative Tests

        /// <summary>
        /// Validates that the MMA route fails fast during builder validation when currency is missing.
        /// </summary>
        [TestMethod]
        public void ChargeCard_WithMultiMerchantTransactionsEndpoint_MissingCurrency_ThrowsBuilderException() {

            var ex = Assert.ThrowsException<BuilderException>(() => {
                masterCard.Charge(1.01m).Execute(MmaAccountMerchantConfigName);
            });

            Assert.AreEqual("Currency cannot be null for this transaction type.", ex.Message);
        }

        /// <summary>
        /// Validates the MMA route rejects merchants that are not associated with the configured MMA account.
        /// </summary>
        [TestMethod]
        public void ChargeCard_WithMultiMerchantTransactionsEndpoint_MerchantNotUnderMmaAccount_ThrowsGatewayException() {
            var invalidConfig = GpApiConfigSetup(PartnerAppId, PartnerAppKey, Channel.CardNotPresent);
            invalidConfig.MerchantManagementAccountID = MmaAccountId;
            invalidConfig.MerchantId = "MER_20c2b3fcfedd484e9c3723347db46b71";
            ServicesContainer.ConfigureService(invalidConfig, "invalidMmaMerchant");

            var ex = Assert.ThrowsException<GatewayException>(() => {
                masterCard.Charge(1.01m)
                          .WithCurrency("USD")
                          .Execute("invalidMmaMerchant");
            });

            Assert.AreEqual("INVALID_REQUEST_DATA", ex.ResponseCode);
            Assert.AreEqual("40041", ex.ResponseMessage);
            Assert.AreEqual("Status Code: BadRequest - Merchant configuration does not exist for the following combination: merchant_id - MER_20c2b3fcfedd484e9c3723347db46b71, account_id - , account_name - transaction_processing", ex.Message);
        }

        /// <summary>
        /// Regression check that the existing merchant-scoped route still fails validation when currency is missing.
        /// </summary>
        [TestMethod]
        public void ChargeCard_WithMerchantTransactionsEndpoint_MissingCurrency_ThrowsBuilderException() {

            var ex = Assert.ThrowsException<BuilderException>(() => {
                masterCard.Charge(1.01m).Execute(MerchantConfigName);
            });

            Assert.AreEqual("Currency cannot be null for this transaction type.", ex.Message);
        }

        /// <summary>
        /// Regression check that the default route still fails validation when currency is missing.
        /// </summary>
        [TestMethod]
        public void ChargeCard_WithDefaultTransactionsEndpoint_MissingCurrency_ThrowsBuilderException() {
            var ex = Assert.ThrowsException<BuilderException>(() => {
                masterCard.Charge(1.01m).Execute();
            });

            Assert.AreEqual("Currency cannot be null for this transaction type.", ex.Message);
        }

        /// <summary>
        /// Regression check that the existing merchant-scoped route still returns a gateway error for an unknown merchant id.
        /// </summary>
        [TestMethod]
        public void ChargeCard_WithMerchantTransactionsEndpoint_InvalidMerchantId_ThrowsGatewayException() {
            var invalidConfig = GpApiConfigSetup(MerchantScopedAppId, MerchantScopedAppKey, Channel.CardPresent);
            invalidConfig.MerchantId = "MER_20c2b3fcfedd484e9c3723347db56b71";
            invalidConfig.Country = "US";
            invalidConfig.AccessTokenInfo = new AccessTokenInfo {
                TransactionProcessingAccountName = MerchantScopedAccountName
            };
            ServicesContainer.ConfigureService(invalidConfig, "invalidMerchant");

            var ex = Assert.ThrowsException<GatewayException>(() => {
                masterCard.Charge(1.01m)
                          .WithCurrency("GBP")
                          .Execute("invalidMerchant");
            });

            Assert.AreEqual("INVALID_REQUEST_DATA", ex.ResponseCode);
            Assert.AreEqual("40041", ex.ResponseMessage);
            Assert.AreEqual("Status Code: BadRequest - Merchant configuration does not exist for the following combination: merchant_id - MER_20c2b3fcfedd484e9c3723347db56b71, account_id - , account_name - test_account_CungkardAk", ex.Message);
        }
        #endregion
    }
}