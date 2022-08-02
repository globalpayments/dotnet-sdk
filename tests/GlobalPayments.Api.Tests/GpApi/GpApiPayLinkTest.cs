using System;
using System.Threading;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Enums;
using GlobalPayments.Api.Entities.Reporting;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Utils;
using GlobalPayments.Api.Utils.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Environment = GlobalPayments.Api.Entities.Environment;

namespace GlobalPayments.Api.Tests.GpApi {
    
    [TestClass]
    public class GpApiPayLinkTest : BaseGpApiTests {
        
        private PayLinkData payLink;
        private CreditCardData card;
        private Address shippingAddress;
        private BrowserData browserData;

        private const decimal AMOUNT = 7.8m;
        private const string CURRENCY = "GBP";

        [TestInitialize]
        public void TestInitialize() {
            ServicesContainer.ConfigureService(new GpApiConfig {
                AppId = "v2yRaFOLwFaQc0fSZTCyAdQCBNByGpVK",
                AppKey = "oKZpWitk6tORoCVT",
                Channel = Channel.CardNotPresent,
                Environment = Environment.TEST,
                Country = "GB",
                AccessTokenInfo = new AccessTokenInfo {
                    TransactionProcessingAccountName = "LinkManagement"
                },
                RequestLogger = new RequestConsoleLogger(),
                EnableLogging = true
            });

            payLink = new PayLinkData {
                Type = PayLinkType.PAYMENT,
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
                ExpMonth = expMonth,
                ExpYear = expYear,
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
        }

        [TestMethod]
        public void ReportPayLinkDetail() {
            var paylinkId = "LNK_hUh2IIO1YoyDU3wGwkcb4e6SE9v5dY";

            var response = PayLinkService.PayLinkDetail(paylinkId)
                .Execute();

            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(PayLinkSummary));
            Assert.AreEqual(paylinkId, response.Id);
        }

