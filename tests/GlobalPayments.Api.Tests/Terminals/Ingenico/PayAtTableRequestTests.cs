using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Terminals;
using GlobalPayments.Api.Terminals.Ingenico;
using GlobalPayments.Api.Terminals.Ingenico.Requests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace GlobalPayments.Api.Tests.Terminals.Ingenico {
    [TestClass]
    public class PayAtTableRequestTests {

        private IDeviceInterface _device;

        public PayAtTableRequestTests() {
            _device = DeviceService.Create(new ConnectionConfig() {
                DeviceType = DeviceType.Ingenico_EPOS_Desk5000,
                ConnectionMode = ConnectionModes.PAY_AT_TABLE,
                Port = "18101",
                Timeout = 10 * 1000
            });

            Assert.IsNotNull(_device);

            _device.OnPayAtTableRequest += _device_OnPayAtTableRequest;
        }

        private void _device_OnPayAtTableRequest(PATRequest request) {
            // Success ConfirmaitonOK
            //Thread.Sleep(8 * 1000);

            if (request.RequestType == PATRequestType.TableList) {
                var test = request.TableId;
                _device.PayAtTableResponse()
                    .WithXMLPath("C:\\Users\\steven.tan\\Desktop\\PAY@TABLE SAMPLE RESPONSE\\tablelistsample.xml")
                    .Execute();
            } else if (request.RequestType == PATRequestType.Ticket) {
                _device.PayAtTableResponse()
                    .WithPayAtTableResponseType(PATResponseType.CONF_OK)
                    .WithAmount(123.45M)
                    .WithPaymentMode(PATPaymentMode.NO_ADDITIONAL)
                    .Execute();
            } else if (request.RequestType == PATRequestType.SplitSaleReport) {
                _device.PayAtTableResponse()
                    .WithPayAtTableResponseType(PATResponseType.CONF_OK)
                    .WithAmount(123.45M)
                    .WithPaymentMode(PATPaymentMode.NO_ADDITIONAL)
                    .Execute();
            } else if (request.RequestType == PATRequestType.TableReceipt) {
                _device.PayAtTableResponse()
                    .WithXMLPath("C:\\Users\\steven.tan\\Desktop\\PAY@TABLE SAMPLE RESPONSE\\receiptrequestsample.xml")
                    .Execute();
            } else if (request.RequestType == PATRequestType.TransactionOutcome) {
                _device.PayAtTableResponse()
                    .WithPayAtTableResponseType(PATResponseType.CONF_OK)
                    .WithAmount(123.45M)
                    .WithPaymentMode(PATPaymentMode.NO_ADDITIONAL)
                    .Execute();
            } else if (request.RequestType == PATRequestType.TableLock) {
                _device.PayAtTableResponse()
                    .WithPayAtTableResponseType(PATResponseType.CONF_OK)
                    .WithAmount(123M)
                    .WithPaymentMode(PATPaymentMode.NO_ADDITIONAL)
                    .Execute();
            }
        }

        [TestMethod]
        public void TableLock() {
            Thread.Sleep(20 * 1000);
        }
    }
}
