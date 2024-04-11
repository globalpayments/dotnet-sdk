using System;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.GpEcom {
    [TestClass]
    public class GpEcomReportingTest {
        public GpEcomReportingTest() {
            var config = new GpEcomConfig {
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                RebatePassword = "rebate",
                RefundPassword = "refund",
                ServiceUrl = "https://api.sandbox.realexpayments.com/epage-remote.cgi",
            };
            ServicesContainer.ConfigureService(config);
        }

        [TestMethod]
        public void GetTransactionDetail() {
            const string orderId = "Y-McGGBlhESVARbFu525sg";
            
            var response = ReportingService.TransactionDetail(orderId)
                .Execute();
            
            Assert.IsNotNull(response);
            Assert.AreEqual(orderId, response.OrderId);
            Assert.AreEqual("H2zb4OYh1j5pNh4q", response.SchemeReferenceData);
            Assert.AreEqual("17019444760659420", response.TransactionId);
            Assert.AreEqual("U", response.AvsResponseCode);
            Assert.AreEqual("M", response.CvnResponseCode);
            Assert.AreEqual("00", response.GatewayResponseCode);
            Assert.AreEqual("(00)[ test system ] Authorised", response.GatewayResponseMessage);
            Assert.AreEqual("PASS", response.FraudRuleInfo);
        }

        [TestMethod]
        public void GetTransactionDetail_WithRandomId() {
            var orderId = Guid.NewGuid().ToString().Replace("-", "");
            try {
                ReportingService.TransactionDetail(orderId)
                    .Execute();
            }
            catch (GatewayException ex) {
                Assert.AreEqual("508", ex.ResponseCode);
                Assert.AreEqual("Original transaction not found.", ex.ResponseMessage);
            }
        }
        
        [TestMethod]
        public void GetTransactionDetail_WithNullId() {
            try {
                ReportingService.TransactionDetail(null)
                    .Execute();
            }
            catch (BuilderException ex) {
                Assert.AreEqual("TransactionId cannot be null for this transaction type.", ex.Message);
            }
        }
    }
}
