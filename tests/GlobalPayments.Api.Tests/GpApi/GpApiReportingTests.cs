using GlobalPayments.Api.Builders;
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
        public void ReportTransactionDetailWrongID()
        {
            string transactionId = "TRN_123456";
            try
            {
                TransactionSummary response = ReportingService.TransactionDetail(transactionId)
                .Execute();
            }
            catch (GatewayException ex)
            {
                Assert.AreEqual("RESOURCE_NOT_FOUND", ex.ResponseCode);
                Assert.AreEqual("40118", ex.ResponseMessage);
                Assert.AreEqual("Status Code: NotFound - Transactions TRN_123456 not found at this /ucp/transactions/TRN_123456", ex.Message);
            }
        }

        [TestMethod]
        public void ReportFindTransactions_By_StartDate_And_EndDate() {
            DateTime startDate = DateTime.UtcNow.AddDays(-30);
            DateTime endDate = DateTime.UtcNow.AddDays(-10);
            List<TransactionSummary> transactions = ReportingService.FindTransactions()
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(SearchCriteria.StartDate, startDate)
                .And(SearchCriteria.EndDate, endDate)
                .Execute();
            Assert.IsNotNull(transactions);
            Assert.IsTrue(transactions is List<TransactionSummary>);
            Assert.IsTrue(transactions.TrueForAll(t => t.TransactionDate?.Date >= startDate.Date && t.TransactionDate?.Date <= endDate.Date));
        }

        [TestMethod]
        public void ReportFindTransactions_By_Id() {
            string transactionId = "TRN_Q1PBfsrhwhzvsbkcm9jI5iZ9mHVmvC";
            List<TransactionSummary> transactions = ReportingService.FindTransactions()
                .WithTransactionId(transactionId)
                .Execute();
            Assert.IsNotNull(transactions);
            Assert.IsTrue(transactions is List<TransactionSummary>);
            Assert.IsTrue(transactions.Count == 1);
            Assert.IsTrue(transactions.TrueForAll(t => t.TransactionId == transactionId));
        }

        [TestMethod]
        public void ReportFindTransactions_WrongId()
        {
            string transactionId = "TRN_B2RDfsrhwhzvsbkci4JdTiZ9mHVmvC";
            List<TransactionSummary> transactions = ReportingService.FindTransactions()
                .WithTransactionId(transactionId)
                .Execute();
            Assert.IsNotNull(transactions);
            Assert.IsTrue(transactions is List<TransactionSummary>);
            Assert.IsTrue(transactions.Count == 0);
        }

        [TestMethod]
        public void ReportFindTransactions_By_BatchId() {
            string batchId = "BAT_845591";
            List<TransactionSummary> transactions = ReportingService.FindTransactions()
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(SearchCriteria.BatchId, batchId)
                .Execute();
            Assert.IsNotNull(transactions);
            Assert.IsTrue(transactions is List<TransactionSummary>);
            Assert.IsTrue(transactions.TrueForAll(t => t.BatchSequenceNumber == batchId));
        }

        [TestMethod]
        public void ReportFindTransactions_By_Type() {
            var paymentType = PaymentType.Sale;
            List<TransactionSummary> transactions = ReportingService.FindTransactions()
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(SearchCriteria.PaymentType, paymentType)
                .Execute();
            Assert.IsNotNull(transactions);
            Assert.IsTrue(transactions is List<TransactionSummary>);
            Assert.IsTrue(transactions.TrueForAll(t => t.TransactionType == EnumConverter.GetMapping(Target.GP_API, paymentType)));
        }

        [TestMethod]
        public void ReportFindTransactions_By_Amount_And_Currency_And_Country() {
            decimal amount = 1.12M;
            string currency = "aud"; //This is case sensitive
            string country = "AU"; //This is case sensitive
            List<TransactionSummary> transactions = ReportingService.FindTransactions()
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(DataServiceCriteria.Amount, amount)
                .And(DataServiceCriteria.Currency, currency)
                .And(DataServiceCriteria.Country, country)
                .Execute();
            Assert.IsNotNull(transactions);
            Assert.IsTrue(transactions is List<TransactionSummary>);
            Assert.IsTrue(transactions.TrueForAll(t => t.Amount == amount && t.Currency == currency && t.Country == country));
        }

        [TestMethod]
        public void ReportFindTransactions_By_Channel() {
            var channel = Channel.CardNotPresent;
            List<TransactionSummary> transactions = ReportingService.FindTransactions()
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(SearchCriteria.Channel, channel)
                .Execute();
            Assert.IsNotNull(transactions);
            Assert.IsTrue(transactions is List<TransactionSummary>);
            Assert.IsTrue(transactions.TrueForAll(t => t.Channel == EnumConverter.GetMapping(Target.GP_API, channel)));
        }

        [TestMethod]
        public void ReportFindTransactions_By_Status() {
            var transactionStatus = TransactionStatus.Captured;
            List<TransactionSummary> transactions = ReportingService.FindTransactions()
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(SearchCriteria.TransactionStatus, transactionStatus)
                .Execute();
            Assert.IsNotNull(transactions);
            Assert.IsTrue(transactions is List<TransactionSummary>);
            Assert.IsTrue(transactions.TrueForAll(t => t.TransactionStatus == EnumConverter.GetMapping(Target.GP_API, transactionStatus)));
        }

        [TestMethod]
        public void ReportFindTransactions_By_CardBrand_And_AuthCode() {
            string cardBrand = "VISA";
            string authCode = "12345";
            List<TransactionSummary> transactions = ReportingService.FindTransactions()
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(SearchCriteria.CardBrand, cardBrand)
                .And(SearchCriteria.AuthCode, authCode)
                .Execute();
            Assert.IsNotNull(transactions);
            Assert.IsTrue(transactions is List<TransactionSummary>);
            Assert.IsTrue(transactions.TrueForAll(t => t.CardType == cardBrand && t.AuthCode == authCode));
        }

        [TestMethod]
        public void ReportFindTransactions_By_Reference() {
            string referenceNumber = "e1f2f968-e9cc-45b2-b41f-61cad13754aa";
            List<TransactionSummary> transactions = ReportingService.FindTransactions()
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(SearchCriteria.ReferenceNumber, referenceNumber)
                .Execute();
            Assert.IsNotNull(transactions);
            Assert.IsTrue(transactions is List<TransactionSummary>);
            Assert.IsTrue(transactions.TrueForAll(t => t.ReferenceNumber == referenceNumber));
        }

        [TestMethod]
        public void ReportFindTransactions_By_BrandReference() {
            string brandReference = "D5v2Nv8h91Me3DTh";
            List<TransactionSummary> transactions = ReportingService.FindTransactions()
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(SearchCriteria.BrandReference, brandReference)
                .Execute();
            Assert.IsNotNull(transactions);
            Assert.IsTrue(transactions is List<TransactionSummary>);
            Assert.IsTrue(transactions.TrueForAll(t => t.BrandReference == brandReference));
        }

        [TestMethod]
        public void ReportFindTransactions_By_EntryMode() {
            var paymentEntryMode = PaymentEntryMode.Ecom;
            List<TransactionSummary> transactions = ReportingService.FindTransactions()
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(SearchCriteria.PaymentEntryMode, paymentEntryMode)
                .Execute();
            Assert.IsNotNull(transactions);
            Assert.IsTrue(transactions is List<TransactionSummary>);
            Assert.IsTrue(transactions.TrueForAll(t => t.EntryMode == EnumConverter.GetMapping(Target.GP_API, paymentEntryMode)));
        }

        [TestMethod]
        public void ReportFindTransactions_By_Number_First6_And_Last4()
        {
            var firstSix = "426397";
            var lastFour = "5262";
            List<TransactionSummary> transactions = ReportingService.FindTransactions()
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(SearchCriteria.CardNumberFirstSix, firstSix)
                .And(SearchCriteria.CardNumberLastFour, lastFour)
                .Execute();
            Assert.IsNotNull(transactions);
            Assert.IsTrue(transactions is List<TransactionSummary>);
            Assert.IsTrue(transactions.TrueForAll(t => t.MaskedCardNumber.StartsWith(firstSix) &&
            t.MaskedCardNumber.EndsWith(lastFour)));
        }

        [TestMethod]
        public void ReportFindTransactions_By_Token_First6_And_Last4()
        {
            var firstSix = "426397";
            var lastFour = "5262";
            List<TransactionSummary> transactions = ReportingService.FindTransactions()
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(SearchCriteria.TokenFirstSix, firstSix)
                .And(SearchCriteria.TokenLastFour, lastFour)
                .Execute();
            Assert.IsNotNull(transactions);
            Assert.IsTrue(transactions is List<TransactionSummary>);
            Assert.IsTrue(transactions.TrueForAll(t => t.TokenPanLastFour.Contains(lastFour)));
        }

        [TestMethod]
        public void ReportFindTransactions_By_Name() {
            var name = "James Mason";
            List<TransactionSummary> transactions = ReportingService.FindTransactions()
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Ascending)
                .WithPaging(1, 10)
                .Where(SearchCriteria.Name, name)
                .Execute();
            Assert.IsNotNull(transactions);
            Assert.IsTrue(transactions is List<TransactionSummary>);
            Assert.IsTrue(transactions.TrueForAll(t => t.CardHolderName == name));
        }

        [TestMethod]
        public void ReportFindTransactions_Order_By_Status()
        {
            DateTime startDate = DateTime.UtcNow.AddDays(-30);
            DateTime endDate = DateTime.UtcNow.AddDays(-10);
            List<TransactionSummary> transactions = ReportingService.FindTransactions()
                .OrderBy(TransactionSortProperty.Status, SortDirection.Ascending)
                .WithPaging(1, 10)
                .Where(SearchCriteria.StartDate, startDate)
                .And(SearchCriteria.EndDate, endDate)
                .Execute();
            Assert.IsNotNull(transactions);
            Assert.IsTrue(transactions.Count > 0);
            Assert.IsTrue(transactions is List<TransactionSummary>);
        }

        [TestMethod]
        public void ReportFindTransactions_Order_By_Type()
        {
            DateTime startDate = DateTime.UtcNow.AddDays(-30);
            DateTime endDate = DateTime.UtcNow.AddDays(-10);
            List<TransactionSummary> transactions = ReportingService.FindTransactions()
                .OrderBy(TransactionSortProperty.Type, SortDirection.Ascending)
                .WithPaging(1, 10)
                .Where(SearchCriteria.StartDate, startDate)
                .And(SearchCriteria.EndDate, endDate)
                .Execute();
            Assert.IsNotNull(transactions);
            Assert.IsTrue(transactions.Count > 0);
            Assert.IsTrue(transactions is List<TransactionSummary>);
        }

        [TestMethod]
        public void ReportFindTransactions_Order_By_DepositId()
        {
            DateTime startDate = DateTime.UtcNow.AddDays(-30);
            DateTime endDate = DateTime.UtcNow.AddDays(-10);
            List<TransactionSummary> transactions = ReportingService.FindTransactions()
                .OrderBy(TransactionSortProperty.DepositId, SortDirection.Ascending)
                .WithPaging(1, 10)
                .Where(SearchCriteria.StartDate, startDate)
                .And(SearchCriteria.EndDate, endDate)
                .Execute();
            Assert.IsNotNull(transactions);
            Assert.IsTrue(transactions.Count > 0);
            Assert.IsTrue(transactions is List<TransactionSummary>);
        }

        [TestMethod]
        public void CompareResults_reportFindTransactions_OrderBy_TypeAndTimeCreated()
        {
            DateTime startDate = DateTime.UtcNow.AddDays(-30);
            DateTime endDate = DateTime.UtcNow.AddDays(-10);

            List<TransactionSummary> transactionsByTimeCreated = ReportingService.FindTransactions()
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Ascending)
                .WithPaging(1, 10)
                .Where(SearchCriteria.StartDate, startDate)
                .And(SearchCriteria.EndDate, endDate)
                .Execute();

            List<TransactionSummary> transactionsByType = ReportingService.FindTransactions()
                .OrderBy(TransactionSortProperty.Type, SortDirection.Ascending)
                .WithPaging(1, 10)
                .Where(SearchCriteria.StartDate, startDate)
                .And(SearchCriteria.EndDate, endDate)
                .Execute();

            Assert.IsNotNull(transactionsByTimeCreated);
            Assert.IsTrue(transactionsByTimeCreated.Count > 0);
            Assert.IsNotNull(transactionsByType);
            Assert.IsTrue(transactionsByType.Count > 0);

            Assert.AreNotEqual(transactionsByTimeCreated, transactionsByType);
        }

        [TestMethod]
        public void ReportFindTransactions_OrderBy_TimeCreated_Ascending() {
            List<TransactionSummary> transactions = ReportingService.FindTransactions()
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Ascending)
                .WithPaging(1, 25)
                .Execute();
            Assert.IsNotNull(transactions);
            Assert.IsTrue(transactions is List<TransactionSummary>);
            Assert.IsTrue(transactions.SequenceEqual(transactions.OrderBy(t => t.TransactionDate)));
        }

        [TestMethod]
        public void ReportFindTransactions_OrderBy_TimeCreated_Descending() {
            List<TransactionSummary> transactions = ReportingService.FindTransactions()
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .WithPaging(1, 25)
                .Execute();
            Assert.IsNotNull(transactions);
            Assert.IsTrue(transactions is List<TransactionSummary>);
            Assert.IsTrue(transactions.SequenceEqual(transactions.OrderByDescending(t => t.TransactionDate)));
        }

        [TestMethod]
        public void ReportFindTransactions_OrderBy_Status_Ascending() {
            List<TransactionSummary> transactions = ReportingService.FindTransactions()
                .OrderBy(TransactionSortProperty.Status, SortDirection.Ascending)
                .WithPaging(1, 25)
                .Execute();
            Assert.IsNotNull(transactions);
            Assert.IsTrue(transactions is List<TransactionSummary>);
            Assert.IsTrue(transactions.SequenceEqual(transactions.OrderBy(t => t.Status)));
        }

        [TestMethod]
        public void ReportFindTransactions_OrderBy_Status_Descending() {
            List<TransactionSummary> transactions = ReportingService.FindTransactions()
                .OrderBy(TransactionSortProperty.Status, SortDirection.Descending)
                .WithPaging(1, 25)
                .Execute();
            Assert.IsNotNull(transactions);
            Assert.IsTrue(transactions is List<TransactionSummary>);
            Assert.IsTrue(transactions.SequenceEqual(transactions.OrderByDescending(t => t.Status)));
        }

        [TestMethod]
        public void ReportFindTransactions_OrderBy_Type_Ascending() {
            List<TransactionSummary> transactions = ReportingService.FindTransactions()
                .OrderBy(TransactionSortProperty.Type, SortDirection.Ascending)
                .WithPaging(1, 25)
                .Execute();
            Assert.IsNotNull(transactions);
            Assert.IsTrue(transactions is List<TransactionSummary>);
            Assert.IsTrue(transactions.SequenceEqual(transactions.OrderBy(t => t.TransactionType)));
        }

        [TestMethod]
        public void ReportFindTransactions_OrderBy_Type_Descending() {
            List<TransactionSummary> transactions = ReportingService.FindTransactions()
                .OrderBy(TransactionSortProperty.Type, SortDirection.Descending)
                .WithPaging(1, 25)
                .Execute();
            Assert.IsNotNull(transactions);
            Assert.IsTrue(transactions is List<TransactionSummary>);
            Assert.IsTrue(transactions.SequenceEqual(transactions.OrderByDescending(t => t.TransactionType)));
        }

        [TestMethod]
        public void ReportFindTransactions_Without_Mandatory_StartDate()
        {
            List<TransactionSummary> transactions = ReportingService.FindTransactions()
                .OrderBy(TransactionSortProperty.Type, SortDirection.Ascending)
                .WithPaging(1, 10)
                .Execute();

            Assert.IsNotNull(transactions);
            Assert.IsTrue(transactions.Count > 0);
        }
        #endregion

        #region Settlement Transactions
        [TestMethod]
        public void ReportFindSettlementTransactions_By_StartDate_And_EndDate() {
            DateTime startDate = DateTime.UtcNow.AddDays(-30);
            DateTime endDate = DateTime.UtcNow.AddDays(-10);
            List<TransactionSummary> transactions = ReportingService.FindSettlementTransactions()
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(SearchCriteria.StartDate, startDate)
                .And(SearchCriteria.EndDate, endDate)
                .Execute();
            Assert.IsNotNull(transactions);
            Assert.IsTrue(transactions is List<TransactionSummary>);
            Assert.IsTrue(transactions.TrueForAll(t => t.TransactionDate?.Date >= startDate.Date && t.TransactionDate?.Date <= endDate.Date));
        }

        [TestMethod]
        public void ReportFindSettlementTransactions_OrderBy_TimeCreated()
        {
            DateTime startDate = DateTime.UtcNow.AddDays(-30);
            List<TransactionSummary> transactions = ReportingService.FindSettlementTransactions()
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(SearchCriteria.StartDate, startDate)
                .Execute();
            Assert.IsNotNull(transactions);
            Assert.IsTrue(transactions is List<TransactionSummary>);
        }

        [TestMethod]
        public void ReportFindSettlementTransactions_OrderBy_Status()
        {
            DateTime startDate = DateTime.UtcNow.AddDays(-30);

            List<TransactionSummary> transactions = ReportingService.FindSettlementTransactions()
                .OrderBy(TransactionSortProperty.Status, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(SearchCriteria.StartDate, startDate)
                .Execute();
            Assert.IsNotNull(transactions);
            Assert.IsTrue(transactions is List<TransactionSummary>);
        }

        [TestMethod]
        public void ReportFindSettlementTransactions_OrderBy_Type()
        {
            DateTime startDate = DateTime.UtcNow.AddDays(-30);

            List<TransactionSummary> transactions = ReportingService.FindSettlementTransactions()
                .OrderBy(TransactionSortProperty.Type, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(SearchCriteria.StartDate, startDate)
                .Execute();
            Assert.IsNotNull(transactions);
            Assert.IsTrue(transactions is List<TransactionSummary>);
        }

        [TestMethod]
        public void ReportFindSettlementTransactions_OrderBy_DepositId()
        {
            DateTime startDate = DateTime.UtcNow.AddDays(-30);

            List<TransactionSummary> transactions = ReportingService.FindSettlementTransactions()
                .OrderBy(TransactionSortProperty.DepositId, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(SearchCriteria.StartDate, startDate)
                .Execute();
            Assert.IsNotNull(transactions);
            Assert.IsTrue(transactions is List<TransactionSummary>);
        }

        [TestMethod]
        public void CompareResults_reportFindSettlementTransactions_OrderBy_TypeAndTimeCreated()
        {
            DateTime startDate = DateTime.UtcNow.AddDays(-30);

            List<TransactionSummary> transactionsByType = ReportingService.FindSettlementTransactions()
                .OrderBy(TransactionSortProperty.Type, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(SearchCriteria.StartDate, startDate)
                .Execute();

            List<TransactionSummary> transactionsByTime = ReportingService.FindSettlementTransactions()
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(SearchCriteria.StartDate, startDate)
                .Execute();

            Assert.IsNotNull(transactionsByType);
            Assert.IsTrue(transactionsByType is List<TransactionSummary>);
            Assert.IsNotNull(transactionsByTime);
            Assert.IsTrue(transactionsByTime is List<TransactionSummary>);
            Assert.AreNotEqual(transactionsByType, transactionsByTime);
        }

        [TestMethod]
        public void ReportFindSettlementTransactions_By_Number_First6_And_Number_Last4()
        {
            String firstSix = "543458";
            String lastFour = "7652";
            DateTime startDate = DateTime.UtcNow.AddDays(-30);
            List<TransactionSummary> transactions = ReportingService.FindSettlementTransactions()
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(SearchCriteria.StartDate, startDate)
                .And(SearchCriteria.CardNumberFirstSix, firstSix)
                .And(SearchCriteria.CardNumberLastFour, lastFour)
                .Execute();
            Assert.IsNotNull(transactions);
            Assert.IsTrue(transactions is List<TransactionSummary>);
            Assert.IsTrue(transactions.TrueForAll(t => t.MaskedCardNumber.StartsWith(firstSix) &&
            t.MaskedCardNumber.EndsWith(lastFour)));
        }

        [TestMethod]
        public void ReportFindSettlementTransactions_By_CardBrand()
        {
            String cardBrand = "MASTERCARD";

            DateTime startDate = DateTime.UtcNow.AddDays(-30);
            List<TransactionSummary> transactions = ReportingService.FindSettlementTransactions()
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(SearchCriteria.StartDate, startDate)
                .And(SearchCriteria.CardBrand, cardBrand)
                .Execute();
            Assert.IsNotNull(transactions);
            Assert.IsTrue(transactions is List<TransactionSummary>);
            Assert.IsTrue(transactions.TrueForAll(t => t.CardType == cardBrand));
        }

        [TestMethod]
        public void ReportFindSettlementTransactions_By_InvalidCardBrand()
        {
            String cardBrand = "MASTER";

            DateTime startDate = DateTime.UtcNow.AddDays(-30);
            List<TransactionSummary> transactions = ReportingService.FindSettlementTransactions()
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(SearchCriteria.StartDate, startDate)
                .And(SearchCriteria.CardBrand, cardBrand)
                .Execute();
            Assert.IsNotNull(transactions);
            Assert.IsTrue(transactions is List<TransactionSummary>);
            Assert.IsTrue(transactions.Count == 0);
        }

        [TestMethod]
        public void ReportFindSettlementTransactions_By_DepositStatus()
        {
            var depositStatus = DepositStatus.Delayed;
            DateTime startDate = DateTime.UtcNow.AddDays(-30);
            List<TransactionSummary> transactions = ReportingService.FindSettlementTransactions()
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(SearchCriteria.StartDate, startDate)
                .And(SearchCriteria.DepositStatus, depositStatus)
                .Execute();
            Assert.IsNotNull(transactions);
            Assert.IsTrue(transactions is List<TransactionSummary>);
        }

        [TestMethod]
        public void ReportFindSettlementTransactions_By_ARN()
        {
            String arn = "74500010037624410827759";

            DateTime startDate = DateTime.UtcNow.AddDays(-30);
            List<TransactionSummary> transactions = ReportingService.FindSettlementTransactions()
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(SearchCriteria.StartDate, startDate)
                .And(SearchCriteria.AquirerReferenceNumber, arn)
                .Execute();
            Assert.IsNotNull(transactions);
            Assert.IsTrue(transactions is List<TransactionSummary>);
            Assert.IsTrue(transactions.TrueForAll(t => t.AquirerReferenceNumber == arn));
        }

        [TestMethod]
        public void ReportFindSettlementTransactions_By_WrongARN()
        {
            String arn = "00000010037624410827527";

            DateTime startDate = DateTime.UtcNow.AddDays(-30);
            List<TransactionSummary> transactions = ReportingService.FindSettlementTransactions()
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(SearchCriteria.StartDate, startDate)
                .And(SearchCriteria.AquirerReferenceNumber, arn)
                .Execute();
            Assert.IsNotNull(transactions);
            Assert.IsTrue(transactions is List<TransactionSummary>);
            Assert.IsTrue(transactions.Count == 0);
        }

        [TestMethod]
        public void ReportFindSettlementTransactions_By_BrandReference()
        {
            String brandReference = "MCF1CZ5ME5405";

            DateTime startDate = DateTime.UtcNow.AddDays(-30);
            List<TransactionSummary> transactions = ReportingService.FindSettlementTransactions()
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(SearchCriteria.StartDate, startDate)
                .And(SearchCriteria.BrandReference, brandReference)
                .Execute();
            Assert.IsNotNull(transactions);
            Assert.IsTrue(transactions is List<TransactionSummary>);
            Assert.IsTrue(transactions.TrueForAll(t => t.BrandReference == brandReference));
        }

        [TestMethod]
        public void ReportFindSettlementTransactions_By_WrongBrandReference()
        {
            String brandReference = "MCF1CZ5ME5001";

            DateTime startDate = DateTime.UtcNow.AddDays(-30);
            List<TransactionSummary> transactions = ReportingService.FindSettlementTransactions()
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(SearchCriteria.StartDate, startDate)
                .And(SearchCriteria.BrandReference, brandReference)
                .Execute();
            Assert.IsNotNull(transactions);
            Assert.IsTrue(transactions is List<TransactionSummary>);
            Assert.IsTrue(transactions.Count == 0);
        }

        [TestMethod]
        public void ReportFindSettlementTransactions_By_CardBrand_And_AuthCode()
        {
            String cardBrand = "MASTERCARD";
            String authCode = "028010";

            DateTime startDate = DateTime.UtcNow.AddDays(-30);
            List<TransactionSummary> transactions = ReportingService.FindSettlementTransactions()
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(SearchCriteria.StartDate, startDate)
                .And(SearchCriteria.CardBrand, cardBrand)
                .And(SearchCriteria.AuthCode, authCode)
                .Execute();
            Assert.IsNotNull(transactions);
            Assert.IsTrue(transactions is List<TransactionSummary>);
            Assert.IsTrue(transactions.TrueForAll(t => t.CardType == cardBrand));
            Assert.IsTrue(transactions.TrueForAll(t => t.AuthCode == authCode));
        }

        [TestMethod]
        public void ReportFindSettlementTransactions_By_Reference()
        {
            String reference = "50080513769";

            DateTime startDate = DateTime.UtcNow.AddDays(-30);
            List<TransactionSummary> transactions = ReportingService.FindSettlementTransactions()
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(SearchCriteria.StartDate, startDate)
                .And(SearchCriteria.ReferenceNumber, reference)
                .Execute();
            Assert.IsNotNull(transactions);
            Assert.IsTrue(transactions is List<TransactionSummary>);
            Assert.IsTrue(transactions.TrueForAll(t => t.ReferenceNumber == reference));
        }

        [TestMethod]
        public void ReportFindSettlementTransactions_By_RandomReference()
        {
            String reference = Guid.NewGuid().ToString();

            DateTime startDate = DateTime.UtcNow.AddDays(-30);
            List<TransactionSummary> transactions = ReportingService.FindSettlementTransactions()
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(SearchCriteria.StartDate, startDate)
                .And(SearchCriteria.ReferenceNumber, reference)
                .Execute();
            
            Assert.IsNotNull(transactions);
            Assert.IsTrue(transactions is List<TransactionSummary>);
            Assert.IsTrue(transactions.Count == 0);
        }

        [TestMethod]
        public void ReportFindSettlementTransactions_By_Status()
        {
            var status = TransactionStatus.Rejected;

            DateTime startDate = DateTime.UtcNow.AddDays(-30);
            List<TransactionSummary> transactions = ReportingService.FindSettlementTransactions()
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(SearchCriteria.StartDate, startDate)
                .And(SearchCriteria.TransactionStatus, status)
                .Execute();
            Assert.IsNotNull(transactions);
            Assert.IsTrue(transactions is List<TransactionSummary>);
            Assert.IsTrue(transactions.TrueForAll(t => t.Status == status.ToString()));
        }

        [TestMethod]
        public void ReportFindSettlementTransactions_By_DepositId()
        {
            var depositId = "DEP_2342423429";

            DateTime startDate = DateTime.UtcNow.AddDays(-30);
            List<TransactionSummary> transactions = ReportingService.FindSettlementTransactions()
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(SearchCriteria.StartDate, startDate)
                .And(DataServiceCriteria.DepositReference, depositId)
                .Execute();
            Assert.IsNotNull(transactions);
            Assert.IsTrue(transactions is List<TransactionSummary>);
            Assert.IsTrue(transactions.TrueForAll(t => t.DepositReference == depositId));
        }

        [TestMethod]
        public void ReportFindSettlementTransactions_By_RandomDepositId()
        {
            var depositId = Guid.NewGuid().ToString();

            DateTime startDate = DateTime.UtcNow.AddDays(-30);
            List<TransactionSummary> transactions = ReportingService.FindSettlementTransactions()
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(SearchCriteria.StartDate, startDate)
                .And(DataServiceCriteria.DepositReference, depositId)
                .Execute();
            Assert.IsNotNull(transactions);
            Assert.IsTrue(transactions is List<TransactionSummary>);
            Assert.IsTrue(transactions.Count == 0);
        }

        [TestMethod]
        public void ReportFindSettlementTransactions_By_FromDepositTimeCreated_And_ToDepositTimeCreated()
        {
            DateTime startDate = DateTime.UtcNow.AddDays(-90);
            DateTime startDateDeposit = DateTime.UtcNow.AddDays(-89);
            DateTime endDateDeposit = DateTime.UtcNow.AddDays(-1);

            List<TransactionSummary> transactions = ReportingService.FindSettlementTransactions()
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(SearchCriteria.StartDate, startDate)
                .And(DataServiceCriteria.StartDepositDate, startDateDeposit)
                .And(DataServiceCriteria.EndDepositDate, endDateDeposit)
                .Execute();
            Assert.IsNotNull(transactions);
            Assert.IsTrue(transactions is List<TransactionSummary>);
            Assert.IsTrue(transactions.TrueForAll(t => t.DepositDate?.Date >= startDateDeposit.Date && t.DepositDate?.Date <= endDateDeposit.Date));
        }

        [TestMethod]
        public void ReportFindSettlementTransactions_By_FromBatchTimeCreated_And_ToBatchTimeCreated()
        {
            DateTime startDate = DateTime.UtcNow.AddDays(-90);
            DateTime startBatchDate = DateTime.UtcNow.AddDays(-89);
            DateTime endBatchDate = DateTime.UtcNow.AddDays(-1);

            List<TransactionSummary> transactions = ReportingService.FindSettlementTransactions()
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(SearchCriteria.StartDate, startDate)
                .And(DataServiceCriteria.StartBatchDate, startBatchDate)
                .And(DataServiceCriteria.EndBatchDate, endBatchDate)
                .Execute();
            Assert.IsNotNull(transactions);
            Assert.IsTrue(transactions is List<TransactionSummary>);
            Assert.IsTrue(transactions.TrueForAll(t => t.BatchCloseDate?.Date >= startBatchDate.Date 
            && t.BatchCloseDate?.Date <= endBatchDate.Date));
        }

        [TestMethod]
        public void ReportFindSettlementTransactions_By_SystemMid_And_SystemHierarchy()
        {
            String systemMid = "101023947262";
            String systemHierarchy = "055-70-024-011-019";

            DateTime startDate = DateTime.UtcNow.AddDays(-30);
            List<TransactionSummary> transactions = ReportingService.FindSettlementTransactions()
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(SearchCriteria.StartDate, startDate)
                .And(DataServiceCriteria.MerchantId, systemMid)
                .And(DataServiceCriteria.SystemHierarchy, systemHierarchy)
                .Execute();
            Assert.IsNotNull(transactions);
            Assert.IsTrue(transactions is List<TransactionSummary>);
            Assert.IsTrue(transactions.TrueForAll(t => t.MerchantId == systemMid 
            && t.MerchantHierarchy == systemHierarchy));
        }

        [TestMethod]
        public void ReportFindSettlementTransactions_By_NonExistent_SystemMid()
        {
            String systemMid = "100023947222";

            DateTime startDate = DateTime.UtcNow.AddDays(-30);
            List<TransactionSummary> transactions = ReportingService.FindSettlementTransactions()
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(SearchCriteria.StartDate, startDate)
                .And(DataServiceCriteria.MerchantId, systemMid)
                .Execute();
            Assert.IsNotNull(transactions);
            Assert.IsTrue(transactions is List<TransactionSummary>);
            Assert.IsTrue(transactions.Count == 0);
        }

        [TestMethod]
        public void ReportFindSettlementTransactions_By_Random_SystemHierarchy()
        {
            String systemHierarchy = Guid.NewGuid().ToString();

            DateTime startDate = DateTime.UtcNow.AddDays(-30);
            try
            {
                ReportingService.FindSettlementTransactions()
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(SearchCriteria.StartDate, startDate)
                .And(DataServiceCriteria.SystemHierarchy, systemHierarchy)
                .Execute();
            } catch(GatewayException ex)
            {
                Assert.AreEqual("INVALID_REQUEST_DATA", ex.ResponseCode);
                Assert.AreEqual("40105", ex.ResponseMessage);
                Assert.IsTrue(ex.Message.Equals("Status Code: BadRequest - " +
                    "Invalid Value provided in the input field - system.hierarchy"));
            }
        }

        [TestMethod]
        public void ReportFindSettlementTransactions_By_Invalid_SystemMid()
        {
            String systemMid = Guid.NewGuid().ToString();

            DateTime startDate = DateTime.UtcNow.AddDays(-30);
            try
            {
                ReportingService.FindSettlementTransactions()
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(SearchCriteria.StartDate, startDate)
                .And(DataServiceCriteria.MerchantId, systemMid)
                .Execute();
            }
            catch (GatewayException ex)
            {
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
        public void ReportDepositDetailWrongId()
        {
            string depositId = "DEP_1112423111";
            try
            {
                DepositSummary response = ReportingService.DepositDetail(depositId)
                .Execute();
            }
            catch (GatewayException ex)
            {
                Assert.AreEqual("RESOURCE_NOT_FOUND", ex.ResponseCode);
                Assert.AreEqual("40118", ex.ResponseMessage);
                Assert.IsTrue(ex.Message.StartsWith("Status Code: NotFound - Deposits DEP_1112423111 not found"));
            }
        }

        [TestMethod]
        public void ReportFindDeposits_By_StartDate_Order_By_TimeCreated() {
            DateTime startDate = DateTime.UtcNow.AddDays(-30);
            List<DepositSummary> deposits = ReportingService.FindDeposits()
                .OrderBy(DepositSortProperty.TimeCreated, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(SearchCriteria.StartDate, startDate)
                .Execute();
            Assert.IsNotNull(deposits);
            Assert.IsTrue(deposits is List<DepositSummary>);
            Assert.IsTrue(deposits.TrueForAll(d => d.DepositDate?.Date >= startDate.Date));
        }

        [TestMethod]
        public void ReportFindDeposits_By_EndDate_Order_By_TimeCreated()
        {
            DateTime startDate = DateTime.UtcNow.AddDays(-300);
            DateTime endDate = DateTime.UtcNow;
            List<DepositSummary> deposits = ReportingService.FindDeposits()
                .OrderBy(DepositSortProperty.TimeCreated, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(SearchCriteria.StartDate, startDate)
                .And(SearchCriteria.EndDate, endDate)
                .Execute();
            Assert.IsNotNull(deposits);
            Assert.IsTrue(deposits is List<DepositSummary>);
            Assert.IsTrue(deposits.TrueForAll(d => d.DepositDate?.Date >= startDate.Date && d.DepositDate?.Date <= endDate.Date));
        }

        [TestMethod]
        public void ReportFindDeposits_By_Amount()
        {
            decimal amount = 141;
            List<DepositSummary> deposits = ReportingService.FindDeposits()
                .OrderBy(DepositSortProperty.TimeCreated, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(DataServiceCriteria.Amount, amount)
                .Execute();
            Assert.IsNotNull(deposits);
            Assert.IsTrue(deposits is List<DepositSummary>);
            Assert.IsTrue(deposits.TrueForAll(d => d.Amount == amount));
        }

        [TestMethod]
        public void ReportFindDeposits_By_NotFoundAmount()
        {
            decimal amount = 140;
            List<DepositSummary> deposits = ReportingService.FindDeposits()
                .OrderBy(DepositSortProperty.TimeCreated, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(DataServiceCriteria.Amount, amount)
                .Execute();
            Assert.IsNotNull(deposits);
            Assert.IsTrue(deposits is List<DepositSummary>);
            Assert.IsTrue(deposits.Count==0);
        }

        [TestMethod]
        public void ReportFindDeposits_By_Status()
        {
            string depositStatus = "IRREG";
            List<DepositSummary> deposits = ReportingService.FindDeposits()
                .OrderBy(DepositSortProperty.TimeCreated, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(SearchCriteria.DepositStatus, DepositStatus.Irregular)
                .Execute();
            Assert.IsNotNull(deposits);
            Assert.IsTrue(deposits is List<DepositSummary>);
            Assert.IsTrue(deposits.TrueForAll(d => d.Status == depositStatus));
        }

        [TestMethod]
        public void ReportFindDeposits_By_Masked_Account_Number_Last4()
        {
            string masketAccountNumberLast4 = "9999";
            List<DepositSummary> deposits = ReportingService.FindDeposits()
                .OrderBy(DepositSortProperty.TimeCreated, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(SearchCriteria.AccountNumberLastFour, masketAccountNumberLast4)
                .Execute();
            Assert.IsNotNull(deposits);
            Assert.IsTrue(deposits is List<DepositSummary>);
            Assert.IsTrue(deposits.Count > 0);
        }

        [TestMethod]
        public void ReportFindDeposits_By_SystemHierarchy()
        {
            string systemHierarchy = "055-70-024-011-019";
            List<DepositSummary> deposits = ReportingService.FindDeposits()
                .OrderBy(DepositSortProperty.TimeCreated, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(DataServiceCriteria.SystemHierarchy, systemHierarchy)
                .Execute();
            Assert.IsNotNull(deposits);
            Assert.IsTrue(deposits is List<DepositSummary>);
            Assert.IsTrue(deposits.TrueForAll(d => d.MerchantHierarchy == systemHierarchy));
        }

        [TestMethod]
        public void ReportFindDeposits_By_WrongSystemHierarchy()
        {
            string systemHierarchy = "042-70-013-011-018";
            List<DepositSummary> deposits = ReportingService.FindDeposits()
                .OrderBy(DepositSortProperty.TimeCreated, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(DataServiceCriteria.SystemHierarchy, systemHierarchy)
                .Execute();
            Assert.IsNotNull(deposits);
            Assert.IsTrue(deposits is List<DepositSummary>);
            Assert.IsTrue(deposits.Count==0);
        }

        [TestMethod]
        public void ReportFindDeposits_FilterBy_RandomUUIDSystemHierarchy()
        {
            string systemHierarchy = Guid.NewGuid().ToString();
            try
            {
                ReportingService.FindDeposits()
                .OrderBy(DepositSortProperty.TimeCreated, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(DataServiceCriteria.SystemHierarchy, systemHierarchy)
                .Execute();
            } catch(GatewayException ex)
            {
                Assert.AreEqual("INVALID_REQUEST_DATA", ex.ResponseCode);
                Assert.AreEqual("40105", ex.ResponseMessage);
                Assert.IsTrue(ex.Message.Contains("Invalid Value provided in the input field - system.hierarch"));
            }
        }

        [TestMethod]
        public void ReportFindDeposits_WithoutFromTimeCreated()
        {

            string systemHierarchy = Guid.NewGuid().ToString();
            try
            {
                ReportingService.FindDeposits()
                .OrderBy(DepositSortProperty.TimeCreated, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(DataServiceCriteria.SystemHierarchy, systemHierarchy)
                .Execute();
            }
            catch (GatewayException ex)
            {
                Assert.AreEqual("INVALID_REQUEST_DATA", ex.ResponseCode);
                Assert.AreEqual("40105", ex.ResponseMessage);
                Assert.IsTrue(ex.Message.Contains("Invalid Value provided in the input field - system.hierarch"));
            }
        }

        [TestMethod]
        public void ReportFindDeposits_By_SystemMid()
        {
            string systemMid = "101023947262";
            List<DepositSummary> deposits = ReportingService.FindDeposits()
                .OrderBy(DepositSortProperty.TimeCreated, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(DataServiceCriteria.MerchantId, systemMid)
                .Execute();
            Assert.IsNotNull(deposits);
            Assert.IsTrue(deposits is List<DepositSummary>);
            Assert.IsTrue(deposits.TrueForAll(d => d.MerchantNumber == systemMid));
        }

        [TestMethod]
        public void ReportFindDeposits_By_WrongSystemMid()
        {
            string systemMid = "000023985843";
            var deposits = ReportingService.FindDeposits()
                .OrderBy(DepositSortProperty.TimeCreated, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(DataServiceCriteria.MerchantId, systemMid)
                .Execute();
            Assert.IsNotNull(deposits);
            Assert.IsTrue(deposits is List<DepositSummary>);
            Assert.IsTrue(deposits.Count == 0);
        }

        [TestMethod]
        public void ReportFindDeposits_By_DepositId()
        {
            var depositId = "DEP_2342423440";
            List<DepositSummary> deposits = ReportingService.FindDeposits()
                .OrderBy(DepositSortProperty.TimeCreated, SortDirection.Descending)
                .WithPaging(1, 10)
                .WithDepositId(depositId)
                .Execute();
            Assert.IsNotNull(deposits);
            Assert.IsTrue(deposits.Count == 1);
            Assert.IsTrue(deposits.TrueForAll(d => d.DepositId == depositId));
        }

        [TestMethod]
        public void ReportFindDeposits_By_WrongDepositId()
        {

            string depositId = "DEP_1112423111";
            DateTime startDate = DateTime.UtcNow.AddDays(-30);

            List<DepositSummary> deposits = ReportingService.FindDeposits()
                .OrderBy(DepositSortProperty.TimeCreated, SortDirection.Descending)
                .WithDepositId(depositId)
                .WithPaging(1, 10)
                .Where(SearchCriteria.StartDate, startDate)
                .Execute();
            Assert.IsNotNull(deposits);
            Assert.IsTrue(deposits.Count == 0);
        }

        [TestMethod]
        public void ReportFindDeposits_Order_By_DepositId() {
            List<DepositSummary> deposits = ReportingService.FindDeposits()
                .OrderBy(DepositSortProperty.DepositId, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(SearchCriteria.StartDate, DateTime.UtcNow.AddDays(-30))
                .Execute();
            Assert.IsNotNull(deposits);
            Assert.IsTrue(deposits is List<DepositSummary>);
        }

        [TestMethod]
        public void ReportFindDeposits_Order_By_Status() {
            List<DepositSummary> deposits = ReportingService.FindDeposits()
                .OrderBy(DepositSortProperty.Status, SortDirection.Ascending)
                .WithPaging(1, 10)
                .Where(SearchCriteria.StartDate, DateTime.UtcNow.AddDays(-30))
                .Execute();
            Assert.IsNotNull(deposits);
            Assert.IsTrue(deposits is List<DepositSummary>);
        }

        [TestMethod]
        public void ReportFindDeposits_Order_By_Type() {
            List<DepositSummary> deposits = ReportingService.FindDeposits()
                .OrderBy(DepositSortProperty.Type, SortDirection.Ascending)
                .WithPaging(1, 10)
                .Where(SearchCriteria.StartDate, DateTime.UtcNow.AddDays(-30))
                .Execute();
            Assert.IsNotNull(deposits);
            Assert.IsTrue(deposits is List<DepositSummary>);
        }

        [TestMethod]
        public void CompareResults_reportFindDepositsWithCriteria_OrderBy_DepositId_And_Type()
        {
            List<DepositSummary> depositsById = ReportingService.FindDeposits()
                .OrderBy(DepositSortProperty.DepositId, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(SearchCriteria.StartDate, DateTime.UtcNow.AddDays(-30))
                .Execute();

            List<DepositSummary> depositsByType = ReportingService.FindDeposits()
                .OrderBy(DepositSortProperty.Type, SortDirection.Ascending)
                .WithPaging(1, 10)
                .Where(SearchCriteria.StartDate, DateTime.UtcNow.AddDays(-30))
                .Execute();

            Assert.IsNotNull(depositsById);
            Assert.IsTrue(depositsById.Count > 0);
            Assert.IsNotNull(depositsByType);
            Assert.IsTrue(depositsByType.Count > 0);

            Assert.AreNotEqual(depositsById, depositsByType);
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
        public void ReportDisputeDetailWrongId()
        {
            string disputeId = "DIS_SAND_"+ Guid.NewGuid().ToString();
            try
            {
                ReportingService.DisputeDetail(disputeId)
                .Execute();
            } catch (GatewayException ex)
            {
                Assert.AreEqual("INVALID_REQUEST_DATA", ex.ResponseCode);
                Assert.AreEqual("40073", ex.ResponseMessage);
                Assert.IsTrue(ex.Message.Contains("Status Code: BadRequest - 101,Unable to locate dispute record for that ID. Please recheck the ID provided."));
            }
        }

        [TestMethod]
        public void ReportFindDisputes_By_ARN() {
            string arn = "135091790340196";
            List<DisputeSummary> disputes = ReportingService.FindDisputes()
                .WithPaging(1, 10)
                .Where(DataServiceCriteria.StartStageDate, new DateTime(2020, 1, 1))
                .And(SearchCriteria.AquirerReferenceNumber, arn)
                .Execute();
            Assert.IsNotNull(disputes);
            Assert.IsTrue(disputes is List<DisputeSummary>);
            Assert.IsTrue(disputes.TrueForAll(d => d.TransactionARN == arn));
        }

        [TestMethod]
        public void ReportFindDisputes_By_Brand() {
            string cardBrand = "VISA";
            List<DisputeSummary> disputes = ReportingService.FindDisputes()
                .WithPaging(1, 10)
                .Where(DataServiceCriteria.StartStageDate, new DateTime(2020, 1, 1))
                .And(SearchCriteria.CardBrand, cardBrand)
                .Execute();
            Assert.IsNotNull(disputes);
            Assert.IsTrue(disputes is List<DisputeSummary>);
            Assert.IsTrue(disputes.TrueForAll(d => d.TransactionCardType == cardBrand));
        }

        [TestMethod]
        public void ReportFindDisputes_By_Status() {
            DisputeStatus disputeStatus = DisputeStatus.UnderReview;
            List<DisputeSummary> disputes = ReportingService.FindDisputes()
                .WithPaging(1, 10)
                .Where(DataServiceCriteria.StartStageDate, new DateTime(2020, 1, 1))
                .And(SearchCriteria.DisputeStatus, disputeStatus)
                .Execute();
            Assert.IsNotNull(disputes);
            Assert.IsTrue(disputes is List<DisputeSummary>);
            Assert.IsTrue(disputes.TrueForAll(d => d.CaseStatus == EnumConverter.GetMapping(Target.GP_API, disputeStatus)));
        }

        [TestMethod]
        public void ReportFindDisputes_By_Stage() {
            DisputeStage disputeStage = DisputeStage.Chargeback;
            List<DisputeSummary> disputes = ReportingService.FindDisputes()
                .WithPaging(1, 10)
                .Where(DataServiceCriteria.StartStageDate, new DateTime(2020, 1, 1))
                .And(SearchCriteria.DisputeStage, disputeStage)
                .Execute();
            Assert.IsNotNull(disputes);
            Assert.IsTrue(disputes is List<DisputeSummary>);
            Assert.IsTrue(disputes.TrueForAll(d => d.CaseStage == EnumConverter.GetMapping(Target.GP_API, disputeStage)));
        }

        [TestMethod]
        public void ReportFindDisputes_By_From_And_To_Stage_Time_Created()
        {
            DateTime startDate = DateTime.UtcNow.AddDays(-60);
            DateTime endDate = DateTime.UtcNow;
            List<DisputeSummary> disputes = ReportingService.FindDisputes()
                .WithPaging(1, 10)
                .Where(DataServiceCriteria.StartStageDate, startDate)
                .And(DataServiceCriteria.EndStageDate, endDate)
                .Execute();
            Assert.IsNotNull(disputes);
            Assert.IsTrue(disputes is List<DisputeSummary>);
            Assert.IsTrue(disputes.TrueForAll(d => d.CaseIdTime >= startDate.Date && d.CaseIdTime <= endDate.Date));
        }

        [TestMethod]
        public void ReportFindDisputes_Filter_By_From_And_To_Adjustment_Time_Created()
        {
            DateTime startDate = new DateTime(2020, 1, 1);
            DateTime endDate = DateTime.UtcNow;
            List<DisputeSummary> disputes = ReportingService.FindDisputes()
                .WithPaging(1, 10)
                .Where(DataServiceCriteria.StartStageDate, new DateTime(2020, 1, 1))
                .And(DataServiceCriteria.StartAdjustmentDate, startDate)
                .And(DataServiceCriteria.EndAdjustmentDate, endDate)
                .Execute();
            Assert.IsNotNull(disputes);
            Assert.IsTrue(disputes is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindDisputes_By_Adjustment_Funding()
        {
            AdjustmentFunding adjustmentFunding = AdjustmentFunding.Credit;

            List<DisputeSummary> disputes = ReportingService.FindDisputes()
                .WithPaging(1, 10)
                .Where(DataServiceCriteria.StartStageDate, new DateTime(2020, 1, 1))
                .And(DataServiceCriteria.AdjustmentFunding, adjustmentFunding)
                .Execute();
            Assert.IsNotNull(disputes);
            Assert.IsTrue(disputes is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindDisputes_By_MerchantId_And_SystemHierarchy() {
            string merchantId = "8593872";
            string systemHierarchy = "111-23-099-002-005";
            List<DisputeSummary> disputes = ReportingService.FindDisputes()
                .WithPaging(1, 10)
                .Where(DataServiceCriteria.StartStageDate, new DateTime(2020, 1, 1))
                .And(DataServiceCriteria.MerchantId, merchantId)
                .And(DataServiceCriteria.SystemHierarchy, systemHierarchy)
                .Execute();
            Assert.IsNotNull(disputes);
            Assert.IsTrue(disputes is List<DisputeSummary>);
            Assert.IsTrue(disputes.TrueForAll(d => d.CaseMerchantId == merchantId && d.MerchantHierarchy == systemHierarchy));
        }

        [TestMethod]
        public void ReportFindDisputes_Order_By_Id() {
            List<DisputeSummary> disputes = ReportingService.FindDisputes()
                .OrderBy(DisputeSortProperty.Id, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(DataServiceCriteria.StartStageDate, new DateTime(2020, 1, 1))
                .Execute();
            Assert.IsNotNull(disputes);
            Assert.IsTrue(disputes is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindDisputes_Order_By_ARN() {
            List<DisputeSummary> disputes = ReportingService.FindDisputes()
                .OrderBy(DisputeSortProperty.ARN, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(DataServiceCriteria.StartStageDate, new DateTime(2020, 6, 9))
                // EndStageDate must be set in order to be able to sort by ARN
                .And(DataServiceCriteria.EndStageDate, new DateTime(2020, 6, 22))
                .Execute();
            Assert.IsNotNull(disputes);
            Assert.IsTrue(disputes is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindDisputes_Order_By_Brand() {
            List<DisputeSummary> disputes = ReportingService.FindDisputes()
                .OrderBy(DisputeSortProperty.Brand, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(DataServiceCriteria.StartStageDate, new DateTime(2020, 1, 1))
                .Execute();
            Assert.IsNotNull(disputes);
            Assert.IsTrue(disputes is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindDisputes_Order_By_Status() {
            List<DisputeSummary> disputes = ReportingService.FindDisputes()
                .OrderBy(DisputeSortProperty.Status, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(DataServiceCriteria.StartStageDate, new DateTime(2020, 1, 1))
                .Execute();
            Assert.IsNotNull(disputes);
            Assert.IsTrue(disputes is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindDisputes_Order_By_Stage() {
            List<DisputeSummary> disputes = ReportingService.FindDisputes()
                .OrderBy(DisputeSortProperty.Stage, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(DataServiceCriteria.StartStageDate, new DateTime(2020, 1, 1))
                .Execute();
            Assert.IsNotNull(disputes);
            Assert.IsTrue(disputes is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindDisputes_Order_By_FromStageTimeCreated() {
            List<DisputeSummary> disputes = ReportingService.FindDisputes()
                .OrderBy(DisputeSortProperty.FromStageTimeCreated, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(DataServiceCriteria.StartStageDate, new DateTime(2020, 1, 1))
                .Execute();
            Assert.IsNotNull(disputes);
            Assert.IsTrue(disputes is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindDisputes_Order_By_ToStageTimeCreated() {
            List<DisputeSummary> disputes = ReportingService.FindDisputes()
                .OrderBy(DisputeSortProperty.ToStageTimeCreated, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(DataServiceCriteria.StartStageDate, new DateTime(2020, 1, 1))
                .Execute();
            Assert.IsNotNull(disputes);
            Assert.IsTrue(disputes is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindDisputes_Order_By_AdjustmentFunding() {
            List<DisputeSummary> disputes = ReportingService.FindDisputes()
                .OrderBy(DisputeSortProperty.AdjustmentFunding, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(DataServiceCriteria.StartStageDate, new DateTime(2020, 1, 1))
                .Execute();
            Assert.IsNotNull(disputes);
            Assert.IsTrue(disputes is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindDisputes_Order_By_FromAdjustmentTimeCreated() {
            List<DisputeSummary> disputes = ReportingService.FindDisputes()
                .OrderBy(DisputeSortProperty.FromAdjustmentTimeCreated, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(DataServiceCriteria.StartStageDate, new DateTime(2020, 1, 1))
                .Execute();
            Assert.IsNotNull(disputes);
            Assert.IsTrue(disputes is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindDisputes_Order_By_ToAdjustmentTimeCreated() {
            List<DisputeSummary> disputes = ReportingService.FindDisputes()
                .OrderBy(DisputeSortProperty.ToAdjustmentTimeCreated, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(DataServiceCriteria.StartStageDate, new DateTime(2020, 1, 1))
                .Execute();
            Assert.IsNotNull(disputes);
            Assert.IsTrue(disputes is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindDisputes_Order_By_Id_With_Brand_VISA() {
            List<DisputeSummary> disputes = ReportingService.FindDisputes()
                .OrderBy(DisputeSortProperty.Id, SortDirection.Ascending)
                .WithPaging(1, 10)
                .Where(DataServiceCriteria.StartStageDate, new DateTime(2020, 1, 1))
                .And(SearchCriteria.CardBrand, "VISA")
                .Execute();
            Assert.IsNotNull(disputes);
            Assert.IsTrue(disputes is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindDisputes_Order_By_Id_With_Status_UnderReview() {
            List<DisputeSummary> disputes = ReportingService.FindDisputes()
                .OrderBy(DisputeSortProperty.Id, SortDirection.Ascending)
                .WithPaging(1, 10)
                .Where(DataServiceCriteria.StartStageDate, new DateTime(2020, 1, 1))
                .And(SearchCriteria.DisputeStatus, DisputeStatus.UnderReview)
                .Execute();
            Assert.IsNotNull(disputes);
            Assert.IsTrue(disputes is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindDisputes_Order_By_Id_With_Stage_Chargeback() {
            List<DisputeSummary> disputes = ReportingService.FindDisputes()
                .OrderBy(DisputeSortProperty.Id, SortDirection.Ascending)
                .WithPaging(1, 10)
                .Where(DataServiceCriteria.StartStageDate, new DateTime(2020, 1, 1))
                .And(SearchCriteria.DisputeStage, DisputeStage.Chargeback)
                .Execute();
            Assert.IsNotNull(disputes);
            Assert.IsTrue(disputes is List<DisputeSummary>);
        }

        [TestMethod]
        public void AcceptLabilityFromDispute()
        {
            //var response = TransactionType.DisputeAcceptance
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
        public void ReportSettlementDisputeDetailWrongId()
        {
            string settlementDisputeId = "OIS_111";
            try
            {
                DisputeSummary response = ReportingService.SettlementDisputeDetail(settlementDisputeId)
                .Execute();
            }
            catch (GatewayException ex)
            {
                Assert.AreEqual("RESOURCE_NOT_FOUND", ex.ResponseCode);
                Assert.AreEqual("40118", ex.ResponseMessage);
                Assert.AreEqual("Status Code: NotFound - Disputes OIS_111 not found at this " +
                    "/ucp/settlement/disputes/OIS_111?account_name=Settlement%20Reporting", ex.Message);
            }
        }

        [TestMethod]
        public void ReportFindSettlementDisputes_Order_By_Id() {
            List<DisputeSummary> summary = ReportingService.FindSettlementDisputes()
                .OrderBy(DisputeSortProperty.Id, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(DataServiceCriteria.StartStageDate, new DateTime(2020, 1, 1))
                .Execute();
            Assert.IsNotNull(summary);
            Assert.IsTrue(summary is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindSettlementDisputes_OrderBy_Brand()
        {
            List<DisputeSummary> summary = ReportingService.FindSettlementDisputes()
                .OrderBy(DisputeSortProperty.Brand, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(DataServiceCriteria.StartStageDate, new DateTime(2020, 1, 1))
                .Execute();
            Assert.IsNotNull(summary);
            Assert.IsTrue(summary is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindSettlementDisputes_OrderBy_Stage()
        {
            List<DisputeSummary> summary = ReportingService.FindSettlementDisputes()
                .OrderBy(DisputeSortProperty.Stage, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(DataServiceCriteria.StartStageDate, new DateTime(2020, 1, 1))
                .Execute();
            Assert.IsNotNull(summary);
            Assert.IsTrue(summary is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindSettlementDisputes_OrderBy_FromStageTimeCreated()
        {
            List<DisputeSummary> summary = ReportingService.FindSettlementDisputes()
                .OrderBy(DisputeSortProperty.FromStageTimeCreated, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(DataServiceCriteria.StartStageDate, new DateTime(2020, 1, 1))
                .Execute();
            Assert.IsNotNull(summary);
            Assert.IsTrue(summary is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindSettlementDisputes_OrderBy_ToStageTimeCreated()
        {
            List<DisputeSummary> summary = ReportingService.FindSettlementDisputes()
                .OrderBy(DisputeSortProperty.ToStageTimeCreated, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(DataServiceCriteria.StartStageDate, new DateTime(2020, 1, 1))
                .Execute();
            Assert.IsNotNull(summary);
            Assert.IsTrue(summary is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindSettlementDisputes_OrderBy_AdjustmentFunding()
        {
            List<DisputeSummary> summary = ReportingService.FindSettlementDisputes()
                .OrderBy(DisputeSortProperty.AdjustmentFunding, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(DataServiceCriteria.StartStageDate, new DateTime(2020, 1, 1))
                .Execute();
            Assert.IsNotNull(summary);
            Assert.IsTrue(summary is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindSettlementDisputes_OrderBy_FromAdjustmentTimeCreated()
        {
            List<DisputeSummary> summary = ReportingService.FindSettlementDisputes()
                .OrderBy(DisputeSortProperty.FromAdjustmentTimeCreated, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(DataServiceCriteria.StartStageDate, new DateTime(2020, 1, 1))
                .Execute();
            Assert.IsNotNull(summary);
            Assert.IsTrue(summary is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindSettlementDisputes_OrderBy_ToAdjustmentTimeCreated()
        {
            List<DisputeSummary> summary = ReportingService.FindSettlementDisputes()
                .OrderBy(DisputeSortProperty.ToAdjustmentTimeCreated, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(DataServiceCriteria.StartStageDate, new DateTime(2020, 1, 1))
                .Execute();
            Assert.IsNotNull(summary);
            Assert.IsTrue(summary is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindSettlementDisputes_FilterBy_ARN()
        {
            String arn = "71400011129688701392096";

            List<DisputeSummary> summary = ReportingService.FindSettlementDisputes()
                .OrderBy(DisputeSortProperty.Id, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(DataServiceCriteria.StartStageDate, new DateTime(2020, 1, 1))
                .And(SearchCriteria.AquirerReferenceNumber, arn)
                .Execute();
            Assert.IsNotNull(summary);
            Assert.IsTrue(summary is List<DisputeSummary>);
            Assert.IsTrue(summary.TrueForAll(d => d.TransactionARN == arn));
        }

        [TestMethod]
        public void ReportFindSettlementDisputes_FilterBy_ARN_NotFound()
        {
            String arn = "00000011129654301392121";

            List<DisputeSummary> summary = ReportingService.FindSettlementDisputes()
                .OrderBy(DisputeSortProperty.Id, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(DataServiceCriteria.StartStageDate, new DateTime(2020, 1, 1))
                .And(SearchCriteria.AquirerReferenceNumber, arn)
                .Execute();
            Assert.IsNotNull(summary);
            Assert.IsTrue(summary is List<DisputeSummary>);
            Assert.IsTrue(summary.Count == 0);
        }

        [TestMethod]
        public void ReportFindSettlementDisputes_FilterBy_Brand()
        {
            String brand = "VISA";

            List<DisputeSummary> summary = ReportingService.FindSettlementDisputes()
                .OrderBy(DisputeSortProperty.Id, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(DataServiceCriteria.StartStageDate, new DateTime(2020, 1, 1))
                .And(SearchCriteria.CardBrand, brand)
                .Execute();
            Assert.IsNotNull(summary);
            Assert.IsTrue(summary is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindSettlementDisputes_FilterBy_Brand_NotFound()
        {
            String brand = "MASTERCAR";

            List<DisputeSummary> summary = ReportingService.FindSettlementDisputes()
                .OrderBy(DisputeSortProperty.Id, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(DataServiceCriteria.StartStageDate, new DateTime(2020, 1, 1))
                .And(SearchCriteria.CardBrand, brand)
                .Execute();
            Assert.IsNotNull(summary);
            Assert.IsTrue(summary is List<DisputeSummary>);
            Assert.IsTrue(summary.Count == 0);
        }

        [TestMethod]
        public void ReportFindSettlementDisputes_FilterBy_Status()
        {
            String status = "WITH_MERCHANT";
            List<DisputeSummary> summary = ReportingService.FindSettlementDisputes()
                .OrderBy(DisputeSortProperty.Id, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(DataServiceCriteria.StartStageDate, new DateTime(2020, 1, 1))
                .And(SearchCriteria.DisputeStatus, DisputeStatus.WithMerchant)
                .Execute();
            Assert.IsNotNull(summary);
            Assert.IsTrue(summary is List<DisputeSummary>);
            Assert.IsTrue(summary.TrueForAll(d => d.CaseStatus == status));
        }

        [TestMethod]
        public void ReportFindSettlementDisputes_FilterBy_Stage()
        {
            String stage = "CHARGEBACK";
            List<DisputeSummary> summary = ReportingService.FindSettlementDisputes()
                .OrderBy(DisputeSortProperty.Id, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(DataServiceCriteria.StartStageDate, new DateTime(2020, 1, 1))
                .And(SearchCriteria.DisputeStage, DisputeStage.Chargeback)
                .Execute();
            Assert.IsNotNull(summary);
            Assert.IsTrue(summary is List<DisputeSummary>);
            Assert.IsTrue(summary.TrueForAll(d => d.CaseStage == stage));
        }

        [TestMethod]
        public void ReportFindSettlementDisputes_FilterBy_FromAndToStageTimeCreated()
        {
            var startDate = new DateTime(2020, 1, 1);
            var endDate = DateTime.Now;
            List<DisputeSummary> summary = ReportingService.FindSettlementDisputes()
                .OrderBy(DisputeSortProperty.Id, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(DataServiceCriteria.StartStageDate, startDate)
                .And(DataServiceCriteria.EndStageDate, endDate)
                .Execute();
            Assert.IsNotNull(summary);
            Assert.IsTrue(summary is List<DisputeSummary>);
            Assert.IsTrue(summary.TrueForAll(d => d.CaseTime >= startDate.Date && d.CaseTime <= endDate.Date));
        }

        [TestMethod]
        public void ReportFindSettlementDisputes_FilterBy_AdjustmentFunding()
        {
            var adjustmentFunding = AdjustmentFunding.Credit;
            List<DisputeSummary> summary = ReportingService.FindSettlementDisputes()
                .OrderBy(DisputeSortProperty.Id, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(DataServiceCriteria.StartStageDate, new DateTime(2020, 1, 1))
                .And(DataServiceCriteria.AdjustmentFunding, adjustmentFunding)
                .Execute();
            Assert.IsNotNull(summary);
            Assert.IsTrue(summary is List<DisputeSummary>);
            Assert.IsTrue(summary.TrueForAll(d => d.LastAdjustmentFunding == adjustmentFunding.ToString()));
        }

        [TestMethod]
        public void ReportFindSettlementDisputes_FilterBy_FromAndToAdjustmentTimeCreated()
        {
            var startDate = new DateTime(2020, 1, 1);
            var endDate = DateTime.Now;
            List<DisputeSummary> summary = ReportingService.FindSettlementDisputes()
                .OrderBy(DisputeSortProperty.Id, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(DataServiceCriteria.StartStageDate, startDate)
                .And(DataServiceCriteria.StartAdjustmentDate, startDate)
                .And(DataServiceCriteria.EndAdjustmentDate, endDate)
                .Execute();
            Assert.IsNotNull(summary);
            Assert.IsTrue(summary is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindSettlementDisputes_FilterBy_SystemMidAndHierarchy()
        {
            String systemMid = "101023947262";
            String systemHierarchy = "055-70-024-011-019";

            List<DisputeSummary> summary = ReportingService.FindSettlementDisputes()
                .OrderBy(DisputeSortProperty.Id, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(DataServiceCriteria.StartStageDate, new DateTime(2020, 1, 1))
                .And(DataServiceCriteria.MerchantId, systemMid)
                .And(DataServiceCriteria.SystemHierarchy, systemHierarchy)
                .Execute();

            Assert.IsNotNull(summary);
            Assert.IsTrue(summary is List<DisputeSummary>);
            Assert.IsTrue(summary.TrueForAll(d => d.CaseMerchantId == systemMid
            && d.MerchantHierarchy == systemHierarchy));
        }

        [TestMethod]
        public void ReportFindSettlementDisputes_FilterBy_WrongSystemMid()
        {
            String systemMid = "000023947222";

            List<DisputeSummary> summary = ReportingService.FindSettlementDisputes()
                .OrderBy(DisputeSortProperty.Id, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(DataServiceCriteria.StartStageDate, new DateTime(2020, 1, 1))
                .And(DataServiceCriteria.MerchantId, systemMid)
                .Execute();

            Assert.IsNotNull(summary);
            Assert.IsTrue(summary is List<DisputeSummary>);
            Assert.IsTrue(summary.Count == 0);
        }

        [TestMethod]
        public void ReportFindSettlementDisputes_FilterBy_WrongSystemHierarchy()
        {
            String systemHierarchy = "000-70-024-011-111";

            List<DisputeSummary> summary = ReportingService.FindSettlementDisputes()
                .OrderBy(DisputeSortProperty.Id, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(DataServiceCriteria.StartStageDate, new DateTime(2020, 1, 1))
                .And(DataServiceCriteria.SystemHierarchy, systemHierarchy)
                .Execute();

            Assert.IsNotNull(summary);
            Assert.IsTrue(summary is List<DisputeSummary>);
            Assert.IsTrue(summary.Count == 0);
        }

        [TestMethod]
        public void ReportFindSettlementDisputes_Order_By_ARN() {
            List<DisputeSummary> summary = ReportingService.FindSettlementDisputes()
                .OrderBy(DisputeSortProperty.ARN, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(DataServiceCriteria.StartStageDate, new DateTime(2020, 1, 1))
                .Execute();
            Assert.IsNotNull(summary);
            Assert.IsTrue(summary is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindSettlementDisputes_Order_By_Id_With_Status_UnderReview() {
            List<DisputeSummary> summary = ReportingService.FindSettlementDisputes()
                .OrderBy(DisputeSortProperty.Id, SortDirection.Ascending)
                .WithPaging(1, 10)
                .Where(DataServiceCriteria.StartStageDate, new DateTime(2020, 1, 1))
                .And(SearchCriteria.DisputeStatus, DisputeStatus.UnderReview)
                .Execute();
            Assert.IsNotNull(summary);
            Assert.IsTrue(summary is List<DisputeSummary>);
        }
        #endregion
    }
}
