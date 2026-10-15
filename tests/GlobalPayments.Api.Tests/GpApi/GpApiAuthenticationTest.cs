using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Logging;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Utils;
using GlobalPayments.Api.Utils.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading;

namespace GlobalPayments.Api.Tests.GpApi {

    [TestClass]
    public class GpApiAuthenticationTest : BaseGpApiTests {
        private const string TokenizationAccountId = "TKA_b3a46f0f351f43cfad20acf5c32fea50";
        private CreditCardData card;
        private GpApiConfig gpApiConfig;

        [TestInitialize]
        public void TestInitialize() {
            card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = ExpMonth,
                ExpYear = ExpYear,
                Cvn = "123",
            };

            gpApiConfig = new GpApiConfig {
                AppId = AppId,
                AppKey = AppKey,
                RequestLogger = new RequestConsoleLogger()
            };
        }

        [TestMethod]
        public void GenerateAccessTokenManual() {
            var info = GpApiService.GenerateTransactionKey(gpApiConfig);
            AssertAccessTokenResponse(info);
        }

        [TestMethod]
        public void GenerateAccessToken_WithRestrictedTokenYes_MasksTokenizationAccountId() {
            var config = new GpApiConfig {
                AppId = AppId,
                AppKey = AppKey,
                RestrictedToken = true,
                Permissions = new[] { "PMT_POST_Create_Single" },
                RequestLogger = new RequestConsoleLogger()
            };

            var accessTokenInfo = GpApiService.GenerateTransactionKey(config);
            Assert.IsNotNull(accessTokenInfo);
            Assert.IsFalse(string.IsNullOrEmpty(accessTokenInfo.Token));

            config.AccessTokenInfo = accessTokenInfo;
            ServicesContainer.RemoveConfig();
            ServicesContainer.ConfigureService(config);

            var token = card.Tokenize(false, paymentMethodUsageMode: PaymentMethodUsageMode.Single);
            Assert.IsFalse(string.IsNullOrEmpty(token));

            Assert.AreEqual(TokenizationAccountId, accessTokenInfo.TokenizationAccountID);
            Assert.IsFalse(string.IsNullOrEmpty(accessTokenInfo.TokenizationAccountID));
        }

        [TestMethod]
        public void GenerateAccessToken_WithRestrictedTokenNo_ExposesTokenizationAccountId() {
            var config = new GpApiConfig {
                AppId = AppId,
                AppKey = AppKey,
                RestrictedToken = false,
                Permissions = new[] { "PMT_POST_Create_Single" },
                RequestLogger = new RequestConsoleLogger()
            };

            var accessTokenInfo = GpApiService.GenerateTransactionKey(config);
            Assert.IsNotNull(accessTokenInfo);
            Assert.IsFalse(string.IsNullOrEmpty(accessTokenInfo.Token));

            config.AccessTokenInfo = accessTokenInfo;
            ServicesContainer.RemoveConfig();
            ServicesContainer.ConfigureService(config);

            var token = card.Tokenize(false, paymentMethodUsageMode: PaymentMethodUsageMode.Single);

            Assert.IsFalse(string.IsNullOrEmpty(token));
            Assert.AreEqual(TokenizationAccountId, accessTokenInfo.TokenizationAccountID);
            Assert.IsFalse(string.IsNullOrEmpty(accessTokenInfo.TokenizationAccountID));
        }

        [TestMethod]
        public void GenerateAccessToken_WithoutRestrictedToken_ExposesTokenizationAccountId()
        {
            var config = new GpApiConfig
            {
                AppId = AppId,
                AppKey = AppKey,
                Permissions = new[] { "PMT_POST_Create_Single" },
                RequestLogger = new RequestConsoleLogger()
            };

            var accessTokenInfo = GpApiService.GenerateTransactionKey(config);
            Assert.IsNotNull(accessTokenInfo);
            Assert.IsFalse(string.IsNullOrEmpty(accessTokenInfo.Token));

            config.AccessTokenInfo = accessTokenInfo;
            ServicesContainer.RemoveConfig();
            ServicesContainer.ConfigureService(config);

            var token = card.Tokenize(false, paymentMethodUsageMode: PaymentMethodUsageMode.Single);
            Assert.IsFalse(string.IsNullOrEmpty(token));

            Assert.AreEqual(TokenizationAccountId, accessTokenInfo.TokenizationAccountID);
            Assert.IsFalse(string.IsNullOrEmpty(accessTokenInfo.TokenizationAccountID));
        }

