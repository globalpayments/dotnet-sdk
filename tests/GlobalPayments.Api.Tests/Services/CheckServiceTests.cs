using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace GlobalPayments.Api.Tests.Services {
    [TestClass]
    public class CheckServiceTests {
        CheckService service;
        eCheck check;
        Address address;

        public CheckServiceTests() {
            service = new CheckService(new PorticoConfig {
                SecretApiKey = "skapi_cert_MTyMAQBiHVEAewvIzXVFcmUd2UcyBge_eCpaASUp0A"
            });

            check = TestData.TestChecks.HeartlandACH();

            address = new Address {
                StreetAddress1 = "123 Main St.",
                City = "Downtown",
                State = "NJ",
                PostalCode = "12345"
            };
        }

        [TestMethod]
        public void CheckServiceSale() {
            Transaction response = service.Charge(10m)
                .WithCurrency("USD")
                .WithPaymentMethod(check)
                .WithAddress(address)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void CheckServiceVoidGatewayTxnId() {
            Transaction response = service.Charge(11m)
                .WithAllowDuplicates(true)
                .WithCurrency("USD")
                .WithPaymentMethod(check)
                .WithAddress(address)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            Transaction voidResponse = service.Void(response.TransactionId).Execute();
            Assert.IsNotNull(voidResponse);
            Assert.AreEqual("00", voidResponse.ResponseCode);
        }

        [TestMethod]
        public void CheckServiceVoidClientTxnId() {
            string clientTxnId = new Random().Next(100000, 999999).ToString();

            Transaction response = service.Charge(12m)
                .WithAllowDuplicates(true)
                .WithCurrency("USD")
                .WithClientTransactionId(clientTxnId)
                .WithPaymentMethod(check)
                .WithAddress(address)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            Transaction voidResponse = service.Void(clientTxnId, true).Execute();
            Assert.IsNotNull(voidResponse);
            Assert.AreEqual("00", voidResponse.ResponseCode);
        }
    }
}
