using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Terminals;
using GlobalPayments.Api.Terminals.UPA;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.Terminals.UPA
{
    [TestClass]
    public class UpaReportTests
    {
        static IDeviceInterface _device;

        [ClassInitialize]
        public static void Initialize(TestContext context) {
            _device = DeviceService.Create(new ConnectionConfig {
                DeviceType = DeviceType.NUCLEUS_SATURN_1000,
                ConnectionMode = ConnectionModes.TCP_IP,
                IpAddress = "192.168.1.10",
                Port = "8081",
                Timeout = 30000,
                RequestIdProvider = new RandomIdProvider()
            });
            Assert.IsNotNull(_device);
        }

        [TestMethod]
        public void GetSafReport() {
            var report = _device.GetSAFReport().Where(UpaSearchCriteria.EcrId, 13).And(UpaSearchCriteria.ReportOutput, ReportOutput.Print)
                .Execute() as SafReportResponse;
            Assert.IsNotNull(report);
            Assert.AreEqual("Success", report.Status);
        }

        [TestMethod]
        public void GetBatchReport() {
            var report = _device.GetBatchReport()
                    .Where(UpaSearchCriteria.Batch, 1234).And(UpaSearchCriteria.EcrId, 13)
                    .Execute() as BatchReportResponse;
            Assert.IsNotNull(report);
            Assert.AreEqual("Success", report.Status);
        }

    }
}
