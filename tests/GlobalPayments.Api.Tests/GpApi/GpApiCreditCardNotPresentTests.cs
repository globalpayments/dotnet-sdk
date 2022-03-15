using System;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Utils.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using static GlobalPayments.Api.Tests.GpApi.GpApiAvsCheckTestCards;
using Environment = GlobalPayments.Api.Entities.Environment;

namespace GlobalPayments.Api.Tests.GpApi {
    [TestClass]
    public class GpApiCreditCardNotPresentTests : BaseGpApiTests {
        private CreditCardData card;
        private const decimal AMOUNT = 7.8m;
        private const string CURRENCY = "USD";

        [TestInitialize]
        public void TestInitialize() {
            ServicesContainer.ConfigureService(new GpApiConfig {
                AppId = APP_ID,
                AppKey = APP_KEY,
                Channel = Channel.CardNotPresent,
                ChallengeNotificationUrl = "https://ensi808o85za.x.pipedream.net/",
                MethodNotificationUrl = "https://ensi808o85za.x.pipedream.net/",
                MerchantContactUrl = "https://enp4qhvjseljg.x.pipedream.net/",
                // RequestLogger = new RequestFileLogger(@"C:\temp\transit\finger.txt"),
                RequestLogger = new RequestConsoleLogger(),
                EnableLogging = true,
                // DO NO DELETE - usage example for some settings
                // DynamicHeaders = new Dictionary<string, string>
                // {
                //     ["x-gp-platform"] = "prestashop;version=1.7.2",
                //     ["x-gp-extension"] = "coccinet;version=2.4.1"
                // }
            });

            card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = expMonth,
                ExpYear = expYear,
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
            Assert.AreEqual(SUCCESS, response?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response?.ResponseMessage);
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
            Assert.AreEqual(SUCCESS, response?.ResponseCode);
            Assert.AreEqual("VERIFIED", response?.ResponseMessage);
            Assert.IsNotNull(response.FingerPrint);           
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
            Assert.AreEqual(SUCCESS, response?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response?.ResponseMessage);
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
            Assert.AreEqual(DECLINED, response?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Declined), response?.ResponseMessage);
            Assert.AreEqual("",response.FingerPrint);           
            Assert.AreEqual("",response.FingerPrintIndicator);           
        }
        
