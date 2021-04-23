using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Terminals;
using GlobalPayments.Api.Terminals.Ingenico;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;

namespace GlobalPayments.Api.Tests.Terminals.Ingenico {
    [TestClass]
    public class IngenicoTransactionTests {
        IDeviceInterface _device;

        public IngenicoTransactionTests() {
            _device = DeviceService.Create(new ConnectionConfig() {
                DeviceType = DeviceType.Ingenico_EPOS_Desk5000,
                ConnectionMode = ConnectionModes.SERIAL,
                Port = "1",
                BaudRate = BaudRate.r9600,
                Parity = System.IO.Ports.Parity.Even,
                DataBits = DataBits.Seven,
                Handshake = System.IO.Ports.Handshake.None,
                StopBits = System.IO.Ports.StopBits.One,
                Timeout = 1 * 1000
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

        [TestMethod]
        public void Ticket() {
            try {
                _device.OnMessageSent += (message) => {
                    Assert.IsNotNull(message);
                };

                var ticket = _device.GetLastReceipt(ReceiptType.REPORT)
                    .Execute();

                Assert.IsNotNull(ticket);
            } catch (Exception e) {
                Thread.Sleep(5000);
                _device.OnMessageSent += (message) => {
                    Assert.IsNotNull(message);
                };

                var ticket = _device.GetLastReceipt(ReceiptType.REPORT)
                    .Execute();

                Assert.IsNotNull(ticket);
            }
        }

        [TestMethod]
        public void SaleTest() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };

            var resp = _device.Sale(6.18m)
                .WithReferenceNumber(1)
                .WithCurrencyCode("826")
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
                .WithTaxFree(TaxFreeType.CASH)
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

        [TestMethod]
        public void TaxFreeCreditCardRefundTest() {
            try {
                var respone = _device.Refund(5m)
                    .WithTaxFree(TaxFreeType.CREDIT)
                    .Execute();

                Assert.IsNotNull(respone);
            } catch (ApiException e) {
                Assert.Fail(e.Message);
                //throw e;
            } catch (Exception e) {
                Assert.Fail(e.Message);
                //throw e;
            }
        }

        [TestMethod]
        public void TaxFreeCashRefundTest() {
            try {
                var respone = _device.Refund(5m)
                    .WithReferenceNumber(1)
                    .WithTaxFree(TaxFreeType.CASH)
                    .Execute();

                Assert.IsNotNull(respone);
            } catch (Exception e) {
                Assert.Fail(e.Message);
            }
        }
    }
}
