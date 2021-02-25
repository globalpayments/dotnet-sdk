using GlobalPayments.Api.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Environment = GlobalPayments.Api.Entities.Environment;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Tests.ProPay
{
    [TestClass]
    public class ProPayInNetworkTransactionTests {
        private PayFacService _service;

        public ProPayInNetworkTransactionTests() {
            _service = new PayFacService();
            ServicesContainer.ConfigureService(new PorticoConfig()
            {
                //CertificationStr = "C7277D317D1840F5ADEBE600CF47B9",  // Disbursement
                CertificationStr = "5dbacb0fc504dd7bdc2eadeb7039dd",
                TerminalID = "7039dd",
                Environment = Environment.TEST,
                X509CertificatePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"ProPay\TestData\testCertificate.crt"),
                ProPayUS = true
            });
        }

        [TestMethod]
        public void DisburseFunds() {
            var response = _service.DisburseFunds() // This method in the ProPay API requires a different, special CertificationStr value from a disbursement account
                .WithReceivingAccountNumber("718134204")
                .WithAmount("100")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void SpendBackTransaction() {
            var response = _service.SpendBack()
                .WithAccountNumber("718037672")
                .WithReceivingAccountNumber("718134204")
                .WithAmount("100")
                .WithAllowPending(false)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void ReverseSplitPay() {
            var response = _service.ReverseSplitPay()
                .WithAccountNumber("718037672")
                .WithAmount("100")
                .WithCCAmount("100")
                .WithRequireCCRefund(false)
                .WithTransNum("6")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.IsNotNull(response.PayFacData.Amount);
            Assert.IsNotNull(response.PayFacData.RecAccountNum);
            Assert.IsNotNull(response.PayFacData.TransNum);
        }

        [TestMethod]
        public void SplitFunds() {
            var response = _service.SplitFunds()
                .WithAccountNumber("718134204")
                .WithReceivingAccountNumber("718037672")
                .WithAmount("100")
                .WithTransNum("9")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }
    }
}
