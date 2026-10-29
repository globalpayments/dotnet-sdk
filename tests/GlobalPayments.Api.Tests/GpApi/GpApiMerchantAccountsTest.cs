using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Enums;
using GlobalPayments.Api.Entities.PayFac;
using GlobalPayments.Api.Entities.Reporting;
using GlobalPayments.Api.Logging;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Utils.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GlobalPayments.Api.Tests.GpApi
{
    [TestClass]
    public class GpApiMerchantAccountsTest : BaseGpApiTests {
        private List<MerchantAccountSummary> Accounts;
        private PayFacService payFacService;
        private static GpApiConfig config;
        private const string TRAConfigName = "AccountsSyncTra";
        private const string TRAAccountId = "TRA_263b79e8e3cd422c84308381725a005a";
        private const string FmaAccountId = "FMA_a07e67cdfdc641c4a5fe77a7f9f96cdd";
        private const string FmaConfigName = "AccountsSyncFma";
        private const string ConfigName = "MerchantAccountsSyncFma";
        private const string MerchantId = "MER_20c2b3fcfedd484e9c3723347db56b71";
        private const string AccountId = "FMA_99d20eb3e84d43cd98afa26467c43f64";
        private readonly DateTime startDate = DateTime.UtcNow.AddYears(-1);
        private readonly DateTime endDate = DateTime.UtcNow.AddDays(-3);

        [ClassInitialize]
        public static void ClassInitialize(TestContext context) {
            ServicesContainer.RemoveConfig();

            config = GpApiConfigSetup(AppIdForMerchant, AppKeyForMerchant, Channel.CardNotPresent);
            config.EnableLogging = true;
            config.RequestLogger = new RequestConsoleLogger();
            ServicesContainer.ConfigureService(config);

            GpApiConfig traGpApiConfig = GpApiConfigSetup(AppId, AppKey, Channel.CardNotPresent);
            traGpApiConfig.EnableLogging = true;
            traGpApiConfig.RequestLogger = new RequestConsoleLogger();
            ServicesContainer.ConfigureService(traGpApiConfig, TRAConfigName);

            GpApiConfig fmaconfig = GpApiConfigSetup(MerchantFmaAppId, MerchantFmaAppKey, Channel.CardNotPresent);
            fmaconfig.MerchantId = MerchantId;
            fmaconfig.EnableLogging = true;
            fmaconfig.RequestLogger = new RequestConsoleLogger();
            ServicesContainer.ConfigureService(fmaconfig, ConfigName);
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
        
        /// <summary>
        /// Retrieves a non-empty sandbox account page and verifies every returned field group.
        /// </summary>
        [TestMethod]
        public void FindAccountsInfo() {
            var fromDate = new DateTime(2022, 5, 21);
            var response = ReportingService.FindAccounts(1, 10)
                .OrderBy(MerchantAccountsSortProperty.TIME_CREATED, SortDirection.Ascending)
                .Where(SearchCriteria.StartDate, fromDate)
                .And(SearchCriteria.EndDate, endDate)
                .Execute();

            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Results);
            Assert.IsTrue(response.Results.Count > 0, "The sandbox must contain an account in the requested date range.");
            Assert.IsTrue(response.TotalRecordCount >= response.Results.Count);
            Assert.AreEqual(response.Results.Count, response.CurrentPageSize);
            Assert.AreEqual(1, response.Page);
            Assert.AreEqual(10, response.PageSize);
            Assert.AreEqual("ASC", response.Order);
            Assert.AreEqual("TIME_CREATED", response.OrderBy);
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.MerchantId));
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.MerchantName));
            Assert.AreEqual(fromDate, response.FromTimeCreated);
            Assert.AreEqual(endDate.Date.AddDays(1).AddMilliseconds(-1), response.ToTimeCreated);
            Assert.IsNotNull(response.Action);
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Action.Id));
            Assert.AreEqual("ACCOUNT_LIST", response.Action.Type);
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Action.TimeCreated));
            Assert.AreEqual(Success, response.Action.ResultCode);
            Assert.IsTrue(response.Action.AppId == config.AppId, "The action must identify the configured app.");
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Action.AppName));

            foreach (var rs in response.Results) {
                Assert.IsNotNull(rs);
                Assert.IsFalse(string.IsNullOrWhiteSpace(rs.Id));
                Assert.IsTrue(rs.Type.HasValue);
                Assert.IsFalse(string.IsNullOrWhiteSpace(rs.Name));
                Assert.IsTrue(rs.Status.HasValue);
                Assert.IsNotNull(rs.Permissions);
                Assert.IsTrue(rs.Permissions.Count > 0);
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

        /// <summary>
        /// Retrieves the designated TRA account and verifies its mapped processing settings and action.
        /// </summary>
        [TestMethod]
        [TestCategory("Sandbox")]
        public void FindAccountDetails_WithTransactionProcessingId_ReadOnly()
        {
            var response = ReportingService.AccountDetail(TRAAccountId).Execute(TRAConfigName);

            Assert.IsNotNull(response);
            Assert.AreEqual(TRAAccountId, response.Id);
            Assert.AreEqual(MerchantAccountType.TRANSACTION_PROCESSING, response.Type);
            Assert.AreEqual("Tranasaction_processing_VT_truust", response.Name);
            Assert.AreEqual(MerchantAccountStatus.ACTIVE, response.Status);
            Assert.AreEqual("2026-03-13T07:39:07.000Z", response.TimeCreated);
            Assert.AreEqual(string.Empty, response.TimeLastUpdated);
            Assert.IsNotNull(response.Permissions);
            CollectionAssert.Contains(response.Permissions, "ACC_GET_Single");
            Assert.IsNotNull(response.Countries);
            Assert.AreEqual(244, response.Countries.Count);
            CollectionAssert.Contains(response.Countries, "US");
            Assert.IsNotNull(response.Currencies);
            Assert.AreEqual(172, response.Currencies.Count);
            CollectionAssert.Contains(response.Currencies, "USD");
            CollectionAssert.AreEqual(new[] { Channel.CardNotPresent }, response.Channels);
            CollectionAssert.AreEquivalent(new[] { PaymentMethodName.BankTransfer, PaymentMethodName.Card }, response.PaymentMethods);

            Assert.IsNotNull(response.AccountConfigurations);
            Assert.AreEqual(1, response.AccountConfigurations.Count);
            var configuration = response.AccountConfigurations[0];
            Assert.IsNotNull(configuration);
            Assert.AreEqual("RTC_0168143076070933714650517457841669582323", configuration.Id);
            Assert.AreEqual("ACTIVE", configuration.Status);
            Assert.AreEqual(string.Empty, configuration.SerialNumber);
            CollectionAssert.AreEquivalent(new[] {
                "AUTHORIZE", "DEVICE_LIST", "DEVICE_SINGLE", "LINK_CREATE",
                "LINK_EDIT", "LINK_LIST", "LINK_SINGLE"
            }, configuration.Actions);
            CollectionAssert.AreEquivalent(response.Countries, configuration.Countries);
            CollectionAssert.AreEquivalent(response.Currencies, configuration.Currencies);
            CollectionAssert.AreEquivalent(response.Channels, configuration.Channels);
            CollectionAssert.AreEquivalent(response.PaymentMethods, configuration.PaymentMethods);
            Assert.IsNotNull(configuration.Platform);
            Assert.AreEqual("PFC_fbb6c72a9d62461990fd6933e56791a1", configuration.Platform.Id);
            Assert.AreEqual("PTF_fbb6c72a9d62461990fd6933e56791a1", configuration.Platform.Reference);
            Assert.AreEqual("TRUUST", configuration.Platform.Name);
            Assert.AreEqual("truust_testing_VT", configuration.Platform.PlatformName);
            Assert.AreEqual("truust_testing_mid01", configuration.Platform.PlatformMid);
            Assert.AreEqual("GB", configuration.Platform.Country);
            Assert.IsFalse(string.IsNullOrWhiteSpace(configuration.Platform.Email));
            Assert.AreEqual("ENABLE", configuration.Platform.Authentication);
            Assert.IsFalse(string.IsNullOrWhiteSpace(configuration.Platform.MaskedSecretKeyLast4));
            Assert.IsNull(configuration.Platform.MaskedCertstrKeyLast4);
            Assert.IsNull(configuration.Platform.TermId);
            Assert.IsNull(response.PaymentProcessingStatistics);
            Assert.IsNull(response.Capabilities);
            Assert.IsNull(response.Platform);

            Assert.IsNotNull(response.Action);
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Action.Id));
            Assert.AreEqual("ACCOUNT_SINGLE", response.Action.Type);
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Action.TimeCreated));
            Assert.AreEqual(Success, response.Action.ResultCode);
            Assert.IsTrue(response.Action.AppId == AppId, "The action must identify the configured app.");
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Action.AppName));
        }

        /// <summary>
        /// Sets the designated merchant-scoped FMA status to ACTIVE using a status-only PATCH.
        /// </summary>
        [TestMethod]
        [TestCategory("Sandbox")]
        public void EditFmaAccount_WithMerchantId_UpdatesStatus()
        {
            var response = new PayFacService().EditAccount()
                .WithAccountNumber(AccountId)
                .WithAccountUpdateData(new AccountUpdateData
                {
                    Status = MerchantAccountStatus.ACTIVE
                })
                .Execute(ConfigName);

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.AreEqual(Success, response.ResponseMessage);
            Assert.AreEqual(AccountId, response.AccountId);
            Assert.IsNotNull(response.AccountDetails);
            Assert.AreEqual(AccountId, response.AccountDetails.Id);
            Assert.AreEqual(MerchantAccountType.FUND_MANAGEMENT, response.AccountDetails.Type);
            Assert.AreEqual(MerchantAccountStatus.ACTIVE, response.AccountDetails.Status);
            Assert.IsNotNull(response.Action);
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Action.Id));
            Assert.AreEqual(Success, response.Action.ResultCode);
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Action.TimeCreated));
            Assert.IsTrue(response.Action.AppId == MerchantFmaAppId, "The action must identify the configured app.");

            var account = ReportingService.AccountDetail(AccountId).Execute(ConfigName);

            Assert.IsNotNull(account);
            Assert.AreEqual(AccountId, account.Id);
            Assert.AreEqual(MerchantAccountStatus.ACTIVE, account.Status);
        }

        /// <summary>
        /// Rejects a status-only edit without an account ID before sending the PATCH.
        /// </summary>
        [TestMethod]
        public void EditFmaAccount_WithoutAccountId_ThrowsBuilderException()
        {
            var error = Assert.ThrowsException<BuilderException>(() =>
                new PayFacService().EditAccount()
                    .WithAccountUpdateData(new AccountUpdateData
                    {
                        Status = MerchantAccountStatus.ACTIVE
                    })
                    .Execute(ConfigName));

            Assert.AreEqual("AccountNumber cannot be null for this transaction type.", error.Message);
        }

        /// <summary>
        /// Retrieves the merchant-scoped FMA and checks all returned field groups.
        /// </summary>
        [TestMethod]
        public void FindFmaAccountDetails_WithMerchantId_ReadOnly()
        {
            var response = ReportingService.AccountDetail(AccountId).Execute(ConfigName);

            Assert.IsNotNull(response);
            Assert.AreEqual(AccountId, response.Id);
            Assert.AreEqual(MerchantAccountType.FUND_MANAGEMENT, response.Type);
            Assert.AreEqual(MerchantAccountStatus.ACTIVE, response.Status);
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Name));
            Assert.AreEqual("2025-01-23T09:40:58.000Z", response.TimeCreated);
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.TimeLastUpdated));
            Assert.IsNotNull(response.Permissions);
            CollectionAssert.AreEquivalent(new[] {
                "APP_GET_Single", "ACC_GET_BTR_List", "ACC_GET_DIS_List", "ACC_GET_List",
                "PFC_GET_List", "TRF_POST_Transfer", "ACC_GET_Single", "ACC_GET_Single_Platform",
                "ACT_POST_Auto_Action", "ACT_POST_Multiple", "APP_GET_List", "APP_GET_Single_Key",
                "APP_PATCH_Edit", "APP_POST_Create", "APP_POST_Search_PFCValidate", "FMA_GET_List",
                "FMA_LIMIT_Notification", "FND_POST_Create", "GET_Single", "PFC_GET_Single",
                "TRF_POST_Reverse"
            }, response.Permissions);
            Assert.IsNotNull(response.Capabilities);
            CollectionAssert.AreEquivalent(new[] {
                "PAY_OUT_ENABLE", "CARD_PROCESSING_OVER_LIMIT_ENABLE",
                "BANK_TRANSFER_PROCESSING_OVER_LIMIT_ENABLE"
            }, response.Capabilities);

            var statistics = response.PaymentProcessingStatistics;
            Assert.IsNotNull(statistics);
            Assert.AreEqual(50000m, statistics.CardTransactionLimitAmount);
            Assert.AreEqual(300000m, statistics.CardMonthlyLimitAmount);
            Assert.AreEqual(10000m, statistics.BankTransferPerTransactionLimitAmount);
            Assert.AreEqual(3000000m, statistics.BankTransferMonthlyLimitAmount);
            Assert.IsTrue(statistics.BankTransferMonthToDateProcessedAmount.HasValue);
            Assert.IsTrue(statistics.CardMonthToDateProcessedAmount.HasValue);
            Assert.IsTrue(statistics.AvailableBalanceAmount.HasValue);
            Assert.IsTrue(statistics.PendingBalanceAmount.HasValue);
            Assert.IsTrue(statistics.ReserveBalanceAmount.HasValue);
            Assert.AreEqual(500m, statistics.AllowedNegativeBalanceAmount);
            Assert.AreEqual(100m, statistics.DisablePayoutCardLimitPercentage);
            Assert.AreEqual(100m, statistics.DisablePayoutBankTransferLimitPercentage);
            Assert.IsTrue(statistics.CardMonthToDateProcessedPercentage.HasValue);
            Assert.IsTrue(statistics.BankTransferMonthToDateProcessedPercentage.HasValue);

            Assert.IsNotNull(response.Platform);
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Platform.Id));
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Platform.Reference));
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Platform.Name));
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Platform.PlatformName));
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Platform.PlatformMid));
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Platform.MaskedCertstrKeyLast4));
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Platform.TermId));
            Assert.IsNotNull(response.Action);
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Action.Id));
            Assert.AreEqual("ACCOUNT_SINGLE", response.Action.Type);
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Action.TimeCreated));
            Assert.AreEqual(Success, response.Action.ResultCode);
            Assert.IsTrue(response.Action.AppId == MerchantFmaAppId, "The action must identify the configured app.");
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Action.AppName));
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
                .And(SearchCriteria.ResourceId, FmaAccountId)
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

            Assert.IsNotNull(response);
            Assert.AreEqual(accountId, response.Id);
        }

        /// <summary>
        /// Retrieves the designated sandbox FMA using merchant credentials without changing account state.
        /// </summary>
        [TestMethod]
        public void FindFmaAccountDetails_ReadOnly() {
            var fmaConfig = GpApiConfigSetup(AppIdForMerchant, AppKeyForMerchant, Channel.CardNotPresent);
            fmaConfig.EnableLogging = false;
            fmaConfig.RequestLogger = new RequestConsoleLogger();
            ServicesContainer.ConfigureService(fmaConfig, FmaConfigName);

                var response = ReportingService.AccountDetail(FmaAccountId).Execute(FmaConfigName);

                Assert.IsNotNull(response);
                Assert.AreEqual(FmaAccountId, response.Id);
                Assert.AreEqual(MerchantAccountType.FUND_MANAGEMENT, response.Type);
                Assert.AreEqual("Sandbox FMA", response.Name);
                Assert.AreEqual(MerchantAccountStatus.ACTIVE, response.Status);
                Assert.AreEqual("2022-11-02T02:00:47.000Z", response.TimeCreated);
                Assert.AreEqual(string.Empty, response.TimeLastUpdated);
                CollectionAssert.AreEquivalent(new[] {
                    "FMA_GET_List", "FND_POST_Create", "ACC_GET_List", "ACC_GET_Single",
                    "ACC_GET_Single_Platform", "ACT_POST_Auto_Action", "ACT_POST_Multiple",
                    "APP_GET_List", "APP_GET_Single", "APP_GET_Single_Key",
                    "APP_POST_Search_PFCValidate", "GET_Single", "PFC_GET_List", "PFC_GET_Single",
                    "TRF_POST_Reverse", "TRF_POST_Transfer"
                }, response.Permissions);
                CollectionAssert.AreEquivalent(new[] {
                    "PAY_OUT_ENABLE", "CARD_PROCESSING_OVER_LIMIT_ENABLE",
                    "BANK_TRANSFER_PROCESSING_OVER_LIMIT_ENABLE"
                }, response.Capabilities);

                var statistics = response.PaymentProcessingStatistics;
                Assert.IsNotNull(statistics);
                Assert.AreEqual(50000m, statistics.CardTransactionLimitAmount);
                Assert.AreEqual(300000m, statistics.CardMonthlyLimitAmount);
                Assert.AreEqual(10000m, statistics.BankTransferPerTransactionLimitAmount);
                Assert.AreEqual(3000000m, statistics.BankTransferMonthlyLimitAmount);
                Assert.AreEqual(0m, statistics.BankTransferMonthToDateProcessedAmount);
                Assert.AreEqual(0m, statistics.CardMonthToDateProcessedAmount);
                Assert.AreEqual(1.17m, statistics.AvailableBalanceAmount);
                Assert.AreEqual(0m, statistics.PendingBalanceAmount);
                Assert.AreEqual(0m, statistics.ReserveBalanceAmount);
                Assert.AreEqual(500m, statistics.AllowedNegativeBalanceAmount);
                Assert.AreEqual(100m, statistics.DisablePayoutCardLimitPercentage);
                Assert.AreEqual(100m, statistics.DisablePayoutBankTransferLimitPercentage);
                Assert.AreEqual(0m, statistics.CardMonthToDateProcessedPercentage);
                Assert.AreEqual(0m, statistics.BankTransferMonthToDateProcessedPercentage);

                Assert.IsNotNull(response.Platform);
                Assert.IsFalse(string.IsNullOrWhiteSpace(response.Platform.Id));
                Assert.IsFalse(string.IsNullOrWhiteSpace(response.Platform.Reference));
                Assert.AreEqual("PROPAY", response.Platform.Name);
                Assert.AreEqual("Sandbox Propay", response.Platform.PlatformName);
                Assert.IsFalse(string.IsNullOrWhiteSpace(response.Platform.PlatformMid));
                Assert.IsFalse(string.IsNullOrWhiteSpace(response.Platform.MaskedCertstrKeyLast4));
                Assert.IsFalse(string.IsNullOrWhiteSpace(response.Platform.TermId));
                Assert.IsNotNull(response.Action);
                Assert.IsFalse(string.IsNullOrWhiteSpace(response.Action.Id));
                Assert.AreEqual("ACCOUNT_SINGLE", response.Action.Type);
                Assert.IsFalse(string.IsNullOrWhiteSpace(response.Action.TimeCreated));
                Assert.AreEqual(Success, response.Action.ResultCode);
                Assert.IsTrue(response.Action.AppId == AppIdForMerchant, "The action must identify the configured merchant app.");
                Assert.IsFalse(string.IsNullOrWhiteSpace(response.Action.AppName));

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

        #region Transfer and Split
        [TestMethod]
        public void TransferFunds() {
            var merchants = GetMerchants();
            Assert.IsNotNull(merchants);
            var merchantSender = merchants.FirstOrDefault();
            Assert.IsNotNull(merchantSender);
            var merchantRecipient = merchants.FirstOrDefault(x => x.Id != merchantSender.Id);
            Assert.IsNotNull(merchantRecipient);
            var accountSender = GetAccountByType(merchantSender.Id, MerchantAccountType.FUND_MANAGEMENT);
            Assert.IsNotNull(accountSender);
            var accountRecipient = GetAccountByType(merchantRecipient.Id, MerchantAccountType.FUND_MANAGEMENT);
            Assert.IsNotNull(accountRecipient);
            
            var funds = new AccountFunds();
            funds.AccountId = accountSender.Id;
            funds.AccountName = accountSender.Name;
            funds.RecipientAccountId = accountRecipient.Id;
            funds.MerchantId = merchantSender.Id;
            funds.UsableBalanceMode = UsableBalanceMode.AVAILABLE_AND_PENDING_BALANCE;

            var description = Path.GetRandomFileName().Replace(".", "").Substring(0, 11);

            var transfer = funds.Transfer(1m)
                .WithClientTransactionId("")
                .WithDescription(description)
                .Execute();

            Assert.IsNotNull(transfer.TransactionId);
            Assert.AreEqual(1m, transfer.BalanceAmount);
            Assert.AreEqual(Success, transfer.ResponseMessage.ToUpper());
            Assert.AreEqual(Success, transfer.ResponseCode.ToUpper());
        }

        [TestMethod]
        public void TransferFunds_WithoutSenderAccountName() {
            var merchants = GetMerchants();

            var merchantSender = merchants.FirstOrDefault();
            var merchantRecipient = merchants.FirstOrDefault(x => x.Id != merchantSender.Id);

            var accountSender = GetAccountByType(merchantSender.Id, MerchantAccountType.FUND_MANAGEMENT);
            var accountRecipient = GetAccountByType(merchantRecipient.Id, MerchantAccountType.FUND_MANAGEMENT);

            var funds = new AccountFunds();
            funds.AccountId = accountSender.Id;
            funds.RecipientAccountId = accountRecipient.Id;
            funds.MerchantId = merchantSender.Id;
            funds.UsableBalanceMode = UsableBalanceMode.AVAILABLE_AND_PENDING_BALANCE;

            var description = Path.GetRandomFileName().Replace(".", "").Substring(0, 11);

            var transfer = funds.Transfer(0.01m)
                .WithClientTransactionId("")
                .WithDescription(description)
                .Execute();

            Assert.IsNotNull(transfer.TransactionId);
            Assert.AreEqual(0.01m, transfer.BalanceAmount);
            Assert.AreEqual(Success, transfer.ResponseMessage.ToUpper());
            Assert.AreEqual(Success, transfer.ResponseCode.ToUpper());
        }

        [TestMethod]
        public void TransferFunds_WithoutUsableBalanceMode() {
            var merchants = GetMerchants();

            var merchantSender = merchants.FirstOrDefault();
            var merchantRecipient = merchants.FirstOrDefault(x => x.Id != merchantSender.Id);

            var accountSender = GetAccountByType(merchantSender.Id, MerchantAccountType.FUND_MANAGEMENT);
            var accountRecipient = GetAccountByType(merchantRecipient.Id, MerchantAccountType.FUND_MANAGEMENT);

            var funds = new AccountFunds();
            funds.AccountId = accountSender.Id;
            funds.AccountName = accountSender.Name;
            funds.RecipientAccountId = accountRecipient.Id;
            funds.MerchantId = merchantSender.Id;

            var description = Path.GetRandomFileName().Replace(".", "").Substring(0, 11);

            var transfer = funds.Transfer(0.01m)
                .WithClientTransactionId("")
                .WithDescription(description)
                .Execute();

            Assert.IsNotNull(transfer.TransactionId);
            Assert.AreEqual(0.01m, transfer.BalanceAmount);
            Assert.AreEqual(Success, transfer.ResponseMessage.ToUpper());
            Assert.AreEqual(Success, transfer.ResponseCode.ToUpper());
        }

        [TestMethod]
        public void TransferFunds_WithoutSenderAccountId() {
            var merchants = GetMerchants();

            var merchantSender = merchants.FirstOrDefault();
            var merchantRecipient = merchants.FirstOrDefault(x => x.Id != merchantSender.Id);

            var accountSender = GetAccountByType(merchantSender.Id, MerchantAccountType.FUND_MANAGEMENT);
            var accountRecipient = GetAccountByType(merchantRecipient.Id, MerchantAccountType.FUND_MANAGEMENT);

            var funds = new AccountFunds();
            funds.AccountName = accountSender.Name;
            funds.RecipientAccountId = accountRecipient.Id;
            funds.MerchantId = merchantSender.Id;
            funds.UsableBalanceMode = UsableBalanceMode.AVAILABLE_AND_PENDING_BALANCE;

            var description = Path.GetRandomFileName().Replace(".", "").Substring(0, 11);

            var transfer = funds.Transfer(0.01m)
                .WithClientTransactionId("")
                .WithDescription(description)
                .Execute();

            Assert.IsNotNull(transfer.TransactionId);
            Assert.AreEqual(0.01m, transfer.BalanceAmount);
            Assert.AreEqual(Success, transfer.ResponseMessage.ToUpper());
            Assert.AreEqual(Success, transfer.ResponseCode.ToUpper());
        }
        
        [TestMethod]
        public void TransferFunds_OnlyMandatoryFields() {
            var merchants = GetMerchants();

            var merchantSender = merchants.FirstOrDefault();
            var merchantRecipient = merchants.FirstOrDefault(x => x.Id != merchantSender.Id);

            var accountSender = GetAccountByType(merchantSender.Id, MerchantAccountType.FUND_MANAGEMENT);
            var accountRecipient = GetAccountByType(merchantRecipient.Id, MerchantAccountType.FUND_MANAGEMENT);

            var funds = new AccountFunds();
            funds.AccountId = accountSender.Id;
            funds.RecipientAccountId = accountRecipient.Id;
            funds.MerchantId = merchantSender.Id;
            funds.UsableBalanceMode = UsableBalanceMode.AVAILABLE_AND_PENDING_BALANCE;

            var transfer = funds.Transfer(0.11m)
                .Execute();

            Assert.IsNotNull(transfer.TransactionId);
            Assert.AreEqual(0.11m, transfer.BalanceAmount);
            Assert.AreEqual(Success, transfer.ResponseMessage.ToUpper());
            Assert.AreEqual(Success, transfer.ResponseCode.ToUpper());
        }
        
        [TestMethod]
        public void TransferFunds_WithIdempotency() {
            var merchants = GetMerchants();

            var merchantSender = merchants.FirstOrDefault();
            var merchantRecipient = merchants.FirstOrDefault(x => x.Id != merchantSender.Id);

            var accountSender = GetAccountByType(merchantSender.Id, MerchantAccountType.FUND_MANAGEMENT);
            var accountRecipient = GetAccountByType(merchantRecipient.Id, MerchantAccountType.FUND_MANAGEMENT);

            var funds = new AccountFunds();
            funds.AccountId = accountSender.Id;
            funds.AccountName = accountSender.Name;
            funds.RecipientAccountId = accountRecipient.Id;
            funds.MerchantId = merchantSender.Id;
            funds.UsableBalanceMode = UsableBalanceMode.AVAILABLE_AND_PENDING_BALANCE;

            var description = Path.GetRandomFileName().Replace(".", "").Substring(0, 11);

            var idempotencyKey = Guid.NewGuid().ToString();
            var transfer = funds.Transfer(1m)
                .WithClientTransactionId("")
                .WithDescription(description)
                .WithIdempotencyKey(idempotencyKey)
                .Execute();

            Assert.IsNotNull(transfer.TransactionId);
            Assert.AreEqual(1m, transfer.BalanceAmount);
            Assert.AreEqual(Success, transfer.ResponseMessage.ToUpper());
            Assert.AreEqual(Success, transfer.ResponseCode.ToUpper());
            
            var exceptionCaught = false;
            try {
                funds.Transfer(1m)
                    .WithClientTransactionId("")
                    .WithDescription(description)
                    .WithIdempotencyKey(idempotencyKey)
                    .Execute();
            } catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("DUPLICATE_ACTION", ex.ResponseCode);
                Assert.AreEqual("40039", ex.ResponseMessage);
                Assert.AreEqual($"Status Code: Conflict - Idempotency Key seen before: id={transfer.TransactionId}, status=SUCCESS",
                    ex.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        #endregion

        #region Transfer error scenarios

        [TestMethod]
        public void TransferFunds_WithoutRecipientAccountId() {
            var merchants = GetMerchants();

            var merchantSender = merchants.FirstOrDefault();
            var accountSender = GetAccountByType(merchantSender.Id, MerchantAccountType.FUND_MANAGEMENT);

            var funds = new AccountFunds();
            funds.AccountId = accountSender.Id;
            funds.AccountName = accountSender.Name;
            funds.MerchantId = merchantSender.Id;
            funds.UsableBalanceMode = UsableBalanceMode.AVAILABLE_AND_PENDING_BALANCE;

            var description = Path.GetRandomFileName().Replace(".", "").Substring(0, 11);

            var exceptionCaught = false;
            try {
                funds.Transfer(0.01m)
                   .WithClientTransactionId("")
                   .WithDescription(description)
                   .Execute();
            } catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("Status Code: BadRequest - Request expects the following conditionally mandatory fields recipient_account_id, recipient_account_name.", ex.Message);
                Assert.AreEqual("INVALID_REQUEST_DATA", ex.ResponseCode);
                Assert.AreEqual("40007", ex.ResponseMessage);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void TransferFunds_WithoutSenderAccountIdAndAccountName() {
            var merchants = GetMerchants();

            var merchantSender = merchants.FirstOrDefault();
            var merchantRecipient = merchants.FirstOrDefault(x => x.Id != merchantSender.Id);

            var accountRecipient = GetAccountByType(merchantRecipient.Id, MerchantAccountType.FUND_MANAGEMENT);

            var funds = new AccountFunds();
            funds.RecipientAccountId = accountRecipient.Id;
            funds.MerchantId = merchantSender.Id;
            funds.UsableBalanceMode = UsableBalanceMode.AVAILABLE_AND_PENDING_BALANCE;

            var description = Path.GetRandomFileName().Replace(".", "").Substring(0, 11);

            var exceptionCaught = false;
            try {
                funds.Transfer(0.01m)
                   .WithClientTransactionId("")
                   .WithDescription(description)
                   .Execute();
            } catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("Status Code: BadRequest - Request expects the following conditionally mandatory fields account_id, account_name.", ex.Message);
                Assert.AreEqual("INVALID_REQUEST_DATA", ex.ResponseCode);
                Assert.AreEqual("40007", ex.ResponseMessage);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }
        
        [TestMethod]
        public void TransferFunds_WithoutMerchantId() {
            var merchants = GetMerchants();

            var merchantSender = merchants.FirstOrDefault();
            var merchantRecipient = merchants.FirstOrDefault(x => x.Id != merchantSender.Id);

            var accountSender = GetAccountByType(merchantSender.Id, MerchantAccountType.FUND_MANAGEMENT);
            var accountRecipient = GetAccountByType(merchantRecipient.Id, MerchantAccountType.FUND_MANAGEMENT);

            var funds = new AccountFunds();
            funds.AccountId = accountSender.Id;
            funds.RecipientAccountId = accountRecipient.Id;
            funds.UsableBalanceMode = UsableBalanceMode.AVAILABLE_AND_PENDING_BALANCE;

            var exceptionCaught = false;
            try {
                funds.Transfer(0.01m)
                    .WithClientTransactionId("")
                    .Execute();
            } catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("Status Code: Forbidden - Access token and merchant info do not match", ex.Message);
                Assert.AreEqual("ACTION_NOT_AUTHORIZED", ex.ResponseCode);
                Assert.AreEqual("40003", ex.ResponseMessage);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }
        
        [TestMethod]
        public void TransferFunds_WithoutAmount() {
            var merchants = GetMerchants();

            var merchantSender = merchants.FirstOrDefault();
            var merchantRecipient = merchants.FirstOrDefault(x => x.Id != merchantSender.Id);

            var accountSender = GetAccountByType(merchantSender.Id, MerchantAccountType.FUND_MANAGEMENT);
            var accountRecipient = GetAccountByType(merchantRecipient.Id, MerchantAccountType.FUND_MANAGEMENT);

            var funds = new AccountFunds();
            funds.AccountId = accountSender.Id;
            funds.AccountName = accountSender.Name;
            funds.RecipientAccountId = accountRecipient.Id;
            funds.MerchantId = merchantSender.Id;
            funds.UsableBalanceMode = UsableBalanceMode.AVAILABLE_AND_PENDING_BALANCE;

            var description = Path.GetRandomFileName().Replace(".", "").Substring(0, 11);

            var exceptionCaught = false;
            try {
                funds.Transfer()
                   .WithClientTransactionId("")
                   .WithDescription(description)
                   .Execute();
            } catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("Status Code: BadRequest - Request expects the following fields amount", ex.Message);
                Assert.AreEqual("MANDATORY_DATA_MISSING", ex.ResponseCode);
                Assert.AreEqual("40005", ex.ResponseMessage);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }
        
        [TestMethod]
        public void TransferFunds_WithRandomAccountId() {
            var merchants = GetMerchants();

            var merchantSender = merchants.FirstOrDefault();
            var merchantRecipient = merchants.FirstOrDefault(x => x.Id != merchantSender.Id);

            var accountRecipient = GetAccountByType(merchantRecipient.Id, MerchantAccountType.FUND_MANAGEMENT);

            var funds = new AccountFunds();
            funds.AccountId = Guid.NewGuid().ToString();
            funds.RecipientAccountId = accountRecipient.Id;
            funds.MerchantId = merchantSender.Id;

            var exceptionCaught = false;
            try {
                funds.Transfer(2m)
                    .WithClientTransactionId("")
                    .Execute();
            } catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.IsTrue(ex.Message.Contains("Status Code: BadRequest - Merchant configuration does not exist for the following combination:"));
                Assert.AreEqual("INVALID_REQUEST_DATA", ex.ResponseCode);
                Assert.AreEqual("40041", ex.ResponseMessage);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }
        
        [TestMethod]
        public void TransferFunds_WithRandomRecipientId() {
            var merchants = GetMerchants();
            var merchantSender = merchants.FirstOrDefault();
            var accountSender = GetAccountByType(merchantSender.Id, MerchantAccountType.FUND_MANAGEMENT);

            var funds = new AccountFunds();
            funds.AccountId = accountSender.Id;
            funds.AccountName = accountSender.Name;
            funds.RecipientAccountId = Guid.NewGuid().ToString();
            funds.MerchantId = merchantSender.Id;
            funds.UsableBalanceMode = UsableBalanceMode.AVAILABLE_AND_PENDING_BALANCE;

            var exceptionCaught = false;
            try {
                funds.Transfer(2m)
                    .WithClientTransactionId("")
                    .Execute();
            } catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("Status Code: BadRequest - Transfers may only be initiated between accounts under the same partner program", ex.Message);
                Assert.AreEqual("INVALID_REQUEST_DATA", ex.ResponseCode);
                Assert.AreEqual("40041", ex.ResponseMessage);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }
        
        [TestMethod]
        public void TransferFunds_WithRandomMerchantId() {
            var funds = new AccountFunds {
                AccountId = Guid.NewGuid().ToString(),
                RecipientAccountId = Guid.NewGuid().ToString(),
                MerchantId = Guid.NewGuid().ToString()
            };

            var exceptionCaught = false;
            try {
                funds.Transfer(2m)
                    .WithClientTransactionId("")
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
        public void AddFunds()
        {
            var amount = "10";
            var currency = "USD";
            var accountId = "FMA_a78b841dfbd14803b3a31e4e0c514c72";
            var merchantId = "MER_5096d6b88b0b49019c870392bd98ddac";
            var merchant = User.FromId(merchantId, UserType.MERCHANT);
        
            var response = merchant.AddFunds()
                .WithAmount(amount)
                .WithAccountNumber(accountId)
                .WithPaymentMethodName(PaymentMethodName.BankTransfer)
                .WithPaymentMethodType(PaymentMethodType.Credit)
                .WithCurrency(currency)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("SUCCESS", response.ResponseCode);
            Assert.IsNotNull(response.FundsAccountDetails);
            Assert.AreEqual(FundsStatus.CAPTURED.ToString(), response.FundsAccountDetails.Status);
            Assert.AreEqual(amount, response.FundsAccountDetails.Amount.ToString());
            Assert.AreEqual(currency, response.FundsAccountDetails.Currency);
            Assert.AreEqual("CREDIT", response.FundsAccountDetails.PaymentMethodType);
            Assert.AreEqual("BANK_TRANSFER", response.FundsAccountDetails.PaymentMethodName);
            Assert.IsNotNull(response.FundsAccountDetails.Account);
            Assert.AreEqual(accountId, response.FundsAccountDetails.Account.Id);
        }

        [TestMethod]
        public void AddFunds_OnlyMandatory()
        {
            var amount = "10";
            var accountId = "FMA_a78b841dfbd14803b3a31e4e0c514c72";
            var merchantId = "MER_5096d6b88b0b49019c870392bd98ddac";
            var merchant = User.FromId(merchantId, UserType.MERCHANT);

            var response = merchant.AddFunds()
                .WithAmount(amount)
                .WithAccountNumber(accountId)                
                .WithPaymentMethodType(PaymentMethodType.Credit)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("SUCCESS", response.ResponseCode);
            Assert.IsNotNull(response.FundsAccountDetails);
            Assert.AreEqual(FundsStatus.CAPTURED.ToString(), response.FundsAccountDetails.Status);
            Assert.AreEqual(amount, response.FundsAccountDetails.Amount.ToString());
            Assert.AreEqual("CREDIT", response.FundsAccountDetails.PaymentMethodType);
            Assert.AreEqual("BANK_TRANSFER", response.FundsAccountDetails.PaymentMethodName);
            Assert.IsNotNull(response.FundsAccountDetails.Account);
            Assert.AreEqual(accountId, response.FundsAccountDetails.Account.Id);
        }

        [TestMethod]
        public void AddFunds_InsufficientFunds()
        {
            var amount = "1000000";
            var accountId = "FMA_a78b841dfbd14803b3a31e4e0c514c72";
            var merchantId = "MER_5096d6b88b0b49019c870392bd98ddac";
            var merchant = User.FromId(merchantId, UserType.MERCHANT);

            var response = merchant.AddFunds()
                .WithAmount(amount)
                .WithAccountNumber(accountId)                
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("DECLINED", response.ResponseCode);
            Assert.IsNotNull(response.FundsAccountDetails);
            Assert.AreEqual(FundsStatus.DECLINE.ToString(), response.FundsAccountDetails.Status);
            Assert.AreEqual(amount, response.FundsAccountDetails.Amount.ToString());
            Assert.AreEqual("DEBIT", response.FundsAccountDetails.PaymentMethodType);
            Assert.AreEqual("BANK_TRANSFER", response.FundsAccountDetails.PaymentMethodName);
            Assert.IsNotNull(response.FundsAccountDetails.Account);
            Assert.AreEqual(accountId, response.FundsAccountDetails.Account.Id);
        }

        [TestMethod]
        public void AddFunds_WithoutAmount()
        {
            var currency = "USD";
            var accountId = "FMA_a78b841dfbd14803b3a31e4e0c514c72";
            var merchantId = "MER_5096d6b88b0b49019c870392bd98ddac";
            var merchant = User.FromId(merchantId, UserType.MERCHANT);

            var errorFound = false;
            try
            {
                var response = merchant.AddFunds()
                 .WithAccountNumber(accountId)                 
                 .WithPaymentMethodName(PaymentMethodName.BankTransfer)
                 .WithPaymentMethodType(PaymentMethodType.Credit)
                 .WithCurrency(currency)
                 .Execute();
            }
            catch (BuilderException ex)
            {
                errorFound = true;
                Assert.AreEqual("Amount cannot be null for this transaction type.", ex.Message);
            }
            finally {
                Assert.IsTrue(errorFound);
            }
        }

        [TestMethod]
        public void AddFunds_WithoutAccountNumber()
        {
            var amount = "10";
            var currency = "USD";            
            var merchantId = "MER_5096d6b88b0b49019c870392bd98ddac";
            var merchant = User.FromId(merchantId, UserType.MERCHANT);

            var errorFound = false;
            try
            {
                var response = merchant.AddFunds()
                 .WithAmount(amount)                 
                 .WithPaymentMethodName(PaymentMethodName.BankTransfer)
                 .WithPaymentMethodType(PaymentMethodType.Credit)
                 .WithCurrency(currency)
                 .Execute();
            }
            catch (BuilderException ex)
            {
                errorFound = true;
                Assert.AreEqual("AccountNumber cannot be null for this transaction type.", ex.Message);
            }
            finally
            {
                Assert.IsTrue(errorFound);
            }
        }

        [TestMethod]
        public void AddFunds_WithoutUserRef()
        {
            var amount = "10";
            var currency = "USD";
            var accountId = "FMA_a78b841dfbd14803b3a31e4e0c514c72";                     

            var errorFound = false;
            try
            {
                var response = new User().AddFunds()
                 .WithAmount(amount)
                 .WithAccountNumber(accountId)
                 .WithPaymentMethodName(PaymentMethodName.BankTransfer)
                 .WithPaymentMethodType(PaymentMethodType.Credit)
                 .WithCurrency(currency)
                 .Execute();
            }
            catch (GatewayException ex)
            {
                errorFound = true;
                Assert.AreEqual("property UserId or config MerchantId cannot be null for this transactionType", ex.Message);
            }
            finally
            {
                Assert.IsTrue(errorFound);
            }
        }

        #endregion


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

        private List<MerchantSummary> GetMerchants()
        {
            var merchants = new ReportingService().FindMerchants(1, 10)
              .OrderBy(MerchantAccountsSortProperty.TIME_CREATED, SortDirection.Ascending)
              .Where(SearchCriteria.MerchantStatus, MerchantAccountStatus.ACTIVE)
              .And(SearchCriteria.StartDate, DateTime.UtcNow.AddYears(-3))
              .Execute();

            return merchants.Results;
        }

        private MerchantAccountSummary GetAccountByType(string merchantSenderId, MerchantAccountType merchantAccountType)
        {
            var response = ReportingService.FindAccounts(1, 10)
                   .OrderBy(MerchantAccountsSortProperty.TIME_CREATED, SortDirection.Ascending)
                   .Where(SearchCriteria.StartDate, DateTime.UtcNow.AddYears(-3))
                   .And(SearchCriteria.EndDate, EndDate)
                   .And(DataServiceCriteria.MerchantId, merchantSenderId)
                   .And(SearchCriteria.AccountStatus, MerchantAccountStatus.ACTIVE)
                   .Execute();

            return response.Results.FirstOrDefault(x => x.Type == merchantAccountType);
        }
    }
}