using System;
using System.Threading;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Terminals;
using GlobalPayments.Api.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.Terminals.HPA
{
    [TestClass]
    public class HpaCreditTests
    {
        IDeviceInterface _device;
        public HpaCreditTests() { 
            _device = DeviceService.Create(new ConnectionConfig
            {
                DeviceType = DeviceType.HPA_ISC250,
                ConnectionMode = ConnectionModes.TCP_IP,
                IpAddress = "10.12.220.39",
                Port = "12345",
                Timeout = 30000,
                RequestIdProvider = new RequestIdProvider()
            });
            Assert.IsNotNull(_device);
            _device.OpenLane();
        }

        [TestCleanup]
        public void WaitAndReset() {
            Thread.Sleep(3000);
            _device.Reset();
        }

        [TestMethod]
        public void CreditSale() {
            _device.OnMessageSent += (message) =>
            {
                Assert.IsNotNull(message);
            };
            var response = _device.CreditSale(10m)
               .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void CreditSaleWithSignatureCapture() {
            _device.OnMessageSent += (message) =>
            {
                Assert.IsNotNull(message);
            };
            var response = _device.CreditSale(12m)
           .WithSignatureCapture(true)
           .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void CreditSaleNoAmount() {
            _device.CreditSale().Execute();
        }

        [TestMethod]
        public void CreditAuth() {
            _device.OnMessageSent += (message) =>
            {
                Assert.IsNotNull(message);
            };
            var response = _device.CreditAuth(12m)
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            WaitAndReset();

            var captureResponse = _device.CreditCapture(12m)
                .WithTransactionId(response.TransactionId)
                .Execute();
            Assert.IsNotNull(captureResponse);
            Assert.AreEqual("00", captureResponse.ResponseCode, captureResponse.ResponseText);
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void CreditAuthNoAmount() {
            _device.CreditAuth().Execute();
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void CaptureNoTransactionId() {
            _device.CreditCapture().Execute();
        }

        [TestMethod]
        public void CreditRefundByCard() {
            _device.OnMessageSent += (message) =>
            {
                Assert.IsNotNull(message);
            };
            var returnResponse = _device.CreditRefund(14m).Execute();
            Assert.IsNotNull(returnResponse);
            Assert.AreEqual("00", returnResponse.ResponseCode, returnResponse.ResponseText);
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void CreditRefundNoAmount() {
            _device.CreditRefund().Execute();
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void CreditRefundByTransactionIdNoAuthCode() {
            _device.CreditRefund(13m)
            .WithTransactionId("1234567")
            .Execute();
        }

        [TestMethod]
        public void CreditVerify() {
            _device.OnMessageSent += (message) =>
            {
                Assert.IsNotNull(message);
            };
            var response = _device.CreditVerify().Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void CreditVoid() {
            var str_message = string.Empty;
            _device.OnMessageSent += (message) =>
            {
                str_message = message;
                Assert.IsNotNull(message);
            };
            var saleResponse = _device.CreditSale(17m).Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            WaitAndReset();

            var voidResponse = _device.CreditVoid()
                .WithTransactionId(saleResponse.TransactionId)
                .Execute();
            Assert.IsNotNull(voidResponse);
            Assert.AreEqual("00", voidResponse.ResponseCode, voidResponse.ResponseText);
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void CreditVoidNoTransactionId() {
            _device.CreditVoid().Execute();
        }

        [TestMethod]
        public void CreditStartCard() {
            var response = _device.StartCard(PaymentMethodType.Credit);
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.DeviceResponseCode);
        }
    }
}
