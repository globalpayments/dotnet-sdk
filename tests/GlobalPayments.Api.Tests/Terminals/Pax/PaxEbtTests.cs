using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Terminals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.Terminals.Pax {
    [TestClass]
    public class PaxEbtTests {
        IDeviceInterface _device;

        public PaxEbtTests() {
            _device = DeviceService.Create(new ConnectionConfig {
                DeviceType = DeviceType.PAX_S300,
                ConnectionMode = ConnectionModes.HTTP,
                IpAddress = "10.12.220.172",
                Port = "10009",
                RequestIdProvider = new RandomIdProvider()
            });
            Assert.IsNotNull(_device);
        }

        [TestMethod]
        public void EbtFoodstampPurchase() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
                //Assert.IsTrue(message.StartsWith("[STX]T04[FS]1.35[FS]01[FS]1000[FS][US][US][US]F[US][US]1[FS]1[FS][FS][ETX]"));
            };
        
            var response = _device.Sale(10m)
                .WithPaymentMethodType(PaymentMethodType.EBT)
                .WithCurrency(CurrencyType.FOODSTAMPS)
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void EbtCashBenefitPurchase() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
                //Assert.IsTrue(message.StartsWith("[STX]T04[FS]1.35[FS]01[FS]1000[FS][US][US][US]C[US][US]1[FS]2[FS][FS][ETX]"));
            };

            var response = _device.Sale (10m)
                .WithPaymentMethodType(PaymentMethodType.EBT)
                .WithCurrency(CurrencyType.CASH_BENEFITS)
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void EbtPurchaseWithCashback() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
                //Assert.IsTrue(message.StartsWith("[STX]T04[FS]1.35[FS]01[FS]1000[US][US]1000[FS][US][US][US]C[US][US]1[FS]1[FS][FS][ETX]"));
            };

            var response = _device.Sale(10m)
                .WithPaymentMethodType(PaymentMethodType.EBT)
                .WithCurrency(CurrencyType.CASH_BENEFITS)
                .WithAllowDuplicates(true)
                .WithCashBack(10m)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void EbtVoucherPurchase() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
                //Assert.IsTrue(message.StartsWith("[STX]T04[FS]1.35[FS]01[FS]1000[FS][US][US][US]V[US][US]1[FS]3[FS][FS][ETX]"));
            };

            var response = _device.Sale(10m)
                .WithPaymentMethodType(PaymentMethodType.EBT)
                .WithCurrency(CurrencyType.VOUCHER)
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void EbtFoodstampBalanceInquiry() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
                //Assert.IsTrue(message.StartsWith("[STX]T04[FS]1.35[FS]23[FS][FS][US][US][US]F[FS]5[FS][FS][ETX]"));
            };

            var response = _device.Balance()
                .WithPaymentMethodType(PaymentMethodType.EBT)
                .WithCurrency(CurrencyType.FOODSTAMPS)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void EbtCashBenefitsBalanceInquiry() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
                //Assert.IsTrue(message.StartsWith("[STX]T04[FS]1.35[FS]23[FS][FS][US][US][US]C[US][US]1[FS]6[FS][FS][ETX]"));
            };

            var response = _device.Balance()
                .WithPaymentMethodType(PaymentMethodType.EBT)
                .WithCurrency(CurrencyType.CASH_BENEFITS)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void EbtBalanceInquiryWithVoucher() {
            _device.Balance()
                .WithPaymentMethodType(PaymentMethodType.EBT)
                .WithCurrency(CurrencyType.VOUCHER)
                .Execute();
        }

        [TestMethod]
        public void EbtFoodstampRefund() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
                //Assert.IsTrue(message.StartsWith("[STX]T04[FS]1.35[FS]02[FS]1000[FS][US][US][US]F[FS]9[FS][FS][ETX]"));
            };

            var response = _device.Refund(10m)
                .WithPaymentMethodType(PaymentMethodType.EBT)
                .WithCurrency(CurrencyType.FOODSTAMPS)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.DeviceResponseText);
        }

        [TestMethod]
        public void EbtCashBenefitRefund() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
                //Assert.IsTrue(message.StartsWith("[STX]T04[FS]1.35[FS]02[FS]1000[FS][US][US][US]F[FS]10[FS][FS][ETX]"));
            };

            var response = _device.Refund(10m)
                .WithPaymentMethodType(PaymentMethodType.EBT)
                .WithCurrency(CurrencyType.FOODSTAMPS)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void EbtRefundAllowDup() {
            _device.Refund(11m)
                .WithPaymentMethodType(PaymentMethodType.EBT)
                .WithAllowDuplicates(true)
                .Execute();
        }

        [TestMethod]
        public void EbtCashBenefitWithdrawal() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
                //Assert.IsTrue(message.StartsWith("[STX]T04[FS]1.35[FS]07[FS]1000[FS][US][US][US]C[FS]12[FS][FS][ETX]"));
            };

            var response = _device.Withdrawal(10m)
                .WithPaymentMethodType(PaymentMethodType.EBT)
                .WithCurrency(CurrencyType.CASH_BENEFITS)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void EbtBenefitWithdrawlAllowDup() {
            _device.Withdrawal(10m)
                .WithPaymentMethodType(PaymentMethodType.EBT)
                .WithAllowDuplicates(true)
                .Execute();
        }
    }
}
