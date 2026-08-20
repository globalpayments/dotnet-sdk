using System;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.GpApi {
    /// <summary>
    /// Tests for the Click to Pay decrypt flow via the GP-API /decrypt endpoint.
    /// Region: Europe (UK/CZ/PL), Gateway: GP-API, Environment: Sandbox.
    /// The positive decrypt tests require a valid token in CTP_VISA_TOKEN when running end-to-end.
    /// </summary>
    [TestClass]
    public class GpApiClickToPayDecryptTest : BaseGpApiTests {
        private const string EuConfigName = "EU_CTP";
        private const string EuTokenizeConfigName = "EU_CTP_TOKENIZE";
        // Visa Click to Pay encrypted token (sandbox). Replace with a fresh capture for live runs.
        private const string CTP_VISA_TOKEN =
            "eyJraWQiOiJKN00zRTE0V1pVUDhHRkxUQkUzODEzcm1ydC0tRGRFTTZGZEZORVdQakpjNVJfTHZrIiwiYWxnIjoiUlMyNTYiLCJqdGkiOiJOV000WlRGak9HSXRPREF4T1MwME56aGtMVGsyWldJdE0yTXhNR0l5TWpZMFpHTTUiLCJpYXQiOjE3NzU1NDg5ODJ9.eyJzcmNDb3JyZWxhdGlvbklkIjoiYWE4N2ZhNzYtYjBlYi00MTFlLWMzZTQtMTRkOGU1OGUyZjAxIiwic3JjaVRyYW5zYWN0aW9uSWQiOiI3MjYyOWQ2Ni0zYTBmLTRjMjUtOGU3Ny0wMzYxYzU1NDhkZGYiLCJtYXNrZWRDYXJkIjp7InNyY0RpZ2l0YWxDYXJkSWQiOiI5ZmFmYjM4NTA2MzQwY2YyMzE3MjFmZDgzNjBlYWQwMiIsInBhbkJpbiI6IjQzOTU4NCIsInBhbkxhc3RGb3VyIjoiMDExMCIsInRva2VuQmluUmFuZ2UiOiI0OTA2MjQ2OTciLCJwYXltZW50QWNjb3VudFJlZmVyZW5jZSI6IlYwMDEwMDEzMDI0MzI1NjcxNjE0MjU3NzQ0MTgwIiwidG9rZW5MYXN0Rm91ciI6IjAyMDAiLCJwYW5FeHBpcmF0aW9uTW9udGgiOiIxMCIsInBhbkV4cGlyYXRpb25ZZWFyIjoiMjAzMiIsImRpZ2l0YWxDYXJkRGF0YSI6eyJzdGF0dXMiOiJBQ1RJVkUiLCJkZXNjcmlwdG9yTmFtZSI6Ik9CTiIsImFydFVyaSI6Imh0dHBzOi8vc2FuZGJveC5hc3NldHMudmltcy52aXNhLmNvbS92aW1zL2NhcmRhcnQvNWFmMzczNGNjYTRlNDM5ZTk3ZjAwZTQ0NzRkODI0NDdfaW1hZ2VBQDJ4LnBuZyIsImFydEhlaWdodCI6MjEwLCJhcnRXaWR0aCI6MzM0fSwiZGF0ZW9mQ2FyZENyZWF0ZWQiOjE3NzI1MzcwNDI2ODMsImRhdGVvZkNhcmRMYXN0VXNlZCI6MTc3NTExMjQ0MDc4NywibWFza2VkQmlsbGluZ0FkZHJlc3MiOnsiYWRkcmVzc0lkIjoiZmI0NGQ1ZTUtNGJmMS00NWJhLWJhZDgtNWNkOTk2MzRmMDhmIiwiY291bnRyeUNvZGUiOiJHQiJ9LCJlbGlnaWJsZSI6ZmFsc2UsInBheW1lbnRDYXJkVHlwZSI6IkRFQklUIiwidG9rZW5JZCI6ImNjOTE4YTIxMDAxYmMwMTgxNWM2MTQ5MjQxMWJlMzAyIn0sIm1hc2tlZENvbnN1bWVyIjp7InNyY0NvbnN1bWVySWQiOiJDRkpHcXdxNDZEaDZZaEVRdXA2YzM4d2ZQVENGSmlXYXhHS3NUNmpTZWJNPSIsImZpcnN0TmFtZSI6IlQqKioqKiIsImxhc3ROYW1lIjoiVioqKioqIiwiZnVsbE5hbWUiOiJUKioqKiogVioqKioqIiwiZW1haWxBZGRyZXNzIjoidmxhKipAZ2xvYmFscGF5LmNvbSIsIm1vYmlsZU51bWJlciI6eyJjb3VudHJ5Q29kZSI6IjQwIiwicGhvbmVOdW1iZXIiOiIqKioqKioqKjU2NTQifSwiY291bnRyeUNvZGUiOiJHQiIsImxhbmd1YWdlQ29kZSI6ImVuLUdCIiwic3RhdHVzIjoiQUNUSVZFIn0sImFzc3VyYW5jZURhdGEiOnsiZWNpIjoiMDcifSwiaXNHdWVzdENoZWNrb3V0IjpmYWxzZSwiaXNOZXdVc2VyIjpmYWxzZX0.PLKjFxIbA1mR0rEqELHmsNMhOv7P-ocTS4BskuIdwpL6q3lSpfeBymQ3U1p6oUdSbk1q0qoaThX-s845P9cDugl8K0r79Ng3huMUGfgXL25opdWKRUrIciS0y13hgUjyBku44_pvZuoAQ1ua0F1y6maKBia6_T0bFTKKQVLUBuZzIe_viL3i2m388M95chAVrSCum5XBFG46XysAox1L7FNm2I_UvE0QEmWvewVzwjd4BHfCVhGzCfr2mLURHHJYKvAEyT7WLYkCq4VpvKKkm4O-DouKE358OJtBiXYma7jlGc2IyWV0-gf2VV6m07h6o4TGYcORP9OMIg6GrEOaew";
        private const string CTP_DPA_REFERENCE = "08f56394-4599-af88-ff38-1a64db7c6502";

        /// <summary>
        /// Registers the EU Click to Pay decrypt config and the EU tokenization config as named
        /// services once for the class; both are static and never change between tests.
        /// </summary>
        [ClassInitialize]
        public static void ClassInitialize(TestContext context) {
            // Click to Pay decrypt is EU-only (UK/CZ/PL); use the EU CTP app + EU service URL.
            var euConfig = GpApiConfigSetup(EuHppAppId, EuHppAppKey, Channel.CardNotPresent);
            euConfig.ServiceUrl = ServiceEndpoints.GP_API_EU_TEST;
            euConfig.AccessTokenInfo = new AccessTokenInfo {
                TransactionProcessingAccountName = "GPECOM_Transaction_Processing_CNP"
            };
            ServicesContainer.ConfigureService(euConfig, EuConfigName);

            var tokenizeConfig = GpApiConfigSetup(EuAppId, EuAppKey, Channel.CardNotPresent);
            tokenizeConfig.ServiceUrl = ServiceEndpoints.GP_API_EU_TEST;
            tokenizeConfig.AccessTokenInfo = new AccessTokenInfo {
                TransactionProcessingAccountName = "GPECOM_Transaction_Processing_CNP"
            };
            tokenizeConfig.Permissions = new[] {
                "PMT_POST_Create_Single",
                "BIN_GET_Details",
                "INS_POST_Query",
                "TRN_POST_Initiate",
                "CCS_POST_DCC",
                "AUT_POST_Initiate",
                "AUT_POST_Check_Availability",
                "AUT_POST_Results"
            };
            ServicesContainer.ConfigureService(tokenizeConfig, EuTokenizeConfigName);
        }

        #region Positive Tests

        /// <summary>
        /// Positive: decrypts a Click to Pay encrypted token through the GP-API /decrypt endpoint.
        /// Verifies the response surfaces the DEC_ID (DecryptId), the single-use PMT_ID (Token), the
        /// CLICK_TO_PAY provider, the mapped card details (brand, last 4, ECI), the payment account
        /// reference, and the payer details returned by the wallet (name, language, verification type,
        /// time created).
        /// Prerequisite: CTP_VISA_TOKEN must hold a freshly generated Click to Pay encrypted token;
        /// replace it before running end-to-end or the gateway returns
        /// 40085 "Invalid token provided.".
        /// </summary>
        [TestMethod]
        public void ClickToPayDecrypt() {
            // Arrange
            var card = new CreditCardData {
                CardHolderName = "James Mason",
                Token = CTP_VISA_TOKEN,
                MobileType = EncyptedMobileType.CLICK_TO_PAY,
                CardType = "visa",
                DpaReference = CTP_DPA_REFERENCE
            };

            // Act
            var response = card.Decrypt()
                .WithCurrency("EUR")
                .Execute(EuConfigName);

            // Assert
            Assert.IsNotNull(response);
            Assert.AreEqual(Success, response.ResponseCode);
            Assert.IsFalse(string.IsNullOrEmpty(response.DecryptId));
            Assert.IsFalse(string.IsNullOrEmpty(response.Token));
            Assert.AreEqual("CLICK_TO_PAY", response.DigitalWalletProvider);
            Assert.IsNotNull(response.CardDetails);
            Assert.IsFalse(string.IsNullOrEmpty(response.CardDetails.Brand));
            Assert.IsFalse(string.IsNullOrEmpty(response.CardDetails.Eci));
            Assert.IsFalse(string.IsNullOrEmpty(response.CardLast4));
            Assert.IsFalse(string.IsNullOrEmpty(response.PaymentAccountReference));
            Assert.IsNotNull(response.PayerDetails);
            Assert.IsFalse(string.IsNullOrEmpty(response.PayerDetails.FirstName));
            Assert.IsFalse(string.IsNullOrEmpty(response.PayerDetails.LastName));
            Assert.IsFalse(string.IsNullOrEmpty(response.PayerDetails.Language));
            Assert.IsFalse(string.IsNullOrEmpty(response.PayerDetails.VerificationType));
            Assert.IsFalse(string.IsNullOrEmpty(response.PayerDetails.TimeCreated));
        }

        /// <summary>
        /// Positive end-to-end flow: first decrypts the Click to Pay encrypted token to obtain the DEC_ID
        /// and single-use PMT_ID, then charges the PMT_ID while referencing the DEC_ID and DPA reference.
        /// Verifies the decrypt step returns both identifiers and that the subsequent charge is captured.
        /// Prerequisite: CTP_VISA_TOKEN must hold a freshly generated Click to Pay encrypted token,
        /// and the PMT_ID it produces is single-use (a second charge attempt returns
        /// 40116 "payment_method ... not found").
        /// </summary>
        [TestMethod]
        public void ClickToPayDecryptThenCharge() {
            // Arrange
            var card = new CreditCardData {
                CardHolderName = "James Mason",
                Token = CTP_VISA_TOKEN,
                MobileType = EncyptedMobileType.CLICK_TO_PAY,
                CardType = "visa",
                DpaReference = CTP_DPA_REFERENCE
            };

            // Act
            var decrypted = card.Decrypt()
                .WithCurrency("EUR")
                .Execute(EuConfigName);

            // Assert
            Assert.IsNotNull(decrypted);
            Assert.AreEqual(Success, decrypted.ResponseCode);
            Assert.IsFalse(string.IsNullOrEmpty(decrypted.DecryptId));
            Assert.IsFalse(string.IsNullOrEmpty(decrypted.Token));

            var chargeCard = new CreditCardData {
                CardHolderName = "James Mason",
                MobileType = EncyptedMobileType.CLICK_TO_PAY,
                Token = decrypted.Token,
                DecryptId = decrypted.DecryptId,
                DpaReference = CTP_DPA_REFERENCE
            };

            var response = chargeCard.Charge(10m)
                .WithCurrency("EUR")
                .Execute(EuConfigName);

            Assert.IsNotNull(response);
            // Post-decrypt charge returns the raw processor approval code ("00"), not the "SUCCESS" status.
            Assert.AreEqual("00", response.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response.ResponseMessage);
            Assert.IsFalse(string.IsNullOrEmpty(response.TransactionId));
        }

        /// <summary>
        /// Positive: decrypts then charges the PMT_ID while sending an SCA exemption
        /// (authentication.three_ds.exempt_status = LOW_VALUE), mirroring the Java decrypt-then-charge
        /// test. Verifies the charge is captured.
        /// </summary>
        [TestMethod]
        public void ClickToPayDecryptThenCharge_WithThreeDsExemptStatus() {
            // Arrange
            var card = new CreditCardData {
                CardHolderName = "James Mason",
                Token = CTP_VISA_TOKEN,
                MobileType = EncyptedMobileType.CLICK_TO_PAY,
                CardType = "visa",
                DpaReference = CTP_DPA_REFERENCE
            };

            // Act
            var decrypted = card.Decrypt()
                .WithCurrency("EUR")
                .Execute(EuConfigName);

            // Assert
            Assert.IsNotNull(decrypted);
            Assert.AreEqual(Success, decrypted.ResponseCode);

            var chargeCard = new CreditCardData {
                CardHolderName = "Jason",
                MobileType = EncyptedMobileType.CLICK_TO_PAY,
                Token = decrypted.Token,
                DecryptId = decrypted.DecryptId,
                DpaReference = CTP_DPA_REFERENCE,
                ThreeDSecure = new ThreeDSecure { ExemptStatus = ExemptStatus.LOW_VALUE }
            };

            var response = chargeCard.Charge(1m)
                .WithCurrency("EUR")
                .Execute(EuConfigName);

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response.ResponseMessage);
            Assert.IsFalse(string.IsNullOrEmpty(response.TransactionId));
        }

        /// <summary>
        /// Positive: decrypts then charges the PMT_ID with a dynamic descriptor (payment_method.narrative)
        /// and a client reference. Verifies the charge is captured and echoes the reference.
        /// </summary>
        [TestMethod]
        public void ClickToPayDecryptThenCharge_WithDynamicDescriptorAndReference() {
            // Arrange
            var card = new CreditCardData {
                CardHolderName = "James Mason",
                Token = CTP_VISA_TOKEN,
                MobileType = EncyptedMobileType.CLICK_TO_PAY,
                CardType = "visa",
                DpaReference = CTP_DPA_REFERENCE
            };

            // Act
            var decrypted = card.Decrypt()
                .WithCurrency("EUR")
                .Execute(EuConfigName);

            // Assert
            Assert.IsNotNull(decrypted);
            Assert.AreEqual(Success, decrypted.ResponseCode);

            var chargeCard = new CreditCardData {
                CardHolderName = "Jason",
                MobileType = EncyptedMobileType.CLICK_TO_PAY,
                Token = decrypted.Token,
                DecryptId = decrypted.DecryptId,
                DpaReference = CTP_DPA_REFERENCE
            };

            var reference = Guid.NewGuid().ToString();
            var response = chargeCard.Charge(2m)
                .WithCurrency("EUR")
                .WithDynamicDescriptor("OBN")
                .WithClientTransactionId(reference)
                .Execute(EuConfigName);

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response.ResponseMessage);
            Assert.AreEqual(reference, response.ReferenceNumber);
        }

        /// <summary>
        /// Positive: decrypts then charges the PMT_ID in a single request that combines the SCA exemption
        /// (authentication.three_ds.exempt_status = LOW_VALUE), the dynamic descriptor
        /// (payment_method.narrative = "OBN"), the payer name and a client reference. Verifies the charge
        /// is captured and echoes the reference.
        /// </summary>
        [TestMethod]
        public void ClickToPayDecryptThenCharge_WithThreeDsExemptStatusAndDynamicDescriptor() {
            // Arrange
            var card = new CreditCardData {
                CardHolderName = "James Mason",
                Token = CTP_VISA_TOKEN,
                MobileType = EncyptedMobileType.CLICK_TO_PAY,
                CardType = "visa",
                DpaReference = CTP_DPA_REFERENCE
            };

            // Act
            var decrypted = card.Decrypt()
                .WithCurrency("EUR")
                .Execute(EuConfigName);

            // Assert
            Assert.IsNotNull(decrypted);
            Assert.AreEqual(Success, decrypted.ResponseCode);

            var chargeCard = new CreditCardData {
                CardHolderName = "Jason",
                MobileType = EncyptedMobileType.CLICK_TO_PAY,
                Token = decrypted.Token,
                DecryptId = decrypted.DecryptId,
                DpaReference = CTP_DPA_REFERENCE,
                ThreeDSecure = new ThreeDSecure { ExemptStatus = ExemptStatus.LOW_VALUE }
            };

            var reference = Guid.NewGuid().ToString();
            var response = chargeCard.Charge(1m)
                .WithCurrency("EUR")
                .WithDynamicDescriptor("OBN")
                .WithClientTransactionId(reference)
                .Execute(EuConfigName);

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response.ResponseMessage);
            Assert.AreEqual(reference, response.ReferenceNumber);
        }

        /// <summary>
        /// Positive: tokenizes a card as a single-use payment method with no CVV (SINGLE_NO_CVN) and
        /// verifies a token is returned. Exercises the cvv_present=NO field the SDK now sends on the
        /// /payment-methods request when CreditCardData.CvvPresent is set.
        /// Uses the EU tokenization app that carries the PMT_POST_Create_Single permission.
        /// </summary>
        [TestMethod]
        public void TokenizeCardWithoutCvv_SingleUseNoCvn() {
            // Arrange
            var card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 5,
                ExpYear = 2030,
                CvvPresent = CvvPresent.NO
            };

            // Act
            var token = card.Tokenize(false, EuTokenizeConfigName, PaymentMethodUsageMode.Single);

            // Assert
            Assert.IsFalse(string.IsNullOrEmpty(token));
        }

        #endregion

        #region Negative Tests

        /// <summary>
        /// Negative: a decrypt request with a Click to Pay token but a mismatched (all-zero) DPA reference
        /// is rejected by the gateway. Verifies a GatewayException is thrown with a populated response
        /// message (the sandbox returns 40085 "Invalid token provided.").
        /// </summary>
        [TestMethod]
        public void ClickToPayDecrypt_WithMismatchedDpaReference_ShouldFail() {
            // Arrange
            var card = new CreditCardData {
                CardHolderName = "James Mason",
                Token = CTP_VISA_TOKEN,
                MobileType = EncyptedMobileType.CLICK_TO_PAY,
                CardType = "visa",
                DpaReference = "00000000-0000-0000-0000-000000000000"
            };

            // Act
            var ex = Assert.ThrowsException<GatewayException>(() =>
                card.Decrypt()
                    .WithCurrency("EUR")
                    .Execute(EuConfigName));

            // Assert
            Assert.AreEqual("INVALID_REQUEST_DATA", ex.ResponseCode);
            Assert.AreEqual("40085", ex.ResponseMessage);
            Assert.AreEqual("Status Code: BadRequest - Invalid token provided.", ex.Message);
        }

        /// <summary>
        /// Negative: a decrypt request without any encrypted token is rejected by the gateway.
        /// Verifies a GatewayException is thrown with the MANDATORY_DATA_MISSING / 40005 error naming the
        /// missing payment_token.data field.
        /// </summary>
        [TestMethod]
        public void ClickToPayDecrypt_MissingToken_ShouldFail() {
            // Arrange
            var card = new CreditCardData {
                CardHolderName = "James Mason",
                MobileType = EncyptedMobileType.CLICK_TO_PAY,
                CardType = "visa",
                DpaReference = CTP_DPA_REFERENCE
            };

            // Act
            var ex = Assert.ThrowsException<GatewayException>(() =>
                card.Decrypt()
                    .WithCurrency("EUR")
                    .Execute(EuConfigName));

            // Assert
            Assert.AreEqual("MANDATORY_DATA_MISSING", ex.ResponseCode);
            Assert.AreEqual("40005", ex.ResponseMessage);
            Assert.AreEqual("Status Code: BadRequest - Request expects the following fields payment_method.digital_wallet.payment_token.data", ex.Message);
        }

        /// <summary>
        /// Negative: a decrypt request with a valid Click to Pay token but no DPA reference is rejected by
        /// the gateway.
        /// </summary>
        [TestMethod]
        public void ClickToPayDecrypt_WithMissingDpaReference_ShouldFail() {
            // Arrange
            var card = new CreditCardData {
                CardHolderName = "James Mason",
                Token = CTP_VISA_TOKEN,
                MobileType = EncyptedMobileType.CLICK_TO_PAY,
                CardType = "visa"
            };

            // Act
            var ex = Assert.ThrowsException<GatewayException>(() =>
                card.Decrypt()
                    .WithCurrency("EUR")
                    .Execute(EuConfigName));

            // Assert
            Assert.AreEqual("MANDATORY_DATA_MISSING", ex.ResponseCode);
            Assert.AreEqual("40005", ex.ResponseMessage);
            Assert.AreEqual("Status Code: BadRequest - Request expects the following fields payment_method.digital_wallet.payment_token.dpa_reference", ex.Message);
        }

        #endregion
    }
}
