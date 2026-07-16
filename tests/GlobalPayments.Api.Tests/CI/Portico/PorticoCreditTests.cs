using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Tests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.CI {
    [TestClass]
    public class PorticoCreditTests {
        private static CiTestingHarness _harness;
        private CreditCardData _card;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context) {
            _harness = new CiTestingHarness(
                "https://cert.api2.heartlandportico.com",
                CiTestingHarness.CacheMode.Locked,
                "PorticoCreditTests");
        }

        [TestInitialize]
        public void TestInitialize() {
            _card = new CreditCardData {
                Number = "4111111111111111",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvn = "123"
            };
        }

        private void ConfigurePorticoService() {
            var config = new PorticoConfig {
                SecretApiKey = "skapi_cert_MTeSAQAfG1UA9qQDrzl-kz4toXvARyieptFwSKP24w",
                DeveloperId = "002914",
                VersionNumber = "3026",
                EnableLogging = true
            };
            config.ServiceUrl = _harness.GetTestingUrl();
            ServicesContainer.ConfigureService(config);
        }

        [TestMethod]
        public void CreditSale() {
            _harness.SetFunction("Portico|Credit Transactions|CreditSale");
            ConfigurePorticoService();
            var clientTxnId = _harness.GenerateRandomId("creditSale");

           
            var response = _card.Charge(15.5m)
                .WithCurrency("USD")
                .WithClientTransactionId(clientTxnId)
                .WithUniqueDeviceId("5678")
                .WithAllowDuplicates(true)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.AreEqual(clientTxnId, response.ClientTransactionId);
        }

        [TestMethod]
        public void CreditTxnEdit() {
            _harness.SetFunction("Portico|Credit Transactions|CreditTxnEdit - aka Gratuity");
            ConfigurePorticoService();
            var clientTxnId = _harness.GenerateRandomId("creditTxnEdit_charge");

            var charge = _card.Charge(15m)
                .WithCurrency("USD")
                .WithClientTransactionId(clientTxnId)
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(charge);
            Assert.AreEqual("00", charge.ResponseCode);
            
            var edit = charge.Edit()
                .WithAmount(17m)
                .WithCurrency("USD")
                .WithGratuity(2m)
                .Execute();
            Assert.IsNotNull(edit);
            Assert.AreEqual("00", edit.ResponseCode);
        }

        [TestMethod]
        public void CreditAdditionalAuth() {
            _harness.SetFunction("Portico|Credit Transactions|CreditAdditionalAuth");
            ConfigurePorticoService();
            var clientTxnId = _harness.GenerateRandomId("creditAdditionalAuth_auth");

            var auth = _card.Authorize(10m)
                .WithCurrency("USD")
                .WithClientTransactionId(clientTxnId)
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(auth);
            Assert.AreEqual("00", auth.ResponseCode);
            Assert.AreEqual(clientTxnId, auth.ClientTransactionId);
            
            var additional = Transaction.FromId(auth.TransactionId)
                .Increment(10m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(additional);
            Assert.AreEqual("00", additional.ResponseCode);
        }
    }
}
