using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Tests.TestData;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.Services {
    [TestClass]
    public class EbtServiceTests {
        EbtService service;
        EBTTrackData card;

        public EbtServiceTests() {
            service = new EbtService(new PorticoConfig {
                SecretApiKey = "skapi_cert_MaePAQBr-1QAqjfckFC8FTbRTT120bVQUlfVOjgCBw"
            });

            card = TestCards.VisaSwipe().AsEBT("32539F50C245A6A93D123412324000AA");
        }

        [TestMethod]
        public void EbtServiceBalanceInquiry() {
            Transaction response = service.BalanceInquiry()
                .WithPaymentMethod(card)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void EbtServiceBenefitsWithdrawal() {
            Transaction response = service.BenefitWithdrawal(10m)
                .WithCurrency("USD")
                .WithPaymentMethod(card)
                .WithAllowDuplicates(true)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void EbtServiceSale() {
            Transaction response = service.Charge(11m)
                .WithCurrency("USD")
                .WithPaymentMethod(card)
                .WithAllowDuplicates(true)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void EbtServiceRefundByCard() {
            Transaction response = service.Charge(12m)
                .WithCurrency("USD")
                .WithPaymentMethod(card)
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            Transaction refundResponse = service.Refund(12m)
                .WithCurrency("USD")
                .WithPaymentMethod(card)
                .Execute();
            Assert.IsNotNull(refundResponse);
            Assert.AreEqual("00", refundResponse.ResponseCode);
        }
    }
}
