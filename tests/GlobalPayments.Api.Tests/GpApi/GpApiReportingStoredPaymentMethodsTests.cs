using System;
using System.Collections.Generic;
using System.Linq;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Utils;
using GlobalPayments.Api.Utils.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.GpApi {
    
    [TestClass]
    public class GpApiReportingStoredPaymentMethodsTests : BaseGpApiReportingTest {
        private static string Token;

        private static CreditCardData Card = new CreditCardData {
            Number = "4111111111111111",
            ExpMonth = DateTime.Now.Month,
            ExpYear = DateTime.Now.Year + 1,
            Cvn = "123"
        };

        [ClassInitialize]
        public static void ClassInitialize(TestContext context) {
            ServicesContainer.ConfigureService(new GpApiConfig {
                AppId = APP_ID,
                AppKey = APP_KEY,
                RequestLogger = new RequestConsoleLogger()
            });

            try {
                Token = Card.Tokenize();
                Assert.IsTrue(!string.IsNullOrEmpty(Token), "Token could not be generated.");
            }
            catch (GatewayException ex) {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void ReportStoredPaymentMethodDetail() {
            var response = ReportingService.StoredPaymentMethodDetail(Token)
                .Execute();
            Assert.IsNotNull(response);
            Assert.IsTrue(response is StoredPaymentMethodSummary);
            Assert.AreEqual(Token, response.Id);
        }

        [TestMethod]
        public void ReportStoredPaymentMethodDetailWithNonExistentId() {
            var storedPaymentMethodId = $"PMT_{Guid.NewGuid()}";
            var exceptionCaught = false;
            try {
                ReportingService.StoredPaymentMethodDetail(storedPaymentMethodId)
                    .Execute();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("RESOURCE_NOT_FOUND", ex.ResponseCode);
                Assert.AreEqual("40118", ex.ResponseMessage);
                Assert.AreEqual(
                    $"Status Code: NotFound - PAYMENT_METHODS {storedPaymentMethodId} not found at this /ucp/payment-methods/{storedPaymentMethodId}",
                    ex.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void ReportStoredPaymentMethodDetailWithRandomId() {
            var storedPaymentMethodId = Guid.NewGuid().ToString();
            var exceptionCaught = false;
            try {
                ReportingService.StoredPaymentMethodDetail(storedPaymentMethodId)
                    .Execute();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("INVALID_REQUEST_DATA", ex.ResponseCode);
                Assert.AreEqual("40213", ex.ResponseMessage);
                Assert.AreEqual(
                    $"Status Code: BadRequest - payment_method.id: {storedPaymentMethodId} contains unexpected data",
                    ex.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void ReportFindStoredPaymentMethodsPaged_By_Id() {
            var result = ReportingService.FindStoredPaymentMethodsPaged(FIRST_PAGE, PAGE_SIZE)
                .Where(DataServiceCriteria.StoredPaymentMethodId, Token)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<StoredPaymentMethodSummary>);
            Assert.IsTrue(result.Results.TrueForAll(r => r.Id == Token));
        }

        [TestMethod]
        public void ReportFindStoredPaymentMethodsPaged_By_RandomId() {
            var storedPaymentMethodId = $"PMT_{Guid.NewGuid()}";

            var result = ReportingService.FindStoredPaymentMethodsPaged(FIRST_PAGE, PAGE_SIZE)
                .Where(DataServiceCriteria.StoredPaymentMethodId, storedPaymentMethodId)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.AreEqual(0, result?.Results.Count);
        }

        [TestMethod]
        [Ignore]
        // TODO: Reported the the GP API team
        // Endpoint is retrieving not filtered results
        public void ReportFindStoredPaymentMethodsPaged_By_NumberLast4() {
            var numberLast4 = Card.Number.Substring(Card.Number.Length - 4);
            var result = ReportingService.FindStoredPaymentMethodsPaged(FIRST_PAGE, PAGE_SIZE)
                .Where(DataServiceCriteria.CardNumberLastFour, numberLast4)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<StoredPaymentMethodSummary>);
            Assert.IsTrue(result.Results.TrueForAll(r => r.CardLast4 == "xxxxxxxxxxxx" + numberLast4));
        }

        [TestMethod]
        [Ignore]
        // TODO: Reported the the GP API team
        // Endpoint is retrieving not filtered results
        public void ReportFindStoredPaymentMethodsPaged_By_NumberLast4_Set0000() {
            const string numberLast4 = "0000";
            var result = ReportingService.FindStoredPaymentMethodsPaged(FIRST_PAGE, PAGE_SIZE)
                .Where(DataServiceCriteria.CardNumberLastFour, numberLast4)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.AreEqual(0, result?.Results.Count);
        }

        [TestMethod]
        public void ReportFindStoredPaymentMethodsPaged_By_Reference() {
            var response = ReportingService.StoredPaymentMethodDetail(Token)
                .Execute();
            Assert.IsNotNull(response?.Reference);

            var result = ReportingService.FindStoredPaymentMethodsPaged(FIRST_PAGE, PAGE_SIZE)
                .Where(SearchCriteria.ReferenceNumber, response.Reference)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<StoredPaymentMethodSummary>);
            Assert.IsTrue(result.Results.TrueForAll(r => r.Reference == response.Reference));
        }

        [TestMethod]
        public void ReportFindStoredPaymentMethodsPaged_By_Status() {
            const StoredPaymentMethodStatus status = StoredPaymentMethodStatus.Active;
            var result = ReportingService.FindStoredPaymentMethodsPaged(FIRST_PAGE, PAGE_SIZE)
                .Where(SearchCriteria.StoredPaymentMethodStatus, status)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<StoredPaymentMethodSummary>);
            Assert.IsTrue(result.Results.TrueForAll(r => r.Status == EnumConverter.GetMapping(Target.GP_API, status)));
        }

        [TestMethod]
        public void ReportFindStoredPaymentMethodsPaged_By_StartDate_And_EndDate() {
            var result = ReportingService.FindStoredPaymentMethodsPaged(FIRST_PAGE, PAGE_SIZE)
                .Where(SearchCriteria.StartDate, REPORTING_START_DATE)
                .And(SearchCriteria.EndDate, REPORTING_END_DATE)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<StoredPaymentMethodSummary>);
            Assert.IsTrue(result.Results.TrueForAll(r =>
                r.TimeCreated.Date >= REPORTING_START_DATE.Date && r.TimeCreated.Date <= REPORTING_END_DATE.Date));
        }

        [TestMethod]
        public void ReportFindStoredPaymentMethodsPaged_By_StartDate_And_EndDate_CurrentDay() {
            var result = ReportingService.FindStoredPaymentMethodsPaged(FIRST_PAGE, PAGE_SIZE)
                .Where(SearchCriteria.StartDate, REPORTING_END_DATE)
                .And(SearchCriteria.EndDate, REPORTING_END_DATE)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<StoredPaymentMethodSummary>);
            Assert.IsTrue(result.Results.TrueForAll(t =>
                t.TimeCreated.Date >= REPORTING_END_DATE.Date && t.TimeCreated.Date <= REPORTING_END_DATE.Date));
        }

        [TestMethod]
        public void ReportFindStoredPaymentMethodsPaged_By_StartLastUpdatedDate_And_EndLastUpdatedDate() {
            var result = ReportingService.FindStoredPaymentMethodsPaged(FIRST_PAGE, PAGE_SIZE)
                .Where(DataServiceCriteria.StartLastUpdatedDate, REPORTING_START_DATE)
                .And(DataServiceCriteria.EndLastUpdatedDate, REPORTING_LAST_MONTH_DATE)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<StoredPaymentMethodSummary>);
            //ToDo: There is no way to validate the response data
        }

        [TestMethod]
        public void ReportFindStoredPaymentMethodsPaged_OrderBy_TimeCreated_Ascending() {
            var result = ReportingService.FindStoredPaymentMethodsPaged(FIRST_PAGE, PAGE_SIZE)
                .OrderBy(StoredPaymentMethodSortProperty.TimeCreated, SortDirection.Ascending)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<StoredPaymentMethodSummary>);
            Assert.IsTrue(result.Results.SequenceEqual(result.Results.OrderBy(r => r.TimeCreated)));
        }

        [TestMethod]
        public void ReportFindStoredPaymentMethodsPaged_OrderBy_TimeCreated_Descending() {
            var result = ReportingService.FindStoredPaymentMethodsPaged(FIRST_PAGE, PAGE_SIZE)
                .OrderBy(StoredPaymentMethodSortProperty.TimeCreated, SortDirection.Descending)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<StoredPaymentMethodSummary>);
            Assert.IsTrue(result.Results.SequenceEqual(result.Results.OrderByDescending(r => r.TimeCreated)));
        }

        [TestMethod]
        public void ReportFindStoredPaymentMethodsPaged_OrderBy_TimeCreated() {
            var resultAsc = ReportingService.FindStoredPaymentMethodsPaged(FIRST_PAGE, PAGE_SIZE)
                .OrderBy(StoredPaymentMethodSortProperty.TimeCreated, SortDirection.Ascending)
                .Execute();
            Assert.IsNotNull(resultAsc?.Results);
            Assert.IsTrue(resultAsc.Results is List<StoredPaymentMethodSummary>);
            Assert.IsTrue(resultAsc.Results.SequenceEqual(resultAsc.Results.OrderBy(r => r.TimeCreated)));

            var resultDesc = ReportingService.FindStoredPaymentMethodsPaged(FIRST_PAGE, PAGE_SIZE)
                .OrderBy(StoredPaymentMethodSortProperty.TimeCreated, SortDirection.Descending)
                .Execute();
            Assert.IsNotNull(resultDesc?.Results);
            Assert.IsTrue(resultDesc.Results is List<StoredPaymentMethodSummary>);
            Assert.IsTrue(resultDesc.Results.SequenceEqual(resultDesc.Results.OrderByDescending(r => r.TimeCreated)));

            Assert.IsFalse(resultAsc.Results.SequenceEqual(resultDesc.Results));
        }
    }
}