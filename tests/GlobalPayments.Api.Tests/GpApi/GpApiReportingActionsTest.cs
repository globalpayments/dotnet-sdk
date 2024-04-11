using System;
using System.Collections.Generic;
using System.Linq;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.GpApi
{
    [TestClass]
    public class GpApiReportingActionsTest : BaseGpApiReportingTest {
        private static ActionSummary SampleAction;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context) {
            var gpApiConfig = GpApiConfigSetup(AppId, AppKey, Channel.CardNotPresent);
            ServicesContainer.ConfigureService(gpApiConfig);

            SampleAction = ReportingService.FindActionsPaged(1, 1)
                .Where(SearchCriteria.StartDate, DateTime.UtcNow.AddDays(-5))
                .Execute().Results?.FirstOrDefault();
        }

        [TestMethod]
        public void ReportActionDetail() {
            var actionId = SampleAction.Id;
            
            var response = ReportingService.ActionDetail(actionId)
                .Execute();
            Assert.IsNotNull(response);
            Assert.IsTrue(response is ActionSummary);
            Assert.AreEqual(actionId, response.Id);
        }

        [TestMethod]
        public void ReportActionDetailWithRandomId() {
            var actionId = $"ACT_{Guid.NewGuid()}";
            var exceptionCaught = false;

            try {
                ReportingService.ActionDetail(actionId)
                    .Execute();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("RESOURCE_NOT_FOUND", ex.ResponseCode);
                Assert.AreEqual("40118", ex.ResponseMessage);
                Assert.AreEqual($"Status Code: NotFound - Actions {actionId} not found at this /ucp/actions/{actionId}",
                    ex.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void ReportFindActionsPaged_By_Id() {
            var actionId = SampleAction.Id;
            
            var result = ReportingService.FindActionsPaged(FirstPage,PageSize)
                .Where(SearchCriteria.ActionId, actionId)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<ActionSummary>);
            Assert.IsTrue(result.Results.TrueForAll(a => a.Id == actionId));
        }

        [TestMethod]
        public void ReportFindActionsPaged_By_Random_Id() {
            var actionId = $"ACT_{Guid.NewGuid()}";

            var result = ReportingService.FindActionsPaged(FirstPage,PageSize)
                .Where(SearchCriteria.ActionId, actionId)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.AreEqual(0, result.TotalRecordCount);
        }

        [TestMethod]
        public void ReportFindActionsPaged_By_Type() {
            var actionType = SampleAction.Type;

            var result = ReportingService.FindActionsPaged(FirstPage,PageSize)
                .Where(SearchCriteria.ActionType, actionType)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<ActionSummary>);
            Assert.IsTrue(result.Results.TrueForAll(a => a.Type == actionType));
        }

        [TestMethod]
        public void ReportFindActionsPaged_By_Random_Type() {
            const string actionType = "USERS";

            var result = ReportingService.FindActionsPaged(FirstPage,PageSize)
                .Where(SearchCriteria.ActionType, actionType)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.AreEqual(0, result.TotalRecordCount);
        }

        [TestMethod]
        public void ReportFindActionsPaged_By_Resource() {
            var resource = SampleAction.Resource;

            var result = ReportingService.FindActionsPaged(FirstPage,PageSize)
                .Where(SearchCriteria.Resource, resource)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<ActionSummary>);
            Assert.IsTrue(result.Results.TrueForAll(a => a.Resource == resource));
        }

        [TestMethod]
        public void ReportFindActionsPaged_By_ResourceStatus() {
            var resourceStatus = !string.IsNullOrEmpty(SampleAction.ResourceStatus)
                ? SampleAction.ResourceStatus
                : "AVAILABLE";

            var result = ReportingService.FindActionsPaged(FirstPage,PageSize)
                .Where(SearchCriteria.ResourceStatus, resourceStatus)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<ActionSummary>);
            Assert.IsTrue(result.Results.TrueForAll(a => a.ResourceStatus == resourceStatus));
        }

        [TestMethod]
        public void ReportFindActionsPaged_By_ResourceId() {
            var resourceId = SampleAction.ResourceId;

            if (string.IsNullOrWhiteSpace(resourceId)) {
                resourceId = ReportingService.FindActionsPaged(FirstPage,PageSize)
                    .Where(SearchCriteria.StartDate, ReportingStartDate)
                    .Execute().Results?.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.ResourceId))?.ResourceId;
            }

            var result = ReportingService.FindActionsPaged(FirstPage,PageSize)
                .Where(SearchCriteria.ResourceId, resourceId)
                .And(SearchCriteria.Resource, "TRANSACTION")
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<ActionSummary>);
            Assert.IsTrue(result.Results.TrueForAll(a => a.ResourceId == resourceId));
        }

        [TestMethod]
        public void ReportFindActionsPaged_By_StartDate_And_EndDate() {
            var result = ReportingService.FindActionsPaged(FirstPage,PageSize)
                .Where(SearchCriteria.StartDate, ReportingStartDate)
                .And(SearchCriteria.EndDate, ReportingEndDate)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<ActionSummary>);
            Assert.IsTrue(result.Results.TrueForAll(a =>
                a.TimeCreated.Date >= ReportingStartDate.Date && a.TimeCreated.Date <= ReportingEndDate.Date));
        }

        [TestMethod]
        public void ReportFindActionsPaged_By_MerchantName() {
            var merchantName = SampleAction.MerchantName;

            var result = ReportingService.FindActionsPaged(FirstPage,PageSize)
                .Where(SearchCriteria.MerchantName, merchantName)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<ActionSummary>);
            Assert.IsTrue(result.Results.TrueForAll(a => a.MerchantName == merchantName));
        }

        [TestMethod]
        public void ReportFindActionsPaged_By_Random_MerchantName() {
            var exceptionCaught = false;
            try {
                ReportingService.FindActionsPaged(FirstPage,PageSize)
                    .Where(SearchCriteria.MerchantName, Guid.NewGuid().ToString())
                    .Execute();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("ACTION_NOT_AUTHORIZED", ex.ResponseCode);
                Assert.AreEqual("40003", ex.ResponseMessage);
                Assert.AreEqual("Status Code: Forbidden - Token does not match merchant_name in the request",
                    ex.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void ReportFindActionsPaged_By_AccountName() {
            var accountName = !string.IsNullOrEmpty(SampleAction.AccountName)
                ? SampleAction.AccountName
                : "Tokenization";

            var result = ReportingService.FindActionsPaged(FirstPage,PageSize)
                .Where(SearchCriteria.AccountName, accountName)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<ActionSummary>);
            Assert.IsTrue(result.Results.TrueForAll(a => a.AccountName == accountName));
        }

        [TestMethod]
        public void ReportFindActionsPaged_By_AppName() {
            var appName = SampleAction.AppName;

            var result = ReportingService.FindActionsPaged(FirstPage,PageSize)
                .Where(SearchCriteria.AppName, appName)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<ActionSummary>);
            Assert.IsTrue(result.Results.TrueForAll(a => a.AppName == appName));
        }

        [TestMethod]
        public void ReportFindActionsPaged_By_Version() {
            var version = SampleAction.Version;

            var result = ReportingService.FindActionsPaged(FirstPage,PageSize)
                .Where(SearchCriteria.Version, version)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<ActionSummary>);
            Assert.IsTrue(result.Results.TrueForAll(a => a.Version == version));
        }

        [TestMethod]
        public void ReportFindActionsPaged_By_WrongVersion() {
            const string version = "2020-05-10";

            var result = ReportingService.FindActionsPaged(FirstPage,PageSize)
                .Where(SearchCriteria.Version, version)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<ActionSummary>);
            Assert.IsTrue(result.Results.TrueForAll(a => a.Version == version));
            Assert.AreEqual(0, result.TotalRecordCount);
        }

        [TestMethod]
        public void ReportFindActionsPaged_By_ResponseCode() {
            var responseCode = !string.IsNullOrEmpty(SampleAction.ResponseCode) ? SampleAction.ResponseCode : "SUCCESS";

            var result = ReportingService.FindActionsPaged(FirstPage,PageSize)
                .Where(SearchCriteria.ResponseCode, responseCode)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<ActionSummary>);
            Assert.IsTrue(result.Results.TrueForAll(a => a.ResponseCode == responseCode));
        }

        [TestMethod]
        public void ReportFindActionsPaged_By_ResponseCode_Declined() {
            const string responseCode = "DECLINED";

            var result = ReportingService.FindActionsPaged(FirstPage,PageSize)
                .Where(SearchCriteria.ResponseCode, responseCode)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<ActionSummary>);
            Assert.IsTrue(result.Results.TrueForAll(a => a.ResponseCode == responseCode));
        }

        [TestMethod]
        public void ReportFindActionsPaged_By_HttpResponseCode() {
            var httpResponseCode = SampleAction.HttpResponseCode;

            var result = ReportingService.FindActionsPaged(FirstPage,PageSize)
                .Where(SearchCriteria.HttpResponseCode, httpResponseCode)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<ActionSummary>);
            Assert.IsTrue(result.Results.TrueForAll(a => a.HttpResponseCode == httpResponseCode));
        }

        [TestMethod]
        public void ReportFindActionsPaged_By_HttpResponseCode_502() {
            const string httpResponseCode = "502";

            var result = ReportingService.FindActionsPaged(FirstPage,PageSize)
                .Where(SearchCriteria.HttpResponseCode, httpResponseCode)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<ActionSummary>);
            Assert.IsTrue(result.Results.TrueForAll(a => a.HttpResponseCode == httpResponseCode));
        }

        [TestMethod]
        public void ReportFindActionsPaged_OrderBy_TimeCreated_Ascending() {
            var result = ReportingService.FindActionsPaged(FirstPage,PageSize)
                .OrderBy(ActionSortProperty.TimeCreated, SortDirection.Ascending)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<ActionSummary>);
            Assert.IsTrue(result.Results.SequenceEqual(result.Results.OrderBy(r => r.TimeCreated)));
        }

        [TestMethod]
        public void ReportFindActionsPaged_OrderBy_TimeCreated_Descending() {
            var result = ReportingService.FindActionsPaged(FirstPage,PageSize)
                .OrderBy(ActionSortProperty.TimeCreated, SortDirection.Descending)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<ActionSummary>);
            Assert.IsTrue(result.Results.SequenceEqual(result.Results.OrderByDescending(r => r.TimeCreated)));
        }

        [TestMethod]
        public void ReportFindActionsPaged_OrderBy_TimeCreated() {
            var resultDesc = ReportingService.FindActionsPaged(FirstPage,PageSize)
                .OrderBy(ActionSortProperty.TimeCreated, SortDirection.Descending)
                .Execute();
            Assert.IsNotNull(resultDesc?.Results);
            Assert.IsTrue(resultDesc.Results is List<ActionSummary>);
            Assert.IsTrue(resultDesc.Results.SequenceEqual(resultDesc.Results.OrderByDescending(r => r.TimeCreated)));

            var resultAsc = ReportingService.FindActionsPaged(FirstPage,PageSize)
                .OrderBy(ActionSortProperty.TimeCreated, SortDirection.Ascending)
                .Execute();
            Assert.IsNotNull(resultAsc?.Results);
            Assert.IsTrue(resultAsc.Results is List<ActionSummary>);
            Assert.IsTrue(resultAsc.Results.SequenceEqual(resultAsc.Results.OrderBy(r => r.TimeCreated)));

            Assert.AreNotSame(resultAsc, resultDesc);
        }

        [TestMethod]
        public void ReportFindActionsPaged_By_MultipleFilters() {
            var sampleAction = SampleAction;
            if (string.IsNullOrWhiteSpace(sampleAction.AccountName) ||
                string.IsNullOrWhiteSpace(sampleAction.MerchantName) || string.IsNullOrWhiteSpace(sampleAction.Version))
            {
                sampleAction = ReportingService.FindActionsPaged(FirstPage,PageSize)
                    .Where(SearchCriteria.StartDate, DateTime.UtcNow.AddYears(-5))
                    .Execute().Results?.FirstOrDefault(x =>
                        !string.IsNullOrWhiteSpace(x.AccountName) && !string.IsNullOrWhiteSpace(x.MerchantName) &&
                        !string.IsNullOrWhiteSpace(x.Version));
            }

            const string actionType = "AUTHORIZE";
            const string resource = "TRANSACTIONS";
            const string resourceStatus = "DECLINED";
            var accountName = sampleAction.AccountName;
            var merchantName = sampleAction.MerchantName;
            var version = sampleAction.Version;

            PagedResult<ActionSummary> result = ReportingService.FindActionsPaged(FirstPage,PageSize)
                .Where(SearchCriteria.ActionType, actionType)
                .And(SearchCriteria.Resource, resource)
                .And(SearchCriteria.ResourceStatus, resourceStatus)
                .And(SearchCriteria.AccountName, accountName)
                .And(SearchCriteria.MerchantName, merchantName)
                .And(SearchCriteria.Version, version)
                .And(SearchCriteria.StartDate, ReportingStartDate)
                .And(SearchCriteria.EndDate, ReportingLastMonthDate)
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