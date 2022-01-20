using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Utils.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace GlobalPayments.Api.Tests.GpApi {
    [TestClass]
    public class GpApiTokenManagementErrorTests : BaseGpApiTests {
        private CreditCardData _card;
        private string _token;

        [TestInitialize]
        public void TestInitialize() {
            ServicesContainer.ConfigureService(new GpApiConfig {
                AppId = "rkiYguPfTurmGcVhkDbIGKn2IJe2t09M",
                AppKey = "6gFzVGf40S7ZpjJs",
                RequestLogger = new RequestConsoleLogger()
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
                Assert.AreEqual("40213", ex.ResponseMessage);
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
            try {
                tokenizedCard.UpdateTokenExpiry();
            } catch (GatewayException ex) {
                Assert.AreEqual("INVALID_REQUEST_DATA", ex.ResponseCode);
                Assert.AreEqual("40213", ex.ResponseMessage);
                Assert.IsTrue(ex.Message.StartsWith("Status Code: BadRequest - payment_method.id: This_is_not_a_payment_id contains unexpected data"));
            }
        }

        [TestMethod]
        public void UpdateTokenizedPaymentMethod_WithRandomId() {
            var tokenizedCard = new CreditCardData {
                Token = "PMT_" + Guid.NewGuid().ToString(),
                ExpMonth = 12,
                ExpYear = 2030
            };
            try {
                tokenizedCard.UpdateTokenExpiry();
            } catch (GatewayException ex) {
                Assert.AreEqual("RESOURCE_NOT_FOUND", ex.ResponseCode);
                Assert.AreEqual("40116", ex.ResponseMessage);
                Assert.IsTrue(ex.Message.StartsWith("Status Code: NotFound - payment_method"));
            }
        }

        [TestMethod]
        [Ignore]
        // The used credentials on this test have not permissions to delete a tokenized card
        public void DeleteTokenizedPaymentMethod_WithNonExistingId() {

            ServicesContainer.ConfigureService(new GpApiConfig
            {
                AppId = "rkiYguPfTurmGcVhkDbIGKn2IJe2t09M",
                AppKey = "6gFzVGf40S7ZpjJs",
            });

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
                Assert.AreEqual("ACTION_NOT_AUTHORIZED", ex.ResponseCode);
                Assert.AreEqual("40212", ex.ResponseMessage);
                Assert.IsTrue(ex.Message.StartsWith("Status Code: Forbidden - Permission not enabled to execute action"));
            }
        }

        [TestMethod, Ignore]
        // The used credentials on this test have not permissions to delete a tokenized card
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

        [TestMethod, Ignore]
        // The used credentials on this test have not permissions to delete a tokenized card
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
                Assert.AreEqual("40213", ex.ResponseMessage);
                Assert.AreEqual($"Status Code: BadRequest - payment_method.id: {token} contains unexpected data", ex.Message);
            }
        }
    }
}
