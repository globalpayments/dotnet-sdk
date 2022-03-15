using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Utils.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.GpApi {
    [TestClass]
    public class GpApiAuthenticationTests : BaseGpApiTests {
        CreditCardData card;
        private const Environment ENVIRONMENT = Environment.TEST;
        private const string APP_ID = "JF2GQpeCrOivkBGsTRiqkpkdKp67Gxi0";
        private const string APP_KEY = "y7vALnUtFulORlTV";

        [TestInitialize]
        public void TestInitialize() {
            card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = expMonth,
                ExpYear = expYear,
                Cvn = "123",
            };
        }

        [TestMethod]
        public void GenerateAccessTokenManual() {
            var info = GpApiService.GenerateTransactionKey(ENVIRONMENT, APP_ID, APP_KEY);
            assertAccessTokenResponse(info);
        }

        [TestMethod]
        public void GenerateAccessTokenManualWithPermissions() {
            var permissions = new[] { "PMT_POST_Create", "PMT_POST_Detokenize" };

            var info =
                GpApiService.GenerateTransactionKey(ENVIRONMENT, APP_ID, APP_KEY, permissions: permissions);

            Assert.IsNotNull(info);
            Assert.IsNotNull(info.Token);
            Assert.AreEqual("Tokenization", info.TokenizationAccountName);
            Assert.IsNull(info.DataAccountName);
            Assert.IsNull(info.DisputeManagementAccountName);
            Assert.IsNull(info.TransactionProcessingAccountName);
        }

        [TestMethod]
        public void GenerateAccessTokenManualWithWrongPermissions() {
            var permissions = new[] { "TEST_1", "TEST_2" };

            try {
                GpApiService.GenerateTransactionKey(ENVIRONMENT, APP_ID, APP_KEY, permissions: permissions);
            }
            catch (GatewayException ex) {
                Assert.AreEqual("INVALID_REQUEST_DATA", ex.ResponseCode);
                Assert.AreEqual("40119", ex.ResponseMessage);
                Assert.IsTrue(ex.Message.StartsWith("Status Code: BadRequest - Invalid permissions"));
            }
        }

        [TestMethod]
        public void ChargeCard_WithLimitedPermissions() {
            var permissions = new[] { "TRN_POST_Capture" };

            ServicesContainer.ConfigureService(new GpApiConfig {
                Environment = Environment.TEST,
                AppId = "JF2GQpeCrOivkBGsTRiqkpkdKp67Gxi0",
                AppKey = "y7vALnUtFulORlTV",
                Permissions = permissions,
                SecondsToExpire = 60
            });

            var exceptionCaught = false;
            try {
                card.Charge()
                    .WithAmount(1.00m)
                    .WithCurrency("USD")
                    .Execute();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("ACTION_NOT_AUTHORIZED", ex.ResponseCode);
                Assert.AreEqual("40212", ex.ResponseMessage);
                Assert.AreEqual("Status Code: Forbidden - Permission not enabled to execute action", ex.Message);
            }
            finally {
                Assert.IsTrue(exceptionCaught);
            }
        }
        
        [TestMethod]
        public void GenerateAccessTokenManual_AccessToken() {
            var info =
                GpApiService.GenerateTransactionKey(ENVIRONMENT, APP_ID, APP_KEY);
            
            ServicesContainer.ConfigureService(new GpApiConfig {
                AccessTokenInfo = info
            });
            
            var response = card.Verify()
                .WithCurrency("USD")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual(SUCCESS, response?.ResponseCode);
            Assert.AreEqual(VERIFIED, response?.ResponseMessage);
        }

        [TestMethod]
        public void GenerateAccessTokenManualWithSecondsToExpire() {
            ServicesContainer.ConfigureService(new GpApiConfig {
                Environment = Environment.TEST,
                AppId = "JF2GQpeCrOivkBGsTRiqkpkdKp67Gxi0",
                AppKey = "y7vALnUtFulORlTV",
                SecondsToExpire = 60
            });

            var response = card.Verify()
                .WithCurrency("USD")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual(SUCCESS, response?.ResponseCode);
            Assert.AreEqual(VERIFIED, response?.ResponseMessage);
        }

        [TestMethod]
        public void GenerateAccessTokenManualWithMaximumSecondsToExpire() {
            try {
                GpApiService.GenerateTransactionKey(ENVIRONMENT, APP_ID, APP_KEY, 604801);
            }
            catch (GatewayException ex) {
                Assert.AreEqual("INVALID_REQUEST_DATA", ex.ResponseCode);
                Assert.AreEqual("40213", ex.ResponseMessage);
                Assert.AreEqual("Status Code: BadRequest - seconds_to_expire contains unexpected data", ex.Message);
            }
        }
        
        [TestMethod]
        public void GenerateAccessTokenManualWithInvalidSecondsToExpire() {
            try {
                GpApiService.GenerateTransactionKey(ENVIRONMENT, APP_ID, APP_KEY, 10);
            }
            catch (GatewayException ex) {
                Assert.AreEqual("INVALID_REQUEST_DATA", ex.ResponseCode);
                Assert.AreEqual("40213", ex.ResponseMessage);
                Assert.AreEqual("Status Code: BadRequest - seconds_to_expire contains unexpected data", ex.Message);
            }
        }

        [TestMethod]
        public void GenerateAccessTokenManualWithIntervalToExpire() {
            ServicesContainer.ConfigureService(new GpApiConfig {
                Environment = Environment.TEST,
                AppId = "JF2GQpeCrOivkBGsTRiqkpkdKp67Gxi0",
                AppKey = "y7vALnUtFulORlTV",
                IntervalToExpire = IntervalToExpire.THREE_HOURS
            });

            var response = card.Verify()
                .WithCurrency("USD")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual(SUCCESS, response?.ResponseCode);
            Assert.AreEqual(VERIFIED, response?.ResponseMessage);
        }

        [TestMethod]
        public void GenerateAccessTokenManualWithSecondsToExpireAndIntervalToExpire() {
            ServicesContainer.ConfigureService(new GpApiConfig {
                Environment = Environment.TEST,
                AppId = "JF2GQpeCrOivkBGsTRiqkpkdKp67Gxi0",
                AppKey = "y7vALnUtFulORlTV",
                SecondsToExpire = 60,
                IntervalToExpire = IntervalToExpire.FIVE_MINUTES
            });

            var response = card.Verify()
                .WithCurrency("USD")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual(SUCCESS, response?.ResponseCode);
            Assert.AreEqual(VERIFIED, response?.ResponseMessage);
        }

        [TestMethod]
        public void GenerateAccessTokenWithWrongAppId() {
            try {
                GpApiService.GenerateTransactionKey(ENVIRONMENT, APP_ID + "a", APP_KEY);
            }
            catch (GatewayException ex) {
                Assert.AreEqual("ACTION_NOT_AUTHORIZED", ex.ResponseCode);
                Assert.AreEqual("40004", ex.ResponseMessage);
                Assert.AreEqual("Status Code: Forbidden - App credentials not recognized", ex.Message);
            }
        }

        [TestMethod]
        public void GenerateAccessTokenWithWrongAppKey() {
            try {
                GpApiService.GenerateTransactionKey(ENVIRONMENT, APP_ID, APP_KEY + "a");
            }
            catch (GatewayException ex) {
                Assert.AreEqual("ACTION_NOT_AUTHORIZED", ex.ResponseCode);
                Assert.AreEqual("40004", ex.ResponseMessage);
                Assert.AreEqual("Status Code: Forbidden - Credentials not recognized to create access token.", ex.Message);
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

            try {
                card.Verify()
                    .WithCurrency("USD")
                    .Execute();
            }
            catch (GatewayException ex) {
                Assert.AreEqual("NOT_AUTHENTICATED", ex.ResponseCode);
                Assert.AreEqual("40001", ex.ResponseMessage);
                Assert.AreEqual("Status Code: Unauthorized - Invalid access token", ex.Message);
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

            try {
                card.Verify()
                    .WithCurrency("USD")
                    .Execute();
            }
            catch (GatewayException ex) {
                Assert.AreEqual("NOT_AUTHENTICATED", ex.ResponseCode);
                Assert.AreEqual("40001", ex.ResponseMessage);
                Assert.AreEqual("Status Code: Unauthorized - Invalid access token", ex.Message);
            }
        }

        private void assertAccessTokenResponse(AccessTokenInfo accessTokenInfo) {
            Assert.IsNotNull(accessTokenInfo);
            Assert.IsNotNull(accessTokenInfo.Token);
            Assert.AreEqual("Tokenization", accessTokenInfo.TokenizationAccountName);
            Assert.AreEqual("Settlement Reporting", accessTokenInfo.DataAccountName);
            Assert.AreEqual("Dispute Management", accessTokenInfo.DisputeManagementAccountName);
            Assert.AreEqual("Transaction_Processing", accessTokenInfo.TransactionProcessingAccountName);
        }
    }
}