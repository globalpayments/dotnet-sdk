using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace GlobalPayments.Api.Tests.GpApi {
    [TestClass]
    public class GpApiTokenManagementTest : BaseGpApiTests {
        private static string Token;
        private const string CURRENCY = "USD";

        private static CreditCardData Card = new CreditCardData {
            Number = "4111111111111111",
            ExpMonth = ExpMonth,
            ExpYear = ExpYear,
            Cvn = "123"
        };

        [ClassInitialize]
        public static void ClassInitialize(TestContext context) {
            var gpApiConfig = GpApiConfigSetup(AppId, AppKey, Channel.CardNotPresent);
            ServicesContainer.ConfigureService(gpApiConfig);

            try {
                Token = Card.Tokenize();
                Assert.IsTrue(!string.IsNullOrEmpty(Token), "Token could not be generated.");
            }
            catch (GatewayException ex) {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void VerifyTokenizedPaymentMethod() {
            var tokenizedCard = new CreditCardData {
                Token = Token
            };

            var response = tokenizedCard.Verify()
                .WithCurrency(CURRENCY)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual(Success, response.ResponseCode);
            Assert.AreEqual(Verified, response.ResponseMessage);
        }

        [TestMethod]
        public void VerifyTokenizedPaymentMethod_withIdempotencyKey() {
            var idempotencyKey = Guid.NewGuid().ToString();

            var tokenizedCard = new CreditCardData {
                Token = Token
            };

            var response = tokenizedCard.Verify()
                .WithCurrency(CURRENCY)
                .WithIdempotencyKey(idempotencyKey)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual(Success, response.ResponseCode);
            Assert.AreEqual(Verified, response.ResponseMessage);

            var exceptionCaught = false;
            try {
                tokenizedCard.Verify()
                    .WithCurrency(CURRENCY)
                    .WithIdempotencyKey(idempotencyKey)
                    .Execute();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("40039", ex.ResponseMessage);
                Assert.AreEqual("DUPLICATE_ACTION", ex.ResponseCode);
                Assert.AreEqual(
                    $"Status Code: Conflict - Idempotency Key seen before: id={response.TransactionId}",
                    ex.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void UpdateTokenizedPaymentMethod() {
            var tokenizedCard = new CreditCardData {
                Token = Token,
                ExpMonth = 12,
                ExpYear = 2030
            };
            Assert.IsTrue(tokenizedCard.UpdateTokenExpiry());

            var response = tokenizedCard.Verify()
                .WithCurrency(CURRENCY)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual(Success, response.ResponseCode);
            Assert.AreEqual(Verified, response.ResponseMessage);
        }

        [TestMethod]
        public void CreditSaleWithTokenizedPaymentMethod() {
            var tokenizedCard = new CreditCardData {
                Token = Token
            };

            var response = tokenizedCard.Charge(19.99m)
                .WithCurrency(CURRENCY)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual(Success, response.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response.ResponseMessage);
        }

        [TestMethod]
        public void CreditSaleWithTokenizedPaymentMethod_WithStoredCredentials() {
            var tokenizedCard = new CreditCardData {
                Token = Token
            };

            var response = tokenizedCard.Charge(15.25m)
                .WithCurrency("USD")
                .WithStoredCredential(new StoredCredential {
                    Initiator = StoredCredentialInitiator.Merchant,
                    Type = StoredCredentialType.Subscription,
                    Sequence = StoredCredentialSequence.Subsequent,
                    Reason = StoredCredentialReason.Incremental
                })
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual(Success, response.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response.ResponseMessage);
        }

        [ClassCleanup]
        public static void Cleanup() {
            var tokenizedCard = new CreditCardData {
                Token = Token
            };
            Assert.IsTrue(tokenizedCard.DeleteToken());

            var exceptionCaught = false;
            try {
                tokenizedCard.Verify()
                    .WithCurrency("USD")
                    .Execute();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("RESOURCE_NOT_FOUND", ex.ResponseCode);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }
    }
}
