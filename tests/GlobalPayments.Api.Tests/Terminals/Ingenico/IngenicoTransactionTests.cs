using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Terminals;
using GlobalPayments.Api.Terminals.Ingenico;
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
                Timeout = 60000
            });
            Assert.IsNotNull(_device);
        }

        [TestMethod]
        public void Test() {
            SaleTest();
            RefundTest();
            PreAuthTest();
            CompletionTest();
        }

        public void SaleTest() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };

            var resp = _device.Sale(6.18m)
                .WithReferenceNumber(1)
                .WithCurrencyCode("826")
                .WithCashBack(3m)
                .Execute();

            Assert.IsNotNull(resp);

            Thread.Sleep(5000);
        }

        public void RefundTest() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };

            var resp = _device.Refund(6.18m)
                .WithReferenceNumber(1)
                .WithCurrencyCode("826")
                .WithCashBack(3m)
                .Execute();

            Assert.IsNotNull(resp);

            Thread.Sleep(5000);
        }

        public void PreAuthTest() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };

            var resp = _device.Authorize(6.18m)
                .WithReferenceNumber(1)
                .WithCurrencyCode("826")
                .WithCashBack(3m)
                .Execute();

            Assert.IsNotNull(resp);

            Thread.Sleep(5000);
        }

        [TestMethod]
        public void CompletionTest() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };

            var resp = _device.Capture(6.18m)
                .WithAuthCode("025433")
                .WithReferenceNumber(1)
                .WithCurrencyCode("826")
                .Execute();

            Assert.IsNotNull(resp);

            Thread.Sleep(5000);
        }

        [TestMethod]
        public void AsyncCancelTest() {

            Thread thSale = new Thread(new ThreadStart(() => {
                var resp = _device.Sale(6.18m)
                .WithReferenceNumber(1)
                .WithCurrencyCode("826")
                .WithCashBack(3m)
                .Execute();


                Assert.IsNotNull(resp, "Sale Assert");
            }));


            Thread thCancel = new Thread(new ThreadStart(() => {
                var resp = _device.Cancel();

                Assert.IsNotNull(resp, "Cancel assert");
            }));

            thSale.Start();
            Thread.Sleep(2000);
            thCancel.Start();
        }

        [TestMethod]
        public void CancelTest() {
            var resp = _device.Cancel();

            Assert.IsNotNull(resp);
        }
    }
}
