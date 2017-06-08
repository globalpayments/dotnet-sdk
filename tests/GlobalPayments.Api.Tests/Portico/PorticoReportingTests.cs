using System;
using System.Collections.Generic;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.Portico {
    [TestClass]
    public class PorticoReportingTests {
        public PorticoReportingTests() {
            ServicesContainer.Configure(new ServicesConfig {
                SecretApiKey = "skapi_cert_MTeSAQAfG1UA9qQDrzl-kz4toXvARyieptFwSKP24w",
                ServiceUrl = "https://cert.api2.heartlandportico.com"
            });
        }

        [TestMethod]
        public void ReportActivity() {
            var summary = ReportingService.Activity()
                .WithStartDate(DateTime.UtcNow.AddDays(-7))
                .WithEndDate(DateTime.UtcNow.AddDays(-1))
                .Execute();
            Assert.IsNotNull(summary);
            Assert.IsTrue(summary.Count > 0);
        }

        [TestMethod]
        public void ReportTransactionDetail() {
            List<TransactionSummary> summary = ReportingService.Activity()
                .WithStartDate(DateTime.UtcNow.AddDays(-7))
                .WithEndDate(DateTime.UtcNow.AddDays(-1))
                .Execute();

            if (summary.Count > 0) {
                TransactionSummary response = ReportingService.TransactionDetail(summary[0].TransactionId).Execute();
                Assert.IsNotNull(response);
                Assert.AreEqual("00", response.GatewayResponseCode);
            }
        }
    }
}