        [TestMethod]
        public void GenerateAccessTokenManualWithPermissions() {
            var permissions = new[] { "PMT_POST_Create", "PMT_POST_Detokenize" };
            gpApiConfig.Permissions = permissions;

            var info = GpApiService.GenerateTransactionKey(gpApiConfig);

            Assert.IsNotNull(info);
            Assert.IsNotNull(info.Token);
            Assert.AreEqual("tokenization", info.TokenizationAccountName);
            Assert.IsNull(info.DataAccountName);
        }

        [TestMethod]
        public void GenerateAccessTokenManualWithWrongPermissions() {
            var permissions = new[] { "TEST_1", "TEST_2" };
            gpApiConfig.Permissions = permissions;

            var exceptionCaught = false;
            try {
                GpApiService.GenerateTransactionKey(gpApiConfig);
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("INVALID_REQUEST_DATA", ex.ResponseCode);
                Assert.AreEqual("40119", ex.ResponseMessage);
                Assert.IsTrue(ex.Message.StartsWith("Status Code: BadRequest - Invalid permissions"));
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void ChargeCard_WithLimitedPermissions() {
            var permissions = new[] { "TRN_POST_Capture" };
            gpApiConfig.Permissions = permissions;
            gpApiConfig.SecondsToExpire = 60;
            GpApiService.GenerateTransactionKey(gpApiConfig);

            ServicesContainer.ConfigureService(gpApiConfig, "GpApiConfig_2");

            var exceptionCaught = false;
            try {
                card.Charge()
                    .WithAmount(1.00m)
                    .WithCurrency("USD")
                    .Execute("GpApiConfig_2");
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("ACTION_NOT_AUTHORIZED", ex.ResponseCode);
                Assert.AreEqual("40212", ex.ResponseMessage);
                Assert.AreEqual("Status Code: Forbidden - Permission not enabled to execute action", ex.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void GenerateAccessTokenManual_AccessToken() {
            var info = GpApiService.GenerateTransactionKey(gpApiConfig);
            gpApiConfig.AccessTokenInfo = info;
            ServicesContainer.ConfigureService(gpApiConfig);

            var response = card.Verify()
                .WithCurrency("USD")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response?.ResponseCode);
            Assert.AreEqual(Verified, response?.ResponseMessage);
        }

        [TestMethod]
        public void GenerateAccessTokenManualWithSecondsToExpire() {
            gpApiConfig.SecondsToExpire = 60;
            var info = GpApiService.GenerateTransactionKey(gpApiConfig);
            gpApiConfig.AccessTokenInfo = info;
            ServicesContainer.ConfigureService(gpApiConfig);

            var response = card.Verify()
                .WithCurrency("USD")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response?.ResponseCode);
            Assert.AreEqual(Verified, response?.ResponseMessage);
        }

        [TestMethod]
        public void ShouldReSignInAfterTokenExpiration() {
            gpApiConfig.SecondsToExpire = 60;
            AccessTokenInfo accessTokenInfo = GpApiService.GenerateTransactionKey(gpApiConfig);
            AssertAccessTokenResponse(accessTokenInfo);

            ServicesContainer.ConfigureService(gpApiConfig, "resign");

            Transaction response = card.Verify()
                .WithCurrency("USD")
                .Execute("resign");

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.AreEqual("VERIFIED", response.ResponseMessage);

            Thread.Sleep(6001);

            response = card.Verify()
                .WithCurrency("USD")
                .Execute("resign");

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.AreEqual("VERIFIED", response.ResponseMessage);
        }

        [TestMethod]
        public void GenerateAccessTokenManualWithMaximumSecondsToExpire() {
            var exceptionCaught = false;
            try {
                gpApiConfig.SecondsToExpire = 604801;
                GpApiService.GenerateTransactionKey(gpApiConfig);
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("INVALID_REQUEST_DATA", ex.ResponseCode);
                Assert.AreEqual("40213", ex.ResponseMessage);
                Assert.AreEqual("Status Code: BadRequest - seconds_to_expire contains unexpected data", ex.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }
        
        [TestMethod]
        public void GenerateAccessTokenManualWithInvalidSecondsToExpire() {
            var exceptionCaught = false;
            try {
                gpApiConfig.SecondsToExpire = 10;
                GpApiService.GenerateTransactionKey(gpApiConfig);
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("INVALID_REQUEST_DATA", ex.ResponseCode);
                Assert.AreEqual("40213", ex.ResponseMessage);
                Assert.AreEqual("Status Code: BadRequest - seconds_to_expire contains unexpected data", ex.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void GenerateAccessTokenManualWithIntervalToExpire() {
            gpApiConfig.IntervalToExpire = IntervalToExpire.THREE_HOURS;
            var info = GpApiService.GenerateTransactionKey(gpApiConfig);

            gpApiConfig.AccessTokenInfo = info;

            ServicesContainer.ConfigureService(gpApiConfig);

            var response = card.Verify()
                .WithCurrency("USD")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response?.ResponseCode);
            Assert.AreEqual(Verified, response?.ResponseMessage);
        }

        [TestMethod]
        public void GenerateAccessTokenManualWithSecondsToExpireAndIntervalToExpire() {
            gpApiConfig.SecondsToExpire = 60;
            gpApiConfig.IntervalToExpire = IntervalToExpire.THREE_HOURS;
            GpApiService.GenerateTransactionKey(gpApiConfig);

            var response = card.Verify()
                .WithCurrency("USD")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response?.ResponseCode);
            Assert.AreEqual(Verified, response?.ResponseMessage);
        }

        [TestMethod]
        public void GenerateAccessTokenWithWrongAppId() {
            var exceptionCaught = false;
            try {
                gpApiConfig.AppId += "a";
                GpApiService.GenerateTransactionKey(gpApiConfig);
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("ACTION_NOT_AUTHORIZED", ex.ResponseCode);
                Assert.AreEqual("40004", ex.ResponseMessage);
                Assert.AreEqual("Status Code: Forbidden - App credentials not recognized", ex.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void GenerateAccessTokenWithWrongAppKey() {
            var exceptionCaught = false;
            try {
                gpApiConfig.AppKey += "a";
                GpApiService.GenerateTransactionKey(gpApiConfig);
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("ACTION_NOT_AUTHORIZED", ex.ResponseCode);
                Assert.AreEqual("40004", ex.ResponseMessage);
                Assert.AreEqual("Status Code: Forbidden - App credentials not recognized", ex.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void UseInvalidAccessTokenInfo() {
            ServicesContainer.ConfigureService(new GpApiConfig {
                AccessTokenInfo = new AccessTokenInfo {
                    Token = "INVALID_Token_w23e9sd93w3d",
                    DataAccountName = "dataAccount",
                    DisputeManagementAccountName = "disputeAccount",
                    TokenizationAccountName = "tokenizationAccount",
                    TransactionProcessingAccountName = "transactionAccount"
                }
            });

            var exceptionCaught = false;
            try {
                card.Verify()
                    .WithCurrency("USD")
                    .Execute();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("NOT_AUTHENTICATED", ex.ResponseCode);
                Assert.AreEqual("40001", ex.ResponseMessage);
                Assert.AreEqual("Status Code: Unauthorized - Invalid access token", ex.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void UseExpiredAccessTokenInfo() {
            ServicesContainer.ConfigureService(new GpApiConfig {
                AccessTokenInfo = new AccessTokenInfo {
                    Token = "r1SzGAx2K9z5FNiMHkrapfRh8BC8",
                    DataAccountName = "Settlement Reporting",
                    DisputeManagementAccountName = "Dispute Management",
                    TokenizationAccountName = "Tokenization",
                    TransactionProcessingAccountName = "Transaction_Processing"
                }
            });

            var exceptionCaught = false;
            try {
                card.Verify()
                    .WithCurrency("USD")
                    .Execute();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("NOT_AUTHENTICATED", ex.ResponseCode);
                Assert.AreEqual("40001", ex.ResponseMessage);
                Assert.AreEqual("Status Code: Unauthorized - Invalid access token", ex.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        private static void AssertAccessTokenResponse(AccessTokenInfo accessTokenInfo) {
            Assert.IsNotNull(accessTokenInfo);
            Assert.IsNotNull(accessTokenInfo.Token);
            Assert.AreEqual("Bearer", accessTokenInfo.Type);
            Assert.AreEqual(AppId, accessTokenInfo.AppId);
            Assert.IsNotNull(accessTokenInfo.AppName);
            Assert.AreNotEqual(default(System.DateTime), accessTokenInfo.TimeCreated);
            Assert.IsTrue(accessTokenInfo.SecondsToExpire > 0);
            Assert.IsNotNull(accessTokenInfo.Email);
            Assert.IsNotNull(accessTokenInfo.MerchantId);
            Assert.IsNotNull(accessTokenInfo.MerchantName);
            Assert.IsNotNull(accessTokenInfo.Accounts);
            Assert.IsTrue(accessTokenInfo.Accounts.Length > 0);
            Assert.IsTrue(accessTokenInfo.Accounts.All(account => !string.IsNullOrEmpty(account.Id)));
            Assert.IsTrue(accessTokenInfo.Accounts.All(account => !string.IsNullOrEmpty(account.Name)));
            Assert.IsTrue(accessTokenInfo.Accounts.All(account => account.Permissions != null && account.Permissions.Length > 0));
            AssertAccountFields(accessTokenInfo, "DAA_", accessTokenInfo.DataAccountID, accessTokenInfo.DataAccountName);
            AssertAccountFields(accessTokenInfo, "DIA_", accessTokenInfo.DisputeManagementAccountID, accessTokenInfo.DisputeManagementAccountName);
            AssertAccountFields(accessTokenInfo, "TKA_", accessTokenInfo.TokenizationAccountID, accessTokenInfo.TokenizationAccountName);
            AssertAccountFields(accessTokenInfo, "TRA_", accessTokenInfo.TransactionProcessingAccountID, accessTokenInfo.TransactionProcessingAccountName);
            AssertAccountFields(accessTokenInfo, "RAA_", accessTokenInfo.RiskAssessmentAccountID, accessTokenInfo.RiskAssessmentAccountName);
            AssertAccountFields(accessTokenInfo, "MMA_", accessTokenInfo.MerchantManagementAccountID, accessTokenInfo.MerchantManagementAccountName);
            AssertAccountFields(accessTokenInfo, "FPA_", accessTokenInfo.FileProcessingAccountID, accessTokenInfo.FileProcessingAccountName);
            Assert.AreEqual("tokenization", accessTokenInfo.TokenizationAccountName);
            Assert.AreEqual("settlement_reporting", accessTokenInfo.DataAccountName);
            Assert.AreEqual("dispute_management", accessTokenInfo.DisputeManagementAccountName);
            Assert.AreEqual("dcc_rate", accessTokenInfo.TransactionProcessingAccountName);
        }

        private static void AssertAccountFields(AccessTokenInfo accessTokenInfo, string accountPrefix, string accountId, string accountName) {
            var account = accessTokenInfo.Accounts.FirstOrDefault(item => item.Id.StartsWith(accountPrefix));

            Assert.AreEqual(account?.Id, accountId);
            Assert.AreEqual(account?.Name, accountName);
        }
    }
}