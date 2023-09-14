using System;
using System.Collections.Generic;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GlobalPayments.Api.PaymentMethods;

namespace GlobalPayments.Api.Tests.Portico {
    [TestClass]
    public class PorticoReportingTests {
        public PorticoReportingTests() {
            ServicesContainer.ConfigureService(new PorticoConfig {
                SecretApiKey = "skapi_cert_MTeSAQAfG1UA9qQDrzl-kz4toXvARyieptFwSKP24w",
                IsSafDataSupported = true
            });
        }

        [TestMethod]
        public void ReportActivity()
        {
            List<TransactionSummary> summary = ReportingService.Activity()
                .WithTimeZoneConversion(TimeZoneConversion.Merchant)
                .Where(SearchCriteria.StartDate, DateTime.UtcNow.AddDays(-2))
                .And(SearchCriteria.EndDate, DateTime.UtcNow)
                .Execute();
            Assert.IsNotNull(summary);
            Assert.IsTrue(summary.Count > 0);
        }

        [TestMethod]
        public void ReportTransactionDetail() {
            TransactionSummary response = ReportingService.TransactionDetail("1873988120").Execute();
            Assert.IsNotNull(response);
            Assert.IsTrue(response is TransactionSummary);
        }

        [TestMethod]
        public void ReportFindTransactionWithCriteria()
        {
            List<TransactionSummary> summary = ReportingService.FindTransactions()
                .WithTimeZoneConversion(TimeZoneConversion.Merchant)
                .Where(SearchCriteria.StartDate, DateTime.UtcNow.AddDays(-30))
                .And(SearchCriteria.EndDate, DateTime.UtcNow)
                .And(SearchCriteria.SAFIndicator, "Y")
                .Execute();

            Assert.IsNotNull(summary);
        }

        [TestMethod]
        public void ReportFindTransactionWithTransactionId() {
            List<TransactionSummary> summary = ReportingService.FindTransactions("1873988120").Execute();
            Assert.IsNotNull(summary);
        }
        [TestMethod]
        public void FindTransByDate()
        {
            var items = ReportingService.FindTransactions()
                .WithTimeZoneConversion(TimeZoneConversion.Merchant)
                .WithStartDate(DateTime.Today.AddDays(-10))
                .WithEndDate(DateTime.Today)
                .Execute();
            Assert.IsNotNull(items);
            var item = ReportingService.TransactionDetail(items[0].TransactionId)
                .Execute();
            Assert.IsNotNull(item);
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
            List<TransactionSummary> summary = ReportingService.Activity()
                .WithTimeZoneConversion(TimeZoneConversion.Merchant)
                .Where(SearchCriteria.StartDate, DateTime.UtcNow.AddDays(-2))
                .And(SearchCriteria.EndDate, DateTime.UtcNow)
                .Execute();

            Assert.IsNotNull(summary);
            Assert.IsTrue(summary.Count > 0);
        }
        [TestMethod]
        public void EcomWithDirectMarketDataInvoiceAndShipDate()
        {
            EcommerceInfo ecom = new EcommerceInfo
            {
                ShipDay = DateTime.Now.AddDays(1).Day,
                ShipMonth = DateTime.Now.AddDays(1).Month
            };
            CreditCardData card = new CreditCardData
            {
                Number = "4111111111111111",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvn = "123",
                CardHolderName = " UnitTest "
            };

            Transaction response = card.Charge(11m)
                .WithCurrency("USD")
                .WithEcommerceInfo(ecom)
                .WithInvoiceNumber("1234567890")
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            var item = ReportingService.TransactionDetail(response.TransactionId)
                .Execute();
            Assert.IsNotNull(item);
            Assert.AreEqual(ecom.ShipDay.ToString(), item.ShippingDay.ToString());
            Assert.AreEqual(ecom.ShipMonth.ToString(), item.ShippingMonth.ToString());
        }
    }
}
