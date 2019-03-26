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
                IpAddress = "192.168.0.31",
                Port = "10009",
                Timeout = 30000,
                RequestIdProvider = new RandomIdProvider()
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
                "02",
                ControlCodes.FS,
                "cashBack",
                ControlCodes.FS,
                "1000,2000,3000,4000"
            );

            var response = device.SendCustomMessage(message);
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public void SerializeBuilder() {
            var message = device.CreditAuth(10m).Serialize();
            Assert.IsNotNull(message);
            Assert.AreNotEqual(0, message.Length);
        }
    }
}