        [TestMethod]
        public void CreditAuthorizationWithPaymentLinkId()
        {
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
            } 
            catch (GatewayException ex) {
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
                Assert.AreEqual($"Status Code: Conflict - Idempotency Key seen before: id={response.TransactionId}, status=CAPTURED",
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
                Assert.AreEqual($"Status Code: Conflict - Idempotency Key seen before: id={response.TransactionId}, status=REVERSED",
                    ex.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
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
                .WithMultiCapture()
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
                Assert.AreEqual($"Status Code: Conflict - Idempotency Key seen before: id={capture.TransactionId}, status=CAPTURED",
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
            var token = card.Tokenize(paymentMethodUsageMode: PaymentMethodUsageMode.Single);
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
            Assert.AreEqual(SUCCESS, response.ResponseCode);
            Assert.AreEqual(VERIFIED, response.ResponseMessage);
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
            Assert.AreEqual(SUCCESS, response.ResponseCode);
            Assert.AreEqual(VERIFIED, response.ResponseMessage);
        }

        [TestMethod]
        public void CreditVerify_WithIdempotencyKey() {
            var idempotencyKey = Guid.NewGuid().ToString();

            var response = card.Verify()
                .WithCurrency(CURRENCY)
                .WithIdempotencyKey(idempotencyKey)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual(SUCCESS, response?.ResponseCode);
            Assert.AreEqual(VERIFIED, response?.ResponseMessage);

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
                Assert.AreEqual($"Status Code: Conflict - Idempotency Key seen before: id={response.TransactionId}, status=VERIFIED",
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
            foreach (Channel channel in Enum.GetValues(typeof(Channel))) 
            {
                foreach (ManualEntryMethod entryMethod in Enum.GetValues(typeof(ManualEntryMethod)))
                {
                    ServicesContainer.ConfigureService(new GpApiConfig
                    {
                        Environment = Environment.TEST,
                        AppId = APP_ID,
                        AppKey = APP_KEY,
                        SecondsToExpire = 60,
                        Channel = channel,
                        RequestLogger = new RequestConsoleLogger()
                    });
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
            foreach (EntryMethod entryMethod in Enum.GetValues(typeof(EntryMethod))) {
                ServicesContainer.ConfigureService(new GpApiConfig {
                    Environment = Environment.TEST,
                    AppId = APP_ID,
                    AppKey = APP_KEY,
                    SecondsToExpire = 60,
                    Channel = Channel.CardPresent,
                    RequestLogger = new RequestConsoleLogger()
                });

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
                Assert.AreEqual($"Status Code: Conflict - Idempotency Key seen before: id={transaction1.TransactionId}, status=CAPTURED", ex.Message);
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
            Assert.AreEqual(SUCCESS, response?.ResponseCode);
            Assert.AreEqual(VERIFIED, response?.ResponseMessage);
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
        [DataRow(AVS_MASTERCARD_1, "MATCHED", "NOT_CHECKED", "NOT_CHECKED", SUCCESS, TransactionStatus.Captured)]
        [DataRow(AVS_MASTERCARD_2, "MATCHED", "NOT_CHECKED", "NOT_CHECKED", SUCCESS, TransactionStatus.Captured)]
        [DataRow(AVS_MASTERCARD_3, "MATCHED", "NOT_CHECKED", "NOT_CHECKED", SUCCESS, TransactionStatus.Captured)]
        [DataRow(AVS_MASTERCARD_4, "MATCHED", "MATCHED", "MATCHED", SUCCESS, TransactionStatus.Captured)]
        [DataRow(AVS_MASTERCARD_5, "MATCHED", "MATCHED", "NOT_MATCHED", SUCCESS, TransactionStatus.Captured)]
        [DataRow(AVS_MASTERCARD_6, "MATCHED", "NOT_MATCHED", "MATCHED", SUCCESS, TransactionStatus.Captured)]
        [DataRow(AVS_MASTERCARD_7, "MATCHED", "NOT_MATCHED", "NOT_MATCHED", SUCCESS, TransactionStatus.Captured)]
        [DataRow(AVS_MASTERCARD_8, "NOT_MATCHED", "NOT_MATCHED", "MATCHED", SUCCESS, TransactionStatus.Captured)]
        [DataRow(AVS_MASTERCARD_9, "NOT_MATCHED", "NOT_CHECKED", "NOT_CHECKED", SUCCESS, TransactionStatus.Captured)]
        [DataRow(AVS_MASTERCARD_10, "NOT_MATCHED", "NOT_CHECKED", "NOT_CHECKED", SUCCESS, TransactionStatus.Captured)]
        [DataRow(AVS_MASTERCARD_11, "NOT_MATCHED", "NOT_CHECKED", "NOT_CHECKED", SUCCESS, TransactionStatus.Captured)]
        [DataRow(AVS_MASTERCARD_12, "NOT_MATCHED", "MATCHED", "MATCHED", SUCCESS, TransactionStatus.Captured)]
        [DataRow(AVS_MASTERCARD_13, "NOT_MATCHED", "MATCHED", "NOT_MATCHED", SUCCESS, TransactionStatus.Captured)]
        [DataRow(AVS_MASTERCARD_14, "NOT_MATCHED", "NOT_MATCHED", "NOT_MATCHED", SUCCESS, TransactionStatus.Captured)]
        [DataRow(AVS_VISA_1, "NOT_CHECKED", "NOT_CHECKED", "NOT_CHECKED", DECLINED, TransactionStatus.Declined)]
        [DataRow(AVS_VISA_2, "NOT_CHECKED", "NOT_CHECKED", "NOT_CHECKED", DECLINED, TransactionStatus.Declined)]
        [DataRow(AVS_VISA_3, "NOT_CHECKED", "NOT_CHECKED", "NOT_CHECKED", DECLINED, TransactionStatus.Declined)]
        [DataRow(AVS_VISA_4, "NOT_CHECKED", "MATCHED", "MATCHED", DECLINED, TransactionStatus.Declined)]
        [DataRow(AVS_VISA_5, "NOT_CHECKED", "MATCHED", "NOT_MATCHED", DECLINED, TransactionStatus.Declined)]
        [DataRow(AVS_VISA_6, "NOT_CHECKED", "NOT_MATCHED", "MATCHED", DECLINED, TransactionStatus.Declined)]
        [DataRow(AVS_VISA_7, "NOT_CHECKED", "NOT_MATCHED", "NOT_MATCHED", DECLINED, TransactionStatus.Declined)]
        [DataRow(AVS_VISA_8, "NOT_CHECKED", "NOT_CHECKED", "NOT_CHECKED", DECLINED, TransactionStatus.Declined)]
        [DataRow(AVS_VISA_9, "NOT_CHECKED", "NOT_CHECKED", "NOT_CHECKED", DECLINED, TransactionStatus.Declined)]
        [DataRow(AVS_VISA_10, "NOT_CHECKED", "NOT_CHECKED", "NOT_CHECKED", DECLINED, TransactionStatus.Declined)]
        [DataRow(AVS_VISA_11, "NOT_CHECKED", "MATCHED", "MATCHED", DECLINED, TransactionStatus.Declined)]
        [DataRow(AVS_VISA_12, "NOT_CHECKED", "MATCHED", "NOT_MATCHED", DECLINED, TransactionStatus.Declined)]
        [DataRow(AVS_VISA_13, "NOT_CHECKED", "NOT_MATCHED", "MATCHED", DECLINED, TransactionStatus.Declined)]
        [DataRow(AVS_VISA_14, "NOT_CHECKED", "NOT_MATCHED", "NOT_MATCHED", DECLINED, TransactionStatus.Declined)]
        public void CreditSale_CvvResult(string cardNumber, string cvnResponseMessage, 
            string avsResponseCode, string avsAddressResponse, string status, TransactionStatus transactionStatus) {
            
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
            Assert.AreEqual(status, response?.ResponseCode);
            Assert.AreEqual(GetMapping(transactionStatus), response?.ResponseMessage);
            Assert.AreEqual(cvnResponseMessage, response.CvnResponseMessage);
            Assert.AreEqual(avsResponseCode, response.AvsResponseCode);
            Assert.AreEqual(avsAddressResponse, response.AvsAddressResponse);
        }
        
        private void AssertTransactionResponse(Transaction transaction, TransactionStatus transactionStatus) {
            Assert.IsNotNull(transaction);
            Assert.AreEqual(SUCCESS, transaction.ResponseCode);
            Assert.AreEqual(GetMapping(transactionStatus), transaction.ResponseMessage);
        }
    }
}
