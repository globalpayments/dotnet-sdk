using System.Threading;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Terminals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.Terminals.HeartSIP {
    [TestClass]
    public class HsipGiftTests {
        IDeviceInterface _device;

        public HsipGiftTests() {
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

        #region GiftSale
        [TestMethod]
        public void GiftSale() {
            var str_message = string.Empty;
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
                str_message = message;
            };

            var response = _device.GiftSale(1, 1m).Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseText);
        }

        [TestMethod]
        public void GiftSaleWithInvoiceNumber() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };

            var response = _device.GiftSale(4, 8m)
                .WithInvoiceNumber("1234")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

        }

        [TestMethod]
        public void LoyaltySale() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };

            var response = _device.GiftSale(5, 10m)
                .WithCurrency(CurrencyType.POINTS)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void GiftSaleNoAmount() {
            _device.GiftSale(6).Execute();
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void GiftSaleNoCurrency() {
            _device.GiftSale(7, 10m).WithCurrency(null).Execute();
        }
        #endregion

        #region GiftAddValue
        [TestMethod]
        public void GiftAddValue() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };

            var response = _device.GiftAddValue(9)
                .WithAmount(10m)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void LoyaltyAddValue() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };

            var response = _device.GiftAddValue(10)
                .WithCurrency(CurrencyType.POINTS)
                .WithAmount(8m)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void GiftAddValueNoAmount() {
            _device.GiftAddValue(11).Execute();
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void GiftAddValueNoCurrency() {
            _device.GiftAddValue(12).WithCurrency(null).Execute();
        }
        #endregion

        #region GiftVoid
        [TestMethod]
        public void GiftVoid() {
            var saleResponse = _device.GiftSale(13).WithAmount(10m).Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);

            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };

            WaitAndReset();

            var voidResponse = _device.GiftVoid(13)
                .WithTransactionId(saleResponse.TransactionId)
                .Execute();
            Assert.IsNotNull(voidResponse);
            Assert.AreEqual("00", voidResponse.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void GiftVoidNoCurrency() {
            _device.GiftVoid(14)
                .WithCurrency(null)
                .WithTransactionId("1")
                .Execute();
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void GiftVoidNoTransactionId() {
            _device.GiftVoid(15).WithTransactionId(null).Execute();
        }
        #endregion

        #region GiftBalance
        [TestMethod]
        public void GiftBalance() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };

            var response = _device.GiftBalance(16)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void LoyaltyBalance() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };

            var response = _device.GiftBalance(18)
                .WithCurrency(CurrencyType.POINTS)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void GiftBalanceNoCurrency() {
            _device.GiftBalance(19).WithCurrency(null).Execute();
        }
        #endregion

        #region Certification
        [TestMethod]
        public void test_case_15a() {
            var response = _device.GiftBalance(1).Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.AreEqual(10m, response.BalanceAmount);
        }

        [TestMethod]
        public void test_case_15b() {
            var response = _device.GiftAddValue(2).WithAmount(8m).Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void test_case_15c() {
            var response = _device.GiftSale(3, 1m).Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        //public void test_case_15d() {
        //    var response = _device
        //    Assert.IsNotNull(response);
        //    Assert.AreEqual("00", response.ResponseCode);
        //}
        #endregion
    }
}
