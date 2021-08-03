using System;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.GpApi {
    [TestClass]
    public class GpApiDebitTests : BaseGpApiTests {
        [ClassInitialize]
        public static void ClassInitialize(TestContext context) {
            ServicesContainer.ConfigureService(new GpApiConfig {
                AppId = "Uyq6PzRbkorv2D4RQGlldEtunEeGNZll",
                AppKey = "QDsW1ETQKHX6Y4TA",
                Channel = Channel.CardPresent,
            });
        }

        [TestMethod]
        public void DebitSaleSwipe() {
            var track = new DebitTrackData {
                Value =
                    "%B4012002000060016^VI TEST CREDIT^251210118039000000000396?;4012002000060016=25121011803939600000?",
                PinBlock = "32539F50C245A6A93D123412324000AA",
                EntryMethod = EntryMethod.Swipe,
            };
            var response = track.Charge(17.01m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual(SUCCESS, response?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response?.ResponseMessage);
        }

        [TestMethod]
        public void DebitRefundChip() {
            var track = new DebitTrackData {
                TrackData = "%B4012002000060016^VI TEST CREDIT^251210118039000000000396?;4012002000060016=25121011803939600000?",
                EntryMethod = EntryMethod.Swipe,
            };

            string tagData =
                "9F4005F000F0A0019F02060000000025009F03060000000000009F2608D90A06501B48564E82027C005F3401019F360200029F0702FF009F0802008C9F0902008C9F34030403029F2701809F0D05F0400088009F0E0508000000009F0F05F0400098005F280208409F390105FFC605DC4000A800FFC7050010000000FFC805DC4004F8009F3303E0B8C89F1A0208409F350122950500000080005F2A0208409A031409109B02E8009F21030811539C01009F37045EED3A8E4F07A00000000310109F0607A00000000310108407A00000000310109F100706010A03A400029F410400000001";

            var response = track.Refund(15.99m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .WithTagData(tagData)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual(SUCCESS, response?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response?.ResponseMessage);
        }

        [TestMethod]
        public void DebitRefundSwipe() {
            var track = new DebitTrackData {
                Value =
                    "%B4012002000060016^VI TEST CREDIT^251210118039000000000396?;4012002000060016=25121011803939600000?",
                PinBlock = "32539F50C245A6A93D123412324000AA",
                EntryMethod = EntryMethod.Swipe,
            };

            var response = track.Refund(12.99m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual(SUCCESS, response?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response?.ResponseMessage);
        }

        [TestMethod]
        public void DebitReverse() {
            var track = new DebitTrackData {
                Value =
                    "%B4012002000060016^VI TEST CREDIT^251210118039000000000396?;4012002000060016=25121011803939600000?",
                PinBlock = "32539F50C245A6A93D123412324000AA",
                EntryMethod = EntryMethod.Swipe,
            };

            var transaction = track.Charge(4.99m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(transaction);
            Assert.AreEqual(SUCCESS, transaction?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), transaction?.ResponseMessage);

            var response = transaction.Reverse(4.99m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual(SUCCESS, response?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Reversed), response?.ResponseMessage);
        }

        [TestMethod]
        public void DebitSaleSwipeEncrypted() {
            var track = new DebitTrackData {
                Value =
                    "&lt;E1050711%B4012001000000016^VI TEST CREDIT^251200000000000000000000?|LO04K0WFOmdkDz0um+GwUkILL8ZZOP6Zc4rCpZ9+kg2T3JBT4AEOilWTI|+++++++Dbbn04ekG|11;4012001000000016=25120000000000000000?|1u2F/aEhbdoPixyAPGyIDv3gBfF|+++++++Dbbn04ekG|00|||/wECAQECAoFGAgEH2wYcShV78RZwb3NAc2VjdXJlZXhjaGFuZ2UubmV0PX50qfj4dt0lu9oFBESQQNkpoxEVpCW3ZKmoIV3T93zphPS3XKP4+DiVlM8VIOOmAuRrpzxNi0TN/DWXWSjUC8m/PI2dACGdl/hVJ/imfqIs68wYDnp8j0ZfgvM26MlnDbTVRrSx68Nzj2QAgpBCHcaBb/FZm9T7pfMr2Mlh2YcAt6gGG1i2bJgiEJn8IiSDX5M2ybzqRT86PCbKle/XCTwFFe1X|&gt;",
                PinBlock = "32539F50C245A6A93D123412324000AA",
                EntryMethod = EntryMethod.Swipe,
                EncryptionData = new EncryptionData {
                    Version = "01"
                }
            };

            var response = track.Charge(17.01m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual(SUCCESS, response?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response?.ResponseMessage);
        }

        [TestMethod]
        public void DebitSaleSwipeChip() {
            var track = new DebitTrackData {
                Value = ";4024720012345671=18125025432198712345?",
                EntryMethod = EntryMethod.Swipe,
                PinBlock = "AFEC374574FC90623D010000116001EE"
            };

            string tagData =
                "82021C008407A0000002771010950580000000009A031709289C01005F280201245F2A0201245F3401019F02060000000010009F03060000000000009F080200019F090200019F100706010A03A420009F1A0201249F26089CC473F4A4CE18D39F2701809F3303E0F8C89F34030100029F3501229F360200639F370435EFED379F410400000019";

            var response = track.Charge(15.99m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .WithTagData(tagData)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual(SUCCESS, response?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response?.ResponseMessage);
        }

        [TestMethod]
        public void DebitSaleContactlessChip() {
            var track = new DebitTrackData {
                Value = ";4024720012345671=18125025432198712345?",
                EntryMethod = EntryMethod.Proximity,
                PinBlock = "AFEC374574FC90623D010000116001EE"
            };

            string tagData =
                "82021C008407A0000002771010950580000000009A031709289C01005F280201245F2A0201245F3401019F02060000000010009F03060000000000009F080200019F090200019F100706010A03A420009F1A0201249F26089CC473F4A4CE18D39F2701809F3303E0F8C89F34030100029F3501229F360200639F370435EFED379F410400000019";

            var response = track.Charge(25.95m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .WithTagData(tagData)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual(SUCCESS, response?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response?.ResponseMessage);
        }

        [TestMethod]
        public void CreditTrackDataVerify() {
            var creditTrackData = new CreditTrackData {
                TrackData =
                    "%B4012002000060016^VI TEST CREDIT^251210118039000000000396?;4012002000060016=25121011803939600000?"
            };

            var response = creditTrackData.Verify()
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual(SUCCESS, response?.ResponseCode);
            Assert.AreEqual(VERIFIED, response?.ResponseMessage);
        }

        [TestMethod]
        public void CreditCardReauthorizeTransaction() {
            var card = InitializeCreditCardData();

            Transaction chargeTransaction = card.Charge(1.25m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(chargeTransaction);
            Assert.AreEqual(SUCCESS, chargeTransaction?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), chargeTransaction?.ResponseMessage);

            Transaction reverseTransaction = chargeTransaction.Reverse(1.25m)
                .Execute();
            Assert.IsNotNull(reverseTransaction);
            Assert.AreEqual(SUCCESS, reverseTransaction?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Reversed), reverseTransaction?.ResponseMessage);

            Transaction reauthTransaction = reverseTransaction.Reauthorize()
                .Execute();
            Assert.IsNotNull(reauthTransaction);
            Assert.AreEqual(SUCCESS, reauthTransaction?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), reauthTransaction?.ResponseMessage);
            Assert.AreEqual("00", reauthTransaction.AuthorizationCode);
        }

        [TestMethod]
        public void CreditCardReauthorizeAReversedAuthorizedTransaction() {
            var card = InitializeCreditCardData();

            Transaction chargeTransaction = card.Authorize(1.25m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(chargeTransaction);
            Assert.AreEqual(SUCCESS, chargeTransaction?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Preauthorized), chargeTransaction?.ResponseMessage);

            Transaction reverseTransaction = chargeTransaction.Reverse(1.25m)
                .Execute();
            Assert.IsNotNull(reverseTransaction);
            Assert.AreEqual(SUCCESS, reverseTransaction?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Reversed), reverseTransaction?.ResponseMessage);

            Transaction reauthTransaction = reverseTransaction.Reauthorize()
                .Execute();
            Assert.IsNotNull(reauthTransaction);
            Assert.AreEqual(SUCCESS, reauthTransaction?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Preauthorized), reauthTransaction?.ResponseMessage);
            Assert.AreEqual("00", reauthTransaction.AuthorizationCode);
        }

        [TestMethod]
        public void CreditCardReauthorizeTransaction_OldExistentSale() {
            DateTime startDate = DateTime.UtcNow.AddDays(-30);
            DateTime endDate = DateTime.UtcNow;

            PagedResult<TransactionSummary> response = ReportingService.FindTransactionsPaged(1, 1000)
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .Where(SearchCriteria.StartDate, startDate)
                .And(SearchCriteria.EndDate, endDate)
                .And(SearchCriteria.TransactionStatus, TransactionStatus.Preauthorized)
                .And(SearchCriteria.Channel, Channel.CardPresent)
                .Execute();

            Assert.IsNotNull(response?.Results);
            Assert.IsTrue(response.Results.Count > 0);

            var randomNumber = new Random().Next(1, response.Results.Count);
            var transaction = new Transaction {
                TransactionId = response.Results[randomNumber].TransactionId
            };

            Transaction reverseTransaction = transaction.Reverse()
                .Execute();
            Assert.IsNotNull(reverseTransaction);
            Assert.AreEqual(SUCCESS, reverseTransaction?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Reversed), reverseTransaction?.ResponseMessage);

            Transaction reauthTransaction = transaction.Reauthorize()
                .Execute();
            Assert.IsNotNull(reauthTransaction);
            Assert.AreEqual(SUCCESS, reauthTransaction?.ResponseCode);
            Assert.AreEqual("00", reauthTransaction.AuthorizationCode);
        }

        [TestMethod]
        public void CreditCardReauthorizeTransaction_WithIdempotencyKey() {
            string idempotencyKey = Guid.NewGuid().ToString();
            var card = InitializeCreditCardData();

            Transaction chargeTransaction = card.Charge(1.25m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(chargeTransaction);
            Assert.AreEqual(SUCCESS, chargeTransaction?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), chargeTransaction?.ResponseMessage);

            Transaction reverseTransaction = chargeTransaction.Reverse(1.25m)
                .Execute();
            Assert.IsNotNull(reverseTransaction);
            Assert.AreEqual(SUCCESS, reverseTransaction?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Reversed), reverseTransaction?.ResponseMessage);

            Transaction reauthTransaction = reverseTransaction.Reauthorize()
                .WithIdempotencyKey(idempotencyKey)
                .Execute();
            Assert.IsNotNull(reauthTransaction);
            Assert.AreEqual(SUCCESS, reauthTransaction?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), reauthTransaction?.ResponseMessage);
            Assert.AreEqual("00", reauthTransaction.AuthorizationCode);

            var exceptionCaught = false;
            try {
                reverseTransaction.Reauthorize()
                    .WithIdempotencyKey(idempotencyKey)
                    .Execute();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("DUPLICATE_ACTION", ex.ResponseCode);
                Assert.AreEqual("40039", ex.ResponseMessage);
                Assert.AreEqual(
                    $"Status Code: Conflict - Idempotency Key seen before: id={reauthTransaction.TransactionId}, status=CAPTURED",
                    ex.Message);
            }
            finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void CreditCardReauthorizeTransaction_Refund() {
            var card = InitializeCreditCardData();

            Transaction refundTransaction = card.Refund(1.25m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(refundTransaction);
            Assert.AreEqual(SUCCESS, refundTransaction?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), refundTransaction?.ResponseMessage);

            var exceptionCaught = false;
            try {
                refundTransaction.Reauthorize()
                    .Execute();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("INVALID_REQUEST_DATA", ex.ResponseCode);
                Assert.AreEqual("40213", ex.ResponseMessage);
                Assert.AreEqual("Status Code: BadRequest - An error occurred on the server.", ex.Message);
            }
            finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void CreditCardReauthorizeTransaction_NonExistentId() {
            var transaction = new Transaction { TransactionId = Guid.NewGuid().ToString() };
            var exceptionCaught = false;

            try {
                transaction.Reauthorize()
                    .Execute();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("RESOURCE_NOT_FOUND", ex.ResponseCode);
                Assert.AreEqual("40008", ex.ResponseMessage);
                Assert.AreEqual(
                    $"Status Code: NotFound - Transaction {transaction.TransactionId} not found at this location.",
                    ex.Message);
            }
            finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void CreditCardReauthorizeTransaction_SaleWithCapturedStatus() {
            var card = InitializeCreditCardData();

            Transaction transaction = card.Charge(1.25m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(transaction);
            Assert.AreEqual(SUCCESS, transaction?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), transaction?.ResponseMessage);

            var exceptionCaught = false;
            try {
                transaction.Reauthorize()
                    .Execute();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("INVALID_REQUEST_DATA", ex.ResponseCode);
                Assert.AreEqual("40044", ex.ResponseMessage);
                Assert.AreEqual(
                    "Status Code: BadRequest - 36, Invalid original transaction for reauthorization-This error is returned from a CreditAuth or CreditSale if the original transaction referenced by GatewayTxnId cannot be found. This is typically because the original does not meet the criteria for the sale or authorization by GatewayTxnID. This error can also be returned if the original transaction is found, but the card number has been written over with nulls after 30 days.",
                    ex.Message);
            }
            finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        private static CreditCardData InitializeCreditCardData() {
            var card = new CreditCardData {
                Number = "5425230000004415",
                ExpMonth = DateTime.Now.Month,
                ExpYear = DateTime.Now.Year + 1,
                Cvn = "123",
                CardHolderName = "John Smith"
            };

            return card;
        }
    }
}