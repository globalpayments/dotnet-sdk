using System;
using System.Threading;
using System.Threading.Tasks;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Terminals;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Terminals.Ingenico;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.Terminals.Ingenico {
    [TestClass]
    public class PaymentTransactionManagement {
        IDeviceInterface _device;

        public PaymentTransactionManagement() {

            _device = DeviceService.Create(new ConnectionConfig() {
                DeviceType = DeviceType.Ingenico_EPOS_Lane3000,
                ConnectionMode = ConnectionModes.SERIAL,
                Port = "7",
                BaudRate = BaudRate.r9600,
                Parity = System.IO.Ports.Parity.Even,
                StopBits = System.IO.Ports.StopBits.One,
                DataBits = DataBits.Seven,
                Timeout = 65000
            });

            Assert.IsNotNull(_device);
        }

        [TestMethod]
        public void SaleTest1() {
            var response = _device.Sale(12m)
            .WithReferenceNumber(15)
            .WithCashBack(10m)
            .WithCurrencyCode("826")
            .Execute();

            Assert.IsNotNull(response);
        }


        [TestMethod]
        public void SaleTest() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };

            ITerminalResponse response = _device.Sale(5.12m)
                .WithPaymentMode(PaymentMode.MAILORDER)
                .WithCashBack(7.569m)
                .WithReferenceNumber(1)
                .Execute();

            var authCode = response.AuthorizationCode;

            Assert.IsNotNull(response.AuthorizationCode);
        }

        [TestMethod]
        public void RefundTest() {
            var response = _device.Refund(1m)
                .WithPaymentMethodType(PaymentMethodType.Credit)
                .WithReferenceNumber(1)
                .Execute();

            Assert.IsNotNull(response.AuthorizationCode);
        }


        [TestMethod]
        public void SaleNegativeTest() {
            var response = _device.Sale(5.12m)
                .WithCashBack(7.52m)
                .WithReferenceNumber(1)
                .Execute();

            Assert.IsNull(response.AuthorizationCode);
        }

        [TestMethod]
        public void SaleTWithOnMessageSentest() {
            _device.OnBroadcastMessage += (code, message) => {
                Assert.IsNull(code);
                Assert.IsNull(message);
            };


            var response = _device.Refund(5.12m)
                //.WithCashBack(7.5m)
                .WithReferenceNumber(1)
                .Execute();

            Assert.IsNotNull(response.AuthorizationCode);
        }

        [TestMethod]
        public void CancelTest() {
            try {
                _device.Cancel();
            } catch (ApiException ex) {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void AsyncCancelTest() {
            var Task1 = Task.Factory.StartNew(() => {
                var response = _device.Sale(5.12m)
               .WithCashBack(7.5m)
               .WithReferenceNumber(1)
               .Execute();

                Assert.IsNotNull(response);
            });
            var Task2 = Task.Factory.StartNew(() => {
                try {
                    Thread.Sleep(5000);
                    _device.Cancel();
                } catch (Exception ex) {
                    Assert.Fail(ex.Message);
                }
            });

            Task.WaitAll(Task1, Task2);
        }


        [TestMethod]
        public void GetLastReceiptTest() {
            var response = _device
                .GetLastReceipt()
            .Execute();

            string xmlData = response.ToString();

            Assert.IsNotNull(response.ToString());
        }
    }
}
