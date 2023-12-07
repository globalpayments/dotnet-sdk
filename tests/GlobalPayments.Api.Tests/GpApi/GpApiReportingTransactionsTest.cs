using System;
using System.Collections.Generic;
using System.Linq;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.GpApi {
    [TestClass]
    public class GpApiReportingTransactionsTest : BaseGpApiReportingTest {
        
        [ClassInitialize]
        public static void ClassInitialize(TestContext context) {
            var gpApiConfig = GpApiConfigSetup(AppId, AppKey, Channel.CardNotPresent);
            ServicesContainer.ConfigureService(gpApiConfig);
        }

        #region Transactions
        [TestMethod]
        public void ReportTransactionDetail() {
            var transactionId = ReportingService.FindTransactionsPaged(FirstPage, PageSize)
                .Execute().Results.Select(t => t.TransactionId).FirstOrDefault();
            
            var response = ReportingService.TransactionDetail(transactionId)
                .Execute();
            Assert.IsNotNull(response);
            Assert.IsTrue(response is TransactionSummary);
            Assert.AreEqual(transactionId, response.TransactionId);
        }

        [TestMethod]
        public void ReportTransactionDetailWrongId() {
            const string transactionId = "TRN_123456";
            var exceptionCaught = false;
            try {
                ReportingService.TransactionDetail(transactionId)
                .Execute();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("RESOURCE_NOT_FOUND", ex.ResponseCode);
                Assert.AreEqual("40118", ex.ResponseMessage);
                Assert.AreEqual("Status Code: NotFound - Transactions TRN_123456 not found at this /ucp/transactions/TRN_123456", ex.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void ReportFindTransactionsPaged_By_StartDate_And_EndDate() {
            var result = ReportingService.FindTransactionsPaged(FirstPage,PageSize)
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .Where(SearchCriteria.StartDate, ReportingStartDate)
                .And(SearchCriteria.EndDate, ReportingEndDate)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.TrueForAll(t => t.TransactionDate?.Date >= ReportingStartDate.Date && t.TransactionDate?.Date <= ReportingEndDate.Date));
        }

        [TestMethod]
        public void ReportFindTransactionsPaged_By_Id() {
            var transactionId = ReportingService.FindTransactionsPaged(FirstPage,PageSize)
                .Execute().Results.Select(t => t.TransactionId).FirstOrDefault();

            Assert.IsNotNull(transactionId);
            
            var result = ReportingService.FindTransactionsPaged(FirstPage,PageSize)
                .WithTransactionId(transactionId)
                .Execute();
            
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.Count == 1);
            Assert.IsTrue(result.Results.TrueForAll(t => t.TransactionId == transactionId));
        }

        [TestMethod]
        public void ReportFindTransactionsPaged_WrongId() {
            const string transactionId = "TRN_B2RDfsrhwhzvsbkci4JdTiZ9mHVmvC";
            var result = ReportingService.FindTransactionsPaged(FirstPage,PageSize)
                .WithTransactionId(transactionId)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.Count == 0);
        }

        [TestMethod]
        public void ReportFindTransactionsPaged_By_BatchId() {
            const string batchId = "BAT_845591";
            var result = ReportingService.FindTransactionsPaged(FirstPage,PageSize)
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .Where(SearchCriteria.BatchId, batchId)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.TrueForAll(t => t.BatchSequenceNumber == batchId));
        }

        [TestMethod]
        public void ReportFindTransactionsPaged_By_Type() {
            const PaymentType paymentType = PaymentType.Sale;
            var result = ReportingService.FindTransactionsPaged(FirstPage,PageSize)
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .Where(SearchCriteria.PaymentType, paymentType)
                .Execute();
            Assert.IsNotNull(result.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.TrueForAll(t => t.TransactionType == EnumConverter.GetMapping(Target.GP_API, paymentType)));
        }

        [TestMethod]
        public void ReportFindTransactionsPaged_By_Amount_And_Currency_And_Country() {
            const decimal amount = 1.12M;
            const string currency = "aud"; //This is case sensitive
            const string country = "AU"; //This is case sensitive
            var result = ReportingService.FindTransactionsPaged(FirstPage,PageSize)
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .Where(DataServiceCriteria.Amount, amount)
                .And(DataServiceCriteria.Currency, currency)
                .And(DataServiceCriteria.Country, country)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.TrueForAll(t => t.Amount == amount && t.Currency == currency && t.Country == country));
        }

        [TestMethod]
        public void ReportFindTransactionsPaged_By_Channel() {
            const Channel channel = Channel.CardNotPresent;
            var result = ReportingService.FindTransactionsPaged(FirstPage,PageSize)
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .Where(SearchCriteria.Channel, channel)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.TrueForAll(t => t.Channel == EnumConverter.GetMapping(Target.GP_API, channel)));
        }

        [TestMethod]
        public void ReportFindTransactionsPaged_By_Status() {
            const TransactionStatus transactionStatus = TransactionStatus.Captured;
            var result = ReportingService.FindTransactionsPaged(FirstPage,PageSize)
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .Where(SearchCriteria.TransactionStatus, transactionStatus)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.TrueForAll(t => t.TransactionStatus == EnumConverter.GetMapping(Target.GP_API, transactionStatus)));
        }

        [TestMethod]
        public void ReportFindTransactionsPaged_By_CardBrand_And_AuthCode() {
            const string cardBrand = "VISA";
            const string authCode = "12345";
            var result = ReportingService.FindTransactionsPaged(FirstPage,PageSize)
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .Where(SearchCriteria.CardBrand, cardBrand)
                .And(SearchCriteria.AuthCode, authCode)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.TrueForAll(t => t.CardType == cardBrand && t.AuthCode == authCode));
        }

        [TestMethod]
        public void ReportFindTransactionsPaged_By_Reference() {
            const string referenceNumber = "e1f2f968-e9cc-45b2-b41f-61cad13754aa";
            var result = ReportingService.FindTransactionsPaged(FirstPage,PageSize)
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .Where(SearchCriteria.ReferenceNumber, referenceNumber)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.TrueForAll(t => t.ReferenceNumber == referenceNumber));
        }

        [TestMethod]
        public void ReportFindTransactionsPaged_By_BrandReference() {
            const string brandReference = "D5v2Nv8h91Me3DTh";
            var result = ReportingService.FindTransactionsPaged(FirstPage,PageSize)
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .Where(SearchCriteria.BrandReference, brandReference)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.TrueForAll(t => t.BrandReference == brandReference));
        }

        [TestMethod]
        public void ReportFindTransactionsPaged_By_EntryMode() {
            const PaymentEntryMode paymentEntryMode = PaymentEntryMode.Ecom;
            var result = ReportingService.FindTransactionsPaged(FirstPage,PageSize)
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .Where(SearchCriteria.PaymentEntryMode, paymentEntryMode)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.TrueForAll(t => t.EntryMode == EnumConverter.GetMapping(Target.GP_API, paymentEntryMode)));
        }

        [TestMethod]
        public void ReportFindTransactionsPaged_By_Number_First6_And_Last4() {
            const string firstSix = "426397";
            const string lastFour = "5262";
            var result = ReportingService.FindTransactionsPaged(FirstPage,PageSize)
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .Where(SearchCriteria.CardNumberFirstSix, firstSix)
                .And(SearchCriteria.CardNumberLastFour, lastFour)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.TrueForAll(t => t.MaskedCardNumber.StartsWith(firstSix) && t.MaskedCardNumber.EndsWith(lastFour)));
        }

        [TestMethod]
        public void ReportFindTransactionsPaged_By_Token_First6_And_Last4() {
            const string firstSix = "426397";
            const string lastFour = "5262";
            var result = ReportingService.FindTransactionsPaged(FirstPage,PageSize)
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .Where(SearchCriteria.TokenFirstSix, firstSix)
                .And(SearchCriteria.TokenLastFour, lastFour)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.TrueForAll(t => t.MaskedCardNumber.EndsWith(lastFour)));
        }
        
        [TestMethod]
        public void ReportFindTransactionsPaged_By_Token_First6_And_Last4_And_PaymentMethod() {
            const string firstSix = "426397";
            const string lastFour = "5262";
            var result = ReportingService.FindTransactionsPaged(FirstPage,PageSize)
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .Where(SearchCriteria.TokenFirstSix, firstSix)
                .And(SearchCriteria.TokenLastFour, lastFour)
                .And(SearchCriteria.PaymentMethodName, PaymentMethodName.DigitalWallet)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.TrueForAll(t => t.MaskedCardNumber.EndsWith(lastFour)));
        }
        
        [TestMethod]
        public void ReportFindTransactionsPaged_By_Token_First6_And_Last4_And_WrongPaymentMethod() {
            const string firstSix = "426397";
            const string lastFour = "5262";
            var exceptionCaught = false;
            try {
                ReportingService.FindTransactionsPaged(FirstPage,PageSize)
                    .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                    .Where(SearchCriteria.TokenFirstSix, firstSix)
                    .And(SearchCriteria.TokenLastFour, lastFour)
                    .And(SearchCriteria.PaymentMethodName, PaymentMethodName.Card)
                    .Execute();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("INVALID_REQUEST_DATA", ex.ResponseCode);
                Assert.AreEqual("40043", ex.ResponseMessage);
                Assert.AreEqual("Status Code: BadRequest - Request contains unexpected fields: payment_method", ex.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void ReportFindTransactionsPaged_By_PaymentMethod() {
            foreach (PaymentMethodName paymentMethodName in Enum.GetValues(typeof(PaymentMethodName))) {
                var result = ReportingService.FindTransactionsPaged(FirstPage,PageSize)
                    .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                    .Where(SearchCriteria.PaymentMethodName, paymentMethodName)
                    .Execute();
                Assert.IsNotNull(result?.Results);
            }
        }

        [TestMethod]
        public void ReportFindTransactionsPaged_By_Name() {
            const string name = "James Mason";
            var result = ReportingService.FindTransactionsPaged(FirstPage,PageSize)
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Ascending)
                .Where(SearchCriteria.Name, name)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.TrueForAll(t => t.CardHolderName == name));
        }

        [TestMethod]
        public void ReportFindTransactionsPaged_By_StartDate_OrderBy_TimeCreated_Ascending() {
            var result = ReportingService.FindTransactionsPaged(FirstPage,PageSize)
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Ascending)
                .Where(SearchCriteria.StartDate, ReportingStartDate)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results.Count > 0);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.SequenceEqual(result.Results.OrderBy(t => t.TransactionDate)));
        }

        [TestMethod]
        public void ReportFindTransactionsPaged_By_StartDate_OrderBy_Id_Ascending() {
            var result = ReportingService.FindTransactionsPaged(FirstPage,PageSize)
                .OrderBy(TransactionSortProperty.Id, SortDirection.Ascending)
                .Where(SearchCriteria.StartDate, ReportingStartDate)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results.Count > 0);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.SequenceEqual(result.Results.OrderBy(t => t.TransactionId, StringComparer.Ordinal)));
        }

        [TestMethod]
        public void ReportFindTransactionsPaged_By_StartDate_OrderBy_Type_Ascending() {
            var result = ReportingService.FindTransactionsPaged(FirstPage,PageSize)
                .OrderBy(TransactionSortProperty.Type, SortDirection.Ascending)
                .Where(SearchCriteria.StartDate, ReportingStartDate)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results.Count > 0);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.SequenceEqual(result.Results.OrderBy(t => t.TransactionType, StringComparer.Ordinal)));
        }

        [TestMethod]
        public void ReportFindTransactionsPaged_OrderBy_TimeCreated_Ascending() {
            var result = ReportingService.FindTransactionsPaged(FirstPage,PageSize)
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Ascending)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.SequenceEqual(result.Results.OrderBy(t => t.TransactionDate)));
        }

        [TestMethod]
        public void ReportFindTransactionsPaged_OrderBy_TimeCreated_Descending() {
            var result = ReportingService.FindTransactionsPaged(FirstPage,PageSize)
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.SequenceEqual(result.Results.OrderByDescending(t => t.TransactionDate)));
        }

        [TestMethod]
        public void ReportFindTransactionsPaged_OrderBy_Id_Ascending() {
            var result = ReportingService.FindTransactionsPaged(FirstPage,PageSize)
                .OrderBy(TransactionSortProperty.Id, SortDirection.Ascending)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.SequenceEqual(result.Results.OrderBy(t => t.TransactionId, StringComparer.Ordinal)));
        }

        [TestMethod]
        public void ReportFindTransactionsPaged_OrderBy_Id_Descending() {
            var result = ReportingService.FindTransactionsPaged(FirstPage,PageSize)
                .OrderBy(TransactionSortProperty.Id, SortDirection.Descending)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.SequenceEqual(result.Results.OrderByDescending(t => t.TransactionId, StringComparer.Ordinal)));
        }

        [TestMethod]
        public void ReportFindTransactionsPaged_OrderBy_Type_Ascending() {
            var result = ReportingService.FindTransactionsPaged(FirstPage,PageSize)
                .OrderBy(TransactionSortProperty.Type, SortDirection.Ascending)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.SequenceEqual(result.Results.OrderBy(t => t.TransactionType, StringComparer.Ordinal)));
        }

        [TestMethod]
        public void ReportFindTransactionsPaged_OrderBy_Type_Descending() {
            var result = ReportingService.FindTransactionsPaged(FirstPage,PageSize)
                .OrderBy(TransactionSortProperty.Type, SortDirection.Descending)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.SequenceEqual(result.Results.OrderByDescending(t => t.TransactionType, StringComparer.Ordinal)));
        }

        [TestMethod]
        public void ReportFindTransactionsPaged_Without_Mandatory_StartDate() {
            var result = ReportingService.FindTransactionsPaged(FirstPage,PageSize)
                .OrderBy(TransactionSortProperty.Type, SortDirection.Ascending)
                .Execute();

            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results.Count > 0);
        }
        #endregion
        
        #region Settlement Transactions
        [TestMethod]
        public void ReportFindSettlementTransactionsPaged_By_StartDate_And_EndDate() {
            var result = ReportingService.FindSettlementTransactionsPaged(FirstPage, PageSize)
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .Where(SearchCriteria.StartDate, ReportingStartDate)
                .And(SearchCriteria.EndDate, ReportingEndDate)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.TrueForAll(t => t.TransactionDate?.Date >= ReportingStartDate.Date && t.TransactionDate?.Date <= ReportingEndDate.Date));
        }

        [TestMethod]
        public void ReportFindSettlementTransactionsPaged_By_StartDate_OrderBy_TimeCreated_Ascending() {
            var result = ReportingService.FindSettlementTransactionsPaged(FirstPage, PageSize)
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Ascending)
                .Where(SearchCriteria.StartDate, ReportingStartDate)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.SequenceEqual(result.Results.OrderBy(t => t.TransactionDate)));
        }

        [TestMethod]
        public void ReportFindSettlementTransactionsPaged_By_StartDate_OrderBy_TimeCreated_Descending() {
            var result = ReportingService.FindSettlementTransactionsPaged(FirstPage, PageSize)
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .Where(SearchCriteria.StartDate, ReportingStartDate)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.SequenceEqual(result.Results.OrderByDescending(t => t.TransactionDate)));
        }

        [TestMethod]
        public void ReportFindSettlementTransactionsPaged_By_StartDate_OrderBy_Status_Ascending() {
            var result = ReportingService.FindSettlementTransactionsPaged(FirstPage, PageSize)
                .OrderBy(TransactionSortProperty.Status, SortDirection.Ascending)
                .Where(SearchCriteria.StartDate, ReportingStartDate)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.SequenceEqual(result.Results.OrderBy(t => t.TransactionStatus)));
        }

        [TestMethod]
        public void ReportFindSettlementTransactionsPaged_By_StartDate_OrderBy_Status_Descending() {
            var result = ReportingService.FindSettlementTransactionsPaged(FirstPage, PageSize)
                .OrderBy(TransactionSortProperty.Status, SortDirection.Descending)
                .Where(SearchCriteria.StartDate, ReportingStartDate)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.SequenceEqual(result.Results.OrderByDescending(t => t.TransactionStatus)));
        }

        [TestMethod]
        public void ReportFindSettlementTransactionsPaged_By_StartDate_OrderBy_Type_Ascending() {
            var result = ReportingService.FindSettlementTransactionsPaged(FirstPage, PageSize)
                .OrderBy(TransactionSortProperty.Type, SortDirection.Ascending)
                .Where(SearchCriteria.StartDate, ReportingStartDate)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.SequenceEqual(result.Results.OrderBy(t => t.TransactionType)));
        }

        [TestMethod]
        public void ReportFindSettlementTransactionsPaged_By_StartDate_OrderBy_Type_Descending() {
            var result = ReportingService.FindSettlementTransactionsPaged(FirstPage, PageSize)
                .OrderBy(TransactionSortProperty.Type, SortDirection.Descending)
                .Where(SearchCriteria.StartDate, ReportingStartDate)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.SequenceEqual(result.Results.OrderByDescending(t => t.TransactionType)));
        }

        [TestMethod]
        public void ReportFindSettlementTransactionsPaged_By_StartDate_OrderBy_DepositId_Ascending() {
            var result = ReportingService.FindSettlementTransactionsPaged(FirstPage, PageSize)
                .OrderBy(TransactionSortProperty.DepositId, SortDirection.Ascending)
                .Where(SearchCriteria.StartDate, ReportingStartDate)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.SequenceEqual(result.Results.OrderBy(t => t.DepositReference)));
        }

        [TestMethod]
        public void ReportFindSettlementTransactionsPaged_By_StartDate_OrderBy_DepositId_Descending() {
            var result = ReportingService.FindSettlementTransactionsPaged(FirstPage, PageSize)
                .OrderBy(TransactionSortProperty.DepositId, SortDirection.Descending)
                .Where(SearchCriteria.StartDate, ReportingStartDate)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.SequenceEqual(result.Results.OrderByDescending(t => t.DepositReference)));
        }

        [TestMethod]
        public void ReportFindSettlementTransactionsPaged_By_Number_First6_And_Number_Last4() {
            const string firstSix = "543458";
            const string lastFour = "7652";
            
            var result = ReportingService.FindSettlementTransactionsPaged(FirstPage, PageSize)
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .Where(SearchCriteria.StartDate, ReportingStartDate)
                .And(SearchCriteria.CardNumberFirstSix, firstSix)
                .And(SearchCriteria.CardNumberLastFour, lastFour)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.TrueForAll(t => t.MaskedCardNumber.StartsWith(firstSix) && t.MaskedCardNumber.EndsWith(lastFour)));
        }

        [TestMethod]
        public void ReportFindSettlementTransactionsPaged_By_CardBrand() {
            const string cardBrand = "MASTERCARD";
            
            var result = ReportingService.FindSettlementTransactionsPaged(FirstPage, PageSize)
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .Where(SearchCriteria.StartDate, ReportingStartDate)
                .And(SearchCriteria.CardBrand, cardBrand)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.TrueForAll(t => t.CardType == cardBrand));
        }

        [TestMethod]
        public void ReportFindSettlementTransactionsPaged_By_InvalidCardBrand() {
            const string cardBrand = "MIT";

            var result = ReportingService.FindSettlementTransactionsPaged(FirstPage, PageSize)
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .Where(SearchCriteria.StartDate, ReportingStartDate)
                .And(SearchCriteria.CardBrand, cardBrand)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.Count == 0);
        }

        [TestMethod]
        public void ReportFindSettlementTransactionsPaged_By_DepositStatus()
        {
            foreach (DepositStatus depositStatus in Enum.GetValues(typeof(DepositStatus))) {
                var result = ReportingService.FindSettlementTransactionsPaged(FirstPage, PageSize)
                    .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                    .Where(SearchCriteria.StartDate, ReportingStartDate)
                    .And(SearchCriteria.DepositStatus, depositStatus)
                    .Execute();
                Assert.IsNotNull(result?.Results);
                Assert.IsTrue(result.Results is List<TransactionSummary>);
            }
        }

        [TestMethod]
        public void ReportFindSettlementTransactionsPaged_By_ARN() {
            const string arn = "74500010037624410827759";

            var result = ReportingService.FindSettlementTransactionsPaged(FirstPage, PageSize)
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .Where(SearchCriteria.StartDate, ReportingStartDate)
                .And(SearchCriteria.AquirerReferenceNumber, arn)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.TrueForAll(t => t.AquirerReferenceNumber == arn));
        }

        [TestMethod]
        public void ReportFindSettlementTransactionsPaged_By_WrongARN() {
            const string arn = "00000010037624410827527";

            var result = ReportingService.FindSettlementTransactionsPaged(FirstPage, PageSize)
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .Where(SearchCriteria.StartDate, ReportingStartDate)
                .And(SearchCriteria.AquirerReferenceNumber, arn)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.Count == 0);
        }

        [TestMethod]
        public void ReportFindSettlementTransactionsPaged_By_BrandReference() {
            const string brandReference = "MCF1CZ5ME5405";

            var result = ReportingService.FindSettlementTransactionsPaged(FirstPage, PageSize)
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .Where(SearchCriteria.StartDate, ReportingStartDate)
                .And(SearchCriteria.BrandReference, brandReference)
                .Execute();
            Assert.IsNotNull(result.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.TrueForAll(t => t.BrandReference == brandReference));
        }

        [TestMethod]
        public void ReportFindSettlementTransactionsPaged_By_WrongBrandReference() {
            const string brandReference = "MCF1CZ5ME5001";

            var result = ReportingService.FindSettlementTransactionsPaged(FirstPage, PageSize)
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .Where(SearchCriteria.StartDate, ReportingStartDate)
                .And(SearchCriteria.BrandReference, brandReference)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.Count == 0);
        }

        [TestMethod]
        public void ReportFindSettlementTransactionsPaged_By_CardBrand_And_AuthCode() {
            const string cardBrand = "MASTERCARD";
            const string authCode = "028010";

            var result = ReportingService.FindSettlementTransactionsPaged(FirstPage, PageSize)
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .Where(SearchCriteria.StartDate, ReportingStartDate)
                .And(SearchCriteria.CardBrand, cardBrand)
                .And(SearchCriteria.AuthCode, authCode)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.TrueForAll(t => t.CardType == cardBrand));
            Assert.IsTrue(result.Results.TrueForAll(t => t.AuthCode == authCode));
        }

        [TestMethod]
        public void ReportFindSettlementTransactionsPaged_By_Reference() {
            const string reference = "50080513769";

            var result = ReportingService.FindSettlementTransactionsPaged(FirstPage, PageSize)
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .Where(SearchCriteria.StartDate, ReportingStartDate)
                .And(SearchCriteria.ReferenceNumber, reference)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.TrueForAll(t => t.ReferenceNumber == reference));
        }

        [TestMethod]
        public void ReportFindSettlementTransactionsPaged_By_RandomReference() {
            var result = ReportingService.FindSettlementTransactionsPaged(FirstPage, PageSize)
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .Where(SearchCriteria.StartDate, ReportingStartDate)
                .And(SearchCriteria.ReferenceNumber, Guid.NewGuid().ToString())
                .Execute();

            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.Count == 0);
        }

        [TestMethod]
        public void ReportFindSettlementTransactionsPaged_By_Status() {
            foreach (TransactionStatus transactionStatus in Enum.GetValues(typeof(TransactionStatus))) {
                var result = ReportingService.FindSettlementTransactionsPaged(FirstPage, PageSize)
                    .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                    .Where(SearchCriteria.StartDate, ReportingStartDate)
                    .And(SearchCriteria.TransactionStatus, transactionStatus)
                    .Execute();
                Assert.IsNotNull(result?.Results);
                Assert.IsTrue(result.Results is List<TransactionSummary>);
                Assert.IsTrue(result.Results.TrueForAll(t =>
                    t.TransactionStatus == EnumConverter.GetMapping(Target.GP_API, transactionStatus)));
            }
        }

        [TestMethod]
        public void ReportFindSettlementTransactionsPaged_By_DepositReference() {
            const string depositReference = "DEP_2342423429";

            var result = ReportingService.FindSettlementTransactionsPaged(FirstPage, PageSize)
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .Where(SearchCriteria.StartDate, ReportingStartDate)
                .And(DataServiceCriteria.DepositReference, depositReference)
                .Execute();
            Assert.IsNotNull(result.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.TrueForAll(t => t.DepositReference == depositReference));
        }

        [TestMethod]
        public void ReportFindSettlementTransactionsPaged_By_RandomDepositReference() {
            var result = ReportingService.FindSettlementTransactionsPaged(FirstPage, PageSize)
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .Where(SearchCriteria.StartDate, ReportingStartDate)
                .And(DataServiceCriteria.DepositReference, Guid.NewGuid().ToString())
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.Count == 0);
        }

        [TestMethod]
        public void ReportFindSettlementTransactionsPaged_By_FromDepositTimeCreated_And_ToDepositTimeCreated() {
            var result = ReportingService.FindSettlementTransactionsPaged(FirstPage, PageSize)
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .Where(SearchCriteria.StartDate, ReportingStartDate)
                .And(DataServiceCriteria.StartDepositDate, ReportingStartDate)
                .And(DataServiceCriteria.EndDepositDate, ReportingLastMonthDate)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.TrueForAll(t => t.DepositDate?.Date >= ReportingStartDate.Date && t.DepositDate?.Date <= ReportingLastMonthDate.Date));
        }

        [TestMethod]
        public void ReportFindSettlementTransactionsPaged_By_FromBatchTimeCreated_And_ToBatchTimeCreated() {
            var result = ReportingService.FindSettlementTransactionsPaged(FirstPage, PageSize)
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .Where(SearchCriteria.StartDate, ReportingStartDate)
                .And(DataServiceCriteria.StartBatchDate, ReportingStartDate)
                .And(DataServiceCriteria.EndBatchDate, ReportingLastMonthDate)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.TrueForAll(t => t.BatchCloseDate?.Date >= ReportingStartDate.Date && t.BatchCloseDate?.Date <= ReportingLastMonthDate.Date));
        }

        [TestMethod]
        public void ReportFindSettlementTransactionsPaged_By_MerchantId_And_SystemHierarchy() {
            const string merchantId = "101023947262";
            const string systemHierarchy = "055-70-024-011-019";

            var result = ReportingService.FindSettlementTransactionsPaged(FirstPage, PageSize)
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .Where(SearchCriteria.StartDate, ReportingStartDate)
                .And(DataServiceCriteria.MerchantId, merchantId)
                .And(DataServiceCriteria.SystemHierarchy, systemHierarchy)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.TrueForAll(t => t.MerchantId == merchantId && t.MerchantHierarchy == systemHierarchy));
        }

        [TestMethod]
        public void ReportFindSettlementTransactionsPaged_By_NonExistent_MerchantId() {
            const string merchantId = "100023947222";
            
            var result = ReportingService.FindSettlementTransactionsPaged(FirstPage, PageSize)
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .Where(SearchCriteria.StartDate, ReportingStartDate)
                .And(DataServiceCriteria.MerchantId, merchantId)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.Count == 0);
        }

        [TestMethod]
        public void ReportFindSettlementTransactionsPaged_By_Random_SystemHierarchy() {
            var result = ReportingService.FindSettlementTransactionsPaged(FirstPage, PageSize)
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .Where(SearchCriteria.StartDate, ReportingStartDate)
                .And(DataServiceCriteria.SystemHierarchy, Guid.NewGuid().ToString())
                .Execute();

            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.Count == 0);
        }

        [TestMethod]
        public void ReportFindSettlementTransactionsPaged_By_Invalid_MerchantId() {
            var exceptionCaught = false;
            
            try {
                ReportingService.FindSettlementTransactionsPaged(FirstPage, PageSize)
                    .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                    .Where(SearchCriteria.StartDate, ReportingStartDate)
                    .And(DataServiceCriteria.MerchantId, Guid.NewGuid().ToString())
                    .Execute();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("INVALID_REQUEST_DATA", ex.ResponseCode);
                Assert.AreEqual("40090", ex.ResponseMessage);
                Assert.IsTrue(ex.Message.Equals("Status Code: BadRequest - " +
                    "system.mid value is invalid. Please check the format and data provided is correct"));
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        #endregion
    }
    
}