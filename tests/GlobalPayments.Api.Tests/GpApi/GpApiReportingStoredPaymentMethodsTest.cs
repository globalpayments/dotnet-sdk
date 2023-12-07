using System;
using System.Collections.Generic;
using System.Linq;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.GpApi {
    
    [TestClass]
    public class GpApiReportingStoredPaymentMethodsTest : BaseGpApiReportingTest {
        private static string Token;

        private static CreditCardData Card = new CreditCardData {
            Number = "4242424242424242",
            ExpMonth = DateTime.Now.Month,
            ExpYear = DateTime.Now.Year + 1,
            Cvn = "123"
        };

        [ClassInitialize]
        public static void ClassInitialize(TestContext context) {
            var gpApiConfig = GpApiConfigSetup(AppId, AppKey, Channel.CardNotPresent);
            ServicesContainer.ConfigureService(gpApiConfig);

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
            var result = ReportingService.FindStoredPaymentMethodsPaged(FirstPage, PageSize)
                .Where(DataServiceCriteria.StoredPaymentMethodId, Token)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<StoredPaymentMethodSummary>);
            Assert.IsTrue(result.Results.TrueForAll(r => r.Id == Token));
        }

        [TestMethod]
        public void ReportFindStoredPaymentMethodsPaged_By_RandomId() {
            var storedPaymentMethodId = $"PMT_{Guid.NewGuid()}";

            var result = ReportingService.FindStoredPaymentMethodsPaged(FirstPage, PageSize)
                .Where(DataServiceCriteria.StoredPaymentMethodId, storedPaymentMethodId)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.AreEqual(0, result?.Results.Count);
        }

        [TestMethod]
        public void FindStoredPaymentMethod_By_CardInfo()
        {
            Card = new CreditCardData();
            Card.Number = "4242424242424242";
            Card.ExpMonth = 12;
            Card.ExpYear = DateTime.Now.AddYears(1).Year;

            var response = ReportingService.FindStoredPaymentMethodsPaged(FirstPage, PageSize)
            .Where(SearchCriteria.PaymentMethod, Card as IPaymentMethod)
            .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual(true, (response.Results is IList<StoredPaymentMethodSummary>));
            
            foreach (var rs in response.Results)
            {
                Assert.AreEqual(Card.ExpMonth.ToString(), rs.CardExpMonth);
                Assert.AreEqual(true, Card.ExpYear.ToString().EndsWith(rs.CardExpYear));
                Assert.AreEqual(Card.Number.Substring(Card.Number.Length - 4), rs.CardLast4.Substring(rs.CardLast4.Length - 4));
            }           
        }

        [TestMethod]
        public void FindStoredPaymentMethod_By_OnlyCardNumberInfo()
        {
            Card = new CreditCardData();
            Card.Number = "4263970000005262";
            Card.ExpMonth = 12;
            Card.ExpYear = DateTime.Now.AddYears(1).Year;

            var response = ReportingService.FindStoredPaymentMethodsPaged(FirstPage, PageSize)
            .Where(SearchCriteria.PaymentMethod, Card as IPaymentMethod)
            .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual(true, (response.Results is IList<StoredPaymentMethodSummary>));
          
            foreach (var rs in response.Results)
            {
                Assert.AreEqual(Card.ExpMonth.ToString(), rs.CardExpMonth);
                Assert.AreEqual(true, Card.ExpYear.ToString().EndsWith(rs.CardExpYear));
                Assert.AreEqual(Card.Number.Substring(Card.Number.Length - 4), rs.CardLast4.Substring(rs.CardLast4.Length - 4));
            }
        }

        [TestMethod]
        public void FindStoredPaymentMethod_By_WithoutMandatoryCardNumber()
        {
            Card = new CreditCardData();
            var exceptionCaught = false;

            try
            {
                ReportingService.FindStoredPaymentMethodsPaged(FirstPage, PageSize)
                .Where(SearchCriteria.PaymentMethod, Card as IPaymentMethod)
                .Execute();
            }
            catch (GatewayException ex)
            {
                exceptionCaught = true;
                Assert.AreEqual("40005", ex.ResponseMessage);
                Assert.AreEqual("Status Code: BadRequest - Request expects the following fields : number", ex.Message);               
            }
            finally
            {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        [Ignore]
        // TODO: Reported the the GP API team
        // Endpoint is retrieving not filtered results
        public void ReportFindStoredPaymentMethodsPaged_By_NumberLast4() {
            var numberLast4 = Card.Number.Substring(Card.Number.Length - 4);
            var result = ReportingService.FindStoredPaymentMethodsPaged(FirstPage, PageSize)
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
            var result = ReportingService.FindStoredPaymentMethodsPaged(FirstPage, PageSize)
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

            var result = ReportingService.FindStoredPaymentMethodsPaged(FirstPage, PageSize)
                .Where(SearchCriteria.ReferenceNumber, response.Reference)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<StoredPaymentMethodSummary>);
            Assert.IsTrue(result.Results.TrueForAll(r => r.Reference == response.Reference));
        }

        [TestMethod]
        public void ReportFindStoredPaymentMethodsPaged_By_Status() {
            const StoredPaymentMethodStatus status = StoredPaymentMethodStatus.Active;
            var result = ReportingService.FindStoredPaymentMethodsPaged(FirstPage, PageSize)
                .Where(SearchCriteria.StoredPaymentMethodStatus, status)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<StoredPaymentMethodSummary>);
            Assert.IsTrue(result.Results.TrueForAll(r => r.Status == EnumConverter.GetMapping(Target.GP_API, status)));
        }

        [TestMethod]
        public void ReportFindStoredPaymentMethodsPaged_By_StartDate_And_EndDate() {
            var result = ReportingService.FindStoredPaymentMethodsPaged(FirstPage, PageSize)
                .Where(SearchCriteria.StartDate, ReportingStartDate)
                .And(SearchCriteria.EndDate, ReportingEndDate)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<StoredPaymentMethodSummary>);
            Assert.IsTrue(result.Results.TrueForAll(r =>
                r.TimeCreated.Date >= ReportingStartDate.Date && r.TimeCreated.Date <= ReportingEndDate.Date));
        }

        [TestMethod]
        public void ReportFindStoredPaymentMethodsPaged_By_StartDate_And_EndDate_CurrentDay() {
            var result = ReportingService.FindStoredPaymentMethodsPaged(FirstPage, PageSize)
                .Where(SearchCriteria.StartDate, ReportingEndDate)
                .And(SearchCriteria.EndDate, ReportingEndDate)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<StoredPaymentMethodSummary>);
            Assert.IsTrue(result.Results.TrueForAll(t =>
                t.TimeCreated.Date >= ReportingEndDate.Date && t.TimeCreated.Date <= ReportingEndDate.Date));
        }

        [TestMethod]
        public void ReportFindStoredPaymentMethodsPaged_By_StartLastUpdatedDate_And_EndLastUpdatedDate() {
            var result = ReportingService.FindStoredPaymentMethodsPaged(FirstPage, PageSize)
                .Where(DataServiceCriteria.StartLastUpdatedDate, ReportingStartDate)
                .And(DataServiceCriteria.EndLastUpdatedDate, ReportingLastMonthDate)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<StoredPaymentMethodSummary>);
            //ToDo: There is no way to validate the response data
        }

        [TestMethod]
        public void ReportFindStoredPaymentMethodsPaged_OrderBy_TimeCreated_Ascending() {
            var result = ReportingService.FindStoredPaymentMethodsPaged(FirstPage, PageSize)
                .OrderBy(StoredPaymentMethodSortProperty.TimeCreated, SortDirection.Ascending)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<StoredPaymentMethodSummary>);
            Assert.IsTrue(result.Results.SequenceEqual(result.Results.OrderBy(r => r.TimeCreated)));
        }

        [TestMethod]
        public void ReportFindStoredPaymentMethodsPaged_OrderBy_TimeCreated_Descending() {
            var result = ReportingService.FindStoredPaymentMethodsPaged(FirstPage, PageSize)
                .OrderBy(StoredPaymentMethodSortProperty.TimeCreated, SortDirection.Descending)
                .Execute();
            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results is List<StoredPaymentMethodSummary>);
            Assert.IsTrue(result.Results.SequenceEqual(result.Results.OrderByDescending(r => r.TimeCreated)));
        }

        [TestMethod]
        public void ReportFindStoredPaymentMethodsPaged_OrderBy_TimeCreated() {
            var resultAsc = ReportingService.FindStoredPaymentMethodsPaged(FirstPage, PageSize)
                .OrderBy(StoredPaymentMethodSortProperty.TimeCreated, SortDirection.Ascending)
                .Execute();
            Assert.IsNotNull(resultAsc?.Results);
            Assert.IsTrue(resultAsc.Results is List<StoredPaymentMethodSummary>);
            Assert.IsTrue(resultAsc.Results.SequenceEqual(resultAsc.Results.OrderBy(r => r.TimeCreated)));

            var resultDesc = ReportingService.FindStoredPaymentMethodsPaged(FirstPage, PageSize)
                .OrderBy(StoredPaymentMethodSortProperty.TimeCreated, SortDirection.Descending)
                .Execute();
            Assert.IsNotNull(resultDesc?.Results);
            Assert.IsTrue(resultDesc.Results is List<StoredPaymentMethodSummary>);
            Assert.IsTrue(resultDesc.Results.SequenceEqual(resultDesc.Results.OrderByDescending(r => r.TimeCreated)));

            Assert.IsFalse(resultAsc.Results.SequenceEqual(resultDesc.Results));
        }
    }
}