using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Terminals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.Terminals.Pax {
    [TestClass]
    public class PaxMessageTests {
        private IDeviceInterface device;

        public PaxMessageTests() {
            device = DeviceService.Create(new ConnectionConfig {
                DeviceType = DeviceType.PAX_S300,
                ConnectionMode = ConnectionModes.HTTP,
                IpAddress = "10.12.220.172",
                Port = "10009",
                Timeout = 30000
            });
            Assert.IsNotNull(device);
        }

        [TestMethod]
        public void SendCustomResetMessage() {
            DeviceMessage message = TerminalUtilities.BuildRequest("A16");

            var response = device.SendCustomMessage(message);
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public void SendCustomComplexMessage() {
            DeviceMessage message = TerminalUtilities.BuildRequest(
                "A04", // FOR PAX THE FIRST ELEMENT SHOULD ALWAYS BE THE MESSAGE ID
                "00",
                ControlCodes.FS,
                "hostRspBeep",
                ControlCodes.FS,
                "N"
            );

            var response = device.SendCustomMessage(message);
            Assert.IsNotNull(response);
        }
    }
}
