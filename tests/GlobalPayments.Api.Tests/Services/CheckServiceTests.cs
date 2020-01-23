using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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

            check = TestData.TestChecks.Certification();

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
        public void CheckServiceVoid() {
            Transaction response = service.Charge(11m)
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
    }
}
