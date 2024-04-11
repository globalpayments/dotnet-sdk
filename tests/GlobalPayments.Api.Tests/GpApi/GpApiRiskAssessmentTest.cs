using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Enums;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Utils.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Tests.GpApi
{
    [TestClass]
    public class GpApiRiskAssessmentTest : BaseGpApiTests
    {
        private CreditCardData card;
        private Address shippingAddress;
        private BrowserData browserData;

        private const string Currency = "GBP";
        private static readonly decimal Amount = new decimal(10.01);

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            var gpApiConfig = GpApiConfigSetup(AppId, AppKey, Channel.CardNotPresent);
            ServicesContainer.ConfigureService(gpApiConfig);
        }

        public GpApiRiskAssessmentTest()
        {
            // Create card data
            card = new CreditCardData
            {
                ExpMonth = ExpMonth,
                ExpYear = ExpYear,
                CardHolderName = "John Smith",
                Number = "4012001038488884"
            };

            // Shipping address
            shippingAddress = new Address
            {
                StreetAddress1 = "Apartment 852",
                StreetAddress2 = "Complex 741",
                StreetAddress3 = "no",
                City = "Chicago",
                PostalCode = "5001",
                State = "IL",
                CountryCode = "840"
            };

            // Browser data
            browserData = new BrowserData
            {
                AcceptHeader = "text/html,application/xhtml+xml,application/xml;q=9,image/webp,img/apng,*/*;q=0.8",
                ColorDepth = ColorDepth.TWENTY_FOUR_BITS,
                IpAddress = "123.123.123.123",
                JavaEnabled = true,
                Language = "en",
                ChallengeWindowSize = ChallengeWindowSize.WINDOWED_600X400,
                Timezone = "0",
                UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64, x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.110 Safari/537.36"
            };
        }

        [TestMethod]
        public void TransactionRiskAnalysisBasicOption()
        {
            var response = FraudService.RiskAssess(card)
                .WithAmount(Amount)
                .WithCurrency(Currency)
                .WithAuthenticationSource(AuthenticationSource.BROWSER)
                .WithBrowserData(browserData)
                .Execute();

            Assert.AreEqual("SUCCESS", response.ResponseCode);
            Assert.AreEqual(RiskAssessmentStatus.ACCEPTED, response.Status);

        }

        [TestMethod]
        public void RiskAssessment()
        {
            var idempotencyKey = Guid.NewGuid().ToString();
            var response = FraudService.RiskAssess(card)
                .WithAmount(Amount)
                .WithCurrency(Currency)
                .WithAuthenticationSource(AuthenticationSource.BROWSER)
                .WithOrderCreateDate(DateTime.Now)
                .WithReferenceNumber("my_EOS_risk_assessment")
                .WithAddressMatchIndicator(false)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithGiftCardAmount(2)
                .WithGiftCardCount(1)
                .WithGiftCardCurrency(Currency)
                .WithDeliveryEmail("james.mason@example.com")
                .WithDeliveryTimeFrame(DeliveryTimeFrame.SAME_DAY)
                .WithShippingMethod(ShippingMethod.ANOTHER_VERIFIED_ADDRESS)
                .WithShippingNameMatchesCardHolderName(false)
                .WithPreOrderIndicator(PreOrderIndicator.FUTURE_AVAILABILITY)
                .WithPreOrderAvailabilityDate(DateTime.Parse("2019-04-18"))
                .WithReorderIndicator(ReorderIndicator.REORDER)
                .WithOrderTransactionType(OrderTransactionType.GOODS_SERVICE_PURCHASE)
                .WithCustomerAccountId("6dcb24f5-74a0-4da3-98da-4f0aa0e88db3")
                .WithAccountAgeIndicator(AgeIndicator.LESS_THAN_THIRTY_DAYS)
                .WithAccountCreateDate(DateTime.Parse("2019-01-10"))
                .WithAccountChangeDate(DateTime.Parse("2019-01-28"))
                .WithAccountChangeIndicator(AgeIndicator.THIS_TRANSACTION)
                .WithPasswordChangeDate(DateTime.Parse("2019-01-15"))
                .WithPasswordChangeIndicator(AgeIndicator.LESS_THAN_THIRTY_DAYS)
                .WithPhoneNumber("44", "123456789", PhoneNumberType.Home)
                .WithPhoneNumber("44", "1801555888", PhoneNumberType.Work)
                .WithPaymentAccountCreateDate(DateTime.Now)
                .WithPaymentAccountAgeIndicator(AgeIndicator.LESS_THAN_THIRTY_DAYS)
                .WithPreviousSuspiciousActivity(false)
                .WithNumberOfPurchasesInLastSixMonths(3)
                .WithNumberOfTransactionsInLast24Hours(1)
                .WithNumberOfTransactionsInLastYear(5)
                .WithNumberOfAddCardAttemptsInLast24Hours(1)
                .WithShippingAddressCreateDate(DateTime.Now)
                .WithShippingAddressUsageIndicator(AgeIndicator.THIS_TRANSACTION)
                .WithPriorAuthenticationMethod(PriorAuthenticationMethod.FRICTIONLESS_AUTHENTICATION)
                .WithPriorAuthenticationTransactionId(Guid.NewGuid().ToString())
                .WithPriorAuthenticationTimestamp(DateTime.Parse("2022-10-10T16:41:33.333Z"))
                .WithPriorAuthenticationData("secret123")
                .WithMaxNumberOfInstallments(5)
                .WithRecurringAuthorizationFrequency(25)
                .WithRecurringAuthorizationExpiryDate(DateTime.Now)
                .WithCustomerAuthenticationData("secret123")
                .WithCustomerAuthenticationTimestamp(DateTime.Parse("2022-10-10T16:41:33"))
                .WithCustomerAuthenticationMethod(CustomerAuthenticationMethod.MERCHANT_SYSTEM_AUTHENTICATION)
                .WithBrowserData(browserData)
                .WithIdempotencyKey(idempotencyKey)
                .Execute();

            Assert.AreEqual("SUCCESS", response.ResponseCode);
            Assert.AreEqual(RiskAssessmentStatus.ACCEPTED, response.Status);
        }

        [TestMethod]
        public void TransactionRiskAnalysis_AllSources()
        {
            var source = new List<AuthenticationSource>() { AuthenticationSource.BROWSER, AuthenticationSource.MERCHANT_INITIATED, AuthenticationSource.MOBILE_SDK };
            foreach (var item in source)
            {

                var response = FraudService.RiskAssess(card)
                    .WithAmount(Amount)
                    .WithCurrency(Currency)
                    .WithAuthenticationSource(item)
                    .WithBrowserData(browserData)
                    .Execute();

                Assert.AreEqual("SUCCESS", response.ResponseCode);
                Assert.AreEqual(RiskAssessmentStatus.ACCEPTED, response.Status);
                Assert.IsTrue(response.Id.StartsWith("RAS_"));
            }
        }

        [TestMethod]
        public void TransactionRiskAnalysis_AllDeliveryTimeFrames()
        {
            foreach (DeliveryTimeFrame value in (DeliveryTimeFrame[])Enum.GetValues(typeof(DeliveryTimeFrame)))
            {
                var response = FraudService.RiskAssess(card)
                    .WithAmount(Amount)
                    .WithCurrency(Currency)
                    .WithAuthenticationSource(AuthenticationSource.BROWSER)
                    .WithBrowserData(browserData)
                    .WithDeliveryTimeFrame(value)
                    .Execute();

                Assert.AreEqual("SUCCESS", response.ResponseCode);
                Assert.AreEqual(RiskAssessmentStatus.ACCEPTED, response.Status);
                Assert.IsTrue(response.Id.StartsWith("RAS_"));
            }
        }

        [TestMethod]
        public void TransactionRiskAnalysis_AllShippingMethods()
        {
            foreach (ShippingMethod value in (ShippingMethod[])Enum.GetValues(typeof(ShippingMethod)))
            {
                var response = FraudService.RiskAssess(card)
                    .WithAmount(Amount)
                    .WithCurrency(Currency)
                    .WithAuthenticationSource(AuthenticationSource.BROWSER)
                    .WithBrowserData(browserData)
                    .WithShippingMethod(value)
                    .Execute();

                Assert.AreEqual("SUCCESS", response.ResponseCode);
                Assert.AreEqual(RiskAssessmentStatus.ACCEPTED, response.Status);
                Assert.IsTrue(response.Id.StartsWith("RAS_"));
            }
        }

        [TestMethod]
        public void TransactionRiskAnalysis_AllOrderTransactionTypes()
        {
            foreach (OrderTransactionType value in (OrderTransactionType[])Enum.GetValues(typeof(OrderTransactionType)))
            {
                var response = FraudService.RiskAssess(card)
                    .WithAmount(Amount)
                    .WithCurrency(Currency)
                    .WithAuthenticationSource(AuthenticationSource.BROWSER)
                    .WithBrowserData(browserData)
                    .WithOrderTransactionType(value)
                    .Execute();

                Assert.AreEqual("SUCCESS", response.ResponseCode);
                Assert.AreEqual(RiskAssessmentStatus.ACCEPTED, response.Status);
                Assert.IsTrue(response.Id.StartsWith("RAS_"));
            }
        }

        [TestMethod]
        public void TransactionRiskAnalysis_AllPriorAuthenticationMethods()
        {
            foreach (PriorAuthenticationMethod value in (PriorAuthenticationMethod[])Enum.GetValues(typeof(PriorAuthenticationMethod)))
            {
                var response = FraudService.RiskAssess(card)
                    .WithAmount(Amount)
                    .WithCurrency(Currency)
                    .WithAuthenticationSource(AuthenticationSource.BROWSER)
                    .WithBrowserData(browserData)
                    .WithPriorAuthenticationMethod(value)
                    .Execute();

                Assert.AreEqual("SUCCESS", response.ResponseCode);
                Assert.AreEqual(RiskAssessmentStatus.ACCEPTED, response.Status);
                Assert.IsTrue(response.Id.StartsWith("RAS_"));
            }
        }

        [TestMethod]
        public void TransactionRiskAnalysis_AllCustomerAuthenticationMethods()
        {
            foreach (CustomerAuthenticationMethod value in (CustomerAuthenticationMethod[])Enum.GetValues(typeof(CustomerAuthenticationMethod)))
            {
                var response = FraudService.RiskAssess(card)
                    .WithAmount(Amount)
                    .WithCurrency(Currency)
                    .WithAuthenticationSource(AuthenticationSource.BROWSER)
                    .WithBrowserData(browserData)
                    .WithCustomerAuthenticationMethod(value)
                    .Execute();

                Assert.AreEqual("SUCCESS", response.ResponseCode);
                Assert.AreEqual(RiskAssessmentStatus.ACCEPTED, response.Status);
                Assert.AreEqual("Apply Exemption", response.ResponseMessage);
                Assert.IsTrue(response.Id.StartsWith("RAS_"));

            }
        }

        [TestMethod]
        public void TransactionRiskAnalysis_MissingAmount()
        {
            var errorFound = false;
            try
            {
                FraudService.RiskAssess(card)
                    .WithCurrency(Currency)
                    .WithAuthenticationSource(AuthenticationSource.BROWSER)
                    .WithBrowserData(browserData)
                    .Execute();
            }
            catch (GatewayException e)
            {
                errorFound = true;
                Assert.AreEqual("Status Code: BadRequest - Request expects the following field order.amount", e.Message);
                Assert.AreEqual("40005", e.ResponseMessage);
            }
            finally
            {
                Assert.IsTrue(errorFound);
            }
        }

        [TestMethod]
        public void TransactionRiskAnalysis_MissingCurrency()
        {
        var errorFound = false;
            try
            {
                FraudService.RiskAssess(card)
                    .WithAmount(Amount)
                    .WithAuthenticationSource(AuthenticationSource.BROWSER)
                    .WithBrowserData(browserData)
                    .Execute();
            }
            catch (GatewayException e)
            {
                errorFound = true;
                Assert.AreEqual("Status Code: BadRequest - Request expects the following field order.currency", e.Message);
                Assert.AreEqual("40005", e.ResponseMessage);
            }
            finally
            {
                Assert.IsTrue(errorFound);
            }
        }

        [TestMethod]
        public void TransactionRiskAnalysis_MissingSource()
        {
        var errorFound = false;
            try
            {
                FraudService.RiskAssess(card)
                    .WithAmount(Amount)
                    .WithCurrency(Currency)
                    .WithBrowserData(browserData)
                    .Execute();
            }
            catch (GatewayException e)
            {
                errorFound = true;
                Assert.AreEqual("Status Code: BadRequest - Request expects the following field source", e.Message);
                Assert.AreEqual("40005", e.ResponseMessage);
            }
            finally
            {
                Assert.IsTrue(errorFound);
            }
        }

        [TestMethod]
        public void TransactionRiskAnalysis_MissingBrowserData()
        {
        var errorFound = false;
            try
            {
                FraudService.RiskAssess(card)
                    .WithAmount(Amount)
                    .WithCurrency(Currency)
                    .WithAuthenticationSource(AuthenticationSource.BROWSER)
                    .Execute();
            }
            catch (GatewayException e)
            {
                errorFound = true;
                Assert.AreEqual("Status Code: BadRequest - Request expects the following field browser_data.accept_header", e.Message);
                Assert.AreEqual("40005", e.ResponseMessage);
            }
            finally
            {
                Assert.IsTrue(errorFound);
            }           
        }

        [TestMethod]
        public void TransactionRiskAnalysis_MissingCard()
        {
        var errorFound = false;
            try
            {
                FraudService.RiskAssess(new CreditCardData())
                    .WithAmount(Amount)
                    .WithCurrency(Currency)
                    .WithAuthenticationSource(AuthenticationSource.BROWSER)
                    .WithBrowserData(browserData)
                    .Execute();
            }
            catch (GatewayException e)
            {
                errorFound = true;
                Assert.AreEqual("Status Code: BadRequest - Request contains unexpected data payment_method.card.brand", e.Message);
                Assert.AreEqual("40006", e.ResponseMessage);
            }
            finally
            {
                Assert.IsTrue(errorFound);
            }            
        }
    }
}
