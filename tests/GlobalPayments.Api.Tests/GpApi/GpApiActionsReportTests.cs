using System;
using System.Collections.Generic;
using System.Linq;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.GpApi {
    [TestClass]
    public class GpApiActionsReportTests {
        private static ActionSummary SampleAction;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context) {
            ServicesContainer.ConfigureService(new GpApiConfig {
                AppId = "P3LRVjtGRGxWQQJDE345mSkEh2KfdAyg",
                AppKey = "ockJr6pv6KFoGiZA",
            });

            SampleAction = ReportingService.FindActionsPaged(1, 1)
                .Where(SearchCriteria.StartDate, DateTime.UtcNow.AddDays(-5))
                .Execute().Results?.FirstOrDefault();
        }

        [TestMethod]
        public void ReportActionDetail() {
            string actionId = SampleAction.Id;
            ActionSummary response = ReportingService.ActionDetail(actionId)
                .Execute();
            Assert.IsNotNull(response);
            Assert.IsTrue(response is ActionSummary);
            Assert.AreEqual(actionId, response.Id);
        }

        [TestMethod]
        public void ReportActionDetailWithRandomId() {
            string actionId = $"ACT_{Guid.NewGuid()}";
            var exceptionCaught = false;

            try {
                ReportingService.ActionDetail(actionId)
                    .Execute();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("RESOURCE_NOT_FOUND", ex.ResponseCode);
                Assert.AreEqual("40118", ex.ResponseMessage);
                Assert.AreEqual($"Status Code: NotFound - Actions {actionId} not found at this /ucp/actions/{actionId}", ex.Message);
            }
            finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void ReportFindActionsPaged_By_Id() {
            string actionId = SampleAction.Id;
            PagedResult<ActionSummary> result = ReportingService.FindActionsPaged(1, 25)
                .Where(SearchCriteria.ActionId, actionId)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<ActionSummary>);
            Assert.IsTrue(result.Results.TrueForAll(a => a.Id == actionId));
        }

        [TestMethod]
        public void ReportFindActionsPaged_By_Random_Id() {
            var actionId = $"ACT_{Guid.NewGuid()}";

            var result = ReportingService.FindActionsPaged(1, 5)
                .Where(SearchCriteria.ActionId, actionId)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.AreEqual(0, result.TotalRecordCount);
        }

        [TestMethod]
        public void ReportFindActionsPaged_By_Type() {
            string actionType = SampleAction.Type;
            PagedResult<ActionSummary> result = ReportingService.FindActionsPaged(1, 25)
                .Where(SearchCriteria.ActionType, actionType)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<ActionSummary>);
            Assert.IsTrue(result.Results.TrueForAll(a => a.Type == actionType));
        }

        [TestMethod]
        public void ReportFindActionsPaged_By_Random_Type() {
            string actionType = "USERS";
            var result = ReportingService.FindActionsPaged(1, 25)
                .Where(SearchCriteria.ActionType, actionType)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.AreEqual(0, result.TotalRecordCount);
        }

        [TestMethod]
        public void ReportFindActionsPaged_By_Resource() {
            string resource = SampleAction.Resource;
            PagedResult<ActionSummary> result = ReportingService.FindActionsPaged(1, 25)
                .Where(SearchCriteria.Resource, resource)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<ActionSummary>);
            Assert.IsTrue(result.Results.TrueForAll(a => a.Resource == resource));
        }

        [TestMethod]
        public void ReportFindActionsPaged_By_ResourceStatus() {
            string resourceStatus = !string.IsNullOrEmpty(SampleAction.ResourceStatus) ? SampleAction.ResourceStatus : "AVAILABLE";
            PagedResult<ActionSummary> result = ReportingService.FindActionsPaged(1, 25)
                .Where(SearchCriteria.ResourceStatus, resourceStatus)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<ActionSummary>);
            Assert.IsTrue(result.Results.TrueForAll(a => a.ResourceStatus == resourceStatus));
        }

        [TestMethod]
        public void ReportFindActionsPaged_By_ResourceId() {
            string resourceId = SampleAction.ResourceId;
            PagedResult<ActionSummary> result = ReportingService.FindActionsPaged(1, 25)
                .Where(SearchCriteria.ResourceId, resourceId)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<ActionSummary>);
            Assert.IsTrue(result.Results.TrueForAll(a => a.ResourceId == resourceId));
        }

        [TestMethod]
        public void ReportFindActionsPaged_By_StartDate_And_EndDate() {
            DateTime startDate = DateTime.UtcNow.AddDays(-30);
            DateTime endDate = DateTime.UtcNow.AddDays(-10);
            PagedResult<ActionSummary> result = ReportingService.FindActionsPaged(1, 25)
                .Where(SearchCriteria.StartDate, startDate)
                .And(SearchCriteria.EndDate, endDate)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<ActionSummary>);
            Assert.IsTrue(result.Results.TrueForAll(a => a.TimeCreated.Date >= startDate.Date && a.TimeCreated.Date <= endDate.Date));
        }

        [TestMethod]
        public void ReportFindActionsPaged_By_MerchantName() {
            string merchantName = SampleAction.MerchantName;
            PagedResult<ActionSummary> result = ReportingService.FindActionsPaged(1, 25)
                .Where(SearchCriteria.MerchantName, merchantName)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<ActionSummary>);
            Assert.IsTrue(result.Results.TrueForAll(a => a.MerchantName == merchantName));
        }

        [TestMethod]
        public void ReportFindActionsPaged_By_Random_MerchantName() {
            var merchantName = Guid.NewGuid().ToString();
            var exceptionCaught = false;
            try {
                ReportingService.FindActionsPaged(1, 25)
                    .Where(SearchCriteria.MerchantName, merchantName)
                    .Execute();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("ACTION_NOT_AUTHORIZED", ex.ResponseCode);
                Assert.AreEqual("40003", ex.ResponseMessage);
                Assert.AreEqual("Status Code: Forbidden - Token does not match merchant_name in the request", ex.Message);
            }
            finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void ReportFindActionsPaged_By_AccountName() {
            string accountName = !string.IsNullOrEmpty(SampleAction.AccountName) ? SampleAction.AccountName : "Tokenization";
            PagedResult<ActionSummary> result = ReportingService.FindActionsPaged(1, 25)
                .Where(SearchCriteria.AccountName, accountName)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<ActionSummary>);
            Assert.IsTrue(result.Results.TrueForAll(a => a.AccountName == accountName));
        }

        [TestMethod]
        public void ReportFindActionsPaged_By_AppName() {
            string appName = SampleAction.AppName;
            PagedResult<ActionSummary> result = ReportingService.FindActionsPaged(1, 25)
                .Where(SearchCriteria.AppName, appName)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<ActionSummary>);
            Assert.IsTrue(result.Results.TrueForAll(a => a.AppName == appName));
        }

        [TestMethod]
        public void ReportFindActionsPaged_By_Version() {
            string version = SampleAction.Version;
            PagedResult<ActionSummary> result = ReportingService.FindActionsPaged(1, 25)
                .Where(SearchCriteria.Version, version)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<ActionSummary>);
            Assert.IsTrue(result.Results.TrueForAll(a => a.Version == version));
        }

        [TestMethod]
        public void ReportFindActionsPaged_By_WrongVersion() {
            string version = "2020-05-10";
            var result = ReportingService.FindActionsPaged(1, 25)
                .Where(SearchCriteria.Version, version)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<ActionSummary>);
            Assert.IsTrue(result.Results.TrueForAll(a => a.Version == version));
            Assert.AreEqual(0, result.TotalRecordCount);
        }

        [TestMethod]
        public void ReportFindActionsPaged_By_ResponseCode() {
            string responseCode = SampleAction.ResponseCode;
            PagedResult<ActionSummary> result = ReportingService.FindActionsPaged(1, 25)
                .Where(SearchCriteria.ResponseCode, responseCode)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<ActionSummary>);
            Assert.IsTrue(result.Results.TrueForAll(a => a.ResponseCode == responseCode));
        }

        [TestMethod]
        public void ReportFindActionsPaged_By_ResponseCode_Declined() {
            string responseCode = "DECLINED";
            var result = ReportingService.FindActionsPaged(1, 25)
                .Where(SearchCriteria.ResponseCode, responseCode)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<ActionSummary>);
            Assert.IsTrue(result.Results.TrueForAll(a => a.ResponseCode == responseCode));
        }

        [TestMethod]
        public void ReportFindActionsPaged_By_HttpResponseCode() {
            string httpResponseCode = SampleAction.HttpResponseCode;
            PagedResult<ActionSummary> result = ReportingService.FindActionsPaged(1, 25)
                .Where(SearchCriteria.HttpResponseCode, httpResponseCode)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<ActionSummary>);
            Assert.IsTrue(result.Results.TrueForAll(a => a.HttpResponseCode == httpResponseCode));
        }

        [TestMethod]
        public void ReportFindActionsPaged_By_HttpResponseCode_502() {
            string httpResponseCode = "502";
            var result = ReportingService.FindActionsPaged(1, 25)
                .Where(SearchCriteria.HttpResponseCode, httpResponseCode)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<ActionSummary>);
            Assert.IsTrue(result.Results.TrueForAll(a => a.HttpResponseCode == httpResponseCode));
        }

        [TestMethod]
        public void ReportFindActionsPaged_OrderBy_TimeCreated_Ascending() {
            PagedResult<ActionSummary> result = ReportingService.FindActionsPaged(1, 25)
                .OrderBy(ActionSortProperty.TimeCreated, SortDirection.Ascending)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<ActionSummary>);
            Assert.IsTrue(result.Results.SequenceEqual(result.Results.OrderBy(r => r.TimeCreated)));
        }

        [TestMethod]
        public void ReportFindActionsPaged_OrderBy_TimeCreated_Descending() {
            PagedResult<ActionSummary> result = ReportingService.FindActionsPaged(1, 25)
                .OrderBy(ActionSortProperty.TimeCreated, SortDirection.Descending)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<ActionSummary>);
            Assert.IsTrue(result.Results.SequenceEqual(result.Results.OrderByDescending(r => r.TimeCreated)));
        }

        [TestMethod]
        public void ReportFindActionsPaged_OrderBy_TimeCreated() {
            var resultDesc = ReportingService.FindActionsPaged(1, 25)
                .OrderBy(ActionSortProperty.TimeCreated, SortDirection.Descending)
                .Execute();
            Assert.IsNotNull(resultDesc?.Results);
            Assert.IsTrue(resultDesc.Results is List<ActionSummary>);
            Assert.IsTrue(resultDesc.Results.SequenceEqual(resultDesc.Results.OrderByDescending(r => r.TimeCreated)));

            var resultAsc = ReportingService.FindActionsPaged(1, 25)
                .OrderBy(ActionSortProperty.TimeCreated, SortDirection.Ascending)
                .Execute();
            Assert.IsNotNull(resultAsc?.Results);
            Assert.IsTrue(resultAsc.Results is List<ActionSummary>);
            Assert.IsTrue(resultAsc.Results.SequenceEqual(resultAsc.Results.OrderBy(r => r.TimeCreated)));

            Assert.AreNotSame(resultAsc, resultDesc);
        }

        [TestMethod]
        public void ReportFindActionsPaged_By_MultipleFilters() {
            string actionType = "AUTHORIZE";
            string resource = "TRANSACTIONS";
            string resourceStatus = "DECLINED";
            string accountName = "Transaction_Processing";
            string merchantName = "Sandbox_merchant_2";
            string version = "2020-12-22";
            var startDate = DateTime.UtcNow.AddDays(-30);
            var endDate = DateTime.UtcNow.AddDays(-20);

            PagedResult<ActionSummary> result = ReportingService.FindActionsPaged(1, 25)
                .Where(SearchCriteria.ActionType, actionType)
                .And(SearchCriteria.Resource, resource)
                .And(SearchCriteria.ResourceStatus, resourceStatus)
                .And(SearchCriteria.AccountName, accountName)
                .And(SearchCriteria.MerchantName, merchantName)
                .And(SearchCriteria.Version, version)
                .And(SearchCriteria.StartDate, startDate)
                .And(SearchCriteria.EndDate, endDate)
                .Execute();

            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<ActionSummary>);
            Assert.IsTrue(result.Results.TrueForAll(a => a.Type == actionType));
            Assert.IsTrue(result.Results.TrueForAll(a => a.Resource == resource));
            Assert.IsTrue(result.Results.TrueForAll(a => a.ResourceStatus == resourceStatus));
            Assert.IsTrue(result.Results.TrueForAll(a => a.AccountName == accountName));
            Assert.IsTrue(result.Results.TrueForAll(a => a.MerchantName == merchantName));
            Assert.IsTrue(result.Results.TrueForAll(a => a.Version == version));
        }
    }
}