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
            string orderId = "u2RjrtEmaU2f0pB-aGw4Eg";
            
            TransactionSummary response = ReportingService.TransactionDetail(orderId)
                .Execute();
            
            Assert.IsNotNull(response);
            Assert.AreEqual(orderId, response.OrderId);
            Assert.AreEqual("0QApOb88ngvBrZF8", response.SchemeReferenceData);
        }

        [TestMethod]
        public void GetTransactionDetail_WithRandomId() {
            string orderId = Guid.NewGuid().ToString();
            try {
                ReportingService.TransactionDetail(orderId)
                    .Execute();
            }
            catch (GatewayException ex) {
                Assert.AreEqual("508", ex.ResponseCode);
                Assert.AreEqual("Original transaction not found.", ex.ResponseMessage);
            }
        }
    }
}
