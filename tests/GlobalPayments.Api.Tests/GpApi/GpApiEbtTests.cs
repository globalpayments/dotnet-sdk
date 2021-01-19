using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Tests.GpApi {
    [TestClass]
    public class GpApiEbtTests : BaseGpApiTests {
        EBTCardData card;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context) {
            ServicesContainer.ConfigureService(new GpApiConfig {
                AppId = "Uyq6PzRbkorv2D4RQGlldEtunEeGNZll",
                AppKey = "QDsW1ETQKHX6Y4TA",
                Channel = Channel.CardPresent,
            });
        }

        [TestInitialize]
        public void TestInitialize() {
            card = new EBTCardData {
                Number = "4012002000060016",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvn = "123",
                PinBlock = "32539F50C245A6A93D123412324000AA"
            };
        }

        //[TestMethod]
        //public void EbtBalanceInquiry() {
        //    var response = card.BalanceInquiry()
        //        .Execute();
        //    Assert.IsNotNull(response);
        //    Assert.AreEqual(SUCCESS, response?.ResponseCode);
        //}

        [TestMethod]
        public void EbtSale() {
            var response = card.Charge(10m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual(SUCCESS, response?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response?.ResponseMessage);
        }

        [TestMethod]
        public void EbtRefund() {
            var response = card.Refund(10m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual(SUCCESS, response?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response?.ResponseMessage);
        }
    }
}
