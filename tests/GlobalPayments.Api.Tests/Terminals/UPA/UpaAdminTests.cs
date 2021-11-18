using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Terminals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.Terminals.UPA
{
    [TestClass]
    public class UpaAdminTests
    {
        IDeviceInterface _device;

        public UpaAdminTests() {
            _device = DeviceService.Create(new ConnectionConfig {
                DeviceType = DeviceType.NUCLEUS_SATURN_1000,
                ConnectionMode = ConnectionModes.TCP_IP,
                IpAddress = "192.168.1.6",
                Port = "8081",
                Timeout = 30000,
                RequestIdProvider = new RandomIdProvider()
            });
            Assert.IsNotNull(_device);
        }

        [TestMethod]
        public void LineItem()
        {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };
            _device.EcrId = "12";
            var response = _device.LineItem("11");
            Assert.IsNotNull(response);
            Assert.AreEqual("Success", response.Status);

            // Clear the device UI
            _device.Cancel();
        }

        [TestMethod]
        public void LineItemWithRight()
        {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };
            _device.EcrId = "12";
            var response = _device.LineItem("Toothpaste", "12");
            Assert.IsNotNull(response);
            Assert.AreEqual("Success", response.Status);

            // Clear the device UI
            _device.Cancel();
        }

        [TestMethod]
        public void Reboot()
        {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };
            _device.EcrId = "13";
            var response = _device.Reboot();
            Assert.IsNotNull(response);
            Assert.AreEqual("Success", response.Status);
        }

        [TestMethod]
        public void SendStoreAndForward()
        {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };
            _device.EcrId = "12";
            var response = _device.SendStoreAndForward();
            Assert.IsNotNull(response);
            Assert.AreEqual("Success", response.Status);
        }

        [TestMethod]
        public void Cancel()
        {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };
            _device.EcrId = "13";
            _device.Cancel();
        }
    }
}
