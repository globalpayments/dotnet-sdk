using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GlobalPayments.Api.Tests.TestData;
using GlobalPayments.Api.Services;
using System.Linq;
using System.Collections.Generic;
using System;
using System.Threading;

namespace GlobalPayments.Api.Tests.GpApi {
    [TestClass]
    public class GpApiInstallmentTests : BaseGpApiTests {
        private CreditCardData visaCard;
        private CreditCardData masterCard;
        private CreditCardData carnetCard;
        private StoredCredential storedCredential;
        private InstallmentData installmentData;
        private Address address;
        private const decimal AMOUNT = 1.2m;
        private const string CURRENCY = "MXN";
        private const int FirstPage = 1;
        private const int PageSize = 10;
        private DateTime ReportingStartDate = DateTime.UtcNow.AddYears(-1);
        private DateTime ReportingEndDate = DateTime.UtcNow;

        [TestInitialize]
        public void TestInitialize()
        {
            ServicesContainer.RemoveConfig();
            var gpApiConfig = GpApiConfigSetup(AppId, AppKey, Channel.CardNotPresent);

            gpApiConfig.Country = "MX";
            gpApiConfig.AppId = "Vw9O4jOMqozC39Grx8q3oGAvqEjLcgGn";
            gpApiConfig.AppKey = "qgvDUwIhgT8QS2kp";
            gpApiConfig.ServiceUrl = "https://apis-sit.globalpay.com/ucp";
            gpApiConfig.AccessTokenInfo = new AccessTokenInfo
            {
                TransactionProcessingAccountName = "Portico_SIT_405352",
                RiskAssessmentAccountName = "EOS_RiskAssessment",
                TransactionProcessingAccountID = "TRA_ba4aa4dd3cd1426e9eecba3abbd2053c",
                MerchantManagementAccountID = "MER_e3b91f1af988437f85d000eb272b777d"
            };
            ServicesContainer.ConfigureService(gpApiConfig);

            var gpApiSandboxConfig = GpApiConfigSetup(AppId, AppKey, Channel.CardNotPresent);
            ServicesContainer.ConfigureService(gpApiSandboxConfig, "sandboxConfig");

            address = new Address {
                StreetAddress1 = "123 Main St.",
                City = "Downtown",
                State = "NJ",
                Country = "MX",
                PostalCode = "12345"
            };

            storedCredential = new StoredCredential {
                Initiator = StoredCredentialInitiator.CardHolder,
                Type = StoredCredentialType.Installment,
                Sequence = StoredCredentialSequence.Subsequent,
                Reason = StoredCredentialReason.Incremental,
                ContractReference = "dsffdsfds"
            };

            installmentData = new InstallmentData {
                GracePeriodCount = "03",
                Program = "SIP",
                Count = "03",
                Mode = "NO_PROMOTION"
            };

            visaCard = new CreditCardData {
                Number = "4395840190010011",
                ExpMonth = 12,
                ExpYear = 2027,
                Cvn = "840",
                CardPresent = false,
                ReaderPresent = false
            };
            masterCard = new CreditCardData {
                Number = "5120350100064537",
                ExpMonth = 12,
                ExpYear = 2027,
                Cvn = "123",
                CardPresent = false,
                ReaderPresent = false
            };

            carnetCard = new CreditCardData {
                Number = "6363181868200169",
                ExpMonth = 01,
                ExpYear = 2030,
                Cvn = "123",
                CardPresent = false,
                ReaderPresent = false
            };
        }

        #region Sale Test Case for Installment
        [TestMethod]
        public void CreditSaleForInstallmentVisa() {

            var response = visaCard.Charge(AMOUNT)
               .WithCurrency(CURRENCY)
               .WithAddress(address)
               .WithStoredCredential(storedCredential)
               .WithInstallmentData(installmentData)
               .Execute("sandboxConfig");

            AssertTransactionResponse(response, TransactionStatus.Captured);
            Assert.AreEqual(AMOUNT, response.BalanceAmount);
        }

        [TestMethod]
        public void CreditSaleWithoutInstallmentData() {

            var response = visaCard.Charge(AMOUNT)
               .WithCurrency(CURRENCY)
               .WithAddress(address)
               .Execute("sandboxConfig");

            AssertTransactionResponse(response, TransactionStatus.Captured);
            Assert.AreEqual(AMOUNT, response.BalanceAmount);
            Assert.IsNull(response.PayerDetails);
            Assert.IsNull(response.InstallmentData);
        }

