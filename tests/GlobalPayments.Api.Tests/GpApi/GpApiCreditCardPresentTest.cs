using System;
using System.Collections.Generic;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Enums;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Environment = GlobalPayments.Api.Entities.Environment;

namespace GlobalPayments.Api.Tests.GpApi {
    
    [TestClass]
    public class GpApiCreditCardPresentTest : BaseGpApiTests {
        
        private CreditTrackData creditTrackData = new CreditTrackData();
        private CreditCardData card = new CreditCardData();
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
            creditTrackData.Value =
                "%B4012002000060016^VI TEST CREDIT^251210118039000000000396?;4012002000060016=25121011803939600000?";
            creditTrackData.PinBlock = "32539F50C245A6A93D123412324000AA";
            creditTrackData.EntryMethod = EntryMethod.Swipe;

            tagData =
                "9F4005F000F0A0019F02060000000025009F03060000000000009F2608D90A06501B48564E82027C005F3401019F360200029F0702FF009F0802008C9F0902008C9F34030403029F2701809F0D05F0400088009F0E0508000000009F0F05F0400098005F280208409F390105FFC605DC4000A800FFC7050010000000FFC805DC4004F8009F3303E0B8C89F1A0208409F350122950500000080005F2A0208409A031409109B02E8009F21030811539C01009F37045EED3A8E4F07A00000000310109F0607A00000000310108407A00000000310109F100706010A03A400029F410400000001";

