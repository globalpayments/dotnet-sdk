using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Mapping;
using GlobalPayments.Api.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace GlobalPayments.Api.Tests.GpEcom {
    [TestClass]
    public class GpEcomOpenBankingMappingTest {
        [TestMethod]
        public void MapTransactionSummary_ScalarResponse_MapsValues() {
            string rawJson = "{" +
                "\"ob_trans_id\": \"trans-123\"," +
                "\"order_id\": \"order-456\"," +
                "\"amount\": \"1050\"," +
                "\"currency\": \"GBP\"," +
                "\"status\": \"PAID\"," +
                "\"payment_type\": \"FASTERPAYMENTS\"," +
                "\"created_on\": \"2026-06-11T11:29:35Z\"" +
                "}";

            TransactionSummary summary = OpenBankingMapping.MapTransactionSummary(JsonDoc.Parse(rawJson));

            Assert.AreEqual("trans-123", summary.TransactionId);
            Assert.AreEqual("order-456", summary.OrderId);
            Assert.AreEqual(10.5m, summary.Amount);
            Assert.AreEqual("GBP", summary.Currency);
            Assert.AreEqual("PAID", summary.TransactionStatus);
            Assert.IsTrue(summary.TransactionDate.HasValue);
            Assert.AreEqual(new DateTime(2026, 6, 11, 11, 29, 35, DateTimeKind.Utc), summary.TransactionDate.Value.ToUniversalTime());
            Assert.AreEqual("trans-123", summary.BankPaymentResponse.Id);
            Assert.AreEqual("PAID", summary.BankPaymentResponse.PaymentStatus);
        }
        
        [TestMethod]
        public void MapTransactionSummary_CreatedOnAsArray_DoesNotThrowAndDefaultsDate() {
            // AH-2833: created_on is the exact field named in the merchant report. Isolate it
            // to prove the surrounding scalar fields still map while only created_on degrades.
            string rawJson = "{" +
                "\"ob_trans_id\": \"trans-123\"," +
                "\"status\": \"PAID\"," +
                "\"created_on\": [\"2026-06-11T11:29:35Z\"]" +
                "}";

            TransactionSummary summary = OpenBankingMapping.MapTransactionSummary(JsonDoc.Parse(rawJson));

            Assert.AreEqual("trans-123", summary.TransactionId);
            Assert.AreEqual("PAID", summary.TransactionStatus);
            Assert.AreEqual(default(DateTime), summary.TransactionDate);
        }

        [TestMethod]
        public void MapTransactionSummary_ArrayResponse_DegradesWithoutThrowing() {
            // Regression for AH-2833: the gateway occasionally returns scalar fields as
            // JSON arrays. MapTransactionSummary must not throw InvalidCastException or
            // NullReferenceException; unmappable fields degrade to their default value.
            string rawJson = "{" +
                "\"ob_trans_id\": [\"trans-123\"]," +
                "\"order_id\": [\"order-456\"]," +
                "\"amount\": [\"1050\"]," +
                "\"currency\": [\"GBP\"]," +
                "\"status\": [\"PAID\"]," +
                "\"payment_type\": [\"FASTERPAYMENTS\"]," +
                "\"created_on\": [\"2026-06-11T11:29:35Z\"]" +
                "}";

            TransactionSummary summary = OpenBankingMapping.MapTransactionSummary(JsonDoc.Parse(rawJson));

            Assert.IsNotNull(summary);
            Assert.IsNull(summary.TransactionId);
            Assert.IsNull(summary.OrderId);
            Assert.IsNull(summary.Amount);
            Assert.IsNull(summary.Currency);
            Assert.IsNull(summary.TransactionStatus);
            Assert.AreEqual(default(DateTime), summary.TransactionDate);
            Assert.IsNotNull(summary.BankPaymentResponse);
            Assert.IsNull(summary.BankPaymentResponse.Type);
        }
    }
}
