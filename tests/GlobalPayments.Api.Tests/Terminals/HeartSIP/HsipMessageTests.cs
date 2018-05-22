using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Terminals;
using GlobalPayments.Api.Terminals.HeartSIP;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace GlobalPayments.Api.Tests.Terminals.HeartSIP {
    [TestClass]
    public class HsipMessageTests {
        private IDeviceInterface device;

        public HsipMessageTests() {
            device = DeviceService.Create(new ConnectionConfig {
                DeviceType = DeviceType.HSIP_ISC250,
                ConnectionMode = ConnectionModes.TCP_IP,
                IpAddress = "10.12.220.130",
                Port = "12345",
                Timeout = 30000
            });
            Assert.IsNotNull(device);
            device.OpenLane();
        }

        [TestCleanup]
        public void WaitAndReset() {
            Thread.Sleep(3000);
            device.Reset();
        }

        [TestMethod]
        public void SendCustomResetMessage() {
            DeviceMessage message = TerminalUtilities.BuildRequest("<SIP><Version>1.0</Version><ECRId>1004</ECRId><Request>Reset</Request></SIP>", MessageFormat.HeartSIP);

            var response = device.SendCustomMessage(message);
            Assert.IsNotNull(response);
        }
    }
}
