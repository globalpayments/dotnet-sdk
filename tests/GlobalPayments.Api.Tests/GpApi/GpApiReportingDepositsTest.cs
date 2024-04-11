using System;
using System.Collections.Generic;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.GpApi {
    
    [TestClass]
    public class GpApiReportingDepositsTest : BaseGpApiReportingTest {
        
        [ClassInitialize]
        public static void ClassInitialize(TestContext context) {
            var gpApiConfig = GpApiConfigSetup(AppId, AppKey, Channel.CardNotPresent);
            ServicesContainer.ConfigureService(gpApiConfig);
        }

        #region Deposits

        [TestMethod]
        public void ReportDepositDetail() {
            const string depositId = "DEP_2342423423";

            var response = ReportingService.DepositDetail(depositId)
                .Execute();
            Assert.IsNotNull(response);
            Assert.IsTrue(response is DepositSummary);
            Assert.AreEqual(depositId, response.DepositId);
        }

        [TestMethod]
        public void ReportDepositDetailWrongId() {
            const string depositId = "DEP_1112423111";
            var exceptionCaught = false;

            try {
                ReportingService.DepositDetail(depositId)
                    .Execute();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("RESOURCE_NOT_FOUND", ex.ResponseCode);
                Assert.AreEqual("40118", ex.ResponseMessage);
                Assert.IsTrue(ex.Message.StartsWith("Status Code: NotFound - Deposits DEP_1112423111 not found"));
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void ReportFindDepositsPaged_By_StartDate_Order_By_TimeCreated() {
            var result = ReportingService.FindDepositsPaged(FirstPage, PageSize)
                .OrderBy(DepositSortProperty.TimeCreated, SortDirection.Descending)
                .Where(SearchCriteria.StartDate, ReportingStartDate)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DepositSummary>);
            Assert.IsTrue(result.Results.TrueForAll(d => d.DepositDate?.Date >= ReportingStartDate.Date));
        }

        [TestMethod]
        public void ReportFindDepositsPaged_By_EndDate_Order_By_TimeCreated() {
            var result = ReportingService.FindDepositsPaged(FirstPage, PageSize)
                .OrderBy(DepositSortProperty.TimeCreated, SortDirection.Descending)
                .Where(SearchCriteria.StartDate, ReportingStartDate)
                .And(SearchCriteria.EndDate, ReportingEndDate)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DepositSummary>);
            Assert.IsTrue(result.Results.TrueForAll(d =>
                d.DepositDate?.Date >= ReportingStartDate.Date && d.DepositDate?.Date <= ReportingEndDate.Date));
        }

        [TestMethod]
        public void ReportFindDepositsPaged_By_Amount() {
            const decimal amount = 141;

            var result = ReportingService.FindDepositsPaged(FirstPage, PageSize)
                .OrderBy(DepositSortProperty.TimeCreated, SortDirection.Descending)
                .Where(DataServiceCriteria.Amount, amount)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DepositSummary>);
            Assert.IsTrue(result.Results.TrueForAll(d => d.Amount == amount));
        }

        [TestMethod]
        public void ReportFindDepositsPaged_By_NotFoundAmount() {
            const decimal amount = 140;

            var result = ReportingService.FindDepositsPaged(FirstPage, PageSize)
                .OrderBy(DepositSortProperty.TimeCreated, SortDirection.Descending)
                .Where(DataServiceCriteria.Amount, amount)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DepositSummary>);
            Assert.IsTrue(result.Results.Count == 0);
        }

        [TestMethod]
        public void ReportFindDepositsPaged_By_Status() {
            foreach (DepositStatus depositStatus in Enum.GetValues(typeof(DepositStatus))) {
                var result = ReportingService.FindDepositsPaged(FirstPage, PageSize)
                    .OrderBy(DepositSortProperty.TimeCreated, SortDirection.Descending)
                    .Where(SearchCriteria.DepositStatus, depositStatus)
                    .Execute();
                Assert.IsNotNull(result?.Results);
                Assert.IsTrue(result.Results is List<DepositSummary>);
                if (depositStatus == DepositStatus.SplitFunding || depositStatus == DepositStatus.Reserved || depositStatus == DepositStatus.Irregular) {
                    continue;
                }
                Assert.IsTrue(result.Results.TrueForAll(d => d.Status.Trim() == depositStatus.ToString().ToUpper()));
            }
        }

        [TestMethod]
        public void ReportFindDepositsPaged_By_Masked_Account_Number_Last4() {
            const string maskedAccountNumberLast4 = "9999";

            var result = ReportingService.FindDepositsPaged(FirstPage, PageSize)
                .OrderBy(DepositSortProperty.TimeCreated, SortDirection.Descending)
                .Where(SearchCriteria.AccountNumberLastFour, maskedAccountNumberLast4)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DepositSummary>);
            Assert.IsTrue(result.Results.Count > 0);
        }

        [TestMethod]
        public void ReportFindDepositsPaged_By_SystemHierarchy() {
            const string systemHierarchy = "055-70-024-011-019";

            var result = ReportingService.FindDepositsPaged(FirstPage, PageSize)
                .OrderBy(DepositSortProperty.TimeCreated, SortDirection.Descending)
                .Where(DataServiceCriteria.SystemHierarchy, systemHierarchy)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DepositSummary>);
            Assert.IsTrue(result.Results.TrueForAll(d => d.MerchantHierarchy == systemHierarchy));
        }

        [TestMethod]
        public void ReportFindDepositsPaged_By_WrongSystemHierarchy() {
            const string systemHierarchy = "042-70-013-011-018";

            var result = ReportingService.FindDepositsPaged(FirstPage, PageSize)
                .OrderBy(DepositSortProperty.TimeCreated, SortDirection.Descending)
                .Where(DataServiceCriteria.SystemHierarchy, systemHierarchy)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DepositSummary>);
            Assert.IsTrue(result.Results.Count == 0);
        }

        [TestMethod]
        public void ReportFindDepositsPaged_By_RandomUUIDSystemHierarchy() {
            var exceptionCaught = false;
            try {
                ReportingService.FindDepositsPaged(FirstPage, PageSize)
                    .OrderBy(DepositSortProperty.TimeCreated, SortDirection.Descending)
                    .Where(DataServiceCriteria.SystemHierarchy, Guid.NewGuid().ToString())
                    .Execute();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("INVALID_REQUEST_DATA", ex.ResponseCode);
                Assert.AreEqual("40105", ex.ResponseMessage);
                Assert.IsTrue(ex.Message.Contains("Invalid Value provided in the input field - system.hierarch"));
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void ReportFindDepositsPaged_WithoutFromTimeCreated() {
            var exceptionCaught = false;
            try {
                ReportingService.FindDepositsPaged(FirstPage, PageSize)
                    .OrderBy(DepositSortProperty.TimeCreated, SortDirection.Descending)
                    .Where(DataServiceCriteria.SystemHierarchy, Guid.NewGuid().ToString())
                    .Execute();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("INVALID_REQUEST_DATA", ex.ResponseCode);
                Assert.AreEqual("40105", ex.ResponseMessage);
                Assert.IsTrue(ex.Message.Contains("Invalid Value provided in the input field - system.hierarch"));
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void ReportFindDepositsPaged_By_MerchantId() {
            const string merchantId = "101023947262";

            var result = ReportingService.FindDepositsPaged(FirstPage, PageSize)
                .OrderBy(DepositSortProperty.TimeCreated, SortDirection.Descending)
                .Where(DataServiceCriteria.MerchantId, merchantId)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DepositSummary>);
            Assert.IsTrue(result.Results.TrueForAll(d => d.MerchantNumber == merchantId));
        }

        [TestMethod]
        public void ReportFindDepositsPaged_By_Wrong_MerchantId() {
            const string merchantId = "000023985843";

            var result = ReportingService.FindDepositsPaged(FirstPage, PageSize)
                .OrderBy(DepositSortProperty.TimeCreated, SortDirection.Descending)
                .Where(DataServiceCriteria.MerchantId, merchantId)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DepositSummary>);
            Assert.IsTrue(result.Results.Count == 0);
        }

        [TestMethod]
        public void ReportFindDepositsPaged_By_DepositId() {
            const string depositId = "DEP_2342423440";

            var result = ReportingService.FindDepositsPaged(FirstPage, PageSize)
                .OrderBy(DepositSortProperty.TimeCreated, SortDirection.Descending)
                .WithDepositReference(depositId)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results.Count == 1);
            Assert.IsTrue(result.Results.TrueForAll(d => d.DepositId == depositId));
        }

        [TestMethod]
        public void ReportFindDepositsPaged_By_WrongDepositId() {
            const string depositId = "DEP_1112423111";

            var result = ReportingService.FindDepositsPaged(FirstPage, PageSize)
                .OrderBy(DepositSortProperty.TimeCreated, SortDirection.Descending)
                .WithDepositReference(depositId)
                .Where(SearchCriteria.StartDate, ReportingStartDate)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results.Count == 0);
        }

        [TestMethod]
        public void ReportFindDepositsPaged_Order_By_DepositId() {
            var result = ReportingService.FindDepositsPaged(FirstPage, PageSize)
                .OrderBy(DepositSortProperty.DepositId, SortDirection.Descending)
                .Where(SearchCriteria.StartDate, ReportingStartDate)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DepositSummary>);
        }

        [TestMethod]
        public void ReportFindDepositsPaged_Order_By_Status() {
            var result = ReportingService.FindDepositsPaged(FirstPage, PageSize)
                .OrderBy(DepositSortProperty.Status, SortDirection.Ascending)
                .Where(SearchCriteria.StartDate, ReportingStartDate)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DepositSummary>);
        }

        [TestMethod]
        public void ReportFindDepositsPaged_Order_By_Type() {
            var result = ReportingService.FindDepositsPaged(FirstPage, PageSize)
                .OrderBy(DepositSortProperty.Type, SortDirection.Ascending)
                .Where(SearchCriteria.StartDate, ReportingStartDate)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<DepositSummary>);
        }

        [TestMethod]
        public void CompareResults_reportFindDepositsWithCriteria_OrderBy_DepositId_And_Type() {
            var resultById = ReportingService.FindDepositsPaged(FirstPage, PageSize)
                .OrderBy(DepositSortProperty.DepositId, SortDirection.Descending)
                .Where(SearchCriteria.StartDate, ReportingStartDate)
                .Execute();

            var resultByType = ReportingService.FindDepositsPaged(FirstPage, PageSize)
                .OrderBy(DepositSortProperty.Type, SortDirection.Ascending)
                .Where(SearchCriteria.StartDate, ReportingStartDate)
                .Execute();

            Assert.IsNotNull(resultById?.Results);
            Assert.IsTrue(resultById.Results.Count > 0);
            Assert.IsNotNull(resultByType?.Results);
            Assert.IsTrue(resultByType.Results.Count > 0);

            Assert.AreNotEqual(resultById.Results, resultByType.Results);
        }

        #endregion
    }
}