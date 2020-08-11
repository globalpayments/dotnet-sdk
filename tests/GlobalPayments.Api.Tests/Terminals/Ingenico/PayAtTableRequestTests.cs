using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Terminals;
using GlobalPayments.Api.Terminals.Ingenico;
using GlobalPayments.Api.Terminals.Ingenico.Requests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.Terminals.Ingenico {
    [TestClass]
    public class PayAtTableRequestTests {

        private IDeviceInterface _device;

        public PayAtTableRequestTests() {


            TerminalUtilities.log("TERMINAL START CONNECTION...");

            _device = DeviceService.Create(new ConnectionConfig() {
                DeviceType = DeviceType.Ingenico_EPOS_Desk5000,
                ConnectionMode = ConnectionModes.TCP_IP_SERVER,
                Port = "18101",
                Timeout = 60 * 1000,
                DeviceMode = DeviceMode.PAY_AT_TABLE
            });


            Assert.IsNotNull(_device);

            _device.OnPayAtTableRequest += _device_OnPayAtTableRequest;

            TerminalUtilities.log("TERMINAL CONNECTED");
        }

        private void _device_OnPayAtTableRequest(PATRequest request) {
            // Success ConfirmaitonOK

            TerminalUtilities.log("OnPayAtTableRequest " + request.ToString() + "  " + request.RequestType.ToString());

            Thread.Sleep(5 * 1000);



            if (request.RequestType == PATRequestType.TableReceipt) {
                TerminalUtilities.log("Receipt response");
                _device.PayAtTableResponse()
                    .WithXMLPath("C:\\tmp\\receipt.txt")
                    .Execute();
            }
            else {
                _device.PayAtTableResponse()
                    .WithPayAtTableResponseType(PATResponseType.CONF_OK)
                    .WithAmount(25M)
                    .WithPaymentMode(PATPaymentMode.USE_ADDITIONAL)
                    .Execute();
            }
        }

        [TestMethod]
        public void TableLock() {
            Thread.Sleep(3000 * 1000);
        }
    }
}
