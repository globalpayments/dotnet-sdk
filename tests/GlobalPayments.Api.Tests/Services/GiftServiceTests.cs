using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.Services {
    [TestClass]
    public class GiftServiceTests {
        GiftService service;
        GiftCard card;
        GiftCard replacement;

        public GiftServiceTests() {
            service = new GiftService(new PorticoConfig {
                SecretApiKey = "skapi_cert_MaePAQBr-1QAqjfckFC8FTbRTT120bVQUlfVOjgCBw"
            });

            card = new GiftCard { Number = "5022440000000000007" }; ;
            replacement = new GiftCard { TrackData = "%B5022440000000000098^^391200081613?;5022440000000000098=391200081613?" };
        }

        [TestMethod]
        public void GiftServiceActivate() {
            Transaction response = service.Activate(10m)
                .WithCurrency("USD")
                .WithPaymentMethod(card)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void GiftServiceAddValue() {
            Transaction response = service.AddValue(11m)
                .WithCurrency("USD")
                .WithPaymentMethod(card)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void GiftServiceAddAlias() {
            Transaction response = service.AddAlias("2145550199")
                .WithPaymentMethod(card)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void GiftServiceBalanceInquiry() {
            Transaction response = service.BalanceInquiry()
                .WithPaymentMethod(card)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void GiftServiceCharge() {
            Transaction response = service.Charge(10m)
                .WithCurrency("USD")
                .WithPaymentMethod(card)
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void GiftServiceDeactivate() {
            Transaction response = service.Deactivate()
                .WithPaymentMethod(card)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void GiftServiceRemoveAlias() {
            Transaction response = service.RemoveAlias("2145550199")
                .WithPaymentMethod(card)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void GiftServiceReplaceWith() {
            Transaction response = service.ReplaceWith(replacement)
                .WithPaymentMethod(card)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void GiftServiceReverseByCard() {
            Transaction response = service.Charge(10m)
                .WithCurrency("USD")
                .WithPaymentMethod(card)
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            Transaction reverseResponse = service.Reverse(10m)
                .WithPaymentMethod(card)
                .Execute();
            Assert.IsNotNull(reverseResponse);
            Assert.AreEqual("00", reverseResponse.ResponseCode);
        }

        [TestMethod]
        public void GiftServiceReverseByTransactionId() {
            Transaction response = service.Charge(10m)
                .WithCurrency("USD")
                .WithPaymentMethod(card)
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            Transaction reverseResponse = service.Reverse(10m)
                .WithTransactionId(response.TransactionId)
                .Execute();
            Assert.IsNotNull(reverseResponse);
            Assert.AreEqual("00", reverseResponse.ResponseCode);
        }

        [TestMethod]
        public void GiftServiceRewards() {
            Transaction response = service.Rewards(15m)
                .WithCurrency("USD")
                .WithPaymentMethod(card)
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void GiftServiceVoid() {
            Transaction response = service.Charge(10m)
                .WithCurrency("USD")
                .WithPaymentMethod(card)
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            Transaction voidResponse = service.Void(response.TransactionId).Execute();
            Assert.IsNotNull(voidResponse);
            Assert.AreEqual("00", voidResponse.ResponseCode);
        }
    }
}
