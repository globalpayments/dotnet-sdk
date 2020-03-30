using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Terminals;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Terminals.INGENICO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Threading.Tasks;
using ReportType = GlobalPayments.Api.Terminals.INGENICO.ReportType;

namespace GlobalPayments.Api.Tests.Terminals.Ingenico {
    [TestClass]
    public class SerialPaymentTransactions {
        IDeviceInterface _device;

        public SerialPaymentTransactions() {
            _device = DeviceService.Create(new ConnectionConfig() {
                DeviceType = DeviceType.Ingenico_EPOS_Lane3000,
                ConnectionMode = ConnectionModes.SERIAL,
                Port = "5",
                BaudRate = BaudRate.r9600,
                DataBits = DataBits.Seven,
                StopBits = StopBits.One,
                Parity = Parity.Even,
                Handshake = System.IO.Ports.Handshake.None,
                Timeout = 65000,
                RequestIdProvider = new RandomIdProvider()
            });

            Assert.IsNotNull(_device);
        }

        [TestMethod]
        public void CaptureTest() {
            var response = _device.Capture(6.18m)
            .WithReferenceNumber(1)
            .WithCurrencyCode("826")
            .WithTransactionId("011223")
            .Execute();

            Assert.IsNotNull(response);
        }

        [TestMethod]
        public void SaleTest() {
            var response = _device.Sale(6.18m)
            .WithReferenceNumber(1)
            .WithPaymentMode(Api.Terminals.INGENICO.PaymentMode.APPLICATION)
            .WithCurrencyCode("826")
            .WithTableNumber("1")
            .Execute();

            Assert.IsNotNull(response);
        }

        [TestMethod]
        public void Reverse() {
            var response = _device.Reverse(amount: 6.18m)
                .WithReferenceNumber(12)
                .Execute();

            Assert.IsNotNull(response);
        }

        [TestMethod]
        public void SaleRefund() {
            var response = _device.Refund(6.18m)
            .WithReferenceNumber(1)
            .WithPaymentMode(Api.Terminals.INGENICO.PaymentMode.APPLICATION)
            .WithCurrencyCode("826")
            .WithTableNumber("1")
            .Execute();

            Assert.IsNotNull(response);
        }

        [TestMethod]
        public void preAuth() {
            var response = _device.Authorize(20.00m)
            .WithReferenceNumber(1)
            .WithPaymentMode(Api.Terminals.INGENICO.PaymentMode.APPLICATION)
            .WithCurrencyCode("826")
            .Execute();

            Assert.IsNotNull(response);
        }

        [TestMethod]
        public void SaleCancel() {
            var task1 = Task.Factory.StartNew(() => {
                var response = _device.Sale(523m)
                                .WithReferenceNumber(1)
                                .WithPaymentMode(Api.Terminals.INGENICO.PaymentMode.APPLICATION)
                                .WithCurrencyCode("826")
                                .Execute();

                Assert.IsNotNull(response);
            });

            var task2 = Task.Factory.StartNew(() => {
                Thread.Sleep(10000);
                _device.Cancel();
            });

            Task.WaitAll(task1, task2);
        }

        [TestMethod]
        public void TaxFree() {

            var splitR = _device
                .GetLastReceipt(ReceiptType.SPLITR)
                .Execute();

            string test = splitR.ReportData;

            if (splitR.ReportData.Contains("</CREDIT_CARD_RECEIPT>"))
                Assert.IsNotNull(splitR);
        }

        [TestMethod]
        public void Ticket() {
            ITerminalReport res = _device
                .GetLastReceipt(ReceiptType.TICKET)
                .Execute();

            string test = res.ReportData;

            if (res.ReportData.Contains("</CREDIT_CARD_RECEIPT>"))
                Assert.IsNotNull(res);
        }

        [TestMethod]
        public void EOD() {

            /** This example doesn't return XML/Report Data but it intiate End of Day 
            Report and the terminal will return EODOK if success.
            */
            ITerminalResponse res = _device
                .GetReport(ReportType.EOD)
                .Execute();

            Assert.IsNotNull(res);
        }

        [TestMethod]
        public void Cashback() {
            var response = _device.Sale(20.00m)
                    .WithReferenceNumber(01)
                    .WithPaymentMode(Api.Terminals.INGENICO.PaymentMode.APPLICATION)
                    .WithCurrencyCode("826")
                    .WithCashBack(2.00m)
                    .Execute();

            Assert.IsNotNull(response);
        }

        [TestMethod]
        public void AccountVerification() {

            var response = _device.Verify()
                    .WithReferenceNumber(01)
                    .WithPaymentMode(PaymentMode.MAILORDER)
                    .WithCurrencyCode("826")
                    .Execute();

            Assert.IsNotNull(response);
        }

        [TestMethod]
        public void test123() {
            var response = _device.Verify()
                    .WithReferenceNumber(01)
                    .WithPaymentMode(Api.Terminals.INGENICO.PaymentMode.APPLICATION)
                    .WithCurrencyCode("826")
                    .Execute();

            Assert.IsNotNull(response);
        }
    }
}
