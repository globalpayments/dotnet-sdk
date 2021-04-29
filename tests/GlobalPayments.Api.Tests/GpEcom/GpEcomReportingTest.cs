using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace GlobalPayments.Api.Tests.Realex {
    [TestClass]
    public class GpEcomReportingTest {
        public GpEcomReportingTest() {
            ServicesContainer.ConfigureService(new GpEcomConfig {
                DataClientId = "2413abd8-ea1f-4f0e-a4b5-eb5ca682efe2",
                DataClientSecret = "nQ3eO3gV1sV7qC7vX8vY3nB1qR4oQ0dH6wI6wN4aA1oA3sP3aL",
                DataClientUserId = "INTAPIUK",
                MerchantId = "heartlandgpsandbox",
                AccountId = "api",
                SharedSecret = "secret",
                Timeout = 240000, 
            });
        }

        [TestMethod]
        public void GetTransactionDetail() {
            string orderId = "u2RjrtEmaU2f0pB-aGw4Eg";
            
            TransactionSummary response = ReportingService.TransactionDetail(orderId)
                .Execute();
            
            Assert.IsNotNull(response);
            Assert.AreEqual(orderId, response.OrderId);
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
