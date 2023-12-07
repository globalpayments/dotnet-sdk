using System;
using System.Collections.Generic;
using System.Linq;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Enums;
using GlobalPayments.Api.Entities.Reporting;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

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
            
            config = GpApiConfigSetup(AppIdForMerchant, AppKeyForMerchant, Channel.CardNotPresent);
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

        #region Transfer and Split
        [TestMethod]
        public void TransferFunds() {
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
            var amount = "10";
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
              .Execute();

            return merchants.Results;
        }

        private MerchantAccountSummary GetAccountByType(string merchantSenderId, MerchantAccountType merchantAccountType)
        {
            var response = ReportingService.FindAccounts(1, 10)
                   .OrderBy(MerchantAccountsSortProperty.TIME_CREATED, SortDirection.Ascending)
                   .Where(SearchCriteria.StartDate, StartDate)
                   .And(SearchCriteria.EndDate, EndDate)
                   .And(DataServiceCriteria.MerchantId, merchantSenderId)
                   .And(SearchCriteria.AccountStatus, MerchantAccountStatus.ACTIVE)
                   .Execute();

            return response.Results.FirstOrDefault(x => x.Type == merchantAccountType);
        }
    }
}