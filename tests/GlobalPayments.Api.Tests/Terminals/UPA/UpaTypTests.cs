using System;
using System.Threading;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Terminals;
using GlobalPayments.Api.Terminals.Enums;
using GlobalPayments.Api.Terminals.UPA;
using GlobalPayments.Api.Terminals.UPA.Responses;
using GlobalPayments.Api.Utils.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.Terminals.UPA {
    /// <summary>
    /// Test class for Mexico GP-API CP: UPA TYP (Thank You Points) transactions
    /// Tests for Sale, Void, and Reverse transactions with TYP and/or discount
    /// Tests for Summary and Detail reports with TYP
    /// </summary>
    /// <remarks>
    /// Requires a UPA device configured for a Mexico merchant with a TYP (Thank You Points) loyalty account.
    /// All TYP assertion blocks assume TYP is active and will fail if the merchant is not TYP-configured.
    /// Update <c>IpAddress</c> in the constructor to match the test device IP before running.
    /// </remarks>
    [TestCategory("Hardware")]
    [TestClass]
    public class UpaTypTests {
        private const string DEVICE_IP = "192.168.1.21";
        private const string DEVICE_PORT = "8081";
        private const int DEVICE_TIMEOUT_MS = 90000;
        private const string TEST_ECR_ID = "13";
        private const int BATCH_ID = 1006209;
        // UPA terminal needs a brief settle window between a sale and a follow-up void/reverse against real hardware.
        private const int HARDWARE_SETTLE_MS = 5000;

        private static readonly Random _rng = new Random();
        private static IDeviceInterface _device;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context) {
            _device = DeviceService.Create(new ConnectionConfig {
                DeviceType = DeviceType.UPA_DEVICE,
                ConnectionMode = ConnectionModes.TCP_IP,
                IpAddress = DEVICE_IP,
                Port = DEVICE_PORT,
                Timeout = DEVICE_TIMEOUT_MS,
                RequestIdProvider = new RandomIdProvider(),
                LogManagementProvider = new RequestConsoleLogger()
            });
            Assert.IsNotNull(_device);
        }

        [ClassCleanup]
        public static void ClassCleanup() {
            _device?.Dispose();
        }

        #region Sale with TYP and/or Discount Tests

        /// <summary>
        /// Test: Sale with TYP and/or discount
        /// Given: UPA device for Mexico merchant
        /// When: Sale is submitted
        /// Then: UPA response contains TYP Sale Extra Fields
        /// And: All response properties are mapped by SDK object
        /// </summary>
        [TestMethod]
        public void Sale_WithTyp_ShouldReturnSuccess() {
            // Arrange
            decimal amount = 100.00m;

            // Act
            var response = _device.Sale(amount)
                .WithEcrId(TEST_ECR_ID)
                .WithClerkId(123)
                .Execute();

            // Assert
            Assert.IsNotNull(response, "Response should not be null");
            Assert.AreEqual("Success", response.Status, "Transaction should be successful");
            Assert.AreEqual("00", response.DeviceResponseCode, "Device response code should be 00");

            // TYP redemption fields — requires merchant configured with TYP loyalty
            var upaResponse = response as UPAResponseHandler;
            Assert.IsNotNull(upaResponse, "Response must be UPAResponseHandler");
            Assert.IsNotNull(upaResponse.RedeemId, "RedeemId must be present for a TYP sale");
            Assert.IsNotNull(upaResponse.RedeemStatus, "RedeemStatus must be present");
            Assert.AreEqual("COMPLETE", upaResponse.RedeemStatus.ToUpper(), "RedeemStatus must be COMPLETE");
            Assert.IsNotNull(upaResponse.CurrencyAmountRedeemed, "CurrencyAmountRedeemed must be present");
            Assert.IsNotNull(upaResponse.PointsRedeemed, "PointsRedeemed must be present");
            Assert.IsNotNull(upaResponse.DiscountAmountRedeemed, "DiscountAmountRedeemed must be present");
        }

        /// <summary>
        /// Test: Sale with TYP verifying all response fields are properly mapped
        /// </summary>
        [TestMethod]
        public void Sale_WithTypAndInvoiceNumber_ShouldMapAllResponseFields() {
            // Arrange
            decimal amount = 150.00m;

            // Act
            var response = _device.Sale(amount)
                .WithEcrId(TEST_ECR_ID)
                .WithClerkId(456)
                .WithInvoiceNumber(_rng.Next(1000000, 9999999).ToString())
                .Execute();

            // Assert
            Assert.IsNotNull(response, "Response should not be null");
            var upaResponse = response as UPAResponseHandler;
            Assert.IsNotNull(upaResponse, "Response should be UPAResponseHandler type");
            Assert.IsNotNull(upaResponse.RedeemId, "RedeemId must be mapped from device response");
            Assert.IsNotNull(upaResponse.RedeemStatus, "RedeemStatus must be mapped from device response");
            Assert.IsNotNull(upaResponse.CurrencyAmountRedeemed, "CurrencyAmountRedeemed must be mapped as decimal?");
            Assert.IsNotNull(upaResponse.PointsRedeemed, "PointsRedeemed must be mapped as decimal?");
            Assert.IsNotNull(upaResponse.DiscountAmountRedeemed, "DiscountAmountRedeemed must be mapped as decimal?");
        }

        #endregion

        #region Void Sale with TYP and/or Discount Tests

        /// <summary>
        /// Test: Void a sale with TYP and/or discount
        /// Given: UPA device for Mexico merchant
        /// When: Void is submitted for Sale with TYP and/or discount
        /// Then: UPA response contains TYP Void/Reverse Extra Fields
        /// And: All response properties are mapped by SDK object
        /// </summary>
        [TestMethod]
        public void Void_WithTyp_ByTerminalRefNumber_ShouldReturnSuccess() {
            // Arrange - First perform a sale
            decimal amount = 75.00m;
            var saleResponse = _device.Sale(amount)
                .WithEcrId(TEST_ECR_ID)
                .WithClerkId(789)
                .Execute();

            Assert.IsNotNull(saleResponse, "Sale response should not be null");
            Assert.AreEqual("Success", saleResponse.Status, "Sale should be successful");

            Thread.Sleep(HARDWARE_SETTLE_MS);

            // Act - Void the sale
            var voidResponse = _device.Void()
                .WithEcrId(TEST_ECR_ID)
                .WithTerminalRefNumber(saleResponse.TerminalRefNumber)
                .Execute();

            // Assert
            Assert.IsNotNull(voidResponse, "Void response should not be null");
            Assert.AreEqual("Success", voidResponse.Status, "Void should be successful");

            // TYP void fields — requires merchant configured with TYP loyalty
            var upaResponse = voidResponse as UPAResponseHandler;
            Assert.IsNotNull(upaResponse, "Response must be UPAResponseHandler");
            Assert.IsNotNull(upaResponse.VoidRedeemId, "VoidRedeemId must be present for a TYP void");
            Assert.IsNotNull(upaResponse.VoidRedeemStatus, "VoidRedeemStatus must be present");
            Assert.AreEqual("COMPLETE", upaResponse.VoidRedeemStatus.ToUpper(), "VoidRedeemStatus must be COMPLETE");
            Assert.IsNotNull(upaResponse.VoidCurrencyAmountRedeemed, "VoidCurrencyAmountRedeemed must be present");
            Assert.IsNotNull(upaResponse.VoidPointsRedeemed, "VoidPointsRedeemed must be present");
            Assert.IsNotNull(upaResponse.VoidDiscountAmountRedeemed, "VoidDiscountAmountRedeemed must be present");
        }

        /// <summary>
        /// Test: Void with TYP verifying all void response fields are properly mapped
        /// </summary>
        [TestMethod]
        public void Void_WithTyp_ByTransactionId_ShouldMapAllVoidResponseFields() {
            // Arrange - First perform a sale
            decimal amount = 50.00m;
            var saleResponse = _device.Sale(amount)
                .WithEcrId(TEST_ECR_ID)
                .WithClerkId(321)
                .Execute();

            Assert.AreEqual("Success", saleResponse.Status);
            Thread.Sleep(HARDWARE_SETTLE_MS);

            // Act - Void the sale
            var voidResponse = _device.Void()
                .WithEcrId(TEST_ECR_ID)
                .WithTransactionId(saleResponse.TransactionId)
                .Execute();

            // Assert
            Assert.IsNotNull(voidResponse, "Void response should not be null");
            var upaResponse = voidResponse as UPAResponseHandler;
            Assert.IsNotNull(upaResponse, "Response should be UPAResponseHandler type");
            Assert.IsNotNull(upaResponse.VoidRedeemId, "VoidRedeemId must be mapped from device response");
            Assert.IsNotNull(upaResponse.VoidRedeemStatus, "VoidRedeemStatus must be mapped from device response");
            Assert.IsNotNull(upaResponse.VoidCurrencyAmountRedeemed, "VoidCurrencyAmountRedeemed must be mapped as decimal?");
            Assert.IsNotNull(upaResponse.VoidPointsRedeemed, "VoidPointsRedeemed must be mapped as decimal?");
            Assert.IsNotNull(upaResponse.VoidDiscountAmountRedeemed, "VoidDiscountAmountRedeemed must be mapped as decimal?");
        }

        #endregion

        #region Reverse Sale with TYP and/or Discount Tests

        /// <summary>
        /// Test: Reverse a sale with TYP and/or discount
        /// Given: UPA device for Mexico merchant
        /// When: Reverse is submitted for Sale with TYP and/or discount
        /// Then: UPA response contains TYP Void/Reverse Extra Fields
        /// And: All response properties are mapped by SDK object
        /// </summary>
        [TestMethod]
        public void Reverse_WithTyp_ShouldReturnSuccess() {
            // Arrange - First perform a sale
            var saleResponse = _device.Sale(120.00m)
                .WithEcrId(TEST_ECR_ID)
                .WithClerkId(789)
                .Execute();

            Assert.IsNotNull(saleResponse, "Sale response should not be null");
            Assert.AreEqual("Success", saleResponse.Status, "Sale should be successful");
            Assert.IsNotNull(saleResponse.TerminalRefNumber, "TerminalRefNumber should be present");

            Thread.Sleep(HARDWARE_SETTLE_MS);

            // Act - Reverse the transaction using the terminal reference number
            var reversalResponse = _device.Reverse()
                .WithTerminalRefNumber(saleResponse.TerminalRefNumber)
                .WithEcrId(TEST_ECR_ID)
                .Execute();

            // Assert
            Assert.IsNotNull(reversalResponse, "Reversal response should not be null");
            Assert.AreEqual("Success", reversalResponse.Status, "Reversal should be successful");

            // TYP void/reverse fields — requires merchant configured with TYP loyalty
            var upaResponse = reversalResponse as UPAResponseHandler;
            Assert.IsNotNull(upaResponse, "Response must be UPAResponseHandler");
            Assert.IsNotNull(upaResponse.VoidRedeemId, "VoidRedeemId must be present for a TYP reversal");
            Assert.IsNotNull(upaResponse.VoidRedeemStatus, "VoidRedeemStatus must be present");
            Assert.AreEqual("COMPLETE", upaResponse.VoidRedeemStatus.ToUpper(), "VoidRedeemStatus must be COMPLETE");
            Assert.IsNotNull(upaResponse.VoidCurrencyAmountRedeemed, "VoidCurrencyAmountRedeemed must be present");
            Assert.IsNotNull(upaResponse.VoidPointsRedeemed, "VoidPointsRedeemed must be present");
            Assert.IsNotNull(upaResponse.VoidDiscountAmountRedeemed, "VoidDiscountAmountRedeemed must be present");
        }

        #endregion

        #region Summary Reports with TYP Tests

        /// <summary>
        /// Test: List summary reports with TYP
        /// Given: UPA device for Mexico merchant
        /// When: Query for list of summary report info
        /// Then: Request includes TYP Report Extra Fields
        /// And: All response properties are mapped by SDK object
        /// </summary>
        [TestMethod]
        public void GetBatchDetailsReport_WithTypSummary_ShouldReturnSuccess() {
            // Arrange
            var batchId = BATCH_ID;

            // Act
            var report = _device.GetBatchDetailsReport()
                .Where(UpaSearchCriteria.Batch, batchId)
                .And(UpaSearchCriteria.EcrId, TEST_ECR_ID)
                .And(UpaSearchCriteria.ReportType, UpaReportType.Summary)
                .And(UpaSearchCriteria.ReportSubType, UpaReportSubType.ByReference)
                .Execute() as BatchReportResponse;

            // Assert
            Assert.IsNotNull(report, "Report should not be null");
            Assert.AreEqual("Success", report.Status, "Report retrieval should be successful");
        }

        /// <summary>
        /// Test: Summary reports with clerk filter
        /// </summary>
        [TestMethod]
        public void GetBatchDetailsReport_WithTypSummaryAndClerkFilter_ShouldReturnSuccess() {
            // Arrange
            var batchId = BATCH_ID;
            var clerkId = 123;

            // Act
            var report = _device.GetBatchDetailsReport()
                .Where(UpaSearchCriteria.Batch, batchId)
                .And(UpaSearchCriteria.EcrId, TEST_ECR_ID)
                .And(UpaSearchCriteria.ReportType, UpaReportType.Summary)
                .And(UpaSearchCriteria.ReportSubType, UpaReportSubType.ByClerk)
                .And(UpaSearchCriteria.ClerkId, clerkId)
                .Execute() as BatchReportResponse;

            // Assert
            Assert.IsNotNull(report, "Report should not be null");
            Assert.AreEqual("Success", report.Status, "Report retrieval should be successful");
        }

        /// <summary>
        /// Test: Summary reports with both reports flag
        /// </summary>
        [TestMethod]
        public void GetBatchDetailsReport_WithTypBothReports_ShouldReturnSuccess() {
            // Arrange
            var batchId = BATCH_ID;

            // Act
            var report = _device.GetBatchDetailsReport()
                .Where(UpaSearchCriteria.Batch, batchId)
                .And(UpaSearchCriteria.EcrId, TEST_ECR_ID)
                .And(UpaSearchCriteria.BothReports, true)
                .Execute() as BatchReportResponse;

            // Assert
            Assert.IsNotNull(report, "Report should not be null");
            Assert.AreEqual("Success", report.Status, "Report retrieval should be successful");
        }

        #endregion

        #region Detail Reports with TYP Tests

        /// <summary>
        /// Test: Detail reports with TYP
        /// Given: UPA device for Mexico merchant
        /// When: Query for single detail report info
        /// Then: Request includes TYP Report Extra Fields
        /// And: All response properties are mapped by SDK object
        /// </summary>
        [TestMethod]
        public void GetBatchDetailsReport_WithTypDetail_ShouldReturnSuccess() {
            // Arrange
            var batchId = BATCH_ID;

            // Act
            var report = _device.GetBatchDetailsReport()
                .Where(UpaSearchCriteria.Batch, batchId)
                .And(UpaSearchCriteria.EcrId, TEST_ECR_ID)
                .And(UpaSearchCriteria.ReportType, UpaReportType.Detail)
                .And(UpaSearchCriteria.ReportSubType, UpaReportSubType.ByReference)
                .Execute() as BatchReportResponse;

            // Assert
            Assert.IsNotNull(report, "Report should not be null");
            Assert.AreEqual("Success", report.Status, "Report retrieval should be successful");
        }

        /// <summary>
        /// Test: Detail reports with previous batch flag
        /// </summary>
        [TestMethod]
        public void GetBatchDetailsReport_WithTypDetailAndPreviousBatch_ShouldReturnSuccess() {
            // Arrange
            var batchId = BATCH_ID;

            // Act
            var report = _device.GetBatchDetailsReport()
                .Where(UpaSearchCriteria.Batch, batchId)
                .And(UpaSearchCriteria.EcrId, TEST_ECR_ID)
                .And(UpaSearchCriteria.ReportType, UpaReportType.Detail)
                .And(UpaSearchCriteria.ReportSubType, UpaReportSubType.ByReference)
                .And(UpaSearchCriteria.PreviousBatchReport, true)
                .Execute() as BatchReportResponse;

            // Assert
            Assert.IsNotNull(report, "Report should not be null");
            Assert.AreEqual("Success", report.Status, "Report retrieval should be successful");
        }

        /// <summary>
        /// Test: Detail reports with all clerks filter
        /// </summary>
        [TestMethod]
        public void GetBatchDetailsReport_WithTypDetailAllClerks_ShouldReturnSuccess() {
            // Arrange
            var batchId = BATCH_ID;

            // Act
            var report = _device.GetBatchDetailsReport()
                .Where(UpaSearchCriteria.Batch, batchId)
                .And(UpaSearchCriteria.EcrId, TEST_ECR_ID)
                .And(UpaSearchCriteria.ReportType, UpaReportType.Detail)
                .And(UpaSearchCriteria.ReportSubType, UpaReportSubType.ByAllClerks)
                .Execute() as BatchReportResponse;

            // Assert
            Assert.IsNotNull(report, "Report should not be null");
            Assert.AreEqual("Success", report.Status, "Report retrieval should be successful");
        }

        #endregion

        #region Negative Test Cases

        /// <summary>
        /// Test: Reverse with a TerminalRefNumber that is too short (invalid length)
        /// Given: A TerminalRefNumber of only 2 characters ("23") is supplied
        /// When: Reverse is submitted
        /// Then: GatewayException is thrown with ERR011 — INVALID LENGTH
        /// </summary>
        [TestMethod]
        public void Reverse_WithInvalidTerminalRefNumber_ShouldThrowGatewayException() {
            var ex = Assert.ThrowsException<GatewayException>(() => {
                _device.Reverse()
                    .WithTerminalRefNumber("23") // intentionally short (2 chars) to trigger ERR011 INVALID LENGTH
                    .WithEcrId("12") // intentionally uses ECR 12 (not TEST_ECR_ID=13) to isolate the length-validation error path
                    .Execute();
            });
            Assert.AreEqual("Unexpected Device Response : ERR011 - [tranNo:23]-INVALID LENGTH", ex.Message);
        }

        /// <summary>
        /// Test: Void without supplying a TransactionId or TerminalRefNumber
        /// Given: Neither a transaction ID nor a terminal reference number is provided
        /// When: Void is submitted
        /// Then: GatewayException is thrown with VOID003 — NO TRANNO OR REFERENCENUMBER SUPPLIED
        /// </summary>
        [TestMethod]
        public void Void_WithoutRefNumber_ShouldThrowGatewayException() {
            var ex = Assert.ThrowsException<GatewayException>(() => {
                _device.Void()
                    .WithEcrId(TEST_ECR_ID)
                    .Execute();
            });
            Assert.AreEqual("Unexpected Device Response : VOID003 - NO TRANNO OR REFERENCENUMBER SUPPLIED", ex.Message);
        }

        #endregion
    }
}
