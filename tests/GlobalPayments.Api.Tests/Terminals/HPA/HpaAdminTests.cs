using System;
using System.Diagnostics;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Terminals;
using GlobalPayments.Api.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace GlobalPayments.Api.Tests.Terminals.HPA
{
    [TestClass]
    public class HpaAdminTests
    {
        IDeviceInterface _device;
        public HpaAdminTests() {
            _device = DeviceService.Create(new ConnectionConfig
            {
                DeviceType = DeviceType.HPA_ISC250,
                ConnectionMode = ConnectionModes.TCP_IP,
                IpAddress = "10.12.220.39",
                Port = "12345",
                Timeout = 60000,
                RequestIdProvider = new RequestIdProvider()
            });
            Assert.IsNotNull(_device);
        }

        [TestMethod]
        public void Cancel() {
            _device.Cancel();
        }

        [TestMethod]
        public void Initialize() {
            var response = _device.Initialize();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.DeviceResponseCode);
            Assert.IsNotNull(response.SerialNumber);
        }

        [TestMethod]
        public void OpenLane() {
            var response = _device.OpenLane();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.DeviceResponseCode);
        }

        [TestMethod]
        public void CloseLane() {
            var response = _device.CloseLane();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.DeviceResponseCode);
        }

        [TestMethod]
        public void Reset() {
            var response = _device.Reset();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.DeviceResponseCode);
        }

        [TestMethod, Ignore]
        public void Reboot() {
            _device.Reset();
        }

        [TestMethod]
        public void BatchClose() {
            _device.CloseLane();
            _device.OnMessageSent += (message) =>
            {
                Assert.AreEqual(message, "<SIP><Version>1.0</Version><ECRId>1004</ECRId><Request>CloseBatch</Request></SIP>");
            };

            var response = _device.BatchClose();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.DeviceResponseCode, response.DeviceResponseText);
        }

        [TestMethod, ExpectedException(typeof(UnsupportedTransactionException))]
        public void GetSignature_Direct() {
            _device.GetSignatureFile();
        }

        [TestMethod]
        public void GetSignature_Indirect() {
            _device.OnMessageSent += (message) =>
            {
                Assert.IsNotNull(message);
            };
            var response = _device.CreditSale( 120m)
               .WithSignatureCapture(true)
               .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.DeviceResponseCode);
            Assert.AreEqual("0", response.SignatureStatus);
            Assert.IsNotNull(response.SignatureData);
        }

        [TestMethod]
        public void PromptForSignature() {
            _device.OpenLane();

            var response = _device.PromptForSignature();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.DeviceResponseCode);
            Assert.IsNotNull(response.SignatureData);

            _device.Reset();
            _device.CloseLane();
        }
    }
}
