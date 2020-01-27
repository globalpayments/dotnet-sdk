using System.Threading;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Terminals;
using GlobalPayments.Api.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.Terminals.HPA {
    [TestClass]
    public class HpaGiftTests {
        IDeviceInterface _device;

        public HpaGiftTests() {
            _device = DeviceService.Create(new ConnectionConfig {
                DeviceType = DeviceType.HPA_ISC250,
                ConnectionMode = ConnectionModes.TCP_IP,
                IpAddress = "10.12.220.39",
                Port = "12345",
                Timeout = 30000,
                RequestIdProvider = new RandomIdProvider()
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

            var response = _device.Sale(1m)
                .WithPaymentMethodType(PaymentMethodType.Gift)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseText);
        }

        [TestMethod]
        public void GiftSaleWithInvoiceNumber() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };

            var response = _device.Sale(8m)
                .WithPaymentMethodType(PaymentMethodType.Gift)
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

            var response = _device.Sale(10m)
                .WithPaymentMethodType(PaymentMethodType.Gift)
                .WithCurrency(CurrencyType.POINTS)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void GiftSaleNoAmount() {
            _device.Sale()
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

        #region AddValue
        [TestMethod]
        public void AddValue() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };

            var response = _device.AddValue()
                .WithPaymentMethodType(PaymentMethodType.Gift)
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

            var response = _device.AddValue()
                .WithPaymentMethodType(PaymentMethodType.Gift)
               .WithCurrency(CurrencyType.POINTS)
               .WithAmount(8m)
               .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void AddValueNoAmount() {
            _device.AddValue()
                .WithPaymentMethodType(PaymentMethodType.Gift)
                .Execute();
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void AddValueNoCurrency() {
            _device.AddValue()
                .WithPaymentMethodType(PaymentMethodType.Gift)
                .WithCurrency(null)
                .Execute();
        }
        #endregion

        #region GiftVoid
        [TestMethod]
        public void GiftVoid() {
            var saleResponse = _device.Sale().WithAmount(10m).Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);

            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };

            WaitAndReset();

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
            };

            var response = _device.Balance()
                .WithPaymentMethodType(PaymentMethodType.Gift)
                .Execute();    
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void LoyaltyBalance() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };

            var response = _device.Balance()
                .WithPaymentMethodType(PaymentMethodType.Gift)
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
            var response = _device.AddValue().WithAmount(8m).Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Test_case_15c() {
            var response = _device.Sale(1m).Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        //public void test_case_15d() {
        //    var response = _device
        //    Assert.IsNotNull(response);
        //    Assert.AreEqual("00", response.ResponseCode);
        //}
        #endregion

        #region startcard
        [TestMethod]
        public void GiftStartCard() {
            var response = _device.StartCard(PaymentMethodType.Gift);
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.DeviceResponseCode);
        }
        #endregion
    }
}
