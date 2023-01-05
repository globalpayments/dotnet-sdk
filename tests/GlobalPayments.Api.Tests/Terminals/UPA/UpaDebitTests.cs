using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Terminals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.Terminals.UPA
{
    [TestClass]
    public class UpaDebitTests
    {
        IDeviceInterface _device;

        public UpaDebitTests() {
            _device = DeviceService.Create(new ConnectionConfig {
                DeviceType = DeviceType.UPA_DEVICE,
                ConnectionMode = ConnectionModes.TCP_IP,
                IpAddress = "192.168.1.6",
                Port = "8081",
                RequestIdProvider = new RandomIdProvider()
            });
            Assert.IsNotNull(_device);
        }

        [TestMethod]
        public void DebitSale() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };

            var response = _device.Sale()
                .WithEcrId(13)
                .WithPaymentMethodType(PaymentMethodType.Debit)
                .WithClerkId(234)
                .WithAmount(567.10m)
                .WithTaxAmount(13.11m)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void DebitSaleNoAmount() {
            _device.Sale(5m)
                .WithEcrId(13)
                .WithPaymentMethodType(PaymentMethodType.Debit)
                .WithAmount(11.00m)
                .Execute();
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
        }
    }
}
