using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Tests.TestData;
using GlobalPayments.Api.Utils.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.Portico {
    /// <summary>
    /// Tests for CreditIncrementalAuth support of AdditionalTxnFields (InvoiceNbr, Description, CustomerID).
    /// </summary>
    [TestClass]
    public class PorticoCreditIncrementalAuthTests {
        private CreditCardData card;
        private CreditTrackData track;

        /// <summary>
        /// Configures the Portico gateway for CreditIncrementalAuth tests.
        /// </summary>
        public PorticoCreditIncrementalAuthTests() {
            ServicesContainer.ConfigureService(new PorticoConfig {
                SecretApiKey = "skapi_cert_MTeSAQAfG1UA9qQDrzl-kz4toXvARyieptFwSKP24w",
                RequestLogger = new RequestConsoleLogger()
            });
        }

        #region Positive Tests

        /// <summary>
        /// Verifies that all AdditionalTxnFields (InvoiceNbr, Description, CustomerID) are sent for CreditIncrementalAuth on an auth transaction.
        /// </summary>
        [TestMethod]
        public void IncrementalAuth_Auth_WithAllAdditionalTxnFields() {
            track = TestCards.VisaSwipe();

            Transaction response = track.Authorize(11m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            Transaction incremental = response.Increment(23m)
                    .WithCurrency("USD")
                    .WithInvoiceNumber("INV-12345")
                    .WithDescription("Incremental auth with invoice")
                    .WithCustomerId("CUST-001")
                    .Execute();
            Assert.IsNotNull(incremental);
            Assert.AreEqual("00", incremental.ResponseCode);

            Transaction capture = response.Capture().Execute();
            Assert.IsNotNull(capture);
            Assert.AreEqual("00", capture.ResponseCode);
        }

        /// <summary>
        /// Verifies that InvoiceNbr is sent for CreditIncrementalAuth on a MasterCard auth transaction.
        /// </summary>
        [TestMethod]
        public void IncrementalAuth_Auth_WithInvoiceNumber_MasterCard() {
            track = TestCards.MasterCardSwipe();

            Transaction response = track.Authorize(12m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            Transaction incremental = response.Increment(24m)
                    .WithCurrency("USD")
                    .WithInvoiceNumber("INV-MC-001")
                    .Execute();
            Assert.IsNotNull(incremental);
            Assert.AreEqual("00", incremental.ResponseCode);

            Transaction capture = response.Capture().Execute();
            Assert.IsNotNull(capture);
            Assert.AreEqual("00", capture.ResponseCode);
        }

        /// <summary>
        /// Verifies that InvoiceNbr is sent for CreditIncrementalAuth using a manual card entry auth.
        /// </summary>
        [TestMethod]
        public void IncrementalAuth_Manual_WithInvoiceNumber() {
            card = TestCards.VisaManual(true, true);

            Transaction response = card.Authorize(115m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            Transaction incremental = response.Increment(23m)
                    .WithCurrency("USD")
                    .WithInvoiceNumber("INV-MANUAL-001")
                    .Execute();
            Assert.IsNotNull(incremental);
            Assert.AreEqual("00", incremental.ResponseCode);

            Transaction capture = response.Capture().Execute();
            Assert.IsNotNull(capture);
            Assert.AreEqual("00", capture.ResponseCode);
        }

        #endregion

        #region Regression Tests

        /// <summary>
        /// Regression: Verifies that CreditIncrementalAuth still works without any AdditionalTxnFields.
        /// </summary>
        [TestMethod]
        public void IncrementalAuth_WithoutAdditionalTxnFields() {
            track = TestCards.VisaSwipe();

            Transaction response = track.Authorize(115m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            Transaction incremental = response.Increment(23m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(incremental);
            Assert.AreEqual("00", incremental.ResponseCode);

            Transaction capture = response.Capture().Execute();
            Assert.IsNotNull(capture);
            Assert.AreEqual("00", capture.ResponseCode);
        }

        #endregion

        #region Negative Tests

        /// <summary>
        /// Verifies that CreditIncrementalAuth with an invalid transaction ID throws a GatewayException.
        /// </summary>
        [TestMethod]
        public void IncrementalAuth_InvalidTransactionId() {
            var transaction = Transaction.FromId("999999999");

            var ex = Assert.ThrowsException<GatewayException>(() => {
                transaction.Increment(23m)
                        .WithCurrency("USD")
                        .WithInvoiceNumber("INV-INVALID")
                        .Execute();
            });

            Assert.IsNotNull(ex.ResponseMessage);
        }

        /// <summary>
        /// Verifies that CreditIncrementalAuth without an amount throws a GatewayException.
        /// </summary>
        [TestMethod]
        public void IncrementalAuth_NoAmount() {
            track = TestCards.VisaSwipe();

            Transaction response = track.Authorize(115m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            // Portico returns HTTP 500 SOAP fault when Amt is missing from CreditIncrementalAuth
            var ex = Assert.ThrowsException<GatewayException>(() => {
                response.Increment()
                        .WithCurrency("USD")
                        .WithInvoiceNumber("INV-NO-AMT")
                        .Execute();
            });

            Assert.IsTrue(ex.Message.Contains("Unexpected http status code [InternalServerError]"));
        }

        /// <summary>
        /// Verifies that CreditIncrementalAuth with an empty invoice number succeeds without emitting AdditionalTxnFields.
        /// </summary>
        [TestMethod]
        public void IncrementalAuth_WithEmptyInvoiceNumber() {
            track = TestCards.VisaSwipe();

            Transaction response = track.Authorize(10m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            Transaction incremental = response.Increment(23m)
                    .WithCurrency("USD")
                    .WithInvoiceNumber("")
                    .Execute();
            Assert.IsNotNull(incremental);
            Assert.AreEqual("00", incremental.ResponseCode);

            Transaction capture = response.Capture().Execute();
            Assert.IsNotNull(capture);
            Assert.AreEqual("00", capture.ResponseCode);
        }

        #endregion
    }
}
