using System;
using System.Collections.Generic;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Utils;
using GlobalPayments.Api.Utils.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.GpApi
{
    [TestClass]
    public class GpApiReportingDisputesTests : BaseGpApiReportingTest {
        
        [ClassInitialize]
        public static void ClassInitialize(TestContext context) {
            ServicesContainer.ConfigureService(new GpApiConfig {
                AppId = APP_ID,
                AppKey = APP_KEY,
                RequestLogger = new RequestConsoleLogger(),
                EnableLogging = true
            });
        }

        #region Disputes

        [TestMethod]
        public void ReportDisputeDetail() {
            const string disputeId = "DIS_SAND_abcd1234";

            var response = ReportingService.DisputeDetail(disputeId)
                .Execute();
            Assert.IsNotNull(response);
            Assert.IsTrue(response is DisputeSummary);
            Assert.AreEqual(disputeId, response.CaseId);
        }

        [TestMethod]
        public void ReportDisputeDetail_WrongId() {
            var disputeId = "DIS_SAND_" + Guid.NewGuid();
            var exceptionCaught = false;
            try {
                ReportingService.DisputeDetail(disputeId)
                    .Execute();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("INVALID_REQUEST_DATA", ex.ResponseCode);
                Assert.AreEqual("40073", ex.ResponseMessage);
                Assert.IsTrue(ex.Message.Contains(
                    "Status Code: BadRequest - 101,Unable to locate dispute record for that ID. Please recheck the ID provided."));
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void ReportFindDisputesPaged_By_ARN() {
            const string arn = "135091790340196";
            var result = ReportingService.FindDisputesPaged(FIRST_PAGE, PAGE_SIZE)
                .Where(SearchCriteria.AquirerReferenceNumber, arn)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
            Assert.IsTrue(result.Results.TrueForAll(d => d.TransactionARN == arn));
        }

        [TestMethod]
        public void ReportFindDisputesPaged_By_Brand() {
            const string cardBrand = "VISA";
            var result = ReportingService.FindDisputesPaged(FIRST_PAGE, PAGE_SIZE)
                .Where(DataServiceCriteria.StartStageDate, REPORTING_START_DATE)
                .And(SearchCriteria.CardBrand, cardBrand)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
            Assert.IsTrue(result.Results.TrueForAll(d => d.TransactionCardType == cardBrand));
        }

        [TestMethod]
        public void ReportFindDisputesPaged_By_Status() {
            foreach (DisputeStatus disputeStatus in Enum.GetValues(typeof(DisputeStatus))) {
                var result = ReportingService.FindDisputesPaged(FIRST_PAGE, PAGE_SIZE)
                    .Where(DataServiceCriteria.StartStageDate, REPORTING_START_DATE)
                    .And(SearchCriteria.DisputeStatus, disputeStatus)
                    .Execute();
                Assert.IsNotNull(result?.Results);
                Assert.IsTrue(result.Results is List<DisputeSummary>);
                Assert.IsTrue(result.Results.TrueForAll(d =>
                    d.CaseStatus == EnumConverter.GetMapping(Target.GP_API, disputeStatus)));
            }
        }

        [TestMethod]
        public void ReportFindDisputesPaged_By_Stage() {
            foreach (DisputeStage disputeStage in Enum.GetValues(typeof(DisputeStage))) {
                if (disputeStage == DisputeStage.Goodfaith) {
                    continue;
                }

                var result = ReportingService.FindDisputesPaged(FIRST_PAGE, PAGE_SIZE)
                    .Where(DataServiceCriteria.StartStageDate, REPORTING_START_DATE)
                    .And(SearchCriteria.DisputeStage, disputeStage)
                    .Execute();
                Assert.IsNotNull(result?.Results);
                Assert.IsTrue(result.Results is List<DisputeSummary>);
                Assert.IsTrue(result.Results.TrueForAll(d =>
                    d.CaseStage == EnumConverter.GetMapping(Target.GP_API, disputeStage)));
            }
        }

        [TestMethod]
        public void ReportFindDisputesPaged_By_From_And_To_Stage_Time_Created() {
            var result = ReportingService.FindDisputesPaged(FIRST_PAGE, PAGE_SIZE)
                .Where(DataServiceCriteria.StartStageDate, REPORTING_START_DATE)
                .And(DataServiceCriteria.EndStageDate, REPORTING_END_DATE)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
            Assert.IsTrue(result.Results.TrueForAll(d =>
                d.CaseIdTime >= REPORTING_START_DATE.Date && d.CaseIdTime <= REPORTING_END_DATE.Date));
        }

        [TestMethod]
        public void ReportFindDisputesPaged_By_MerchantId() {
            const string merchantId = "8593872";

            var result = ReportingService.FindDisputesPaged(FIRST_PAGE, PAGE_SIZE)
                .Where(DataServiceCriteria.StartStageDate, REPORTING_START_DATE)
                .And(DataServiceCriteria.MerchantId, merchantId)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
            Assert.IsTrue(result.Results.TrueForAll(d => d.CaseMerchantId == merchantId));
        }

        [TestMethod]
        public void ReportFindDisputesPaged_By_SystemHierarchy() {
            const string systemHierarchy = "111-23-099-002-005";

            var result = ReportingService.FindDisputesPaged(FIRST_PAGE, PAGE_SIZE)
                .Where(DataServiceCriteria.StartStageDate, REPORTING_START_DATE)
                .And(DataServiceCriteria.SystemHierarchy, systemHierarchy)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
            Assert.IsTrue(result.Results.TrueForAll(d => d.MerchantHierarchy == systemHierarchy));
        }

        [TestMethod]
        public void ReportFindDisputesPaged_Order_By_Id() {
            var result = ReportingService.FindDisputesPaged(FIRST_PAGE, PAGE_SIZE)
                .OrderBy(DisputeSortProperty.Id, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, REPORTING_START_DATE)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindDisputesPaged_Order_By_ARN() {
            var result = ReportingService.FindDisputesPaged(FIRST_PAGE, PAGE_SIZE)
                .OrderBy(DisputeSortProperty.ARN, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, REPORTING_START_DATE)
                // EndStageDate must be set in order to be able to sort by ARN
                .And(DataServiceCriteria.EndStageDate, REPORTING_LAST_MONTH_DATE)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindDisputesPaged_Order_By_Brand() {
            var result = ReportingService.FindDisputesPaged(FIRST_PAGE, PAGE_SIZE)
                .OrderBy(DisputeSortProperty.Brand, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, REPORTING_START_DATE)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindDisputesPaged_Order_By_Status() {
            var result = ReportingService.FindDisputesPaged(FIRST_PAGE, PAGE_SIZE)
                .OrderBy(DisputeSortProperty.Status, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, REPORTING_START_DATE)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindDisputesPaged_Order_By_Stage() {
            var result = ReportingService.FindDisputesPaged(FIRST_PAGE, PAGE_SIZE)
                .OrderBy(DisputeSortProperty.Stage, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, REPORTING_START_DATE)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
        }

        [TestMethod]
        [Ignore]
        // orderBy FromStageTimeCreated is not supported anymore
        public void ReportFindDisputesPaged_Order_By_FromStageTimeCreated() {
            var result = ReportingService.FindDisputesPaged(FIRST_PAGE, PAGE_SIZE)
                .OrderBy(DisputeSortProperty.FromStageTimeCreated, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, REPORTING_START_DATE)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindDisputesPaged_Order_By_ToStageTimeCreated() {
            var result = ReportingService.FindDisputesPaged(FIRST_PAGE, PAGE_SIZE)
                .OrderBy(DisputeSortProperty.ToStageTimeCreated, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, REPORTING_START_DATE)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindDisputesPaged_Order_By_AdjustmentFunding() {
            var result = ReportingService.FindDisputesPaged(FIRST_PAGE, PAGE_SIZE)
                .OrderBy(DisputeSortProperty.AdjustmentFunding, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, REPORTING_START_DATE)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindDisputesPaged_Order_By_FromAdjustmentTimeCreated() {
            var result = ReportingService.FindDisputesPaged(FIRST_PAGE, PAGE_SIZE)
                .OrderBy(DisputeSortProperty.FromAdjustmentTimeCreated, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, REPORTING_START_DATE)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindDisputesPaged_Order_By_ToAdjustmentTimeCreated() {
            var result = ReportingService.FindDisputesPaged(FIRST_PAGE, PAGE_SIZE)
                .OrderBy(DisputeSortProperty.ToAdjustmentTimeCreated, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, REPORTING_START_DATE)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindDisputesPaged_Order_By_Id_With_Brand_VISA() {
            var result = ReportingService.FindDisputesPaged(FIRST_PAGE, PAGE_SIZE)
                .OrderBy(DisputeSortProperty.Id, SortDirection.Ascending)
                .Where(DataServiceCriteria.StartStageDate, REPORTING_START_DATE)
                .And(SearchCriteria.CardBrand, "VISA")
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindDisputesPaged_Order_By_Id_With_Status_UnderReview() {
            var result = ReportingService.FindDisputesPaged(FIRST_PAGE, PAGE_SIZE)
                .OrderBy(DisputeSortProperty.Id, SortDirection.Ascending)
                .Where(DataServiceCriteria.StartStageDate, REPORTING_START_DATE)
                .And(SearchCriteria.DisputeStatus, DisputeStatus.UnderReview)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindDisputesPaged_Order_By_Id_With_Stage_Chargeback() {
            var result = ReportingService.FindDisputesPaged(FIRST_PAGE, PAGE_SIZE)
                .OrderBy(DisputeSortProperty.Id, SortDirection.Ascending)
                .Where(DataServiceCriteria.StartStageDate, REPORTING_START_DATE)
                .And(SearchCriteria.DisputeStage, DisputeStage.Chargeback)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
        }

        #endregion

        #region Settlement disputes

        [TestMethod]
        public void ReportSettlementDisputeDetail() {
            const string settlementDisputeId = "DIS_810";
            var response = ReportingService.SettlementDisputeDetail(settlementDisputeId)
                .Execute();
            Assert.IsNotNull(response);
            Assert.IsTrue(response is DisputeSummary);
            Assert.AreEqual(settlementDisputeId, response.CaseId);
        }

        [TestMethod]
        public void ReportSettlementDisputeDetailWrongId() {
            const string disputeId = "DIS_666";
            var exceptionCaught = false;
            try {
                ReportingService.SettlementDisputeDetail(disputeId)
                    .Execute();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("RESOURCE_NOT_FOUND", ex.ResponseCode);
                Assert.AreEqual("40118", ex.ResponseMessage);
                Assert.AreEqual(
                    $"Status Code: NotFound - Disputes {disputeId} not found at this /ucp/settlement/disputes/{disputeId}",
                    ex.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void ReportFindSettlementDisputesPaged_Order_By_Id() {
            var result = ReportingService.FindSettlementDisputesPaged(FIRST_PAGE, PAGE_SIZE)
                .OrderBy(DisputeSortProperty.Id, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, REPORTING_START_DATE)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindSettlementDisputesByDpositIdPaged_Order_By_Id() {
            const string depositId = "DEP_2342423423";

            var result = ReportingService.FindSettlementDisputesPaged(FIRST_PAGE, PAGE_SIZE)
                .OrderBy(DisputeSortProperty.Id, SortDirection.Descending)
                .Where(DataServiceCriteria.DepositReference, depositId)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
            foreach (var disputes in result.Results) {
                Assert.AreEqual(disputes.DepositReference, depositId);
            }
        }

        [TestMethod]
        public void ReportFindSettlementDisputesPaged_OrderBy_Brand() {
            var result = ReportingService.FindSettlementDisputesPaged(FIRST_PAGE, PAGE_SIZE)
                .OrderBy(DisputeSortProperty.Brand, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, REPORTING_START_DATE)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindSettlementDisputesPaged_OrderBy_Stage() {
            var result = ReportingService.FindSettlementDisputesPaged(FIRST_PAGE, PAGE_SIZE)
                .OrderBy(DisputeSortProperty.Stage, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, REPORTING_START_DATE)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindSettlementDisputesPaged_OrderBy_FromStageTimeCreated() {
            var result = ReportingService.FindSettlementDisputesPaged(FIRST_PAGE, PAGE_SIZE)
                .OrderBy(DisputeSortProperty.FromStageTimeCreated, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, REPORTING_START_DATE)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindSettlementDisputesPaged_OrderBy_ToStageTimeCreated() {
            var result = ReportingService.FindSettlementDisputesPaged(FIRST_PAGE, PAGE_SIZE)
                .OrderBy(DisputeSortProperty.ToStageTimeCreated, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, REPORTING_START_DATE)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindSettlementDisputesPaged_OrderBy_AdjustmentFunding() {
            var result = ReportingService.FindSettlementDisputesPaged(FIRST_PAGE, PAGE_SIZE)
                .OrderBy(DisputeSortProperty.AdjustmentFunding, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, REPORTING_START_DATE)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindSettlementDisputesPaged_OrderBy_FromAdjustmentTimeCreated() {
            var result = ReportingService.FindSettlementDisputesPaged(FIRST_PAGE, PAGE_SIZE)
                .OrderBy(DisputeSortProperty.FromAdjustmentTimeCreated, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, REPORTING_START_DATE)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindSettlementDisputesPaged_OrderBy_ToAdjustmentTimeCreated() {
            var result = ReportingService.FindSettlementDisputesPaged(FIRST_PAGE, PAGE_SIZE)
                .OrderBy(DisputeSortProperty.ToAdjustmentTimeCreated, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, REPORTING_START_DATE)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindSettlementDisputesPaged_By_ARN() {
            const string arn = "71400011129688701392096";

            var result = ReportingService.FindSettlementDisputesPaged(FIRST_PAGE, PAGE_SIZE)
                .OrderBy(DisputeSortProperty.Id, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, REPORTING_START_DATE)
                .And(SearchCriteria.AquirerReferenceNumber, arn)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
            Assert.IsTrue(result.Results.TrueForAll(d => d.TransactionARN == arn));
        }

        [TestMethod]
        public void ReportFindSettlementDisputesPaged_By_ARN_NotFound() {
            const string arn = "00000011129654301392121";

            var result = ReportingService.FindSettlementDisputesPaged(FIRST_PAGE, PAGE_SIZE)
                .OrderBy(DisputeSortProperty.Id, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, REPORTING_START_DATE)
                .And(SearchCriteria.AquirerReferenceNumber, arn)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
            Assert.IsTrue(result.Results.Count == 0);
        }

        [TestMethod]
        public void ReportFindSettlementDisputesPaged_By_Brand() {
            const string brand = "VISA";

            var result = ReportingService.FindSettlementDisputesPaged(FIRST_PAGE, PAGE_SIZE)
                .OrderBy(DisputeSortProperty.Id, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, REPORTING_START_DATE)
                .And(SearchCriteria.CardBrand, brand)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindSettlementDisputesPaged_By_Brand_NotFound() {
            const string brand = "MASTERCAR";

            var result = ReportingService.FindSettlementDisputesPaged(FIRST_PAGE, PAGE_SIZE)
                .OrderBy(DisputeSortProperty.Id, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, REPORTING_START_DATE)
                .And(SearchCriteria.CardBrand, brand)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
            Assert.IsTrue(result.Results.Count == 0);
        }

        [TestMethod]
        public void ReportFindSettlementDisputesPaged_By_Status() {
            var result = ReportingService.FindSettlementDisputesPaged(FIRST_PAGE, PAGE_SIZE)
                .OrderBy(DisputeSortProperty.Id, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, REPORTING_START_DATE)
                .And(SearchCriteria.DisputeStatus, DisputeStatus.Funded)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
            Assert.IsTrue(result.Results.TrueForAll(d => d.CaseStatus == EnumConverter.GetMapping(Target.GP_API, DisputeStatus.Funded)));
        }

        [TestMethod]
        public void ReportFindSettlementDisputesPaged_By_DepositDate() {
            var result = ReportingService.FindSettlementDisputesPaged(FIRST_PAGE, PAGE_SIZE)
                .OrderBy(DisputeSortProperty.Id, SortDirection.Descending)
                .Where(DataServiceCriteria.StartDepositDate, REPORTING_START_DATE)
                .And(DataServiceCriteria.EndDepositDate, REPORTING_END_DATE)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
            Assert.IsTrue(result.Results.TrueForAll(d =>
                d.DepositDate >= REPORTING_START_DATE && d.DepositDate <= REPORTING_END_DATE));
        }

        [TestMethod]
        public void ReportFindSettlementDisputesPaged_By_Stage() {
            foreach (DisputeStage stage in Enum.GetValues(typeof(DisputeStage))) {
                var result = ReportingService.FindSettlementDisputesPaged(FIRST_PAGE, PAGE_SIZE)
                    .OrderBy(DisputeSortProperty.Id, SortDirection.Descending)
                    .Where(DataServiceCriteria.StartStageDate, REPORTING_START_DATE)
                    .And(SearchCriteria.DisputeStage, stage)
                    .Execute();
                Assert.IsNotNull(result?.Results);
                Assert.IsTrue(result.Results is List<DisputeSummary>);
                Assert.IsTrue(result.Results.TrueForAll(d => d.CaseStage == stage.ToString().ToUpper()));
            }
        }

        [TestMethod]
        public void ReportFindSettlementDisputesPaged_By_From_And_To_Stage_Time_Created() {
            var result = ReportingService.FindSettlementDisputesPaged(FIRST_PAGE, PAGE_SIZE)
                .OrderBy(DisputeSortProperty.Id, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, REPORTING_START_DATE)
                .And(DataServiceCriteria.EndStageDate, REPORTING_LAST_MONTH_DATE)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
            Assert.IsTrue(result.Results.TrueForAll(d =>
                d.CaseIdTime >= REPORTING_START_DATE.Date && d.CaseIdTime <= REPORTING_LAST_MONTH_DATE.Date));
        }

        [TestMethod]
        public void ReportFindSettlementDisputesPaged_By_MerchantId() {
            const string merchantId = "101023947262";

            var result = ReportingService.FindSettlementDisputesPaged(FIRST_PAGE, PAGE_SIZE)
                .OrderBy(DisputeSortProperty.Id, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, REPORTING_START_DATE)
                .And(DataServiceCriteria.MerchantId, merchantId)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
            Assert.IsTrue(result.Results.TrueForAll(d => d.CaseMerchantId == merchantId));
        }

        [TestMethod]
        public void ReportFindSettlementDisputesPaged_By_SystemHierarchy() {
            const string systemHierarchy = "055-70-024-011-019";

            var result = ReportingService.FindSettlementDisputesPaged(FIRST_PAGE, PAGE_SIZE)
                .OrderBy(DisputeSortProperty.Id, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, REPORTING_START_DATE)
                .And(DataServiceCriteria.SystemHierarchy, systemHierarchy)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
            Assert.IsTrue(result.Results.TrueForAll(d => d.MerchantHierarchy == systemHierarchy));
        }

        [TestMethod]
        public void ReportFindSettlementDisputesPaged_By_Wrong_MerchantId() {
            const string merchantId = "000023947222";

            var result = ReportingService.FindSettlementDisputesPaged(FIRST_PAGE, PAGE_SIZE)
                .OrderBy(DisputeSortProperty.Id, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, REPORTING_START_DATE)
                .And(DataServiceCriteria.MerchantId, merchantId)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
            Assert.IsTrue(result.Results.Count == 0);
        }

        [TestMethod]
        public void ReportFindSettlementDisputesPaged_By_Wrong_SystemHierarchy() {
            const string systemHierarchy = "000-70-024-011-111";

            var result = ReportingService.FindSettlementDisputesPaged(FIRST_PAGE, PAGE_SIZE)
                .OrderBy(DisputeSortProperty.Id, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, REPORTING_START_DATE)
                .And(DataServiceCriteria.SystemHierarchy, systemHierarchy)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
            Assert.IsTrue(result.Results.Count == 0);
        }

        [TestMethod]
        public void ReportFindSettlementDisputesPaged_Order_By_ARN() {
            var result = ReportingService.FindSettlementDisputesPaged(FIRST_PAGE, PAGE_SIZE)
                .OrderBy(DisputeSortProperty.ARN, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, REPORTING_START_DATE)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindSettlementDisputesPaged_Order_By_Id_With_Status_UnderReview() {
            var result = ReportingService.FindSettlementDisputesPaged(FIRST_PAGE, PAGE_SIZE)
                .OrderBy(DisputeSortProperty.Id, SortDirection.Ascending)
                .Where(DataServiceCriteria.StartStageDate, REPORTING_START_DATE)
                .And(SearchCriteria.DisputeStatus, DisputeStatus.UnderReview)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
        }
        #endregion
    }
}