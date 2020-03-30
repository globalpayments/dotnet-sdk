using System.Threading;
using System.Threading.Tasks;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Terminals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.Terminals.Ingenico {
    [TestClass]
    public class TransactionTest {
        IDeviceInterface _device;

        public TransactionTest() {
            _device = DeviceService.Create(new ConnectionConfig() {
                DeviceType = DeviceType.Ingenico_EPOS_Lane3000,
                ConnectionMode = ConnectionModes.TCP_IP_SERVER,
                Port = "18101",
                Timeout = 60000,
                RequestIdProvider = new RandomIdProvider()
            });
            Assert.IsNotNull(_device);
        }


        [TestMethod]
        public void AsyncCancelTest() {
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
                Assert.AreEqual(respCancel.Status, "CANCEL_DONE");
            });

            Thread.Sleep(10000);
            _device.Dispose();

            Task.WaitAll(tsk1, tsk2);
        }

        [TestMethod]
        public void ReverseTest() {
            
            Thread.Sleep(10000);

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
                Assert.AreEqual(resp.Status, "REVERSAL_SUCCESS");
            }
            else Assert.IsNull(resSale);
        }

        [TestMethod]
        public void DuplicTest() {

            var duplicate = _device.Duplicate(12.5m)
                .WithReferenceNumber(39)
                .Execute();

            _device.Dispose();
            Assert.IsNotNull(duplicate);
            Assert.AreEqual(duplicate.TransactionAmount, 12.5m);
        }
    }
}
