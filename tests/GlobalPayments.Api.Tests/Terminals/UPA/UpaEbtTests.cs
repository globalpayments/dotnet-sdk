using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Terminals;
using GlobalPayments.Api.Utils.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.Terminals.UPA
{
    [TestClass]
    public class UpaEbtTests
    {
        IDeviceInterface _device;

        public UpaEbtTests() {
            _device = DeviceService.Create(new ConnectionConfig {
                DeviceType = DeviceType.UPA_DEVICE,
                ConnectionMode = ConnectionModes.TCP_IP,
                IpAddress = "192.168.8.181",
                Port = "8081",
                Timeout = 20000,
                RequestIdProvider = new RandomIdProvider(),
                LogManagementProvider = new RequestConsoleLogger()
            });
            Assert.IsNotNull(_device);
        }
        
        [TestMethod]
        public void EbtPurchase() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };
        
            var response = _device.Sale(9)
                .WithPaymentMethodType(PaymentMethodType.EBT)
                .Execute();
            
            Assert.IsNotNull(response);
            Assert.AreEqual("Success", response.DeviceResponseText);
            Assert.AreEqual("00", response.DeviceResponseCode);
        }

        [TestMethod]
        public void EbtFoodstampPurchase() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };
        
            var response = _device.Sale().WithEcrId(13)
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

            var response = _device.Sale ().WithEcrId(13)
                .WithPaymentMethodType(PaymentMethodType.EBT)
                .WithAmount(345.12m)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void EbtVoucherPurchase() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };

            var response = _device.Sale().WithEcrId(13)
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
        public void EbtBalance() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };

            var response = _device.Balance()
                .WithPaymentMethodType(PaymentMethodType.EBT)
                .Execute();
            
            Assert.IsNotNull(response);
            Assert.AreEqual("Success", response.DeviceResponseText);
            Assert.AreEqual("00", response.DeviceResponseCode);
        }

        [TestMethod]
        public void EndOfDay()
        {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };

            _device.EcrId = "13";
            var response = _device.EndOfDay();
            Assert.IsNotNull(response);
            Assert.AreEqual("Success", response.DeviceResponseText);
            Assert.AreEqual("00", response.DeviceResponseCode);
        }
    }
}
