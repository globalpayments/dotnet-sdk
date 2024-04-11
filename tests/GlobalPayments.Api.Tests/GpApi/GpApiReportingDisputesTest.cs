using System;
using System.Collections.Generic;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.GpApi
{
    [TestClass]
    public class GpApiReportingDisputesTest : BaseGpApiReportingTest {
        
        [ClassInitialize]
        public static void ClassInitialize(TestContext context) {
            var gpApiConfig = GpApiConfigSetup(AppId, AppKey, Channel.CardNotPresent);
            ServicesContainer.ConfigureService(gpApiConfig);
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
            var result = ReportingService.FindDisputesPaged(FirstPage, PageSize)
                .Where(SearchCriteria.AquirerReferenceNumber, arn)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
            Assert.IsTrue(result.Results.TrueForAll(d => d.TransactionARN == arn));
        }

        [TestMethod]
        public void ReportFindDisputesPaged_By_Brand() {
            const string cardBrand = "VISA";
            var result = ReportingService.FindDisputesPaged(FirstPage, PageSize)
                .Where(DataServiceCriteria.StartStageDate, ReportingStartDate)
                .And(SearchCriteria.CardBrand, cardBrand)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
            Assert.IsTrue(result.Results.TrueForAll(d => d.TransactionCardType == cardBrand));
        }

        [TestMethod]
        public void ReportFindDisputesPaged_By_Status() {
            foreach (DisputeStatus disputeStatus in Enum.GetValues(typeof(DisputeStatus))) {
                var result = ReportingService.FindDisputesPaged(FirstPage, PageSize)
                    .Where(DataServiceCriteria.StartStageDate, ReportingStartDate)
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

                var result = ReportingService.FindDisputesPaged(FirstPage, PageSize)
                    .Where(DataServiceCriteria.StartStageDate, ReportingStartDate)
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
            var result = ReportingService.FindDisputesPaged(FirstPage, PageSize)
                .Where(DataServiceCriteria.StartStageDate, ReportingStartDate)
                .And(DataServiceCriteria.EndStageDate, ReportingEndDate)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
            Assert.IsTrue(result.Results.TrueForAll(d =>
                d.CaseIdTime >= ReportingStartDate.Date && d.CaseIdTime <= ReportingEndDate.Date));
        }

        [TestMethod]
        public void ReportFindDisputesPaged_By_MerchantId() {
            const string merchantId = "8593872";

            var result = ReportingService.FindDisputesPaged(FirstPage, PageSize)
                .Where(DataServiceCriteria.StartStageDate, ReportingStartDate)
                .And(DataServiceCriteria.MerchantId, merchantId)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
            Assert.IsTrue(result.Results.TrueForAll(d => d.CaseMerchantId == merchantId));
        }

        [TestMethod]
        public void ReportFindDisputesPaged_By_SystemHierarchy() {
            const string systemHierarchy = "111-23-099-002-005";

            var result = ReportingService.FindDisputesPaged(FirstPage, PageSize)
                .Where(DataServiceCriteria.StartStageDate, ReportingStartDate)
                .And(DataServiceCriteria.SystemHierarchy, systemHierarchy)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
            Assert.IsTrue(result.Results.TrueForAll(d => d.MerchantHierarchy == systemHierarchy));
        }

        [TestMethod]
        public void ReportFindDisputesPaged_Order_By_Id() {
            var result = ReportingService.FindDisputesPaged(FirstPage, PageSize)
                .OrderBy(DisputeSortProperty.Id, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, ReportingStartDate)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindDisputesPaged_Order_By_ARN() {
            var result = ReportingService.FindDisputesPaged(FirstPage, PageSize)
                .OrderBy(DisputeSortProperty.ARN, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, ReportingStartDate)
                // EndStageDate must be set in order to be able to sort by ARN
                .And(DataServiceCriteria.EndStageDate, ReportingLastMonthDate)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindDisputesPaged_Order_By_Brand() {
            var result = ReportingService.FindDisputesPaged(FirstPage, PageSize)
                .OrderBy(DisputeSortProperty.Brand, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, ReportingStartDate)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindDisputesPaged_Order_By_Status() {
            var result = ReportingService.FindDisputesPaged(FirstPage, PageSize)
                .OrderBy(DisputeSortProperty.Status, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, ReportingStartDate)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindDisputesPaged_Order_By_Stage() {
            var result = ReportingService.FindDisputesPaged(FirstPage, PageSize)
                .OrderBy(DisputeSortProperty.Stage, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, ReportingStartDate)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
        }

        [TestMethod]
        [Ignore]
        // orderBy FromStageTimeCreated is not supported anymore
        public void ReportFindDisputesPaged_Order_By_FromStageTimeCreated() {
            var result = ReportingService.FindDisputesPaged(FirstPage, PageSize)
                .OrderBy(DisputeSortProperty.FromStageTimeCreated, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, ReportingStartDate)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindDisputesPaged_Order_By_ToStageTimeCreated() {
            var result = ReportingService.FindDisputesPaged(FirstPage, PageSize)
                .OrderBy(DisputeSortProperty.ToStageTimeCreated, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, ReportingStartDate)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindDisputesPaged_Order_By_AdjustmentFunding() {
            var result = ReportingService.FindDisputesPaged(FirstPage, PageSize)
                .OrderBy(DisputeSortProperty.AdjustmentFunding, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, ReportingStartDate)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindDisputesPaged_Order_By_FromAdjustmentTimeCreated() {
            var result = ReportingService.FindDisputesPaged(FirstPage, PageSize)
                .OrderBy(DisputeSortProperty.FromAdjustmentTimeCreated, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, ReportingStartDate)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindDisputesPaged_Order_By_ToAdjustmentTimeCreated() {
            var result = ReportingService.FindDisputesPaged(FirstPage, PageSize)
                .OrderBy(DisputeSortProperty.ToAdjustmentTimeCreated, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, ReportingStartDate)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindDisputesPaged_Order_By_Id_With_Brand_VISA() {
            var result = ReportingService.FindDisputesPaged(FirstPage, PageSize)
                .OrderBy(DisputeSortProperty.Id, SortDirection.Ascending)
                .Where(DataServiceCriteria.StartStageDate, ReportingStartDate)
                .And(SearchCriteria.CardBrand, "VISA")
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindDisputesPaged_Order_By_Id_With_Status_UnderReview() {
            var result = ReportingService.FindDisputesPaged(FirstPage, PageSize)
                .OrderBy(DisputeSortProperty.Id, SortDirection.Ascending)
                .Where(DataServiceCriteria.StartStageDate, ReportingStartDate)
                .And(SearchCriteria.DisputeStatus, DisputeStatus.UnderReview)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindDisputesPaged_Order_By_Id_With_Stage_Chargeback() {
            var result = ReportingService.FindDisputesPaged(FirstPage, PageSize)
                .OrderBy(DisputeSortProperty.Id, SortDirection.Ascending)
                .Where(DataServiceCriteria.StartStageDate, ReportingStartDate)
                .And(SearchCriteria.DisputeStage, DisputeStage.Chargeback)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
        }

        [TestMethod]
        public void FindDocumentAssociatedWithDispute()
        {
            var disputeId = "DIS_SAND_abcd1235";
            var disputeDocumentId = "DOC_MyEvidence_234234AVCDE-1";
            var response = ReportingService.DocumentDisputeDetail(disputeId)
                .WithDisputeDocumentId(disputeDocumentId)
                .Where(SearchCriteria.DisputeDocumentId, disputeDocumentId)
                .Execute();

            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(DisputeDocument));
            Assert.AreEqual(disputeDocumentId, response.Id);
            Assert.IsNotNull(response.Base64Content);
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
            var result = ReportingService.FindSettlementDisputesPaged(FirstPage, PageSize)
                .OrderBy(DisputeSortProperty.Id, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, ReportingStartDate)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindSettlementDisputesByDpositIdPaged_Order_By_Id() {
            const string depositId = "DEP_2342423423";

            var result = ReportingService.FindSettlementDisputesPaged(FirstPage, PageSize)
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
            var result = ReportingService.FindSettlementDisputesPaged(FirstPage, PageSize)
                .OrderBy(DisputeSortProperty.Brand, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, ReportingStartDate)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindSettlementDisputesPaged_OrderBy_Stage() {
            var result = ReportingService.FindSettlementDisputesPaged(FirstPage, PageSize)
                .OrderBy(DisputeSortProperty.Stage, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, ReportingStartDate)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindSettlementDisputesPaged_OrderBy_FromStageTimeCreated() {
            var result = ReportingService.FindSettlementDisputesPaged(FirstPage, PageSize)
                .OrderBy(DisputeSortProperty.FromStageTimeCreated, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, ReportingStartDate)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindSettlementDisputesPaged_OrderBy_ToStageTimeCreated() {
            var result = ReportingService.FindSettlementDisputesPaged(FirstPage, PageSize)
                .OrderBy(DisputeSortProperty.ToStageTimeCreated, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, ReportingStartDate)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindSettlementDisputesPaged_OrderBy_AdjustmentFunding() {
            var result = ReportingService.FindSettlementDisputesPaged(FirstPage, PageSize)
                .OrderBy(DisputeSortProperty.AdjustmentFunding, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, ReportingStartDate)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindSettlementDisputesPaged_OrderBy_FromAdjustmentTimeCreated() {
            var result = ReportingService.FindSettlementDisputesPaged(FirstPage, PageSize)
                .OrderBy(DisputeSortProperty.FromAdjustmentTimeCreated, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, ReportingStartDate)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindSettlementDisputesPaged_OrderBy_ToAdjustmentTimeCreated() {
            var result = ReportingService.FindSettlementDisputesPaged(FirstPage, PageSize)
                .OrderBy(DisputeSortProperty.ToAdjustmentTimeCreated, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, ReportingStartDate)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindSettlementDisputesPaged_By_ARN() {
            const string arn = "71400011129688701392096";

            var result = ReportingService.FindSettlementDisputesPaged(FirstPage, PageSize)
                .OrderBy(DisputeSortProperty.Id, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, ReportingStartDate)
                .And(SearchCriteria.AquirerReferenceNumber, arn)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
            Assert.IsTrue(result.Results.TrueForAll(d => d.TransactionARN == arn));
        }

        [TestMethod]
        public void ReportFindSettlementDisputesPaged_By_ARN_NotFound() {
            const string arn = "00000011129654301392121";

            var result = ReportingService.FindSettlementDisputesPaged(FirstPage, PageSize)
                .OrderBy(DisputeSortProperty.Id, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, ReportingStartDate)
                .And(SearchCriteria.AquirerReferenceNumber, arn)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
            Assert.IsTrue(result.Results.Count == 0);
        }

        [TestMethod]
        public void ReportFindSettlementDisputesPaged_By_Brand() {
            const string brand = "VISA";

            var result = ReportingService.FindSettlementDisputesPaged(FirstPage, PageSize)
                .OrderBy(DisputeSortProperty.Id, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, ReportingStartDate)
                .And(SearchCriteria.CardBrand, brand)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindSettlementDisputesPaged_By_Brand_NotFound() {
            const string brand = "MASTERCAR";

            var result = ReportingService.FindSettlementDisputesPaged(FirstPage, PageSize)
                .OrderBy(DisputeSortProperty.Id, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, ReportingStartDate)
                .And(SearchCriteria.CardBrand, brand)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
            Assert.IsTrue(result.Results.Count == 0);
        }

        [TestMethod]
        public void ReportFindSettlementDisputesPaged_By_Status() {
            var result = ReportingService.FindSettlementDisputesPaged(FirstPage, PageSize)
                .OrderBy(DisputeSortProperty.Id, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, ReportingStartDate)
                .And(SearchCriteria.DisputeStatus, DisputeStatus.Funded)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
            Assert.IsTrue(result.Results.TrueForAll(d => d.CaseStatus == EnumConverter.GetMapping(Target.GP_API, DisputeStatus.Funded)));
        }

        [TestMethod]
        public void ReportFindSettlementDisputesPaged_By_DepositDate() {
            var result = ReportingService.FindSettlementDisputesPaged(FirstPage, PageSize)
                .OrderBy(DisputeSortProperty.Id, SortDirection.Descending)
                .Where(DataServiceCriteria.StartDepositDate, ReportingStartDate)
                .And(DataServiceCriteria.EndDepositDate, ReportingEndDate)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
            Assert.IsTrue(result.Results.TrueForAll(d =>
                d.DepositDate >= ReportingStartDate && d.DepositDate <= ReportingEndDate));
        }

        [TestMethod]
        public void ReportFindSettlementDisputesPaged_By_Stage() {
            foreach (DisputeStage stage in Enum.GetValues(typeof(DisputeStage))) {
                var result = ReportingService.FindSettlementDisputesPaged(FirstPage, PageSize)
                    .OrderBy(DisputeSortProperty.Id, SortDirection.Descending)
                    .Where(DataServiceCriteria.StartStageDate, ReportingStartDate)
                    .And(SearchCriteria.DisputeStage, stage)
                    .Execute();
                Assert.IsNotNull(result?.Results);
                Assert.IsTrue(result.Results is List<DisputeSummary>);
                Assert.IsTrue(result.Results.TrueForAll(d => d.CaseStage == stage.ToString().ToUpper()));
            }
        }

        [TestMethod]
        public void ReportFindSettlementDisputesPaged_By_From_And_To_Stage_Time_Created() {
            var result = ReportingService.FindSettlementDisputesPaged(FirstPage, PageSize)
                .OrderBy(DisputeSortProperty.Id, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, ReportingStartDate)
                .And(DataServiceCriteria.EndStageDate, ReportingLastMonthDate)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
            Assert.IsTrue(result.Results.TrueForAll(d =>
                d.CaseIdTime >= ReportingStartDate.Date && d.CaseIdTime <= ReportingLastMonthDate.Date));
        }

        [TestMethod]
        public void ReportFindSettlementDisputesPaged_By_MerchantId() {
            const string merchantId = "101023947262";

            var result = ReportingService.FindSettlementDisputesPaged(FirstPage, PageSize)
                .OrderBy(DisputeSortProperty.Id, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, ReportingStartDate)
                .And(DataServiceCriteria.MerchantId, merchantId)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
            Assert.IsTrue(result.Results.TrueForAll(d => d.CaseMerchantId == merchantId));
        }

        [TestMethod]
        public void ReportFindSettlementDisputesPaged_By_SystemHierarchy() {
            const string systemHierarchy = "055-70-024-011-019";

            var result = ReportingService.FindSettlementDisputesPaged(FirstPage, PageSize)
                .OrderBy(DisputeSortProperty.Id, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, ReportingStartDate)
                .And(DataServiceCriteria.SystemHierarchy, systemHierarchy)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
            Assert.IsTrue(result.Results.TrueForAll(d => d.MerchantHierarchy == systemHierarchy));
        }

        [TestMethod]
        public void ReportFindSettlementDisputesPaged_By_Wrong_MerchantId() {
            const string merchantId = "000023947222";

            var result = ReportingService.FindSettlementDisputesPaged(FirstPage, PageSize)
                .OrderBy(DisputeSortProperty.Id, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, ReportingStartDate)
                .And(DataServiceCriteria.MerchantId, merchantId)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
            Assert.IsTrue(result.Results.Count == 0);
        }

        [TestMethod]
        public void ReportFindSettlementDisputesPaged_By_Wrong_SystemHierarchy() {
            const string systemHierarchy = "000-70-024-011-111";

            var result = ReportingService.FindSettlementDisputesPaged(FirstPage, PageSize)
                .OrderBy(DisputeSortProperty.Id, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, ReportingStartDate)
                .And(DataServiceCriteria.SystemHierarchy, systemHierarchy)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
            Assert.IsTrue(result.Results.Count == 0);
        }

        [TestMethod]
        public void ReportFindSettlementDisputesPaged_Order_By_ARN() {
            var result = ReportingService.FindSettlementDisputesPaged(FirstPage, PageSize)
                .OrderBy(DisputeSortProperty.ARN, SortDirection.Descending)
                .Where(DataServiceCriteria.StartStageDate, ReportingStartDate)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
        }

        [TestMethod]
        public void ReportFindSettlementDisputesPaged_Order_By_Id_With_Status_UnderReview() {
            var result = ReportingService.FindSettlementDisputesPaged(FirstPage, PageSize)
                .OrderBy(DisputeSortProperty.Id, SortDirection.Ascending)
                .Where(DataServiceCriteria.StartStageDate, ReportingStartDate)
                .And(SearchCriteria.DisputeStatus, DisputeStatus.UnderReview)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DisputeSummary>);
        }
        #endregion
    }
}