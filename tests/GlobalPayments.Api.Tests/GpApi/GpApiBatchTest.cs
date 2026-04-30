using System;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Utils;
using GlobalPayments.Api.Utils.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.GpApi {
    [TestClass]
    public class GpApiBatchTest : BaseGpApiTests {
        private CreditTrackData creditTrackData;

        private const string CURRENCY = "USD";
        private const decimal AMOUNT = 1.99m;
        private const string TAG_DATA =
            "82021C008407A0000002771010950580000000009A031709289C01005F280201245F2A0201245F3401019F02060000000010009F03060000000000009F080200019F090200019F100706010A03A420009F1A0201249F26089CC473F4A4CE18D39F2701809F3303E0F8C89F34030100029F3501229F360200639F370435EFED379F410400000019";

        [TestInitialize]
        public void TestInitialize() {
            ServicesContainer.RemoveConfig();
            var gpApiConfig = GpApiConfigSetup(AppId, AppKey, Channel.CardPresent);
            ServicesContainer.ConfigureService(gpApiConfig);

            creditTrackData = new CreditTrackData {
                Value =
                    "%B4012002000060016^VI TEST CREDIT^291210118039000000000396?;4012002000060016=29121011803939600000?",
                EntryMethod = EntryMethod.Swipe,
            };
        }

        [TestMethod]
        public void CloseBatch_ActionNotAuthorized() {
            ServicesContainer.ConfigureService(new GpApiConfig {
                AppId = "OWTP5ptQZKGj7EnvPt3uqO844XDBt8Oj",
                AppKey = "qM31FmlFiyXRHGYh",
                Channel = Channel.CardPresent
            });

    
            WaitForGpApiReplication();

            var exceptionCaught = false;
            try {
                var transaction = creditTrackData
                           .Charge(AMOUNT)
                           .WithCurrency(CURRENCY)
                           .Execute();
                Assert.IsNotNull(transaction);
                Assert.AreEqual("00", transaction.ResponseCode);
                Assert.AreEqual(GetMapping(TransactionStatus.Captured), transaction.ResponseMessage);
                BatchService.CloseBatch(transaction.BatchSummary.BatchReference);
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("ACTION_NOT_AUTHORIZED", ex.ResponseCode);
                Assert.AreEqual("40004", ex.ResponseMessage);
                Assert.AreEqual("Status Code: Forbidden - App credentials not recognized", ex.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void CloseBatch() {
            var chargeTransaction = creditTrackData.Charge(AMOUNT)
                .WithCurrency(CURRENCY)
                .Execute();
            Assert.IsNotNull(chargeTransaction);
            Assert.AreEqual("00", chargeTransaction.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), chargeTransaction.ResponseMessage);

            //TODO - remove when api fix polling issue
            WaitForGpApiReplication();

            var batchSummary = BatchService.CloseBatch(chargeTransaction.BatchSummary.BatchReference);
            AssertBatchCloseResponse(batchSummary, AMOUNT);
        }

        [TestMethod]
        public void CloseBatch_ChipTransaction() {
            var transaction = creditTrackData.Charge(AMOUNT)
                .WithCurrency(CURRENCY)
                .WithTagData(TAG_DATA)
                .Execute();
            Assert.IsNotNull(transaction);
            Assert.AreEqual("00", transaction.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), transaction.ResponseMessage);

            //TODO - remove when api fix polling issue
            WaitForGpApiReplication();

            var batchSummary = BatchService.CloseBatch(transaction.BatchSummary.BatchReference);
            AssertBatchCloseResponse(batchSummary, AMOUNT);
        }

        [TestMethod]
        public void CloseBatch_AuthAndCapture() {
            var authTransaction = creditTrackData.Authorize(AMOUNT)
                .WithCurrency(CURRENCY)
                .Execute();
            Assert.IsNotNull(authTransaction);
            Assert.AreEqual("00", authTransaction.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Preauthorized), authTransaction.ResponseMessage);

            var captureTransaction = authTransaction.Capture(AMOUNT)
                .WithCurrency(CURRENCY)
                .Execute();
            Assert.IsNotNull(captureTransaction);
            Assert.AreEqual("00", captureTransaction.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), captureTransaction.ResponseMessage);

            //TODO - remove when api fix polling issue
            WaitForGpApiReplication();

            var batchSummary = BatchService.CloseBatch(captureTransaction.BatchSummary.BatchReference);
            AssertBatchCloseResponse(batchSummary, AMOUNT);
        }

        [TestMethod]
        public void CloseBatch_ContactlessTransaction() {
            var debitCard = new DebitTrackData {
                Value = ";4024720012345671=29125025432198712345?",
                EntryMethod = EntryMethod.Proximity,
                PinBlock = "AFEC374574FC90623D010000116001EE"
            };

            var transaction = debitCard.Charge(AMOUNT)
                .WithCurrency(CURRENCY)
                .WithTagData(TAG_DATA)
                .Execute();
            Assert.IsNotNull(transaction);
            Assert.AreEqual("00", transaction.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), transaction.ResponseMessage);

            //TODO - remove when api fix polling issue
            WaitForGpApiReplication();

            var batchSummary = BatchService.CloseBatch(transaction.BatchSummary.BatchReference);
            AssertBatchCloseResponse(batchSummary, AMOUNT);
        }

        [TestMethod]
        public void CloseBatch_MultipleChargeCreditTrackData() {
            var firstTransaction = creditTrackData.Charge(1.25m)
                .WithCurrency(CURRENCY)
                .Execute();
            Assert.IsNotNull(firstTransaction);
            Assert.AreEqual("00", firstTransaction.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), firstTransaction.ResponseMessage);

            var secondTransaction = creditTrackData.Charge(2.03m)
                .WithCurrency(CURRENCY)
                .Execute();
            Assert.IsNotNull(secondTransaction);
            Assert.AreEqual("00", secondTransaction.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), secondTransaction.ResponseMessage);

            //TODO - remove when api fix polling issue
            WaitForGpApiReplication();

            var batchSummary = BatchService.CloseBatch(secondTransaction.BatchSummary.BatchReference);
            AssertBatchCloseResponse(batchSummary, 3.28m);
        }

        [TestMethod]
        public void CloseBatch_Refund_CreditTrackData() {
            var transaction = creditTrackData.Charge(AMOUNT)
                .WithCurrency(CURRENCY)
                .Execute();
            Assert.IsNotNull(transaction);
            Assert.AreEqual("00", transaction.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), transaction.ResponseMessage);

            var refundTransaction = transaction.Refund()
                .WithCurrency(CURRENCY)
                .Execute();
            Assert.IsNotNull(refundTransaction);
            Assert.AreEqual("00", refundTransaction.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), refundTransaction.ResponseMessage);

            //TODO - remove when api fix polling issue
            WaitForGpApiReplication();

            var batchSummary = BatchService.CloseBatch(refundTransaction.BatchSummary.BatchReference);
            AssertBatchCloseResponse(batchSummary, 0);
        }

        [TestMethod]
        public void CloseBatch_DebitTrackData() {
            var debitCard = new DebitTrackData {
                Value =
                    "%B4012002000060016^VI TEST CREDIT^291210118039000000000396?;4012002000060016=29121011803939600000?",
                PinBlock = "32539F50C245A6A93D123412324000AA",
                EntryMethod = EntryMethod.Swipe,
            };

            var transaction = debitCard.Charge(AMOUNT)
                .WithCurrency(CURRENCY)
                .Execute();
            Assert.IsNotNull(transaction);
            Assert.AreEqual("00", transaction.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), transaction.ResponseMessage);

            //TODO - remove when api fix polling issue
            WaitForGpApiReplication();

            var batchSummary = BatchService.CloseBatch(transaction.BatchSummary.BatchReference);
            AssertBatchCloseResponse(batchSummary, AMOUNT);
        }

        [TestMethod]
        public void CloseBatch_Reverse_DebitTrackData() {
            var debitCard = new DebitTrackData {
                Value =
                    "%B4012002000060016^VI TEST CREDIT^291210118039000000000396?;4012002000060016=29121011803939600000?",
                PinBlock = "32539F50C245A6A93D123412324000AA",
                EntryMethod = EntryMethod.Swipe,
            };

            var transaction = debitCard.Authorize(AMOUNT)
                .WithCurrency(CURRENCY)
                .Execute();
            Assert.IsNotNull(transaction);
            Assert.AreEqual("00", transaction.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Preauthorized), transaction.ResponseMessage);

            var reverseTransaction = transaction.Reverse()
                .WithCurrency(CURRENCY)
                .Execute();
            Assert.IsNotNull(reverseTransaction);
            Assert.AreEqual("00", reverseTransaction.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Reversed), reverseTransaction.ResponseMessage);

            //TODO - remove when api fix polling issue
            WaitForGpApiReplication();

            var exceptionCaught = false;
            try {
                BatchService.CloseBatch(transaction.BatchSummary.BatchReference);
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("INVALID_REQUEST_DATA", ex.ResponseCode);
                Assert.AreEqual("40007", ex.ResponseMessage);
                Assert.AreEqual("Status Code: BadRequest - Request expects the following conditionally mandatory fields account_id, account_name OR batch_id.", ex.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void CloseBatch_WithCardNumberDetails() {
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = ExpMonth,
                ExpYear = ExpYear,
                Cvn = "123",
                CardPresent = true
            };

            var chargeTransaction = card.Charge(AMOUNT)
                .WithCurrency(CURRENCY)
                .Execute();
            Assert.IsNotNull(chargeTransaction);
            Assert.AreEqual("00", chargeTransaction.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), chargeTransaction.ResponseMessage);

            //TODO - remove when api fix polling issue
            WaitForGpApiReplication();

            var batchSummary = BatchService.CloseBatch(chargeTransaction.BatchSummary.BatchReference);
            AssertBatchCloseResponse(batchSummary, AMOUNT);
        }

        [TestMethod]
        public void CloseBatch_WithCardNumberDetails_DeclinedTransaction() {
            var card = new CreditCardData {
                Number = "38865000000705",
                ExpMonth = ExpMonth,
                ExpYear = ExpYear,
                Cvn = "852",
                CardPresent = true
            };

            var chargeTransaction = card.Charge(AMOUNT)
                .WithCurrency(CURRENCY)
                .Execute();

            Assert.IsNotNull(chargeTransaction);
            Assert.AreEqual("14", chargeTransaction.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Declined), chargeTransaction.ResponseMessage);

            //TODO - remove when api fix polling issue
            WaitForGpApiReplication();

            var exceptionCaught = false;
            try {
                BatchService.CloseBatch(chargeTransaction.BatchSummary.BatchReference);
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("Status Code: BadGateway - 1,Gateway system error", ex.Message);
                Assert.AreEqual("SYSTEM_ERROR_DOWNSTREAM", ex.ResponseCode);
                Assert.AreEqual("50004", ex.ResponseMessage);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void CloseBatch_ReverseTransaction() {
            var chargeTransaction = creditTrackData.Authorize(AMOUNT)
                .WithCurrency(CURRENCY)
                .Execute();
            Assert.IsNotNull(chargeTransaction);
            Assert.AreEqual("00", chargeTransaction.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Preauthorized), chargeTransaction.ResponseMessage);

            var response = chargeTransaction.Reverse()
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Reversed), response.ResponseMessage);

            //TODO - remove when api fix polling issue
            WaitForGpApiReplication();

            var exceptionCaught = false;
            try {
                BatchService.CloseBatch(chargeTransaction.BatchSummary.BatchReference);
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("Status Code: BadRequest - Request expects the following conditionally mandatory fields account_id, account_name OR batch_id.", ex.Message);
                Assert.AreEqual("INVALID_REQUEST_DATA", ex.ResponseCode);
                Assert.AreEqual("40007", ex.ResponseMessage);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void CloseBatch_Auth_CreditCardData() {
            var transaction = creditTrackData.Authorize(AMOUNT)
                .WithCurrency(CURRENCY)
                .Execute();
            Assert.IsNotNull(transaction);
            Assert.AreEqual("00", transaction.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Preauthorized), transaction.ResponseMessage);

            //TODO - remove when api fix polling issue
            WaitForGpApiReplication();

            var exceptionCaught = false;
            try {
                BatchService.CloseBatch(transaction.BatchSummary.BatchReference);
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("INVALID_REQUEST_DATA", ex.ResponseCode);
                Assert.AreEqual("40007", ex.ResponseMessage);
                Assert.AreEqual("Status Code: BadRequest - Request expects the following conditionally mandatory fields account_id, account_name OR batch_id.", ex.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        [Ignore]
        //TODO - add idempotency key as header
        public void CloseBatch_WithIdempotency() {
            var idempotency = Guid.NewGuid().ToString();

            var transaction = creditTrackData.Charge(AMOUNT)
                .WithCurrency(CURRENCY)
                .Execute();
            Assert.IsNotNull(transaction);
            Assert.AreEqual("00", transaction.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), transaction.ResponseMessage);

            var batchSummary = BatchService.CloseBatch(transaction.BatchSummary.BatchReference);
            //.setIdempotency - ToDo

            Assert.IsNotNull(batchSummary);
            Assert.AreEqual(Closed, batchSummary.Status);

            BatchService.CloseBatch(transaction.BatchSummary.BatchReference);
        }

        [Ignore]
        [TestMethod]
        public void CloseBatch_WithClosedBatchReference() {
            var transaction = creditTrackData.Charge(AMOUNT)
                .WithCurrency(CURRENCY)
                .Execute();
            Assert.IsNotNull(transaction);
            Assert.AreEqual("00", transaction.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), transaction.ResponseMessage);

            //TODO - remove when api fix polling issue
            WaitForGpApiReplication();

            var batchSummary = BatchService.CloseBatch(transaction.BatchSummary.BatchReference);
            AssertBatchCloseResponse(batchSummary, AMOUNT);

            //TODO - remove when api fix polling issue
            WaitForGpApiReplication();

            var exceptionCaught = false;
            try {
                BatchService.CloseBatch(transaction.BatchSummary.BatchReference);
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("Status Code: BadGateway - Action failed unexpectedly. Please try again ", ex.Message);
                Assert.AreEqual("ACTION_FAILED", ex.ResponseCode);
                Assert.AreEqual("500010", ex.ResponseMessage);
            }
            finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void CloseBatch_Verify_MissingBatchId() {
            var transaction = creditTrackData.Verify()
                .WithCurrency(CURRENCY)
                .Execute();

            Assert.IsNotNull(transaction);
            Assert.AreEqual("00", transaction.ResponseCode);
            Assert.AreEqual(Verified, transaction.ResponseMessage);

            var exceptionCaught = false;
            try {
                BatchService.CloseBatch(transaction.BatchSummary.BatchReference);
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("INVALID_REQUEST_DATA", ex.ResponseCode);
                Assert.AreEqual("40007", ex.ResponseMessage);
                Assert.AreEqual($"Status Code: BadRequest - Request expects the following conditionally mandatory fields account_id, account_name OR batch_id.",
                    ex.Message);
            }
            finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void CloseBatch_CardNotPresentChannel() {
            var gpApiConfig = GpApiConfigSetup(AppId, AppKey, Channel.CardNotPresent);
            ServicesContainer.ConfigureService(gpApiConfig, "GP_API_CONFIG_NAME");

            var creditCardData = new CreditCardData {
                Number = "5425230000004415",
                ExpMonth = ExpMonth,
                ExpYear = ExpYear,
                Cvn = "852",
            };

            var transaction = creditCardData.Charge(AMOUNT)
                .WithCurrency(CURRENCY)
                .Execute("GP_API_CONFIG_NAME");
            Assert.IsNotNull(transaction);
            Assert.AreEqual("00", transaction.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), transaction.ResponseMessage);

            //TODO - remove when api fix polling issue
            WaitForGpApiReplication();

            var exceptionCaught = false;
            try
            {
                BatchService.CloseBatch(transaction.BatchSummary.BatchReference, "GP_API_CONFIG_NAME");
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("Status Code: BadGateway - -2,Authentication error—Verify and correct credentials",
                    ex.Message);
                Assert.AreEqual("UNAUTHORIZED_DOWNSTREAM", ex.ResponseCode);
                Assert.AreEqual("50002", ex.ResponseMessage);
            }
            finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void CloseBatch_WithInvalidBatchReference() {
            var batchReference = GenerationUtils.GenerateOrderId();

            var exceptionCaught = false;
            try {
                BatchService.CloseBatch(batchReference);
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("INVALID_REQUEST_DATA", ex.ResponseCode);
                Assert.AreEqual("40213", ex.ResponseMessage);
                Assert.AreEqual($"Status Code: BadRequest - batch_id contains unexpected data",
                    ex.Message);
                Assert.AreEqual("INVALID_REQUEST_DATA", ex.ResponseCode);
                Assert.AreEqual("40213", ex.ResponseMessage);
            }
            finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        private static void AssertBatchCloseResponse(BatchSummary batchSummary, decimal amount) {
            Assert.IsNotNull(batchSummary);
            Assert.AreEqual(Closed, batchSummary.Status);
            Assert.IsTrue(batchSummary.TransactionCount >= 1);
            Assert.IsTrue(batchSummary.TotalAmount >= amount);
        }
    }
}