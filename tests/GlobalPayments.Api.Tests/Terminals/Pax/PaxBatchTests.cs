using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Terminals;
using GlobalPayments.Api.Terminals.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.Terminals.Pax {
    [TestClass]
    public class PaxBatchTests {
        IDeviceInterface _device;

        public PaxBatchTests() {
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
        public void BatchClose() {
            var response = _device.BatchClose();
            Assert.IsNotNull(response);
            Assert.AreEqual("OK", response.DeviceResponseText);
        }
    }
}
