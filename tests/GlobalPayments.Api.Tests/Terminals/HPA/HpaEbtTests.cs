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
    public class HpaEbtTests {
        IDeviceInterface _device;

        public HpaEbtTests() {
            _device = DeviceService.Create(new ConnectionConfig {
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
        public void EbtPurchase() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };
            var response = _device.EbtPurchase(10m)
              .WithAllowDuplicates(true)
              .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void EbtBalanceInquiry() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };
            var response = _device.EbtBalance().Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void EbtRefund() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };
            var response = _device.EbtRefund(10m).Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.DeviceResponseText);
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void EbtRefundAllowDup() {
            _device.EbtRefund().WithAllowDuplicates(true).Execute();
        }

        [TestMethod, ExpectedException(typeof(UnsupportedTransactionException))]
        public void EbtCashBenefitWithdrawal() {
            var response = _device.EbtWithdrawl(10m).Execute();
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void EbtBenefitWithdrawlAllowDup() {
            _device.EbtWithdrawl(10m).WithAllowDuplicates(true).Execute();
        }

        [TestMethod]
        public void EbtStartCard() {
            var response = _device.StartCard(PaymentMethodType.EBT);
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.DeviceResponseCode);
        }
    }
}
