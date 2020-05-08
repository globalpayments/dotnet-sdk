using GlobalPayments.Api.Services;
using GlobalPayments.Api.Terminals;
using GlobalPayments.Api.Terminals.Ingenico;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;

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
        public void StateParsing() {
            string resp = "010000006180S1810e0b8c8f000f0a001V109712019054H011T0822164769";
            byte[] rawResp = Encoding.UTF8.GetBytes(resp, 0, resp.Length);

            var stateResp = new StateResponse(rawResp);

            var s = stateResp.ReferenceNumber;

            Assert.IsNotNull(stateResp);
        }

        [TestMethod]
        public void CancelResponseParsing() {
            string resp = "019000006180                                                       826CANCELDONE";
            byte[] rawResp = Encoding.UTF8.GetBytes(resp, 0, resp.Length);

            var TerminalResponse = new CancelResponse(rawResp);

            Assert.IsNotNull(TerminalResponse);
        }
    }
}
