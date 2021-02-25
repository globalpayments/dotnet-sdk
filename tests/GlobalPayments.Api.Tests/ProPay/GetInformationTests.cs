using GlobalPayments.Api.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Environment = GlobalPayments.Api.Entities.Environment;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Tests.ProPay
{
    [TestClass]
    public class GetInformationTests {
        private PayFacService _service;

        public GetInformationTests() {
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
        public void GetAccountInfo() {
            var response = _service.GetAccountDetails()
                .WithAccountNumber("718135687")
                //.WithSourceEmail("user2509@user.com")
                //.WithExternalID("79065399")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void GetAccountBalance() {
            var response = _service.GetAccountDetails()
                .WithAccountNumber("718037672")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

    }
}
