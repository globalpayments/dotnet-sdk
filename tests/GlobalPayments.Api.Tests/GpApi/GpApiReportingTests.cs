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
        public void ReportFindDisputes_By_ARN() {
            string arn = "135091790340196";
            List<DisputeSummary> disputes = ReportingService.FindDisputes()
                .WithPaging(1, 10)
                .Where(DataServiceCriteria.StartStageDate, DateTime.Now.AddYears(-2).AddDays(1))
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
                .Where(DataServiceCriteria.StartStageDate, DateTime.Now.AddYears(-2).AddDays(1))
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
                .Where(DataServiceCriteria.StartStageDate, DateTime.Now.AddYears(-2).AddDays(1))
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
                .Where(DataServiceCriteria.StartStageDate, DateTime.Now.AddYears(-2).AddDays(1))
                .And(SearchCriteria.DisputeStage, disputeStage)
                .Execute();
            Assert.IsNotNull(disputes);
            Assert.IsTrue(disputes is List<DisputeSummary>);
            Assert.IsTrue(disputes.TrueForAll(d => d.CaseStage == EnumConverter.GetMapping(Target.GP_API, disputeStage)));
        }

        [TestMethod]
        public void ReportFindDisputes_By_MerchantId_And_SystemHierarchy() {
            string merchantId = "8593872";
            string systemHierarchy = "111-23-099-002-005";
            List<DisputeSummary> disputes = ReportingService.FindDisputes()
                .WithPaging(1, 10)
                .Where(DataServiceCriteria.StartStageDate, DateTime.Now.AddYears(-2).AddDays(1))
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
                .Where(DataServiceCriteria.StartStageDate, DateTime.Now.AddYears(-2).AddDays(1))
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
                .Where(DataServiceCriteria.StartStageDate, DateTime.Now.AddYears(-2).AddDays(1))
                .Execute();
            Assert.IsNotNull(disputes);
            Assert.IsTrue(disputes is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindDisputes_Order_By_Status() {
            List<DisputeSummary> disputes = ReportingService.FindDisputes()
                .OrderBy(DisputeSortProperty.Status, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(DataServiceCriteria.StartStageDate, DateTime.Now.AddYears(-2).AddDays(1))
                .Execute();
            Assert.IsNotNull(disputes);
            Assert.IsTrue(disputes is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindDisputes_Order_By_Stage() {
            List<DisputeSummary> disputes = ReportingService.FindDisputes()
                .OrderBy(DisputeSortProperty.Stage, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(DataServiceCriteria.StartStageDate, DateTime.Now.AddYears(-2).AddDays(1))
                .Execute();
            Assert.IsNotNull(disputes);
            Assert.IsTrue(disputes is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindDisputes_Order_By_FromStageTimeCreated() {
            List<DisputeSummary> disputes = ReportingService.FindDisputes()
                .OrderBy(DisputeSortProperty.FromStageTimeCreated, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(DataServiceCriteria.StartStageDate, DateTime.Now.AddYears(-2).AddDays(1))
                .Execute();
            Assert.IsNotNull(disputes);
            Assert.IsTrue(disputes is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindDisputes_Order_By_ToStageTimeCreated() {
            List<DisputeSummary> disputes = ReportingService.FindDisputes()
                .OrderBy(DisputeSortProperty.ToStageTimeCreated, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(DataServiceCriteria.StartStageDate, DateTime.Now.AddYears(-2).AddDays(1))
                .Execute();
            Assert.IsNotNull(disputes);
            Assert.IsTrue(disputes is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindDisputes_Order_By_AdjustmentFunding() {
            List<DisputeSummary> disputes = ReportingService.FindDisputes()
                .OrderBy(DisputeSortProperty.AdjustmentFunding, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(DataServiceCriteria.StartStageDate, DateTime.Now.AddYears(-2).AddDays(1))
                .Execute();
            Assert.IsNotNull(disputes);
            Assert.IsTrue(disputes is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindDisputes_Order_By_FromAdjustmentTimeCreated() {
            List<DisputeSummary> disputes = ReportingService.FindDisputes()
                .OrderBy(DisputeSortProperty.FromAdjustmentTimeCreated, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(DataServiceCriteria.StartStageDate, DateTime.Now.AddYears(-2).AddDays(1))
                .Execute();
            Assert.IsNotNull(disputes);
            Assert.IsTrue(disputes is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindDisputes_Order_By_ToAdjustmentTimeCreated() {
            List<DisputeSummary> disputes = ReportingService.FindDisputes()
                .OrderBy(DisputeSortProperty.ToAdjustmentTimeCreated, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(DataServiceCriteria.StartStageDate, DateTime.Now.AddYears(-2).AddDays(1))
                .Execute();
            Assert.IsNotNull(disputes);
            Assert.IsTrue(disputes is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindDisputes_Order_By_Id_With_Brand_VISA() {
            List<DisputeSummary> disputes = ReportingService.FindDisputes()
                .OrderBy(DisputeSortProperty.Id, SortDirection.Ascending)
                .WithPaging(1, 10)
                .Where(DataServiceCriteria.StartStageDate, DateTime.Now.AddYears(-2).AddDays(1))
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
                .Where(DataServiceCriteria.StartStageDate, DateTime.Now.AddYears(-2).AddDays(1))
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
                .Where(DataServiceCriteria.StartStageDate, DateTime.Now.AddYears(-2).AddDays(1))
                .And(SearchCriteria.DisputeStage, DisputeStage.Chargeback)
                .Execute();
            Assert.IsNotNull(disputes);
            Assert.IsTrue(disputes is List<DisputeSummary>);
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
        public void ReportFindSettlementDisputes_Order_By_Id() {
            List<DisputeSummary> summary = ReportingService.FindSettlementDisputes()
                .OrderBy(DisputeSortProperty.Id, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(DataServiceCriteria.StartStageDate, DateTime.Now.AddYears(-2).AddDays(1))
                .Execute();
            Assert.IsNotNull(summary);
            Assert.IsTrue(summary is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindSettlementDisputes_Order_By_ARN() {
            List<DisputeSummary> summary = ReportingService.FindSettlementDisputes()
                .OrderBy(DisputeSortProperty.ARN, SortDirection.Descending)
                .WithPaging(1, 10)
                .Where(DataServiceCriteria.StartStageDate, DateTime.Now.AddYears(-2).AddDays(1))
                .Execute();
            Assert.IsNotNull(summary);
            Assert.IsTrue(summary is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindSettlementDisputes_Order_By_Id_With_Status_UnderReview() {
            List<DisputeSummary> summary = ReportingService.FindSettlementDisputes()
                .OrderBy(DisputeSortProperty.Id, SortDirection.Ascending)
                .WithPaging(1, 10)
                .Where(DataServiceCriteria.StartStageDate, DateTime.Now.AddYears(-2).AddDays(1))
                .And(SearchCriteria.DisputeStatus, DisputeStatus.UnderReview)
                .Execute();
            Assert.IsNotNull(summary);
            Assert.IsTrue(summary is List<DisputeSummary>);
        }
        #endregion
    }
}