        [TestMethod]
        public void CreditSaleForInstallmentMC() {

            var response = masterCard.Charge(AMOUNT)
               .WithCurrency(CURRENCY)
               .WithAddress(address)
               .WithStoredCredential(storedCredential)
               .WithInstallmentData(installmentData)
               .Execute("sandboxConfig");

            AssertTransactionResponse(response, TransactionStatus.Captured);
            Assert.AreEqual(AMOUNT, response.BalanceAmount);
            Assert.IsNull(response.PayerDetails);
        }

        #endregion

        #region Reporting Test Case

        [TestMethod]
        public void ReportTransactionDetailForInstallmentById() {

            var transaction = visaCard.Charge(AMOUNT)
               .WithCurrency(CURRENCY)
               .WithAddress(address)
               .WithStoredCredential(storedCredential)
               .WithInstallmentData(installmentData)
               .Execute();

            Thread.Sleep(2000);

            var response = ReportingService.TransactionDetail(transaction.TransactionId)
                .Execute();
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.InstallmentData);
            Assert.AreEqual(installmentData.Count, response.InstallmentData.Count);
            Assert.AreEqual(installmentData.Mode, response.InstallmentData.Mode);
            Assert.AreEqual(installmentData.Program, response.InstallmentData.Program);
            Assert.AreEqual(installmentData.GracePeriodCount, response.InstallmentData.GracePeriodCount);

            Assert.IsTrue(response is TransactionSummary);
            Assert.AreEqual(transaction.TransactionId, response.TransactionId);
        }

        [TestMethod]
        public void ReportTransactionDetailWithoutInstallmentDataById() {

            var transaction = visaCard.Charge(AMOUNT)
               .WithCurrency(CURRENCY)
               .WithAddress(address)
               .Execute();

            Thread.Sleep(2000);

            var response = ReportingService.TransactionDetail(transaction.TransactionId)
                .Execute();
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.InstallmentData);
            Assert.IsTrue(string.IsNullOrEmpty(response.InstallmentData.Count));
            Assert.IsTrue(string.IsNullOrEmpty(response.InstallmentData.Mode));
            Assert.IsTrue(string.IsNullOrEmpty(response.InstallmentData.Program));
            Assert.IsTrue(string.IsNullOrEmpty(response.InstallmentData.GracePeriodCount));

            Assert.IsTrue(response is TransactionSummary);
            Assert.AreEqual(transaction.TransactionId, response.TransactionId);
        }

        [TestMethod]
        public void ReportTransactionDetailForStatusInThreeDSecure() {
            var result = ReportingService.FindTransactionsPaged(2, PageSize)
                .Where(SearchCriteria.TransactionStatus, TransactionStatus.Authenticated)
                .Execute().Results.FirstOrDefault();

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.ThreeDSecure);
            Assert.AreEqual("AUTHENTICATION_SUCCESSFUL", result.ThreeDSecure.Status);
        }

        [TestMethod]
        public void ReportFindTransactionDetailForInstallment() {

            var result = ReportingService.FindTransactionsPaged(FirstPage, PageSize)
                .OrderBy(TransactionSortProperty.TimeCreated, SortDirection.Descending)
                .Where(SearchCriteria.StartDate, ReportingStartDate)
                .And(SearchCriteria.EndDate, ReportingEndDate)
                .Execute();

            Assert.IsNotNull(result?.Results);
            Assert.IsTrue(result.Results.TrueForAll(x => x.InstallmentData != null));
            Assert.IsTrue(result.Results is List<TransactionSummary>);
            Assert.IsTrue(result.Results.TrueForAll(t => t.TransactionDate?.Date >= ReportingStartDate.Date && t.TransactionDate?.Date <= ReportingEndDate.Date));
        }
        #endregion

        #region Private Methods                  
        private void AssertTransactionResponse(Transaction transaction, TransactionStatus transactionStatus) {
            Assert.IsNotNull(transaction);
            Assert.AreEqual(Success, transaction.ResponseCode);
            Assert.AreEqual(GetMapping(transactionStatus), transaction.ResponseMessage);
        }
        #endregion
    }
}
