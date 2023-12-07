using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Enums;
using GlobalPayments.Api.Entities.Reporting;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace GlobalPayments.Api.Tests.GpApi {
    [TestClass]
    public class GpApiAchTest : BaseGpApiTests {
        private eCheck eCheck;
        private Address address;
        private Customer customer;

        private const string CURRENCY = "USD";
        private const decimal AMOUNT = 10m;

        [TestInitialize]
        public void TestInitialize() {
            ServicesContainer.RemoveConfig();
            var gpApiConfig = GpApiConfigSetup(AppId, AppKey, Channel.CardNotPresent);
            ServicesContainer.ConfigureService(gpApiConfig);

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

            AssertResponse(response, TransactionStatus.Captured);
        }

        [TestMethod]
        public void CreditSaleThenSplit()
        {
            var config = GpApiConfigSetup(AppIdForMerchant, AppKeyForMerchant, Channel.CardNotPresent);
            ServicesContainer.ConfigureService(config);

            var merchants = GetMerchants();

            var merchantProcessing = merchants.FirstOrDefault();
            var merchantId = merchantProcessing.Id;
            var accountProcessing = GetAccountByType(merchantId, MerchantAccountType.TRANSACTION_PROCESSING);

            config.MerchantId = merchantId;
            config.AccessTokenInfo = new AccessTokenInfo { TransactionProcessingAccountID = accountProcessing.Id };

            var merchantConfigName = "config_" + merchantId;
            ServicesContainer.ConfigureService(config, merchantConfigName);

            var transaction = eCheck.Charge(AMOUNT)
               .WithCurrency(CURRENCY)
               .WithAddress(address)
               .WithCustomer(customer)
               .Execute(merchantConfigName);

            var merchantSplitting = merchants.FirstOrDefault(x => x.Id != merchantId);

            var accountRecipient = GetAccountByType(merchantId, MerchantAccountType.FUND_MANAGEMENT);
            var accountSplitting = GetAccountByType(merchantSplitting.Id, MerchantAccountType.FUND_MANAGEMENT);

            Assert.IsNotNull(accountRecipient);

            var fundsData = new FundsData();
            fundsData.RecipientAccountId = accountSplitting.Id;
            fundsData.MerchantId = merchantId;

            var splitResponse = transaction.Split(8m)
                .WithFundsData(fundsData)
                .WithReference("Split Identifier")
                .WithDescription("Split Test")
                .Execute();

            AssertResponse(splitResponse, TransactionStatus.Captured);

            ServicesContainer.RemoveConfig(merchantConfigName);
            Thread.Sleep(30000);
        }

        [TestMethod]
        public void CreditSaleThenSplitThenReverse_WithConfigMerchantId()
        {
            var gpApiConfig = GpApiConfigSetup(AppIdForMerchant, AppKeyForMerchant, Channel.CardNotPresent);
            ServicesContainer.ConfigureService(gpApiConfig);

            var merchants = GetMerchants();

            var merchantProcessing = merchants.FirstOrDefault();
            var merchantId = merchantProcessing.Id;
            var accountProcessing = GetAccountByType(merchantId, MerchantAccountType.TRANSACTION_PROCESSING);

            gpApiConfig.MerchantId = merchantId;
            gpApiConfig.AccessTokenInfo = new AccessTokenInfo { TransactionProcessingAccountID = accountProcessing.Id };           

            var merchantConfigName = "config_" + merchantId;
            ServicesContainer.ConfigureService(gpApiConfig, merchantConfigName);

            var transaction = eCheck.Charge(AMOUNT)
               .WithCurrency(CURRENCY)
               .WithAddress(address)
               .WithCustomer(customer)
               .Execute(merchantConfigName);
            
            var merchantSplitting = merchants.FirstOrDefault(x => x.Id != merchantId);

            var accountRecipient = GetAccountByType(merchantId, MerchantAccountType.FUND_MANAGEMENT);
            var accountSplitting = GetAccountByType(merchantSplitting.Id, MerchantAccountType.FUND_MANAGEMENT);

            Assert.IsNotNull(accountRecipient);

            var fundsData = new FundsData();
            fundsData.RecipientAccountId = accountSplitting.Id;
            fundsData.MerchantId = merchantId;

            var transferAmount = 8m;
            var transferReference = "Split Identifier";
            var transferDescription = "Split Test";

            var splitResponse = transaction.Split(transferAmount)
                .WithFundsData(fundsData)
                .WithReference(transferReference)
                .WithDescription(transferDescription)
                .Execute();

            AssertResponse(splitResponse, TransactionStatus.Captured);

            Assert.IsNotNull(splitResponse.TransfersFundsAccounts);

            var transferFund = splitResponse.TransfersFundsAccounts.FirstOrDefault();

            Assert.AreEqual("00", transferFund.Status);
            Assert.AreEqual(transferAmount, transferFund.Amount);
            Assert.AreEqual(transferReference, transferFund.Reference);
            Assert.AreEqual(transferDescription, transferFund.Description);

            var trfTransaction = Transaction.FromId(transferFund.Id, PaymentMethodType.Account_Funds);

            var reverse = trfTransaction.Reverse()
                .Execute(merchantConfigName);

            AssertResponse(reverse, TransactionStatus.Funded);

            ServicesContainer.RemoveConfig(merchantConfigName);
            Thread.Sleep(30000);
        }

        [TestMethod]
        public void CreditSaleThenSplitThenReverse_WithFundsData()
        {
            var config = GpApiConfigSetup(AppIdForMerchant, AppKeyForMerchant, Channel.CardNotPresent);
            ServicesContainer.ConfigureService(config);

            var merchants = GetMerchants();

            var merchantProcessing = merchants.FirstOrDefault();
            var merchantId = merchantProcessing.Id;
            var accountProcessing = GetAccountByType(merchantId, MerchantAccountType.TRANSACTION_PROCESSING);

            config.MerchantId = merchantId;
            config.AccessTokenInfo = new AccessTokenInfo { TransactionProcessingAccountID = accountProcessing.Id };

            var merchantConfigName = "config_" + merchantId;
            ServicesContainer.ConfigureService(config, merchantConfigName);

            var transaction = eCheck.Charge(AMOUNT)
               .WithCurrency(CURRENCY)
               .WithAddress(address)
               .WithCustomer(customer)
               .Execute(merchantConfigName);

            var merchantSplitting = merchants.FirstOrDefault(x => x.Id != merchantId);

            var accountRecipient = GetAccountByType(merchantId, MerchantAccountType.FUND_MANAGEMENT);
            var accountSplitting = GetAccountByType(merchantSplitting.Id, MerchantAccountType.FUND_MANAGEMENT);

            Assert.IsNotNull(accountRecipient);

            var fundsData = new FundsData();
            fundsData.RecipientAccountId = accountSplitting.Id;
            fundsData.MerchantId = merchantId;

            var transferAmount = 8m;
            var transferReference = "Split Identifier";
            var transferDescription = "Split Test";

            var splitResponse = transaction.Split(transferAmount)
                .WithFundsData(fundsData)
                .WithReference(transferReference)
                .WithDescription(transferDescription)
                .Execute();

            AssertResponse(splitResponse, TransactionStatus.Captured);

            Assert.IsNotNull(splitResponse.TransfersFundsAccounts);

            var transferFund = splitResponse.TransfersFundsAccounts.FirstOrDefault();

            Assert.AreEqual("00", transferFund.Status);
            Assert.AreEqual(transferAmount, transferFund.Amount);
            Assert.AreEqual(transferReference, transferFund.Reference);
            Assert.AreEqual(transferDescription, transferFund.Description);

            var trfTransaction = Transaction.FromId(transferFund.Id, PaymentMethodType.Account_Funds);

            var reverse = trfTransaction.Reverse()
                .WithFundsData(fundsData)
                .Execute();

            AssertResponse(reverse, TransactionStatus.Funded);

            ServicesContainer.RemoveConfig(merchantConfigName);
            Thread.Sleep(30000);
        }

        [TestMethod]
        public void CreditSaleThenSplit_WithoutFundsData()
        {
            var config = GpApiConfigSetup(AppIdForMerchant, AppKeyForMerchant, Channel.CardNotPresent);
            ServicesContainer.ConfigureService(config);

            var transaction = new Transaction {TransactionId = Guid.NewGuid().ToString() };

            var exceptionCaught = false;
            try
            {                
                var splitResponse = transaction.Split(8m)
                   //.WithFundsData(fundsData)
                   .WithReference("Split Identifier")
                   .WithDescription("Split Test")
                   .Execute();
            }
            catch (BuilderException ex)
            {
                exceptionCaught = true;
                Assert.AreEqual("FundsData cannot be null for this transaction type.", ex.Message);                
            }
            finally
            {
                ServicesContainer.RemoveConfig();
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void CreditSaleThenSplit_WithoutAmount()
        {
            var config = GpApiConfigSetup(AppIdForMerchant, AppKeyForMerchant, Channel.CardNotPresent);
            ServicesContainer.ConfigureService(config);

            var merchants = GetMerchants();

            var merchantProcessing = merchants.FirstOrDefault();
            var merchantId = merchantProcessing.Id;
            var accountProcessing = GetAccountByType(merchantId, MerchantAccountType.TRANSACTION_PROCESSING);

            config.MerchantId = merchantId;
            config.AccessTokenInfo = new AccessTokenInfo { TransactionProcessingAccountID = accountProcessing.Id };

            var merchantConfigName = "config_" + merchantId;
            ServicesContainer.ConfigureService(config, merchantConfigName);

            var transaction = eCheck.Charge(AMOUNT)
               .WithCurrency(CURRENCY)
               .WithAddress(address)
               .WithCustomer(customer)
               .Execute(merchantConfigName);

            var merchantSplitting = merchants.FirstOrDefault(x => x.Id != merchantId);

            var accountRecipient = GetAccountByType(merchantId, MerchantAccountType.FUND_MANAGEMENT);
            var accountSplitting = GetAccountByType(merchantSplitting.Id, MerchantAccountType.FUND_MANAGEMENT);

            Assert.IsNotNull(accountRecipient);

            var fundsData = new FundsData();
            fundsData.RecipientAccountId = accountSplitting.Id;
            fundsData.MerchantId = merchantId;            

            var exceptionCaught = false;
            try {
                var splitResponse = transaction.Split()
                .WithFundsData(fundsData)
                .WithReference("Split Identifier")
                .WithDescription("Split Test")
                .Execute();
            }
            catch (BuilderException ex) {
                exceptionCaught = true;
                Assert.AreEqual("Amount cannot be null for this transaction type.", ex.Message);                
            }
            finally {
                Assert.IsTrue(exceptionCaught);
                ServicesContainer.RemoveConfig(merchantConfigName);
                Thread.Sleep(30000);
            }  
        }

        [TestMethod]
        public void CreditSaleThenSplit_WithoutRecipientId()
        {
            var config = GpApiConfigSetup(AppIdForMerchant, AppKeyForMerchant, Channel.CardNotPresent);
            ServicesContainer.ConfigureService(config);

            var merchants = GetMerchants();

            var merchantProcessing = merchants.FirstOrDefault();
            var merchantId = merchantProcessing.Id;
            var accountProcessing = GetAccountByType(merchantId, MerchantAccountType.TRANSACTION_PROCESSING);

            config.MerchantId = merchantId;
            config.AccessTokenInfo = new AccessTokenInfo { TransactionProcessingAccountID = accountProcessing.Id };

            var merchantConfigName = "config_" + merchantId;
            ServicesContainer.ConfigureService(config, merchantConfigName);

            var transaction = eCheck.Charge(AMOUNT)
               .WithCurrency(CURRENCY)
               .WithAddress(address)
               .WithCustomer(customer)
               .Execute(merchantConfigName);

            var merchantSplitting = merchants.FirstOrDefault(x => x.Id != merchantId);

            var accountRecipient = GetAccountByType(merchantId, MerchantAccountType.FUND_MANAGEMENT);
            var accountSplitting = GetAccountByType(merchantSplitting.Id, MerchantAccountType.FUND_MANAGEMENT);

            Assert.IsNotNull(accountRecipient);

            var fundsData = new FundsData();
            //fundsData.RecipientAccountId = accountSplitting.Id;
            fundsData.MerchantId = merchantId;

            var exceptionCaught = false;
            try
            {
                var splitResponse = transaction.Split(0.01m)
                .WithFundsData(fundsData)
                .WithReference("Split Identifier")
                .WithDescription("Split Test")
                .Execute();
            }
            catch (GatewayException ex)
            {
                exceptionCaught = true;
                Assert.AreEqual("Status Code: BadRequest - Transfers may only be initiated between accounts under the same partner program", ex.Message);
                Assert.AreEqual("INVALID_REQUEST_DATA", ex.ResponseCode);
                Assert.AreEqual("40041", ex.ResponseMessage);
            }
            finally
            {
                ServicesContainer.RemoveConfig(merchantConfigName);
                Assert.IsTrue(exceptionCaught);
                Thread.Sleep(30000);
            }
        }

        [TestMethod]
        public void CheckRefund()
        {
            var response = eCheck.Refund(AMOUNT)
                .WithCurrency(CURRENCY)
                .WithAddress(address)
                .Execute();

            AssertResponse(response, TransactionStatus.Captured);
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

            AssertResponse(resp, TransactionStatus.Captured);
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
                .And(SearchCriteria.PaymentMethodName, PaymentMethodName.BankTransfer)
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

            AssertResponse(resp, TransactionStatus.Captured);
        }

        private void AssertResponse(Transaction response, TransactionStatus transactionStatus) {
            Assert.IsNotNull(response);
            Assert.AreEqual(Success, response?.ResponseCode);
            Assert.AreEqual(GetMapping(transactionStatus), response?.ResponseMessage);
        }

        private List<MerchantSummary> GetMerchants()
        {
            var merchants = new ReportingService().FindMerchants(1, 10)
              .OrderBy(MerchantAccountsSortProperty.TIME_CREATED, SortDirection.Descending)
              .Where(SearchCriteria.MerchantStatus, MerchantAccountStatus.ACTIVE)
              .Execute();

            return merchants.Results;
        }

        private MerchantAccountSummary GetAccountByType(string merchantSenderId, MerchantAccountType merchantAccountType)
        {
            var response = ReportingService.FindAccounts(1, 10)
                   .OrderBy(MerchantAccountsSortProperty.TIME_CREATED, SortDirection.Descending)
                   .Where(SearchCriteria.StartDate, StartDate)
                   .And(SearchCriteria.EndDate, EndDate)
                    .And(DataServiceCriteria.MerchantId, merchantSenderId)
                   .And(SearchCriteria.AccountStatus, MerchantAccountStatus.ACTIVE)
                   .Execute();

            return response.Results.FirstOrDefault(x => x.Type == merchantAccountType);
        }
    }
}
