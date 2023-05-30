using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Terminals;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Threading.Tasks;

namespace GlobalPayments.Api.Tests.Terminals.Pax {
    [TestClass]
    public class PaxAsyncTests {
        IDeviceInterface _device;

        public PaxAsyncTests() {
            _device = DeviceService.Create(new ConnectionConfig {
                DeviceType = DeviceType.PAX_DEVICE,
                ConnectionMode = ConnectionModes.TCP_IP,
                IpAddress = "10.12.220.172",
                //IpAddress = "192.168.0.31",
                Port = "10009",
                RequestIdProvider = new RandomIdProvider()
            });
            Assert.IsNotNull(_device);
        }

        [TestCleanup]
        public void CleanUp() {
            Thread.Sleep(10000);
        }

        [TestMethod]
        public void CancelSale() {
            Task.Factory.StartNew(() => {
                var response = _device.Sale(10m).Execute();
                Assert.AreEqual("100002", response.DeviceResponseCode);
                Assert.AreEqual("ABORTED", response.DeviceResponseText);
            });
            Task.Factory.StartNew(() => {
                Thread.Sleep(5000);
                try {
                    _device.Cancel();
                }
                catch (ApiException exc) {
                    Assert.Fail(exc.Message);
                }
            });
        }
    }
}
