using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Terminals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.Terminals.Pax {
    [TestClass]
    public class PaxDebitTests {
        IDeviceInterface _device;

        public PaxDebitTests() {
            _device = DeviceService.Create(new ServicesConfig {
                SecretApiKey = "skapi_cert_MTyMAQBiHVEAewvIzXVFcmUd2UcyBge_eCpaASUp0A",
                ServiceUrl = "https://cert.api2.heartlandportico.com",
                DeviceConnectionConfig = new ConnectionConfig {
                    DeviceType = DeviceType.PaxS300,
                    ConnectionMode = ConnectionModes.HTTP,
                    IpAddress = "10.12.220.172",
                    Port = "10009"
                }
            });
            Assert.IsNotNull(_device);
        }

        [TestMethod]
        public void DebitSale() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
                Assert.IsTrue(message.StartsWith("[STX]T02[FS]1.35[FS]01[FS]1000[FS][US][US][US][US][US]1[FS]5[FS][FS][ETX]"));
            };

            var response = _device.DebitSale(5, 10m)
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void DebitSaleNoAmount() {
            _device.DebitSale(5).Execute();
        }

        [TestMethod]
        public void DebitRefund() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
                Assert.IsTrue(message.StartsWith("[STX]T02[FS]1.35[FS]02[FS]1000[FS][FS]6[FS][FS][ETX]"));
            };

            var response = _device.DebitRefund(6, 10m).Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.DeviceResponseText);
        }

        [TestMethod]
        public void DebitRefundByTransactionId() {
            var response = _device.DebitSale(5, 10m)
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            string transactionId = response.TransactionId;
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
                Assert.IsTrue(message.StartsWith("[STX]T02[FS]1.35[FS]02[FS]1000[FS][FS]5[FS][FS]HREF=" + transactionId + "[ETX]"));
            };

            var response2 = _device.DebitRefund(5, 10m)
                .WithTransactionId(transactionId)
                .Execute();
            Assert.IsNotNull(response2);
            Assert.AreEqual("00", response2.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void DebitRefund_NoAmount() {
            _device.DebitRefund(5).Execute();
        }
    }
}
