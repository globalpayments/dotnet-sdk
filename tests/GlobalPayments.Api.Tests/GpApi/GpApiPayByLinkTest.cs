using System;
using System.Threading;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Enums;
using GlobalPayments.Api.Entities.Reporting;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.GpApi {
    
    [TestClass]
    public class GpApiPayByLinkTest : BaseGpApiTests {
        
        private PayByLinkData payByLink;
        private CreditCardData card;
        private Address shippingAddress;
        private BrowserData browserData;
        private string PayByLinkId;

        private const decimal AMOUNT = 7.8m;
        private const string CURRENCY = "GBP";

        [TestInitialize]
        public void TestInitialize() {
            ServicesContainer.RemoveConfig();
            
            var gpApiConfig = GpApiConfigSetup("v2yRaFOLwFaQc0fSZTCyAdQCBNByGpVK", "oKZpWitk6tORoCVT", Channel.CardNotPresent);
            gpApiConfig.Country = "GB";
            gpApiConfig.AccessTokenInfo = new AccessTokenInfo { TransactionProcessingAccountName = "LinkManagement" };
            ServicesContainer.ConfigureService(gpApiConfig);

            payByLink = new PayByLinkData {
                Type = PayByLinkType.PAYMENT,
                UsageMode = PaymentMethodUsageMode.Single,
                AllowedPaymentMethods = new[] { PaymentMethodName.Card },
                UsageLimit = 1,
                Name = "Mobile Bill Payment",
                IsShippable = true,
                ShippingAmount = 1.23m,
                ExpirationDate = DateTime.UtcNow.AddDays(10), //date('Y-m-d H:i:s') + 10;
                Images = new string[] { "test", "test2", "test3" },
                ReturnUrl = "https://www.example.com/returnUrl",
                StatusUpdateUrl = "https://www.example.com/statusUrl",
                CancelUrl = "https://www.example.com/returnUrl"
            };

            card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = ExpMonth,
                ExpYear = ExpYear,
                Cvn = "123",
                CardPresent = true
            };

            shippingAddress = new Address {
                StreetAddress1 = "Apartment 852",
                StreetAddress2 = "Complex 741",
                StreetAddress3 = "no",
                City = "Chicago",
                PostalCode = "5001",
                State = "IL",
                CountryCode = "840"
            };

            browserData = new BrowserData {
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
            };

            var response = PayByLinkService.FindPayByLink(1, 1)
                .OrderBy(PayByLinkSortProperty.TimeCreated, SortDirection.Ascending)
                .Where(SearchCriteria.StartDate, StartDate)
                .And(SearchCriteria.EndDate, EndDate)
                .And(SearchCriteria.PayByLinkStatus, PayByLinkStatus.ACTIVE)
                .Execute();
            if (response.Results.Count > 0) {
                PayByLinkId = response.Results[0].Id;
            }
        }

        [TestMethod]
        public void ReportPayByLinkDetail() {
            const string payByLinkId = "LNK_hUh2IIO1YoyDU3wGwkcb4e6SE9v5dY";

            var response = PayByLinkService.PayByLinkDetail(payByLinkId)
                .Execute();

            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(PayByLinkSummary));
            Assert.AreEqual(payByLinkId, response.Id);
        }

        [TestMethod]
        public void ReportPayByLinkDetail_RandomLinkId() {
            var payByLinkId = Guid.NewGuid().ToString();

            var exceptionCaught = false;
            try {
                PayByLinkService.PayByLinkDetail(payByLinkId)
                    .Execute();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("RESOURCE_NOT_FOUND", ex.ResponseCode);
                Assert.AreEqual("40118", ex.ResponseMessage);
                Assert.AreEqual($"Status Code: NotFound - Links {payByLinkId} not found at this /ucp/links/{payByLinkId}",
                    ex.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void ReportPayByLinkDetail_NullLinkId() {
            var exceptionCaught = false;
            try {
                PayByLinkService.PayByLinkDetail(null)
                    .Execute();
            }
            catch (BuilderException ex) {
                exceptionCaught = true;
                Assert.AreEqual("PayByLinkId cannot be null for this transaction type.",
                    ex.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void FindPayByLink_ByDate() {
            var response = PayByLinkService.FindPayByLink(1, 10)
                .OrderBy(PayByLinkSortProperty.TimeCreated, SortDirection.Ascending)
                .Where(SearchCriteria.StartDate, StartDate)
                .And(SearchCriteria.EndDate, EndDate)
                .Execute();

            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Results);
            /** @var PayByLinkSummary $randomPayByLink */
            var randomPayByLinkResponseObject = response.Results[0];
            Assert.IsNotNull(randomPayByLinkResponseObject);
            Assert.IsInstanceOfType(randomPayByLinkResponseObject, typeof(PayByLinkSummary));
        }

        [TestMethod]
        public void FindPayByLink_ByDate_NoResults() {
            var response = PayByLinkService.FindPayByLink(1, 10)
                .OrderBy(PayByLinkSortProperty.TimeCreated, SortDirection.Ascending)
                .Where(SearchCriteria.StartDate, StartDate.AddMonths(-24))
                .And(SearchCriteria.EndDate, EndDate.AddMonths(-22))
                .Execute();

            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Results);
            Assert.AreEqual(0, response.Results.Count);
            Assert.AreEqual(0, response.TotalRecordCount);
        }

        [TestMethod]
        public void CreatePayByLink() {
            payByLink.Type = PayByLinkType.PAYMENT;
            payByLink.UsageMode = PaymentMethodUsageMode.Single;
            payByLink.AllowedPaymentMethods = new PaymentMethodName[] { PaymentMethodName.Card };
            payByLink.UsageLimit = 1;
            payByLink.Name = "Mobile Bill Payment";
            payByLink.IsShippable = true;
            payByLink.ShippingAmount = 1.23m;
            payByLink.ExpirationDate = DateTime.UtcNow.AddDays(10); //date('Y-m-d H:i:s') + 10;
            payByLink.Images = new string[] { "test", "test2", "test3" };
            payByLink.ReturnUrl = "https://www.example.com/returnUrl";
            payByLink.StatusUpdateUrl = "https://www.example.com/statusUrl";
            payByLink.CancelUrl = "https://www.example.com/returnUrl";

            var response = PayByLinkService.Create(payByLink, AMOUNT)
                .WithCurrency(CURRENCY)
                .WithClientTransactionId(GenerationUtils.GenerateRecurringKey())
                .WithDescription("March and April Invoice")
                .Execute();

            Assert.AreEqual("SUCCESS", response.ResponseCode);
            Assert.AreEqual(PayByLinkStatus.ACTIVE.ToString().ToUpper(), response.ResponseMessage.ToUpper());
            Assert.AreEqual(AMOUNT, response.BalanceAmount);
            Assert.IsNotNull(response.PayByLinkResponse.Url);
            Assert.IsNotNull(response.PayByLinkResponse.Id);
            Assert.IsNotNull(response.PayByLinkResponse.IsShippable);
            Assert.IsTrue(response.PayByLinkResponse.IsShippable != null && response.PayByLinkResponse.IsShippable.Value);
        }
        
        [TestMethod]
        public void CreatePayByLink_WithIdempotency() {
            var idempotency = Guid.NewGuid().ToString();
            var response = PayByLinkService.Create(payByLink, AMOUNT)
                .WithCurrency(CURRENCY)
                .WithClientTransactionId(GenerationUtils.GenerateRecurringKey())
                .WithDescription("March and April Invoice")
                .WithIdempotencyKey(idempotency)
                .Execute();

            Assert.AreEqual("SUCCESS", response.ResponseCode);
            Assert.AreEqual(PayByLinkStatus.ACTIVE.ToString().ToUpper(), response.ResponseMessage.ToUpper());
            Assert.AreEqual(AMOUNT, response.BalanceAmount);
            Assert.IsNotNull(response.PayByLinkResponse.Url);
            Assert.IsNotNull(response.PayByLinkResponse.Id);
            
            var exceptionCaught = false;
            try {
                PayByLinkService.Create(payByLink, AMOUNT)
                    .WithCurrency(CURRENCY)
                    .WithClientTransactionId(GenerationUtils.GenerateRecurringKey())
                    .WithDescription("March and April Invoice")
                    .WithIdempotencyKey(idempotency)
                    .Execute();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("40039", ex.ResponseMessage);
                Assert.AreEqual("DUPLICATE_ACTION", ex.ResponseCode);
                Assert.AreEqual(
                    $"Status Code: Conflict - Idempotency Key seen before: id={response.PayByLinkResponse.Id}, status={response.ResponseMessage}",
                    ex.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void CreatePayByLink_MultipleUsage() {
            payByLink.UsageMode = PaymentMethodUsageMode.Multiple;
            payByLink.UsageLimit = 2;

            var response = PayByLinkService.Create(payByLink, AMOUNT)
                .WithCurrency(CURRENCY)
                .WithClientTransactionId(GenerationUtils.GenerateRecurringKey())
                .WithDescription("March and April Invoice")
                .Execute();

            AssertTransactionResponse(response);
            Assert.AreEqual(2, response.PayByLinkResponse.UsageLimit);
        }

        [TestMethod]
        public void CreatePayByLink_ThenCharge() {
            string[] imagesList = {
                "One", "Two", "Three"
            };
            payByLink.Images = imagesList;

            var response = PayByLinkService.Create(payByLink, AMOUNT)
                .WithCurrency(CURRENCY)
                .WithClientTransactionId(GenerationUtils.GenerateRecurringKey())
                .WithDescription("March and April Invoice")
                .Execute();

            AssertTransactionResponse(response);

            ServicesContainer.ConfigureService(SetupTransactionConfig(), "createTransaction");

            var charge = card.Charge(AMOUNT)
                .WithCurrency(CURRENCY)
                .WithPaymentLinkId(response.PayByLinkResponse.Id)
                .Execute("createTransaction");

            Assert.IsNotNull(charge);
            Assert.AreEqual(Success, charge?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), charge?.ResponseMessage);

            Thread.Sleep(2000);

            var getPayByLinkById = PayByLinkService.PayByLinkDetail(response.PayByLinkResponse.Id)
                .Execute();

            Assert.IsNotNull(getPayByLinkById);
            Assert.IsInstanceOfType(getPayByLinkById, typeof(PayByLinkSummary));
            Assert.AreEqual(response.PayByLinkResponse.Id, getPayByLinkById.Id);
            Assert.AreEqual(1, getPayByLinkById.Transactions.Count);
        }

        [TestMethod]
        public void CreatePayByLink_ThenCharge_DifferentAmount() {
            var response = PayByLinkService.Create(payByLink, AMOUNT)
                .WithCurrency(CURRENCY)
                .WithClientTransactionId(GenerationUtils.GenerateRecurringKey())
                .WithDescription("March and April Invoice")
                .Execute();

            AssertTransactionResponse(response);

            ServicesContainer.ConfigureService(SetupTransactionConfig(), "createTransaction");

            var charge = card.Charge(AMOUNT + 2)
                .WithCurrency(CURRENCY)
                .WithPaymentLinkId(response.PayByLinkResponse.Id)
                .Execute("createTransaction");

            Assert.IsNotNull(charge);
            Assert.AreEqual(Success, charge?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), charge?.ResponseMessage);

            Thread.Sleep(2000);

            var getPayByLinkById = PayByLinkService.PayByLinkDetail(response.PayByLinkResponse.Id)
                .Execute();

            Assert.IsNotNull(getPayByLinkById);
            Assert.IsInstanceOfType(getPayByLinkById, typeof(PayByLinkSummary));
            Assert.AreEqual(response.PayByLinkResponse.Id, getPayByLinkById.Id);
            Assert.AreEqual(1, getPayByLinkById.Transactions.Count);
            Assert.AreEqual(AMOUNT + 2, getPayByLinkById.Transactions[0].Amount);
        }

        [TestMethod]
        public void CreatePayByLink_MultipleUsage_ThenCharge() {
            payByLink.UsageMode = PaymentMethodUsageMode.Multiple;
            payByLink.UsageLimit = 2;

            var response = PayByLinkService.Create(payByLink, AMOUNT)
                .WithCurrency(CURRENCY)
                .WithClientTransactionId(GenerationUtils.GenerateRecurringKey())
                .WithDescription("March and April Invoice")
                .Execute();

            AssertTransactionResponse(response);

            ServicesContainer.ConfigureService(SetupTransactionConfig(), "createTransaction");

            for (var i = 1; i <= payByLink.UsageLimit; i++) {
                var charge = card.Charge(AMOUNT)
                    .WithCurrency(CURRENCY)
                    .WithPaymentLinkId(response.PayByLinkResponse.Id)
                    .Execute("createTransaction");

                Assert.IsNotNull(charge);
                Assert.AreEqual(Success, charge?.ResponseCode);
                Assert.AreEqual(GetMapping(TransactionStatus.Captured), charge?.ResponseMessage);
            }

            Thread.Sleep(2000);

            var getPayByLinkById = PayByLinkService.PayByLinkDetail(response.PayByLinkResponse.Id)
                .Execute();

            Assert.IsNotNull(getPayByLinkById);
            Assert.IsInstanceOfType(getPayByLinkById, typeof(PayByLinkSummary));
            Assert.AreEqual(response.PayByLinkResponse.Id, getPayByLinkById.Id);
            Assert.AreEqual(2, getPayByLinkById.Transactions.Count);
        }

        [TestMethod]
        public void CreatePayByLink_ThenAuthorizeAndCapture() {
            var response = PayByLinkService.Create(payByLink, AMOUNT)
                .WithCurrency(CURRENCY)
                .WithClientTransactionId(GenerationUtils.GenerateRecurringKey())
                .WithDescription("March and April Invoice")
                .Execute();

            AssertTransactionResponse(response);

            ServicesContainer.ConfigureService(SetupTransactionConfig(), "createTransaction");

            var authTransaction = card.Authorize(AMOUNT)
                .WithCurrency(CURRENCY)
                .WithPaymentLinkId(response.PayByLinkResponse.Id)
                .Execute("createTransaction");

            Assert.IsNotNull(authTransaction);
            Assert.AreEqual(Success, authTransaction?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Preauthorized), authTransaction?.ResponseMessage);

            var chargeTransaction = authTransaction.Capture(AMOUNT)
                .WithCurrency(CURRENCY)
                .WithPaymentLinkId(response.PayByLinkResponse.Id)
                .Execute("createTransaction");

            Assert.IsNotNull(chargeTransaction);
            Assert.AreEqual(Success, chargeTransaction?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), chargeTransaction?.ResponseMessage);

            Thread.Sleep(2000);

            var getPayByLinkById = PayByLinkService.PayByLinkDetail(response.PayByLinkResponse.Id)
                .Execute();

            Assert.IsNotNull(getPayByLinkById);
            Assert.IsInstanceOfType(getPayByLinkById, typeof(PayByLinkSummary));
            Assert.AreEqual(response.PayByLinkResponse.Id, getPayByLinkById.Id);
            Assert.AreEqual(1, getPayByLinkById.Transactions.Count);
            Assert.AreEqual(GetMapping(TransactionStatus.Preauthorized), getPayByLinkById.Transactions[0].TransactionStatus);
        }

        [TestMethod]
        public void CreatePayByLink_ThenCharge_WithTokenizedCard() {
            var response = PayByLinkService.Create(payByLink, AMOUNT)
                .WithCurrency(CURRENCY)
                .WithClientTransactionId(GenerationUtils.GenerateRecurringKey())
                .WithDescription("March and April Invoice")
                .Execute();

            AssertTransactionResponse(response);

            ServicesContainer.ConfigureService(SetupTransactionConfig(), "createTransaction");

            var tokenizedCard = new CreditCardData {
                Token = card.Tokenize("createTransaction")
            };

            var charge = tokenizedCard.Charge(AMOUNT)
                .WithCurrency(CURRENCY)
                .WithPaymentLinkId(response.PayByLinkResponse.Id)
                .Execute("createTransaction");

            Assert.IsNotNull(charge);
            Assert.AreEqual(Success, charge?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), charge?.ResponseMessage);

            Thread.Sleep(2000);

            var getPayByLinkById = PayByLinkService.PayByLinkDetail(response.PayByLinkResponse.Id)
                .Execute();

            Assert.IsNotNull(getPayByLinkById);
            Assert.IsInstanceOfType(getPayByLinkById, typeof(PayByLinkSummary));
            Assert.AreEqual(response.PayByLinkResponse.Id, getPayByLinkById.Id);
            Assert.AreEqual(1, getPayByLinkById.Transactions.Count);
        }

        [TestMethod]
        public void CreatePayByLink_ThenCharge_With3DS() {
            var response = PayByLinkService.Create(payByLink, AMOUNT)
                .WithCurrency(CURRENCY)
                .WithClientTransactionId(GenerationUtils.GenerateRecurringKey())
                .WithDescription("March and April Invoice")
                .Execute();

            AssertTransactionResponse(response);

            ServicesContainer.ConfigureService(SetupTransactionConfig(), "createTransaction");

            card.Number = GpApi3DSTestCards.CARD_AUTH_SUCCESSFUL_V2_2;

            var secureEcom = Secure3dService.CheckEnrollment(card)
                .WithCurrency(CURRENCY)
                .WithAmount(AMOUNT)
                .Execute("createTransaction");

            Assert.IsNotNull(secureEcom);
            Assert.AreEqual("ENROLLED", secureEcom.Enrolled, "Card not enrolled");
            Assert.AreEqual(Secure3dVersion.Two, secureEcom.Version);
            Assert.AreEqual("AVAILABLE", secureEcom.Status);

            var initAuth = Secure3dService.InitiateAuthentication(card, secureEcom)
                .WithAmount(AMOUNT)
                .WithCurrency(CURRENCY)
                .WithAuthenticationSource(AuthenticationSource.BROWSER)
                .WithMethodUrlCompletion(MethodUrlCompletion.YES)
                .WithOrderCreateDate(DateTime.Now)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithBrowserData(browserData)
                .Execute("createTransaction");

            Assert.IsNotNull(initAuth);
            Assert.AreEqual("SUCCESS_AUTHENTICATED", initAuth.Status);

            secureEcom = Secure3dService.GetAuthenticationData()
                .WithServerTransactionId(initAuth.ServerTransactionId)
                .Execute("createTransaction");

            Assert.AreEqual("SUCCESS_AUTHENTICATED", secureEcom.Status);
            Assert.AreEqual(secureEcom.LiabilityShift, "YES");

            card.ThreeDSecure = secureEcom;

            var charge = card.Charge(AMOUNT)
                .WithCurrency(CURRENCY)
                .WithPaymentLinkId(response.PayByLinkResponse.Id)
                .Execute("createTransaction");

            Assert.IsNotNull(charge);
            Assert.AreEqual(Success, charge?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), charge?.ResponseMessage);

            Thread.Sleep(2000);

            var getPayByLinkById = PayByLinkService.PayByLinkDetail(response.PayByLinkResponse.Id)
                .Execute();

            Assert.IsNotNull(getPayByLinkById);
            Assert.IsInstanceOfType(getPayByLinkById, typeof(PayByLinkSummary));
            Assert.AreEqual(response.PayByLinkResponse.Id, getPayByLinkById.Id);
            Assert.AreEqual(1, getPayByLinkById.Transactions.Count);
        }

        [TestMethod]
        public void EditPayByLink() {
            var response = PayByLinkService.FindPayByLink(1, 10)
                .OrderBy(PayByLinkSortProperty.TimeCreated, SortDirection.Ascending)
                .Where(SearchCriteria.StartDate, DateTime.UtcNow.AddDays(-10))
                .And(SearchCriteria.EndDate, EndDate)
                .And(SearchCriteria.PayByLinkStatus, PayByLinkStatus.ACTIVE)
                .Execute();

            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Results);
            /** @var PayByLinkSummary $randomPayByLink */
            var randomPayByLinkResponseObject = response.Results[0];
            Assert.IsNotNull(randomPayByLinkResponseObject);
            Assert.IsInstanceOfType(randomPayByLinkResponseObject, typeof(PayByLinkSummary));
            Assert.IsNotNull(randomPayByLinkResponseObject.Id);

            var payByLink = new PayByLinkData();
            payByLink.Name = "Test of Test";
            payByLink.UsageMode = PaymentMethodUsageMode.Multiple;
            payByLink.Type = PayByLinkType.PAYMENT;
            payByLink.UsageLimit = 5;
            //payByLink.IsShippable = false;
            
            const decimal amount = 10.08m;
            var editResponse = PayByLinkService.Edit(randomPayByLinkResponseObject.Id)
                .WithAmount(amount)
                .WithPayByLinkData(payByLink)
                .WithDescription("Update PayByLink description")
                .Execute();

            Assert.AreEqual("SUCCESS", editResponse.ResponseCode);
            Assert.AreEqual(PayByLinkStatus.ACTIVE.ToString(), editResponse.ResponseMessage);
            Assert.AreEqual(amount, editResponse.BalanceAmount);
            Assert.IsNotNull(editResponse.PayByLinkResponse.Url);
            Assert.IsNotNull(editResponse.PayByLinkResponse.Id);
        }

        [TestMethod]
        public void CreatePayByLink_MissingType() {
            payByLink.Type = null;

            var exceptionCaught = false;
            try {
                PayByLinkService.Create(payByLink, AMOUNT)
                    .WithCurrency(CURRENCY)
                    .WithClientTransactionId(GenerationUtils.GenerateRecurringKey())
                    .WithDescription("March and April Invoice")
                    .Execute();
            }
            catch (GatewayException e) {
                exceptionCaught = true;
                Assert.AreEqual("40005", e.ResponseMessage);
                Assert.AreEqual("Status Code: BadRequest - Request expects the following field type", e.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void CreatePayByLink_MissingUsageMode() {
            payByLink.UsageMode = null;

            var exceptionCaught = false;
            try {
                PayByLinkService.Create(payByLink, AMOUNT)
                    .WithCurrency(CURRENCY)
                    .WithClientTransactionId(GenerationUtils.GenerateRecurringKey())
                    .WithDescription("March and April Invoice")
                    .Execute();
            }
            catch (GatewayException e) {
                exceptionCaught = true;
                Assert.AreEqual("40005", e.ResponseMessage);
                Assert.AreEqual("Status Code: BadRequest - Request expects the following field usage_mode", e.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void CreatePayByLink_MissingPaymentMethods() {
            payByLink.AllowedPaymentMethods = null;

            var exceptionCaught = false;
            try {
                PayByLinkService.Create(payByLink, AMOUNT)
                    .WithCurrency(CURRENCY)
                    .WithClientTransactionId(GenerationUtils.GenerateRecurringKey())
                    .WithDescription("March and April Invoice")
                    .Execute();
            }
            catch (GatewayException e) {
                exceptionCaught = true;
                Assert.AreEqual("40005", e.ResponseMessage);
                Assert.AreEqual("Status Code: BadRequest - Request expects the following field transactions.allowed_payment_methods",
                    e.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void CreatePayByLink_MissingName() {
            payByLink.Name = null;

            var exceptionCaught = false;
            try {
                PayByLinkService.Create(payByLink, AMOUNT)
                    .WithCurrency(CURRENCY)
                    .WithClientTransactionId(GenerationUtils.GenerateRecurringKey())
                    .WithDescription("March and April Invoice")
                    .Execute();
            }
            catch (GatewayException e) {
                exceptionCaught = true;
                Assert.AreEqual("40005", e.ResponseMessage);
                Assert.AreEqual("Status Code: BadRequest - Request expects the following field name", e.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void CreatePayByLink_MissingDescription() {
            var exceptionCaught = false;
            try {
                PayByLinkService.Create(payByLink, AMOUNT)
                    .WithCurrency(CURRENCY)
                    .WithClientTransactionId(GenerationUtils.GenerateRecurringKey())
                    .Execute();
            }
            catch (GatewayException e) {
                exceptionCaught = true;
                Assert.AreEqual("40005", e.ResponseMessage);
                Assert.AreEqual("Status Code: BadRequest - Request expects the following field description", e.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void CreatePayByLink_MissingCurrency() {
            var exceptionCaught = false;
            try {
                PayByLinkService.Create(payByLink, AMOUNT)
                    .WithDescription("March and April Invoice")
                    .Execute();
            }
            catch (GatewayException e) {
                exceptionCaught = true;
                Assert.AreEqual("40005", e.ResponseMessage);
                Assert.AreEqual("Status Code: BadRequest - Request expects the following field transactions.currency",
                    e.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void EditPayByLink_MissingType() {
            payByLink.Type = null;

            var exceptionCaught = false;
            try {
                PayByLinkService.Edit(PayByLinkId)
                    .WithAmount(AMOUNT)
                    .WithPayByLinkData(payByLink)
                    .WithDescription("Update PayByLink description")
                    .Execute();
            }
            catch (BuilderException e) {
                exceptionCaught = true;
                Assert.AreEqual("Type cannot be null for this transaction type.", e.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void EditPayByLink_MissingUsageMode() {
            payByLink.UsageMode = null;

            var exceptionCaught = false;
            try {
                PayByLinkService.Edit(PayByLinkId)
                    .WithAmount(AMOUNT)
                    .WithPayByLinkData(payByLink)
                    .WithDescription("Update PayByLink description")
                    .Execute();
            }
            catch (BuilderException e) {
                exceptionCaught = true;
                Assert.AreEqual("UsageMode cannot be null for this transaction type.", e.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void EditPayByLink_MissingName() {
            payByLink.Name = null;

            var exceptionCaught = false;
            try {
                PayByLinkService.Edit(PayByLinkId)
                    .WithAmount(AMOUNT)
                    .WithPayByLinkData(payByLink)
                    .WithDescription("Update PayByLink description")
                    .Execute();
            }
            catch (GatewayException e) {
                exceptionCaught = true;
                Assert.AreEqual("Status Code: BadRequest - Request expects the following field name", e.Message);
                Assert.AreEqual("40005", e.ResponseMessage);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void EditPayByLink_MissingUsageLimit() {
            payByLink.UsageLimit = null;

            var exceptionCaught = false;
            try {
                PayByLinkService.Edit(PayByLinkId)
                    .WithAmount(AMOUNT)
                    .WithPayByLinkData(payByLink)
                    .WithDescription("Update PayByLink description")
                    .Execute();
            }
            catch (BuilderException e) {
                exceptionCaught = true;
                Assert.AreEqual("UsageLimit cannot be null for this transaction type.", e.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void EditPayByLink_MissingDescription() {
            var exceptionCaught = false;
            try {                

                PayByLinkService.Edit(PayByLinkId)
                    .WithAmount(AMOUNT)
                    .WithPayByLinkData(payByLink)
                    .Execute();
            }
            catch (GatewayException e) {
                exceptionCaught = true;
                Assert.AreEqual("Status Code: BadRequest - Request expects the following field description", e.Message);
                Assert.AreEqual("40005", e.ResponseMessage);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void EditPayByLink_MissingAmount() {
            var exceptionCaught = false;
            try {
                PayByLinkService.Edit(PayByLinkId)
                    .WithAmount(null)
                    .WithPayByLinkData(payByLink)
                    .WithDescription("Update PayByLink description")
                    .Execute();
            }
            catch (BuilderException e) {
                exceptionCaught = true;
                Assert.AreEqual("Amount cannot be null for this transaction type.", e.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void EditPayByLink_MissingPayByLinkData() {
            var exceptionCaught = false;
            try {
                PayByLinkService.Edit(PayByLinkId)
                    .WithAmount(AMOUNT)
                    .WithPayByLinkData(null)
                    .WithDescription("Update PayByLink description")
                    .Execute();
            }
            catch (BuilderException e) {
                exceptionCaught = true;
                Assert.AreEqual("PayByLinkData cannot be null for this transaction type.", e.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void EditPayByLink_RandomPayByLinkId() {
            var exceptionCaught = false;
            try {
                PayByLinkService.Edit(Guid.NewGuid().ToString())
                    .WithAmount(AMOUNT)
                    .WithPayByLinkData(payByLink)
                    .WithDescription("Update PayByLink description")
                    .Execute();
            }
            catch (GatewayException e) {
                exceptionCaught = true;
                Assert.AreEqual("40108", e.ResponseMessage);
                Assert.AreEqual("Status Code: BadRequest - You cannot update a link that has a 400 status", e.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void FindPayByLink_ByStatus() {
            var response = PayByLinkService.FindPayByLink(1, 10)
                .OrderBy(PayByLinkSortProperty.TimeCreated, SortDirection.Ascending)
                .Where(SearchCriteria.StartDate, StartDate)
                .And(SearchCriteria.EndDate, EndDate)
                .And(SearchCriteria.PayByLinkStatus, PayByLinkStatus.EXPIRED)
                .Execute();

            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Results);
            /** @var PayByLinkSummary $randomPayByLink */
            var randomPayByLinkResponseObject = response.Results[0];
            Assert.IsNotNull(randomPayByLinkResponseObject);
            Assert.IsInstanceOfType(randomPayByLinkResponseObject, typeof(PayByLinkSummary));
            Assert.AreEqual(PayByLinkStatus.EXPIRED, randomPayByLinkResponseObject.Status);
        }

        [TestMethod]
        public void FindPayByLink_ByUsageModeAndName() {
            const string name = "iphone 14";

            var response = PayByLinkService.FindPayByLink(1, 10)
                .OrderBy(PayByLinkSortProperty.TimeCreated, SortDirection.Ascending)
                .Where(SearchCriteria.StartDate, StartDate)
                .And(SearchCriteria.EndDate, EndDate)
                .And(SearchCriteria.PaymentMethodUsageMode, PaymentMethodUsageMode.Single)
                .And(SearchCriteria.DisplayName, name)
                .Execute();

            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Results);
            /** @var PayByLinkSummary $randomPayByLink */
            var randomPayByLinkResponseObject = response.Results[0];
            Assert.IsNotNull(randomPayByLinkResponseObject);
            Assert.IsInstanceOfType(randomPayByLinkResponseObject, typeof(PayByLinkSummary));
            Assert.AreEqual(PaymentMethodUsageMode.Single, randomPayByLinkResponseObject.UsageMode);
            Assert.AreEqual(name, randomPayByLinkResponseObject.Name);
        }

        [TestMethod]
        public void FindPayByLink_ByAmount() {
            const decimal amount = 10.01m;

            var response = PayByLinkService.FindPayByLink(1, 10)
                .OrderBy(PayByLinkSortProperty.TimeCreated, SortDirection.Ascending)
                .Where(SearchCriteria.StartDate, StartDate)
                .And(SearchCriteria.EndDate, EndDate)
                .And(DataServiceCriteria.Amount, amount)
                .Execute();

            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Results);
            /** @var PayByLinkSummary $randomPayByLink */
            var randomPayByLinkResponseObject = response.Results[0];
            Assert.IsNotNull(randomPayByLinkResponseObject);
            Assert.IsInstanceOfType(randomPayByLinkResponseObject, typeof(PayByLinkSummary));
            Assert.AreEqual(amount, randomPayByLinkResponseObject.Amount);
        }

        [TestMethod]
        public void FindPayByLink_ByExpirationDate() {
            var expirationDate = new DateTime(2024, 05, 09);

            var response = PayByLinkService.FindPayByLink(1, 10)
                .OrderBy(PayByLinkSortProperty.TimeCreated, SortDirection.Ascending)
                .Where(SearchCriteria.StartDate, StartDate)
                .And(SearchCriteria.EndDate, EndDate)
                .And(SearchCriteria.ExpirationDate, expirationDate)
                .Execute();

            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Results);
            /** @var PayByLinkSummary $randomPayByLink */
            var randomPayByLinkResponseObject = response.Results[0];
            Assert.IsNotNull(randomPayByLinkResponseObject);
            Assert.IsInstanceOfType(randomPayByLinkResponseObject, typeof(PayByLinkSummary));
        }
        
        private void AssertTransactionResponse(Transaction transaction) {
            Assert.AreEqual("SUCCESS", transaction.ResponseCode);
            Assert.AreEqual(PayByLinkStatus.ACTIVE.ToString().ToUpper(), transaction.ResponseMessage.ToUpper());
            Assert.AreEqual(AMOUNT, transaction.BalanceAmount);
            Assert.IsNotNull(transaction.PayByLinkResponse.Url);
            Assert.IsNotNull(transaction.PayByLinkResponse.Id);
        }

        private static GpApiConfig SetupTransactionConfig() {
            return GpApiConfigSetup(AppId, AppKey, Channel.CardNotPresent);
        }
    }
}