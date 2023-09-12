using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Utils.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.GpApi {

    [TestClass]
    public class GpApiAuthenticationTest : BaseGpApiTests {
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
        public void GenerateAccessTokenManualWithPermissions() {
            var permissions = new[] { "PMT_POST_Create", "PMT_POST_Detokenize" };
            gpApiConfig.Permissions = permissions;

            var info = GpApiService.GenerateTransactionKey(gpApiConfig);

            Assert.IsNotNull(info);
            Assert.IsNotNull(info.Token);
            Assert.AreEqual("tokenization", info.TokenizationAccountName);
            Assert.IsNull(info.DataAccountName);
            Assert.IsNull(info.DisputeManagementAccountName);
            Assert.IsNull(info.TransactionProcessingAccountName);
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
            Assert.AreEqual(Success, response?.ResponseCode);
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
            Assert.AreEqual(Success, response?.ResponseCode);
            Assert.AreEqual(Verified, response?.ResponseMessage);
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
            Assert.AreEqual(Success, response?.ResponseCode);
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
            Assert.AreEqual(Success, response?.ResponseCode);
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
                Assert.AreEqual("Status Code: Forbidden - Credentials not recognized to create access token.", ex.Message);
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
            Assert.AreEqual("tokenization", accessTokenInfo.TokenizationAccountName);
            Assert.AreEqual("settlement_reporting", accessTokenInfo.DataAccountName);
            Assert.AreEqual("dispute_management", accessTokenInfo.DisputeManagementAccountName);
            Assert.AreEqual("transaction_processing", accessTokenInfo.TransactionProcessingAccountName);
        }
    }
}