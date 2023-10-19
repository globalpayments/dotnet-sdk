using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Terminals;
using GlobalPayments.Api.Terminals.UPA;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

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
                IpAddress = "192.168.1.130",
                Port = "8081",
                Timeout = 30000,
                RequestIdProvider = new RandomIdProvider()
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
            var batchId = 1234;
            var report = _device.GetBatchReport()
                    .Where(UpaSearchCriteria.Batch, batchId).And(UpaSearchCriteria.EcrId, 13)
                    .Execute() as BatchReportResponse;
            Assert.IsNotNull(report);
            Assert.AreEqual("Success", report.Status);
        }

        [TestMethod]
        public void GetOpenTabDetailsReport() {
            var report = _device.GetOpenTabDetails()
                .Where(UpaSearchCriteria.EcrId, 13)
                .Execute();

            Assert.IsNotNull(report);
            Assert.AreEqual("Success", report.Status);
        }

        [TestMethod]
        public void CloseAllOpenTabs() {
            var report = _device.GetOpenTabDetails()
                .Where(UpaSearchCriteria.EcrId, 13)
                .Execute() as OpenTabDetailsResponse;

            var tabsToClose = report.OpenTabs;

            Thread.Sleep(1000);

            foreach(var openTab in tabsToClose) {
                var captureResponse = _device.Capture(openTab.AuthorizedAmount)
                    .WithTransactionId(openTab.TransactionId)
                    .WithGratuity(0.00m)
                    .Execute();
                Assert.AreEqual("00", captureResponse.ResponseCode);

                Thread.Sleep(1000);
            }            
        }
    }
}
