using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Terminals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.Terminals.Pax {
    [TestClass]
    public class PaxGiftTests {
        IDeviceInterface _device;

        public PaxGiftTests() {
            _device = DeviceService.Create(new ConnectionConfig {
                DeviceType = DeviceType.PAX_S300,
                ConnectionMode = ConnectionModes.HTTP,
                IpAddress = "10.12.220.172",
                Port = "10009"
            });
            Assert.IsNotNull(_device);
        }

        #region GiftSale
        [TestMethod]
        public void GiftSale() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
                Assert.IsTrue(message.StartsWith("[STX]T06[FS]1.35[FS]01[FS]1000[FS][FS]1[FS][FS][ETX]"));
            };

            var response = _device.GiftSale(1, 10m).Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void GiftSaleManual() {
            var card = new GiftCard { Number = "5022440000000000098" };

            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
                Assert.IsTrue(message.StartsWith("[STX]T06[FS]1.35[FS]01[FS]1000[FS]5022440000000000098[FS]2[FS][FS][ETX]"));
            };

            var response = _device.GiftSale(2, 10m).WithPaymentMethod(card).Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void GiftSaleWithInvoiceNumber() {
            var card = new GiftCard { Number = "5022440000000000098" };

            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
                Assert.IsTrue(message.StartsWith("[STX]T06[FS]1.35[FS]01[FS]800[FS]5022440000000000098[FS]4[US]1234[FS][FS][ETX]"));
            };

            var response = _device.GiftSale(4, 8m)
                .WithPaymentMethod(card)
                .WithInvoiceNumber("1234")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

        }

        [TestMethod]
        public void LoyaltySaleManual() {
            var card = new GiftCard { Number = "5022440000000000098" };

            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
                Assert.IsTrue(message.StartsWith("[STX]T08[FS]1.35[FS]01[FS]1000[FS]5022440000000000098[FS]5[FS][FS][ETX]"));
            };

            var response = _device.GiftSale(5, 10m)
                .WithCurrency(CurrencyType.POINTS)
                .WithPaymentMethod(card)
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
        public void GiftAddValueManual() {
            var card = new GiftCard { Number = "5022440000000000098" };

            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
                Assert.IsTrue(message.StartsWith("[STX]T06[FS]1.35[FS]10[FS]1000[FS]5022440000000000098[FS]8[FS][FS][ETX]"));
            };

            var response = _device.GiftAddValue(8)
                .WithPaymentMethod(card)
                .WithAmount(10m)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void GiftAddValue() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
                Assert.IsTrue(message.StartsWith("[STX]T06[FS]1.35[FS]10[FS]1000[FS][FS]9[FS][FS][ETX]"));
            };

            var response = _device.GiftAddValue(9)
                .WithAmount(10m)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void LoyaltyAddValueManual() {
            var card = new GiftCard() { Number = "5022440000000000098" };

            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
                Assert.IsTrue(message.StartsWith("[STX]T08[FS]1.35[FS]10[FS]800[FS]5022440000000000098[FS]10[FS][FS][ETX]"));
            };

            var response = _device.GiftAddValue(10)
                .WithPaymentMethod(card)
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
        public void GiftVoidManual() {
            var card = new GiftCard { Number = "5022440000000000098" };
            var saleResponse = _device.GiftSale(13).WithAmount(10m).WithPaymentMethod(card).Execute();

            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
                Assert.IsTrue(message.StartsWith("[STX]T06[FS]1.35[FS]16[FS][FS][FS]13[FS][FS]HREF=" + saleResponse.TransactionId + "[ETX]"));
            };

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
                Assert.IsTrue(message.StartsWith("[STX]T06[FS]1.35[FS]23[FS][FS][FS]16[FS][FS][ETX]"));
            };

            var response = _device.GiftBalance(16)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void GiftBalanceManual() {
            var card = new GiftCard { Number = "5022440000000000098" };

            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
                Assert.IsTrue(message.StartsWith("[STX]T06[FS]1.35[FS]23[FS][FS]5022440000000000098[FS]17[FS][FS][ETX]"));
            };

            var response = _device.GiftBalance(17)
                .WithPaymentMethod(card)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void LoyaltyBalanceManual() {
            var card = new GiftCard { Number = "5022440000000000098" };

            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
                Assert.IsTrue(message.StartsWith("[STX]T08[FS]1.35[FS]23[FS][FS]5022440000000000098[FS]18[FS][FS][ETX]"));
            };

            var response = _device.GiftBalance(18)
                .WithPaymentMethod(card)
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
