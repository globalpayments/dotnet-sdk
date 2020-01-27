using System;
using System.Collections.Generic;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.Portico {
    [TestClass]
    public class PorticoReportingTests {
        public PorticoReportingTests() {
            ServicesContainer.ConfigureService(new PorticoConfig {
                SecretApiKey = "skapi_cert_MTeSAQAfG1UA9qQDrzl-kz4toXvARyieptFwSKP24w"
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
            TransactionSummary response = ReportingService.TransactionDetail("1038021900").Execute();
            Assert.IsNotNull(response);
            Assert.IsTrue(response is TransactionSummary);
        }

        [TestMethod]
        public void ReportFindTransactionWithCriteria() {
            List<TransactionSummary> summary = ReportingService.FindTransactions()
                .WithTimeZoneConversion(TimeZoneConversion.Merchant)
                .Where(SearchCriteria.StartDate, DateTime.UtcNow.AddDays(-30))
                .And(SearchCriteria.EndDate, DateTime.UtcNow)
                .Execute();

            Assert.IsNotNull(summary);
        }

        [TestMethod]
        public void ReportFindTransactionWithTransactionId() {
            List<TransactionSummary> summary = ReportingService.FindTransactions("1038021900").Execute();
            Assert.IsNotNull(summary);
        }

        [TestMethod]
        public void ReportFindTransactionNoCriteria() {
            List<TransactionSummary> summary = ReportingService.FindTransactions().Execute();
            Assert.IsNotNull(summary);
        }
        [TestMethod]
        public void ReportActivityWithNewCryptoURL() {
            ServicesContainer.ConfigureService(new PorticoConfig {
                SecretApiKey = "skapi_cert_MTyMAQBiHVEAewvIzXVFcmUd2UcyBge_eCpaASUp0A"
            });

            var summary = ReportingService.Activity()
                .WithStartDate(DateTime.UtcNow.AddDays(-7))
                .WithEndDate(DateTime.UtcNow.AddDays(-1))
                .Execute();
            Assert.IsNotNull(summary);
        }
    }
}
