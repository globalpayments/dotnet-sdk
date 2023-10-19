using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Enums;
using GlobalPayments.Api.Entities.UPA;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Terminals;
using GlobalPayments.Api.Terminals.UPA;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System;

namespace GlobalPayments.Api.Tests.Terminals.UPA
{
    [TestClass]
    public class UpaAdminTests
    {
        IDeviceInterface _device;

        public UpaAdminTests() {
            _device = DeviceService.Create(new ConnectionConfig {
                DeviceType = DeviceType.UPA_DEVICE,
                ConnectionMode = ConnectionModes.TCP_IP,
                IpAddress = "192.168.1.130",
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
            _device.EcrId = "13";
            var response = _device.SendStoreAndForward();
            Assert.IsNotNull(response);
            Assert.AreEqual("Success", response.Status);
        }

        [TestMethod]
        public void PromptAndGetSignatureFile() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };
            _device.EcrId = "13";            
            var response = _device.PromptAndGetSignatureFile("please Sign","", null);
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.SignatureData);

            // save as bmp
            SaveSignatureFile(response.SignatureData, @"C:\temp\signature11.bmp");

            // convert and save as png
            var pngData = BmpToPng(response.SignatureData);
            SaveSignatureFile(pngData, @"C:\temp\signature1.png");
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

        [TestMethod]
        public void StartCardTransaction() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };
            _device.EcrId = "13";
            var paramObj = new UpaParam() {
                Timeout = 60,
                AcquisitionTypes = AcquisitionType.Swipe,
                Header = "Header",
                DisplayTotalAmount = "Y",
                PromptForManual = true,
                BrandIcon1 = 4,
                BrandIcon2 = 3
            };
            var upaIndicator = new ProcessingIndicator() {
                QuickChip = "Y",
                CheckLuhn = "Y",
                SecurityCode = "Y",
                CardTypeFilter = CardTypeFilter.GIFT
            };
            var upaTrans = new UpaTransactionData() {
                TotalAmount = 11.2m,
                CashBackAmount = 2.5m,
                TranDate = DateTime.Now,
                TranTime = DateTime.Now,
                TransType = TransactionType.Sale
            };

            var response = _device.StartCardTransaction(paramObj, upaIndicator, upaTrans);
            Assert.IsNotNull(response);
            Assert.AreEqual("Success", response.Status);
        }
        [TestMethod]
        public void DeleteSafWithSafReferenceNumber()
        {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };
            _device.EcrId = "13";
            var response = _device.DeleteSaf("P0000007");
            Assert.IsNotNull(response);
            Assert.AreEqual("Success", response.Status);
        }
        [TestMethod]
        public void DeleteSafWithSafReferenceNumberAndTranNo()
        {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };
            _device.EcrId = "13";
            var response = _device.DeleteSaf("P0000007", "0005");
            Assert.IsNotNull(response);
            Assert.AreEqual("Success", response.Status);
        }

        [TestMethod]
        public void RegisterPOS()
        {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };
            _device.EcrId = "13";

            var response = _device.RegisterPOS("com.global.testapp", 1, false, 1);
            Assert.IsNotNull(response);
            Assert.AreEqual("Success", response.Status);
        }

        #region helper methods
        private void SaveSignatureFile(byte[] signatureData, string filename) {
            using (var sw = new BinaryWriter(File.OpenWrite(filename)))
            {
                sw.Write(signatureData);
                sw.Flush();
            }
        }

        private byte[] BmpToPng(byte[] signatureData) {
            using (var ms = new MemoryStream(signatureData)) {
                using (var pngStream = new MemoryStream()) {
                    var bmp = new Bitmap(ms);
                    bmp.Save(pngStream, ImageFormat.Png);
                    return pngStream.ToArray();
                }
            }
        }
        #endregion
    }
}