        [TestMethod]
        public void ReportPayLinkDetail_RandomLinkId() {
            var paylinkId = Guid.NewGuid().ToString();

            var exceptionCaught = false;
            try {
                PayLinkService.PayLinkDetail(paylinkId)
                    .Execute();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("RESOURCE_NOT_FOUND", ex.ResponseCode);
                Assert.AreEqual("40118", ex.ResponseMessage);
                Assert.AreEqual($"Status Code: NotFound - Links {paylinkId} not found at this /ucp/links/{paylinkId}",
                    ex.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void ReportPayLinkDetail_NullLinkId() {
            var exceptionCaught = false;
            try {
                PayLinkService.PayLinkDetail(null)
                    .Execute();
            }
            catch (BuilderException ex) {
                exceptionCaught = true;
                Assert.AreEqual("PayLinkId cannot be null for this transaction type.",
                    ex.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void FindPayLinkByDate() {
            var response = PayLinkService.FindPayLink(1, 10)
                .OrderBy(PayLinkSortProperty.TimeCreated, SortDirection.Ascending)
                .Where(SearchCriteria.StartDate, startDate)
                .And(SearchCriteria.EndDate, endDate)
                .Execute();

            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Results);
            /** @var PayLinkSummary $randomPayLink */
            var randomPayLink = response.Results[0];
            Assert.IsNotNull(randomPayLink);
            Assert.IsInstanceOfType(randomPayLink, typeof(PayLinkSummary));
        }

        [TestMethod]
        public void FindPayLinkByDate_NoResults() {
            var response = PayLinkService.FindPayLink(1, 10)
                .OrderBy(PayLinkSortProperty.TimeCreated, SortDirection.Ascending)
                .Where(SearchCriteria.StartDate, startDate.AddYears(-1))
                .And(SearchCriteria.EndDate, endDate.AddYears(-1))
                .Execute();

            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Results);
            Assert.AreEqual(0, response.Results.Count);
            Assert.AreEqual(0, response.TotalRecordCount);
        }

        [TestMethod]
        public void CreatePayLink() {
            payLink.Type = PayLinkType.PAYMENT;
            payLink.UsageMode = PaymentMethodUsageMode.Single;
            payLink.AllowedPaymentMethods = new PaymentMethodName[] { PaymentMethodName.Card };
            payLink.UsageLimit = 1;
            payLink.Name = "Mobile Bill Payment";
            payLink.IsShippable = true;
            payLink.ShippingAmount = 1.23m;
            payLink.ExpirationDate = DateTime.UtcNow.AddDays(10); //date('Y-m-d H:i:s') + 10;
            payLink.Images = new string[] { "test", "test2", "test3" };
            payLink.ReturnUrl = "https://www.example.com/returnUrl";
            payLink.StatusUpdateUrl = "https://www.example.com/statusUrl";
            payLink.CancelUrl = "https://www.example.com/returnUrl";

            var response = PayLinkService.Create(payLink, AMOUNT)
                .WithCurrency(CURRENCY)
                .WithClientTransactionId(GenerationUtils.GenerateRecurringKey())
                .WithDescription("March and April Invoice")
                .Execute();

            Assert.AreEqual("SUCCESS", response.ResponseCode);
            Assert.AreEqual(PayLinkStatus.ACTIVE.ToString().ToUpper(), response.ResponseMessage.ToUpper());
            Assert.AreEqual(AMOUNT, response.BalanceAmount);
            Assert.IsNotNull(response.PayLinkResponse.Url);
            Assert.IsNotNull(response.PayLinkResponse.Id);
        }

        [TestMethod]
        public void CreatePayLink_MultipleUsage() {
            payLink.UsageMode = PaymentMethodUsageMode.Multiple;
            payLink.UsageLimit = 2;

            var response = PayLinkService.Create(payLink, AMOUNT)
                .WithCurrency(CURRENCY)
                .WithClientTransactionId(GenerationUtils.GenerateRecurringKey())
                .WithDescription("March and April Invoice")
                .Execute();

            AssertTransactionResponse(response);
            Assert.AreEqual(2, response.PayLinkResponse.UsageLimit);
        }

        [TestMethod]
        public void CreatePayLink_ThenCharge() {
            string[] imagesList = {
                "One", "Two", "Three"
            };
            payLink.Images = imagesList;

            var response = PayLinkService.Create(payLink, AMOUNT)
                .WithCurrency(CURRENCY)
                .WithClientTransactionId(GenerationUtils.GenerateRecurringKey())
                .WithDescription("March and April Invoice")
                .Execute();

            AssertTransactionResponse(response);

            ServicesContainer.ConfigureService(SetupTransactionConfig(), "createTransaction");

            var charge = card.Charge(AMOUNT)
                .WithCurrency(CURRENCY)
                .WithPaymentLinkId(response.PayLinkResponse.Id)
                .Execute("createTransaction");

            Assert.IsNotNull(charge);
            Assert.AreEqual(SUCCESS, charge?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), charge?.ResponseMessage);

            Thread.Sleep(2000);

            var getPayLinkById = PayLinkService.PayLinkDetail(response.PayLinkResponse.Id)
                .Execute();

            Assert.IsNotNull(getPayLinkById);
            Assert.IsInstanceOfType(getPayLinkById, typeof(PayLinkSummary));
            Assert.AreEqual(response.PayLinkResponse.Id, getPayLinkById.Id);
            Assert.AreEqual(1, getPayLinkById.Transactions.Count);
        }

        [TestMethod]
        public void CreatePayLink_ThenCharge_DifferentAmount() {
            var response = PayLinkService.Create(payLink, AMOUNT)
                .WithCurrency(CURRENCY)
                .WithClientTransactionId(GenerationUtils.GenerateRecurringKey())
                .WithDescription("March and April Invoice")
                .Execute();

            AssertTransactionResponse(response);

            ServicesContainer.ConfigureService(SetupTransactionConfig(), "createTransaction");

            var charge = card.Charge(AMOUNT + 2)
                .WithCurrency(CURRENCY)
                .WithPaymentLinkId(response.PayLinkResponse.Id)
                .Execute("createTransaction");

            Assert.IsNotNull(charge);
            Assert.AreEqual(SUCCESS, charge?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), charge?.ResponseMessage);

            Thread.Sleep(2000);

            var getPayLinkById = PayLinkService.PayLinkDetail(response.PayLinkResponse.Id)
                .Execute();

            Assert.IsNotNull(getPayLinkById);
            Assert.IsInstanceOfType(getPayLinkById, typeof(PayLinkSummary));
            Assert.AreEqual(response.PayLinkResponse.Id, getPayLinkById.Id);
            Assert.AreEqual(1, getPayLinkById.Transactions.Count);
            Assert.AreEqual(AMOUNT + 2, getPayLinkById.Transactions[0].Amount);
        }

        [TestMethod]
        public void CreatePayLink_MultipleUsage_ThenCharge() {
            payLink.UsageMode = PaymentMethodUsageMode.Multiple;
            payLink.UsageLimit = 2;

            var response = PayLinkService.Create(payLink, AMOUNT)
                .WithCurrency(CURRENCY)
                .WithClientTransactionId(GenerationUtils.GenerateRecurringKey())
                .WithDescription("March and April Invoice")
                .Execute();

            AssertTransactionResponse(response);

            ServicesContainer.ConfigureService(SetupTransactionConfig(), "createTransaction");

            for (var i = 1; i <= payLink.UsageLimit; i++) {
                var charge = card.Charge(AMOUNT)
                    .WithCurrency(CURRENCY)
                    .WithPaymentLinkId(response.PayLinkResponse.Id)
                    .Execute("createTransaction");

                Assert.IsNotNull(charge);
                Assert.AreEqual(SUCCESS, charge?.ResponseCode);
                Assert.AreEqual(GetMapping(TransactionStatus.Captured), charge?.ResponseMessage);
            }

            Thread.Sleep(2000);

            var getPayLinkById = PayLinkService.PayLinkDetail(response.PayLinkResponse.Id)
                .Execute();

            Assert.IsNotNull(getPayLinkById);
            Assert.IsInstanceOfType(getPayLinkById, typeof(PayLinkSummary));
            Assert.AreEqual(response.PayLinkResponse.Id, getPayLinkById.Id);
            Assert.AreEqual(2, getPayLinkById.Transactions.Count);
        }

        [TestMethod]
        public void CreatePayLink_ThenAuthorizeAndCapture() {
            var response = PayLinkService.Create(payLink, AMOUNT)
                .WithCurrency(CURRENCY)
                .WithClientTransactionId(GenerationUtils.GenerateRecurringKey())
                .WithDescription("March and April Invoice")
                .Execute();

            AssertTransactionResponse(response);

            ServicesContainer.ConfigureService(SetupTransactionConfig(), "createTransaction");

            var authTransaction = card.Authorize(AMOUNT)
                .WithCurrency(CURRENCY)
                .WithPaymentLinkId(response.PayLinkResponse.Id)
                .Execute("createTransaction");

            Assert.IsNotNull(authTransaction);
            Assert.AreEqual(SUCCESS, authTransaction?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Preauthorized), authTransaction?.ResponseMessage);

            var chargeTransaction = authTransaction.Capture(AMOUNT)
                .WithCurrency(CURRENCY)
                .WithPaymentLinkId(response.PayLinkResponse.Id)
                .Execute("createTransaction");

            Assert.IsNotNull(chargeTransaction);
            Assert.AreEqual(SUCCESS, chargeTransaction?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), chargeTransaction?.ResponseMessage);

            Thread.Sleep(2000);

            var getPayLinkById = PayLinkService.PayLinkDetail(response.PayLinkResponse.Id)
                .Execute();

            Assert.IsNotNull(getPayLinkById);
            Assert.IsInstanceOfType(getPayLinkById, typeof(PayLinkSummary));
            Assert.AreEqual(response.PayLinkResponse.Id, getPayLinkById.Id);
            Assert.AreEqual(1, getPayLinkById.Transactions.Count);
            Assert.AreEqual(GetMapping(TransactionStatus.Preauthorized), getPayLinkById.Transactions[0].TransactionStatus);
        }