            card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = ExpMonth,
                ExpYear = ExpYear,
                Cvn = "123",
                CardHolderName = "John Smith",
                CardPresent = true
            };
        }

        private void InitCreditTrackData(EntryMethod entryMethod = EntryMethod.Swipe)
        {
            creditTrackData = new CreditTrackData();
            creditTrackData.Value =
                "%B4012002000060016^VI TEST CREDIT^251210118039000000000396?;4012002000060016=25121011803939600000?";            
            creditTrackData.EntryMethod = entryMethod;
        }

        #region Create sale using Credit track data
        
        [TestMethod]
        public void CreditTrackData_SaleSwipe() {
            var response = creditTrackData.Charge(amount)
                .WithCurrency(currency)
                .Execute();
            AssertTransactionResponse(response, TransactionStatus.Captured);
        }
        
        [TestMethod]
        public void CreditTrackData_SaleSwipe_Chip() {
            creditTrackData.PinBlock = null;
            var response = creditTrackData.Charge(amount)
                .WithCurrency(currency)
                .WithTagData(tagData)
                .Execute();
            AssertTransactionResponse(response, TransactionStatus.Captured);
        }
        
        [TestMethod]
        public void CreditTrackData_SaleSwipe_Chip_WrongPin() {
            var response = creditTrackData.Charge(amount)
                .WithCurrency(currency)
                .WithTagData(tagData)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual(Declined, response?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Declined), response?.ResponseMessage);
            Assert.AreEqual("55", response.AuthorizationCode);
        }
        
        [TestMethod]
        public void CreditTrackData_SaleSwipe_AuthorizeThenCapture() {
            var response = creditTrackData.Authorize(amount)
                .WithCurrency(currency)
                .Execute();
            AssertTransactionResponse(response, TransactionStatus.Preauthorized);
            
            var captureResponse = response.Capture(amount)
                .WithCurrency(currency)
                .Execute();
            AssertTransactionResponse(captureResponse, TransactionStatus.Captured);
        }
        
        [TestMethod]
        public void CreditTrackData_RefundSwipe() {
            var response = creditTrackData.Refund(amount)
                .WithCurrency(currency)
                .Execute();
            AssertTransactionResponse(response, TransactionStatus.Captured);
        }
        
        [TestMethod]
        public void CreditTrackData_RefundChip() {
            creditTrackData.PinBlock = null;
            var response = creditTrackData.Refund(amount)
                .WithCurrency(currency)
                .WithTagData(tagData)
                .Execute();
            AssertTransactionResponse(response, TransactionStatus.Captured);
        }
        
        [TestMethod]
        public void CreditTrackData_SwipeEncrypted() {
            creditTrackData.Value = "&lt;E1050711%B4012001000000016^VI TEST CREDIT^251200000000000000000000?|LO04K0WFOmdkDz0um+GwUkILL8ZZOP6Zc4rCpZ9+kg2T3JBT4AEOilWTI|+++++++Dbbn04ekG|11;4012001000000016=25120000000000000000?|1u2F/aEhbdoPixyAPGyIDv3gBfF|+++++++Dbbn04ekG|00|||/wECAQECAoFGAgEH2wYcShV78RZwb3NAc2VjdXJlZXhjaGFuZ2UubmV0PX50qfj4dt0lu9oFBESQQNkpoxEVpCW3ZKmoIV3T93zphPS3XKP4+DiVlM8VIOOmAuRrpzxNi0TN/DWXWSjUC8m/PI2dACGdl/hVJ/imfqIs68wYDnp8j0ZfgvM26MlnDbTVRrSx68Nzj2QAgpBCHcaBb/FZm9T7pfMr2Mlh2YcAt6gGG1i2bJgiEJn8IiSDX5M2ybzqRT86PCbKle/XCTwFFe1X|&gt;";
            creditTrackData.EncryptionData = EncryptionData.Version1();
            var response = creditTrackData.Charge(amount)
                .WithCurrency(currency)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual(Declined, response?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Declined), response?.ResponseMessage);
            Assert.AreEqual("14", response.AuthorizationCode);
        }
        
        [TestMethod]
        public void CreditTrackData_SaleSwipeChip_ExpiredCreditTrackDataDetails() {
            var trackData = new CreditTrackData {
                Value = ";4024720012345671=18125025432198712345?",
                PinBlock = "AFEC374574FC90623D010000116001EE",
                EntryMethod = EntryMethod.Swipe
            };
            const string tag = "82021C008407A0000002771010950580000000009A031709289C01005F280201245F2A0201245F3401019F02060000000010009F03060000000000009F080200019F090200019F100706010A03A420009F1A0201249F26089CC473F4A4CE18D39F2701809F3303E0F8C89F34030100029F3501229F360200639F370435EFED379F410400000019";
            
            var response = trackData.Charge(amount)
                .WithCurrency(currency)
                .WithTagData(tag)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual(Declined, response?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Declined), response?.ResponseMessage);
            Assert.AreEqual("54", response.AuthorizationCode);
        }
        
        [TestMethod]
        public void CreditTrackData_SaleContactlessChip() {
            creditTrackData.PinBlock = null;
            creditTrackData.EntryMethod = EntryMethod.Proximity;
            
            var response = creditTrackData.Charge(amount)
                .WithCurrency(currency)
                .WithTagData(tagData)
                .Execute();
            AssertTransactionResponse(response, TransactionStatus.Captured);
        }
        
        [TestMethod]
        public void CreditTrackData_SaleContactlessSwipe() {
            creditTrackData.PinBlock = null;

            const string tag = "9F4005F000F0A0019F02060000000025009F03060000000000009F2608D90A06501B48564E82027C005F3401019F360200029F0702FF009F0802008C9F0902008C9F34030403029F2701809F0D05F0400088009F0E0508000000009F0F05F0400098005F280208409F390191FFC605DC4000A800FFC7050010000000FFC805DC4004F8009F3303E0B8C89F1A0208409F350122950500000080005F2A0208409A031409109B02E8009F21030811539C01009F37045EED3A8E4F07A00000000310109F0607A00000000310108407A00000000310109F100706010A03A400029F410400000001";
            
            var response = creditTrackData.Charge(amount)
                .WithCurrency(currency)
                .WithTagData(tag)
                .Execute();
            AssertTransactionResponse(response, TransactionStatus.Captured);
        }

        [TestMethod]
        public void AdjustSaleTransaction() {
            var card = new CreditTrackData();
                card.Value = "%B4012002000060016^VI TEST CREDIT^251210118039000000000396?;4012002000060016=25121011803939600000?";

            var tagData = "9F4005F000F0A0019F02060000000025009F03060000000000009F2608D90A06501B48564E82027C005F3401019F360200029F0702FF009F0802008C9F0902008C9F34030403029F2701809F0D05F0400088009F0E0508000000009F0F05F0400098005F280208409F390105FFC605DC4000A800FFC7050010000000FFC805DC4004F8009F3303E0B8C89F1A0208409F350122950500000080005F2A0208409A031409109B02E8009F21030811539C01009F37045EED3A8E4F07A00000000310109F0607A00000000310108407A00000000310109F100706010A03A400029F410400000001";
            
            card.EntryMethod = EntryMethod.Proximity;

            var transaction = card.Charge(10)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .WithTagData(tagData)
                .Execute();

                AssertTransactionResponse(transaction, TransactionStatus.Captured);

            var response = transaction.Edit()
                .WithAmount((decimal)10.01)
                .WithTagData(tagData)
                .WithGratuity((decimal)5.01)
                .Execute();

                AssertTransactionResponse(response, TransactionStatus.Captured);        
        }

        [TestMethod]
        public void AdjustAuthTransaction()
        {
            InitCreditTrackData(EntryMethod.Proximity);
           
            tagData = "9F4005F000F0A0019F02060000000025009F03060000000000009F2608D90A06501B48564E82027C005F3401019F360200029F0702FF009F0802008C9F0902008C9F34030403029F2701809F0D05F0400088009F0E0508000000009F0F05F0400098005F280208409F390105FFC605DC4000A800FFC7050010000000FFC805DC4004F8009F3303E0B8C89F1A0208409F350122950500000080005F2A0208409A031409109B02E8009F21030811539C01009F37045EED3A8E4F07A00000000310109F0607A00000000310108407A00000000310109F100706010A03A400029F410400000001";
                        
            var transaction = creditTrackData.Authorize(10)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .WithTagData(tagData)
                .Execute();

            AssertTransactionResponse(transaction, TransactionStatus.Preauthorized);

            var response = transaction.Edit()
                .WithAmount((decimal)10.01)
                .WithTagData(tagData)
                .WithGratuity((decimal)5.01)
                .WithMultiCapture((int)(StoredCredentialSequence.First), 1)
                .Execute();

            AssertTransactionResponse(response, TransactionStatus.Preauthorized);
        }

        [TestMethod]
        public void AdjustSaleTransaction_AdjustAmountHigherThanSale()
        {
            InitCreditTrackData();
                        
            var transaction = creditTrackData.Charge(10)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)                
                .Execute();

            AssertTransactionResponse(transaction, TransactionStatus.Captured);

            var response = transaction.Edit()
                .WithAmount((decimal)10 + 2)                
                .Execute();

            AssertTransactionResponse(response, TransactionStatus.Captured);
        }

        [TestMethod]
        public void AdjustSaleTransaction_AdjustOnlyTag()
        {
            InitCreditTrackData(EntryMethod.Proximity);

            tagData = "9F4005F000F0A0019F02060000000025009F03060000000000009F2608D90A06501B48564E82027C005F3401019F360200029F0702FF009F0802008C9F0902008C9F34030403029F2701809F0D05F0400088009F0E0508000000009F0F05F0400098005F280208409F390105FFC605DC4000A800FFC7050010000000FFC805DC4004F8009F3303E0B8C89F1A0208409F350122950500000080005F2A0208409A031409109B02E8009F21030811539C01009F37045EED3A8E4F07A00000000310109F0607A00000000310108407A00000000310109F100706010A03A400029F410400000001";

            var transaction = creditTrackData.Charge(10)
                .WithCurrency("USD")                
                .WithTagData(tagData)
                .WithAllowDuplicates(true)
                .Execute();

            AssertTransactionResponse(transaction, TransactionStatus.Captured);

            var response = transaction.Edit()
                .WithTagData(tagData)                
                .Execute();

            AssertTransactionResponse(response, TransactionStatus.Captured);
        }

        [TestMethod]
        public void AdjustSaleTransaction_AdjustOnlyGratuity()
        {
            InitCreditTrackData();
                       
            var transaction = creditTrackData.Charge(10)
                .WithCurrency("USD")
                .WithChipCondition(EmvLastChipRead.Successful)
                .WithAllowDuplicates(true)
                .Execute();

            AssertTransactionResponse(transaction, TransactionStatus.Captured);

            var response = transaction.Edit()
                .WithGratuity(1)
                .Execute();

            AssertTransactionResponse(response, TransactionStatus.Captured);
        }

        [TestMethod]
        public void AdjustSaleTransaction_AdjustAmountToZero()
        {
            InitCreditTrackData();

            var transaction = creditTrackData.Charge(10)
                .WithCurrency("USD")
                .WithChipCondition(EmvLastChipRead.Successful)
                .WithAllowDuplicates(true)
                .Execute();

            AssertTransactionResponse(transaction, TransactionStatus.Captured);

            var response = transaction.Edit()
                .WithAmount(0)
                .Execute();

            AssertTransactionResponse(response, TransactionStatus.Captured);
        }

        [TestMethod]
        public void AdjustSaleTransaction_AdjustGratuityToZero()
        {
            InitCreditTrackData();

            var transaction = creditTrackData.Charge(10)
                .WithCurrency("USD")
                .WithChipCondition(EmvLastChipRead.Successful)
                .WithAllowDuplicates(true)
                .Execute();

            AssertTransactionResponse(transaction, TransactionStatus.Captured);

            var response = transaction.Edit()
                .WithGratuity(0)
                .Execute();

            AssertTransactionResponse(response, TransactionStatus.Captured);
        }

        [TestMethod]
        public void AdjustSaleTransaction_WithoutMandatory()
        {
            InitCreditTrackData();

            var transaction = creditTrackData.Charge(10)
                .WithCurrency("USD")
                .WithChipCondition(EmvLastChipRead.Successful)
                .WithAllowDuplicates(true)
                .Execute();

            AssertTransactionResponse(transaction, TransactionStatus.Captured);

            var exceptionCaught = false;

            try
            {
                transaction.Edit()
                    .Execute();
            }
            catch (GatewayException ex)
            {
                exceptionCaught = true;
                Assert.AreEqual("40005", ex.ResponseMessage);
                Assert.AreEqual("Status Code: BadRequest - Request expects the following fields [amount or tag or gratuityAmount]", ex.Message);
            }
            finally
            {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void AdjustSaleTransaction_TransactionNotFound()
        {   
            var transaction = new Transaction();
            transaction.TransactionId = Guid.NewGuid().ToString();
            
            var exceptionCaught = false;
            try
            {
                transaction.Edit()
                    .Execute();
            }
            catch (GatewayException ex)
            {
                exceptionCaught = true;
                Assert.AreEqual("40008", ex.ResponseMessage);
                Assert.AreEqual($"Status Code: NotFound - Transaction {transaction.TransactionId} not found at this location.", ex.Message);
            }
            finally
            {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void CreditTrackData_SaleContactlessChip_WrongPin() {
            creditTrackData.EntryMethod = EntryMethod.Proximity;
            
            var response = creditTrackData.Charge(amount)
                .WithCurrency(currency)
                .WithTagData(tagData)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual(Declined, response?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Declined), response?.ResponseMessage);
            Assert.AreEqual("55", response.AuthorizationCode);
        }
        
        [TestMethod]
        public void CreditTrackData_SaleSwipe_Refund() {
            var response = creditTrackData.Charge(amount)
                .WithCurrency(currency)
                .Execute();
            AssertTransactionResponse(response, TransactionStatus.Captured);
            
            var refundTransaction = response.Refund(amount)
                .WithCurrency(currency)
                .Execute();
            AssertTransactionResponse(refundTransaction, TransactionStatus.Captured);
        }
        
        [TestMethod]
        public void CreditTrackData_SaleSwipe_Reverse() {
            var response = creditTrackData.Charge(amount)
                .WithCurrency(currency)
                .Execute();
            AssertTransactionResponse(response, TransactionStatus.Captured);
            
            var reverse = response.Reverse(amount)
                .Execute();
            AssertTransactionResponse(reverse, TransactionStatus.Reversed);
        }
        
        [TestMethod]
        public void CreditTrackData_RefundChip_Rejected() {
            var exceptionCaught = false;
            try {
                creditTrackData.Refund(amount)
                    .WithCurrency(currency)
                    .WithTagData(tagData)
                    .Execute();
            } catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("INVALID_REQUEST_DATA", ex.ResponseCode);
                Assert.AreEqual("40029", ex.ResponseMessage);
                Assert.AreEqual("Status Code: BadRequest - 34,Transaction rejected because the provided data was invalid. Online PINBlock Authentication not supported on offline transaction.", ex.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }
        
        [TestMethod]
        public void CreditTrackDataSale_WithoutPermissions() {
            var permissions = new[] { "TRN_POST_Capture" };
            const string withoutPermissions = "GpApiConfig_WithoutPermissions";

            ServicesContainer.ConfigureService(new GpApiConfig {
                Environment = Environment.TEST,
                AppId = AppId,
                AppKey = AppKey,
                Permissions = permissions,
                SecondsToExpire = 60
            }, withoutPermissions);

            var exceptionCaught = false;
            try {
                creditTrackData.Charge(amount)
                    .WithCurrency(currency)
                    .Execute(withoutPermissions);
            } catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("ACTION_NOT_AUTHORIZED", ex.ResponseCode);
                Assert.AreEqual("40212", ex.ResponseMessage);
                Assert.AreEqual("Status Code: Forbidden - Permission not enabled to execute action", ex.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }
        
        #endregion

        #region Create sale using Credit Card Data

        [TestMethod]
        public void CreditCard_SaleManual() {
            var response = card.Charge(amount)
                .WithCurrency(currency)
                .Execute();
            AssertTransactionResponse(response, TransactionStatus.Captured);
        }
        
        [TestMethod]
        public void CreditCard_SaleManual_AuthorizeThenCapture() {
            var response = card.Authorize(amount)
                .WithCurrency(currency)
                .Execute();
            AssertTransactionResponse(response, TransactionStatus.Preauthorized);
            
            var capture = response.Capture(amount)
                .WithCurrency(currency)
                .Execute();
            AssertTransactionResponse(capture, TransactionStatus.Captured);
        }
        
        [TestMethod]
        public void CreditCard_RefundManual() {
            var response = card.Refund(amount)
                .WithCurrency(currency)
                .Execute();
            AssertTransactionResponse(response, TransactionStatus.Captured);
        }
        
        [TestMethod]
        public void CreditCard_SaleManual_ThenRefund() {
            var response = card.Charge(amount)
                .WithCurrency(currency)
                .Execute();
            AssertTransactionResponse(response, TransactionStatus.Captured);
            
            var refund = response.Refund(amount)
                .WithCurrency(currency)
                .Execute();
            AssertTransactionResponse(refund, TransactionStatus.Captured);
        }
        
        [TestMethod]
        public void CreditCard_SaleManual_ThenReverse() {
            var response = card.Charge(amount)
                .WithCurrency(currency)
                .Execute();
            AssertTransactionResponse(response, TransactionStatus.Captured);
            
            var refund = response.Reverse(amount)
                .WithCurrency(currency)
                .Execute();
            AssertTransactionResponse(refund, TransactionStatus.Reversed);
        }

        #endregion

        #region Verify

        [TestMethod]
        public void CreditVerify_CreditTrackDataDetails() {
            var response = creditTrackData.Verify()
                .WithCurrency(currency)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual(Success, response?.ResponseCode);
            Assert.AreEqual(Verified, response?.ResponseMessage);
        }
        
        [TestMethod]
        public void CreditVerify_CardNumberDetails() {
            var response = card.Verify()
                .WithCurrency(currency)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual(Success, response?.ResponseCode);
            Assert.AreEqual(Verified, response?.ResponseMessage);
        }
        
        [TestMethod]
        public void CreditVerify_CardNumber_ExpiredCard() {
            card.Number = "4000120000001154";
            card.ExpYear = DateTime.Now.Year - 1;
            
            var response = card.Verify()
                .WithCurrency(currency)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("NOT_VERIFIED", response?.ResponseCode);
            Assert.AreEqual("NOT_VERIFIED", response?.ResponseMessage);
        }

        #endregion

        #region Reauthorize

        [TestMethod]
        public void CreditCardReauthorizeTransaction() {
            var chargeTransaction = card.Charge(amount)
                .WithCurrency(currency)
                .Execute();
            AssertTransactionResponse(chargeTransaction, TransactionStatus.Captured);

            var reverseTransaction = chargeTransaction.Reverse(amount)
                .Execute();
            AssertTransactionResponse(reverseTransaction, TransactionStatus.Reversed);

            var reauthTransaction = reverseTransaction.Reauthorize()
                .Execute();
            AssertTransactionResponse(reauthTransaction, TransactionStatus.Captured);
            Assert.AreEqual("00", reauthTransaction.AuthorizationCode);
        }

        [TestMethod]
        public void CreditCardReauthorizeAReversedAuthorizedTransaction() {
            var authTransaction = card.Authorize(amount)
                .WithCurrency(currency)
                .Execute();
            AssertTransactionResponse(authTransaction, TransactionStatus.Preauthorized);

            var reverseTransaction = authTransaction.Reverse(amount)
                .Execute();
            AssertTransactionResponse(reverseTransaction, TransactionStatus.Reversed);

            var reauthTransaction = reverseTransaction.Reauthorize()
                .Execute();
            AssertTransactionResponse(reauthTransaction, TransactionStatus.Preauthorized);
            Assert.AreEqual("00", reauthTransaction.AuthorizationCode);
        }

        [TestMethod]
        public void CreditCardReauthorizeTransaction_OldExistentSale() {
            DateTime startDate = DateTime.UtcNow.AddDays(-30);
            DateTime endDate = DateTime.UtcNow;

            var response = ReportingService.FindTransactionsPaged(1, 1000)
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

            var reverseTransaction = transaction.Reverse()
                .Execute();
            AssertTransactionResponse(reverseTransaction, TransactionStatus.Reversed);

            var reauthTransaction = transaction.Reauthorize()
                .Execute();
            AssertTransactionResponse(reauthTransaction, TransactionStatus.Captured);
            Assert.AreEqual("00", reauthTransaction.AuthorizationCode);
        }

        [TestMethod]
        public void CreditCardReauthorizeTransaction_WithIdempotencyKey() {
            var chargeTransaction = card.Charge(amount)
                .WithCurrency(currency)
                .Execute();
            AssertTransactionResponse(chargeTransaction, TransactionStatus.Captured);

            var reverseTransaction = chargeTransaction.Reverse(amount)
                .Execute();
            AssertTransactionResponse(reverseTransaction, TransactionStatus.Reversed);

            var idempotencyKey = Guid.NewGuid().ToString();
            var reauthTransaction = reverseTransaction.Reauthorize()
                .WithIdempotencyKey(idempotencyKey)
                .Execute();
            AssertTransactionResponse(reauthTransaction, TransactionStatus.Captured);
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
                    $"Status Code: Conflict - Idempotency Key seen before: id={reauthTransaction.TransactionId}",
                    ex.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void CreditCardReauthorizeTransaction_Refund() {
            var refundTransaction = card.Refund(amount)
                .WithCurrency(currency)
                .Execute();
            AssertTransactionResponse(refundTransaction, TransactionStatus.Captured);

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
            } finally {
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
            var transaction = card.Charge(amount)
                .WithCurrency(currency)
                .Execute();
            AssertTransactionResponse(transaction, TransactionStatus.Captured);

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
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        #endregion

        [TestMethod]
        public void IncrementalAuth() {

            var transaction = card.Authorize(amount)
                .WithCurrency(currency)
                .Execute();

            Assert.IsNotNull(transaction);
            Assert.AreEqual("SUCCESS", transaction.ResponseCode);
            Assert.AreEqual(TransactionStatus.Preauthorized.ToString().ToUpper(), transaction.ResponseMessage.ToUpper());

            var lodgingInfo = new LodgingData();
                lodgingInfo.bookingReference = "s9RpaDwXq1sPRkbP";
                lodgingInfo.StayDuration = 10;
                lodgingInfo.CheckInDate = DateTime.Now;
                lodgingInfo.CheckOutDate = DateTime.Now.AddDays(7);
                lodgingInfo.Rate = (decimal)13.49;
            var item1 = new LodgingItems();
            item1.Types = LodgingItemType.NO_SHOW.ToString();
            item1.Reference = "item_1";
            item1.TotalAmount = "13.49";
            item1.paymentMethodProgramCodes = new string[1] { PaymentMethodProgram.ASSURED_RESERVATION.ToString() };
            lodgingInfo.Items = new List<LodgingItems>() { item1 };

            transaction = transaction.AdditionalAuth(10)
                .WithCurrency(currency)
                .WithLodgingData(lodgingInfo)
                .Execute();

            Assert.IsNotNull(transaction);
            //echo '---'. $transaction->authorizationCode. '--->'. $transaction->cardBrandTransactionId;
            Assert.AreEqual("SUCCESS", transaction.ResponseCode);
            Assert.AreEqual(TransactionStatus.Preauthorized.ToString().ToUpper(), transaction.ResponseMessage.ToUpper());
            Assert.AreEqual((decimal)12.12, transaction.AuthorizedAmount);

            var capture = transaction.Capture()
                .Execute();

            Assert.IsNotNull(capture);
            Assert.AreEqual("SUCCESS", capture.ResponseCode);
            Assert.AreEqual(TransactionStatus.Captured.ToString().ToUpper(), capture.ResponseMessage.ToUpper());
        }
        
        [TestMethod]
        public void IncrementalAuth_WithoutLodgingData() {
            var transaction = card.Authorize(amount)
                .WithCurrency(currency)
                .Execute();

            Assert.IsNotNull(transaction);
            Assert.AreEqual("SUCCESS", transaction.ResponseCode);
            Assert.AreEqual(TransactionStatus.Preauthorized.ToString().ToUpper(), transaction.ResponseMessage.ToUpper());

            transaction = transaction.AdditionalAuth(10)
                .WithCurrency(currency)
                // .WithLodgingData(lodgingInfo)
                .Execute();

            Assert.IsNotNull(transaction);
            Assert.AreEqual("SUCCESS", transaction.ResponseCode);
            Assert.AreEqual(TransactionStatus.Preauthorized.ToString().ToUpper(), transaction.ResponseMessage.ToUpper());
            Assert.AreEqual((decimal)12.12, transaction.AuthorizedAmount);

            var capture = transaction.Capture()
                .Execute();

            Assert.IsNotNull(capture);
            Assert.AreEqual("SUCCESS", capture.ResponseCode);
            Assert.AreEqual(TransactionStatus.Captured.ToString().ToUpper(), capture.ResponseMessage.ToUpper());
        }
        
        [TestMethod]
        public void IncrementalAuth_Reverse() {
            var transaction = card.Authorize(amount)
                .WithCurrency(currency)
                .Execute();

            Assert.IsNotNull(transaction);
            Assert.AreEqual("SUCCESS", transaction.ResponseCode);
            Assert.AreEqual(TransactionStatus.Preauthorized.ToString().ToUpper(), transaction.ResponseMessage.ToUpper());

            var lodgingInfo = new LodgingData {
                bookingReference = "s9RpaDwXq1sPRkbP",
                StayDuration = 10,
                CheckInDate = DateTime.Now,
                CheckOutDate = DateTime.Now.AddDays(7),
                Rate = (decimal)13.49
            };
            
            var item1 = new LodgingItems {
                Types = LodgingItemType.NO_SHOW.ToString(),
                Reference = "item_1",
                TotalAmount = "13.49",
                paymentMethodProgramCodes = new string[1] { PaymentMethodProgram.ASSURED_RESERVATION.ToString() }
            };
            
            lodgingInfo.Items = new List<LodgingItems>() { item1 };

            transaction = transaction.AdditionalAuth(10)
                .WithCurrency(currency)
                .WithLodgingData(lodgingInfo)
                .Execute();

            Assert.IsNotNull(transaction);
            Assert.AreEqual("SUCCESS", transaction.ResponseCode);
            Assert.AreEqual(TransactionStatus.Preauthorized.ToString().ToUpper(), transaction.ResponseMessage.ToUpper());
            Assert.AreEqual((decimal)12.12, transaction.AuthorizedAmount);

            var capture = transaction.Reverse()
                .Execute();

            Assert.IsNotNull(capture);
            Assert.AreEqual("SUCCESS", capture.ResponseCode);
            Assert.AreEqual(TransactionStatus.Reversed.ToString().ToUpper(), capture.ResponseMessage.ToUpper());
        }
        
        [TestMethod]
        public void IncrementalAuth_Charge() {
            var transaction = card.Charge(amount)
                .WithCurrency(currency)
                .Execute();

            Assert.IsNotNull(transaction);
            Assert.AreEqual("SUCCESS", transaction.ResponseCode);
            Assert.AreEqual(TransactionStatus.Captured.ToString().ToUpper(), transaction.ResponseMessage.ToUpper());

            var lodgingInfo = new LodgingData {
                bookingReference = "s9RpaDwXq1sPRkbP",
                StayDuration = 10,
                CheckInDate = DateTime.Now,
                CheckOutDate = DateTime.Now.AddDays(7),
                Rate = (decimal)13.49
            };
            
            var item1 = new LodgingItems {
                Types = LodgingItemType.NO_SHOW.ToString(),
                Reference = "item_1",
                TotalAmount = "13.49",
                paymentMethodProgramCodes = new string[1] { PaymentMethodProgram.ASSURED_RESERVATION.ToString() }
            };
            
            lodgingInfo.Items = new List<LodgingItems>() { item1 };

            var exceptionCaught = false;
            try {
                transaction.AdditionalAuth(10)
                    .WithCurrency(currency)
                    .WithLodgingData(lodgingInfo)
                    .Execute();
            } catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("INVALID_ACTION", ex.ResponseCode);
                Assert.AreEqual("40290", ex.ResponseMessage);
                Assert.AreEqual("Status Code: BadRequest - Cannot PROCESS Incremental Authorization over a transaction that does not have a status of PREAUTHORIZED.", ex.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void IncrementalAuth_RandomTransactionId() {
            var randomTransactionId = Guid.NewGuid().ToString();
            var transaction = new Transaction {
                TransactionId = randomTransactionId
            };

            var lodgingInfo = new LodgingData {
                bookingReference = "s9RpaDwXq1sPRkbP",
                StayDuration = 10,
                CheckInDate = DateTime.Now,
                CheckOutDate = DateTime.Now.AddDays(7),
                Rate = (decimal)13.49
            };
            var item1 = new LodgingItems {
                Types = LodgingItemType.NO_SHOW.ToString(),
                Reference = "item_1",
                TotalAmount = "13.49",
                paymentMethodProgramCodes = new string[1] { PaymentMethodProgram.ASSURED_RESERVATION.ToString() }
            };
            
            lodgingInfo.Items = new List<LodgingItems>() { item1 };

            var exceptionCaught = false;
            try {
                transaction.AdditionalAuth(10)
                    .WithCurrency(currency)
                    .WithLodgingData(lodgingInfo)
                    .Execute();
            } catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("RESOURCE_NOT_FOUND", ex.ResponseCode);
                Assert.AreEqual("40008", ex.ResponseMessage);
                Assert.AreEqual($"Status Code: NotFound - Transaction {randomTransactionId} not found at this location.", ex.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }
        
        private void AssertTransactionResponse(Transaction transaction, TransactionStatus transactionStatus) {
            Assert.IsNotNull(transaction);
            Assert.AreEqual(Success, transaction?.ResponseCode);
            Assert.AreEqual(GetMapping(transactionStatus), transaction?.ResponseMessage);
        }
    }
}