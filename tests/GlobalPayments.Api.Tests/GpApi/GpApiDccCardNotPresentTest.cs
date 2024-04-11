using System;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Utils.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.GpApi
{
    [TestClass]
    public class GpApiDccCardNotPresentTest : BaseGpApiTests {
        private static CreditCardData card;
        private const string CURRENCY = "EUR";
        private const decimal Amount = 10m;

        [TestInitialize]
        public void TestInitialize() {
            var gpApiConfig = GpApiConfigSetup(AppId, AppKey, Channel.CardNotPresent);
            gpApiConfig.AccessTokenInfo = new AccessTokenInfo { TransactionProcessingAccountName = "dcc" };
            ServicesContainer.ConfigureService(gpApiConfig);

            card = new CreditCardData {
                Number = "4006097467207025",
                ExpMonth = ExpMonth,
                ExpYear = ExpYear,
                CardPresent = true
            };
        }

        [TestMethod]
        public void CreditGetDccInfo() {
            var dccDetails = card.GetDccRate()
                .WithAmount(Amount)
                .WithCurrency(CURRENCY)
                .Execute();
            var expectedDccAmountValue = GetDccAmount(dccDetails);
            AssertDccInfoResponse(dccDetails, expectedDccAmountValue);

            WaitForGpApiReplication();
            var transaction = card.Charge(Amount)
                .WithCurrency(CURRENCY)
                .WithDccRateData(dccDetails.DccRateData)
                .Execute();
            AssertTransactionResponse(transaction, TransactionStatus.Captured, expectedDccAmountValue);
        }

        [TestMethod]
        public void CreditDccRateAuthorize() {
            var dccDetails = card.GetDccRate()
                .WithAmount(Amount)
                .WithCurrency(CURRENCY)
                .Execute();
            var expectedDccAmountValue = GetDccAmount(dccDetails);
            AssertDccInfoResponse(dccDetails, expectedDccAmountValue);

            WaitForGpApiReplication();
            var response = card.Authorize(Amount)
                .WithCurrency(CURRENCY)
                .WithDccRateData(dccDetails.DccRateData)
                .Execute();
            AssertTransactionResponse(response, TransactionStatus.Preauthorized, expectedDccAmountValue);
        }

        [TestMethod]
        public void CreditDccRateRefundStandalone() {
            var dccDetails = card.GetDccRate()
                .WithAmount(Amount)
                .WithCurrency(CURRENCY)
                .Execute();
            var expectedDccAmountValue = GetDccAmount(dccDetails);
            AssertDccInfoResponse(dccDetails, expectedDccAmountValue);

            WaitForGpApiReplication();
            var response = card.Refund(Amount)
                .WithCurrency(CURRENCY)
                .WithDccRateData(dccDetails.DccRateData)
                .Execute();
            AssertTransactionResponse(response, TransactionStatus.Captured, expectedDccAmountValue);
        }

        [TestMethod]
        public void CreditDccRateReversal() {
            var dccDetails = card.GetDccRate()
                .WithAmount(Amount)
                .WithCurrency(CURRENCY)
                .Execute();
            var expectedDccAmountValue = GetDccAmount(dccDetails);
            AssertDccInfoResponse(dccDetails, expectedDccAmountValue);

            WaitForGpApiReplication();
            var transaction = card.Charge(Amount)
                .WithCurrency(CURRENCY)
                .WithDccRateData(dccDetails.DccRateData)
                .Execute();
            AssertTransactionResponse(transaction, TransactionStatus.Captured, expectedDccAmountValue);

            var reverse = transaction.Reverse(Amount)
                .WithDccRateData(transaction.DccRateData)
                .Execute();
            AssertTransactionResponse(reverse, TransactionStatus.Reversed, expectedDccAmountValue);
        }

        [TestMethod]
        public void CreditDccRateRefund() {
            var dccDetails = card.GetDccRate()
                .WithAmount(Amount)
                .WithCurrency(CURRENCY)
                .Execute();
            var expectedDccAmountValue = GetDccAmount(dccDetails);
            AssertDccInfoResponse(dccDetails, expectedDccAmountValue);

            WaitForGpApiReplication();
            var transaction = card.Charge(Amount)
                .WithCurrency(CURRENCY)
                .WithDccRateData(dccDetails.DccRateData)
                .Execute();
            AssertTransactionResponse(transaction, TransactionStatus.Captured, expectedDccAmountValue);

            var refund = transaction.Refund()
                .WithCurrency(CURRENCY)
                .WithDccRateData(dccDetails.DccRateData)
                .Execute();
            AssertTransactionResponse(refund, TransactionStatus.Captured, expectedDccAmountValue);
        }

        [TestMethod]
        public void CreditDccRateAuthorizationThenCapture() {
            var dccDetails = card.GetDccRate()
                .WithAmount(Amount)
                .WithCurrency(CURRENCY)
                .Execute();
            var expectedDccAmountValue = GetDccAmount(dccDetails);
            AssertDccInfoResponse(dccDetails, expectedDccAmountValue);

            WaitForGpApiReplication();
            var transaction = card.Authorize(Amount)
                .WithCurrency(CURRENCY)
                .WithDccRateData(dccDetails.DccRateData)
                .Execute();
            AssertTransactionResponse(transaction, TransactionStatus.Preauthorized, expectedDccAmountValue);

            var capture = transaction.Capture()
                .WithDccRateData(dccDetails.DccRateData)
                .Execute();
            AssertTransactionResponse(capture, TransactionStatus.Captured, expectedDccAmountValue);
        }

        [TestMethod]
        public void CreditDccRateCardTokenizationThenPayingWithToken() {
            var tokenizedCard = new CreditCardData {
                Token = card.Tokenize()
            };

            var dccDetails = tokenizedCard.GetDccRate()
                .WithAmount(Amount)
                .WithCurrency(CURRENCY)
                .Execute();
            var expectedDccAmountValue = GetDccAmount(dccDetails);
            AssertDccInfoResponse(dccDetails, expectedDccAmountValue);

            WaitForGpApiReplication();
            var response = tokenizedCard.Charge(Amount)
                .WithCurrency(CURRENCY)
                .WithDccRateData(dccDetails.DccRateData)
                .Execute();
            AssertTransactionResponse(response, TransactionStatus.Captured, expectedDccAmountValue);
        }

        [TestMethod]
        public void CreditGetDccInfo_WithIdempotencyKey() {
            var idempotencyKey = Guid.NewGuid().ToString();

            var dccDetails = card.GetDccRate()
                .WithAmount(Amount)
                .WithCurrency(CURRENCY)
                .WithIdempotencyKey(idempotencyKey)
                .Execute();
            var expectedDccAmountValue = GetDccAmount(dccDetails);
            AssertDccInfoResponse(dccDetails, expectedDccAmountValue);

            WaitForGpApiReplication();
            var exceptionCaught = false;
            try {
                card.GetDccRate()
                    .WithAmount(Amount)
                    .WithCurrency(CURRENCY)
                    .WithIdempotencyKey(idempotencyKey)
                    .Execute();
            } catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("DUPLICATE_ACTION", ex.ResponseCode);
                Assert.AreEqual("40039", ex.ResponseMessage);
                Assert.IsTrue(
                    ex.Message.StartsWith("Status Code: Conflict - Idempotency Key seen before: id=" + dccDetails.TransactionId));
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void CreditGetDccInfo_RateNotAvailable() {
            card.Number = "4263970000005262";
            var dccDetails = card.GetDccRate()
                .WithAmount(Amount)
                .WithCurrency(CURRENCY)
                .Execute();

            Assert.IsNotNull(dccDetails);
            Assert.AreEqual(Success, dccDetails?.ResponseCode);
            Assert.AreEqual("NOT_AVAILABLE", dccDetails?.ResponseMessage);
            Assert.IsNotNull(dccDetails.DccRateData);

            WaitForGpApiReplication();
            var transaction = card.Charge(Amount)
                .WithCurrency(CURRENCY)
                .WithDccRateData(dccDetails.DccRateData)
                .Execute();
            AssertTransactionResponse(transaction, TransactionStatus.Captured, Amount);
        }

        [TestMethod]
        public void CreditGetDccInfo_InvalidCardNumber() {
            card.Number = "4000000000005262";
            var exceptionCaught = false;
            try {
                card.GetDccRate()
                    .WithAmount(Amount)
                    .WithCurrency(CURRENCY)
                    .Execute();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("INVALID_REQUEST_DATA", ex.ResponseCode);
                Assert.AreEqual("40090", ex.ResponseMessage);
                Assert.AreEqual(
                    "Status Code: BadRequest - card.number value is invalid. Please check the format and data provided is correct.",
                    ex.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void CreditGetDccInfo_WithoutAmount() {
            card.Number = "4263970000005262";
            var exceptionCaught = false;
            try {
                card.GetDccRate()
                    .WithCurrency(CURRENCY)
                    .Execute();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("MANDATORY_DATA_MISSING", ex.ResponseCode);
                Assert.AreEqual("40005", ex.ResponseMessage);
                Assert.AreEqual("Status Code: BadRequest - Request expects the following fields : amount",
                    ex.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void CreditGetDccInfo_WithoutCurrency() {
            card.Number = "4263970000005262";
            var exceptionCaught = false;
            try {
                card.GetDccRate()
                    .WithAmount(Amount)
                    .Execute();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("MANDATORY_DATA_MISSING", ex.ResponseCode);
                Assert.AreEqual("40005", ex.ResponseMessage);
                Assert.AreEqual("Status Code: BadRequest - Request expects the following fields : currency",
                    ex.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        private void AssertDccInfoResponse(Transaction dccDetails, decimal expectedDccAmountValue) {
            Assert.IsNotNull(dccDetails);
            Assert.AreEqual(Success, dccDetails?.ResponseCode);
            Assert.AreEqual("AVAILABLE", dccDetails?.ResponseMessage);
            Assert.IsNotNull(dccDetails.DccRateData);
            Assert.AreEqual(expectedDccAmountValue, dccDetails.DccRateData.CardHolderAmount);
        }

        private void AssertTransactionResponse(Transaction transaction, TransactionStatus transactionStatus, decimal expectedDccAmountValue) {
            Assert.IsNotNull(transaction);
            Assert.AreEqual(Success, transaction?.ResponseCode);
            Assert.AreEqual(GetMapping(transactionStatus), transaction?.ResponseMessage);
            if (!transactionStatus.Equals(TransactionStatus.Reversed)) {
                Assert.AreEqual(expectedDccAmountValue, transaction.DccRateData.CardHolderAmount);
            }
        }

        private decimal GetDccAmount(Transaction dccDetails) {
            var rate = dccDetails.DccRateData.CardHolderRate.GetValueOrDefault(0);
            return Amount * rate;
        }
    }
}