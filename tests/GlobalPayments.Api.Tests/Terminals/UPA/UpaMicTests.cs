using System;
using System.IO;
using System.Threading;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Terminals;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Tests.GpApi;
using GlobalPayments.Api.Utils.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.Terminals.UPA
{
    [TestClass]
    public class UpaMicTests : BaseGpApiTests {
        private IDeviceInterface _device;

        [TestInitialize]
        public void TestInitialize() {
            _device = DeviceService.Create(new ConnectionConfig {
                DeviceType = DeviceType.UPA_DEVICE,
                ConnectionMode = ConnectionModes.MIC,
                GatewayConfig = new GpApiConfig {
                    AppId = MitcUpaAppId,
                    AppKey = MitcUpaAppKey,
                    Channel = Channel.CardPresent,
                    Country = "CA",
                    DeviceCurrency = "CAD",
                    EnableLogging = true,
                    RequestLogger = new RequestConsoleLogger(),
                    MethodNotificationUrl = "https://ensi808o85za.x.pipedream.net/",
                    AccessTokenInfo = new AccessTokenInfo {
                        TransactionProcessingAccountName = "9187"
                    }
                },
                Timeout = 30000,
                RequestIdProvider = new RandomIdProvider()
            });
        }
        
        [TestMethod]
        public void Ping() {
            try
            {
                _device.EcrId = "1";
                _device.Ping();
            }
            catch (ApiException exc) {
                Assert.Fail(exc.Message + exc.StackTrace);
            }
        }
        
        [TestMethod]
        public void CreditSale() {
            try {
                ITerminalResponse response = _device.Sale(10m)
                    .WithEcrId(13)
                    .Execute();
                Assert.IsNotNull(response);
                Assert.AreEqual("00", response.DeviceResponseCode);
                Assert.AreEqual("COMPLETE", response.DeviceResponseText);
            }
            catch (ApiException exc) {
                Assert.Fail(exc.Message + exc.StackTrace);
            }
        }
        
        [TestMethod]
        public void CreditSale_WithZeroTip() {
            try {
                ITerminalResponse response = _device.Sale(10m)
                    .WithGratuity(0m)
                    .WithEcrId(13)
                    .Execute();
                Assert.IsNotNull(response);
                Assert.AreEqual("00", response.DeviceResponseCode);
                Assert.AreEqual("COMPLETE", response.DeviceResponseText);
            }
            catch (ApiException exc) {
                Assert.Fail(exc.Message + exc.StackTrace);
            }
        }
        
        [TestMethod]
        public void CreditSale_WithTip() {
            try {
                ITerminalResponse response = _device.Sale(10m)
                    .WithGratuity(1m)
                    .WithEcrId(13)
                    .Execute();
                Assert.IsNotNull(response);
                Assert.AreEqual("1.00", response.TipAmount.ToString());
                Assert.AreEqual("11.00", response.TransactionAmount.ToString());
                Assert.AreEqual("00", response.DeviceResponseCode);
                Assert.AreEqual("COMPLETE", response.DeviceResponseText);
            }
            catch (ApiException exc) {
                Assert.Fail(exc.Message + exc.StackTrace);
            }
        }
        
        [TestMethod]
        public void CreditSale_EcrNotSet() {
            try {
                ITerminalResponse response = _device.Sale(10m)
                    .Execute();
                Assert.IsNotNull(response);
                Assert.AreEqual("00", response.DeviceResponseCode);
                Assert.AreEqual("COMPLETE", response.DeviceResponseText);
            }
            catch (ApiException exc) {
                Assert.Fail(exc.Message + exc.StackTrace);
            }
        }

        [TestMethod]
        public void AddLineItem() {
            try {
                _device.EcrId = "1";
                
                IDeviceResponse response = _device.LineItem("Line Item #1", "10.00");

                Assert.IsNotNull(response);
                Assert.AreEqual("00", response.DeviceResponseCode);
                Assert.AreEqual("COMPLETE", response.DeviceResponseText);
            }
            catch (ApiException exc) {
                Assert.Fail(exc.Message + exc.StackTrace);
            }
        }

        [TestMethod]
        public void CreditAuth_Declined()
        {
            try
            {
                ITerminalResponse response = _device.Authorize(10m)
                    .Execute();
                Assert.IsNotNull(response);
                Assert.AreEqual("ERROR", response.ResponseText);
                Assert.AreEqual("DECLINED", response.DeviceResponseText);
            }
            catch (ApiException exc)
            {
                Assert.Fail(exc.Message + exc.StackTrace);
            }
        }

        [TestMethod]
        public void CreditAuthCompletion() {
            try {
                ITerminalResponse capture = _device.Capture(10m)
                    .WithEcrId("1")
                    .WithTransactionId("0001")
                    .Execute();
                
                Assert.IsNotNull(capture);
                Assert.AreEqual("ERROR", capture.ResponseText);
                Assert.AreEqual("DECLINED", capture.DeviceResponseText);
            }
            catch (ApiException exc) {
                Assert.Fail(exc.Message + exc.StackTrace);
            }
        }
        
        [TestMethod]
        public void CreditAuthAndCaptureWithClientTransactionId() {
            try {
                ITerminalResponse response = _device.Authorize(10m)
                    .WithClientTransactionId("asdcdf999")
                    .Execute();
                Assert.IsNotNull(response);
                Assert.AreEqual("00", response.DeviceResponseCode);
                Assert.AreEqual("INITIATED", response.DeviceResponseText);

                ITerminalResponse capture = _device.Capture(10m)
                    .WithClientTransactionId("asdcdf999")
                    .WithTransactionId(response.TransactionId)
                    .Execute();
                Assert.IsNotNull(capture);
                Assert.AreEqual("00", capture.DeviceResponseCode);
                Assert.AreEqual("INITIATED", capture.DeviceResponseText);
            }
            catch (ApiException exc) {
                Assert.Fail(exc.Message + exc.StackTrace);
            }
        }
        
        [TestMethod]
        public void CreditDeletePreAuth() {
            try {
                ITerminalResponse capture = _device.DeletePreAuth()
                    .WithTransactionId("0001")
                    .WithEcrId("1")
                    .Execute();
                Assert.IsNotNull(capture);
                Assert.AreEqual("ERROR", capture.ResponseText);
                Assert.AreEqual("DECLINED", capture.DeviceResponseText);
            }
            catch (ApiException exc) {
                Assert.Fail(exc.Message + exc.StackTrace);
            }
        }
        
        [TestMethod]
        public void GetOpenTabDetails() {
            try {
                _device.EcrId = "1";
                ITerminalReport response = _device.GetOpenTabDetails()
                    .Execute();
                Assert.IsNotNull(response);
                Assert.AreEqual("00", response.DeviceResponseCode);
                Assert.AreEqual("COMPLETE", response.DeviceResponseText);
            }
            catch (ApiException exc) {
                Assert.Fail(exc.Message + exc.StackTrace);
            }
        }

        [TestMethod]
        public void CreditCapture_RandomId() {
            try {
                ITerminalResponse response = _device.Capture(10m)
                    .WithTransactionId(Guid.NewGuid().ToString())
                    .WithEcrId("1")
                    .Execute();

                Assert.IsNotNull(response);
                Assert.AreEqual("ERROR", response.ResponseText);
                Assert.AreEqual("DECLINED", response.DeviceResponseText);
            }
            catch (ApiException exc) {
                Assert.Fail(exc.Message + exc.StackTrace);
            }
        }

        [TestMethod]
        public void CreditRefund() {
            try {
                ITerminalResponse response = _device.Refund(10m)
                    .Execute();
                Assert.IsNotNull(response);
                Assert.AreEqual("00", response.DeviceResponseCode);
                Assert.AreEqual("COMPLETE", response.DeviceResponseText);
            }
            catch (ApiException exc) {
                Assert.Fail(exc.Message + exc.StackTrace);
            }
        }

        [TestMethod]
        public void CreditRefund_Linked() {
            ITerminalResponse response = _device.Sale(10m)
                .WithGratuity(0m)
                .WithEcrId(13)
                .Execute();
            
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.DeviceResponseCode);
            Assert.AreEqual("COMPLETE", response.DeviceResponseText);

            Thread.Sleep(10000);
            
            ITerminalResponse refundResponse = _device.Refund(10m)
                .WithTransactionId(response.TransactionId)
                .WithAuthCode(response.ApprovalCode)
                .Execute();
            Assert.IsNotNull(refundResponse);
            Assert.AreEqual("00", refundResponse.DeviceResponseCode);
            Assert.AreEqual("COMPLETE", refundResponse.DeviceResponseText);
        }

        [TestMethod]
        public void CreditVerify() {
            try {
                ITerminalResponse response = _device.Verify()
                    .Execute();
                Assert.IsNotNull(response);
                Assert.AreEqual("00", response.DeviceResponseCode);
                Assert.AreEqual("COMPLETE", response.DeviceResponseText);
            }
            catch (ApiException exc) {
                Assert.Fail(exc.Message + exc.StackTrace);
            }
        }

        [TestMethod]
        public void CreditVoidWithTranNo() {
            ITerminalResponse response = _device.Sale(10m)
                .WithGratuity(0m)
                .WithEcrId(13)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.DeviceResponseCode);
            Assert.AreEqual("COMPLETE", response.DeviceResponseText);

            Thread.Sleep(5000);

            ITerminalResponse voidResponse = _device.Void()
                .WithTransactionId(response.TransactionId)
                .WithEcrId("13")
                .Execute();

            Assert.IsNotNull(voidResponse);
            Assert.AreEqual("00", voidResponse.DeviceResponseCode);
            Assert.AreEqual("COMPLETE", voidResponse.DeviceResponseText);
        }
        
        [TestMethod]
        public void CreditVoidWithReferenceNumber() {
            ITerminalResponse response = _device.Sale(10m)
                .WithGratuity(0m)
                .WithEcrId(13)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.DeviceResponseCode);
            Assert.AreEqual("COMPLETE", response.DeviceResponseText);

            Thread.Sleep(5000);

            ITerminalResponse voidResponse = _device.Void()
                .WithTerminalRefNumber(response.TerminalRefNumber)
                .WithEcrId("13")
                .Execute();

            Assert.IsNotNull(voidResponse);
            Assert.AreEqual("00", voidResponse.DeviceResponseCode);
            Assert.AreEqual("COMPLETE", voidResponse.DeviceResponseText);
        }
        
        [TestMethod]
        public void SendSaf() {
            try {
                _device.EcrId = "1";
                _device.SendStoreAndForward();
            }
            catch (ApiException exc) {
                Assert.Fail(exc.Message + exc.StackTrace);
            }
        }

        [TestMethod]
        public void GetSafReport_Declined()
        {
            try
            {
                _device.EcrId = "1";
                ITerminalReport response = _device.GetSAFReport()
                    .Execute();
                Assert.IsNotNull(response);
                Assert.AreEqual("ERROR", response.DeviceResponseCode);
                Assert.AreEqual("DECLINED", response.DeviceResponseText);
            }
            catch (ApiException exc)
            {
                Assert.Fail(exc.Message + exc.StackTrace);
            }
        }

        [TestMethod]
        public void GetSafReport() {
            try {
                _device.EcrId = "1";
                ITerminalReport response = _device.GetSAFReport()
                    .Execute();
                Assert.IsNotNull(response);
                Assert.AreEqual("SUCCESS", response.DeviceResponseCode);
                Assert.AreEqual("COMPLETE", response.DeviceResponseText);
            }
            catch (ApiException exc) {
                Assert.Fail(exc.Message + exc.StackTrace);
            }
        }
        
        [TestMethod]
        public void GetBatchReport() {
            try {
                _device.EcrId = "1";
                ITerminalReport response = _device.GetBatchReport()
                    .Execute();
                Assert.IsNotNull(response);
                Assert.AreEqual("SUCCESS", response.DeviceResponseCode);
                Assert.AreEqual("COMPLETE", response.DeviceResponseText);
            }
            catch (ApiException exc) {
                Assert.Fail(exc.Message + exc.StackTrace);
            }
        }

        [TestMethod]
        public void CreditSale_WithoutAmount() {
            bool exceptionCaught = false;
            try {
                _device.Sale()
                    .Execute();
            }
            catch (BuilderException exc) {
                exceptionCaught = true;
                Assert.AreEqual("Amount cannot be null for this transaction type.", exc.Message);
            }
            finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void CreditAuth_WithoutAmount() {
            bool exceptionCaught = false;
            try {
                _device.Authorize()
                    .Execute();
            }
            catch (BuilderException exc) {
                exceptionCaught = true;
                Assert.AreEqual("Amount cannot be null for this transaction type.", exc.Message);
            }
            finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void CreditCapture_WithoutTransactionId() {
            bool exceptionCaught = false;
            try {
                _device.Capture()
                    .Execute();
            }
            catch (BuilderException exc) {
                exceptionCaught = true;
                Assert.AreEqual("TransactionId cannot be null for this transaction type.", exc.Message);
            }
            finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void CreditRefund_WithoutAmount() {
            bool exceptionCaught = false;
            try {
                _device.Refund()
                    .Execute();
            }
            catch (BuilderException exc) {
                exceptionCaught = true;
                Assert.AreEqual("Amount cannot be null for this transaction type.", exc.Message);
            }
            finally {
                Assert.IsTrue(exceptionCaught);
            }
        }
        
        [TestMethod]
        public void CancelSale() {
            _device.EcrId = "1";
            try {
                _device.Cancel();
            }
            catch (ApiException exc) {
                Assert.Fail(exc.Message + exc.StackTrace);
            }
        }

        [TestMethod]
        public void Reboot() {
            try {
                _device.EcrId = "1";
                IDeviceResponse response = _device.Reboot();
                Assert.IsNotNull(response);
                Assert.AreEqual("00", response.DeviceResponseCode);
                Assert.AreEqual("COMPLETE", response.DeviceResponseText);
            }
            catch (ApiException exc) {
                Assert.Fail(exc.Message + exc.StackTrace);
            }
        }
        
               
        [TestMethod]
        public void GetSignatureFile() {
            try {
                _device.EcrId = "1";
                ISignatureResponse response = _device.PromptAndGetSignatureFile("Please sign", "and confirm", 1);
                Assert.IsNotNull(response);
                Assert.AreEqual("COMPLETE", response.DeviceResponseText);
                Assert.AreEqual("SUCCESS", response.DeviceResponseCode);
                
                // save as bmp
                SaveSignatureFile(response.SignatureData, @"C:\temp\signature" + new Random().Next(1, 100) + ".bmp");

                // convert and save as png
                var pngData = BmpToPng(response.SignatureData);
                SaveSignatureFile(pngData, @"C:\temp\signature1.png");
            }
            catch (ApiException exc) {
                Assert.Fail(exc.Message + exc.StackTrace);
            }
        }
        
        [TestMethod]
        public void EndOfDay() {
            try {
                _device.EcrId = "13";
                var response = _device.EndOfDay();
                
                Assert.IsNotNull(response);
                Assert.AreEqual("COMPLETE", response.DeviceResponseText);
                Assert.AreEqual("SUCCESS", response.DeviceResponseCode);
                Assert.IsNotNull(response.BatchId);
            }
            catch (ApiException exc) {
                Assert.Fail(exc.Message + exc.StackTrace);
            }
        }
        
        #region helper methods
        private void SaveSignatureFile(byte[] signatureData, string filename) {
            using (var sw = new BinaryWriter(File.OpenWrite(filename)))
            {
                sw.Write(signatureData);
                sw.Flush();
                sw.Close();
            }
        }

        private byte[] BmpToPng(byte[] signatureData) {
            using (var ms = new MemoryStream(signatureData)) {
                using (var pngStream = new MemoryStream()) {
                    //var bmp = new Bitmap(ms);
                    //bmp.Save(pngStream, ImageFormat.Png);
                    return pngStream.ToArray();
                }
            }
        }
        #endregion
    }
}