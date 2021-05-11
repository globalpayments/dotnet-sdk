using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GlobalPayments.Api.Tests.GpApi {
    [TestClass]
    public class GpApiReportingTests {
        [ClassInitialize]
        public static void ClassInitialize(TestContext context) {
            ServicesContainer.ConfigureService(new GpApiConfig {
                AppId = "JF2GQpeCrOivkBGsTRiqkpkdKp67Gxi0",
                AppKey = "y7vALnUtFulORlTV",
            });
        }

        #region Transactions
        [TestMethod]
        public void ReportTransactionDetail() {
            string transactionId = "TRN_TvY1QFXxQKtaFSjNaLnDVdo3PZ7ivz";
            TransactionSummary response = ReportingService.TransactionDetail(transactionId)
                .Execute();
            Assert.IsNotNull(response);
            Assert.IsTrue(response is TransactionSummary);
            Assert.AreEqual(transactionId, response.TransactionId);
        }

        [TestMethod]
        public void ReportTransactionDetailWrongID() {
            string transactionId = "TRN_123456";
            try {
                TransactionSummary response = ReportingService.TransactionDetail(transactionId)
                .Execute();
            }
            catch (GatewayException ex) {
                Assert.AreEqual("RESOURCE_NOT_FOUND", ex.ResponseCode);
                Assert.AreEqual("40118", ex.ResponseMessage);
                Assert.AreEqual("Status Code: NotFound - Transactions TRN_123456 not found at this /ucp/transactions/TRN_123456", ex.Message);
            }
        }

        [TestMethod]
        public void ReportFindTransactionsPaged_By_StartDate_And_EndDate() {
            DateTime startDate = DateTime.UtcNow.AddDays(-30);
            DateTime endDate = DateTime.UtcNow.AddDays(-10);
            PagedResult<TransactionSummary> result = ReportingService.FindTransactionsPaged(1, 10)
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .Where(SearchCriteria.StartDate, startDate)
                .And(SearchCriteria.EndDate, endDate)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.TrueForAll(t => t.TransactionDate?.Date >= startDate.Date && t.TransactionDate?.Date <= endDate.Date));
        }

        [TestMethod]
        public void ReportFindTransactionsPaged_By_Id() {
            string transactionId = ReportingService.FindTransactionsPaged(1, 1)
                .Execute().Results.Select(t => t.TransactionId).FirstOrDefault();

            Assert.IsNotNull(transactionId);
            
            PagedResult<TransactionSummary> result = ReportingService.FindTransactionsPaged(1, 10)
                .WithTransactionId(transactionId)
                .Execute();
            
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.Count == 1);
            Assert.IsTrue(result.Results.TrueForAll(t => t.TransactionId == transactionId));
        }

        [TestMethod]
        public void ReportFindTransactionsPaged_WrongId() {
            string transactionId = "TRN_B2RDfsrhwhzvsbkci4JdTiZ9mHVmvC";
            PagedResult<TransactionSummary> result = ReportingService.FindTransactionsPaged(1, 10)
                .WithTransactionId(transactionId)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.Count == 0);
        }

        [TestMethod]
        public void ReportFindTransactionsPaged_By_BatchId() {
            string batchId = "BAT_845591";
            PagedResult<TransactionSummary> result = ReportingService.FindTransactionsPaged(1, 10)
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .Where(SearchCriteria.BatchId, batchId)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.TrueForAll(t => t.BatchSequenceNumber == batchId));
        }

        [TestMethod]
        public void ReportFindTransactionsPaged_By_Type() {
            var paymentType = PaymentType.Sale;
            PagedResult<TransactionSummary> result = ReportingService.FindTransactionsPaged(1, 10)
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .Where(SearchCriteria.PaymentType, paymentType)
                .Execute();
            Assert.IsNotNull(result.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.TrueForAll(t => t.TransactionType == EnumConverter.GetMapping(Target.GP_API, paymentType)));
        }

        [TestMethod]
        public void ReportFindTransactionsPaged_By_Amount_And_Currency_And_Country() {
            decimal amount = 1.12M;
            string currency = "aud"; //This is case sensitive
            string country = "AU"; //This is case sensitive
            PagedResult<TransactionSummary> result = ReportingService.FindTransactionsPaged(1, 10)
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
            var channel = Channel.CardNotPresent;
            PagedResult<TransactionSummary> result = ReportingService.FindTransactionsPaged(1, 10)
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .Where(SearchCriteria.Channel, channel)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.TrueForAll(t => t.Channel == EnumConverter.GetMapping(Target.GP_API, channel)));
        }

        [TestMethod]
        public void ReportFindTransactionsPaged_By_Status() {
            var transactionStatus = TransactionStatus.Captured;
            PagedResult<TransactionSummary> result = ReportingService.FindTransactionsPaged(1, 10)
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .Where(SearchCriteria.TransactionStatus, transactionStatus)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.TrueForAll(t => t.TransactionStatus == EnumConverter.GetMapping(Target.GP_API, transactionStatus)));
        }

        [TestMethod]
        public void ReportFindTransactionsPaged_By_CardBrand_And_AuthCode() {
            string cardBrand = "VISA";
            string authCode = "12345";
            PagedResult<TransactionSummary> result = ReportingService.FindTransactionsPaged(1, 10)
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
            string referenceNumber = "e1f2f968-e9cc-45b2-b41f-61cad13754aa";
            PagedResult<TransactionSummary> result = ReportingService.FindTransactionsPaged(1, 10)
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .Where(SearchCriteria.ReferenceNumber, referenceNumber)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.TrueForAll(t => t.ReferenceNumber == referenceNumber));
        }

        [TestMethod]
        public void ReportFindTransactionsPaged_By_BrandReference() {
            string brandReference = "D5v2Nv8h91Me3DTh";
            PagedResult<TransactionSummary> result = ReportingService.FindTransactionsPaged(1, 10)
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .Where(SearchCriteria.BrandReference, brandReference)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.TrueForAll(t => t.BrandReference == brandReference));
        }

        [TestMethod]
        public void ReportFindTransactionsPaged_By_EntryMode() {
            var paymentEntryMode = PaymentEntryMode.Ecom;
            PagedResult<TransactionSummary> result = ReportingService.FindTransactionsPaged(1, 10)
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .Where(SearchCriteria.PaymentEntryMode, paymentEntryMode)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.TrueForAll(t => t.EntryMode == EnumConverter.GetMapping(Target.GP_API, paymentEntryMode)));
        }

        [TestMethod]
        public void ReportFindTransactionsPaged_By_Number_First6_And_Last4() {
            var firstSix = "426397";
            var lastFour = "5262";
            PagedResult<TransactionSummary> result = ReportingService.FindTransactionsPaged(1, 10)
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
            var firstSix = "426397";
            var lastFour = "5262";
            PagedResult<TransactionSummary> result = ReportingService.FindTransactionsPaged(1, 10)
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .Where(SearchCriteria.TokenFirstSix, firstSix)
                .And(SearchCriteria.TokenLastFour, lastFour)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.TrueForAll(t => t.TokenPanLastFour.Contains(lastFour)));
        }

        [TestMethod]
        public void ReportFindTransactionsPaged_By_Name() {
            var name = "James Mason";
            PagedResult<TransactionSummary> result = ReportingService.FindTransactionsPaged(1, 10)
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Ascending)
                .Where(SearchCriteria.Name, name)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.TrueForAll(t => t.CardHolderName == name));
        }

        [TestMethod]
        public void ReportFindTransactionsPaged_By_StartDate_OrderBy_TimeCreated_Ascending() {
            DateTime startDate = DateTime.UtcNow.AddDays(-30);
            PagedResult<TransactionSummary> result = ReportingService.FindTransactionsPaged(1, 10)
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Ascending)
                .Where(SearchCriteria.StartDate, startDate)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results.Count > 0);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.SequenceEqual(result.Results.OrderBy(t => t.TransactionDate)));
        }

        [TestMethod]
        public void ReportFindTransactionsPaged_By_StartDate_OrderBy_Id_Ascending() {
            DateTime startDate = DateTime.UtcNow.AddDays(-30);
            PagedResult<TransactionSummary> result = ReportingService.FindTransactionsPaged(1, 10)
                .OrderBy(TransactionSortProperty.Id, SortDirection.Ascending)
                .Where(SearchCriteria.StartDate, startDate)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results.Count > 0);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.SequenceEqual(result.Results.OrderBy(t => t.TransactionId, StringComparer.Ordinal)));
        }

        [TestMethod]
        public void ReportFindTransactionsPaged_By_StartDate_OrderBy_Type_Ascending() {
            DateTime startDate = DateTime.UtcNow.AddDays(-30);
            PagedResult<TransactionSummary> result = ReportingService.FindTransactionsPaged(1, 10)
                .OrderBy(TransactionSortProperty.Type, SortDirection.Ascending)
                .Where(SearchCriteria.StartDate, startDate)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results.Count > 0);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.SequenceEqual(result.Results.OrderBy(t => t.TransactionType, StringComparer.Ordinal)));
        }

        [TestMethod]
        public void ReportFindTransactionsPaged_OrderBy_TimeCreated_Ascending() {
            PagedResult<TransactionSummary> result = ReportingService.FindTransactionsPaged(1, 25)
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Ascending)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.SequenceEqual(result.Results.OrderBy(t => t.TransactionDate)));
        }

        [TestMethod]
        public void ReportFindTransactionsPaged_OrderBy_TimeCreated_Descending() {
            PagedResult<TransactionSummary> result = ReportingService.FindTransactionsPaged(1, 25)
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.SequenceEqual(result.Results.OrderByDescending(t => t.TransactionDate)));
        }

        [TestMethod]
        public void ReportFindTransactionsPaged_OrderBy_Id_Ascending() {
            PagedResult<TransactionSummary> result = ReportingService.FindTransactionsPaged(1, 25)
                .OrderBy(TransactionSortProperty.Id, SortDirection.Ascending)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.SequenceEqual(result.Results.OrderBy(t => t.TransactionId, StringComparer.Ordinal)));
        }

        [TestMethod]
        public void ReportFindTransactionsPaged_OrderBy_Id_Descending() {
            PagedResult<TransactionSummary> result = ReportingService.FindTransactionsPaged(1, 25)
                .OrderBy(TransactionSortProperty.Id, SortDirection.Descending)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.SequenceEqual(result.Results.OrderByDescending(t => t.TransactionId, StringComparer.Ordinal)));
        }

        [TestMethod]
        public void ReportFindTransactionsPaged_OrderBy_Type_Ascending() {
            PagedResult<TransactionSummary> result = ReportingService.FindTransactionsPaged(1, 25)
                .OrderBy(TransactionSortProperty.Type, SortDirection.Ascending)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.SequenceEqual(result.Results.OrderBy(t => t.TransactionType, StringComparer.Ordinal)));
        }

        [TestMethod]
        public void ReportFindTransactionsPaged_OrderBy_Type_Descending() {
            PagedResult<TransactionSummary> result = ReportingService.FindTransactionsPaged(1, 25)
                .OrderBy(TransactionSortProperty.Type, SortDirection.Descending)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.SequenceEqual(result.Results.OrderByDescending(t => t.TransactionType, StringComparer.Ordinal)));
        }

        [TestMethod]
        public void ReportFindTransactionsPaged_Without_Mandatory_StartDate() {
            PagedResult<TransactionSummary> result = ReportingService.FindTransactionsPaged(1, 10)
                .OrderBy(TransactionSortProperty.Type, SortDirection.Ascending)
                .Execute();

            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results.Count > 0);
        }
        #endregion

        #region Settlement Transactions
        [TestMethod]
        public void ReportFindSettlementTransactionsPaged_By_StartDate_And_EndDate() {
            DateTime startDate = DateTime.UtcNow.AddDays(-30);
            DateTime endDate = DateTime.UtcNow.AddDays(-10);
            PagedResult<TransactionSummary> result = ReportingService.FindSettlementTransactionsPaged(1, 10)
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .Where(SearchCriteria.StartDate, startDate)
                .And(SearchCriteria.EndDate, endDate)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.TrueForAll(t => t.TransactionDate?.Date >= startDate.Date && t.TransactionDate?.Date <= endDate.Date));
        }

        [TestMethod]
        public void ReportFindSettlementTransactionsPaged_By_StartDate_OrderBy_TimeCreated_Ascending() {
            DateTime startDate = DateTime.UtcNow.AddDays(-30);
            PagedResult<TransactionSummary> result = ReportingService.FindSettlementTransactionsPaged(1, 10)
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Ascending)
                .Where(SearchCriteria.StartDate, startDate)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.SequenceEqual(result.Results.OrderBy(t => t.TransactionDate)));
        }

        [TestMethod]
        public void ReportFindSettlementTransactionsPaged_By_StartDate_OrderBy_TimeCreated_Descending() {
            DateTime startDate = DateTime.UtcNow.AddDays(-30);
            PagedResult<TransactionSummary> result = ReportingService.FindSettlementTransactionsPaged(1, 10)
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .Where(SearchCriteria.StartDate, startDate)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.SequenceEqual(result.Results.OrderByDescending(t => t.TransactionDate)));
        }

        [TestMethod]
        public void ReportFindSettlementTransactionsPaged_By_StartDate_OrderBy_Status_Ascending() {
            DateTime startDate = DateTime.UtcNow.AddDays(-30);
            PagedResult<TransactionSummary> result = ReportingService.FindSettlementTransactionsPaged(1, 10)
                .OrderBy(TransactionSortProperty.Status, SortDirection.Ascending)
                .Where(SearchCriteria.StartDate, startDate)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.SequenceEqual(result.Results.OrderBy(t => t.TransactionStatus)));
        }

        [TestMethod]
        public void ReportFindSettlementTransactionsPaged_By_StartDate_OrderBy_Status_Descending() {
            DateTime startDate = DateTime.UtcNow.AddDays(-30);
            PagedResult<TransactionSummary> result = ReportingService.FindSettlementTransactionsPaged(1, 10)
                .OrderBy(TransactionSortProperty.Status, SortDirection.Descending)
                .Where(SearchCriteria.StartDate, startDate)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.SequenceEqual(result.Results.OrderByDescending(t => t.TransactionStatus)));
        }

        [TestMethod]
        public void ReportFindSettlementTransactionsPaged_By_StartDate_OrderBy_Type_Ascending() {
            DateTime startDate = DateTime.UtcNow.AddDays(-30);
            PagedResult<TransactionSummary> result = ReportingService.FindSettlementTransactionsPaged(1, 10)
                .OrderBy(TransactionSortProperty.Type, SortDirection.Ascending)
                .Where(SearchCriteria.StartDate, startDate)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.SequenceEqual(result.Results.OrderBy(t => t.TransactionType)));
        }

        [TestMethod]
        public void ReportFindSettlementTransactionsPaged_By_StartDate_OrderBy_Type_Descending() {
            DateTime startDate = DateTime.UtcNow.AddDays(-30);
            PagedResult<TransactionSummary> result = ReportingService.FindSettlementTransactionsPaged(1, 10)
                .OrderBy(TransactionSortProperty.Type, SortDirection.Descending)
                .Where(SearchCriteria.StartDate, startDate)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.SequenceEqual(result.Results.OrderByDescending(t => t.TransactionType)));
        }

        [TestMethod]
        public void ReportFindSettlementTransactionsPaged_By_StartDate_OrderBy_DepositId_Ascending() {
            DateTime startDate = DateTime.UtcNow.AddDays(-30);
            PagedResult<TransactionSummary> result = ReportingService.FindSettlementTransactionsPaged(1, 10)
                .OrderBy(TransactionSortProperty.DepositId, SortDirection.Ascending)
                .Where(SearchCriteria.StartDate, startDate)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.SequenceEqual(result.Results.OrderBy(t => t.DepositReference)));
        }

        [TestMethod]
        public void ReportFindSettlementTransactionsPaged_By_StartDate_OrderBy_DepositId_Descending() {
            DateTime startDate = DateTime.UtcNow.AddDays(-30);
            PagedResult<TransactionSummary> result = ReportingService.FindSettlementTransactionsPaged(1, 10)
                .OrderBy(TransactionSortProperty.DepositId, SortDirection.Descending)
                .Where(SearchCriteria.StartDate, startDate)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.SequenceEqual(result.Results.OrderByDescending(t => t.DepositReference)));
        }

        [TestMethod]
        public void ReportFindSettlementTransactionsPaged_By_Number_First6_And_Number_Last4() {
            string firstSix = "543458";
            string lastFour = "7652";
            DateTime startDate = DateTime.UtcNow.AddDays(-30);
            PagedResult<TransactionSummary> result = ReportingService.FindSettlementTransactionsPaged(1, 10)
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .Where(SearchCriteria.StartDate, startDate)
                .And(SearchCriteria.CardNumberFirstSix, firstSix)
                .And(SearchCriteria.CardNumberLastFour, lastFour)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.TrueForAll(t => t.MaskedCardNumber.StartsWith(firstSix) && t.MaskedCardNumber.EndsWith(lastFour)));
        }

        [TestMethod]
        public void ReportFindSettlementTransactionsPaged_By_CardBrand() {
            string cardBrand = "MASTERCARD";

            DateTime startDate = DateTime.UtcNow.AddDays(-30);
            PagedResult<TransactionSummary> result = ReportingService.FindSettlementTransactionsPaged(1, 10)
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .Where(SearchCriteria.StartDate, startDate)
                .And(SearchCriteria.CardBrand, cardBrand)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.TrueForAll(t => t.CardType == cardBrand));
        }

        [TestMethod]
        public void ReportFindSettlementTransactionsPaged_By_InvalidCardBrand() {
            string cardBrand = "MASTER";

            DateTime startDate = DateTime.UtcNow.AddDays(-30);
            PagedResult<TransactionSummary> result = ReportingService.FindSettlementTransactionsPaged(1, 10)
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .Where(SearchCriteria.StartDate, startDate)
                .And(SearchCriteria.CardBrand, cardBrand)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.Count == 0);
        }

        [TestMethod]
        public void ReportFindSettlementTransactionsPaged_By_DepositStatus() {
            var depositStatus = DepositStatus.Delayed;
            DateTime startDate = DateTime.UtcNow.AddDays(-30);
            PagedResult<TransactionSummary> result = ReportingService.FindSettlementTransactionsPaged(1, 10)
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .Where(SearchCriteria.StartDate, startDate)
                .And(SearchCriteria.DepositStatus, depositStatus)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
        }

        [TestMethod]
        public void ReportFindSettlementTransactionsPaged_By_ARN() {
            string arn = "74500010037624410827759";

            DateTime startDate = DateTime.UtcNow.AddDays(-30);
            PagedResult<TransactionSummary> result = ReportingService.FindSettlementTransactionsPaged(1, 10)
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .Where(SearchCriteria.StartDate, startDate)
                .And(SearchCriteria.AquirerReferenceNumber, arn)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.TrueForAll(t => t.AquirerReferenceNumber == arn));
        }

        [TestMethod]
        public void ReportFindSettlementTransactionsPaged_By_WrongARN() {
            string arn = "00000010037624410827527";

            DateTime startDate = DateTime.UtcNow.AddDays(-30);
            PagedResult<TransactionSummary> result = ReportingService.FindSettlementTransactionsPaged(1, 10)
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .Where(SearchCriteria.StartDate, startDate)
                .And(SearchCriteria.AquirerReferenceNumber, arn)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.Count == 0);
        }

        [TestMethod]
        public void ReportFindSettlementTransactionsPaged_By_BrandReference() {
            string brandReference = "MCF1CZ5ME5405";

            DateTime startDate = DateTime.UtcNow.AddDays(-30);
            PagedResult<TransactionSummary> result = ReportingService.FindSettlementTransactionsPaged(1, 10)
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .Where(SearchCriteria.StartDate, startDate)
                .And(SearchCriteria.BrandReference, brandReference)
                .Execute();
            Assert.IsNotNull(result.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.TrueForAll(t => t.BrandReference == brandReference));
        }

        [TestMethod]
        public void ReportFindSettlementTransactionsPaged_By_WrongBrandReference() {
            string brandReference = "MCF1CZ5ME5001";

            DateTime startDate = DateTime.UtcNow.AddDays(-30);
            PagedResult<TransactionSummary> result = ReportingService.FindSettlementTransactionsPaged(1, 10)
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .Where(SearchCriteria.StartDate, startDate)
                .And(SearchCriteria.BrandReference, brandReference)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.Count == 0);
        }

        [TestMethod]
        public void ReportFindSettlementTransactionsPaged_By_CardBrand_And_AuthCode() {
            string cardBrand = "MASTERCARD";
            string authCode = "028010";

            DateTime startDate = DateTime.UtcNow.AddDays(-30);
            PagedResult<TransactionSummary> result = ReportingService.FindSettlementTransactionsPaged(1, 10)
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .Where(SearchCriteria.StartDate, startDate)
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
            string reference = "50080513769";

            DateTime startDate = DateTime.UtcNow.AddDays(-30);
            PagedResult<TransactionSummary> result = ReportingService.FindSettlementTransactionsPaged(1, 10)
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .Where(SearchCriteria.StartDate, startDate)
                .And(SearchCriteria.ReferenceNumber, reference)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.TrueForAll(t => t.ReferenceNumber == reference));
        }

        [TestMethod]
        public void ReportFindSettlementTransactionsPaged_By_RandomReference() {
            string reference = Guid.NewGuid().ToString();

            DateTime startDate = DateTime.UtcNow.AddDays(-30);
            PagedResult<TransactionSummary> result = ReportingService.FindSettlementTransactionsPaged(1, 10)
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .Where(SearchCriteria.StartDate, startDate)
                .And(SearchCriteria.ReferenceNumber, reference)
                .Execute();

            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.Count == 0);
        }

        [TestMethod]
        public void ReportFindSettlementTransactionsPaged_By_Status() {
            var status = TransactionStatus.Rejected;

            DateTime startDate = DateTime.UtcNow.AddDays(-30);
            PagedResult<TransactionSummary> result = ReportingService.FindSettlementTransactionsPaged(1, 10)
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .Where(SearchCriteria.StartDate, startDate)
                .And(SearchCriteria.TransactionStatus, status)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.TrueForAll(t => t.Status == status.ToString()));
        }

        [TestMethod]
        public void ReportFindSettlementTransactionsPaged_By_DepositReference() {
            var depositReference = "DEP_2342423429";

            DateTime startDate = DateTime.UtcNow.AddDays(-30);
            PagedResult<TransactionSummary> result = ReportingService.FindSettlementTransactionsPaged(1, 10)
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .Where(SearchCriteria.StartDate, startDate)
                .And(DataServiceCriteria.DepositReference, depositReference)
                .Execute();
            Assert.IsNotNull(result.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.TrueForAll(t => t.DepositReference == depositReference));
        }

        [TestMethod]
        public void ReportFindSettlementTransactionsPaged_By_RandomDepositReference() {
            var depositReference = Guid.NewGuid().ToString();

            DateTime startDate = DateTime.UtcNow.AddDays(-30);
            PagedResult<TransactionSummary> result = ReportingService.FindSettlementTransactionsPaged(1, 10)
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .Where(SearchCriteria.StartDate, startDate)
                .And(DataServiceCriteria.DepositReference, depositReference)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.Count == 0);
        }

        [TestMethod]
        public void ReportFindSettlementTransactionsPaged_By_FromDepositTimeCreated_And_ToDepositTimeCreated() {
            DateTime startDate = DateTime.UtcNow.AddDays(-90);
            DateTime startDateDeposit = DateTime.UtcNow.AddDays(-89);
            DateTime endDateDeposit = DateTime.UtcNow.AddDays(-1);

            PagedResult<TransactionSummary> result = ReportingService.FindSettlementTransactionsPaged(1, 10)
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .Where(SearchCriteria.StartDate, startDate)
                .And(DataServiceCriteria.StartDepositDate, startDateDeposit)
                .And(DataServiceCriteria.EndDepositDate, endDateDeposit)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.TrueForAll(t => t.DepositDate?.Date >= startDateDeposit.Date && t.DepositDate?.Date <= endDateDeposit.Date));
        }

        [TestMethod]
        public void ReportFindSettlementTransactionsPaged_By_FromBatchTimeCreated_And_ToBatchTimeCreated() {
            DateTime startDate = DateTime.UtcNow.AddDays(-90);
            DateTime startBatchDate = DateTime.UtcNow.AddDays(-89);
            DateTime endBatchDate = DateTime.UtcNow.AddDays(-1);

            PagedResult<TransactionSummary> result = ReportingService.FindSettlementTransactionsPaged(1, 10)
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .Where(SearchCriteria.StartDate, startDate)
                .And(DataServiceCriteria.StartBatchDate, startBatchDate)
                .And(DataServiceCriteria.EndBatchDate, endBatchDate)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.TrueForAll(t => t.BatchCloseDate?.Date >= startBatchDate.Date && t.BatchCloseDate?.Date <= endBatchDate.Date));
        }

        [TestMethod]
        public void ReportFindSettlementTransactionsPaged_By_MerchantId_And_SystemHierarchy() {
            string merchantId = "101023947262";
            string systemHierarchy = "055-70-024-011-019";

            DateTime startDate = DateTime.UtcNow.AddDays(-30);
            PagedResult<TransactionSummary> result = ReportingService.FindSettlementTransactionsPaged(1, 10)
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .Where(SearchCriteria.StartDate, startDate)
                .And(DataServiceCriteria.MerchantId, merchantId)
                .And(DataServiceCriteria.SystemHierarchy, systemHierarchy)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.TrueForAll(t => t.MerchantId == merchantId && t.MerchantHierarchy == systemHierarchy));
        }

        [TestMethod]
        public void ReportFindSettlementTransactionsPaged_By_NonExistent_MerchantId() {
            string merchantId = "100023947222";
            DateTime startDate = DateTime.UtcNow.AddDays(-30);
            PagedResult<TransactionSummary> result = ReportingService.FindSettlementTransactionsPaged(1, 10)
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .Where(SearchCriteria.StartDate, startDate)
                .And(DataServiceCriteria.MerchantId, merchantId)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.Count == 0);
        }

        [TestMethod]
        public void ReportFindSettlementTransactionsPaged_By_Random_SystemHierarchy() {
            string systemHierarchy = Guid.NewGuid().ToString();

            DateTime startDate = DateTime.UtcNow.AddDays(-30);
            try {
                ReportingService.FindSettlementTransactionsPaged(1, 10)
                    .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                    .Where(SearchCriteria.StartDate, startDate)
                    .And(DataServiceCriteria.SystemHierarchy, systemHierarchy)
                    .Execute();
            }
            catch (GatewayException ex) {
                Assert.AreEqual("INVALID_REQUEST_DATA", ex.ResponseCode);
                Assert.AreEqual("40105", ex.ResponseMessage);
                Assert.IsTrue(ex.Message.Equals("Status Code: BadRequest - " +
                    "Invalid Value provided in the input field - system.hierarchy"));
            }
        }

        [TestMethod]
        public void ReportFindSettlementTransactionsPaged_By_Invalid_MerchantId() {
            string merchantId = Guid.NewGuid().ToString();
            DateTime startDate = DateTime.UtcNow.AddDays(-30);
            try {
                ReportingService.FindSettlementTransactionsPaged(1, 10)
                    .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                    .Where(SearchCriteria.StartDate, startDate)
                    .And(DataServiceCriteria.MerchantId, merchantId)
                    .Execute();
            }
            catch (GatewayException ex) {
                Assert.AreEqual("INVALID_REQUEST_DATA", ex.ResponseCode);
                Assert.AreEqual("40100", ex.ResponseMessage);
                Assert.IsTrue(ex.Message.Equals("Status Code: BadRequest - " +
                    "Invalid Value provided in the input field - system.mid"));
            }
        }

        #endregion

        #region Deposits
        [TestMethod]
        public void ReportDepositDetail() {
            string depositId = "DEP_2342423423";
            DepositSummary response = ReportingService.DepositDetail(depositId)
                .Execute();
            Assert.IsNotNull(response);
            Assert.IsTrue(response is DepositSummary);
            Assert.AreEqual(depositId, response.DepositId);
        }

        [TestMethod]
        public void ReportDepositDetailWrongId() {
            string depositId = "DEP_1112423111";
            try {
                DepositSummary response = ReportingService.DepositDetail(depositId)
                    .Execute();
            }
            catch (GatewayException ex) {
                Assert.AreEqual("RESOURCE_NOT_FOUND", ex.ResponseCode);
                Assert.AreEqual("40118", ex.ResponseMessage);
                Assert.IsTrue(ex.Message.StartsWith("Status Code: NotFound - Deposits DEP_1112423111 not found"));
            }
        }

        [TestMethod]
        public void ReportFindDepositsPaged_By_StartDate_Order_By_TimeCreated() {
            DateTime startDate = DateTime.UtcNow.AddDays(-30);
            PagedResult<DepositSummary> result = ReportingService.FindDepositsPaged(1, 10)
                .OrderBy(DepositSortProperty.TimeCreated, SortDirection.Descending)
                .Where(SearchCriteria.StartDate, startDate)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DepositSummary>);
            Assert.IsTrue(result.Results.TrueForAll(d => d.DepositDate?.Date >= startDate.Date));
        }

        [TestMethod]
        public void ReportFindDepositsPaged_By_EndDate_Order_By_TimeCreated() {
            DateTime startDate = DateTime.UtcNow.AddDays(-300);
            DateTime endDate = DateTime.UtcNow;
            PagedResult<DepositSummary> result = ReportingService.FindDepositsPaged(1, 10)
                .OrderBy(DepositSortProperty.TimeCreated, SortDirection.Descending)
                .Where(SearchCriteria.StartDate, startDate)
                .And(SearchCriteria.EndDate, endDate)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DepositSummary>);
            Assert.IsTrue(result.Results.TrueForAll(d => d.DepositDate?.Date >= startDate.Date && d.DepositDate?.Date <= endDate.Date));
        }

        [TestMethod]
        public void ReportFindDepositsPaged_By_Amount() {
            decimal amount = 141;
            PagedResult<DepositSummary> result = ReportingService.FindDepositsPaged(1, 10)
                .OrderBy(DepositSortProperty.TimeCreated, SortDirection.Descending)
                .Where(DataServiceCriteria.Amount, amount)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DepositSummary>);
            Assert.IsTrue(result.Results.TrueForAll(d => d.Amount == amount));
        }

        [TestMethod]
        public void ReportFindDepositsPaged_By_NotFoundAmount() {
            decimal amount = 140;
            PagedResult<DepositSummary> result = ReportingService.FindDepositsPaged(1, 10)
                .OrderBy(DepositSortProperty.TimeCreated, SortDirection.Descending)
                .Where(DataServiceCriteria.Amount, amount)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DepositSummary>);
            Assert.IsTrue(result.Results.Count == 0);
        }

        [TestMethod]
        public void ReportFindDepositsPaged_By_Status() {
            string depositStatus = "IRREG";
            PagedResult<DepositSummary> result = ReportingService.FindDepositsPaged(1, 10)
                .OrderBy(DepositSortProperty.TimeCreated, SortDirection.Descending)
                .Where(SearchCriteria.DepositStatus, DepositStatus.Irregular)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DepositSummary>);
            Assert.IsTrue(result.Results.TrueForAll(d => d.Status == depositStatus));
        }

        [TestMethod]
        public void ReportFindDepositsPaged_By_Masked_Account_Number_Last4() {
            string masketAccountNumberLast4 = "9999";
            PagedResult<DepositSummary> result = ReportingService.FindDepositsPaged(1, 10)
                .OrderBy(DepositSortProperty.TimeCreated, SortDirection.Descending)
                .Where(SearchCriteria.AccountNumberLastFour, masketAccountNumberLast4)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DepositSummary>);
            Assert.IsTrue(result.Results.Count > 0);
        }

        [TestMethod]
        public void ReportFindDepositsPaged_By_SystemHierarchy() {
            string systemHierarchy = "055-70-024-011-019";
            PagedResult<DepositSummary> result = ReportingService.FindDepositsPaged(1, 10)
                .OrderBy(DepositSortProperty.TimeCreated, SortDirection.Descending)
                .Where(DataServiceCriteria.SystemHierarchy, systemHierarchy)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DepositSummary>);
            Assert.IsTrue(result.Results.TrueForAll(d => d.MerchantHierarchy == systemHierarchy));
        }

        [TestMethod]
        public void ReportFindDepositsPaged_By_WrongSystemHierarchy() {
            string systemHierarchy = "042-70-013-011-018";
            PagedResult<DepositSummary> result = ReportingService.FindDepositsPaged(1, 10)
                .OrderBy(DepositSortProperty.TimeCreated, SortDirection.Descending)
                .Where(DataServiceCriteria.SystemHierarchy, systemHierarchy)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DepositSummary>);
            Assert.IsTrue(result.Results.Count == 0);
        }

        [TestMethod]
        public void ReportFindDepositsPaged_By_RandomUUIDSystemHierarchy() {
            string systemHierarchy = Guid.NewGuid().ToString();
            try {
                ReportingService.FindDepositsPaged(1, 10)
                    .OrderBy(DepositSortProperty.TimeCreated, SortDirection.Descending)
                    .Where(DataServiceCriteria.SystemHierarchy, systemHierarchy)
                    .Execute();
            }
            catch (GatewayException ex) {
                Assert.AreEqual("INVALID_REQUEST_DATA", ex.ResponseCode);
                Assert.AreEqual("40105", ex.ResponseMessage);
                Assert.IsTrue(ex.Message.Contains("Invalid Value provided in the input field - system.hierarch"));
            }
        }

        [TestMethod]
        public void ReportFindDepositsPaged_WithoutFromTimeCreated() {

            string systemHierarchy = Guid.NewGuid().ToString();
            try {
                ReportingService.FindDepositsPaged(1, 10)
                    .OrderBy(DepositSortProperty.TimeCreated, SortDirection.Descending)
                    .Where(DataServiceCriteria.SystemHierarchy, systemHierarchy)
                    .Execute();
            }
            catch (GatewayException ex) {
                Assert.AreEqual("INVALID_REQUEST_DATA", ex.ResponseCode);
                Assert.AreEqual("40105", ex.ResponseMessage);
                Assert.IsTrue(ex.Message.Contains("Invalid Value provided in the input field - system.hierarch"));
            }
        }

        [TestMethod]
        public void ReportFindDepositsPaged_By_MerchantId() {
            string merchantId = "101023947262";
            PagedResult<DepositSummary> result = ReportingService.FindDepositsPaged(1, 10)
                .OrderBy(DepositSortProperty.TimeCreated, SortDirection.Descending)
                .Where(DataServiceCriteria.MerchantId, merchantId)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DepositSummary>);
            Assert.IsTrue(result.Results.TrueForAll(d => d.MerchantNumber == merchantId));
        }

        [TestMethod]
        public void ReportFindDepositsPaged_By_Wrong_MerchantId() {
            string merchantId = "000023985843";
            var result = ReportingService.FindDepositsPaged(1, 10)
                .OrderBy(DepositSortProperty.TimeCreated, SortDirection.Descending)
                .Where(DataServiceCriteria.MerchantId, merchantId)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DepositSummary>);
            Assert.IsTrue(result.Results.Count == 0);
        }

        [TestMethod]
        public void ReportFindDepositsPaged_By_DepositId() {
            var depositId = "DEP_2342423440";
            PagedResult<DepositSummary> result = ReportingService.FindDepositsPaged(1, 10)
                .OrderBy(DepositSortProperty.TimeCreated, SortDirection.Descending)
                .WithDepositReference(depositId)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results.Count == 1);
            Assert.IsTrue(result.Results.TrueForAll(d => d.DepositId == depositId));
        }

        [TestMethod]
        public void ReportFindDepositsPaged_By_WrongDepositId() {

            string depositId = "DEP_1112423111";
            DateTime startDate = DateTime.UtcNow.AddDays(-30);

            PagedResult<DepositSummary> result = ReportingService.FindDepositsPaged(1, 10)
                .OrderBy(DepositSortProperty.TimeCreated, SortDirection.Descending)
                .WithDepositReference(depositId)
                .Where(SearchCriteria.StartDate, startDate)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results.Count == 0);
        }

        [TestMethod]
        public void ReportFindDepositsPaged_Order_By_DepositId() {
            PagedResult<DepositSummary> result = ReportingService.FindDepositsPaged(1, 10)
                .OrderBy(DepositSortProperty.DepositId, SortDirection.Descending)
                .Where(SearchCriteria.StartDate, DateTime.UtcNow.AddDays(-30))
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DepositSummary>);
        }

        [TestMethod]
        public void ReportFindDepositsPaged_Order_By_Status() {
            PagedResult<DepositSummary> result = ReportingService.FindDepositsPaged(1, 10)
                .OrderBy(DepositSortProperty.Status, SortDirection.Ascending)
                .Where(SearchCriteria.StartDate, DateTime.UtcNow.AddDays(-30))
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DepositSummary>);
        }

        [TestMethod]
        public void ReportFindDepositsPaged_Order_By_Type() {
            PagedResult<DepositSummary> result = ReportingService.FindDepositsPaged(1, 10)
                .OrderBy(DepositSortProperty.Type, SortDirection.Ascending)
                .Where(SearchCriteria.StartDate, DateTime.UtcNow.AddDays(-30))
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DepositSummary>);
        }

        [TestMethod]
        public void CompareResults_reportFindDepositsWithCriteria_OrderBy_DepositId_And_Type() {
            PagedResult<DepositSummary> resultById = ReportingService.FindDepositsPaged(1, 10)
                .OrderBy(DepositSortProperty.DepositId, SortDirection.Descending)
                .Where(SearchCriteria.StartDate, DateTime.UtcNow.AddDays(-30))
                .Execute();

            PagedResult<DepositSummary> resultByType = ReportingService.FindDepositsPaged(1, 10)
                .OrderBy(DepositSortProperty.Type, SortDirection.Ascending)
                .Where(SearchCriteria.StartDate, DateTime.UtcNow.AddDays(-30))
                .Execute();

            Assert.IsNotNull(resultById?.Results);
            Assert.IsTrue(resultById.Results.Count > 0);
            Assert.IsNotNull(resultByType?.Results);
            Assert.IsTrue(resultByType.Results.Count > 0);

            Assert.AreNotEqual(resultById.Results, resultByType.Results);
        }
        #endregion

        #region Disputes
        [TestMethod]
        public void ReportDisputeDetail() {
            string disputeId = "DIS_SAND_abcd1234";
            DisputeSummary response = ReportingService.DisputeDetail(disputeId)
                .Execute();
            Assert.IsNotNull(response);
            Assert.IsTrue(response is DisputeSummary);
            Assert.AreEqual(disputeId, response.CaseId);
        }

        [TestMethod]
        public void ReportDisputeDetail_WrongId() {
            string disputeId = "DIS_SAND_" + Guid.NewGuid().ToString();
            try {
                ReportingService.DisputeDetail(disputeId)
                    .Execute();
            }
            catch (GatewayException ex) {
                Assert.AreEqual("INVALID_REQUEST_DATA", ex.ResponseCode);
                Assert.AreEqual("40073", ex.ResponseMessage);
                Assert.IsTrue(ex.Message.Contains("Status Code: BadRequest - 101,Unable to locate dispute record for that ID. Please recheck the ID provided."));
            }
        }

        [TestMethod]
        public void ReportFindDisputesPaged_By_ARN() {
            string arn = "135091790340196";
            PagedResult<DisputeSummary> result = ReportingService.FindDisputesPaged(1, 10)
                .Where(DataServiceCriteria.StartStageDate, new DateTime(2020, 1, 1))
                .And(SearchCriteria.AquirerReferenceNumber, arn)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
            Assert.IsTrue(result.Results.TrueForAll(d => d.TransactionARN == arn));
        }

        [TestMethod]
        public void ReportFindDisputesPaged_By_Brand() {
            string cardBrand = "VISA";
            PagedResult<DisputeSummary> result = ReportingService.FindDisputesPaged(1, 10)
                .Where(DataServiceCriteria.StartStageDate, new DateTime(2020, 1, 1))
                .And(SearchCriteria.CardBrand, cardBrand)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
            Assert.IsTrue(result.Results.TrueForAll(d => d.TransactionCardType == cardBrand));
        }

        [TestMethod]
        public void ReportFindDisputesPaged_By_Status() {
            DisputeStatus disputeStatus = DisputeStatus.UnderReview;
            PagedResult<DisputeSummary> result = ReportingService.FindDisputesPaged(1, 10)
                .Where(DataServiceCriteria.StartStageDate, new DateTime(2020, 1, 1))
                .And(SearchCriteria.DisputeStatus, disputeStatus)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
            Assert.IsTrue(result.Results.TrueForAll(d => d.CaseStatus == EnumConverter.GetMapping(Target.GP_API, disputeStatus)));
        }

        [TestMethod]
        public void ReportFindDisputesPaged_By_Stage() {
            DisputeStage disputeStage = DisputeStage.Chargeback;
            PagedResult<DisputeSummary> result = ReportingService.FindDisputesPaged(1, 10)
                .Where(DataServiceCriteria.StartStageDate, new DateTime(2020, 1, 1))
                .And(SearchCriteria.DisputeStage, disputeStage)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
            Assert.IsTrue(result.Results.TrueForAll(d => d.CaseStage == EnumConverter.GetMapping(Target.GP_API, disputeStage)));
        }

        [TestMethod]
        public void ReportFindDisputesPaged_By_From_And_To_Stage_Time_Created() {
            DateTime startDate = DateTime.UtcNow.AddDays(-60);
            DateTime endDate = DateTime.UtcNow;
            PagedResult<DisputeSummary> result = ReportingService.FindDisputesPaged(1, 10)
                .Where(DataServiceCriteria.StartStageDate, startDate)
                .And(DataServiceCriteria.EndStageDate, endDate)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
            Assert.IsTrue(result.Results.TrueForAll(d => d.CaseIdTime >= startDate.Date && d.CaseIdTime <= endDate.Date));
        }

        [TestMethod]
        public void ReportFindDisputesPaged_By_MerchantId() {
            string merchantId = "8593872";
            PagedResult<DisputeSummary> result = ReportingService.FindDisputesPaged(1, 10)
                .Where(DataServiceCriteria.StartStageDate, new DateTime(2020, 1, 1))
                .And(DataServiceCriteria.MerchantId, merchantId)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
            Assert.IsTrue(result.Results.TrueForAll(d => d.CaseMerchantId == merchantId));
        }

        [TestMethod]
        public void ReportFindDisputesPaged_By_SystemHierarchy() {
            string systemHierarchy = "111-23-099-002-005";
            PagedResult<DisputeSummary> result = ReportingService.FindDisputesPaged(1, 10)
                .Where(DataServiceCriteria.StartStageDate, new DateTime(2020, 1, 1))
                .And(DataServiceCriteria.SystemHierarchy, systemHierarchy)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
            Assert.IsTrue(result.Results.TrueForAll(d => d.MerchantHierarchy == systemHierarchy));
        }

        [TestMethod]
        public void ReportFindDisputesPaged_Order_By_Id() {
            PagedResult<DisputeSummary> result = ReportingService.FindDisputesPaged(1, 10)
                .OrderBy(DisputeSortProperty.Id, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, new DateTime(2020, 1, 1))
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindDisputesPaged_Order_By_ARN() {
            PagedResult<DisputeSummary> result = ReportingService.FindDisputesPaged(1, 10)
                .OrderBy(DisputeSortProperty.ARN, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, new DateTime(2020, 6, 9))
                // EndStageDate must be set in order to be able to sort by ARN
                .And(DataServiceCriteria.EndStageDate, new DateTime(2020, 6, 22))
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindDisputesPaged_Order_By_Brand() {
            PagedResult<DisputeSummary> result = ReportingService.FindDisputesPaged(1, 10)
                .OrderBy(DisputeSortProperty.Brand, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, new DateTime(2020, 1, 1))
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindDisputesPaged_Order_By_Status() {
            PagedResult<DisputeSummary> result = ReportingService.FindDisputesPaged(1, 10)
                .OrderBy(DisputeSortProperty.Status, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, new DateTime(2020, 1, 1))
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindDisputesPaged_Order_By_Stage() {
            PagedResult<DisputeSummary> result = ReportingService.FindDisputesPaged(1, 10)
                .OrderBy(DisputeSortProperty.Stage, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, new DateTime(2020, 1, 1))
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindDisputesPaged_Order_By_FromStageTimeCreated() {
            PagedResult<DisputeSummary> result = ReportingService.FindDisputesPaged(1, 10)
                .OrderBy(DisputeSortProperty.FromStageTimeCreated, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, new DateTime(2020, 1, 1))
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindDisputesPaged_Order_By_ToStageTimeCreated() {
            PagedResult<DisputeSummary> result = ReportingService.FindDisputesPaged(1, 10)
                .OrderBy(DisputeSortProperty.ToStageTimeCreated, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, new DateTime(2020, 1, 1))
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindDisputesPaged_Order_By_AdjustmentFunding() {
            PagedResult<DisputeSummary> result = ReportingService.FindDisputesPaged(1, 10)
                .OrderBy(DisputeSortProperty.AdjustmentFunding, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, new DateTime(2020, 1, 1))
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindDisputesPaged_Order_By_FromAdjustmentTimeCreated() {
            PagedResult<DisputeSummary> result = ReportingService.FindDisputesPaged(1, 10)
                .OrderBy(DisputeSortProperty.FromAdjustmentTimeCreated, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, new DateTime(2020, 1, 1))
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindDisputesPaged_Order_By_ToAdjustmentTimeCreated() {
            PagedResult<DisputeSummary> result = ReportingService.FindDisputesPaged(1, 10)
                .OrderBy(DisputeSortProperty.ToAdjustmentTimeCreated, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, new DateTime(2020, 1, 1))
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindDisputesPaged_Order_By_Id_With_Brand_VISA() {
            PagedResult<DisputeSummary> result = ReportingService.FindDisputesPaged(1, 10)
                .OrderBy(DisputeSortProperty.Id, SortDirection.Ascending)
                .Where(DataServiceCriteria.StartStageDate, new DateTime(2020, 1, 1))
                .And(SearchCriteria.CardBrand, "VISA")
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindDisputesPaged_Order_By_Id_With_Status_UnderReview() {
            PagedResult<DisputeSummary> result = ReportingService.FindDisputesPaged(1, 10)
                .OrderBy(DisputeSortProperty.Id, SortDirection.Ascending)
                .Where(DataServiceCriteria.StartStageDate, new DateTime(2020, 1, 1))
                .And(SearchCriteria.DisputeStatus, DisputeStatus.UnderReview)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindDisputesPaged_Order_By_Id_With_Stage_Chargeback() {
            PagedResult<DisputeSummary> result = ReportingService.FindDisputesPaged(1, 10)
                .OrderBy(DisputeSortProperty.Id, SortDirection.Ascending)
                .Where(DataServiceCriteria.StartStageDate, new DateTime(2020, 1, 1))
                .And(SearchCriteria.DisputeStage, DisputeStage.Chargeback)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
        }

        #endregion

        #region Settlement disputes
        [TestMethod]
        public void ReportSettlementDisputeDetail() {
            string settlementDisputeId = "DIS_810";
            DisputeSummary response = ReportingService.SettlementDisputeDetail(settlementDisputeId)
                .Execute();
            Assert.IsNotNull(response);
            Assert.IsTrue(response is DisputeSummary);
            Assert.AreEqual(settlementDisputeId, response.CaseId);
        }

        [TestMethod]
        public void ReportSettlementDisputeDetailWrongId() {
            string disputeId = "DIS_666";
            try {
                ReportingService.SettlementDisputeDetail(disputeId)
                    .Execute();
            }
            catch (GatewayException ex) {
                Assert.AreEqual("RESOURCE_NOT_FOUND", ex.ResponseCode);
                Assert.AreEqual("40118", ex.ResponseMessage);
                Assert.AreEqual($"Status Code: NotFound - Disputes {disputeId} not found at this /ucp/settlement/disputes/{disputeId}", ex.Message);
            }
        }

        [TestMethod]
        public void ReportFindSettlementDisputesPaged_Order_By_Id() {
            PagedResult<DisputeSummary> result = ReportingService.FindSettlementDisputesPaged(1, 10)
                .OrderBy(DisputeSortProperty.Id, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, new DateTime(2020, 1, 1))
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindSettlementDisputesPaged_OrderBy_Brand() {
            PagedResult<DisputeSummary> result = ReportingService.FindSettlementDisputesPaged(1, 10)
                .OrderBy(DisputeSortProperty.Brand, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, new DateTime(2020, 1, 1))
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindSettlementDisputesPaged_OrderBy_Stage() {
            PagedResult<DisputeSummary> result = ReportingService.FindSettlementDisputesPaged(1, 10)
                .OrderBy(DisputeSortProperty.Stage, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, new DateTime(2020, 1, 1))
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindSettlementDisputesPaged_OrderBy_FromStageTimeCreated() {
            PagedResult<DisputeSummary> result = ReportingService.FindSettlementDisputesPaged(1, 10)
                .OrderBy(DisputeSortProperty.FromStageTimeCreated, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, new DateTime(2020, 1, 1))
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindSettlementDisputesPaged_OrderBy_ToStageTimeCreated() {
            PagedResult<DisputeSummary> result = ReportingService.FindSettlementDisputesPaged(1, 10)
                .OrderBy(DisputeSortProperty.ToStageTimeCreated, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, new DateTime(2020, 1, 1))
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindSettlementDisputesPaged_OrderBy_AdjustmentFunding() {
            PagedResult<DisputeSummary> result = ReportingService.FindSettlementDisputesPaged(1, 10)
                .OrderBy(DisputeSortProperty.AdjustmentFunding, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, new DateTime(2020, 1, 1))
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindSettlementDisputesPaged_OrderBy_FromAdjustmentTimeCreated() {
            PagedResult<DisputeSummary> result = ReportingService.FindSettlementDisputesPaged(1, 10)
                .OrderBy(DisputeSortProperty.FromAdjustmentTimeCreated, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, new DateTime(2020, 1, 1))
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindSettlementDisputesPaged_OrderBy_ToAdjustmentTimeCreated() {
            PagedResult<DisputeSummary> result = ReportingService.FindSettlementDisputesPaged(1, 10)
                .OrderBy(DisputeSortProperty.ToAdjustmentTimeCreated, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, new DateTime(2020, 1, 1))
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindSettlementDisputesPaged_By_ARN() {
            string arn = "71400011129688701392096";

            PagedResult<DisputeSummary> result = ReportingService.FindSettlementDisputesPaged(1, 10)
                .OrderBy(DisputeSortProperty.Id, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, new DateTime(2020, 1, 1))
                .And(SearchCriteria.AquirerReferenceNumber, arn)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
            Assert.IsTrue(result.Results.TrueForAll(d => d.TransactionARN == arn));
        }

        [TestMethod]
        public void ReportFindSettlementDisputesPaged_By_ARN_NotFound() {
            string arn = "00000011129654301392121";

            PagedResult<DisputeSummary> result = ReportingService.FindSettlementDisputesPaged(1, 10)
                .OrderBy(DisputeSortProperty.Id, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, new DateTime(2020, 1, 1))
                .And(SearchCriteria.AquirerReferenceNumber, arn)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
            Assert.IsTrue(result.Results.Count == 0);
        }

        [TestMethod]
        public void ReportFindSettlementDisputesPaged_By_Brand() {
            string brand = "VISA";

            PagedResult<DisputeSummary> result = ReportingService.FindSettlementDisputesPaged(1, 10)
                .OrderBy(DisputeSortProperty.Id, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, new DateTime(2020, 1, 1))
                .And(SearchCriteria.CardBrand, brand)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindSettlementDisputesPaged_By_Brand_NotFound() {
            string brand = "MASTERCAR";

            PagedResult<DisputeSummary> result = ReportingService.FindSettlementDisputesPaged(1, 10)
                .OrderBy(DisputeSortProperty.Id, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, new DateTime(2020, 1, 1))
                .And(SearchCriteria.CardBrand, brand)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
            Assert.IsTrue(result.Results.Count == 0);
        }

        [TestMethod]
        public void ReportFindSettlementDisputesPaged_By_Status() {
            string status = "WITH_MERCHANT";
            PagedResult<DisputeSummary> result = ReportingService.FindSettlementDisputesPaged(1, 10)
                .OrderBy(DisputeSortProperty.Id, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, new DateTime(2020, 1, 1))
                .And(SearchCriteria.DisputeStatus, DisputeStatus.WithMerchant)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
            Assert.IsTrue(result.Results.TrueForAll(d => d.CaseStatus == status));
        }

        [TestMethod]
        public void ReportFindSettlementDisputesPaged_By_Stage() {
            string stage = "CHARGEBACK";
            PagedResult<DisputeSummary> result = ReportingService.FindSettlementDisputesPaged(1, 10)
                .OrderBy(DisputeSortProperty.Id, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, new DateTime(2020, 1, 1))
                .And(SearchCriteria.DisputeStage, DisputeStage.Chargeback)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
            Assert.IsTrue(result.Results.TrueForAll(d => d.CaseStage == stage));
        }

        [TestMethod]
        public void ReportFindSettlementDisputesPaged_By_From_And_To_Stage_Time_Created() {
            var startDate = new DateTime(2020, 1, 1);
            var endDate = DateTime.Now;
            PagedResult<DisputeSummary> result = ReportingService.FindSettlementDisputesPaged(1, 10)
                .OrderBy(DisputeSortProperty.Id, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, startDate)
                .And(DataServiceCriteria.EndStageDate, endDate)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
            Assert.IsTrue(result.Results.TrueForAll(d => d.CaseIdTime >= startDate.Date && d.CaseIdTime <= endDate.Date));
        }

        [TestMethod]
        public void ReportFindSettlementDisputesPaged_By_MerchantId() {
            string merchantId = "101023947262";

            PagedResult<DisputeSummary> result = ReportingService.FindSettlementDisputesPaged(1, 10)
                .OrderBy(DisputeSortProperty.Id, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, new DateTime(2020, 1, 1))
                .And(DataServiceCriteria.MerchantId, merchantId)
                .Execute();

            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
            Assert.IsTrue(result.Results.TrueForAll(d => d.CaseMerchantId == merchantId));
        }

        [TestMethod]
        public void ReportFindSettlementDisputesPaged_By_SystemHierarchy() {
            string systemHierarchy = "055-70-024-011-019";

            PagedResult<DisputeSummary> result = ReportingService.FindSettlementDisputesPaged(1, 10)
                .OrderBy(DisputeSortProperty.Id, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, new DateTime(2020, 1, 1))
                .And(DataServiceCriteria.SystemHierarchy, systemHierarchy)
                .Execute();

            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
            Assert.IsTrue(result.Results.TrueForAll(d => d.MerchantHierarchy == systemHierarchy));
        }

        [TestMethod]
        public void ReportFindSettlementDisputesPaged_By_Wrong_MerchantId() {
            string merchantId = "000023947222";

            PagedResult<DisputeSummary> result = ReportingService.FindSettlementDisputesPaged(1, 10)
                .OrderBy(DisputeSortProperty.Id, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, new DateTime(2020, 1, 1))
                .And(DataServiceCriteria.MerchantId, merchantId)
                .Execute();

            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
            Assert.IsTrue(result.Results.Count == 0);
        }

        [TestMethod]
        public void ReportFindSettlementDisputesPaged_By_Wrong_SystemHierarchy() {
            string systemHierarchy = "000-70-024-011-111";

            PagedResult<DisputeSummary> result = ReportingService.FindSettlementDisputesPaged(1, 10)
                .OrderBy(DisputeSortProperty.Id, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, new DateTime(2020, 1, 1))
                .And(DataServiceCriteria.SystemHierarchy, systemHierarchy)
                .Execute();

            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
            Assert.IsTrue(result.Results.Count == 0);
        }

        [TestMethod]
        public void ReportFindSettlementDisputesPaged_Order_By_ARN() {
            PagedResult<DisputeSummary> result = ReportingService.FindSettlementDisputesPaged(1, 10)
                .OrderBy(DisputeSortProperty.ARN, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, new DateTime(2020, 1, 1))
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindSettlementDisputesPaged_Order_By_Id_With_Status_UnderReview() {
            PagedResult<DisputeSummary> result = ReportingService.FindSettlementDisputesPaged(1, 10)
                .OrderBy(DisputeSortProperty.Id, SortDirection.Ascending)
                .Where(DataServiceCriteria.StartStageDate, new DateTime(2020, 1, 1))
                .And(SearchCriteria.DisputeStatus, DisputeStatus.UnderReview)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
        }
        #endregion
    }
}
