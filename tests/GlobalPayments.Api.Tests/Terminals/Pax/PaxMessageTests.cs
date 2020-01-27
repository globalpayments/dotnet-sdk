using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Terminals;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace GlobalPayments.Api.Tests.Terminals.Pax {
    [TestClass]
    public class PaxMessageTests {
        private IDeviceInterface device;

        public PaxMessageTests() {
            device = DeviceService.Create(new ConnectionConfig {
                DeviceType = DeviceType.PAX_S300,
                ConnectionMode = ConnectionModes.TCP_IP,
                IpAddress = "10.12.220.172",
                //IpAddress = "192.168.0.31",
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
        public void UpdateResourceFile() {
            using (var stream = File.OpenRead(@"C:\temp\idle-screen.zip")) {
                int count, offset = 0;
                byte[] buffer = new byte[3000];
                while ((count = stream.Read(buffer, 0, buffer.Length)) != 0) {
                    string data = Convert.ToBase64String(buffer).Trim();

                    DeviceMessage message = TerminalUtilities.BuildRequest(
                        "A18",
                        offset,
                        ControlCodes.FS,
                        data,
                        ControlCodes.FS,
                        ((offset + count) == stream.Length) ? "0" : "1"
                    );
                    var response = device.SendCustomMessage(message);
                    Assert.IsNotNull(response);
                    Assert.IsTrue(response.Contains("OK"));

                    offset += count;
                }
            } 
        }

        [TestMethod]
        public void SerializeBuilder() {
            var message = device.Authorize(10m).Serialize();
            Assert.IsNotNull(message);
            Assert.AreNotEqual(0, message.Length);
        }

        [TestMethod]
        public void DecodeHttpMessage() {
            byte[] buffer = Convert.FromBase64String("AlQwMBwxLjM1HDAxFTI3MjYVHx8fHx8xFTE1NTU1OTY1NjAVFRUVFQNt");
            var message = new DeviceMessage(buffer);
            Assert.IsNotNull(message.ToString());
        }

        [TestMethod]
        public void SetSafParams() {
            DeviceMessage message = TerminalUtilities.BuildRequest(
                "A54",
                "1",
                ControlCodes.FS,
                // start date,
                ControlCodes.FS,
                // end date,
                ControlCodes.FS,
                // duration in days,
                ControlCodes.FS,
                // max number,
                ControlCodes.FS,
                "500", // total ceiling,
                ControlCodes.FS,
                // ceiling per card brand,
                ControlCodes.FS,
                "100", // halo per card type,
                ControlCodes.FS,
                // upload mode,
                ControlCodes.FS,
                // auto upload interval time,
                ControlCodes.FS
                // delete SAF confirmation
            );

            var response = device.SendCustomMessage(message);
            Assert.IsNotNull(response);
        }
    }
}
