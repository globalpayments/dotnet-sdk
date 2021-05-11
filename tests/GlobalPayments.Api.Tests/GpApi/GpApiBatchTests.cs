using System;
using System.Threading;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.GpApi {
    [TestClass]
    public class GpApiBatchTests : BaseGpApiTests {
        private CreditTrackData creditCard;

        private const string CURRENCY = "USD";

        private const string TAG_DATA =
            "82021C008407A0000002771010950580000000009A031709289C01005F280201245F2A0201245F3401019F02060000000010009F03060000000000009F080200019F090200019F100706010A03A420009F1A0201249F26089CC473F4A4CE18D39F2701809F3303E0F8C89F34030100029F3501229F360200639F370435EFED379F410400000019";

        [ClassInitialize]
        public static void ClassInitialize(TestContext context) {
            ServicesContainer.ConfigureService(new GpApiConfig {
                AppId = "P3LRVjtGRGxWQQJDE345mSkEh2KfdAyg",
                AppKey = "ockJr6pv6KFoGiZA",
                Channel = Channel.CardPresent
            });
        }

        [TestInitialize]
        public void TestInitialize() {
            creditCard = new CreditTrackData() {
                Value =
                    "%B4012002000060016^VI TEST CREDIT^251210118039000000000396?;4012002000060016=25121011803939600000?",
                EntryMethod = EntryMethod.Swipe,
            };
        }

        [TestMethod]
        public void CloseBatch() {
            var chargeTransaction = creditCard.Charge(1.99m)
                .WithCurrency(CURRENCY)
                .Execute();
            AssertTransactionResponse(chargeTransaction, TransactionStatus.Captured);

            //TODO - remove when api fix polling issue
            Thread.Sleep(1000);

            var batchSummary = BatchService.CloseBatch(chargeTransaction.BatchSummary.BatchReference);
            AssertBatchCloseResponse(batchSummary, 1.99m);
        }

        [TestMethod]
        public void CloseBatch_ChipTransaction() {
            var transaction = creditCard.Charge(1.99m)
                .WithCurrency(CURRENCY)
                .WithTagData(TAG_DATA)
                .Execute();
            AssertTransactionResponse(transaction, TransactionStatus.Captured);

            //TODO - remove when api fix polling issue
            Thread.Sleep(1000);

            var batchSummary = BatchService.CloseBatch(transaction.BatchSummary.BatchReference);
            AssertBatchCloseResponse(batchSummary, 1.99m);
        }

        [TestMethod]
        public void CloseBatch_AuthAndCapture() {
            var authTransaction = creditCard.Authorize(1.99m)
                .WithCurrency(CURRENCY)
                .Execute();
            AssertTransactionResponse(authTransaction, TransactionStatus.Preauthorized);

            var captureTransaction = authTransaction.Capture(1.99m)
                .WithCurrency(CURRENCY)
                .Execute();
            AssertTransactionResponse(captureTransaction, TransactionStatus.Captured);

            //TODO - remove when api fix polling issue
            Thread.Sleep(1000);

            var batchSummary = BatchService.CloseBatch(captureTransaction.BatchSummary.BatchReference);
            AssertBatchCloseResponse(batchSummary, 1.99m);
        }

        [TestMethod]
        public void CloseBatch_ContactlessTransaction() {
            var debitCard = new DebitTrackData {
                Value = ";4024720012345671=18125025432198712345?",
                EntryMethod = EntryMethod.Proximity,
                PinBlock = "AFEC374574FC90623D010000116001EE"
            };

            var transaction = debitCard.Charge(1.99m)
                .WithCurrency(CURRENCY)
                .WithTagData(TAG_DATA)
                .Execute();
            AssertTransactionResponse(transaction, TransactionStatus.Captured);

            //TODO - remove when api fix polling issue
            Thread.Sleep(1000);

            var batchSummary = BatchService.CloseBatch(transaction.BatchSummary.BatchReference);
            AssertBatchCloseResponse(batchSummary, 1.99m);
        }

        [TestMethod]
        public void CloseBatch_MultipleChargeCreditTrackData() {
            var firstTransaction = creditCard.Charge(1.25m)
                .WithCurrency(CURRENCY)
                .Execute();
            AssertTransactionResponse(firstTransaction, TransactionStatus.Captured);

            var secondTransaction = creditCard.Charge(2.03m)
                .WithCurrency(CURRENCY)
                .Execute();
            AssertTransactionResponse(secondTransaction, TransactionStatus.Captured);

            //TODO - remove when api fix polling issue
            Thread.Sleep(1000);

            var batchSummary = BatchService.CloseBatch(secondTransaction.BatchSummary.BatchReference);
            AssertBatchCloseResponse(batchSummary, 3.28m);
        }

        [TestMethod]
        public void CloseBatch_Refund_CreditTrackData() {
            var transaction = creditCard.Charge(1.99m)
                .WithCurrency(CURRENCY)
                .Execute();
            AssertTransactionResponse(transaction, TransactionStatus.Captured);

            var refundTransaction = transaction.Refund()
                .WithCurrency(CURRENCY)
                .Execute();
            AssertTransactionResponse(refundTransaction, TransactionStatus.Captured);

            //TODO - remove when api fix polling issue
            Thread.Sleep(1000);

            var batchSummary = BatchService.CloseBatch(refundTransaction.BatchSummary.BatchReference);
            AssertBatchCloseResponse(batchSummary, 0);
        }

        [TestMethod]
        public void CloseBatch_DebitTrackData() {
            var debitCard = new DebitTrackData {
                Value =
                    "%B4012002000060016^VI TEST CREDIT^251210118039000000000396?;4012002000060016=25121011803939600000?",
                PinBlock = "32539F50C245A6A93D123412324000AA",
                EntryMethod = EntryMethod.Swipe,
            };

            var transaction = debitCard.Charge(1.99m)
                .WithCurrency(CURRENCY)
                .Execute();
            AssertTransactionResponse(transaction, TransactionStatus.Captured);

            //TODO - remove when api fix polling issue
            Thread.Sleep(1000);

            var batchSummary = BatchService.CloseBatch(transaction.BatchSummary.BatchReference);
            AssertBatchCloseResponse(batchSummary, 1.99m);
        }

        [TestMethod]
        public void CloseBatch_Reverse_DebitTrackData() {
            var debitCard = new DebitTrackData {
                Value =
                    "%B4012002000060016^VI TEST CREDIT^251210118039000000000396?;4012002000060016=25121011803939600000?",
                PinBlock = "32539F50C245A6A93D123412324000AA",
                EntryMethod = EntryMethod.Swipe,
            };

            var transaction = debitCard.Authorize(1.99m)
                .WithCurrency(CURRENCY)
                .Execute();
            AssertTransactionResponse(transaction, TransactionStatus.Preauthorized);

            var reverseTransaction = transaction.Reverse()
                .WithCurrency(CURRENCY)
                .Execute();
            AssertTransactionResponse(reverseTransaction, TransactionStatus.Reversed);

            //TODO - remove when api fix polling issue
            Thread.Sleep(1000);

            var exceptionCaught = false;
            try {
                BatchService.CloseBatch(transaction.BatchSummary.BatchReference);
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("MANDATORY_DATA_MISSING", ex.ResponseCode);
                Assert.AreEqual("40223", ex.ResponseMessage);
                Assert.AreEqual("Status Code: BadRequest - Request expects the batch_id", ex.Message);
            }
            finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void CloseBatch_WithCardNumberDetails() {
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 05,
                ExpYear = 2025,
                Cvn = "123",
            };

            var chargeTransaction = card.Charge(1.99m)
                .WithCurrency(CURRENCY)
                .Execute();
            AssertTransactionResponse(chargeTransaction, TransactionStatus.Captured);

            //TODO - remove when api fix polling issue
            Thread.Sleep(1000);

            var batchSummary = BatchService.CloseBatch(chargeTransaction.BatchSummary.BatchReference);
            AssertBatchCloseResponse(batchSummary, 1.99m);
        }

        [TestMethod]
        public void CloseBatch_WithCardNumberDetails_DeclinedTransaction() {
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 05,
                ExpYear = 2025,
                Cvn = "852",
            };

            var chargeTransaction = card.Charge(1.99m)
                .WithCurrency(CURRENCY)
                .Execute();

            Assert.IsNotNull(chargeTransaction);
            Assert.AreEqual("DECLINED", chargeTransaction?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Declined), chargeTransaction?.ResponseMessage);

            //TODO - remove when api fix polling issue
            Thread.Sleep(1000);

            var exceptionCaught = false;
            try {
                BatchService.CloseBatch(chargeTransaction.BatchSummary.BatchReference);
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("INVALID_BATCH_ACTION", ex.ResponseCode);
                Assert.AreEqual("40017", ex.ResponseMessage);
                Assert.AreEqual("Status Code: BadRequest - 9,No transaction associated with batch", ex.Message);
            }
            finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        [Ignore]
        //TODO - add idempotency key as header
        public void CloseBatch_WithIdempotency() {
            var idempotency = Guid.NewGuid().ToString();

            var transaction = creditCard.Charge(1.99m)
                .WithCurrency(CURRENCY)
                .Execute();
            AssertTransactionResponse(transaction, TransactionStatus.Captured);

            var batchSummary = BatchService.CloseBatch(transaction.BatchSummary.BatchReference);
            //.setIdempotency - ToDo

            Assert.IsNotNull(batchSummary);
            Assert.AreEqual(CLOSED, batchSummary?.Status);

            BatchService.CloseBatch(transaction.BatchSummary.BatchReference);
        }

        [TestMethod]
        public void CloseBatch_WithClosedBatchReference() {
            var transaction = creditCard.Charge(1.25m)
                .WithCurrency(CURRENCY)
                .Execute();
            AssertTransactionResponse(transaction, TransactionStatus.Captured);

            //TODO - remove when api fix polling issue
            Thread.Sleep(1000);

            var batchSummary = BatchService.CloseBatch(transaction.BatchSummary.BatchReference);
            AssertBatchCloseResponse(batchSummary, 1.25m);

            //TODO - remove when api fix polling issue
            Thread.Sleep(1000);

            var exceptionCaught = false;
            try {
                BatchService.CloseBatch(batchSummary.BatchReference);
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("INVALID_BATCH_ACTION", ex.ResponseCode);
                Assert.AreEqual("40014", ex.ResponseMessage);
                Assert.AreEqual("Status Code: BadRequest - 5,No current batch", ex.Message);
            }
            finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void CloseBatch_Verify_MissingBatchId() {
            var transaction = creditCard.Verify()
                .WithCurrency(CURRENCY)
                .Execute();

            Assert.IsNotNull(transaction);
            Assert.AreEqual(SUCCESS, transaction?.ResponseCode);
            Assert.AreEqual(VERIFIED, transaction?.ResponseMessage);

            var exceptionCaught = false;
            try {
                BatchService.CloseBatch(transaction.BatchSummary.BatchReference);
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("MANDATORY_DATA_MISSING", ex.ResponseCode);
                Assert.AreEqual("40223", ex.ResponseMessage);
                Assert.AreEqual($"Status Code: BadRequest - Request expects the batch_id",
                    ex.Message);
            }
            finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void CloseBatch_CardNotPresentChannel() {
            ServicesContainer.ConfigureService(new GpApiConfig {
                AppId = "P3LRVjtGRGxWQQJDE345mSkEh2KfdAyg",
                AppKey = "ockJr6pv6KFoGiZA",
                Channel = Channel.CardNotPresent,
            });

            var creditCardData = new CreditCardData {
                Number = "5425230000004415",
                ExpMonth = 05,
                ExpYear = 2025,
                Cvn = "852",
            };

            var transaction = creditCardData.Charge(1.25m)
                .WithCurrency(CURRENCY)
                .Execute();
            AssertTransactionResponse(transaction, TransactionStatus.Captured);

            //TODO - remove when api fix polling issue
            Thread.Sleep(1000);

            var exceptionCaught = false;
            try {
                BatchService.CloseBatch(transaction.BatchSummary.BatchReference);
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("UNAUTHORIZED_DOWNSTREAM", ex.ResponseCode);
                Assert.AreEqual("50002", ex.ResponseMessage);
                Assert.AreEqual($"Status Code: BadGateway - -2,Authentication error—Verify and correct credentials",
                    ex.Message);
            }
            finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void CloseBatch_WithInvalidBatchReference() {
            var batchReference = Guid.NewGuid().ToString().Replace("-", "");

            var exceptionCaught = false;
            try {
                BatchService.CloseBatch(batchReference);
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("RESOURCE_NOT_FOUND", ex.ResponseCode);
                Assert.AreEqual("40118", ex.ResponseMessage);
                Assert.AreEqual($"Status Code: NotFound - Batch {batchReference} not found at this location.",
                    ex.Message);
            }
            finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        private static void AssertBatchCloseResponse(BatchSummary batchSummary, decimal amount) {
            Assert.IsNotNull(batchSummary);
            Assert.AreEqual(CLOSED, batchSummary?.Status);
            Assert.IsTrue(batchSummary?.TransactionCount >= 1);
            Assert.IsTrue(batchSummary?.TotalAmount >= amount);
        }

        private void AssertTransactionResponse(Transaction transaction, TransactionStatus transactionStatus) {
            Assert.IsNotNull(transaction);
            Assert.AreEqual(SUCCESS, transaction?.ResponseCode);
            Assert.AreEqual(GetMapping(transactionStatus), transaction?.ResponseMessage);
        }
    }
}