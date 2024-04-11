using System;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Utils.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static GlobalPayments.Api.Tests.GpApi.GpApiAvsCheckTestCards;

namespace GlobalPayments.Api.Tests.GpApi {
    [TestClass]
    public class GpApiCreditCardNotPresentTest : BaseGpApiTests {
        private CreditCardData card;
        private const decimal AMOUNT = 7.8m;
        private const string CURRENCY = "GBP";

        [TestInitialize]
        public void TestInitialize() {
            ServicesContainer.RemoveConfig();
            var gpApiConfig = GpApiConfigSetup(AppId, AppKey, Channel.CardNotPresent);
            ServicesContainer.ConfigureService(gpApiConfig);

            card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = ExpMonth,
                ExpYear = ExpYear,
                Cvn = "123",
                CardPresent = true
            };
        }

        [TestMethod]
        public void CreditAuthorization() {
            var transaction = card.Authorize(AMOUNT)
                .WithCurrency(CURRENCY)
                .Execute();
            AssertTransactionResponse(transaction, TransactionStatus.Preauthorized);

            var capture = transaction.Capture(AMOUNT + 1m)
                .WithGratuity(1m)
                .Execute();
            AssertTransactionResponse(capture, TransactionStatus.Captured);
        }

        [TestMethod]
        public void CreditSaleWithFingerPrint() {
            var address = new Address {
                StreetAddress1 = "123 Main St.",
                City = "Downtown",
                State = "NJ",
                Country = "US",
                PostalCode = "12345"
            };
            var customer = new Customer {
                DeviceFingerPrint = "ALWAYS"
            };

            var response = card.Charge(69)
                .WithCurrency("GBP")
                .WithAddress(address)
                .WithCustomerData(customer)
                .Execute();
            
            Assert.IsNotNull(response);
            Assert.AreEqual(Success, response.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response.ResponseMessage);
            Assert.IsNotNull(response.FingerPrint);
            Assert.IsNotNull(response.FingerPrintIndicator);           
            Assert.AreEqual("EXISTS",response.FingerPrintIndicator);           
        }

        [TestMethod]
        public void VerifyTokenizedPaymentMethodWithFingerprint() {
            var customer = new Customer {
                DeviceFingerPrint = "ALWAYS"
            };
            var tokenizedCard = new CreditCardData {
                Token = card.Tokenize(),
            };

            var response = tokenizedCard.Verify()
                .WithCurrency("GBP")
                .WithCustomerData(customer)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual(Success, response.ResponseCode);
            Assert.AreEqual("VERIFIED", response.ResponseMessage);
            Assert.IsNotNull(response.FingerPrint);           
        }

        [TestMethod]
        public void UpdatePaymentToken() {
            var response = ReportingService.FindStoredPaymentMethodsPaged(1, 1)
                .OrderBy(StoredPaymentMethodSortProperty.TimeCreated, SortDirection.Descending)
                .Where(SearchCriteria.StartDate, StartDate)
                .And(SearchCriteria.EndDate, EndDate)
                .Execute();

            Assert.AreEqual(1, response.Results.Count);
            var pmtToken = response.Results[0].Id;
            Assert.IsNotNull(pmtToken);            
            var tokenizedCard = new CreditCardData {
                Token = pmtToken,
                CardHolderName = "James BondUp",
                ExpYear = ExpYear,
                ExpMonth = ExpMonth,
                Number = "4263970000005262"
            };

            var updateTokenResponse = tokenizedCard.UpdateToken()
                .WithPaymentMethodUsageMode(PaymentMethodUsageMode.Multiple)
                .Execute();

            Assert.AreEqual("SUCCESS", updateTokenResponse.ResponseCode);
            Assert.AreEqual("ACTIVE", updateTokenResponse.ResponseMessage);
            Assert.AreEqual(pmtToken, updateTokenResponse.Token);
            Assert.AreEqual(PaymentMethodUsageMode.Multiple, updateTokenResponse.TokenUsageMode);
        }
        
        [TestMethod]
        public void UpdatePaymentToken_UsageModeOnly()
        {
            var response = ReportingService.FindStoredPaymentMethodsPaged(1, 1)
                .OrderBy(StoredPaymentMethodSortProperty.TimeCreated, SortDirection.Descending)
                .Where(SearchCriteria.StartDate, StartDate)
                .And(SearchCriteria.EndDate, EndDate)
                .Execute();

            Assert.AreEqual(1, response.Results.Count);
            var pmtToken = response.Results[0].Id;
            Assert.IsNotNull(pmtToken);            
            var tokenizedCard = new CreditCardData {
                Token = pmtToken,
                CardHolderName = "James BondUp",
                Number = "4263970000005262"
            };

            var updateTokenResponse = tokenizedCard.UpdateToken()
                .WithPaymentMethodUsageMode(PaymentMethodUsageMode.Multiple)
                .Execute();

            Assert.AreEqual("SUCCESS", updateTokenResponse.ResponseCode);
            Assert.AreEqual("ACTIVE", updateTokenResponse.ResponseMessage);
            Assert.AreEqual(pmtToken, updateTokenResponse.Token);
            Assert.AreEqual(PaymentMethodUsageMode.Multiple, updateTokenResponse.TokenUsageMode);
        }

