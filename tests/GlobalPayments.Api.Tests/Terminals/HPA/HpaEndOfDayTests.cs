using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Terminals;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace GlobalPayments.Api.Tests.Terminals.HPA {
    [TestClass]
    public class HpaEndOfDayTests {
        IDeviceInterface _device;

        public HpaEndOfDayTests() {
            _device = DeviceService.Create(new ConnectionConfig {
                DeviceType = DeviceType.HPA_ISC250,
                ConnectionMode = ConnectionModes.TCP_IP,
                IpAddress = "10.12.220.39",
                Port = "12345",
                Timeout = 20000,
                RequestIdProvider = new RandomIdProvider()
            });
            Assert.IsNotNull(_device);
        }

        [TestMethod]
        public void Test_000_Reset() {
            var response = _device.Reset();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.DeviceResponseCode);
        }

        [TestMethod]
        public void Test_001_OpenLane() {
            var response = _device.OpenLane();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.DeviceResponseCode);
        }

        [TestMethod]
        public void Test_002_CreditTransaction() {
            var response = _device.Sale(10m)
                .WithAllowDuplicates(true)
               .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Test_002b_Reset() {
            var response = _device.Reset();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.DeviceResponseCode);
        }

        [TestMethod]
        public void Test_003_CloseLane() {
            var response = _device.CloseLane();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.DeviceResponseCode);
        }

        [TestMethod]
        public void Test_004_SAF_Mode_On() {
            var response = _device.SetStoreAndForwardMode(true);
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.DeviceResponseCode);

            _device.Reset();
        }

        [TestMethod]
        public void Test_005_OpenLane() {
            var response = _device.OpenLane();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.DeviceResponseCode);
        }

        [TestMethod]
        public void Test_006_CreditTransaction() {
            var response = _device.Sale(10m)
               .WithAllowDuplicates(true)
               .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Test_006b_Reset() {
            var response = _device.Reset();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.DeviceResponseCode);
        }

        [TestMethod]
        public void Test_007_CloseLane() {
            var response = _device.CloseLane();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.DeviceResponseCode);
        }

        [TestMethod]
        public void Test_008_SAF_Mode_Off() {
            var response = _device.SetStoreAndForwardMode(false);
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.DeviceResponseCode);

            _device.Reset();
        }

        [TestMethod]
        public void Test_999_EndOfDay() {
            var response = _device.EndOfDay();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.DeviceResponseCode);
            Assert.IsNotNull(response.AttachmentResponseText);
            Assert.IsNotNull(response.BatchCloseResponseText);
            Assert.IsNotNull(response.EmvOfflineDeclineResponseText);
            Assert.IsNotNull(response.EmvPDLResponseText);
            Assert.IsNotNull(response.EmvTransactionCertificationResponseText);
            Assert.IsNotNull(response.HeartBeatResponseText);
            Assert.IsNotNull(response.ReversalResponseText);
            Assert.IsNotNull(response.SafResponseText);
            
            //REVERSALS
            var reversalResponse = response.ReversalResponse;
            if (reversalResponse != null) {
                Assert.AreEqual("00", reversalResponse.DeviceResponseCode);
            }

            //EMV OFFLINE DECLINE
            var emvOfflineDeclineResponse = response.EmvOfflineDeclineResponse;
            if (emvOfflineDeclineResponse != null) {
                Assert.AreEqual("00", emvOfflineDeclineResponse.DeviceResponseCode);
            }

            //TRANSACTION CERTIFICATE
            var emvTransactionCertificateResponse = response.EmvTransactionCertificationResponse;
            if (emvTransactionCertificateResponse != null) {
                Assert.AreEqual("00", emvTransactionCertificateResponse.DeviceResponseCode);
            }

            //ATTACHEMENT
            var attachmentResponse = response.AttachmentResponse;
            if (attachmentResponse != null) {
                Assert.AreEqual("00", attachmentResponse.DeviceResponseCode);
            }

            //BATCH CLOSE
            var batchCloseResponse = response.BatchCloseResponse;
            if (batchCloseResponse != null) {
                Assert.AreEqual("00", batchCloseResponse.DeviceResponseCode);
            }

            List<string> batchCloseResponseCodes = new List<string>();
            batchCloseResponseCodes.Add("00");
            batchCloseResponseCodes.Add("2501");

            //HEART BEAT
            var heartBeatResponse = response.HeartBeatResponse;
            if (heartBeatResponse != null) {
                Assert.AreEqual("00", heartBeatResponse.DeviceResponseCode);
            }

            //EMV PDL
            var emvPDLResponse = response.EmvPDLResponse;
            if (emvPDLResponse != null) {
                Assert.AreEqual("00", emvPDLResponse.DeviceResponseCode);
            }

            //SAF RESPONSE
            var safResponse = response.SAFResponse;
            if (safResponse != null) {
                Assert.AreEqual("00", safResponse.DeviceResponseCode);
            }

            //BATCH REPORT
            var batchReportResponse = response.BatchReportResponse;
            if (batchReportResponse != null) {
                // Check some values here
            }
        }

        [TestMethod]
        public void Test_999b_Reset() {
            var response = _device.Reset();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.DeviceResponseCode);
        }
    }
}
