using System;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Utils.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.GpApi {
    [TestClass]
    public class GpApiTokenManagementErrorTests : BaseGpApiTests {
        private CreditCardData _card;

        [TestInitialize]
        public void TestInitialize() {
            ServicesContainer.ConfigureService(new GpApiConfig {
                AppId = APP_ID,
                AppKey = APP_KEY,
                RequestLogger = new RequestConsoleLogger()
            });
        }

        [TestMethod]
        public void VerifyTokenizedPaymentMethod_WithMalformedId() {
            const string token = "This_is_not_a_payment_id";

            var tokenizedCard = new CreditCardData {
                Token = token,
            };

            var exceptionCaught = false;
            try {
                tokenizedCard.Verify()
                    .WithCurrency("USD")
                    .Execute();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("INVALID_REQUEST_DATA", ex.ResponseCode);
                Assert.AreEqual("40213", ex.ResponseMessage);
                Assert.AreEqual($"Status Code: BadRequest - payment_method.id: {token} contains unexpected data", ex.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void VerifyTokenizedPaymentMethod_WithMissingCardNumber() {
            _card = new CreditCardData {
                ExpMonth = expMonth,
                ExpYear = expYear,
            };
            var exceptionCaught = false;
            try {
                _card.Tokenize();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("MANDATORY_DATA_MISSING", ex.ResponseCode);
                Assert.AreEqual("40005", ex.ResponseMessage);
                Assert.AreEqual("Status Code: BadRequest - Request expects the following fields : number", ex.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void VerifyTokenizedPaymentMethod_WithRandomId() {
            var tokenizedCard = new CreditCardData {
                Token = "PMT_" + Guid.NewGuid(),
            };

            var exceptionCaught = false;
            try {
                tokenizedCard.Verify()
                    .WithCurrency("USD")
                    .Execute();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("RESOURCE_NOT_FOUND", ex.ResponseCode);
                Assert.AreEqual("40116", ex.ResponseMessage);
                Assert.IsTrue(ex.Message.StartsWith("Status Code: NotFound - payment_method"));
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void UpdateTokenizedPaymentMethod_WithMalformedId() {
            var tokenizedCard = new CreditCardData {
                Token = "This_is_not_a_payment_id",
                ExpMonth = expMonth,
                ExpYear = expYear
            };
            var exceptionCaught = false;
            try {
                tokenizedCard.UpdateTokenExpiry();
            } catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("INVALID_REQUEST_DATA", ex.ResponseCode);
                Assert.AreEqual("40213", ex.ResponseMessage);
                Assert.IsTrue(ex.Message.StartsWith("Status Code: BadRequest - payment_method.id: This_is_not_a_payment_id contains unexpected data"));
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void UpdateTokenizedPaymentMethod_WithRandomId() {
            var tokenizedCard = new CreditCardData {
                Token = "PMT_" + Guid.NewGuid(),
                ExpMonth = expMonth,
                ExpYear = expYear
            };
            var exceptionCaught = false;
            try {
                tokenizedCard.UpdateTokenExpiry();
            } catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("RESOURCE_NOT_FOUND", ex.ResponseCode);
                Assert.AreEqual("40116", ex.ResponseMessage);
                Assert.IsTrue(ex.Message.StartsWith("Status Code: NotFound - payment_method"));
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        [Ignore]
        // The used credentials on this test have not permissions to delete a tokenized card
        public void DeleteTokenizedPaymentMethod_WithNonExistingId() {
            _card = new CreditCardData {
                Number = "4111111111111111",
                ExpMonth = expMonth,
                ExpYear = expYear,
                Cvn = "123"
            };

            var tokenizedCard = new CreditCardData {
                Token = _card.Tokenize()
            };

            Assert.IsNotNull(tokenizedCard.Token);
            Assert.IsTrue(tokenizedCard.DeleteToken());
            Assert.IsFalse(tokenizedCard.DeleteToken());

            var exceptionCaught = false;
            try {
                tokenizedCard.Verify()
                    .WithCurrency("USD")
                    .Execute();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("ACTION_NOT_AUTHORIZED", ex.ResponseCode);
                Assert.AreEqual("40212", ex.ResponseMessage);
                Assert.IsTrue(ex.Message.StartsWith("Status Code: Forbidden - Permission not enabled to execute action"));
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod, Ignore]
        // The used credentials on this test have not permissions to delete a tokenized card
        public void DeleteTokenizedPaymentMethod_WithRandomId() {
            var tokenizedCard = new CreditCardData {
                Token = "PMT_" + Guid.NewGuid()
            };

            Assert.IsFalse(tokenizedCard.DeleteToken());

            var exceptionCaught = false;
            try {
                tokenizedCard.Verify()
                    .WithCurrency("USD")
                    .Execute();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("RESOURCE_NOT_FOUND", ex.ResponseCode);
                Assert.AreEqual("40116", ex.ResponseMessage);
                Assert.IsTrue(ex.Message.StartsWith("Status Code: NotFound - payment_method"));
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod, Ignore]
        // The used credentials on this test have not permissions to delete a tokenized card
        public void DeleteTokenizedPaymentMethod_WithMalformedId() {
            const string token = "This_is_not_a_payment_id";

            var tokenizedCard = new CreditCardData {
                Token = token
            };

            Assert.IsFalse(tokenizedCard.DeleteToken());

            var exceptionCaught = false;
            try {
                tokenizedCard.Verify()
                    .WithCurrency("USD")
                    .Execute();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("INVALID_REQUEST_DATA", ex.ResponseCode);
                Assert.AreEqual("40213", ex.ResponseMessage);
                Assert.AreEqual($"Status Code: BadRequest - payment_method.id: {token} contains unexpected data", ex.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }
    }
}
