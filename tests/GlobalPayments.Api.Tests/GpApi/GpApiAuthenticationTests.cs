using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.GpApi {
    [TestClass]
    public class GpApiAuthenticationTests : BaseGpApiTests {
        CreditCardData card;

        [TestInitialize]
        public void TestInitialize() {
            card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 05,
                ExpYear = 2025,
                Cvn = "852",
            };
        }

        [TestMethod]
        public void GenerateAccessTokenManual() {
            var environment = Entities.Environment.TEST;
            string appId = "JF2GQpeCrOivkBGsTRiqkpkdKp67Gxi0";
            string appKey = "y7vALnUtFulORlTV";

            AccessTokenInfo info = GpApiService.GenerateTransactionKey(environment, appId, appKey);

            Assert.IsNotNull(info);
            Assert.IsNotNull(info.Token);
            Assert.IsNotNull(info.DataAccountName);
            Assert.IsNotNull(info.DisputeManagementAccountName);
            Assert.IsNotNull(info.TokenizationAccountName);
            Assert.IsNotNull(info.TransactionProcessingAccountName);
        }

        [TestMethod]
        public void GenerateAccessTokenManualWithPermissions() {
            var environment = Entities.Environment.TEST;
            string appId = "JF2GQpeCrOivkBGsTRiqkpkdKp67Gxi0";
            string appKey = "y7vALnUtFulORlTV";
            string[] permissions = new string[] { "PMT_POST_Create", "PMT_POST_Detokenize" };

            AccessTokenInfo info = GpApiService.GenerateTransactionKey(environment, appId, appKey, permissions: permissions);

            Assert.IsNotNull(info);
            Assert.IsNotNull(info.Token);
            Assert.IsNotNull(info.TokenizationAccountName);
            Assert.IsNull(info.DataAccountName);
            Assert.IsNull(info.DisputeManagementAccountName);
            Assert.IsNull(info.TransactionProcessingAccountName);
        }

        [TestMethod]
        public void GenerateAccessTokenManualWithWrongPermissions() {
            var environment = Entities.Environment.TEST;
            string appId = "JF2GQpeCrOivkBGsTRiqkpkdKp67Gxi0";
            string appKey = "y7vALnUtFulORlTV";
            string[] permissions = new string[] { "TEST_1", "TEST_2" };

            try {
                AccessTokenInfo info = GpApiService.GenerateTransactionKey(environment, appId, appKey, permissions: permissions);
            }
            catch (GatewayException ex) {
                Assert.AreEqual("INVALID_REQUEST_DATA", ex.ResponseCode);
                Assert.AreEqual("40119", ex.ResponseMessage);
                Assert.IsTrue(ex.Message.StartsWith("Status Code: BadRequest - Invalid permissions"));
            }
        }

        [TestMethod]
        public void GenerateAccessTokenManualWithSecondsToExpire() {
            ServicesContainer.ConfigureService(new GpApiConfig {
                Environment = Entities.Environment.TEST,
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
        public void GenerateAccessTokenManualWithIntervalToExpire() {
            ServicesContainer.ConfigureService(new GpApiConfig {
                Environment = Entities.Environment.TEST,
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
                Environment = Entities.Environment.TEST,
                AppId = "JF2GQpeCrOivkBGsTRiqkpkdKp67Gxi0",
                AppKey = "y7vALnUtFulORlTV",
                SecondsToExpire = 6000,
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
        public void GenerateAccessTokenManualWrongCredentials() {
            var environment = Entities.Environment.TEST;
            string appId = "T53SFpeCrOivkBGs84FtlpkdKp32FkYl";
            string appKey = "j8rTLnUtFulOU7VL";
            try {
                GpApiService.GenerateTransactionKey(environment, appId, appKey);
            }
            catch (GatewayException ex) {
                Assert.AreEqual("ACTION_NOT_AUTHORIZED", ex.ResponseCode);
                Assert.AreEqual("40004", ex.ResponseMessage);
                Assert.AreEqual("Status Code: Forbidden - Credentials not recognized to create access token.", ex.Message);
            }
        }

        [TestMethod]
        public void UseInvalidAccessToken() {
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
                //Because of the automatic retry this is the final error
                Assert.AreEqual("MANDATORY_DATA_MISSING", ex.ResponseCode);
                Assert.AreEqual("40005", ex.ResponseMessage);
                Assert.AreEqual("Status Code: BadRequest - Request expects the following fields app_id", ex.Message);
            }
        }

        [TestMethod]
        public void UseExpiredAccessToken() {
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
                //Because of the automatic retry this is the final error
                Assert.AreEqual("MANDATORY_DATA_MISSING", ex.ResponseCode);
                Assert.AreEqual("40005", ex.ResponseMessage);
                Assert.AreEqual("Status Code: BadRequest - Request expects the following fields app_id", ex.Message);
            }
        }
    }
}
