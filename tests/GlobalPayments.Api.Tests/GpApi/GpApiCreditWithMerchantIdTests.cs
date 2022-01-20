using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Utils.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace GlobalPayments.Api.Tests.GpApi {
    [TestClass]
    public class GpApiCreditWithMerchantIdTests : BaseGpApiTests {
        CreditCardData card;
        string merchantId = "MER_c4c0df11039c48a9b63701adeaa296c3";

        [TestInitialize]
        public void TestInitialize() {
            ServicesContainer.ConfigureService(new GpApiConfig
            {
                AppId = "zKxybfLqH7vAOtBQrApxD5AUpS3ITaPz",
                AppKey = "GAMlgEojm6hxZTLI",
                Channel = Channel.CardNotPresent,
                AccessTokenInfo = new AccessTokenInfo
                {
                    TransactionProcessingAccountName = "transaction_processing",
                    TokenizationAccountName = "Tokenization"
                },
                MerchantId = merchantId,
                ChallengeNotificationUrl = "https://ensi808o85za.x.pipedream.net/",
                MethodNotificationUrl = "https://ensi808o85za.x.pipedream.net/",
                MerchantContactUrl = "https://enp4qhvjseljg.x.pipedream.net/",
                RequestLogger = new RequestConsoleLogger(),
                // DO NO DELETE - usage example for some settings
                //DynamicHeaders = new Dictionary<string, string>
                //{
                //    ["x-gp-platform"] = "prestashop;version=1.7.2",
                //    ["x-gp-extension"] = "coccinet;version=2.4.1"
                //}
            });

            card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 05,
                ExpYear = 2025,
                Cvn = "852",
            };
        }

        [TestMethod]
        public void CreditAuthorization() {
            var transaction = card.Authorize(14m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)

                .Execute();
            Assert.IsNotNull(transaction);
            Assert.AreEqual(SUCCESS, transaction?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Preauthorized), transaction?.ResponseMessage);

            var capture = transaction.Capture(16m)
                .WithGratuity(2m)

                .Execute();
            Assert.IsNotNull(capture);
            Assert.AreEqual(SUCCESS, capture?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), capture?.ResponseMessage);
        }

        [TestMethod]
        public void CreditAuthorization_CaptureLowerAmount() {
            var transaction = card.Authorize(5m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(transaction);
            Assert.AreEqual(SUCCESS, transaction?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Preauthorized), transaction?.ResponseMessage);

            var capture = transaction.Capture(2.99m)
                .WithGratuity(2m)
                .Execute();
            Assert.IsNotNull(capture);
            Assert.AreEqual(SUCCESS, capture?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), capture?.ResponseMessage);
        }

        [TestMethod]
        public void CreditAuthorization_CaptureHigherAmount() {
            decimal amount = 10m;
            var transaction = card.Authorize(amount)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)

                .Execute();
            Assert.IsNotNull(transaction);
            Assert.AreEqual(SUCCESS, transaction?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Preauthorized), transaction?.ResponseMessage);

            var capture = transaction.Capture(amount * 1.15m)
                .WithGratuity(2m)

                .Execute();
            Assert.IsNotNull(capture);
            Assert.AreEqual(SUCCESS, capture?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), capture?.ResponseMessage);
        }

        [TestMethod]
        public void CreditAuthorization_CaptureHigherAmount_WithError() {
            decimal amount = 10m;
            var transaction = card.Authorize(amount)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)

                .Execute();
            Assert.IsNotNull(transaction);
            Assert.AreEqual(SUCCESS, transaction?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Preauthorized), transaction?.ResponseMessage);

            try {
                var capture = transaction.Capture(amount * 1.16m)
                    .WithGratuity(2m)
    
                    .Execute();
            }
            catch (GatewayException ex) {
                Assert.AreEqual("50020", ex.ResponseMessage);
                Assert.AreEqual("INVALID_REQUEST_DATA", ex.ResponseCode);
                Assert.AreEqual("Status Code: BadRequest - Can't settle for more than 115% of that which you authorised ", ex.Message);
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

            var response = card.Charge(19.99m)
                .WithCurrency("USD")
                .WithAddress(address)

                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual(SUCCESS, response?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response?.ResponseMessage);
        }

        [TestMethod]
        public void CreditSale_WithRequestMultiUseToken() {
            var response = card.Charge(19.99m)
                .WithCurrency("USD")
                .WithRequestMultiUseToken(true)

                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual(SUCCESS, response?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response?.ResponseMessage);
            Assert.IsNotNull(response.Token);
        }

        [TestMethod]
        public void CreditRefund() {
            var response = card.Refund(16m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)

                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual(SUCCESS, response?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response?.ResponseMessage);
        }

        [TestMethod]
        public void CreditRefundTransaction() {
            decimal amount = 10.95m;
            var transaction = card.Charge(amount)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)

                .Execute();
            Assert.IsNotNull(transaction);
            Assert.AreEqual(SUCCESS, transaction?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), transaction?.ResponseMessage);

            var response = transaction.Refund(amount)
                .WithCurrency("USD")

                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual(SUCCESS, response?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response?.ResponseMessage);
        }

        [TestMethod]
        public void CreditRefundTransaction_WithIdempotencyKey() {
            decimal amount = 10.95m;
            string idempotencyKey = Guid.NewGuid().ToString();

            var transaction = card.Charge(amount)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)

                .Execute();
            Assert.IsNotNull(transaction);
            Assert.AreEqual(SUCCESS, transaction?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), transaction?.ResponseMessage);

            var response = transaction.Refund(amount)
                .WithCurrency("USD")
                .WithIdempotencyKey(idempotencyKey)

                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual(SUCCESS, response?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response?.ResponseMessage);

            var exceptionCaught = false;
            try {
                transaction.Refund(amount)
                    .WithCurrency("USD")
                    .WithIdempotencyKey(idempotencyKey)
    
                    .Execute();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("DUPLICATE_ACTION", ex.ResponseCode);
                Assert.AreEqual("40039", ex.ResponseMessage);
                Assert.AreEqual($"Status Code: Conflict - Idempotency Key seen before: id={response.TransactionId}, status=CAPTURED",
                    ex.Message);
            }
            finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void CreditRefundTransaction_RefundLowerAmount() {
            var transaction = card.Charge(5.95m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)

                .Execute();
            Assert.IsNotNull(transaction);
            Assert.AreEqual(SUCCESS, transaction?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), transaction?.ResponseMessage);

            var response = transaction.Refund(3.25m)
                .WithCurrency("USD")

                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual(SUCCESS, response?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response?.ResponseMessage);
        }

        [TestMethod]
        public void CreditRefundTransaction_RefundHigherAmount() {
            decimal amount = 5.95m;
            var transaction = card.Charge(amount)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)

                .Execute();
            Assert.IsNotNull(transaction);
            Assert.AreEqual(SUCCESS, transaction?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), transaction?.ResponseMessage);

            try {
                var response = transaction.Refund(amount * 1.1m)
                    .WithCurrency("USD")
    
                    .Execute();
            }
            catch (GatewayException ex) {
                Assert.AreEqual("40087", ex.ResponseMessage);
                Assert.AreEqual("INVALID_REQUEST_DATA", ex.ResponseCode);
                Assert.AreEqual("Status Code: BadRequest - You may only refund up to 100% of the original amount ", ex.Message);
            }
        }

        [TestMethod]
        public void CreditRefundTransactionWrongId() {
            var transaction = card.Charge(10.95m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)

                .Execute();
            transaction.TransactionId = "TRN_wrongid3213213";
            transaction.Token = "TRN_wrongid3213213";

            try {
                transaction.Refund(10.95m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
    
                    .Execute();
            }
            catch (GatewayException ex) {
                Assert.AreEqual("RESOURCE_NOT_FOUND", ex.ResponseCode);
                Assert.AreEqual("40008", ex.ResponseMessage);
                Assert.AreEqual($"Status Code: NotFound - Transaction {transaction.TransactionId} not found at this location.", ex.Message);
            }
        }

        [TestMethod]
        public void CreditReverseTransaction() {
            var transaction = card.Charge(12.99m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)

                .Execute();
            Assert.IsNotNull(transaction);
            Assert.AreEqual(SUCCESS, transaction?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), transaction?.ResponseMessage);

            var response = transaction.Reverse(12.99m)

                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual(SUCCESS, response?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Reversed), response?.ResponseMessage);
        }

        [TestMethod]
        public void CreditReverseTransaction_WithIdempotencyKey() {
            string idempotencyKey = Guid.NewGuid().ToString();

            var transaction = card.Authorize(12.99m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)

                .Execute();
            Assert.IsNotNull(transaction);
            Assert.AreEqual(SUCCESS, transaction?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Preauthorized), transaction?.ResponseMessage);

            var response = transaction.Reverse(12.99m)
                .WithIdempotencyKey(idempotencyKey)

                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual(SUCCESS, response?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Reversed), response?.ResponseMessage);

            var exceptionCaught = false;
            try {
                transaction.Reverse(12.99m)
                    .WithIdempotencyKey(idempotencyKey)
    
                    .Execute();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("DUPLICATE_ACTION", ex.ResponseCode);
                Assert.AreEqual("40039", ex.ResponseMessage);
                Assert.AreEqual($"Status Code: Conflict - Idempotency Key seen before: id={response.TransactionId}, status=REVERSED",
                    ex.Message);
            }
            finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void CreditReverseTransactionWrongId() {
            var transaction = card.Charge(12.99m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)

                .Execute();
            transaction.TransactionId = "TRN_wrongid3213213";
            transaction.Token = "TRN_wrongid3213213";

            try {
                transaction.Refund(10.95m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
    
                    .Execute();
            }
            catch (GatewayException ex) {
                Assert.AreEqual("RESOURCE_NOT_FOUND", ex.ResponseCode);
                Assert.AreEqual("40008", ex.ResponseMessage);
                Assert.AreEqual($"Status Code: NotFound - Transaction {transaction.TransactionId} not found at this location.", ex.Message);
            }
        }

        [TestMethod]
        public void CreditPartialReverseTransaction() {
            var transaction = card.Charge(3.99m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)

                .Execute();
            Assert.IsNotNull(transaction);
            Assert.AreEqual(SUCCESS, transaction?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), transaction?.ResponseMessage);

            try {
                transaction.Reverse(1.29m)
    
                    .Execute();
            }
            catch (GatewayException ex) {
                Assert.AreEqual("INVALID_REQUEST_DATA", ex.ResponseCode);
                Assert.AreEqual("40214", ex.ResponseMessage);
                Assert.AreEqual("Status Code: BadRequest - partial reversal not supported", ex.Message);
            }
        }

        [TestMethod]
        public void CreditAuthorizationForMultiCapture() {
            var authorization = card.Authorize(14m)
                .WithCurrency("USD")
                .WithMultiCapture(true)
                .WithAllowDuplicates(true)

                .Execute();
            Assert.IsNotNull(authorization);
            Assert.AreEqual(SUCCESS, authorization?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Preauthorized), authorization?.ResponseMessage);

            var capture1 = authorization.Capture(3m)

                .Execute();
            Assert.IsNotNull(capture1);
            Assert.AreEqual(SUCCESS, capture1?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), capture1?.ResponseMessage);

            var capture2 = authorization.Capture(5m)

                .Execute();
            Assert.IsNotNull(capture2);
            Assert.AreEqual(SUCCESS, capture2?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), capture2?.ResponseMessage);

            var capture3 = authorization.Capture(7m)

                .Execute();
            Assert.IsNotNull(capture3);
            Assert.AreEqual(SUCCESS, capture3?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), capture3?.ResponseMessage);
        }

        [TestMethod]
        public void CreditAuthorizationAndCapture_WithIdempotencyKey() {
            string idempotencyKey = Guid.NewGuid().ToString();

            var authorization = card.Authorize(14m)
                .WithCurrency("USD")
                .WithMultiCapture(true)
                .WithAllowDuplicates(true)

                .Execute();
            Assert.IsNotNull(authorization);
            Assert.AreEqual(SUCCESS, authorization?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Preauthorized), authorization?.ResponseMessage);

            var capture = authorization.Capture(14m)
                .WithIdempotencyKey(idempotencyKey)

                .Execute();
            Assert.IsNotNull(capture);
            Assert.AreEqual(SUCCESS, capture?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), capture?.ResponseMessage);

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
            }
            finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void CreditCaptureWrongId() {
            var authorization = card.Authorize(14m)
                .WithCurrency("USD")
                .WithMultiCapture(true)
                .WithAllowDuplicates(true)

                .Execute();
            Assert.IsNotNull(authorization);
            Assert.AreEqual(SUCCESS, authorization?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Preauthorized), authorization?.ResponseMessage);
            authorization.TransactionId = "TRN_wrongid3213213";
            authorization.Token = "TRN_wrongid3213213";

            try {
                authorization.Capture(3m)
    
                    .Execute();
            }
            catch (GatewayException ex) {
                Assert.AreEqual("RESOURCE_NOT_FOUND", ex.ResponseCode);
                Assert.AreEqual("40008", ex.ResponseMessage);
                Assert.AreEqual($"Status Code: NotFound - Transaction {authorization.TransactionId} not found at this location.", ex.Message);
            }
        }

        [TestMethod]
        public void SaleWithTokenizedPaymentMethod() {
            var tokenizedCard = new CreditCardData {
                Token = card.Tokenize(),
            };

            var response = tokenizedCard.Charge(17.01m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)

                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual(SUCCESS, response?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response?.ResponseMessage);
        }

        [TestMethod]
        public void CardTokenizationThenPayingWithToken_SingleToMultiUse()
        {
            var token = card.Tokenize(paymentMethodUsageMode: PaymentMethodUsageMode.Single);

            Assert.IsNotNull(token);

            var tokenizedCard = new CreditCardData
            {
                Token = token,
                CardHolderName = "James Mason"
            };

            var response = tokenizedCard.Charge(17.01m)
                .WithCurrency("USD")
                .WithRequestMultiUseToken(true)

                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual(SUCCESS, response?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response?.ResponseMessage);
            Assert.IsTrue(response.Token.StartsWith("PMT_"));

            tokenizedCard.Token = response.Token;
            tokenizedCard.Charge(10)
                .WithCurrency("USD")

                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual(SUCCESS, response?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response?.ResponseMessage);
        }

        [TestMethod]
        public void CreditVerify() {
            var response = card.Verify()
                .WithCurrency("USD")

                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual(SUCCESS, response?.ResponseCode);
            Assert.AreEqual(VERIFIED, response?.ResponseMessage);
        }

        [TestMethod]
        public void CreditVerify_WithAddress() {
            var address = new Address {
                PostalCode = "750241234",
                StreetAddress1 = "6860 Dallas Pkwy",
            };

            var response = card.Verify()
                .WithCurrency("USD")
                .WithAddress(address)

                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual(SUCCESS, response?.ResponseCode);
            Assert.AreEqual(VERIFIED, response?.ResponseMessage);
        }

        [TestMethod]
        public void CreditVerify_WithIdempotencyKey() {
            string idempotencyKey = Guid.NewGuid().ToString();

            var response = card.Verify()
                .WithCurrency("USD")
                .WithIdempotencyKey(idempotencyKey)

                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual(SUCCESS, response?.ResponseCode);
            Assert.AreEqual(VERIFIED, response?.ResponseMessage);

            var exceptionCaught = false;
            try {
                card.Verify()
                    .WithCurrency("USD")
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
            try {
                var response = card.Verify()
    
                    .Execute();
            }
            catch (GatewayException ex) {
                Assert.AreEqual("40005", ex.ResponseMessage);
                Assert.AreEqual("MANDATORY_DATA_MISSING", ex.ResponseCode);
                Assert.AreEqual("Status Code: BadRequest - Request expects the following fields currency", ex.Message);
            }
        }

        [TestMethod]
        public void CreditVerify_InvalidCVV() {
            card.Cvn = "1234";

            var exceptionCaught = false;
            try {
                card.Verify()
                .WithCurrency("USD")

                .Execute();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("INVALID_REQUEST_DATA", ex.ResponseCode);
                Assert.AreEqual("40085", ex.ResponseMessage);
                Assert.AreEqual("Status Code: BadRequest - Security Code/CVV2/CVC must be 3 digits", ex.Message);
            }
            finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void CreditVerify_NotNumericCVV() {
            card.Cvn = "SMA";

            var exceptionCaught = false;
            try {
                card.Verify()
                    .WithCurrency("USD")
    
                    .Execute();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("SYSTEM_ERROR_DOWNSTREAM", ex.ResponseCode);
                Assert.AreEqual("50018", ex.ResponseMessage);
                Assert.AreEqual("Status Code: BadGateway - The line number 12 which contains '         [number] XXX [/number] ' does not conform to the schema", ex.Message);
            }
            finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void CreditVerify_CP() {
            ServicesContainer.ConfigureService(new GpApiConfig {
                Environment = Entities.Environment.TEST,
                AppId = "zKxybfLqH7vAOtBQrApxD5AUpS3ITaPz",
                AppKey = "GAMlgEojm6hxZTLI",
                SecondsToExpire = 60,
                Channel = Channel.CardPresent,
                MerchantId = merchantId,
                AccessTokenInfo = new AccessTokenInfo
                {
                    TransactionProcessingAccountName = "transaction_processing",
                    TokenizationAccountName = "transaction_processing"
                },
                RequestLogger = new RequestConsoleLogger()
            });
            card.CardPresent = true;
            card.Cvn = "123";
            var response = card.Verify()
                .WithCurrency("USD")

                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual(SUCCESS, response?.ResponseCode);
            Assert.AreEqual(VERIFIED, response?.ResponseMessage);
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
                        Environment = Entities.Environment.TEST,
                        AppId = "zKxybfLqH7vAOtBQrApxD5AUpS3ITaPz",
                        AppKey = "GAMlgEojm6hxZTLI",
                        SecondsToExpire = 60,
                        Channel = channel,
                        MerchantId = merchantId,
                        AccessTokenInfo = new AccessTokenInfo
                        {
                            TransactionProcessingAccountName = "transaction_processing",
                            TokenizationAccountName = "transaction_processing"
                        },
                        RequestLogger = new RequestConsoleLogger()
                    });
                    if (channel == Channel.CardPresent)
                        card.CardPresent = true;
                    card.Cvn = "123";
                    card.EntryMethod = entryMethod;

                    var response = card.Charge(11m)
                        .WithCurrency("USD")
        
                        .Execute();

                    Assert.IsNotNull(response);
                    Assert.AreEqual(SUCCESS, response?.ResponseCode);
                    Assert.AreEqual("CAPTURED", response?.ResponseMessage);

                }
            }
        }

        [TestMethod]
        public void CreditSaleWithEntryMethod()
        {
            foreach (EntryMethod entryMethod in Enum.GetValues(typeof(EntryMethod)))
            {
                ServicesContainer.ConfigureService(new GpApiConfig
                {
                    Environment = Entities.Environment.TEST,
                    AppId = "zKxybfLqH7vAOtBQrApxD5AUpS3ITaPz",
                    AppKey = "GAMlgEojm6hxZTLI",
                    SecondsToExpire = 60,
                    Channel = Channel.CardPresent,
                    AccessTokenInfo = new AccessTokenInfo
                    {
                        TransactionProcessingAccountName = "transaction_processing",
                        TokenizationAccountName = "transaction_processing"
                    },
                    MerchantId = merchantId,
                    RequestLogger = new RequestConsoleLogger()
                });

                var creditTrackData = new CreditTrackData
                {
                    TrackData =
                        "%B4012002000060016^VI TEST CREDIT^251210118039000000000396?;4012002000060016=25121011803939600000?",
                    EntryMethod = entryMethod
                };

                var response = creditTrackData.Charge(11m)
                    .WithCurrency("USD")
    
                    .Execute();

                Assert.IsNotNull(response);
                Assert.AreEqual(SUCCESS, response?.ResponseCode);
                Assert.AreEqual("CAPTURED", response?.ResponseMessage);

            }
        }

        [TestMethod, Ignore]
        //To be removed
        public void CreditVerify_CP_CVNNotMatched() {
            ServicesContainer.ConfigureService(new GpApiConfig {
                Environment = Entities.Environment.TEST,
                AppId = "zKxybfLqH7vAOtBQrApxD5AUpS3ITaPz",
                AppKey = "GAMlgEojm6hxZTLI",
                SecondsToExpire = 60,
                Channel = Channel.CardPresent,
                MerchantId = merchantId,
                AccessTokenInfo = new AccessTokenInfo
                {
                    TransactionProcessingAccountName = "transaction_processing",
                    TokenizationAccountName = "transaction_processing"
                },
                RequestLogger = new RequestConsoleLogger(),
            });
            var response = card.Verify()
                .WithCurrency("USD")

                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("NOT_VERIFIED", response?.ResponseCode);
            Assert.AreEqual("NOT_VERIFIED", response?.ResponseMessage);
            Assert.AreEqual("NOT_MATCHED", response?.CvnResponseMessage);
        }

        [TestMethod]
        public void CreditChargeTransactions_WithSameIdempotencyKey() {
            string idempotencyKey = Guid.NewGuid().ToString();

            var transaction1 = card.Charge(4.95m)
                .WithCurrency("USD")
                .WithIdempotencyKey(idempotencyKey)

                .Execute();
            Assert.IsNotNull(transaction1);
            Assert.AreEqual(SUCCESS, transaction1?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), transaction1?.ResponseMessage);

            try {
                card.Charge(4.95m)
                    .WithCurrency("USD")
                    .WithIdempotencyKey(idempotencyKey)
    
                    .Execute();
            }
            catch (GatewayException ex) {
                Assert.AreEqual("DUPLICATE_ACTION", ex.ResponseCode);
                Assert.AreEqual("40039", ex.ResponseMessage);
                Assert.AreEqual($"Status Code: Conflict - Idempotency Key seen before: id={transaction1.TransactionId}, status=CAPTURED", ex.Message);
            }
        }

        [TestMethod]
        public void CreditVerify_WithStoredCredentials() {
            var response = card.Verify()
                .WithCurrency("USD")
                .WithStoredCredential(new StoredCredential {
                    Initiator = StoredCredentialInitiator.CardHolder,
                    Type = StoredCredentialType.Subscription,
                    Sequence = StoredCredentialSequence.First,
                    Reason = StoredCredentialReason.Incremental
                })

                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual(SUCCESS, response?.ResponseCode);
        }

        [TestMethod]
        public void CreditSale_WithStoredCredentials() {
            var response = card.Charge(15.25m)
                .WithCurrency("USD")
                .WithStoredCredential(new StoredCredential {
                    Initiator = StoredCredentialInitiator.CardHolder,
                    Type = StoredCredentialType.Subscription,
                    Sequence = StoredCredentialSequence.First,
                    Reason = StoredCredentialReason.Incremental
                })

                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual(SUCCESS, response?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response?.ResponseMessage);
        }

        [TestMethod]
        public void CreditSale_WithStoredCredentials_RecurringPayment()
        {
            var amount = 15.00m;
            var currency = "USD";
            var card = new CreditCardData
            {
                Number = "4263970000005262",
                ExpMonth = 05,
                ExpYear = 2025,
            };

            var tokenizedCard = new CreditCardData
            {
                Token = card.Tokenize(),
            };

            var secureEcom = Secure3dService.CheckEnrollment(tokenizedCard)
                .WithCurrency(currency)
                .WithAmount(amount)
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
                .WithAmount(amount)
                .WithCurrency(currency)
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

            var response = tokenizedCard.Charge(amount)
                .WithCurrency(currency)

                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual(SUCCESS, response?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response?.ResponseMessage);
            Assert.IsNotNull(response.CardBrandTransactionId);

            var response2 = tokenizedCard.Charge(amount)
                .WithCurrency(currency)
                .WithStoredCredential(new StoredCredential
                {
                    Initiator = StoredCredentialInitiator.Merchant,
                    Type = StoredCredentialType.Recurring,
                    Sequence = StoredCredentialSequence.Subsequent,
                    Reason = StoredCredentialReason.Incremental
                })
                .WithCardBrandStorage(StoredCredentialInitiator.Merchant, response.CardBrandTransactionId)

                .Execute();

            Assert.IsNotNull(response2);
            Assert.AreEqual(SUCCESS, response2?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response2?.ResponseMessage);
        }
    }
}
