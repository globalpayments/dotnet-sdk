using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Terminals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.Terminals.UPA
{
    [TestClass]
    public class UpaEbtTests
    {
        IDeviceInterface _device;

        public UpaEbtTests() {
            _device = DeviceService.Create(new ConnectionConfig {
                DeviceType = DeviceType.NUCLEUS_SATURN_1000,
                ConnectionMode = ConnectionModes.TCP_IP,
                IpAddress = "192.168.1.10",
                Port = "8081",
                RequestIdProvider = new RandomIdProvider()
            });
            Assert.IsNotNull(_device);
        }

        [TestMethod]
        public void EbtFoodstampPurchase() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };
        
            var response = _device.Sale(10m).WithEcrId(13)
                .WithPaymentMethodType(PaymentMethodType.EBT)
                .WithClerkId(13)
                .WithAmount(134.12m)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void EbtCashBenefitPurchase() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };

            var response = _device.Sale (10m).WithEcrId(13)
                .WithPaymentMethodType(PaymentMethodType.EBT)
                .WithAmount(345.12m)
                .WithGratuity(12.02m)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void EbtVoucherPurchase() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };

            var response = _device.Sale(10m).WithEcrId(13)
                .WithPaymentMethodType(PaymentMethodType.EBT)
                .WithAmount(123.10m)
                .WithInvoiceNumber("11223")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void EbtFoodstampBalanceInquiry()
        {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };

            var response = _device.Balance().WithEcrId(13)
                .WithPaymentMethodType(PaymentMethodType.EBT)
                .WithCurrency(CurrencyType.FOODSTAMPS)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void EbtCashBenefitsBalanceInquiry()
        {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };

            var response = _device.Balance().WithEcrId(13)
                .WithPaymentMethodType(PaymentMethodType.EBT)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void CreditTipAdjust()
        {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };

            var response = _device.TipAdjust()
                .WithPaymentMethodType(PaymentMethodType.EBT)
                .WithEcrId(13)
                .WithClerkId(123)
                .WithGratuity(12.12m)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void CreditEndOfDay()
        {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };

            _device.EcrId = "13";
            var response = _device.EndOfDay();
            Assert.IsNotNull(response);
        }
        [TestMethod]
        public void CreditVerify()
        {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };

            var response = _device.Verify()
                .WithPaymentMethodType(PaymentMethodType.EBT)
                .WithEcrId(13)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }
    }
}