        [TestMethod]
        public void CreatePayLink_ThenCharge_WithTokenizedCard() {
            var response = PayLinkService.Create(payLink, AMOUNT)
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
                .WithPaymentLinkId(response.PayLinkResponse.Id)
                .Execute("createTransaction");

            Assert.IsNotNull(charge);
            Assert.AreEqual(SUCCESS, charge?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), charge?.ResponseMessage);

            Thread.Sleep(2000);

            var getPayLinkById = PayLinkService.PayLinkDetail(response.PayLinkResponse.Id)
                .Execute();

            Assert.IsNotNull(getPayLinkById);
            Assert.IsInstanceOfType(getPayLinkById, typeof(PayLinkSummary));
            Assert.AreEqual(response.PayLinkResponse.Id, getPayLinkById.Id);
            Assert.AreEqual(1, getPayLinkById.Transactions.Count);
        }

        [TestMethod]
        public void CreatePayLink_ThenCharge_With3DS() {
            var response = PayLinkService.Create(payLink, AMOUNT)
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
                .WithPaymentLinkId(response.PayLinkResponse.Id)
                .Execute("createTransaction");

            Assert.IsNotNull(charge);
            Assert.AreEqual(SUCCESS, charge?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), charge?.ResponseMessage);

            Thread.Sleep(2000);