        [TestMethod]
        public void CardTokenizationThenUpdateAndThenCharge()
        {
            var tokenId = card.Tokenize();
        
            card.Token = tokenId;
            card.CardHolderName = "GpApi";

            var responseUpdateToken = card.UpdateToken()
                .WithPaymentMethodUsageMode(PaymentMethodUsageMode.Multiple)
                .Execute();
            Assert.IsNotNull(responseUpdateToken);
            Assert.AreEqual("SUCCESS", responseUpdateToken.ResponseCode);
            Assert.AreEqual("ACTIVE", responseUpdateToken.ResponseMessage);
            Assert.AreEqual(PaymentMethodUsageMode.Multiple, responseUpdateToken.TokenUsageMode);

            var chargeResponse = card.Charge(1)
                .WithCurrency(CURRENCY)
                .Execute();
            Assert.IsNotNull(chargeResponse);
            Assert.AreEqual("SUCCESS", chargeResponse.ResponseCode);
            Assert.AreEqual(TransactionStatus.Captured.ToString().ToUpper(), chargeResponse.ResponseMessage);
        }

        [TestMethod]
        public void CardTokenizationThenUpdateToSingleUsage() {
            var tokenizedCard = new CreditCardData {
                Token = $"PMT_{Guid.NewGuid()}"
            };

            var exceptionCaught = false;
            try {
                tokenizedCard.UpdateToken()
                    .WithPaymentMethodUsageMode(PaymentMethodUsageMode.Single)
                    .Execute();
            } catch (GatewayException e) {
                exceptionCaught = true;
                Assert.AreEqual("50020", e.ResponseMessage);
                Assert.AreEqual("Status Code: BadRequest - Tokentype can only be MULTI", e.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void CardTokenizationThenUpdateWithoutUsageMode() {
            var tokenizedCard = new CreditCardData {
                Token = $"PMT_{Guid.NewGuid()}"
            };

            var exceptionCaught = false;
            try {
                tokenizedCard.UpdateToken()
                    .Execute();
            } catch (GatewayException e) {
                exceptionCaught = true;
                Assert.AreEqual("50021", e.ResponseMessage);
                Assert.AreEqual("Status Code: BadRequest - Mandatory Fields missing [card expdate] See Developers Guide", e.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void CardTokenizationWithSomeCardInfoThenUpdateWithoutUsageMode() {
            var tokenizedCard = new CreditCardData {
                ExpMonth = ExpMonth,
                ExpYear = ExpYear,
                Token = $"PMT_{Guid.NewGuid()}"
            };
            var exceptionCaught = false;
            try {
                tokenizedCard.UpdateToken()
                    .Execute();
            } catch (GatewayException e) {
                exceptionCaught = true;
                Assert.AreEqual("40116", e.ResponseMessage);
                Assert.AreEqual($"Status Code: NotFound - payment_method {tokenizedCard.Token} not found at this location.", e.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void CreditSaleWithFingerPrint_OnSuccess() {
            var customer = new Customer {
                DeviceFingerPrint = "ON_SUCCESS"
            };

            var response = card.Charge(2)
                .WithCurrency("GBP")
                .WithCustomerData(customer)
                .Execute();
            
            Assert.IsNotNull(response);
            Assert.AreEqual(Success, response.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response.ResponseMessage);
            Assert.IsNotNull(response.FingerPrint);
            Assert.IsNotNull(response.FingerPrintIndicator);           
            Assert.AreEqual("EXISTS",response.FingerPrintIndicator);           
        }
        
        [TestMethod]
        public void CreditSaleWithFingerPrint_OnSuccess_WithDeclinedAuth() {
            card.Number = "4000120000001154";
            var customer = new Customer {
                DeviceFingerPrint = "ON_SUCCESS"
            };

            var response = card.Charge(2)
                .WithCurrency("GBP")
                .WithCustomerData(customer)
                .Execute();
            
            Assert.IsNotNull(response);
            Assert.AreEqual(Declined, response.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Declined), response.ResponseMessage);
            Assert.AreEqual("",response.FingerPrint);           
            Assert.AreEqual("",response.FingerPrintIndicator);           
        }
        
        [TestMethod]
        public void CreditAuthorizationWithPaymentLinkId() {
            var transaction = card.Authorize(14m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .WithPaymentLinkId("LNK_W1xgWehivDP8P779cFDDTZwzL01EEw4")
                .Execute();
            
            AssertTransactionResponse(transaction, TransactionStatus.Preauthorized);
        }

        [TestMethod]
        public void CreditAuthorization_CaptureLowerAmount() {
            var transaction = card.Authorize(5m)
                .WithCurrency(CURRENCY)
                .Execute();
            AssertTransactionResponse(transaction, TransactionStatus.Preauthorized);

            var capture = transaction.Capture(2.99m)
                .WithGratuity(2m)
                .Execute();
            
            AssertTransactionResponse(capture, TransactionStatus.Captured);
        }

        [TestMethod]
        public void CreditAuthorization_CaptureHigherAmount() {
            var transaction = card.Authorize(AMOUNT)
                .WithCurrency(CURRENCY)
                .Execute();
            AssertTransactionResponse(transaction, TransactionStatus.Preauthorized);

            var capture = transaction.Capture(AMOUNT * 1.15m)
                .WithGratuity(2m)
                .Execute();
            
            AssertTransactionResponse(capture, TransactionStatus.Captured);
        }

        [TestMethod]
        public void CreditAuthorization_CaptureHigherAmount_WithError() {
            var transaction = card.Authorize(AMOUNT)
                .WithCurrency(CURRENCY)
                .Execute();
            AssertTransactionResponse(transaction, TransactionStatus.Preauthorized);

            var exceptionCaught = false;
            try {
                exceptionCaught = true;
                transaction.Capture(AMOUNT * 1.16m)
                    .WithGratuity(2m)
                    .Execute();
            } catch (GatewayException ex) {
                Assert.AreEqual("50020", ex.ResponseMessage);
                Assert.AreEqual("INVALID_REQUEST_DATA", ex.ResponseCode);
                Assert.AreEqual(
                    "Status Code: BadRequest - Can't settle for more than 115% of that which you authorised ",
                    ex.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
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
            var response = card.Charge(AMOUNT)
                .WithCurrency(CURRENCY)
                .WithAddress(address)
                .Execute();
            AssertTransactionResponse(response, TransactionStatus.Captured);
            Assert.AreEqual(AMOUNT, response.BalanceAmount);
            Assert.IsNull(response.PayerDetails);
        }

        [TestMethod]
        public void CreditSale_WithRequestMultiUseToken() {
            var response = card.Charge(AMOUNT)
                .WithCurrency(CURRENCY)
                .WithRequestMultiUseToken(true)
                .Execute();
            AssertTransactionResponse(response, TransactionStatus.Captured);
            Assert.IsNotNull(response.Token);
            Assert.IsTrue(response.Token.StartsWith("PMT_"));
        }

        [TestMethod]
        public void CreditRefund() {
            var response = card.Refund(AMOUNT)
                .WithCurrency(CURRENCY)
                .Execute();
            AssertTransactionResponse(response, TransactionStatus.Captured);
        }

        [TestMethod]
        public void CreditRefundTransaction() {
            var transaction = card.Charge(AMOUNT)
                .WithCurrency(CURRENCY)
                .Execute();
            AssertTransactionResponse(transaction, TransactionStatus.Captured);

            var response = transaction.Refund(AMOUNT)
                .WithCurrency(CURRENCY)
                .Execute();
            AssertTransactionResponse(response, TransactionStatus.Captured);
        }

        [TestMethod]
        public void CreditVerificationWithStoredCredentials()
        {
           var storeCredentials = new StoredCredential();
            storeCredentials.Initiator = StoredCredentialInitiator.Merchant;
            storeCredentials.Type = StoredCredentialType.Installment;
            storeCredentials.Sequence = StoredCredentialSequence.Subsequent;
            storeCredentials.Reason = StoredCredentialReason.Incremental;

            var response = card.Verify()
                .WithCurrency(CURRENCY)
                .WithStoredCredential(storeCredentials)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("SUCCESS", response.ResponseCode);
            Assert.AreEqual("VERIFIED", response.ResponseMessage);
        }

        [TestMethod]
        public void CreditRefundTransaction_WithIdempotencyKey() {
            var transaction = card.Charge(AMOUNT)
                .WithCurrency(CURRENCY)
                .Execute();
            AssertTransactionResponse(transaction, TransactionStatus.Captured);

            var idempotencyKey = Guid.NewGuid().ToString();
            var response = transaction.Refund(AMOUNT)
                .WithCurrency(CURRENCY)
                .WithIdempotencyKey(idempotencyKey)
                .Execute();
            AssertTransactionResponse(response, TransactionStatus.Captured);

            var exceptionCaught = false;
            try {
                transaction.Refund(AMOUNT)
                    .WithCurrency(CURRENCY)
                    .WithIdempotencyKey(idempotencyKey)
                    .Execute();
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
            var transaction = card.Charge(AMOUNT)
                .WithCurrency(CURRENCY)
                .Execute();
            AssertTransactionResponse(transaction, TransactionStatus.Captured);

            var response = transaction.Refund(AMOUNT - 2m)
                .WithCurrency(CURRENCY)
                .Execute();
            AssertTransactionResponse(response, TransactionStatus.Captured);
            Assert.AreEqual(AMOUNT - 2m, response.BalanceAmount);
        }

        [TestMethod]
        public void CreditRefundTransaction_RefundHigherAmount() {
            const decimal refundAmount = AMOUNT * 1.1m;

            var transaction = card.Charge(AMOUNT)
                .WithCurrency(CURRENCY)
                .Execute();
            AssertTransactionResponse(transaction, TransactionStatus.Captured);

            var response = transaction.Refund(refundAmount)
                .WithCurrency(CURRENCY)
                .Execute();
            AssertTransactionResponse(response, TransactionStatus.Captured);
            Assert.AreEqual(decimal.Round(refundAmount, 2, MidpointRounding.AwayFromZero), response.BalanceAmount);
        }

        [TestMethod]
        public void CreditRefundTransaction_RefundHigherAmountThen115() {
            var transaction = card.Charge(AMOUNT)
                .WithCurrency(CURRENCY)
                .Execute();
            AssertTransactionResponse(transaction, TransactionStatus.Captured);

            var exceptionCaught = false;
            try {
                transaction.Refund(AMOUNT * 1.5m)
                    .WithCurrency(CURRENCY)
                    .Execute();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("40087", ex.ResponseMessage);
                Assert.AreEqual("INVALID_REQUEST_DATA", ex.ResponseCode);
                Assert.AreEqual("Status Code: BadRequest - You may only refund up to 115% of the original amount ",
                    ex.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void CreditRefundTransactionWrongId() {
            var transaction = new Transaction {
                TransactionId = "TRN_" + Guid.NewGuid(),
            };

            var exceptionCaught = false;
            try {
                transaction.Refund(AMOUNT)
                    .WithCurrency(CURRENCY)
                    .Execute();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("RESOURCE_NOT_FOUND", ex.ResponseCode);
                Assert.AreEqual("40008", ex.ResponseMessage);
                Assert.AreEqual($"Status Code: NotFound - Transaction {transaction.TransactionId} not found at this location.", ex.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void CreditReverseTransaction() {
            var transaction = card.Charge(AMOUNT)
                .WithCurrency(CURRENCY)
                .Execute();
            AssertTransactionResponse(transaction, TransactionStatus.Captured);

            var response = transaction.Reverse(AMOUNT)
                .Execute();
            AssertTransactionResponse(response, TransactionStatus.Reversed);
        }
      
        [TestMethod]
        public void CreditReverseTransaction_WithIdempotencyKey() {
            var transaction = card.Authorize(AMOUNT)
                .WithCurrency(CURRENCY)
                .Execute();
            AssertTransactionResponse(transaction, TransactionStatus.Preauthorized);

            var idempotencyKey = Guid.NewGuid().ToString();
            var response = transaction.Reverse(AMOUNT)
                .WithIdempotencyKey(idempotencyKey)
                .Execute();
            AssertTransactionResponse(response, TransactionStatus.Reversed);

            var exceptionCaught = false;
            try {
                transaction.Reverse(AMOUNT)
                    .WithIdempotencyKey(idempotencyKey)
                    .Execute();
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
        public void CreditSale_WithDynamicDescriptor()
        {
            var dynamicDescriptor = "My company";
            var response = card.Charge(50)
                .WithCurrency("EUR")
                .WithDynamicDescriptor(dynamicDescriptor)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("SUCCESS", response.ResponseCode);
            Assert.AreEqual(TransactionStatus.Captured.ToString().ToUpper(), response.ResponseMessage.ToUpper());
        }

        [TestMethod]
        public void CreditReverseTransactionWrongId() {
            var transaction = new Transaction {
                TransactionId = "TRN_" + Guid.NewGuid()
            };

            var exceptionCaught = false;
            try {
                transaction.Refund(AMOUNT)
                    .WithCurrency(CURRENCY)
                    .WithAllowDuplicates(true)
                    .Execute();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("RESOURCE_NOT_FOUND", ex.ResponseCode);
                Assert.AreEqual("40008", ex.ResponseMessage);
                Assert.AreEqual($"Status Code: NotFound - Transaction {transaction.TransactionId} not found at this location.", ex.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void CreditPartialReverseTransaction() {
            var transaction = card.Charge(3.99m)
                .WithCurrency(CURRENCY)
                .Execute();
            AssertTransactionResponse(transaction, TransactionStatus.Captured);

            var exceptionCaught = false;
            try {
                transaction.Reverse(1.29m).Execute();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("INVALID_REQUEST_DATA", ex.ResponseCode);
                Assert.AreEqual("40214", ex.ResponseMessage);
                Assert.AreEqual("Status Code: BadRequest - partial reversal not supported", ex.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void CreditAuthorizationForMultiCapture() {
            var authorization = card.Authorize(14m)
                .WithCurrency("USD")
                .WithMultiCapture(true)
                .Execute();
            AssertTransactionResponse(authorization, TransactionStatus.Preauthorized);
            Assert.IsTrue(authorization.MultiCapture);

            var capture1 = authorization.Capture(3m)
                .Execute();
            AssertTransactionResponse(capture1, TransactionStatus.Captured);

            var capture2 = authorization.Capture(5m)
                .Execute();
            AssertTransactionResponse(capture2, TransactionStatus.Captured);

            var capture3 = authorization.Capture(7m)
                .Execute();
            AssertTransactionResponse(capture3, TransactionStatus.Captured);
        }

        [TestMethod]
        public void CreditAuthorizationAndCapture_WithIdempotencyKey() {
            var authorization = card.Authorize(AMOUNT)
                .WithCurrency(CURRENCY)
                .Execute();
            AssertTransactionResponse(authorization, TransactionStatus.Preauthorized);

            var idempotencyKey = Guid.NewGuid().ToString();
            var capture = authorization.Capture(AMOUNT)
                .WithIdempotencyKey(idempotencyKey)
                .Execute();
            AssertTransactionResponse(capture, TransactionStatus.Captured);

            var exceptionCaught = false;
            try {
                authorization.Capture(14m)
                    .WithIdempotencyKey(idempotencyKey)
                    .Execute();
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
                TransactionId = "TRN_" + Guid.NewGuid()
            };

            var exceptionCaught = false;
            try {
                authorization.Capture(AMOUNT)
                    .Execute();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("RESOURCE_NOT_FOUND", ex.ResponseCode);
                Assert.AreEqual("40008", ex.ResponseMessage);
                Assert.AreEqual(
                    $"Status Code: NotFound - Transaction {authorization.TransactionId} not found at this location.", ex.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void SaleWithTokenizedPaymentMethod() {
            var tokenizedCard = new CreditCardData {
                Token = card.Tokenize(),
            };

            var response = tokenizedCard.Charge(AMOUNT)
                .WithCurrency(CURRENCY)
                .Execute();
            AssertTransactionResponse(response, TransactionStatus.Captured);
        }

        [TestMethod]
        public void CardTokenizationThenPayingWithToken_SingleToMultiUse() {
            var permissions = new[] { "PMT_POST_Create_Single" };
            var gpApiConfig = new GpApiConfig {
                AppId = AppId,
                AppKey = AppKey,
                RequestLogger = new RequestConsoleLogger(),
                Permissions = permissions
            };

            ServicesContainer.ConfigureService(gpApiConfig, "singleUseToken");
            
            var token = card.Tokenize(paymentMethodUsageMode: PaymentMethodUsageMode.Single, configName:"singleUseToken");
            Assert.IsNotNull(token);

            var tokenizedCard = new CreditCardData {
                Token = token,
                CardHolderName = "James Mason"
            };

            var response = tokenizedCard.Charge(AMOUNT)
                .WithCurrency(CURRENCY)
                .WithRequestMultiUseToken(true)
                .Execute();
            AssertTransactionResponse(response, TransactionStatus.Captured);
            Assert.IsTrue(response.Token.StartsWith("PMT_"));

            tokenizedCard.Token = response.Token;
            var charge = tokenizedCard.Charge(AMOUNT)
                .WithCurrency(CURRENCY)
                .Execute();
            AssertTransactionResponse(charge, TransactionStatus.Captured);
        }

        [TestMethod]
        public void CreditVerify() {
            var response = card.Verify()
                .WithCurrency(CURRENCY)
                .Execute();
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
                .WithCurrency(CURRENCY)
                .WithAddress(address)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual(Success, response.ResponseCode);
            Assert.AreEqual(Verified, response.ResponseMessage);
        }

        [TestMethod]
        public void CreditVerify_WithIdempotencyKey() {
            var idempotencyKey = Guid.NewGuid().ToString();

            var response = card.Verify()
                .WithCurrency(CURRENCY)
                .WithIdempotencyKey(idempotencyKey)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual(Success, response.ResponseCode);
            Assert.AreEqual(Verified, response.ResponseMessage);

            var exceptionCaught = false;
            try {
                card.Verify()
                    .WithCurrency(CURRENCY)
                    .WithIdempotencyKey(idempotencyKey)
                    .Execute();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("DUPLICATE_ACTION", ex.ResponseCode);
                Assert.AreEqual("40039", ex.ResponseMessage);
                Assert.AreEqual($"Status Code: Conflict - Idempotency Key seen before: id={response.TransactionId}",
                    ex.Message);
            }
            finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void CreditVerify_WithoutCurrency() {
            var exceptionCaught = false;
            try {
                card.Verify()
                    .Execute();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("40005", ex.ResponseMessage);
                Assert.AreEqual("MANDATORY_DATA_MISSING", ex.ResponseCode);
                Assert.AreEqual("Status Code: BadRequest - Request expects the following fields currency", ex.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void CreditVerify_InvalidCVV() {
            card.Cvn = "1234";

            var exceptionCaught = false;
            try {
                card.Verify()
                .WithCurrency(CURRENCY)
                .Execute();
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
                    .WithCurrency(CURRENCY)
                    .Execute();
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
        public void CreditSaleWithManualEntryMethod()
        {
            foreach (Channel channel in Enum.GetValues(typeof(Channel))) {
                foreach (ManualEntryMethod entryMethod in Enum.GetValues(typeof(ManualEntryMethod)))
                {
                    var gpApiConfig = GpApiConfigSetup(AppId, AppKey, channel);
                    ServicesContainer.ConfigureService(gpApiConfig);
                    
                    card.Cvn = "123";
                    card.EntryMethod = entryMethod;

                    var response = card.Charge(AMOUNT)
                        .WithCurrency(CURRENCY)
                        .Execute();

                    AssertTransactionResponse(response, TransactionStatus.Captured);
                }
            }
        }

        [TestMethod]
        public void CreditSaleWithEntryMethod() {
            var gpApiConfig = GpApiConfigSetup(AppId, AppKey, Channel.CardPresent);
            ServicesContainer.ConfigureService(gpApiConfig);
            
            foreach (EntryMethod entryMethod in Enum.GetValues(typeof(EntryMethod))) {
                var creditTrackData = new CreditTrackData {
                    TrackData =
                        "%B4012002000060016^VI TEST CREDIT^251210118039000000000396?;4012002000060016=25121011803939600000?",
                    EntryMethod = entryMethod
                };

                var response = creditTrackData.Charge(AMOUNT)
                    .WithCurrency(CURRENCY)
                    .Execute();
                AssertTransactionResponse(response, TransactionStatus.Captured);
            }
        }

        [TestMethod]
        public void CreditChargeTransactions_WithSameIdempotencyKey() {
            var idempotencyKey = Guid.NewGuid().ToString();

            var transaction1 = card.Charge(AMOUNT)
                .WithCurrency(CURRENCY)
                .WithIdempotencyKey(idempotencyKey)
                .Execute();
            AssertTransactionResponse(transaction1, TransactionStatus.Captured);

            var exceptionCaught = false;
            try {
                card.Charge(AMOUNT)
                    .WithCurrency(CURRENCY)
                    .WithIdempotencyKey(idempotencyKey)
                    .Execute();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("DUPLICATE_ACTION", ex.ResponseCode);
                Assert.AreEqual("40039", ex.ResponseMessage);
                Assert.AreEqual($"Status Code: Conflict - Idempotency Key seen before: id={transaction1.TransactionId}", ex.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void CreditVerify_WithStoredCredentials() {
            var response = card.Verify()
                .WithCurrency(CURRENCY)
                .WithStoredCredential(new StoredCredential {
                    Initiator = StoredCredentialInitiator.CardHolder,
                    Type = StoredCredentialType.Subscription,
                    Sequence = StoredCredentialSequence.First,
                    Reason = StoredCredentialReason.Incremental
                })
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual(Success, response.ResponseCode);
            Assert.AreEqual(Verified, response.ResponseMessage);
        }

        [TestMethod]
        public void CreditSale_WithStoredCredentials() {
            var response = card.Charge(AMOUNT)
                .WithCurrency(CURRENCY)
                .WithStoredCredential(new StoredCredential {
                    Initiator = StoredCredentialInitiator.CardHolder,
                    Type = StoredCredentialType.Subscription,
                    Sequence = StoredCredentialSequence.First,
                    Reason = StoredCredentialReason.Incremental
                })
                .Execute();
            AssertTransactionResponse(response, TransactionStatus.Captured);
        }

        [TestMethod]
        public void CreditSale_WithCardBrandStorage_RecurringPayment() {
            var tokenizedCard = new CreditCardData {
                Token = card.Tokenize(),
            };

            Transaction response = tokenizedCard.Charge(10.01m)
                        .WithCurrency("GBP")
                        .Execute();

            AssertTransactionResponse(response, TransactionStatus.Captured);

            var response2 = tokenizedCard.Charge(10.01m)
                .WithCurrency(CURRENCY)
                .WithStoredCredential(new StoredCredential
                {
                    Initiator = StoredCredentialInitiator.Merchant,
                    Type = StoredCredentialType.Recurring,
                    Sequence = StoredCredentialSequence.Subsequent,
                    Reason = StoredCredentialReason.Incremental
                })
                .WithCardBrandStorage(StoredCredentialInitiator.Merchant, response.CardBrandTransactionId)
                .Execute();

            AssertTransactionResponse(response2, TransactionStatus.Captured);
            Assert.IsNotNull(response2.CardBrandTransactionId);
        }

        [TestMethod]
        public void CreditSale_WithStoredCredentials_RecurringPayment() {
            var tokenizedCard = new CreditCardData {
                Token = card.Tokenize(),
            };

            var secureEcom = Secure3dService.CheckEnrollment(tokenizedCard)
                .WithCurrency(CURRENCY)
                .WithAmount(AMOUNT)
                .WithAuthenticationSource(AuthenticationSource.BROWSER)
                .Execute();

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
                .WithAmount(AMOUNT)
                .WithCurrency(CURRENCY)
                .WithAuthenticationSource(AuthenticationSource.BROWSER)
                .WithBrowserData(new BrowserData
                {
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
                .Execute();

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

            var response = tokenizedCard.Charge(AMOUNT)
                .WithCurrency(CURRENCY)
                .Execute();
            AssertTransactionResponse(response, TransactionStatus.Captured);
            Assert.IsNotNull(response.CardBrandTransactionId);

            var response2 = tokenizedCard.Charge(AMOUNT)
                .WithCurrency(CURRENCY)
                .WithStoredCredential(new StoredCredential
                {
                    Initiator = StoredCredentialInitiator.Merchant,
                    Type = StoredCredentialType.Recurring,
                    Sequence = StoredCredentialSequence.Subsequent,
                    Reason = StoredCredentialReason.Incremental
                })
                .WithCardBrandStorage(StoredCredentialInitiator.Merchant, response.CardBrandTransactionId)
                .Execute();

            AssertTransactionResponse(response2, TransactionStatus.Captured);
        }
        
        [TestMethod]
        public void CreditSale_ExpiryCard() {
            card.ExpYear = DateTime.Now.Year - 1;
            var exceptionCaught = false;
            try {
                card.Charge(1)
                    .WithCurrency("USD")
                    .Execute();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("INVALID_REQUEST_DATA", ex.ResponseCode);
                Assert.AreEqual("40085", ex.ResponseMessage);
                Assert.AreEqual("Status Code: BadRequest - Expiry date invalid", ex.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [DataTestMethod]
        [DataRow(AVS_MASTERCARD_1, "MATCHED", "NOT_CHECKED", "NOT_CHECKED", Success, TransactionStatus.Captured, "M", "U", "U")]
        [DataRow(AVS_MASTERCARD_2, "MATCHED", "NOT_CHECKED", "NOT_CHECKED", Success, TransactionStatus.Captured, "M", "I", "I")]
        [DataRow(AVS_MASTERCARD_3, "MATCHED", "NOT_CHECKED", "NOT_CHECKED", Success, TransactionStatus.Captured, "M", "P", "P")]
        [DataRow(AVS_MASTERCARD_4, "MATCHED", "MATCHED", "MATCHED", Success, TransactionStatus.Captured, "M", "M", "M")]
        [DataRow(AVS_MASTERCARD_5, "MATCHED", "NOT_MATCHED", "NOT_MATCHED", Success, TransactionStatus.Captured, "M", "N", "N")]
        [DataRow(AVS_MASTERCARD_6, "MATCHED", "NOT_MATCHED", "MATCHED", Success, TransactionStatus.Captured, "M", "N", "M")]
        [DataRow(AVS_MASTERCARD_7, "MATCHED", "NOT_MATCHED", "NOT_MATCHED", Success, TransactionStatus.Captured, "M", "N", "N")]
        [DataRow(AVS_MASTERCARD_8, "NOT_MATCHED", "NOT_MATCHED", "MATCHED", Success, TransactionStatus.Captured, "N", "N", "M")]
        [DataRow(AVS_MASTERCARD_9, "NOT_MATCHED", "NOT_CHECKED", "NOT_CHECKED", Success, TransactionStatus.Captured, "N", "U", "U")]
        [DataRow(AVS_MASTERCARD_10, "NOT_MATCHED", "NOT_CHECKED", "NOT_CHECKED", Success, TransactionStatus.Captured, "N", "I", "I")]
        [DataRow(AVS_MASTERCARD_11, "NOT_MATCHED", "NOT_CHECKED", "NOT_CHECKED", Success, TransactionStatus.Captured, "N", "P", "P")]
        [DataRow(AVS_MASTERCARD_12, "NOT_MATCHED", "NOT_CHECKED", "MATCHED", Success, TransactionStatus.Captured, "N", "P", "M")]
        [DataRow(AVS_MASTERCARD_13, "NOT_MATCHED", "MATCHED", "NOT_MATCHED", Success, TransactionStatus.Captured, "N", "M", "N")]
        [DataRow(AVS_MASTERCARD_14, "NOT_MATCHED", "NOT_MATCHED", "NOT_MATCHED", Success, TransactionStatus.Captured, "N", "N", "N")]
        [DataRow(AVS_VISA_1, "NOT_CHECKED", "NOT_CHECKED", "NOT_CHECKED", Declined, TransactionStatus.Declined, "I", "U", "U")]
        [DataRow(AVS_VISA_2, "NOT_CHECKED", "NOT_CHECKED", "NOT_CHECKED", Declined, TransactionStatus.Declined, "I", "I", "I")]
        [DataRow(AVS_VISA_3, "NOT_CHECKED", "NOT_CHECKED", "NOT_CHECKED", Declined, TransactionStatus.Declined, "I", "P", "P")]
        [DataRow(AVS_VISA_4, "MATCHED", "MATCHED", "MATCHED", Declined, TransactionStatus.Declined, "M", "M", "M")]
        [DataRow(AVS_VISA_5, "NOT_CHECKED", "MATCHED", "NOT_MATCHED", Declined, TransactionStatus.Declined, "I", "M", "N")]
        [DataRow(AVS_VISA_6, "NOT_CHECKED", "NOT_MATCHED", "MATCHED", Declined, TransactionStatus.Declined, "I", "N", "M")]
        [DataRow(AVS_VISA_7, "NOT_CHECKED", "NOT_MATCHED", "NOT_MATCHED", Declined, TransactionStatus.Declined, "I", "N", "N")]
        [DataRow(AVS_VISA_8, "NOT_CHECKED", "NOT_CHECKED", "NOT_CHECKED", Declined, TransactionStatus.Declined, "U", "U", "U")]
        [DataRow(AVS_VISA_9, "NOT_CHECKED", "NOT_CHECKED", "NOT_CHECKED", Declined, TransactionStatus.Declined, "U", "I", "I")]
        [DataRow(AVS_VISA_10, "NOT_CHECKED", "NOT_CHECKED", "NOT_CHECKED", Declined, TransactionStatus.Declined, "U", "P", "P")]
        [DataRow(AVS_VISA_11, "NOT_CHECKED", "MATCHED", "MATCHED", Declined, TransactionStatus.Declined, "U", "M", "M")]
        [DataRow(AVS_VISA_12, "NOT_CHECKED", "MATCHED", "NOT_MATCHED", Declined, TransactionStatus.Declined, "U", "M", "N")]
        [DataRow(AVS_VISA_13, "NOT_CHECKED", "NOT_MATCHED", "MATCHED", Declined, TransactionStatus.Declined, "U", "N", "M")]
        [DataRow(AVS_VISA_14, "NOT_CHECKED", "NOT_MATCHED", "NOT_MATCHED", Declined, TransactionStatus.Declined, "U", "N", "N")]
        public void CreditSale_CvvResult(string cardNumber, string cvnResponseMessage, 
            string avsResponseCode, string avsAddressResponse, string status, TransactionStatus transactionStatus, 
            string cvvResult, string avsPostcode, string addressResult) {
            
            var address = new Address {
                StreetAddress1 = "123 Main St.",
                City = "Downtown",
                State = "NJ",
                Country = "US",
                PostalCode = "12345"
            };

            card.Number = cardNumber;

            var response = card.Charge(1.25m)
                .WithCurrency(CURRENCY)
                .WithAddress(address)
                .Execute();
            
            Assert.IsNotNull(response);
            Assert.AreEqual(status, response.ResponseCode);
            Assert.AreEqual(GetMapping(transactionStatus), response.ResponseMessage);
            Assert.AreEqual(cvnResponseMessage, response.CvnResponseMessage);
            Assert.AreEqual(avsResponseCode, response.AvsResponseCode);
            Assert.AreEqual(avsAddressResponse, response.AvsAddressResponse);
            Assert.AreEqual(cvvResult, response.CardIssuerResponse.CvvResult);
            Assert.AreEqual(avsPostcode, response.CardIssuerResponse.AvsPostalCodeResult);
            Assert.AreEqual(addressResult, response.CardIssuerResponse.AvsAddressResult);
        }
        
        private void AssertTransactionResponse(Transaction transaction, TransactionStatus transactionStatus) {
            Assert.IsNotNull(transaction);
            Assert.AreEqual(Success, transaction.ResponseCode);
            Assert.AreEqual(GetMapping(transactionStatus), transaction.ResponseMessage);
        }
    }
}
