using GlobalPayments.Api.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Environment = GlobalPayments.Api.Entities.Environment;
using System.Collections.Generic;
using System.Text;
using GlobalPayments.Api.Tests.ProPay.TestData;

namespace GlobalPayments.Api.Tests.ProPay
{
    [TestClass]
    public class ProPayFundsTests {
        private PayFacService _service;

        public ProPayFundsTests() {
            _service = new PayFacService();
            ServicesContainer.ConfigureService(new PorticoConfig()
            {
                CertificationStr = "5dbacb0fc504dd7bdc2eadeb7039dd",
                TerminalID = "7039dd",
                Environment = Environment.TEST,
                X509CertificatePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"ProPay\TestData\testCertificate.crt"),
                ProPayUS = true
            });
        }
        [TestMethod]
        public void AddFunds() {
            var response = _service.AddFunds()
                .WithAccountNumber("718134204")
                .WithAmount("100")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void SweepFunds() {
            var response = _service.SweepFunds()
                .WithAccountNumber("718134204")
                .WithAmount("100")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void AddFlashFundsPaymentCard() {
            var response = _service.AddCardFlashFunds()
                .WithAccountNumber("718134204")
                .WithFlashFundsPaymentCardData(TestFundsData.GetFlashFundsPaymentCardData())
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void PushMoneyToFlashFundsCard() {
            var response = _service.PushMoneyToFlashFundsCard()
                .WithAccountNumber("718134204")
                .WithAmount("100")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }
    }
}
