using GlobalPayments.Api.Services;
using GlobalPayments.Api.Terminals;
using GlobalPayments.Api.Terminals.Ingenico;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using System.Threading;

namespace GlobalPayments.Api.Tests.Terminals.Ingenico {
    [TestClass]
    public class TerminalManagementTest {
        IDeviceInterface _device;

        public TerminalManagementTest() {
            _device = DeviceService.Create(new ConnectionConfig() {
                DeviceType = Entities.DeviceType.Ingenico_EPOS_Desk5000,
                ConnectionMode = ConnectionModes.TCP_IP_SERVER,
                Port = "18101",
                Timeout = 30 * 1000
            });
            Assert.IsNotNull(_device);
        }

        [TestMethod]
        public void StateCommandTest() {
            try {
                var resp = _device.GetTerminalStatus();

                Assert.IsNotNull(resp);
            }
            catch (Exception e) {
                Assert.Fail(e.Message);
            }
        }

        [TestMethod]
        public void PIDCommandTest() {
            try {
                var resp = _device.Initialize();

                Assert.IsNotNull(resp);
            }
            catch (Exception e) {
                Assert.Fail(e.Message);
            }
        }

        [TestMethod]
        public void CALLTMSCommandTest() {
            try {
                var resp = _device.GetTerminalConfiguration();

                Assert.IsNotNull(resp);
                Assert.IsNotNull((resp as IngenicoTerminalResponse).PrivateData);
            }
            catch (Exception e) {
                Assert.Fail(e.Message);
            }
        }

        [TestMethod]
        public void LOGONCommandTest() {
            try {
                var resp = _device.TestConnection();

                Assert.IsNotNull(resp);
                Assert.IsNotNull((resp as IngenicoTerminalResponse).PrivateData);
            }
            catch (Exception e) {
                Assert.Fail(e.Message);
            }
        }

        [TestMethod]
        public void RESETCommandTest() {
            try {
                var resp = _device.Reboot();

                Assert.IsNotNull(resp);
                Assert.IsNotNull((resp as IngenicoTerminalResponse).PrivateData);
            }
            catch (Exception e) {
                Assert.Fail(e.Message);
            }
        }
    }
}
