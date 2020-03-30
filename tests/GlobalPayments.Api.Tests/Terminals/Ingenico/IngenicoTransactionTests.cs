using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Terminals;
using GlobalPayments.Api.Terminals.INGENICO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.Terminals.Ingenico {
    [TestClass]
    public class IngenicoTransactionTests {
        IDeviceInterface _device;

        public IngenicoTransactionTests() {
            _device = DeviceService.Create(new ConnectionConfig() {
                DeviceType = Entities.DeviceType.Ingenico_EPOS_Desk5000,
                ConnectionMode = ConnectionModes.TCP_IP_SERVER,
                Port = "18101",
                Timeout = 60000,
                RequestIdProvider = new RandomIdProvider()
            });
            Assert.IsNotNull(_device);
        }

        [TestMethod]
        public void SaleTest() {
            try {
                _device.Dispose();
                Thread.Sleep(25000);

                var resp = _device.Sale(15m)
                    .WithCashBack(2m)
                    .WithCurrencyCode("826")
                    .Execute();

                Assert.IsNotNull(resp);
                Assert.IsNotNull(resp.AuthorizationCode);
            }
            catch (Exception ex) {
                Assert.Fail(ex.Message);
            }
            finally {
                _device.Dispose();
            }
        }

        [TestMethod]
        public void CancelTest() {

            var tsk1 = Task.Factory.StartNew(() => {
                var respSale = _device.Sale(15.12m)
                .WithCashBack(3m)
                .WithReferenceNumber(02)
                .Execute();

                Assert.IsNotNull(respSale);
            });

            var tsk2 = Task.Factory.StartNew(() => {
                Thread.Sleep(7000);

                var respCancel = _device.Cancel(15.12m)
                .WithReferenceNumber(03)
                .Execute();

                Assert.IsNotNull(respCancel);
            });

            Task.WaitAll(tsk1, tsk2);
        }

        [TestMethod]
        public void ReverseTest() {

            var resSale = _device.Sale(125.12m)
                .WithReferenceNumber(55)
                .Execute();

            Thread.Sleep(10000);

            if (resSale != null) {
                var resp = _device.Reverse(amount: 6.18m)
                .WithReferenceNumber(12)
                .Execute();

                var termId = resp.TerminalRefNumber;

                Assert.IsNotNull(termId);
                Assert.IsNotNull(resp);
            }
            else Assert.IsNull(resSale);
        }

        [TestMethod]
        public void DuplicateTest() {
            var resp = _device.Duplicate();
            Assert.IsNotNull(resp);
        }

        [TestMethod]
        public void ParsingRawResponseTest() {
            string res = "09000015120                                                       826CANCELDONE";
            byte[] buffers = Encoding.ASCII.GetBytes(res);

            var pResp = new DataResponse(ASCIIEncoding.ASCII.GetBytes(res.Substring(12, 55)));

            Assert.IsNotNull(pResp);
        }
    }
}
