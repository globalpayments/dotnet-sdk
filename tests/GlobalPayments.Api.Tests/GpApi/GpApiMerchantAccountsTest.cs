using System;
using System.Collections.Generic;
using System.Linq;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Enums;
using GlobalPayments.Api.Entities.Reporting;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Utils.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.GpApi
{
    [TestClass]
    public class GpApiMerchantAccountsTest : BaseGpApiTests {
        private List<MerchantAccountSummary> Accounts;
        private PayFacService payFacService;
        private static GpApiConfig config;
        
        private readonly DateTime startDate = DateTime.UtcNow.AddYears(-1);
        private readonly DateTime endDate = DateTime.UtcNow.AddDays(-3);

        [ClassInitialize]
        public static void ClassInitialize(TestContext context) {
            ServicesContainer.RemoveConfig();
            config = new GpApiConfig {
                AppId = AppIdForMerchant,
                AppKey = AppKeyForMerchant,
                Channel = Channel.CardNotPresent,
                ChallengeNotificationUrl = "https://ensi808o85za.x.pipedream.net/",
                MethodNotificationUrl = "https://ensi808o85za.x.pipedream.net/",
                MerchantContactUrl = "https://enp4qhvjseljg.x.pipedream.net/",
                // RequestLogger = new RequestFileLogger(@"C:\temp\transit\finger.txt"),
                RequestLogger = new RequestConsoleLogger(),
                EnableLogging = true,
            };
            ServicesContainer.ConfigureService(config);
        }

        [TestInitialize]
        public void TestInitialize() {
            payFacService = new PayFacService();

            var response = ReportingService.FindAccounts(1, 10)
                .OrderBy(MerchantAccountsSortProperty.TIME_CREATED, SortDirection.Ascending)
                .Where(SearchCriteria.StartDate, startDate)
                .And(SearchCriteria.EndDate, endDate)
                .And(SearchCriteria.AccountStatus, MerchantAccountStatus.ACTIVE)
                .Execute();

            Accounts = response.Results.Count > 0 ? response.Results : null;
        }
        
        [TestMethod]
        public void FindAccountsInfo() {
            var response = ReportingService.FindAccounts(1, 10)
                .OrderBy(MerchantAccountsSortProperty.TIME_CREATED, SortDirection.Ascending)
                .Where(SearchCriteria.StartDate, startDate)
                .And(SearchCriteria.EndDate, endDate)
                .Execute();

            Assert.IsNotNull(response);

            foreach (var rs in response.Results) {
                Assert.AreEqual(MerchantAccountStatus.ACTIVE.ToString(), rs.Status.ToString());
            }
        }

        [TestMethod]
        public void FindAccountsInfo_SearchByStatusActive() {
            var response = ReportingService.FindAccounts(1, 10)
                .OrderBy(MerchantAccountsSortProperty.TIME_CREATED, SortDirection.Ascending)
                .Where(SearchCriteria.StartDate, startDate)
                .And(SearchCriteria.EndDate, endDate)
                .And(SearchCriteria.AccountStatus, MerchantAccountStatus.ACTIVE)
                .Execute();

            Assert.IsNotNull(response);

            foreach (var rs in response.Results) {
                Assert.AreEqual(MerchantAccountStatus.ACTIVE.ToString(), rs.Status.ToString());
            }
        }

        [TestMethod]
        public void FindAccountsInfo_SearchByStatusInactive() {
            var response = ReportingService.FindAccounts(1, 10)
                .OrderBy(MerchantAccountsSortProperty.TIME_CREATED, SortDirection.Ascending)
                .Where(SearchCriteria.StartDate, startDate)
                .And(SearchCriteria.EndDate, endDate)
                .And(SearchCriteria.AccountStatus, MerchantAccountStatus.INACTIVE)
                .Execute();

            Assert.IsNotNull(response);

            foreach (var rs in response.Results) {
                Assert.AreEqual(MerchantAccountStatus.INACTIVE.ToString(), rs.Status.ToString());
            }
        }

        [TestMethod]
        public void FindAccountsInfo_SearchByName() {
            var response = ReportingService.FindAccounts(1, 10)
                .OrderBy(MerchantAccountsSortProperty.TIME_CREATED, SortDirection.Ascending)
                .Where(SearchCriteria.StartDate, startDate)
                .And(SearchCriteria.EndDate, endDate)
                .And(SearchCriteria.AccountName, "Sandbox FMA")
                .Execute();

            Assert.IsNotNull(response);

            foreach (var rs in response.Results) {
                Assert.AreEqual(MerchantAccountStatus.ACTIVE.ToString(), rs.Status.ToString());
            }
        }

        [TestMethod]
        public void FindAccountsInfo_SearchById() {
            var response = ReportingService.FindAccounts(1, 10)
                .OrderBy(MerchantAccountsSortProperty.TIME_CREATED, SortDirection.Ascending)
                .Where(SearchCriteria.StartDate, startDate)
                .And(SearchCriteria.EndDate, endDate)
                .And(SearchCriteria.ResourceId, "FMA_a07e67cdfdc641c4a5fe77a7f9f96cdd")
                .Execute();

            Assert.IsNotNull(response);

            foreach (var rs in response.Results) {
                Assert.AreEqual(MerchantAccountStatus.ACTIVE.ToString(), rs.Status.ToString());
            }
        }

        [TestMethod]
        public void AccountDetails() {
            var accountId = Accounts.Count > 0 ? Accounts[0].Id : null;

            var response = ReportingService.AccountDetail(accountId)
                .Execute();

            Assert.AreEqual(accountId, response.Id);
        }

        [TestMethod]
        public void AccountDetails_RandomId() {
            var exceptionCaught = false;
            try {
                ReportingService.AccountDetail(Guid.NewGuid().ToString())
                    .Execute();
            } catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("Status Code: NotFound - Retrieve information about this transaction is not supported",
                    ex.Message);
                Assert.AreEqual("INVALID_TRANSACTION_ACTION", ex.ResponseCode);
                Assert.AreEqual("40042", ex.ResponseMessage);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void AccountDetails_NullId() {
            var exceptionCaught = false;
            try {
                ReportingService.AccountDetail("null")
                    .Execute();
            } catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("Status Code: BadRequest - Account details does not exist for null", ex.Message);
                Assert.AreEqual("INVALID_REQUEST_DATA", ex.ResponseCode);
                Assert.AreEqual("40041", ex.ResponseMessage);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void EditAccountInformation() {
            var billingAddress = new Address {
                StreetAddress1 = "123 Merchant Street",
                StreetAddress2 = "Suite 2",
                StreetAddress3 = "foyer",
                City = "Beverly Hills",
                State = "CA",
                PostalCode = "90210",
                CountryCode = "US"
            };

            var creditCardInformation = CardInformation();

            var merchants = new ReportingService().FindMerchants(1, 10)
                .OrderBy(MerchantAccountsSortProperty.TIME_CREATED, SortDirection.Ascending)
                .Where(SearchCriteria.MerchantStatus, MerchantAccountStatus.ACTIVE)
                .Execute();

            Assert.IsTrue(merchants.Results.Count > 0);
            var merchant = User.FromId(merchants.Results[0].Id, UserType.MERCHANT);

            var response = ReportingService.FindAccounts(1, 10)
                    .OrderBy(MerchantAccountsSortProperty.TIME_CREATED, SortDirection.Ascending)
                    .Where(SearchCriteria.StartDate, startDate)
                    .And(SearchCriteria.EndDate, endDate)
                    .And(DataServiceCriteria.MerchantId, merchants.Results[0].Id)
                    .And(SearchCriteria.AccountStatus, MerchantAccountStatus.ACTIVE)
                    .Execute();

            var accountSummary = response.Results.FirstOrDefault(x => x.Type == MerchantAccountType.FUND_MANAGEMENT);
            var exceptionCaught = false;
            try {
                //It can only be tested in production environment
                var editAccountResponse = payFacService.EditAccount()
                .WithAccountNumber(accountSummary.Id)
                .WithUserReference(merchant.UserReference)
                .WithAddress(billingAddress)
                .WithCreditCardData(creditCardInformation)
                .Execute();               
            }
             catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("Status Code: InternalServerError - Miscellaneous error",
                    ex.Message);
                Assert.AreEqual("SYSTEM_ERROR_DOWNSTREAM", ex.ResponseCode);
                Assert.AreEqual("50134", ex.ResponseMessage);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void AccountAddressLookup() {
            var accessTokenInfo = GpApiService.GenerateTransactionKey(config);

            var address = new Address {
                PostalCode = "CB6 1AS",
                StreetAddress1 = "2649",
                StreetAddress2 = "Primrose"
            };
            /** @var MerchantAccountSummary $response */
            var response = ReportingService.AccountDetail(accessTokenInfo?.MerchantManagementAccountID)
                .WithPaging(1, 10)
                .Where(SearchCriteria.Address, address)
                .Execute();

            Assert.IsTrue(response.Addresses.Count > 0);
            Assert.AreEqual(address.PostalCode, response.Addresses[0].PostalCode);
        }
       
        [TestMethod]
        public void EditAccountInformation_WithoutCardDetails() {
            var billingAddress = new Address {
                StreetAddress1 = "123 Merchant Street",
                StreetAddress2 = "Suite 2",
                StreetAddress3 = "foyer",
                City = "Beverly Hills",
                State = "CA",
                PostalCode = "90210",
                CountryCode = "US"
            };

            var merchants = new ReportingService().FindMerchants(1, 10)
               .OrderBy(MerchantAccountsSortProperty.TIME_CREATED, SortDirection.Ascending)
               .Where(SearchCriteria.MerchantStatus, MerchantAccountStatus.ACTIVE)
               .Execute();

            Assert.IsTrue(merchants.Results.Count > 0);
            var merchant = User.FromId(merchants.Results[0].Id, UserType.MERCHANT);

            var response = ReportingService.FindAccounts(1, 10)
                    .OrderBy(MerchantAccountsSortProperty.TIME_CREATED, SortDirection.Ascending)
                    .Where(SearchCriteria.StartDate, startDate)
                    .And(SearchCriteria.EndDate, endDate)
                    .And(DataServiceCriteria.MerchantId, merchants.Results[0].Id)
                    .And(SearchCriteria.AccountStatus, MerchantAccountStatus.ACTIVE)
                    .Execute();

            var accountSummary = response.Results.FirstOrDefault(x => x.Type == MerchantAccountType.FUND_MANAGEMENT);
            var exceptionCaught = false;
            try {
                new PayFacService().EditAccount()
                    .WithAccountNumber(accountSummary.Id)
                    .WithUserReference(merchant.UserReference)
                    .WithAddress(billingAddress)
                    .Execute();
            } catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("Status Code: BadRequest - Request expects the following fields payer.payment_method.name",
                    ex.Message);
                Assert.AreEqual("MANDATORY_DATA_MISSING", ex.ResponseCode);
                Assert.AreEqual("40005", ex.ResponseMessage);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void EditAccountInformation_WithoutAddress() {
            var creditCardInformation = CardInformation();

            var merchants = new ReportingService().FindMerchants(1, 10)
               .OrderBy(MerchantAccountsSortProperty.TIME_CREATED, SortDirection.Ascending)
               .Where(SearchCriteria.MerchantStatus, MerchantAccountStatus.ACTIVE)
               .Execute();

            Assert.IsTrue(merchants.Results.Count > 0);
            var merchant = User.FromId(merchants.Results[0].Id, UserType.MERCHANT);

            var response = ReportingService.FindAccounts(1, 10)
                    .OrderBy(MerchantAccountsSortProperty.TIME_CREATED, SortDirection.Ascending)
                    .Where(SearchCriteria.StartDate, startDate)
                    .And(SearchCriteria.EndDate, endDate)
                    .And(DataServiceCriteria.MerchantId, merchants.Results[0].Id)
                    .And(SearchCriteria.AccountStatus, MerchantAccountStatus.ACTIVE)
                    .Execute();

            var accountSummary = response.Results.FirstOrDefault(x => x.Type == MerchantAccountType.FUND_MANAGEMENT);
            var exceptionCaught = false;
            try {
                new PayFacService().EditAccount()
                    .WithAccountNumber(accountSummary.Id)
                    .WithUserReference(merchant.UserReference)
                    .WithCreditCardData(creditCardInformation)
                    .Execute();
            } catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("Status Code: BadRequest - Request expects the following fields payer.billing_address.line_1",
                    ex.Message);
                Assert.AreEqual("MANDATORY_DATA_MISSING", ex.ResponseCode);
                Assert.AreEqual("40005", ex.ResponseMessage);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void EditAccountInformation_WithoutAccountId() {
            var billingAddress = new Address {
                StreetAddress1 = "123 Merchant Street",
                StreetAddress2 = "Suite 2",
                StreetAddress3 = "foyer",
                City = "Beverly Hills",
                State = "CA",
                PostalCode = "90210",
                CountryCode = "US"
            };

            var creditCardInformation = CardInformation();
            var merchants = new ReportingService().FindMerchants(1, 10)
              .OrderBy(MerchantAccountsSortProperty.TIME_CREATED, SortDirection.Ascending)
              .Where(SearchCriteria.MerchantStatus, MerchantAccountStatus.ACTIVE)
              .Execute();

            Assert.IsTrue(merchants.Results.Count > 0);
            var merchant = User.FromId(merchants.Results[0].Id, UserType.MERCHANT);

            var exceptionCaught = false;
            try {
                new PayFacService().EditAccount()
                    .WithAddress(billingAddress)
                    .WithUserReference(merchant.UserReference)
                    .WithCreditCardData(creditCardInformation)
                    .Execute();
            } catch (BuilderException ex) {
                exceptionCaught = true;
                Assert.AreEqual("AccountNumber cannot be null for this transaction type.",
                    ex.Message);            
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }
        
        [TestMethod]
        public void EditAccountInformation_WithoutUserRef() {
            var billingAddress = new Address {
                StreetAddress1 = "123 Merchant Street",
                StreetAddress2 = "Suite 2",
                StreetAddress3 = "foyer",
                City = "Beverly Hills",
                State = "CA",
                PostalCode = "90210",
                CountryCode = "US"
            };

            var creditCardInformation = CardInformation();

            var merchants = new ReportingService().FindMerchants(1, 10)
                .OrderBy(MerchantAccountsSortProperty.TIME_CREATED, SortDirection.Ascending)
                .Where(SearchCriteria.MerchantStatus, MerchantAccountStatus.ACTIVE)
                .Execute();

            Assert.IsTrue(merchants.Results.Count > 0);

            var response = ReportingService.FindAccounts(1, 10)
                .OrderBy(MerchantAccountsSortProperty.TIME_CREATED, SortDirection.Ascending)
                .Where(SearchCriteria.StartDate, startDate)
                .And(SearchCriteria.EndDate, endDate)
                .And(DataServiceCriteria.MerchantId, merchants.Results[0].Id)
                .And(SearchCriteria.AccountStatus, MerchantAccountStatus.ACTIVE)
                .Execute();

            var accountSummary = response.Results.FirstOrDefault(x => x.Type == MerchantAccountType.FUND_MANAGEMENT);
            var exceptionCaught = false;
            try {
                new PayFacService().EditAccount()
                    .WithAccountNumber(accountSummary.Id)
                    .WithAddress(billingAddress)
                    .WithCreditCardData(creditCardInformation)
                    .Execute();
            } catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("Status Code: NotFound - Retrieve information about this transaction is not supported", ex.Message);
                Assert.AreEqual("INVALID_TRANSACTION_ACTION", ex.ResponseCode);
                Assert.AreEqual("40042", ex.ResponseMessage);          
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }
        
        [TestMethod]
        public void AccountAddressLookup_MissingPostalCode() {
            var accessTokenInfo = GpApiService.GenerateTransactionKey(config);
            var address = new Address {
                StreetAddress1 = "2649",
                StreetAddress2 = "Primrose"
            };

            var exceptionCaught = false;
            try {
                ReportingService.AccountDetail(accessTokenInfo?.MerchantManagementAccountID)
                    .WithPaging(1, 10)
                    .Where(SearchCriteria.Address, address)
                    .Execute();
            } catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("Status Code: BadRequest - Request expects the following fields:postal_code.",
                    ex.Message);
                Assert.AreEqual("MANDATORY_DATA_MISSING", ex.ResponseCode);
                Assert.AreEqual("40251", ex.ResponseMessage);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void AccountAddressLookup_MissingAddressLine() {
            var accessTokenInfo = GpApiService.GenerateTransactionKey(config);
            var address = new Address {
                PostalCode = "CB6 1AS",
            };

            var exceptionCaught = false;
            try {
                ReportingService.AccountDetail(accessTokenInfo?.MerchantManagementAccountID)
                    .WithPaging(1, 10)
                    .Where(SearchCriteria.Address, address)
                    .Execute();
            } catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("Status Code: BadRequest - Request expects the following fields:line_1 or line_2.",
                    ex.Message);
                Assert.AreEqual("MANDATORY_DATA_MISSING", ex.ResponseCode);
                Assert.AreEqual("40251", ex.ResponseMessage);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        private CreditCardData CardInformation() {
            return new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = ExpMonth,
                ExpYear = ExpYear,
                Cvn = "123",
                CardPresent = true,
                CardHolderName = "Jason Mason"
            };
        }
    }
}