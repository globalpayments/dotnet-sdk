using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Utils.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace GlobalPayments.Api.Tests.GpApi {
    [TestClass]
    public class GpApiAchTest : BaseGpApiTests {
        private eCheck eCheck;
        private Address address;
        private Customer customer;

        private const string CURRENCY = "USD";
        private const decimal AMOUNT = 10m;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context) {
            ServicesContainer.ConfigureService(new GpApiConfig {
                AppId = "Uyq6PzRbkorv2D4RQGlldEtunEeGNZll",
                AppKey = "QDsW1ETQKHX6Y4TA",
                Channel = Channel.CardNotPresent,
                RequestLogger  = new RequestConsoleLogger()
            });
        }

        [TestInitialize]
        public void TestInitialize() {
            address = new Address {
                StreetAddress1 = "Apartment 852",
                StreetAddress2 = "Complex 741",
                StreetAddress3 = "no",
                City = "Chicago",
                PostalCode = "5001",
                State = "IL",
                CountryCode = "US"
            };

            var bankAddress = new Address { 
                StreetAddress1 = "12000 Smoketown Rd",
                StreetAddress2 = "Apt 3B",
                StreetAddress3 = "no",
                City = "Mesa",
                PostalCode = "22192",
                State = "AZ",
                CountryCode = "US"
            };

            eCheck = new eCheck {
                AccountNumber = "1234567890",
                RoutingNumber = "122000030",
                AccountType = AccountType.SAVINGS,
                SecCode = SecCode.WEB,
                CheckReference = "123",
                MerchantNotes = "123",
                BankName = "First Union",
                CheckHolderName = "Jane Doe",
                BankAddress = bankAddress
            };

            customer = new Customer {
                Id = "e193c21a-ce64-4820-b5b6-8f46715de931",
                FirstName = "James",
                LastName = "Mason",
                DateOfBirth = "1980-01-01",
                MobilePhone = "+35312345678",
                HomePhone = "+11234589"
            };
        }

        [TestMethod]
        public void CheckSale() {
            var response = eCheck.Charge(AMOUNT)
                .WithCurrency(CURRENCY)
                .WithAddress(address)
                .WithCustomer(customer)
                .Execute();

            assertResponse(response, TransactionStatus.Captured);
        }

        [TestMethod]
        public void CheckRefund()
        {
            var response = eCheck.Refund(AMOUNT)
                .WithCurrency(CURRENCY)
                .WithAddress(address)
                .Execute();

            assertResponse(response, TransactionStatus.Captured);
        }

        [TestMethod, Ignore]
        //GP-API sandbox limitation
        public void CheckRefundExistingSale()
        {
            var amount = 1.29m;

            var response = ReportingService.FindTransactionsPaged(1, 10)
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .Where(SearchCriteria.StartDate, DateTime.Now.AddYears(-1))
                .And(SearchCriteria.EndDate, DateTime.Now.AddDays(-2))
                .And(SearchCriteria.PaymentType, PaymentType.Sale)
                .And(SearchCriteria.PaymentMethod, PaymentMethodName.BankTransfer)
                .And(DataServiceCriteria.Amount, amount)
                .Execute();

            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Results);
            var transactionSummary = response.Results.FirstOrDefault();
            Assert.IsNotNull(transactionSummary);
            Assert.AreEqual(amount, transactionSummary.Amount);
            var transaction = new Transaction {
                TransactionId = transactionSummary.TransactionId,
                PaymentMethodType = PaymentMethodType.ACH
            };

            var resp = transaction.Refund()
                .WithCurrency("USD")
                .Execute();

            assertResponse(resp, TransactionStatus.Captured);
        }

        [TestMethod]
        public void CheckReauthorize() {
            var eCheckReauth = new eCheck { 
                SecCode = SecCode.PPD,
                AccountNumber = "051904524",
                RoutingNumber = "123456780",
            };

            var amount = 1.29m;

            var response = ReportingService.FindTransactionsPaged(1, 10)
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .Where(SearchCriteria.StartDate, DateTime.Now.AddYears(-1))
                .And(SearchCriteria.EndDate, DateTime.Now.AddDays(-2))
                .And(SearchCriteria.PaymentType, PaymentType.Sale)
                .And(SearchCriteria.PaymentMethod, PaymentMethodName.BankTransfer)
                .And(DataServiceCriteria.Amount, amount)
                .Execute();

            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Results);
            var transactionSummary = response.Results.FirstOrDefault();
            Assert.IsNotNull(transactionSummary);
            Assert.AreEqual(amount, transactionSummary.Amount);
            var transaction = new Transaction {
                TransactionId = transactionSummary.TransactionId,
                PaymentMethodType = PaymentMethodType.ACH
            };

            var resp = transaction.Reauthorize()
                .WithDescription("Resubmitting " + transaction.ReferenceNumber)
                .WithBankTransferDetails(eCheckReauth)
                .Execute();

            assertResponse(resp, TransactionStatus.Captured);
        }

        private void assertResponse(Transaction response, TransactionStatus transactionStatus) {
            Assert.IsNotNull(response);
            Assert.AreEqual(SUCCESS, response?.ResponseCode);
            Assert.AreEqual(GetMapping(transactionStatus), response?.ResponseMessage);
        }
    }
}
