using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace GlobalPayments.Api.Tests.GpApi {
    [TestClass]
    public class GpApiTokenManagementTests : BaseGpApiTests {
        private static string Token;

        private static CreditCardData Card = new CreditCardData {
            Number = "4111111111111111",
            ExpMonth = 12,
            ExpYear = 2025,
            Cvn = "123"
        };

        [ClassInitialize]
        public static void ClassInitialize(TestContext context) {
            ServicesContainer.ConfigureService(new GpApiConfig {
                AppId = "rkiYguPfTurmGcVhkDbIGKn2IJe2t09M",
                AppKey = "6gFzVGf40S7ZpjJs",
            });

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
            CreditCardData tokenizedCard = new CreditCardData {
                Token = Token,
            };

            Transaction response = tokenizedCard.Verify()
                .WithCurrency("USD")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("SUCCESS", response.ResponseCode);
            Assert.AreEqual("VERIFIED", response.ResponseMessage);
        }

        [TestMethod]
        public void VerifyTokenizedPaymentMethod_withIdempotencyKey() {
            string idempotencyKey = Guid.NewGuid().ToString();

            CreditCardData tokenizedCard = new CreditCardData {
                Token = Token,
            };

            Transaction response = tokenizedCard.Verify()
                .WithCurrency("USD")
                .WithIdempotencyKey(idempotencyKey)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("SUCCESS", response.ResponseCode);
            Assert.AreEqual("VERIFIED", response.ResponseMessage);

            try {
                tokenizedCard.Verify()
                    .WithCurrency("USD")
                    .WithIdempotencyKey(idempotencyKey)
                    .Execute();
            }
            catch (GatewayException ex) {
                Assert.AreEqual("40039", ex.ResponseMessage);
                Assert.AreEqual("DUPLICATE_ACTION", ex.ResponseCode);
                Assert.AreEqual(
                    $"Status Code: Conflict - Idempotency Key seen before: id={response.TransactionId}, status=VERIFIED",
                    ex.Message);
            }
        }

        [TestMethod]
        public void UpdateTokenizedPaymentMethod() {
            CreditCardData tokenizedCard = new CreditCardData {
                Token = Token,
                ExpMonth = 12,
                ExpYear = 2030
            };
            Assert.IsTrue(tokenizedCard.UpdateTokenExpiry());

            Transaction response = tokenizedCard.Verify()
                .WithCurrency("USD")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("SUCCESS", response.ResponseCode);
            Assert.AreEqual("VERIFIED", response.ResponseMessage);
        }

        [TestMethod]
        public void CreditSaleWithTokenizedPaymentMethod() {
            CreditCardData tokenizedCard = new CreditCardData {
                Token = Token,
            };

            Transaction response = tokenizedCard.Charge(19.99m)
                .WithCurrency("USD")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual(SUCCESS, response?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response?.ResponseMessage);
        }

        [TestMethod]
        public void CreditSaleWithTokenizedPaymentMethod_WithStoredCredentials() {
            CreditCardData tokenizedCard = new CreditCardData {
                Token = Token,
            };

            Transaction response = tokenizedCard.Charge(15.25m)
                .WithCurrency("USD")
                .WithStoredCredential(new StoredCredential {
                    Initiator = StoredCredentialInitiator.Merchant,
                    Type = StoredCredentialType.Subscription,
                    Sequence = StoredCredentialSequence.Subsequent,
                    Reason = StoredCredentialReason.Incremental
                })
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual(SUCCESS, response?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response?.ResponseMessage);
        }

        [ClassCleanup]
        public static void Cleanup() {
            CreditCardData tokenizedCard = new CreditCardData {
                Token = Token
            };
            Assert.IsTrue(tokenizedCard.DeleteToken());

            try {
                tokenizedCard.Verify()
                    .WithCurrency("USD")
                    .Execute();
            }
            catch (GatewayException ex) {
                Assert.AreEqual("RESOURCE_NOT_FOUND", ex.ResponseCode);
            }
        }
    }
}
