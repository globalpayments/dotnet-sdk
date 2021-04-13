using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace GlobalPayments.Api.Tests.GpApi {
    [TestClass]
    public class GpApiTokenManagementTests : BaseGpApiTests {
        private CreditCardData _card;
        private string _token;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context) {
            ServicesContainer.ConfigureService(new GpApiConfig {
                AppId = "P3LRVjtGRGxWQQJDE345mSkEh2KfdAyg",
                AppKey = "ockJr6pv6KFoGiZA",
            });
        }

        [TestInitialize]
        public void Initialize() {
            try {
                _card = new CreditCardData {
                    Number = "4111111111111111",
                    ExpMonth = 12,
                    ExpYear = 2025,
                    Cvn = "123"
                };
                _token = _card.Tokenize();
                Assert.IsTrue(!string.IsNullOrEmpty(_token), "Token could not be generated.");
            }
            catch (ApiException ex) {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void VerifyTokenizedPaymentMethod() {
            var tokenizedCard = new CreditCardData {
                Token = _token,
            };

            var response = tokenizedCard.Verify()
                .WithCurrency("USD")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("SUCCESS", response.ResponseCode);
            Assert.AreEqual("VERIFIED", response.ResponseMessage);
        }

        [TestMethod]
        public void DetokenizePaymentMethod() {
            var tokenizedCard = new CreditCardData {
                Token = _token,
            };

            var response = tokenizedCard.Detokenize();

            Assert.IsNotNull(response);
            Assert.IsNull(response.Token);
            Assert.AreEqual(_card.Number, response.Number);
            Assert.AreEqual(_card.ShortExpiry, response.ShortExpiry);
        }

        [TestMethod]
        public void UpdateTokenizedPaymentMethod() {
            var tokenizedCard = new CreditCardData {
                Token = _token,
                ExpMonth = 12,
                ExpYear = 2030
            };
            Assert.IsTrue(tokenizedCard.UpdateTokenExpiry());

            var response = tokenizedCard.Verify()
                .WithCurrency("USD")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("SUCCESS", response.ResponseCode);
            Assert.AreEqual("VERIFIED", response.ResponseMessage);
        }

        [TestMethod]
        public void CreditSaleWithTokenizedPaymentMethod() {
            CreditCardData tokenizedCard = new CreditCardData {
                Token = _token,
            };
            var response = tokenizedCard.Charge(19.99m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual(SUCCESS, response?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response?.ResponseMessage);
        }

        [TestMethod]
        public void CreditSaleWithTokenizedPaymentMethod_WithStoredCredentials() {
            CreditCardData tokenizedCard = new CreditCardData {
                Token = _token,
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
            Assert.AreEqual(SUCCESS, response?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response?.ResponseMessage);
        }

        [TestCleanup]
        public void Cleanup() {
            var tokenizedCard = new CreditCardData {
                Token = _token
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
