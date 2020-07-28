using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Terminals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.Terminals.Pax {
    [TestClass]
    public class PaxGiftTests {
        private IDeviceInterface _device;
        private GiftCard card;

        public PaxGiftTests() {
            _device = DeviceService.Create(new ConnectionConfig {
                DeviceType = DeviceType.PAX_S300,
                ConnectionMode = ConnectionModes.TCP_IP,
                IpAddress = "10.12.220.172",
                Port = "10009",
                RequestIdProvider = new RandomIdProvider()
            });
            Assert.IsNotNull(_device);

            card = new GiftCard {
                Number = "5022440000000000098"
            };
        }

        #region GiftSale
        [TestMethod]
        public void GiftSale() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
                //Assert.IsTrue(message.StartsWith("[STX]T06[FS]1.35[FS]01[FS]1000[FS][FS]1[FS][FS][ETX]"));
            };

            var response = _device.Sale(10m)
                .WithPaymentMethodType(PaymentMethodType.Gift)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void GiftSaleManual() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
                //Assert.IsTrue(message.StartsWith("[STX]T06[FS]1.35[FS]01[FS]1000[FS]5022440000000000098[FS]2[FS][FS][ETX]"));
            };

            var response = _device.Sale(10m)
                .WithPaymentMethod(card)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void GiftSaleWithInvoiceNumber() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
                //Assert.IsTrue(message.StartsWith("[STX]T06[FS]1.35[FS]01[FS]800[FS]5022440000000000098[FS]4[US]1234[FS][FS][ETX]"));
            };

            var response = _device.Sale(8m)
                .WithPaymentMethod(card)
                .WithInvoiceNumber("1234")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

        }

        [TestMethod]
        public void LoyaltySaleManual() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
                //Assert.IsTrue(message.StartsWith("[STX]T08[FS]1.35[FS]01[FS]1000[FS]5022440000000000098[FS]5[FS][FS][ETX]"));
            };

            var response = _device.Sale(10m)
                .WithCurrency(CurrencyType.POINTS)
                .WithPaymentMethod(card)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void GiftSaleNoAmount() {
            _device.Sale(6m)
                .WithPaymentMethodType(PaymentMethodType.Gift)
                .Execute();
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void GiftSaleNoCurrency() {
            _device.Sale(10m)
                .WithPaymentMethodType(PaymentMethodType.Gift)
                .WithCurrency(null)
                .Execute();
        }
        #endregion

        #region GiftAddValue
        [TestMethod]
        public void GiftAddValueManual() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
                //Assert.IsTrue(message.StartsWith("[STX]T06[FS]1.35[FS]10[FS]1000[FS]5022440000000000098[FS]8[FS][FS][ETX]"));
            };

            var response = _device.AddValue(8m)
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
                //Assert.IsTrue(message.StartsWith("[STX]T06[FS]1.35[FS]10[FS]1000[FS][FS]9[FS][FS][ETX]"));
            };

            var response = _device.AddValue(9m)
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
                //Assert.IsTrue(message.StartsWith("[STX]T08[FS]1.35[FS]10[FS]800[FS]5022440000000000098[FS]10[FS][FS][ETX]"));
            };

            var response = _device.AddValue(10m)
                .WithPaymentMethod(card)
                .WithCurrency(CurrencyType.POINTS)
                .WithAmount(8m)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void GiftAddValueNoAmount() {
            _device.AddValue(11m).Execute();
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void GiftAddValueNoCurrency() {
            _device.AddValue(12m).WithCurrency(null).Execute();
        }
        #endregion

        #region GiftVoid
        [TestMethod]
        public void GiftVoidManual() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
                //Assert.IsTrue(message.StartsWith("[STX]T06[FS]1.35[FS]16[FS][FS][FS]13[FS][FS]HREF=" + saleResponse.TransactionId + "[ETX]"));
            };

            var saleResponse = _device.Sale(13m)
                .WithPaymentMethod(card)
                .Execute();

            var voidResponse = _device.Void()
                .WithPaymentMethodType(PaymentMethodType.Gift)
                .WithTransactionId(saleResponse.TransactionId)
                .Execute();

            Assert.IsNotNull(voidResponse);
            Assert.AreEqual("00", voidResponse.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void GiftVoidNoCurrency() {
            _device.Void()
                .WithPaymentMethodType(PaymentMethodType.Gift)
                .WithCurrency(null)
                .WithTransactionId("1")
                .Execute();
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void GiftVoidNoTransactionId() {
            _device.Void()
                .WithPaymentMethodType(PaymentMethodType.Gift)
                .WithTransactionId(null)
                .Execute();
        }
        #endregion

        #region GiftBalance
        [TestMethod]
        public void GiftBalance() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
                //Assert.IsTrue(message.StartsWith("[STX]T06[FS]1.35[FS]23[FS][FS][FS]16[FS][FS][ETX]"));
            };

            var response = _device.Balance()
                .WithPaymentMethodType(PaymentMethodType.Gift)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void GiftBalanceManual() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
                //Assert.IsTrue(message.StartsWith("[STX]T06[FS]1.35[FS]23[FS][FS]5022440000000000098[FS]17[FS][FS][ETX]"));
            };

            var response = _device.Balance()
                .WithPaymentMethod(card)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void LoyaltyBalanceManual() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
                //Assert.IsTrue(message.StartsWith("[STX]T08[FS]1.35[FS]23[FS][FS]5022440000000000098[FS]18[FS][FS][ETX]"));
            };

            var response = _device.Balance()
                .WithPaymentMethod(card)
                .WithCurrency(CurrencyType.POINTS)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void GiftBalanceNoCurrency() {
            _device.Balance()
                .WithPaymentMethodType(PaymentMethodType.Gift)
                .WithCurrency(null)
                .Execute();
        }
        #endregion

        #region Certification
        [TestMethod]
        public void Test_case_15a() {
            var response = _device.Balance()
                .WithPaymentMethodType(PaymentMethodType.Gift)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.AreEqual(10m, response.BalanceAmount);
        }

        [TestMethod]
        public void Test_case_15b() {
            var response = _device.AddValue(2m)
                .WithAmount(8m)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Test_case_15c() {
            var response = _device.Sale(1m)
                .WithPaymentMethodType(PaymentMethodType.Gift)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }
        #endregion
    }
}
