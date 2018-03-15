using System;
using System.Threading;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Terminals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.Terminals.HeartSIP
{
    [TestClass]
    public class HsipEbtTests {
        IDeviceInterface _device;

        public HsipEbtTests() {
            _device = DeviceService.Create(new ConnectionConfig {
                DeviceType = DeviceType.HSIP_ISC250,
                ConnectionMode = ConnectionModes.TCP_IP,
                IpAddress = "10.12.220.130",
                Port = "12345",
                Timeout = 30000
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

            var response = _device.EbtPurchase(1, 10m)
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

            var response = _device.EbtBalance(5).Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void EbtRefund() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };

            var response = _device.EbtRefund(9, 10m).Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.DeviceResponseText);
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void EbtRefundAllowDup() {
            _device.EbtRefund(11).WithAllowDuplicates(true).Execute();
        }

        [TestMethod, ExpectedException(typeof(UnsupportedTransactionException))]
        public void EbtCashBenefitWithdrawal() {
            var response = _device.EbtWithdrawl(12, 10m).Execute();
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void EbtBenefitWithdrawlAllowDup() {
            _device.EbtWithdrawl(13, 10m).WithAllowDuplicates(true).Execute();
        }
    }
}
