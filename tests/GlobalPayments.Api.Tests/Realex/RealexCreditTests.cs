using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests {
    [TestClass]
    public class RealexCreditTests {
        CreditCardData card;

        [TestInitialize]
        public void Init() {
            ServicesContainer.ConfigureService(new GatewayConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RebatePassword = "rebate",
                RefundPassword = "refund",
                ServiceUrl = "https://api.sandbox.realexpayments.com/epage-remote.cgi"
            });

            card = new CreditCardData {
                Number = "4111111111111111",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvn = "123",
                CardHolderName = "Joe Smith"
            };
        }

        [TestMethod]
        public void CreditAuthorization() {
            var authorization = card.Authorize(14m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(authorization);
            Assert.AreEqual("00", authorization.ResponseCode, authorization.ResponseMessage);

            var capture = authorization.Capture(14m)
                .Execute();
            Assert.IsNotNull(capture);
            Assert.AreEqual("00", capture.ResponseCode, capture.ResponseMessage);
        }

        [TestMethod]
        public void CreditSale() {
            var response = card.Charge(15m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
        }

        [TestMethod]
        public void CreditSaleWithRecurring() {
            var response = card.Charge(15m)
                .WithCurrency("USD")
                .WithRecurringInfo(RecurringType.Fixed, RecurringSequence.First)
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
        }

        [TestMethod]
        public void CreditRefund() {
            var response = card.Refund(16m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
        }

        [TestMethod]
        public void CreditRebate() {
            var response = card.Charge(17m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);

            var rebate = response.Refund(17m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(rebate);
            Assert.AreEqual("00", rebate.ResponseCode, rebate.ResponseMessage);
        }

        [TestMethod]
        public void CreditVoid() {
            var response = card.Charge(15m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);

            var voidResponse = response.Void().Execute();
            Assert.IsNotNull(voidResponse);
            Assert.AreEqual("00", voidResponse.ResponseCode, voidResponse.ResponseMessage);
        }

        [TestMethod]
        public void CreditVerify() {
            var response = card.Verify()
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
        }

        [TestMethod]
        public void CreditFraudResponse() {
            var billingAddress = new Address();
            billingAddress.StreetAddress1 = "Flat 123";
            billingAddress.StreetAddress2 = "House 456";
            billingAddress.StreetAddress3 = "Cul-De-Sac";
            billingAddress.City = "Halifax";
            billingAddress.Province = "West Yorkshire";
            billingAddress.State = "Yorkshire and the Humber";
            billingAddress.Country = "GB";
            billingAddress.PostalCode = "E77 4QJ";

            var shippingAddress = new Address();
            shippingAddress.StreetAddress1 = "House 456";
            shippingAddress.StreetAddress2 = "987 The Street";
            shippingAddress.StreetAddress3 = "Basement Flat";
            shippingAddress.City = "Chicago";
            shippingAddress.State = "Illinois";
            shippingAddress.Province = "Mid West";
            shippingAddress.Country = "US";
            shippingAddress.PostalCode = "50001";

            var fraudResponse = card.Charge(199.99m)
                .WithCurrency("EUR")
                .WithAddress(billingAddress, AddressType.Billing)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithProductId("SID9838383")
                .WithClientTransactionId("Car Part HV")
                .WithCustomerId("E8953893489")
                .WithCustomerIpAddress("123.123.123.123")
                .Execute();
            Assert.IsNotNull(fraudResponse);
            Assert.AreEqual("00", fraudResponse.ResponseCode, fraudResponse.ResponseMessage);
        }
    }
}
