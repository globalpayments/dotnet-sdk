using System;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Mapping;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.GpEcom
{
    [TestClass]
    public class GpEcomOpenBankingMappingTest
    {
        // Reproduces the 500 seen on order NSP6640-112730-ANNOB-20260611122832.
        // The gateway returned created_on as a JSON array. GetValue<DateTime> fails the
        // Convert.ChangeType, and the catch re-casts the List<string> and throws again.
        private const string PaymentWithArrayCreatedOn = @"{
            ""payments"": [
                {
                    ""ob_trans_id"": ""YAIIfkmSPGfIzLHkum"",
                    ""order_id"": ""NSP6640-112730-ANNOB-20260611122832"",
                    ""amount"": ""780"",
                    ""currency"": ""GBP"",
                    ""status"": ""PAID"",
                    ""created_on"": [""2026-06-11T11:29:35.876""]
                }
            ]
        }";

        [TestMethod]
        public void MapTransactionSummary_CreatedOnAsArray_DoesNotThrow() {
            // Before the fix this threw InvalidCastException (List<string> -> DateTime) and surfaced as a 500.
            var result = OpenBankingMapping.MapReportResponse<PagedResult<TransactionSummary>>(
                PaymentWithArrayCreatedOn, ReportType.FindBankPayment);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Results.Count);

            var summary = result.Results[0];
            // The rest of the record still maps. The scalar string fields are exact.
            Assert.AreEqual("YAIIfkmSPGfIzLHkum", summary.TransactionId);
            Assert.AreEqual("NSP6640-112730-ANNOB-20260611122832", summary.OrderId);
            Assert.AreEqual("GBP", summary.Currency);
            Assert.AreEqual("PAID", summary.TransactionStatus);
            // The arrayed date is recovered on a best-effort basis (first element), so it is populated
            // rather than left at default. Exact precision is not guaranteed for a malformed array field.
            Assert.AreNotEqual(default(DateTime), summary.TransactionDate);
        }

        [TestMethod]
        public void MapTransactionSummary_CreatedOnAsString_StillMaps() {
            var raw = PaymentWithArrayCreatedOn.Replace(
                @"""created_on"": [""2026-06-11T11:29:35.876""]",
                @"""created_on"": ""2026-06-11T11:29:35.876""");

            var result = OpenBankingMapping.MapReportResponse<PagedResult<TransactionSummary>>(
                raw, ReportType.FindBankPayment);

            var expected = (DateTime)Convert.ChangeType("2026-06-11T11:29:35.876", typeof(DateTime));
            Assert.AreEqual(expected, result.Results[0].TransactionDate);
        }
    }
}
