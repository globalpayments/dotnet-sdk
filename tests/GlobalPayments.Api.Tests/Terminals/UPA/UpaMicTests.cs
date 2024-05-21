using System;
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
                    AppId = AppId,
                    AppKey = AppKey,
                    Channel = Channel.CardPresent,
                    Country = "US",
                    DeviceCurrency = "USD",
                    EnableLogging = true,
                    RequestLogger = new RequestConsoleLogger(),
                    MethodNotificationUrl = "https://ensi808o85za.x.pipedream.net/",
                    AccessTokenInfo = new AccessTokenInfo {
                        TransactionProcessingAccountName = "transaction_processing"
                    }
                },
                Timeout = 30000,
                RequestIdProvider = new RandomIdProvider()
            });
        }

        [TestMethod]
        public void CreditSale() {
            try {
                ITerminalResponse response = _device.Sale(10m)
                    .WithEcrId(13)
                    .Execute();
                Assert.IsNotNull(response);
                Assert.AreEqual("SUCCESS", response.ResponseText);
                Assert.AreEqual("INITIATED", response.DeviceResponseText);
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
                Assert.AreEqual("SUCCESS", response.ResponseText);
                Assert.AreEqual("INITIATED", response.DeviceResponseText);
            }
            catch (ApiException exc) {
                Assert.Fail(exc.Message + exc.StackTrace);
            }
        }

        [TestMethod]
        public void AddLineItem() {
            try {
                IDeviceResponse response = _device.LineItem("Line Item #1", "10.00");

                Assert.IsNotNull(response);
                Assert.AreEqual("SUCCESS", response.DeviceResponseCode);
                Assert.AreEqual("INITIATED", response.DeviceResponseText);
            }
            catch (ApiException exc) {
                Assert.Fail(exc.Message + exc.StackTrace);
            }
        }

        [TestMethod]
        public void CreditAuth() {
            try {
                ITerminalResponse response = _device.Authorize(10m)
                    .Execute();
                Assert.IsNotNull(response);
                Assert.AreEqual("SUCCESS", response.ResponseText);
                Assert.AreEqual("INITIATED", response.DeviceResponseText);
            }
            catch (ApiException exc) {
                Assert.Fail(exc.Message + exc.StackTrace);
            }
        }

        [TestMethod]
        public void CreditAuthAndCapture() {
            try {
                ITerminalResponse response = _device.Authorize(10m)
                    .Execute();
                Assert.IsNotNull(response);
                Assert.AreEqual("SUCCESS", response.ResponseText);
                Assert.AreEqual("INITIATED", response.DeviceResponseText);

                ITerminalResponse capture = _device.Capture(10m)
                    .WithTransactionId(response.TransactionId)
                    .Execute();
                Assert.IsNotNull(capture);
                Assert.AreEqual("SUCCESS", capture.ResponseText);
                Assert.AreEqual("INITIATED", capture.DeviceResponseText);
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
                Assert.AreEqual("SUCCESS", response.ResponseText);
                Assert.AreEqual("INITIATED", response.DeviceResponseText);

                ITerminalResponse capture = _device.Capture(10m)
                    .WithClientTransactionId("asdcdf999")
                    .WithTransactionId(response.TransactionId)
                    .Execute();
                Assert.IsNotNull(capture);
                Assert.AreEqual("SUCCESS", capture.ResponseText);
                Assert.AreEqual("INITIATED", capture.DeviceResponseText);
            }
            catch (ApiException exc) {
                Assert.Fail(exc.Message + exc.StackTrace);
            }
        }
        
        [TestMethod]
        public void CreditAuthAndThenDelete() {
            try {
                ITerminalResponse response = _device.Authorize(10m)
                    .Execute();
                Assert.IsNotNull(response);
                Assert.AreEqual("SUCCESS", response.ResponseText);
                Assert.AreEqual("INITIATED", response.DeviceResponseText);

                ITerminalResponse capture = _device.DeletePreAuth()
                    .WithTransactionId(response.TransactionId)
                    .Execute();
                Assert.IsNotNull(capture);
                Assert.AreEqual("SUCCESS", capture.ResponseText);
                Assert.AreEqual("INITIATED", capture.DeviceResponseText);
            }
            catch (ApiException exc) {
                Assert.Fail(exc.Message + exc.StackTrace);
            }
        }
        
        [TestMethod]
        public void GetOpenTabDetails() {
            try {
                ITerminalReport response = _device.GetOpenTabDetails()
                    .Execute();
                Assert.IsNotNull(response);
                Assert.AreEqual("SUCCESS", response.DeviceResponseCode);
                Assert.AreEqual("INITIATED", response.DeviceResponseText);
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
                    .Execute();

                Assert.IsNotNull(response);
                Assert.AreEqual("SUCCESS", response.ResponseText);
                Assert.AreEqual("INITIATED", response.DeviceResponseText);
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
                Assert.AreEqual("SUCCESS", response.ResponseText);
                Assert.AreEqual("INITIATED", response.DeviceResponseText);
            }
            catch (ApiException exc) {
                Assert.Fail(exc.Message + exc.StackTrace);
            }
        }

        [TestMethod]
        public void CreditVerify() {
            try {
                ITerminalResponse response = _device.Verify()
                    .Execute();
                Assert.IsNotNull(response);
                Assert.AreEqual("SUCCESS", response.ResponseText);
                Assert.AreEqual("INITIATED", response.DeviceResponseText);
            }
            catch (ApiException exc) {
                Assert.Fail(exc.Message + exc.StackTrace);
            }
        }

        [TestMethod]
        public void CreditVoid() {
            try {
                ITerminalResponse response = _device.Void()
                    .WithTransactionId(Guid.NewGuid().ToString())
                    .Execute();
                Assert.IsNotNull(response);
                Assert.AreEqual("SUCCESS", response.ResponseText);
                Assert.AreEqual("INITIATED", response.DeviceResponseText);
            }
            catch (ApiException exc) {
                Assert.Fail(exc.Message + exc.StackTrace);
            }
        }               
        
        [TestMethod]
        public void SendSaf() {
            try {
                _device.SendStoreAndForward();
            }
            catch (ApiException exc) {
                Assert.Fail(exc.Message + exc.StackTrace);
            }
        }
        
        [TestMethod]
        public void GetSafReport() {
            try {
                ITerminalReport response = _device.GetSAFReport()
                    .Execute();
                Assert.IsNotNull(response);
                Assert.AreEqual("SUCCESS", response.DeviceResponseCode);
                Assert.AreEqual("INITIATED", response.DeviceResponseText);
            }
            catch (ApiException exc) {
                Assert.Fail(exc.Message + exc.StackTrace);
            }
        }
        
        [TestMethod]
        public void GetBatchReport() {
            try {
                ITerminalReport response = _device.GetBatchReport()
                    .Execute();
                Assert.IsNotNull(response);
                Assert.AreEqual("SUCCESS", response.DeviceResponseCode);
                Assert.AreEqual("INITIATED", response.DeviceResponseText);
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
                _device.Reboot();
            }
            catch (ApiException exc) {
                Assert.Fail(exc.Message + exc.StackTrace);
            }
        }
        
        [TestMethod]
        public void RegisterPos() {
            try {
                IDeviceResponse response = _device.RegisterPOS("app");
                Assert.IsNotNull(response);
                Assert.AreEqual("INITIATED", response.DeviceResponseText);
                Assert.AreEqual("SUCCESS", response.DeviceResponseCode);
            }
            catch (ApiException exc) {
                Assert.Fail(exc.Message + exc.StackTrace);
            }
        }
        
        [TestMethod]
        public void GetSignatureFile() {
            try {
                ISignatureResponse response = _device.PromptAndGetSignatureFile("asd", "asdas", 1);
                Assert.IsNotNull(response);
                Assert.AreEqual("INITIATED", response.DeviceResponseText);
                Assert.AreEqual("SUCCESS", response.DeviceResponseCode);
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
                Assert.AreEqual("INITIATED", response.DeviceResponseText);
                Assert.AreEqual("SUCCESS", response.DeviceResponseCode);
            }
            catch (ApiException exc) {
                Assert.Fail(exc.Message + exc.StackTrace);
            }
        }
    }
}