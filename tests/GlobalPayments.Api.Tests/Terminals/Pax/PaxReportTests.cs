using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Terminals;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Terminals.PAX;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace GlobalPayments.Api.Tests.Terminals.Pax {
    [TestClass]
    public class PaxReportTests {
        IDeviceInterface _device;

        public PaxReportTests() {
            _device = DeviceService.Create(new ConnectionConfig {
                DeviceType = DeviceType.PAX_S300,
                ConnectionMode = ConnectionModes.HTTP,
                IpAddress = "10.12.220.172",
                Port = "10009",
                Timeout = 30000,
                RequestIdProvider = new RequestIdProvider()
            });
            Assert.IsNotNull(_device);

            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
                Thread.Sleep(4000);
            };
        }

        [TestMethod]
        public void LocalDetailReport() {
            int recordNumber = 0;
            int totalRecords = 0;
            do {
                var report = _device.LocalDetailReport()
                    .Where(PaxSearchCriteria.RecordNumber, recordNumber++)
                    .Execute() as LocalDetailReport;
                Assert.IsNotNull(report);

                totalRecords = report.TotalRecords;
            }
            while (recordNumber > totalRecords);
        }
    }
}
