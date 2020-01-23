using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Terminals;
using GlobalPayments.Api.Terminals.HPA.Responses;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace GlobalPayments.Api.Tests.Terminals.HPA {
    [TestClass]
    public class HpaAdminTests {
        IDeviceInterface _device;
        public HpaAdminTests() {
            _device = DeviceService.Create(new ConnectionConfig {
                DeviceType = DeviceType.HPA_ISC250,
                ConnectionMode = ConnectionModes.TCP_IP,
                //IpAddress = "10.12.220.39",
                IpAddress = "192.168.0.94",
                Port = "12345",
                Timeout = 20000,
                RequestIdProvider = new RandomIdProvider()
            });
            Assert.IsNotNull(_device);
        }

        private void SaveSignatureFile(string transactionId, byte[] data) {
            using (StreamWriter sw = new StreamWriter(File.OpenWrite(string.Format(@"C:\temp\{0}.bmp", transactionId)))) {
                sw.Write(data);
                sw.Flush();
            }
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

        [TestMethod]
        public void Reboot() {
            var response = _device.Reboot();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.DeviceResponseCode);
        }

        [TestMethod]
        public void BatchClose() {
            _device.CloseLane();
            _device.OnMessageSent += (message) => {
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
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };

            var response = _device.Sale(120m)
               .WithSignatureCapture(true)
               .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.DeviceResponseCode, response.DeviceResponseText);
            Assert.AreEqual("0", response.SignatureStatus);
            Assert.IsNotNull(response.SignatureData);

            SaveSignatureFile(response.TransactionId, response.SignatureData);
        }

        [TestMethod]
        public void PromptForSignature() {
            _device.OpenLane();

            var response = _device.PromptForSignature();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.DeviceResponseCode, response.DeviceResponseText);
            Assert.IsNotNull(response.SignatureData);

            _device.Reset();
            _device.CloseLane();

            SaveSignatureFile("hpa-signature-prompt", response.SignatureData);
        }

        [TestMethod]
        public void SendStoreAndForward() {
            var response = _device.SendStoreAndForward();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.DeviceResponseCode);
        }
        
        [TestMethod]
        public void LineItem() {
            var response = _device.LineItem("Green Beans");
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.DeviceResponseCode);
        }

        [TestMethod]
        public void LineItemWithParams() {
            var response = _device.LineItem("Green Beans", "$0.59", "TOTAL", "$1.19");
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.DeviceResponseCode);
        }

        [TestMethod]
        public void EnableStoreAndForwardMode() {
            var response = _device.SetStoreAndForwardMode(true);
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.DeviceResponseCode);
        }

        [TestMethod]
        public void SendFile() {
            try {
                SipSendFileResponse response = (SipSendFileResponse)_device.SendFile(SendFileType.Logo, @"C:\temp\IDLELOGO.JPG");
                Assert.IsNotNull(response);
                Assert.AreEqual("00", response.DeviceResponseCode);
            }
            catch (ApiException exc) {
                Assert.Fail(exc.Message);
            }
        }
    }
}
