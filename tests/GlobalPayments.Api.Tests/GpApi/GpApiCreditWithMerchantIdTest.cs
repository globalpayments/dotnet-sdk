using System;
using System.Collections.Generic;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Enums;
using GlobalPayments.Api.Entities.Reporting;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.GpApi {
    [TestClass]
    public class GpApiCreditWithMerchantIdTest : BaseGpApiTests {
        private CreditCardData card;
        private static GpApiConfig config;
        private static string merchantConfig;
        private string merchantId;
        private const decimal Amount = 7.8m;
        private const string Currency = "USD";

        [TestInitialize]
        public void TestInitialize() {
            config = GpApiConfigSetup(AppIdForMerchant, AppKeyForMerchant, Channel.CardNotPresent);
            ServicesContainer.ConfigureService(config);

            card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = ExpMonth,
                ExpYear = ExpYear
            };

            var merchants =
                new ReportingService()
                    .FindMerchants(1, 10)
                    .OrderBy(MerchantAccountsSortProperty.TIME_CREATED, SortDirection.Descending)
                    .Where(SearchCriteria.MerchantStatus, MerchantAccountStatus.ACTIVE)
                    .Execute();

            Assert.IsTrue(merchants.Results.Count > 0);
            
            merchantId = merchants.Results[0].Id;

            config.MerchantId = merchantId;
            merchantConfig = "config_" + merchantId;

            var accountsResponse =
                ReportingService
                    .FindAccounts(1, 10)
                    .OrderBy(MerchantAccountsSortProperty.TIME_CREATED, SortDirection.Descending)
                    .Where(SearchCriteria.StartDate, StartDate)
                    .And(SearchCriteria.EndDate, EndDate)
                    .And(DataServiceCriteria.MerchantId, merchantId)
                    .And(SearchCriteria.AccountStatus, MerchantAccountStatus.ACTIVE)
                    .And(SearchCriteria.PaymentMethodName, PaymentMethodName.Card)
                    .Execute();

            Assert.IsNotNull(accountsResponse);

            var transactionAccounts = new List<MerchantAccountSummary>();

            foreach (var account in accountsResponse.Results) {
                if (account.Type == MerchantAccountType.TRANSACTION_PROCESSING &&
                    account.PaymentMethods.Contains(PaymentMethodName.Card)) {
                    transactionAccounts.Add(account);
                }
            }

            var accessTokenInfo = new AccessTokenInfo {
                TransactionProcessingAccountID = transactionAccounts[0].Id
            };

            config.AccessTokenInfo = accessTokenInfo;
            
            ServicesContainer.ConfigureService(config, merchantConfig);
        }

        [TestMethod]
        public void CreditAuthorization() {
            var transaction = card.Authorize(Amount)
                .WithCurrency(Currency)
                .Execute(merchantConfig);
            
            Assert.IsNotNull(transaction);
            Assert.AreEqual(Success, transaction.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Preauthorized), transaction.ResponseMessage);

            var capture = transaction.Capture(Amount + 0.5m)
                .WithGratuity(0.5m)
                .Execute(merchantConfig);
            
            Assert.IsNotNull(capture);
            Assert.AreEqual(Success, capture.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), capture.ResponseMessage);
        }

        [TestMethod]
        public void CreditAuthorization_CaptureLowerAmount() {
            var transaction = card.Authorize(Amount)
                .WithCurrency(Currency)
                .Execute(merchantConfig);
            
            Assert.IsNotNull(transaction);
            Assert.AreEqual(Success, transaction.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Preauthorized), transaction.ResponseMessage);

            var capture = transaction.Capture(2.99m)
                .WithGratuity(2m)
                .Execute(merchantConfig);
            
            Assert.IsNotNull(capture);
            Assert.AreEqual(Success, capture.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), capture.ResponseMessage);
        }

        [TestMethod]
        public void CreditAuthorization_CaptureHigherAmount() {
            var transaction = card.Authorize(Amount)
                .WithCurrency(Currency)
                .Execute(merchantConfig);
            
            Assert.IsNotNull(transaction);
            Assert.AreEqual(Success, transaction.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Preauthorized), transaction.ResponseMessage);

            var capture = transaction.Capture(Amount * 1.15m)
                .WithGratuity(2m)
                .Execute(merchantConfig);
            
            Assert.IsNotNull(capture);
            Assert.AreEqual(Success, capture.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), capture.ResponseMessage);
        }

        [TestMethod]
        public void CreditAuthorization_CaptureHigherAmount_WithError() {
            var transaction = card.Authorize(Amount)
                .WithCurrency(Currency)
                .Execute(merchantConfig);
            
            Assert.IsNotNull(transaction);
            Assert.AreEqual(Success, transaction.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Preauthorized), transaction.ResponseMessage);

            var errorFound = false;
            try {
                transaction.Capture(Amount * 1.16m)
                    .WithGratuity(2m)
                    .Execute(merchantConfig);
            }
            catch (GatewayException ex) {
                errorFound = true;
                Assert.AreEqual("Status Code: BadRequest - Can't settle for more than 115% of that which you authorised ", ex.Message);
                Assert.AreEqual("INVALID_REQUEST_DATA", ex.ResponseCode);
                Assert.AreEqual("50020", ex.ResponseMessage);
            } finally {
                Assert.IsTrue(errorFound);
            }
        }

        [TestMethod]
        public void CreditSale() {
            var address = new Address {
                StreetAddress1 = "123 Main St.",
                City = "Downtown",
                State = "NJ",
                Country = "US",
                PostalCode = "12345"
            };

            var response = card.Charge(Amount)
                .WithCurrency(Currency)
                .WithAddress(address)
                .Execute(merchantConfig);
            
            Assert.IsNotNull(response);
            Assert.AreEqual(Success, response.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response.ResponseMessage);
        }

        [TestMethod]
        public void CreditSale_WithRequestMultiUseToken() {
            var response = card.Charge(Amount)
                .WithCurrency(Currency)
                .WithRequestMultiUseToken(true)
                .Execute(merchantConfig);
            
            Assert.IsNotNull(response);
            Assert.AreEqual(Success, response.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response.ResponseMessage);
            Assert.IsNotNull(response.Token);
        }

        [TestMethod]
        public void CreditRefund() {
            var response = card.Refund(Amount)
                .WithCurrency(Currency)
                .Execute(merchantConfig);
            
            Assert.IsNotNull(response);
            Assert.AreEqual(Success, response.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response.ResponseMessage);
        }

        [TestMethod]
        public void CreditRefundTransaction() {
            var transaction = card.Charge(Amount)
                .WithCurrency(Currency)
                .Execute(merchantConfig);
            
            Assert.IsNotNull(transaction);
            Assert.AreEqual(Success, transaction.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), transaction.ResponseMessage);

            var response = transaction.Refund(Amount)
                .WithCurrency(Currency)
                .Execute(merchantConfig);
            
            Assert.IsNotNull(response);
            Assert.AreEqual(Success, response.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response.ResponseMessage);
        }

        [TestMethod]
        public void CreditRefundTransaction_WithIdempotencyKey() {
            var idempotencyKey = Guid.NewGuid().ToString();

            var transaction = card.Charge(Amount)
                .WithCurrency(Currency)
                .Execute(merchantConfig);
            
            Assert.IsNotNull(transaction);
            Assert.AreEqual(Success, transaction.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), transaction.ResponseMessage);

            var response = transaction.Refund(Amount)
                .WithCurrency(Currency)
                .WithIdempotencyKey(idempotencyKey)
                .Execute(merchantConfig);
            
            Assert.IsNotNull(response);
            Assert.AreEqual(Success, response.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response.ResponseMessage);

            var exceptionCaught = false;
            try {
                transaction.Refund(Amount)
                    .WithCurrency(Currency)
                    .WithIdempotencyKey(idempotencyKey)
                    .Execute(merchantConfig);
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("DUPLICATE_ACTION", ex.ResponseCode);
                Assert.AreEqual("40039", ex.ResponseMessage);
                Assert.AreEqual($"Status Code: Conflict - Idempotency Key seen before: id={response.TransactionId}",
                    ex.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void CreditRefundTransaction_RefundLowerAmount() {
            var transaction = card.Charge(5.95m)
                .WithCurrency(Currency)
                .Execute(merchantConfig);
            
            Assert.IsNotNull(transaction);
            Assert.AreEqual(Success, transaction.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), transaction.ResponseMessage);

            var response = transaction.Refund(3.25m)
                .WithCurrency(Currency)
                .Execute(merchantConfig);
            
            Assert.IsNotNull(response);
            Assert.AreEqual(Success, response.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response.ResponseMessage);
        }

        [TestMethod]
        public void CreditRefundTransaction_RefundHigherAmount() {
            var transaction = card.Charge(Amount)
                .WithCurrency(Currency)
                .Execute(merchantConfig);
            
            Assert.IsNotNull(transaction);
            Assert.AreEqual(Success, transaction.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), transaction.ResponseMessage);

            var errorFound = false;
            try {
                transaction.Refund(Amount * 1.1m)
                    .WithCurrency(Currency)
                    .Execute(merchantConfig);
            }
            catch (GatewayException ex) {
                errorFound = true;
                Assert.AreEqual("40087", ex.ResponseMessage);
                Assert.AreEqual("INVALID_REQUEST_DATA", ex.ResponseCode);
                Assert.AreEqual("Status Code: BadRequest - You may only refund up to 100% of the original amount ", ex.Message);
            } finally {
                Assert.IsTrue(errorFound);
            }
        }

        [TestMethod]
        public void CreditRefundTransactionWrongId() {
            var transaction = new Transaction {
                TransactionId = Guid.NewGuid().ToString(),
                Token = Guid.NewGuid().ToString()
            };
            
            var errorFound = false;
            try {
                transaction.Refund(Amount)
                    .WithCurrency(Currency)
                    .Execute(merchantConfig);
            }
            catch (GatewayException ex) {
                errorFound = true;
                Assert.AreEqual("RESOURCE_NOT_FOUND", ex.ResponseCode);
                Assert.AreEqual("40008", ex.ResponseMessage);
                Assert.AreEqual($"Status Code: NotFound - Transaction {transaction.TransactionId} not found at this location.", ex.Message);
            } finally {
                Assert.IsTrue(errorFound);
            }
        }

        [TestMethod]
        public void CreditReverseTransaction() {
            var transaction = card.Charge(Amount)
                .WithCurrency(Currency)
                .Execute(merchantConfig);
            
            Assert.IsNotNull(transaction);
            Assert.AreEqual(Success, transaction.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), transaction.ResponseMessage);

            var response = transaction.Reverse(Amount)
                .Execute(merchantConfig);
            
            Assert.IsNotNull(response);
            Assert.AreEqual(Success, response.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Reversed), response.ResponseMessage);
        }

        [TestMethod]
        public void CreditReverseTransaction_WithIdempotencyKey() {
            var idempotencyKey = Guid.NewGuid().ToString();

            var transaction = card.Authorize(Amount)
                .WithCurrency(Currency)
                .Execute(merchantConfig);
            
            Assert.IsNotNull(transaction);
            Assert.AreEqual(Success, transaction.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Preauthorized), transaction.ResponseMessage);

            var response = transaction.Reverse(Amount)
                .WithIdempotencyKey(idempotencyKey)
                .Execute(merchantConfig);
            
            Assert.IsNotNull(response);
            Assert.AreEqual(Success, response.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Reversed), response.ResponseMessage);

            var exceptionCaught = false;
            try {
                transaction.Reverse(Amount)
                    .WithIdempotencyKey(idempotencyKey)
                    .Execute(merchantConfig);
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("DUPLICATE_ACTION", ex.ResponseCode);
                Assert.AreEqual("40039", ex.ResponseMessage);
                Assert.AreEqual($"Status Code: Conflict - Idempotency Key seen before: id={response.TransactionId}",
                    ex.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void CreditReverseTransactionWrongId() {
            var transaction = new Transaction {
                TransactionId = Guid.NewGuid().ToString(),
                Token = Guid.NewGuid().ToString()
            };
            
            var errorFound = false;
            try {
                transaction.Refund(Amount)
                    .WithCurrency(Currency)
                    .Execute(merchantConfig);
            }
            catch (GatewayException ex) {
                errorFound = true;
                Assert.AreEqual("RESOURCE_NOT_FOUND", ex.ResponseCode);
                Assert.AreEqual("40008", ex.ResponseMessage);
                Assert.AreEqual($"Status Code: NotFound - Transaction {transaction.TransactionId} not found at this location.", ex.Message);
            } finally {
                Assert.IsTrue(errorFound);
            }
        }

        [TestMethod]
        public void CreditPartialReverseTransaction() {
            var transaction = card.Charge(Amount)
                .WithCurrency(Currency)
                .Execute(merchantConfig);
            
            Assert.IsNotNull(transaction);
            Assert.AreEqual(Success, transaction.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), transaction.ResponseMessage);

            var errorFound = false;
            try {
                transaction.Reverse(1.29m)
                    .Execute(merchantConfig);
            }
            catch (GatewayException ex) {
                errorFound = true;
                Assert.AreEqual("INVALID_REQUEST_DATA", ex.ResponseCode);
                Assert.AreEqual("40214", ex.ResponseMessage);
                Assert.AreEqual("Status Code: BadRequest - partial reversal not supported", ex.Message);
            } finally {
                Assert.IsTrue(errorFound);
            }
        }

        [TestMethod]
        public void CreditAuthorizationForMultiCapture() {
            var authorization = card.Authorize(14m)
                .WithCurrency(Currency)
                .WithMultiCapture(true)
                .Execute(merchantConfig);
            
            Assert.IsNotNull(authorization);
            Assert.AreEqual(Success, authorization.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Preauthorized), authorization.ResponseMessage);

            var capture1 = authorization.Capture(3m)
                .Execute(merchantConfig);
            
            Assert.IsNotNull(capture1);
            Assert.AreEqual(Success, capture1.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), capture1.ResponseMessage);

            var capture2 = authorization.Capture(5m)
                .Execute(merchantConfig);
            
            Assert.IsNotNull(capture2);
            Assert.AreEqual(Success, capture2.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), capture2.ResponseMessage);

            var capture3 = authorization.Capture(7m)
                .Execute(merchantConfig);
            
            Assert.IsNotNull(capture3);
            Assert.AreEqual(Success, capture3.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), capture3.ResponseMessage);
        }

        [TestMethod]
        public void CreditAuthorizationAndCapture_WithIdempotencyKey() {
            var idempotencyKey = Guid.NewGuid().ToString();

            var authorization = card.Authorize(Amount)
                .WithCurrency(Currency)
                .Execute(merchantConfig);
            
            Assert.IsNotNull(authorization);
            Assert.AreEqual(Success, authorization.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Preauthorized), authorization.ResponseMessage);

            var capture = authorization.Capture(Amount)
                .WithIdempotencyKey(idempotencyKey)
                .Execute(merchantConfig);
            
            Assert.IsNotNull(capture);
            Assert.AreEqual(Success, capture.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), capture.ResponseMessage);

            var exceptionCaught = false;
            try {
                authorization.Capture(Amount)
                    .WithIdempotencyKey(idempotencyKey)
                    .Execute(merchantConfig);
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("DUPLICATE_ACTION", ex.ResponseCode);
                Assert.AreEqual("40039", ex.ResponseMessage);
                Assert.AreEqual($"Status Code: Conflict - Idempotency Key seen before: id={capture.TransactionId}",
                    ex.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void CreditCaptureWrongId() {
            var authorization = new Transaction {
                TransactionId = Guid.NewGuid().ToString(),
                Token = Guid.NewGuid().ToString()
            };

            var errorFound = false;
            try {
                authorization.Capture(Amount)
                    .Execute(merchantConfig);
            }
            catch (GatewayException ex) {
                errorFound = true;
                Assert.AreEqual("RESOURCE_NOT_FOUND", ex.ResponseCode);
                Assert.AreEqual("40008", ex.ResponseMessage);
                Assert.AreEqual($"Status Code: NotFound - Transaction {authorization.TransactionId} not found at this location.", ex.Message);
            } finally {
                Assert.IsTrue(errorFound);
            }
        }

        [TestMethod]
        public void SaleWithTokenizedPaymentMethod() {
            var tokenizedCard = new CreditCardData {
                Token = card.Tokenize()
            };

            var response = tokenizedCard.Charge(Amount)
                .WithCurrency(Currency)
                .Execute(merchantConfig);
            
            Assert.IsNotNull(response);
            Assert.AreEqual(Success, response.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response.ResponseMessage);
        }

        [TestMethod]
        public void CardTokenizationThenPayingWithToken_SingleToMultiUse()
        {
            var permissions = new[] { "PMT_POST_Create_Single" };
            var gpApiConfig = GpApiConfigSetup(AppId, AppKey, Channel.CardNotPresent);
            gpApiConfig.Permissions = permissions;

            ServicesContainer.ConfigureService(gpApiConfig, "singleUseToken");
            var token = card.Tokenize(paymentMethodUsageMode: PaymentMethodUsageMode.Single, configName:"singleUseToken");

            Assert.IsNotNull(token);

            var tokenizedCard = new CreditCardData {
                Token = token,
                CardHolderName = "James Mason"
            };

            var response = tokenizedCard.Charge(Amount)
                .WithCurrency(Currency)
                .WithRequestMultiUseToken(true)
                .Execute(merchantConfig);

            Assert.IsNotNull(response);
            Assert.AreEqual(Success, response.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response.ResponseMessage);
            Assert.IsTrue(response.Token.StartsWith("PMT_"));

            tokenizedCard.Token = response.Token;
            tokenizedCard.Charge(Amount)
                .WithCurrency(Currency)
                .Execute(merchantConfig);

            Assert.IsNotNull(response);
            Assert.AreEqual(Success, response.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response.ResponseMessage);
        }

        [TestMethod]
        public void CreditVerify() {
            var response = card.Verify()
                .WithCurrency(Currency)
                .Execute(merchantConfig);
            
            Assert.IsNotNull(response);
            Assert.AreEqual(Success, response.ResponseCode);
            Assert.AreEqual(Verified, response.ResponseMessage);
        }

        [TestMethod]
        public void CreditVerify_WithAddress() {
            var address = new Address {
                PostalCode = "750241234",
                StreetAddress1 = "6860 Dallas Pkwy",
            };

            var response = card.Verify()
                .WithCurrency(Currency)
                .WithAddress(address)
                .Execute(merchantConfig);
            
            Assert.IsNotNull(response);
            Assert.AreEqual(Success, response.ResponseCode);
            Assert.AreEqual(Verified, response.ResponseMessage);
        }

        [TestMethod]
        public void CreditVerify_WithIdempotencyKey() {
            var idempotencyKey = Guid.NewGuid().ToString();

            var response = card.Verify()
                .WithCurrency(Currency)
                .WithIdempotencyKey(idempotencyKey)
                .Execute(merchantConfig);
            
            Assert.IsNotNull(response);
            Assert.AreEqual(Success, response.ResponseCode);
            Assert.AreEqual(Verified, response.ResponseMessage);

            var exceptionCaught = false;
            try {
                card.Verify()
                    .WithCurrency(Currency)
                    .WithIdempotencyKey(idempotencyKey)
                    .Execute(merchantConfig);
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("DUPLICATE_ACTION", ex.ResponseCode);
                Assert.AreEqual("40039", ex.ResponseMessage);
                Assert.AreEqual($"Status Code: Conflict - Idempotency Key seen before: id={response.TransactionId}",
                    ex.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void CreditVerify_WithoutCurrency() {
            var errorFound = false;
            try {
                card.Verify()
                    .Execute(merchantConfig);
            }
            catch (GatewayException ex) {
                errorFound = true;
                Assert.AreEqual("40005", ex.ResponseMessage);
                Assert.AreEqual("MANDATORY_DATA_MISSING", ex.ResponseCode);
                Assert.AreEqual("Status Code: BadRequest - Request expects the following fields currency", ex.Message);
            } finally {
                Assert.IsTrue(errorFound);
            }
        }

        [TestMethod]
        public void CreditVerify_InvalidCVV() {
            card.Cvn = "1234";

            var exceptionCaught = false;
            try {
                card.Verify()
                .WithCurrency(Currency)
                .Execute(merchantConfig);
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("INVALID_REQUEST_DATA", ex.ResponseCode);
                Assert.AreEqual("40085", ex.ResponseMessage);
                Assert.AreEqual("Status Code: BadRequest - Security Code/CVV2/CVC must be 3 digits", ex.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void CreditVerify_NotNumericCVV() {
            card.Cvn = "SMA";

            var exceptionCaught = false;
            try {
                card.Verify()
                    .WithCurrency(Currency)
                    .Execute(merchantConfig);
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("SYSTEM_ERROR_DOWNSTREAM", ex.ResponseCode);
                Assert.AreEqual("50018", ex.ResponseMessage);
                Assert.AreEqual("Status Code: BadGateway - The line number 12 which contains '         [number] XXX [/number] ' does not conform to the schema", ex.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void CreditVerify_CP() {
            var gpApiConfig = GpApiConfigSetup("zKxybfLqH7vAOtBQrApxD5AUpS3ITaPz", "GAMlgEojm6hxZTLI", Channel.CardNotPresent);
            gpApiConfig.AccessTokenInfo = new AccessTokenInfo {
                TransactionProcessingAccountName = "transaction_processing",
                TokenizationAccountName = "transaction_processing"
            };
            ServicesContainer.ConfigureService(gpApiConfig);
            card.CardPresent = true;
            card.Cvn = "123";
            
            var response = card.Verify()
                .WithCurrency(Currency)
                .Execute(merchantConfig);
            
            Assert.IsNotNull(response);
            Assert.AreEqual(Success, response.ResponseCode);
            Assert.AreEqual(Verified, response.ResponseMessage);
        }

        [TestMethod]
        public void CreditSaleWithManualEntryMethod() {
            foreach (Channel channel in Enum.GetValues(typeof(Channel))) {
                foreach (ManualEntryMethod entryMethod in Enum.GetValues(typeof(ManualEntryMethod))) {
                    var gpApiConfig = GpApiConfigSetup("zKxybfLqH7vAOtBQrApxD5AUpS3ITaPz", "GAMlgEojm6hxZTLI", channel);
                    gpApiConfig.AccessTokenInfo = new AccessTokenInfo {
                        TransactionProcessingAccountName = "transaction_processing",
                        TokenizationAccountName = "transaction_processing"
                    };
                    gpApiConfig.MerchantId = merchantId;
                    ServicesContainer.ConfigureService(gpApiConfig);
                    
                    if (channel == Channel.CardPresent)
                        card.CardPresent = true;
                    card.Cvn = "123";
                    card.EntryMethod = entryMethod;

                    var response = card.Charge(Amount)
                        .WithCurrency(Currency)
                        .Execute(merchantConfig);

                    Assert.IsNotNull(response);
                    Assert.AreEqual(Success, response.ResponseCode);
                    Assert.AreEqual("CAPTURED", response.ResponseMessage);
                }
            }
        }

        [TestMethod]
        public void CreditSaleWithEntryMethod() {
            foreach (EntryMethod entryMethod in Enum.GetValues(typeof(EntryMethod))) {
                var gpApiConfig = GpApiConfigSetup("zKxybfLqH7vAOtBQrApxD5AUpS3ITaPz", "GAMlgEojm6hxZTLI", Channel.CardPresent);
                gpApiConfig.AccessTokenInfo = new AccessTokenInfo {
                    TransactionProcessingAccountName = "transaction_processing",
                    TokenizationAccountName = "transaction_processing"
                };
                gpApiConfig.MerchantId = merchantId;
                ServicesContainer.ConfigureService(gpApiConfig);

                var creditTrackData = new CreditTrackData {
                    TrackData =
                        "%B4012002000060016^VI TEST CREDIT^251210118039000000000396?;4012002000060016=25121011803939600000?",
                    EntryMethod = entryMethod
                };

                var response = creditTrackData.Charge(Amount)
                    .WithCurrency(Currency)
                    .Execute(merchantConfig);

                Assert.IsNotNull(response);
                Assert.AreEqual(Success, response.ResponseCode);
                Assert.AreEqual("CAPTURED", response.ResponseMessage);
            }
        }

        [TestMethod]
        public void CreditChargeTransactions_WithSameIdempotencyKey() {
            var idempotencyKey = Guid.NewGuid().ToString();

            var transaction1 = card.Charge(Amount)
                .WithCurrency(Currency)
                .WithIdempotencyKey(idempotencyKey)
                .Execute(merchantConfig);
            
            Assert.IsNotNull(transaction1);
            Assert.AreEqual(Success, transaction1.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), transaction1.ResponseMessage);

            var errorFound = false;
            try {
                card.Charge(Amount)
                    .WithCurrency(Currency)
                    .WithIdempotencyKey(idempotencyKey)
                    .Execute(merchantConfig);
            }
            catch (GatewayException ex) {
                errorFound = true;
                Assert.AreEqual("DUPLICATE_ACTION", ex.ResponseCode);
                Assert.AreEqual("40039", ex.ResponseMessage);
                Assert.AreEqual($"Status Code: Conflict - Idempotency Key seen before: id={transaction1.TransactionId}", ex.Message);
            } finally {
                Assert.IsTrue(errorFound);
            }
        }

        [TestMethod]
        public void CreditVerify_WithStoredCredentials() {
            var response = card.Verify()
                .WithCurrency(Currency)
                .WithStoredCredential(new StoredCredential {
                    Initiator = StoredCredentialInitiator.CardHolder,
                    Type = StoredCredentialType.Subscription,
                    Sequence = StoredCredentialSequence.First,
                    Reason = StoredCredentialReason.Incremental
                })
                .Execute(merchantConfig);
            
            Assert.IsNotNull(response);
            Assert.AreEqual(Success, response.ResponseCode);
        }

        [TestMethod]
        public void CreditSale_WithStoredCredentials() {
            var response = card.Charge(Amount)
                .WithCurrency(Currency)
                .WithStoredCredential(new StoredCredential {
                    Initiator = StoredCredentialInitiator.CardHolder,
                    Type = StoredCredentialType.Subscription,
                    Sequence = StoredCredentialSequence.First,
                    Reason = StoredCredentialReason.Incremental
                })
                .Execute(merchantConfig);
            
            Assert.IsNotNull(response);
            Assert.AreEqual(Success, response.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response.ResponseMessage);
        }

        [TestMethod]
        public void CreditSale_WithStoredCredentials_RecurringPayment() {
            var tokenizedCard = new CreditCardData {
                Token = card.Tokenize(),
            };

            var secureEcom = Secure3dService.CheckEnrollment(tokenizedCard)
                .WithCurrency(Currency)
                .WithAmount(Amount)
                .WithAuthenticationSource(AuthenticationSource.BROWSER)
                .Execute(merchantConfig);

            Assert.IsNotNull(secureEcom);
            Assert.AreEqual("ENROLLED", secureEcom.Enrolled, "Card not enrolled");
            Assert.AreEqual(Secure3dVersion.Two, secureEcom.Version);
            Assert.AreEqual("AVAILABLE", secureEcom.Status);
            Assert.IsNotNull(secureEcom.IssuerAcsUrl);
            Assert.IsNotNull(secureEcom.PayerAuthenticationRequest);
            Assert.IsNotNull(secureEcom.ChallengeReturnUrl);
            Assert.IsNotNull(secureEcom.MessageType);
            Assert.IsNotNull(secureEcom.SessionDataFieldName);
            Assert.IsNull(secureEcom.Eci);

            var initAuth = Secure3dService.InitiateAuthentication(tokenizedCard, secureEcom)
                .WithAmount(Amount)
                .WithCurrency(Currency)
                .WithAuthenticationSource(AuthenticationSource.BROWSER)
                .WithBrowserData(new BrowserData {
                    AcceptHeader = "text/html,application/xhtml+xml,application/xml;q=9,image/webp,img/apng,*/*;q=0.8",
                    ColorDepth = ColorDepth.TWENTY_FOUR_BITS,
                    IpAddress = "123.123.123.123",
                    JavaEnabled = true,
                    Language = "en",
                    ScreenHeight = 1080,
                    ScreenWidth = 1920,
                    ChallengeWindowSize = ChallengeWindowSize.WINDOWED_600X400,
                    Timezone = "0",
                    UserAgent =
                    "Mozilla/5.0 (Windows NT 6.1; Win64, x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.110 Safari/537.36"
                })
                .WithMethodUrlCompletion(MethodUrlCompletion.YES)
                .WithOrderCreateDate(DateTime.Now)
                .Execute(merchantConfig);

            Assert.IsNotNull(initAuth);
            Assert.AreEqual("ENROLLED", initAuth.Enrolled, "Card not enrolled");
            Assert.AreEqual(Secure3dVersion.Two, initAuth.Version);
            Assert.AreEqual("SUCCESS_AUTHENTICATED", initAuth.Status);
            Assert.IsNotNull(initAuth.IssuerAcsUrl);
            Assert.IsNotNull(initAuth.PayerAuthenticationRequest);
            Assert.IsNotNull(initAuth.ChallengeReturnUrl);
            Assert.IsNotNull(initAuth.MessageType);
            Assert.IsNotNull(initAuth.SessionDataFieldName);

            tokenizedCard.ThreeDSecure = initAuth;

            var response = tokenizedCard.Charge(Amount)
                .WithCurrency(Currency)
                .Execute(merchantConfig);

            Assert.IsNotNull(response);
            Assert.AreEqual(Success, response.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response.ResponseMessage);
            Assert.IsNotNull(response.CardBrandTransactionId);

            var response2 = tokenizedCard.Charge(Amount)
                .WithCurrency(Currency)
                .WithStoredCredential(new StoredCredential {
                    Initiator = StoredCredentialInitiator.Merchant,
                    Type = StoredCredentialType.Recurring,
                    Sequence = StoredCredentialSequence.Subsequent,
                    Reason = StoredCredentialReason.Incremental
                })
                .WithCardBrandStorage(StoredCredentialInitiator.Merchant, response.CardBrandTransactionId)
                .Execute(merchantConfig);

            Assert.IsNotNull(response2);
            Assert.AreEqual(Success, response2.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response2.ResponseMessage);
        }
    }
}
