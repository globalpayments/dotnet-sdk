using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Terminals;
using GlobalPayments.Api.Terminals.Enums;
using GlobalPayments.Api.Terminals.UPA;
using GlobalPayments.Api.Terminals.UPA.Responses;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using GlobalPayments.Api.Utils.Logging;
using System.Diagnostics;
using System;
using System.Linq.Expressions;

namespace GlobalPayments.Api.Tests.Terminals.UPA
{
    [TestClass]
    public class UpaReportTests
    {
        static IDeviceInterface _device;

        [ClassInitialize]
        public static void Initialize(TestContext context) {
            _device = DeviceService.Create(new ConnectionConfig {
                DeviceType = DeviceType.UPA_DEVICE,
                ConnectionMode = ConnectionModes.TCP_IP,
                IpAddress = "192.168.1.158",
                Port = "8081",
                Timeout = 30000,
                RequestIdProvider = new RandomIdProvider(),
                LogManagementProvider = new RequestConsoleLogger()
            });
            Assert.IsNotNull(_device);
        }

        [TestMethod]
        // Store and Forward is not currently supported
        public void GetSafReport() {
            var report = _device.GetSAFReport().Where(UpaSearchCriteria.EcrId, 13).And(UpaSearchCriteria.ReportOutput, ReportOutput.ReturnData)
                .Execute() as SafReportResponse;
            Assert.IsNotNull(report);
            Assert.AreEqual("Success", report.Status);
        }

        [TestMethod]
        public void GetBatchReport() {
            // The BatchID needs to be supplied from a valid Batch in order for this test to pass
            var batchId = 1006209;
            var report = _device.GetBatchReport()
                    .Where(UpaSearchCriteria.Batch, batchId).And(UpaSearchCriteria.EcrId, 13)
                    .Execute() as BatchReportResponse;
            Assert.IsNotNull(report);
            Assert.AreEqual("Success", report.Status);
        }

        [TestMethod]
        public void GetBatchDetails() {
            _device.EcrId = "1";
            var response = _device.GetBatchDetails("1006209", true) as BatchReportResponse;

            Assert.IsNotNull(response);
            Assert.AreEqual("Success", response.Status);
            Assert.IsNotNull(response.BatchRecord);
            Assert.IsNotNull(response.BatchRecord.TransactionDetails);
            Assert.AreEqual("1006209", response.BatchRecord.BatchId.ToString());
        }

        [TestMethod]
        public void GetOpenTabDetailsReport() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
                Debug.WriteLine($"{DateTime.Now.ToString("HH:mm:ss:fff")} {message}");
            };
            _device.OnMessageReceived += (message) => {
                Assert.IsNotNull(message);
                Debug.WriteLine($"{DateTime.Now.ToString("HH:mm:ss:fff")} {message}");
            };
            var report = _device.GetOpenTabDetails()
                .Where(UpaSearchCriteria.EcrId, 13)
                .Execute();

            Assert.IsNotNull(report);
            Assert.AreEqual("Success", report.Status);
        }

        [TestMethod]
        public void CloseAllOpenTabs() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
                Debug.WriteLine($"{DateTime.Now.ToString("HH:mm:ss:fff")} {message}");
            };
            _device.OnMessageReceived += (message) => {
                Assert.IsNotNull(message);
                Debug.WriteLine($"{DateTime.Now.ToString("HH:mm:ss:fff")} {message}");
            };
            var report = _device.GetOpenTabDetails()
                .Where(UpaSearchCriteria.EcrId, 13)
                .Execute() as OpenTabDetailsResponse;

            var tabsToClose = report.OpenTabs;

            Thread.Sleep(1000);
            try {
                foreach (var openTab in tabsToClose) {
                    var captureResponse = _device.Capture(openTab.AuthorizedAmount)
                        .WithEcrId("13")
                        .WithTransactionId(openTab.TransactionId)
                        .WithGratuity(0.00m)
                        .Execute();
                    Assert.AreEqual("00", captureResponse.ResponseCode);

                    Thread.Sleep(1000);
                }
            } catch (Exception ex) {
                Debug.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void FindAvailableBatches()
        {
            _device.EcrId = "12";
            var response = _device.FindBatches()
                .Where(UpaSearchCriteria.EcrId, 13)
                .Execute();
            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(BatchList));            
            Assert.IsTrue(((BatchList)response).Batches.Count > 0);                       
            Assert.AreEqual("Success", response.Status);
        }
    }
}
