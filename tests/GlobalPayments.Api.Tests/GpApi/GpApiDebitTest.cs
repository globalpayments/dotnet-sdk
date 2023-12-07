using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.GpApi {
    [TestClass]
    public class GpApiDebitTest : BaseGpApiTests {
        
        private static DebitTrackData debitTrackData = new DebitTrackData();
        private const decimal amount = 12.02m;
        private const string currency = "USD";
        private static string tagData;
        
        [ClassInitialize]
        public static void ClassInitialize(TestContext context) {
            var gpApiConfig = GpApiConfigSetup(AppId, AppKey, Channel.CardPresent);
            ServicesContainer.ConfigureService(gpApiConfig);
        }

        [TestInitialize]
        public void TestInitialize() {
            debitTrackData.Value =
                "%B4012002000060016^VI TEST CREDIT^251210118039000000000396?;4012002000060016=25121011803939600000?";
            debitTrackData.PinBlock = "32539F50C245A6A93D123412324000AA";
            debitTrackData.EntryMethod = EntryMethod.Swipe;

            tagData =
                "9F4005F000F0A0019F02060000000025009F03060000000000009F2608D90A06501B48564E82027C005F3401019F360200029F0702FF009F0802008C9F0902008C9F34030403029F2701809F0D05F0400088009F0E0508000000009F0F05F0400098005F280208409F390105FFC605DC4000A800FFC7050010000000FFC805DC4004F8009F3303E0B8C89F1A0208409F350122950500000080005F2A0208409A031409109B02E8009F21030811539C01009F37045EED3A8E4F07A00000000310109F0607A00000000310108407A00000000310109F100706010A03A400029F410400000001";
        }

        [TestMethod]
        public void DebitSaleSwipe() {
            var response = debitTrackData.Charge(amount)
                .WithCurrency(currency)
                .Execute();
            AssertTransactionResponse(response, TransactionStatus.Captured);
        }
        
        [TestMethod]
        public void DebitSaleSwipe_Chip() {
            var response = debitTrackData.Charge(amount)
                .WithCurrency(currency)
                .WithTagData(tagData)
                .Execute();
            AssertTransactionResponse(response, TransactionStatus.Captured);
        }
        
        [TestMethod]
        public void DebitSaleSwipe_AuthorizeThenCapture() {
            var response = debitTrackData.Authorize(amount)
                .WithCurrency(currency)
                .Execute();
            AssertTransactionResponse(response, TransactionStatus.Preauthorized);
            
            var captureResponse = response.Capture(amount)
                .WithCurrency(currency)
                .Execute();
            AssertTransactionResponse(captureResponse, TransactionStatus.Captured);
        }
        
        [TestMethod]
        public void DebitRefundSwipe() {
            var response = debitTrackData.Refund(amount)
                .WithCurrency(currency)
                .Execute();
            AssertTransactionResponse(response, TransactionStatus.Captured);
        }

        [TestMethod]
        public void DebitRefundChip() {
            debitTrackData.PinBlock = null;
            var response = debitTrackData.Refund(amount)
                .WithCurrency(currency)
                .WithTagData(tagData)
                .Execute();
            AssertTransactionResponse(response, TransactionStatus.Captured);
        }
        
        [TestMethod]
        public void DebitSaleSwipeEncrypted1() {
            debitTrackData.Value = "&lt;E1050711%B4012001000000016^VI TEST CREDIT^251200000000000000000000?|LO04K0WFOmdkDz0um+GwUkILL8ZZOP6Zc4rCpZ9+kg2T3JBT4AEOilWTI|+++++++Dbbn04ekG|11;4012001000000016=25120000000000000000?|1u2F/aEhbdoPixyAPGyIDv3gBfF|+++++++Dbbn04ekG|00|||/wECAQECAoFGAgEH2wYcShV78RZwb3NAc2VjdXJlZXhjaGFuZ2UubmV0PX50qfj4dt0lu9oFBESQQNkpoxEVpCW3ZKmoIV3T93zphPS3XKP4+DiVlM8VIOOmAuRrpzxNi0TN/DWXWSjUC8m/PI2dACGdl/hVJ/imfqIs68wYDnp8j0ZfgvM26MlnDbTVRrSx68Nzj2QAgpBCHcaBb/FZm9T7pfMr2Mlh2YcAt6gGG1i2bJgiEJn8IiSDX5M2ybzqRT86PCbKle/XCTwFFe1X|&gt;";
            debitTrackData.EncryptionData = EncryptionData.Version1();
            var response = debitTrackData.Charge(amount)
                .WithCurrency(currency)
                .Execute();
            AssertTransactionResponse(response, TransactionStatus.Captured);
        }
        
        [TestMethod]
        public void DebitSaleSwipe_NewDebitTrackDataDetails() {
            var debitTrack = new DebitTrackData {
                Value = ";4024720012345671=18125025432198712345?",
                PinBlock = "AFEC374574FC90623D010000116001EE",
                EntryMethod = EntryMethod.Swipe
            };
            const string tag = "82021C008407A0000002771010950580000000009A031709289C01005F280201245F2A0201245F3401019F02060000000010009F03060000000000009F080200019F090200019F100706010A03A420009F1A0201249F26089CC473F4A4CE18D39F2701809F3303E0F8C89F34030100029F3501229F360200639F370435EFED379F410400000019";
            
            var response = debitTrack.Charge(amount)
                .WithCurrency(currency)
                .WithTagData(tag)
                .Execute();
            AssertTransactionResponse(response, TransactionStatus.Captured);
        }
        
        [TestMethod]
        public void DebitSaleContactlessChip_NewDebitTrackDataDetails() {
            var debitTrack = new DebitTrackData {
                Value = ";4024720012345671=18125025432198712345?",
                PinBlock = "AFEC374574FC90623D010000116001EE",
                EntryMethod = EntryMethod.Proximity
            };
            const string tag = "82021C008407A0000002771010950580000000009A031709289C01005F280201245F2A0201245F3401019F02060000000010009F03060000000000009F080200019F090200019F100706010A03A420009F1A0201249F26089CC473F4A4CE18D39F2701809F3303E0F8C89F34030100029F3501229F360200639F370435EFED379F410400000019";
            
            var response = debitTrack.Charge(amount)
                .WithCurrency(currency)
                .WithTagData(tag)
                .Execute();
            AssertTransactionResponse(response, TransactionStatus.Captured);
        }

        [TestMethod]
        public void DebitReverse() {
            var transaction = debitTrackData.Charge(amount)
                .WithCurrency(currency)
                .Execute();
            AssertTransactionResponse(transaction, TransactionStatus.Captured);

            var response = transaction.Reverse(amount)
                .Execute();
            AssertTransactionResponse(response, TransactionStatus.Reversed);
        }

        [TestMethod]
        public void DebitRefundChip_Rejected() {
            var exceptionCaught = false;
            try {
                debitTrackData.Refund(amount)
                    .WithCurrency(currency)
                    .WithTagData(tagData)
                    .Execute();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("INVALID_REQUEST_DATA", ex.ResponseCode);
                Assert.AreEqual("40029", ex.ResponseMessage);
                Assert.AreEqual(
                    "Status Code: BadRequest - 34,Transaction rejected because the provided data was invalid. Online PINBlock Authentication not supported on offline transaction.",
                    ex.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        private void AssertTransactionResponse(Transaction transaction, TransactionStatus transactionStatus) {
            Assert.IsNotNull(transaction);
            Assert.AreEqual(Success, transaction.ResponseCode);
            Assert.AreEqual(GetMapping(transactionStatus), transaction.ResponseMessage);
        }
    }
}