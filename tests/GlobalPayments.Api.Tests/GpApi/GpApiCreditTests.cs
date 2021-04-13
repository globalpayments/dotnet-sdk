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
        public void CreditAuthorization_CaptureLowerAmount() {
            var transaction = card.Authorize(5m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(transaction);
            Assert.AreEqual(SUCCESS, transaction?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Preauthorized), transaction?.ResponseMessage);

            var capture = transaction.Capture(2.99m)
                .WithGratuity(2m)
                .Execute();
            Assert.IsNotNull(capture);
            Assert.AreEqual(SUCCESS, capture?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), capture?.ResponseMessage);
        }

        [TestMethod]
        public void CreditAuthorization_CaptureHigherAmount() {
            decimal amount = 10m;
            var transaction = card.Authorize(amount)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(transaction);
            Assert.AreEqual(SUCCESS, transaction?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Preauthorized), transaction?.ResponseMessage);

            var capture = transaction.Capture(amount * 1.15m)
                .WithGratuity(2m)
                .Execute();
            Assert.IsNotNull(capture);
            Assert.AreEqual(SUCCESS, capture?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), capture?.ResponseMessage);
        }

        [TestMethod]
        public void CreditAuthorization_CaptureHigherAmount_WithError() {
            decimal amount = 10m;
            var transaction = card.Authorize(amount)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(transaction);
            Assert.AreEqual(SUCCESS, transaction?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Preauthorized), transaction?.ResponseMessage);

            try {
                var capture = transaction.Capture(amount * 1.16m)
                    .WithGratuity(2m)
                    .Execute();
            }
            catch (GatewayException ex) {
                Assert.AreEqual("50020", ex.ResponseMessage);
                Assert.AreEqual("INVALID_REQUEST_DATA", ex.ResponseCode);
                Assert.AreEqual("Status Code: BadRequest - Can't settle for more than 115% of that which you authorised.", ex.Message);
            }
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
        public void CreditSale_WithRequestMultiUseToken() {
            var response = card.Charge(19.99m)
                .WithCurrency("USD")
                .WithRequestMultiUseToken(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual(SUCCESS, response?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response?.ResponseMessage);
            Assert.IsNotNull(response.Token);
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
            decimal amount = 10.95m;
            var transaction = card.Charge(amount)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(transaction);
            Assert.AreEqual(SUCCESS, transaction?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), transaction?.ResponseMessage);

            var response = transaction.Refund(amount)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual(SUCCESS, response?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response?.ResponseMessage);
        }

        [TestMethod]
        public void CreditRefundTransaction_RefundLowerAmount() {
            var transaction = card.Charge(5.95m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(transaction);
            Assert.AreEqual(SUCCESS, transaction?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), transaction?.ResponseMessage);

            var response = transaction.Refund(3.25m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual(SUCCESS, response?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response?.ResponseMessage);
        }

        [TestMethod]
        public void CreditRefundTransaction_RefundHigherAmount() {
            decimal amount = 5.95m;
            var transaction = card.Charge(amount)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(transaction);
            Assert.AreEqual(SUCCESS, transaction?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), transaction?.ResponseMessage);

            try {
                var response = transaction.Refund(amount * 1.1m)
                    .WithCurrency("USD")
                    .Execute();
            }
            catch (GatewayException ex) {
                Assert.AreEqual("40087", ex.ResponseMessage);
                Assert.AreEqual("INVALID_REQUEST_DATA", ex.ResponseCode);
                Assert.AreEqual("Status Code: BadRequest - You may only refund up to 100% of the original amount.", ex.Message);
            }
        }

        [TestMethod]
        public void CreditRefundTransactionWrongId() {
            var transaction = card.Charge(10.95m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .Execute();
            transaction.TransactionId = "TRN_wrongid3213213";
            transaction.Token = "TRN_wrongid3213213";

            try {
                transaction.Refund(10.95m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .Execute();
            }
            catch (GatewayException ex) {
                Assert.AreEqual("TRANSACTION_NOT_FOUND", ex.ResponseCode);
                Assert.AreEqual("40008", ex.ResponseMessage);
                Assert.AreEqual("Status Code: NotFound - Transaction to action cannot be found", ex.Message);
            }
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
        public void CreditReverseTransactionWrongId() {
            var transaction = card.Charge(12.99m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .Execute();
            transaction.TransactionId = "TRN_wrongid3213213";
            transaction.Token = "TRN_wrongid3213213";

            try {
                transaction.Refund(10.95m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .Execute();
            }
            catch (GatewayException ex) {
                Assert.AreEqual("TRANSACTION_NOT_FOUND", ex.ResponseCode);
                Assert.AreEqual("40008", ex.ResponseMessage);
                Assert.AreEqual("Status Code: NotFound - Transaction to action cannot be found", ex.Message);
            }
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
        public void CreditCaptureWrongId() {
            var authorization = card.Authorize(14m)
                .WithCurrency("USD")
                .WithMultiCapture(true)
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(authorization);
            Assert.AreEqual(SUCCESS, authorization?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Preauthorized), authorization?.ResponseMessage);
            authorization.TransactionId = "TRN_wrongid3213213";
            authorization.Token = "TRN_wrongid3213213";

            try {
                authorization.Capture(3m)
                    .Execute();
            }
            catch (GatewayException ex) {
                Assert.AreEqual("TRANSACTION_NOT_FOUND", ex.ResponseCode);
                Assert.AreEqual("40008", ex.ResponseMessage);
                Assert.AreEqual("Status Code: NotFound - Transaction to action cannot be found", ex.Message);
            }
        }

        [TestMethod]
        public void SaleWithTokenizedPaymentMethod() {
            var tokenizedCard = new CreditCardData {
                Token = card.Tokenize(),
            };

            var response = tokenizedCard.Charge(17.01m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual(SUCCESS, response?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response?.ResponseMessage);
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
        public void CreditVerify_WithAddress() {
            var address = new Address {
                PostalCode = "750241234",
                StreetAddress1 = "6860 Dallas Pkwy",
            };

            var response = card.Verify()
                .WithCurrency("USD")
                .WithAddress(address)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual(SUCCESS, response?.ResponseCode);
            Assert.AreEqual(VERIFIED, response?.ResponseMessage);
        }

        [TestMethod]
        public void CreditVerify_WithIdempotencyKey() {
            string idempotencyKey = Guid.NewGuid().ToString();

            var response = card.Verify()
                .WithCurrency("USD")
                .WithIdempotencyKey(idempotencyKey)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual(SUCCESS, response?.ResponseCode);
            Assert.AreEqual(VERIFIED, response?.ResponseMessage);
        }

        [TestMethod]
        public void CreditVerify_WithoutCurrency() {
            try {
                var response = card.Verify()
                    .Execute();
            }
            catch (GatewayException ex) {
                Assert.AreEqual("40005", ex.ResponseMessage);
                Assert.AreEqual("MANDATORY_DATA_MISSING", ex.ResponseCode);
                Assert.AreEqual("Status Code: BadRequest - Request expects the following fields currency", ex.Message);
            }
        }

        [TestMethod]
        public void CreditVerify_InvalidCVV() {
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 05,
                ExpYear = 2025,
                Cvn = "SMA",
            };
            try {
                card.Verify()
                .WithCurrency("USD")
                .Execute();
            }
            catch (GatewayException ex) {
                Assert.AreEqual("SYSTEM_ERROR_DOWNSTREAM", ex.ResponseCode);
                Assert.AreEqual("50018", ex.ResponseMessage);
            }
        }

        [TestMethod]
        public void CreditVerify_CP() {
            ServicesContainer.ConfigureService(new GpApiConfig {
                Environment = Entities.Environment.TEST,
                AppId = "JF2GQpeCrOivkBGsTRiqkpkdKp67Gxi0",
                AppKey = "y7vALnUtFulORlTV",
                SecondsToExpire = 60,
                Channel = Channel.CardPresent
            });
            card.Cvn = "123";
            var response = card.Verify()
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual(SUCCESS, response?.ResponseCode);
            Assert.AreEqual(VERIFIED, response?.ResponseMessage);
        }

        [TestMethod]
        public void CreditVerify_CP_CVNNotMatched() {
            ServicesContainer.ConfigureService(new GpApiConfig {
                Environment = Entities.Environment.TEST,
                AppId = "JF2GQpeCrOivkBGsTRiqkpkdKp67Gxi0",
                AppKey = "y7vALnUtFulORlTV",
                SecondsToExpire = 60,
                Channel = Channel.CardPresent
            });
            var response = card.Verify()
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("NOT_VERIFIED", response?.ResponseCode);
            Assert.AreEqual("NOT_VERIFIED", response?.ResponseMessage);
            Assert.AreEqual("NOT_MATCHED", response?.CvnResponseMessage);
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

        [TestMethod]
        public void CreditVerify_WithStoredCredentials() {
            var response = card.Verify()
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual(SUCCESS, response?.ResponseCode);
        }

        [TestMethod]
        public void CreditSale_WithStoredCredentials() {
            var response = card.Charge(15.25m)
                .WithCurrency("USD")
                .WithStoredCredential(new StoredCredential {
                    Initiator = StoredCredentialInitiator.CardHolder,
                    Type = StoredCredentialType.Subscription,
                    Sequence = StoredCredentialSequence.First,
                    Reason = StoredCredentialReason.Incremental
                })
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual(SUCCESS, response?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response?.ResponseMessage);
        }
    }
}
