using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Tests.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace GlobalPayments.Api.Tests.GpApi {
    [TestClass]
    public class GpApiCreditTests : BaseGpApiTests {
        CreditCardData card;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context) {
            ServicesContainer.ConfigureService(new GpApiConfig {
                AppId = "Uyq6PzRbkorv2D4RQGlldEtunEeGNZll",
                AppKey = "QDsW1ETQKHX6Y4TA",
                Channel = Channel.CardNotPresent,
                RequestLogger = new RequestFileLogger(@"C:\temp\gpapi\requestlog.txt")
            });
        }

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
        public void CreditAuthorization() {
            var transaction = card.Authorize(14m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(transaction);
            Assert.AreEqual(SUCCESS, transaction?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Preauthorized), transaction?.ResponseMessage);

            var capture = transaction.Capture(16m)
                .WithGratuity(2m)
                .Execute();
            Assert.IsNotNull(capture);
            Assert.AreEqual(SUCCESS, capture?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), capture?.ResponseMessage);
        }

        [TestMethod]
        public void CreditSale() {
            var address = new Address {
                StreetAddress1 = "123 Main St.",
                City = "Downtown",
                State = "NJ",
                Country = "US",
                PostalCode = "12345"
            };
            var response = card.Charge(19.99m)
                .WithCurrency("USD")
                .WithAddress(address)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual(SUCCESS, response?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response?.ResponseMessage);
        }

        [TestMethod]
        public void CreditRefund() {
            var response = card.Refund(16m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual(SUCCESS, response?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response?.ResponseMessage);
        }

        [TestMethod]
        public void CreditRefundTransaction() {
            var transaction = card.Charge(10.95m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(transaction);
            Assert.AreEqual(SUCCESS, transaction?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), transaction?.ResponseMessage);

            var response = transaction.Refund(10.95m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual(SUCCESS, response?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response?.ResponseMessage);
        }

        [TestMethod]
        public void CreditReverseTransaction() {
            var transaction = card.Charge(12.99m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(transaction);
            Assert.AreEqual(SUCCESS, transaction?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), transaction?.ResponseMessage);

            var response = transaction.Reverse(12.99m)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual(SUCCESS, response?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Reversed), response?.ResponseMessage);
        }

        [TestMethod]
        public void CreditPartialReverseTransaction() {
            var transaction = card.Charge(3.99m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(transaction);
            Assert.AreEqual(SUCCESS, transaction?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), transaction?.ResponseMessage);

            try {
                transaction.Reverse(1.29m).Execute();
            }
            catch (GatewayException ex) {
                Assert.AreEqual("INVALID_REQUEST_DATA", ex.ResponseCode);
                Assert.AreEqual("40006", ex.ResponseMessage);
                Assert.AreEqual("Status Code: BadRequest - partial reversal not supported", ex.Message);
            }
        }

        [TestMethod]
        public void CreditAuthorizationForMultiCapture() {
            var authorization = card.Authorize(14m)
                .WithCurrency("USD")
                .WithMultiCapture(true)
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(authorization);
            Assert.AreEqual(SUCCESS, authorization?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Preauthorized), authorization?.ResponseMessage);

            var capture1 = authorization.Capture(3m)
                .Execute();
            Assert.IsNotNull(capture1);
            Assert.AreEqual(SUCCESS, capture1?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), capture1?.ResponseMessage);

            var capture2 = authorization.Capture(5m)
                .Execute();
            Assert.IsNotNull(capture2);
            Assert.AreEqual(SUCCESS, capture2?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), capture2?.ResponseMessage);

            var capture3 = authorization.Capture(7m)
                .Execute();
            Assert.IsNotNull(capture3);
            Assert.AreEqual(SUCCESS, capture3?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), capture3?.ResponseMessage);
        }

        [TestMethod]
        public void CreditVerify() {
            var response = card.Verify()
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual(SUCCESS, response?.ResponseCode);
            Assert.AreEqual(VERIFIED, response?.ResponseMessage);
        }

        [TestMethod]
        public void CreditChargeTransactions_WithSameIdempotencyKey() {
            string idempotencyKey = Guid.NewGuid().ToString();

            var transaction1 = card.Charge(4.95m)
                .WithCurrency("USD")
                .WithIdempotencyKey(idempotencyKey)
                .Execute();
            Assert.IsNotNull(transaction1);
            Assert.AreEqual(SUCCESS, transaction1?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), transaction1?.ResponseMessage);

            try {
                card.Charge(4.95m)
                    .WithCurrency("USD")
                    .WithIdempotencyKey(idempotencyKey)
                    .Execute();
            }
            catch (GatewayException ex) {
                Assert.AreEqual("DUPLICATE_ACTION", ex.ResponseCode);
                Assert.AreEqual("40039", ex.ResponseMessage);
            }
        }
    }
}
