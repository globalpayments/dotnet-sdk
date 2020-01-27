using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Terminals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.Terminals.Genius {
    [TestClass]
    public class GeniusCreditTests {
        IDeviceInterface _device;
        RandomIdProvider _requestidProvider;

        public GeniusCreditTests() {
            _requestidProvider = new RandomIdProvider();

            _device = DeviceService.Create(new ConnectionConfig {
                DeviceType = DeviceType.GENIUS,
                ConnectionMode = ConnectionModes.HTTP,
                IpAddress = "10.12.220.209",
                Port = "8080",
                Timeout = 30000,
                GatewayConfig = new GeniusConfig {
                    MerchantName = "Test Shane Logsdon",
                    MerchantSiteId = "BKHV2T68",
                    MerchantKey = "AT6AN-ALYJE-YF3AW-3M5NN-UQDG1",
                    RegisterNumber = "35",
                    TerminalId = "3",
                    DBA = "GP ECOM"
                }
            });
            Assert.IsNotNull(_device);
        }

        [TestMethod]
        public void CreditSale() {
            var response = _device.Sale(10m)
                .WithInvoiceNumber(_requestidProvider.GetRequestId().ToString())
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }
    }
}
