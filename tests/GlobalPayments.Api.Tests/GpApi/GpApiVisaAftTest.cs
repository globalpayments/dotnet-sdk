using System;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Enums;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.GpApi {
    /// <summary>
    /// Tests GP-API Visa Direct AFT authorization and sale transactions.
    /// </summary>
    [TestClass]
    public class GpApiVisaAftTest : BaseGpApiTests {
        private const decimal AMOUNT = 19.99m;
        private const string CURRENCY = "GBP";
        private const string AFT_TYPE = "VISA_DIRECT_AFT";
        private CreditCardData card;
        private GpApiConfig config;

        /// <summary>
        /// Configures the UK GP-API sandbox and initializes the Visa test card.
        /// </summary>
        [TestInitialize]
        public void TestInitialize() {
            ServicesContainer.RemoveConfig();
            config = GpApiConfigSetup(AppId, AppKey, Channel.CardNotPresent);
            config.Country = "UK";
            ServicesContainer.ConfigureService(config);

            card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = ExpMonth,
                ExpYear = ExpYear,
                Cvn = "123",
                CardPresent = true
            };
        }

        #region Authorization Tests

        /// <summary>
        /// Verifies GP-API processes a Visa Direct AFT authorization with supplementary data.
        /// </summary>
        [TestMethod]
        public void CreditAuthorizationWithVisaAft() {
            var response = card.Authorize(AMOUNT)
                .WithCurrency(CURRENCY)
                .WithSupplementaryData(
                    AFT_TYPE,
                    "Sender Name",
                    "Sender Address",
                    "Sender City",
                    "Sender Country",
                    "Account Number Type",
                    "Recipient Account Number")
                .WithAllowDuplicates(true)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Preauthorized), response.ResponseMessage);
        }

        /// <summary>
        /// Verifies GP-API captures a Visa Direct AFT sale with supplementary data.
        /// </summary>
        [TestMethod]
        public void CreditSaleWithVisaAft() {
            var response = card.Charge(AMOUNT)
                .WithCurrency(CURRENCY)
                .WithSupplementaryData(
                    AFT_TYPE,
                    "Sender Name",
                    "Sender Address",
                    "Sender City",
                    "Sender Country",
                    "Account Number Type",
                    "Recipient Account Number")
                .WithAllowDuplicates(true)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response.ResponseMessage);
        }

        [TestMethod]
        public void CreditSaleWithInvalidVisaAftType()
        {
            var exception = Assert.ThrowsException<GatewayException>(() => {
                card.Charge(AMOUNT)
                    .WithCurrency(CURRENCY)
                    .WithSupplementaryData(
                        "Sender Name",
                        "Sender Address",
                        "Sender City",
                        "Sender Country",
                        "Account Number Type",
                        "Recipient Account Number")
                    .WithAllowDuplicates(true)
                    .Execute();
            });

            Assert.AreEqual("INVALID_REQUEST_DATA", exception.ResponseCode);
            Assert.AreEqual("40090", exception.ResponseMessage);
            Assert.AreEqual(
                "Status Code: BadRequest - order.supplementary_data.type value is invalid. Please check the format and data provided is correct.",
                exception.Message);
        }

        /// <summary>
        /// Verifies a null supplementary-data type is rejected by the builder collection.
        /// </summary>
        [TestMethod]
        public void CreditSaleWithNullSupplementaryDataType() {
            var exception = Assert.ThrowsException<ArgumentNullException>(() => {
                card.Charge(AMOUNT)
                    .WithCurrency(CURRENCY)
                    .WithSupplementaryData(null, (string[])null);
            });

            Assert.AreEqual("key", exception.ParamName);
        }

        /// <summary>
        /// Verifies a credit sale succeeds when the supplementary-data type is empty.
        /// </summary>
        [TestMethod]
        public void CreditSaleWithEmptySupplementaryDataType() {
            var response = card.Charge(AMOUNT)
                .WithCurrency(CURRENCY)
                .WithSupplementaryData(string.Empty, "Sender Name")
                .WithAllowDuplicates(true)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response.ResponseMessage);
        }

        /// <summary>
        /// Verifies a credit sale succeeds when supplementary-data fields are null.
        /// </summary>
        [TestMethod]
        public void CreditSaleWithNullSupplementaryDataFields() {
            var response = card.Charge(AMOUNT)
                .WithCurrency(CURRENCY)
                .WithSupplementaryData(AFT_TYPE, (string[])null)
                .WithAllowDuplicates(true)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response.ResponseMessage);
        }

        /// <summary>
        /// Verifies a credit sale succeeds when supplementary-data fields are empty.
        /// </summary>
        [TestMethod]
        public void CreditSaleWithEmptySupplementaryDataFields() {
            var response = card.Charge(AMOUNT)
                .WithCurrency(CURRENCY)
                .WithSupplementaryData(AFT_TYPE, new string[0])
                .WithAllowDuplicates(true)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response.ResponseMessage);
        }

        #endregion

        #region Regression Tests

        /// <summary>
        /// Verifies a standard GP-API sale remains successful without supplementary data.
        /// </summary>
        [TestMethod]
        public void CreditSaleWithoutVisaAft() {
            var response = card.Charge(AMOUNT)
                .WithCurrency(CURRENCY)
                .WithAllowDuplicates(true)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response.ResponseMessage);
        }

        #endregion
    }
}
