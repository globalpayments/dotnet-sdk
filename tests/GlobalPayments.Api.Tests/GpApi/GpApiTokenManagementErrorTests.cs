using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace GlobalPayments.Api.Tests.GpApi {
    [TestClass]
    public class GpApiTokenManagementErrorTests : BaseGpApiTests {
        private CreditCardData _card;
        private string _token;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context) {
            ServicesContainer.ConfigureService(new GpApiConfig {
                AppId = "P3LRVjtGRGxWQQJDE345mSkEh2KfdAyg",
                AppKey = "ockJr6pv6KFoGiZA",
            });
        }

        [TestMethod]
        public void VerifyTokenizedPaymentMethod_WithMalformedId() {
            string token = "This_is_not_a_payment_id";

            var tokenizedCard = new CreditCardData {
                Token = token,
            };

            try {
                tokenizedCard.Verify()
                    .WithCurrency("USD")
                    .Execute();
            }
            catch (GatewayException ex) {
                Assert.AreEqual("INVALID_REQUEST_DATA", ex.ResponseCode);
                Assert.AreEqual("40006", ex.ResponseMessage);
                Assert.AreEqual($"Status Code: BadRequest - payment_method.id: {token} contains unexpected data", ex.Message);
            }
        }

        [TestMethod]
        public void VerifyTokenizedPaymentMethod_WithMissingCardNumber() {
            _card = new CreditCardData {
                ExpMonth = 12,
                ExpYear = 2025,
                Cvn = "123"
            };
            try {
                _token = _card.Tokenize();
            }
            catch (GatewayException ex) {
                Assert.AreEqual("MANDATORY_DATA_MISSING", ex.ResponseCode);
                Assert.AreEqual("40005", ex.ResponseMessage);
                Assert.AreEqual("Status Code: BadRequest - Request expects the following fields : number", ex.Message);
            }
        }

        [TestMethod]
        public void VerifyTokenizedPaymentMethod_WithRandomId() {
            var tokenizedCard = new CreditCardData {
                Token = "PMT_" + Guid.NewGuid().ToString(),
            };

            try {
                var response = tokenizedCard.Verify()
                    .WithCurrency("USD")
                    .Execute();
            }
            catch (GatewayException ex) {
                Assert.AreEqual("RESOURCE_NOT_FOUND", ex.ResponseCode);
                Assert.AreEqual("40116", ex.ResponseMessage);
                Assert.IsTrue(ex.Message.StartsWith("Status Code: NotFound - payment_method"));
            }
        }

        [TestMethod]
        public void UpdateTokenizedPaymentMethod_WithMalformedId() {
            var tokenizedCard = new CreditCardData {
                Token = "This_is_not_a_payment_id",
                ExpMonth = 12,
                ExpYear = 2030
            };
            Assert.IsFalse(tokenizedCard.UpdateTokenExpiry());
        }

        [TestMethod]
        public void UpdateTokenizedPaymentMethod_WithRandomId() {
            var tokenizedCard = new CreditCardData {
                Token = "PMT_" + Guid.NewGuid().ToString(),
                ExpMonth = 12,
                ExpYear = 2030
            };
            Assert.IsFalse(tokenizedCard.UpdateTokenExpiry());
        }

        [TestMethod]
        public void DeleteTokenizedPaymentMethod_WithNonExistingId() {
            _card = new CreditCardData {
                Number = "4111111111111111",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvn = "123"
            };

            var tokenizedCard = new CreditCardData {
                Token = _card.Tokenize()
            };

            Assert.IsNotNull(tokenizedCard.Token);
            Assert.IsTrue(tokenizedCard.DeleteToken());
            Assert.IsFalse(tokenizedCard.DeleteToken());

            try {
                tokenizedCard.Verify()
                    .WithCurrency("USD")
                    .Execute();
            }
            catch (GatewayException ex) {
                Assert.AreEqual("RESOURCE_NOT_FOUND", ex.ResponseCode);
                Assert.AreEqual("40116", ex.ResponseMessage);
                Assert.IsTrue(ex.Message.StartsWith("Status Code: NotFound - payment_method"));
            }
        }

        [TestMethod]
        public void DeleteTokenizedPaymentMethod_WithRandomId() {
            var tokenizedCard = new CreditCardData {
                Token = "PMT_" + Guid.NewGuid().ToString()
            };

            Assert.IsFalse(tokenizedCard.DeleteToken());

            try {
                tokenizedCard.Verify()
                    .WithCurrency("USD")
                    .Execute();
            }
            catch (GatewayException ex) {
                Assert.AreEqual("RESOURCE_NOT_FOUND", ex.ResponseCode);
                Assert.AreEqual("40116", ex.ResponseMessage);
                Assert.IsTrue(ex.Message.StartsWith("Status Code: NotFound - payment_method"));
            }
        }

        [TestMethod]
        public void DeleteTokenizedPaymentMethod_WithMalformedId() {
            string token = "This_is_not_a_payment_id";

            var tokenizedCard = new CreditCardData {
                Token = token
            };

            Assert.IsFalse(tokenizedCard.DeleteToken());

            try {
                tokenizedCard.Verify()
                    .WithCurrency("USD")
                    .Execute();
            }
            catch (GatewayException ex) {
                Assert.AreEqual("INVALID_REQUEST_DATA", ex.ResponseCode);
                Assert.AreEqual("40006", ex.ResponseMessage);
                Assert.AreEqual($"Status Code: BadRequest - payment_method.id: {token} contains unexpected data", ex.Message);
            }
        }
    }
}
