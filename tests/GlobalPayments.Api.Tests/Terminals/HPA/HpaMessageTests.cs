using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Terminals;
using GlobalPayments.Api.Terminals.HPA;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace GlobalPayments.Api.Tests.Terminals.HPA {
    [TestClass]
    public class HpaMessageTests {
        private IDeviceInterface device;

        public HpaMessageTests() {
            device = DeviceService.Create(new ConnectionConfig {
                DeviceType = DeviceType.HPA_ISC250,
                ConnectionMode = ConnectionModes.TCP_IP,
                IpAddress = "10.12.220.39",
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
            DeviceMessage message = TerminalUtilities.BuildRequest("<SIP><Version>1.0</Version><ECRId>1004</ECRId><Request>Reset</Request></SIP>", MessageFormat.HPA);

            var response = device.SendCustomMessage(message);
            Assert.IsNotNull(response);
        }
    }
}