            var getPayLinkById = PayLinkService.PayLinkDetail(response.PayLinkResponse.Id)
                .Execute();

            Assert.IsNotNull(getPayLinkById);
            Assert.IsInstanceOfType(getPayLinkById, typeof(PayLinkSummary));
            Assert.AreEqual(response.PayLinkResponse.Id, getPayLinkById.Id);
            Assert.AreEqual(1, getPayLinkById.Transactions.Count);
        }

        [TestMethod]
        public void EditPayLink() {
            var response = PayLinkService.FindPayLink(1, 10)
                .OrderBy(PayLinkSortProperty.TimeCreated, SortDirection.Ascending)
                .Where(SearchCriteria.StartDate, startDate)
                .And(SearchCriteria.EndDate, endDate)
                .Execute();

            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Results);
            /** @var PayLinkSummary $randomPayLink */
            var randomPayLink = response.Results[0];
            Assert.IsNotNull(randomPayLink);
            Assert.IsInstanceOfType(randomPayLink, typeof(PayLinkSummary));
            Assert.IsNotNull(randomPayLink.Id);

            var payLink = new PayLinkData();
            payLink.Name = "Test of Test";
            payLink.UsageMode = PaymentMethodUsageMode.Multiple;
            payLink.Type = PayLinkType.PAYMENT;
            payLink.UsageLimit = 5;
            //payLink.IsShippable = false;
            var amount = 10.08m;
            var editResponse = PayLinkService.Edit(randomPayLink.Id)
                .WithAmount(amount)
                .WithPayLinkData(payLink)
                .WithDescription("Update Paylink description")
                .Execute();

            Assert.AreEqual("SUCCESS", editResponse.ResponseCode);
            Assert.AreEqual(PayLinkStatus.ACTIVE.ToString(), editResponse.ResponseMessage);
            Assert.AreEqual(amount, editResponse.BalanceAmount);
            Assert.IsNotNull(editResponse.PayLinkResponse.Url);
            Assert.IsNotNull(editResponse.PayLinkResponse.Id);
        }

        [TestMethod]
        public void CreatePayLink_MissingType() {
            payLink.Type = null;

            var exceptionCaught = false;
            try {
                PayLinkService.Create(payLink, AMOUNT)
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
        public void CreatePayLink_MissingUsageMode() {
            payLink.UsageMode = null;

            var exceptionCaught = false;
            try {
                PayLinkService.Create(payLink, AMOUNT)
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
        public void CreatePayLink_MissingPaymentMethods() {
            payLink.AllowedPaymentMethods = null;

            var exceptionCaught = false;
            try {
                PayLinkService.Create(payLink, AMOUNT)
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
        public void CreatePayLink_MissingName() {
            payLink.Name = null;

            var exceptionCaught = false;
            try {
                PayLinkService.Create(payLink, AMOUNT)
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
        public void CreatePayLink_MissingShippable() {
            payLink.IsShippable = null;

            var response = PayLinkService.Create(payLink, AMOUNT)
                .WithCurrency(CURRENCY)
                .WithClientTransactionId(GenerationUtils.GenerateRecurringKey())
                .WithDescription("March and April Invoice")
                .Execute();

            AssertTransactionResponse(response);
            Assert.IsFalse(response.PayLinkResponse.IsShippable != null && response.PayLinkResponse.IsShippable.Value);
        }

        [TestMethod]
        public void CreatePayLink_MissingDescription() {
            var exceptionCaught = false;
            try {
                PayLinkService.Create(payLink, AMOUNT)
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
        public void CreatePayLink_MissingCurrency() {
            var exceptionCaught = false;
            try {
                PayLinkService.Create(payLink, AMOUNT)
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
        public void EditPayLink_MissingType() {
            payLink.Type = null;

            var exceptionCaught = false;
            try {
                PayLinkService.Edit(Guid.NewGuid().ToString())
                    .WithAmount(AMOUNT)
                    .WithPayLinkData(payLink)
                    .WithDescription("Update Paylink description")
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
        public void EditPayLink_MissingUsageMode() {
            payLink.UsageMode = null;

            var exceptionCaught = false;
            try {
                PayLinkService.Edit(Guid.NewGuid().ToString())
                    .WithAmount(AMOUNT)
                    .WithPayLinkData(payLink)
                    .WithDescription("Update Paylink description")
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
        public void EditPayLink_MissingName() {
            payLink.Name = null;

            var exceptionCaught = false;
            try {
                PayLinkService.Edit(Guid.NewGuid().ToString())
                    .WithAmount(AMOUNT)
                    .WithPayLinkData(payLink)
                    .WithDescription("Update Paylink description")
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
        public void EditPayLink_MissingUsageLimit() {
            payLink.UsageLimit = null;

            var exceptionCaught = false;
            try {
                PayLinkService.Edit(Guid.NewGuid().ToString())
                    .WithAmount(AMOUNT)
                    .WithPayLinkData(payLink)
                    .WithDescription("Update Paylink description")
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
        public void EditPayLink_MissingDescription() {
            var exceptionCaught = false;
            try {
                PayLinkService.Edit(Guid.NewGuid().ToString())
                    .WithAmount(AMOUNT)
                    .WithPayLinkData(payLink)
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
        public void EditPayLink_MissingAmount() {
            var exceptionCaught = false;
            try {
                PayLinkService.Edit(Guid.NewGuid().ToString())
                    .WithAmount(null)
                    .WithPayLinkData(payLink)
                    .WithDescription("Update Paylink description")
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
        public void EditPayLink_MissingPayLinkData() {
            var exceptionCaught = false;
            try {
                PayLinkService.Edit(Guid.NewGuid().ToString())
                    .WithAmount(AMOUNT)
                    .WithPayLinkData(null)
                    .WithDescription("Update Paylink description")
                    .Execute();
            }
            catch (BuilderException e) {
                exceptionCaught = true;
                Assert.AreEqual("PayLinkData cannot be null for this transaction type.", e.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void EditPayLink_RandomPayLinkId() {
            var exceptionCaught = false;
            try {
                PayLinkService.Edit(Guid.NewGuid().ToString())
                    .WithAmount(AMOUNT)
                    .WithPayLinkData(payLink)
                    .WithDescription("Update Paylink description")
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

        private void AssertTransactionResponse(Transaction transaction) {
            Assert.AreEqual("SUCCESS", transaction.ResponseCode);
            Assert.AreEqual(PayLinkStatus.ACTIVE.ToString().ToUpper(), transaction.ResponseMessage.ToUpper());
            Assert.AreEqual(AMOUNT, transaction.BalanceAmount);
            Assert.IsNotNull(transaction.PayLinkResponse.Url);
            Assert.IsNotNull(transaction.PayLinkResponse.Id);
        }

        private static GpApiConfig SetupTransactionConfig() {
            var config = new GpApiConfig {
                AppId = APP_ID,
                AppKey = APP_KEY,
                Channel = Channel.CardNotPresent,
                ChallengeNotificationUrl = "https://ensi808o85za.x.pipedream.net/",
                MethodNotificationUrl = "https://ensi808o85za.x.pipedream.net/",
                MerchantContactUrl = "https://enp4qhvjseljg.x.pipedream.net/",
                RequestLogger = new RequestConsoleLogger(),
                EnableLogging = true
            };

            return config;
        }
    }
}