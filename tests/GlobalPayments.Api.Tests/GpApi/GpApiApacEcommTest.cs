using System.Globalization;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Utils;
using GlobalPayments.Api.Utils.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.GpApi {

    /// <summary>
    /// APAC eCommerce integration tests for GP-API — AH-2189 / Wave 24713.
    ///
    /// All 9 Phase 1 transaction types are executed against every provisioned currency,
    /// segregated into three test groups by ISO 4217 exponent so that CI failures
    /// surface by exponent family immediately:
    ///
    ///   _Exponent2 — two-decimal currencies (×100 on wire):
    ///     APAC settlement: SGD, HKD, MOP, PHP, MYR
    ///     JPY exception  : ISO exponent 0, but GP-API requires ×100
    ///     Other standard : AED, AUD, BDT, BND, BRL, CAD, CHF, CLP, CNY, DKK,
    ///                      EGP, EUR, GBP, IDR, ILS, INR, LKR, MUR, MVR, MXN,
    ///                      NOK, NZD, PGK, PKR, QAR, RUB, SAR, SEK, THB, TRY,
    ///                      TWD, USD, ZAR
    ///
    ///   _Exponent3 — milli-unit currencies (×1000 on wire):
    ///     BHD, KWD, OMR
    ///
    ///   _Exponent0 — whole-unit currencies (×1 on wire):
    ///     ISK, KRW, VND
    ///
    /// Transaction types covered (Phase 1):
    ///   Sale, Auth/Pre-Auth, Capture, Auth Reversal,
    ///   Refund (linked + standalone), Partial Capture (VT), MOTO (Sale + Auth).
    /// </summary>
    [TestClass]
    public class GpApiApacEcommTest : BaseGpApiTests {

        private const string ApacAppId  = "16Br1RfjChBrsFnWlu7NGIp9LKm2MWWFyGg3SU3UfEl3voA2";
        private const string ApacAppKey = "xV9wnRLmi8qPqvMZoxAH9S0RtoQlodCYuvCboVYUohW6DObtcrYL1uj4YOZilKyu";
        private const string EratyAppId = "hkjrcsGDhWiDt8GEhoDMKy3pzFz5R0Bo";
        private const string EratyAppKey = "cQOKHoAAvNIcEN8s"; //gitleaks:allow
        private const string BrandVisa        = "VISA";
        private const string BrandMastercard  = "MC";
        private const string DefaultConfigName = "defaultCurrency";
        // Works for all exponent families:
        //   Exponent 2 → encoded as "1000"  (10.00 × 100)
        //   Exponent 3 → encoded as "10000" (10.00 × 1000)
        //   Exponent 0 → encoded as "10"    (10.00 rounds to 10 whole units)
        private const decimal Amount = 10.00m;

        private const string VisaCardNumber        = "4263970000005262";
        private const string MastercardCardNumber  = "5425230000004415";

        private CreditCardData _visaCard;
        private CreditCardData _mastercardCard;
        private CreditCardData _motoVisaCard;
        private CreditCardData _motoMastercardCard;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context) {
            var config = GpApiConfigSetup(ApacAppId, ApacAppKey, Channel.CardNotPresent);
            config.Country = "SG";
            ServicesContainer.ConfigureService(config);


            var eratyConfig = GpApiConfigSetup(EratyAppId, EratyAppKey, Channel.CardNotPresent);
            eratyConfig.AccessTokenInfo = new AccessTokenInfo { TransactionProcessingAccountName = "GPECOM_APM_Transaction_Processing" };
            ServicesContainer.ConfigureService(eratyConfig, DefaultConfigName);
        }

        [TestInitialize]
        public void TestInitialize() {
            _visaCard = new CreditCardData {
                Number = VisaCardNumber,
                ExpMonth = ExpMonth,
                ExpYear = ExpYear,
                Cvn = "123",
                CardHolderName = "John Smith"
            };

            _mastercardCard = new CreditCardData {
                Number = MastercardCardNumber,
                ExpMonth = ExpMonth,
                ExpYear = ExpYear,
                Cvn = "123",
                CardHolderName = "Jane Doe"
            };

            _motoVisaCard = new CreditCardData {
                Number = VisaCardNumber,
                ExpMonth = ExpMonth,
                ExpYear = ExpYear,
                Cvn = "123",
                CardHolderName = "John Smith",
                EntryMethod = ManualEntryMethod.Moto
            };

            _motoMastercardCard = new CreditCardData {
                Number = MastercardCardNumber,
                ExpMonth = ExpMonth,
                ExpYear = ExpYear,
                Cvn = "123",
                CardHolderName = "Jane Doe",
                EntryMethod = ManualEntryMethod.Moto
            };
        }

        // -------------------------------------------------------------------------
        // Shared DataRow blocks — each block is pasted verbatim into every method
        // that belongs to the same exponent family.
        //
        // Exponent 2 (39 currencies): APAC settlement + JPY exception + 33 standard (incl. CLP per 24713)
        // Exponent 3  (3 currencies): BHD, KWD, OMR
        // Exponent 0  (3 currencies): ISK, KRW, VND
        // -------------------------------------------------------------------------

        #region Sale Tests

        [DataTestMethod]
        // --- APAC settlement (exponent 2) ---
        [DataRow(BrandVisa, "SGD")] [DataRow(BrandMastercard, "SGD")]
        [DataRow(BrandVisa, "HKD")] [DataRow(BrandMastercard, "HKD")]
        [DataRow(BrandVisa, "MOP")] [DataRow(BrandMastercard, "MOP")]
        [DataRow(BrandVisa, "PHP")] [DataRow(BrandMastercard, "PHP")]
        [DataRow(BrandVisa, "MYR")] [DataRow(BrandMastercard, "MYR")]
        // --- JPY: ISO exponent 0 but GP-API encodes as ×100 ---
        [DataRow(BrandVisa, "JPY")] [DataRow(BrandMastercard, "JPY")]
        // --- Other two-decimal currencies ---
        [DataRow(BrandVisa, "AED")] [DataRow(BrandMastercard, "AED")]
        [DataRow(BrandVisa, "AUD")] [DataRow(BrandMastercard, "AUD")]
        [DataRow(BrandVisa, "BDT")] [DataRow(BrandMastercard, "BDT")]
        [DataRow(BrandVisa, "BND")] [DataRow(BrandMastercard, "BND")]
        [DataRow(BrandVisa, "BRL")] [DataRow(BrandMastercard, "BRL")]
        [DataRow(BrandVisa, "CAD")] [DataRow(BrandMastercard, "CAD")]
        [DataRow(BrandVisa, "CHF")] [DataRow(BrandMastercard, "CHF")]
        [DataRow(BrandVisa, "CNY")] [DataRow(BrandMastercard, "CNY")]
        [DataRow(BrandVisa, "DKK")] [DataRow(BrandMastercard, "DKK")]
        [DataRow(BrandVisa, "EGP")] [DataRow(BrandMastercard, "EGP")]
        [DataRow(BrandVisa, "EUR")] [DataRow(BrandMastercard, "EUR")]
        [DataRow(BrandVisa, "GBP")] [DataRow(BrandMastercard, "GBP")]
        [DataRow(BrandVisa, "IDR")] [DataRow(BrandMastercard, "IDR")]
        [DataRow(BrandVisa, "ILS")] [DataRow(BrandMastercard, "ILS")]
        [DataRow(BrandVisa, "INR")] [DataRow(BrandMastercard, "INR")]
        [DataRow(BrandVisa, "LKR")] [DataRow(BrandMastercard, "LKR")]
        [DataRow(BrandVisa, "MUR")] [DataRow(BrandMastercard, "MUR")]
        [DataRow(BrandVisa, "MVR")] [DataRow(BrandMastercard, "MVR")]
        [DataRow(BrandVisa, "MXN")] [DataRow(BrandMastercard, "MXN")]
        [DataRow(BrandVisa, "NOK")] [DataRow(BrandMastercard, "NOK")]
        [DataRow(BrandVisa, "NZD")] [DataRow(BrandMastercard, "NZD")]
        [DataRow(BrandVisa, "PGK")] [DataRow(BrandMastercard, "PGK")]
        [DataRow(BrandVisa, "PKR")] [DataRow(BrandMastercard, "PKR")]
        [DataRow(BrandVisa, "QAR")] [DataRow(BrandMastercard, "QAR")]
        [DataRow(BrandVisa, "RUB")] [DataRow(BrandMastercard, "RUB")]
        [DataRow(BrandVisa, "SAR")] [DataRow(BrandMastercard, "SAR")]
        [DataRow(BrandVisa, "SEK")] [DataRow(BrandMastercard, "SEK")]
        [DataRow(BrandVisa, "THB")] [DataRow(BrandMastercard, "THB")]
        [DataRow(BrandVisa, "TRY")] [DataRow(BrandMastercard, "TRY")]
        [DataRow(BrandVisa, "TWD")] [DataRow(BrandMastercard, "TWD")]
        [DataRow(BrandVisa, "USD")] [DataRow(BrandMastercard, "USD")]
        [DataRow(BrandVisa, "ZAR")] [DataRow(BrandMastercard, "ZAR")]
        // CLP — exponent 2 per 24713 analysis (overrides ISO 4217 exponent 0)
        [DataRow(BrandVisa, "CLP")] [DataRow(BrandMastercard, "CLP")]
        public void CreditSale_Exponent2(string brand, string currency) {
            var card = NewCard(brand);
            var response = card.Charge(Amount)
                          .WithCurrency(currency)
                          .Execute();
            AssertTransactionResponse(response, TransactionStatus.Captured);
        }

        [DataTestMethod]
        // --- Milli-unit currencies (exponent 3, ×1000) ---
        [DataRow(BrandVisa, "BHD")] [DataRow(BrandMastercard, "BHD")]
        [DataRow(BrandVisa, "KWD")] [DataRow(BrandMastercard, "KWD")]
        [DataRow(BrandVisa, "OMR")] [DataRow(BrandMastercard, "OMR")]
        public void CreditSale_Exponent3(string brand, string currency) {
            var card = NewCard(brand);
            var response = card.Charge(Amount).WithCurrency(currency).Execute();
            AssertTransactionResponse(response, TransactionStatus.Captured);
        }

        [DataTestMethod]
        // --- Whole-unit currencies (exponent 0, ×1) ---
        [DataRow(BrandVisa, "ISK")] [DataRow(BrandMastercard, "ISK")]
        [DataRow(BrandVisa, "KRW")] [DataRow(BrandMastercard, "KRW")]
        [DataRow(BrandVisa, "VND")] [DataRow(BrandMastercard, "VND")]
        public void CreditSale_Exponent0(string brand, string currency) {
            var card = NewCard(brand);
            var response = card.Charge(Amount).WithCurrency(currency).Execute();
            AssertTransactionResponse(response, TransactionStatus.Captured);
        }

        [TestMethod]
        public void CreditSale_Mastercard_HKD() {
            var response = _mastercardCard.Charge(Amount)
                .WithCurrency("HKD")
                .Execute();
            AssertTransactionResponse(response, TransactionStatus.Captured);
        }

        #endregion

        #region Authorization / Pre-Auth Tests

        [DataTestMethod]
        [DataRow(BrandVisa, "SGD")] [DataRow(BrandMastercard, "SGD")]
        [DataRow(BrandVisa, "HKD")] [DataRow(BrandMastercard, "HKD")]
        [DataRow(BrandVisa, "MOP")] [DataRow(BrandMastercard, "MOP")]
        [DataRow(BrandVisa, "PHP")] [DataRow(BrandMastercard, "PHP")]
        [DataRow(BrandVisa, "MYR")] [DataRow(BrandMastercard, "MYR")]
        [DataRow(BrandVisa, "JPY")] [DataRow(BrandMastercard, "JPY")]
        [DataRow(BrandVisa, "AED")] [DataRow(BrandMastercard, "AED")]
        [DataRow(BrandVisa, "AUD")] [DataRow(BrandMastercard, "AUD")]
        [DataRow(BrandVisa, "BDT")] [DataRow(BrandMastercard, "BDT")]
        [DataRow(BrandVisa, "BND")] [DataRow(BrandMastercard, "BND")]
        [DataRow(BrandVisa, "BRL")] [DataRow(BrandMastercard, "BRL")]
        [DataRow(BrandVisa, "CAD")] [DataRow(BrandMastercard, "CAD")]
        [DataRow(BrandVisa, "CHF")] [DataRow(BrandMastercard, "CHF")]
        [DataRow(BrandVisa, "CNY")] [DataRow(BrandMastercard, "CNY")]
        [DataRow(BrandVisa, "DKK")] [DataRow(BrandMastercard, "DKK")]
        [DataRow(BrandVisa, "EGP")] [DataRow(BrandMastercard, "EGP")]
        [DataRow(BrandVisa, "EUR")] [DataRow(BrandMastercard, "EUR")]
        [DataRow(BrandVisa, "GBP")] [DataRow(BrandMastercard, "GBP")]
        [DataRow(BrandVisa, "IDR")] [DataRow(BrandMastercard, "IDR")]
        [DataRow(BrandVisa, "ILS")] [DataRow(BrandMastercard, "ILS")]
        [DataRow(BrandVisa, "INR")] [DataRow(BrandMastercard, "INR")]
        [DataRow(BrandVisa, "LKR")] [DataRow(BrandMastercard, "LKR")]
        [DataRow(BrandVisa, "MUR")] [DataRow(BrandMastercard, "MUR")]
        [DataRow(BrandVisa, "MVR")] [DataRow(BrandMastercard, "MVR")]
        [DataRow(BrandVisa, "MXN")] [DataRow(BrandMastercard, "MXN")]
        [DataRow(BrandVisa, "NOK")] [DataRow(BrandMastercard, "NOK")]
        [DataRow(BrandVisa, "NZD")] [DataRow(BrandMastercard, "NZD")]
        [DataRow(BrandVisa, "PGK")] [DataRow(BrandMastercard, "PGK")]
        [DataRow(BrandVisa, "PKR")] [DataRow(BrandMastercard, "PKR")]
        [DataRow(BrandVisa, "QAR")] [DataRow(BrandMastercard, "QAR")]
        [DataRow(BrandVisa, "RUB")] [DataRow(BrandMastercard, "RUB")]
        [DataRow(BrandVisa, "SAR")] [DataRow(BrandMastercard, "SAR")]
        [DataRow(BrandVisa, "SEK")] [DataRow(BrandMastercard, "SEK")]
        [DataRow(BrandVisa, "THB")] [DataRow(BrandMastercard, "THB")]
        [DataRow(BrandVisa, "TRY")] [DataRow(BrandMastercard, "TRY")]
        [DataRow(BrandVisa, "TWD")] [DataRow(BrandMastercard, "TWD")]
        [DataRow(BrandVisa, "USD")] [DataRow(BrandMastercard, "USD")]
        [DataRow(BrandVisa, "ZAR")] [DataRow(BrandMastercard, "ZAR")]
        // CLP — exponent 2 per 24713 analysis (overrides ISO 4217 exponent 0)
        [DataRow(BrandVisa, "CLP")] [DataRow(BrandMastercard, "CLP")]
        public void CreditAuthorization_Exponent2(string brand, string currency) {
            var card = NewCard(brand);
            var response = card.Authorize(Amount).WithCurrency(currency).Execute();
            AssertTransactionResponse(response, TransactionStatus.Preauthorized);
            AssertRoundTripAmount(response);
        }

        [DataTestMethod]
        [DataRow(BrandVisa, "BHD")] [DataRow(BrandMastercard, "BHD")]
        [DataRow(BrandVisa, "KWD")] [DataRow(BrandMastercard, "KWD")]
        [DataRow(BrandVisa, "OMR")] [DataRow(BrandMastercard, "OMR")]
        public void CreditAuthorization_Exponent3(string brand, string currency) {
            var card = NewCard(brand);
            var response = card.Authorize(Amount).WithCurrency(currency).Execute();
            AssertTransactionResponse(response, TransactionStatus.Preauthorized);
            AssertRoundTripAmount(response);
        }

        [DataTestMethod]
        [DataRow(BrandVisa, "ISK")] [DataRow(BrandMastercard, "ISK")]
        [DataRow(BrandVisa, "KRW")] [DataRow(BrandMastercard, "KRW")]
        [DataRow(BrandVisa, "VND")] [DataRow(BrandMastercard, "VND")]
        public void CreditAuthorization_Exponent0(string brand, string currency) {
            var card = NewCard(brand);
            var response = card.Authorize(Amount).WithCurrency(currency).Execute();
            AssertTransactionResponse(response, TransactionStatus.Preauthorized);
            AssertRoundTripAmount(response);
        }

        #endregion

        #region Capture Tests

        [DataTestMethod]
        [DataRow(BrandVisa, "SGD")] [DataRow(BrandMastercard, "SGD")]
        [DataRow(BrandVisa, "HKD")] [DataRow(BrandMastercard, "HKD")]
        [DataRow(BrandVisa, "MOP")] [DataRow(BrandMastercard, "MOP")]
        [DataRow(BrandVisa, "PHP")] [DataRow(BrandMastercard, "PHP")]
        [DataRow(BrandVisa, "MYR")] [DataRow(BrandMastercard, "MYR")]
        [DataRow(BrandVisa, "JPY")] [DataRow(BrandMastercard, "JPY")]
        [DataRow(BrandVisa, "AED")] [DataRow(BrandMastercard, "AED")]
        [DataRow(BrandVisa, "AUD")] [DataRow(BrandMastercard, "AUD")]
        [DataRow(BrandVisa, "BDT")] [DataRow(BrandMastercard, "BDT")]
        [DataRow(BrandVisa, "BND")] [DataRow(BrandMastercard, "BND")]
        [DataRow(BrandVisa, "BRL")] [DataRow(BrandMastercard, "BRL")]
        [DataRow(BrandVisa, "CAD")] [DataRow(BrandMastercard, "CAD")]
        [DataRow(BrandVisa, "CHF")] [DataRow(BrandMastercard, "CHF")]
        [DataRow(BrandVisa, "CNY")] [DataRow(BrandMastercard, "CNY")]
        [DataRow(BrandVisa, "DKK")] [DataRow(BrandMastercard, "DKK")]
        [DataRow(BrandVisa, "EGP")] [DataRow(BrandMastercard, "EGP")]
        [DataRow(BrandVisa, "EUR")] [DataRow(BrandMastercard, "EUR")]
        [DataRow(BrandVisa, "GBP")] [DataRow(BrandMastercard, "GBP")]
        [DataRow(BrandVisa, "IDR")] [DataRow(BrandMastercard, "IDR")]
        [DataRow(BrandVisa, "ILS")] [DataRow(BrandMastercard, "ILS")]
        [DataRow(BrandVisa, "INR")] [DataRow(BrandMastercard, "INR")]
        [DataRow(BrandVisa, "LKR")] [DataRow(BrandMastercard, "LKR")]
        [DataRow(BrandVisa, "MUR")] [DataRow(BrandMastercard, "MUR")]
        [DataRow(BrandVisa, "MVR")] [DataRow(BrandMastercard, "MVR")]
        [DataRow(BrandVisa, "MXN")] [DataRow(BrandMastercard, "MXN")]
        [DataRow(BrandVisa, "NOK")] [DataRow(BrandMastercard, "NOK")]
        [DataRow(BrandVisa, "NZD")] [DataRow(BrandMastercard, "NZD")]
        [DataRow(BrandVisa, "PGK")] [DataRow(BrandMastercard, "PGK")]
        [DataRow(BrandVisa, "PKR")] [DataRow(BrandMastercard, "PKR")]
        [DataRow(BrandVisa, "QAR")] [DataRow(BrandMastercard, "QAR")]
        [DataRow(BrandVisa, "RUB")] [DataRow(BrandMastercard, "RUB")]
        [DataRow(BrandVisa, "SAR")] [DataRow(BrandMastercard, "SAR")]
        [DataRow(BrandVisa, "SEK")] [DataRow(BrandMastercard, "SEK")]
        [DataRow(BrandVisa, "THB")] [DataRow(BrandMastercard, "THB")]
        [DataRow(BrandVisa, "TRY")] [DataRow(BrandMastercard, "TRY")]
        [DataRow(BrandVisa, "TWD")] [DataRow(BrandMastercard, "TWD")]
        [DataRow(BrandVisa, "USD")] [DataRow(BrandMastercard, "USD")]
        [DataRow(BrandVisa, "ZAR")] [DataRow(BrandMastercard, "ZAR")]
        // CLP — exponent 2 per 24713 analysis (overrides ISO 4217 exponent 0)
        [DataRow(BrandVisa, "CLP")] [DataRow(BrandMastercard, "CLP")]
        public void CreditCapture_Exponent2(string brand, string currency) {
            var card = NewCard(brand);
            var auth = card.Authorize(Amount).WithCurrency(currency).Execute();
            AssertTransactionResponse(auth, TransactionStatus.Preauthorized);
            var capture = auth.Capture(Amount).Execute();
            AssertTransactionResponse(capture, TransactionStatus.Captured);
        }

        [DataTestMethod]
        [DataRow(BrandVisa, "BHD")] [DataRow(BrandMastercard, "BHD")]
        [DataRow(BrandVisa, "KWD")] [DataRow(BrandMastercard, "KWD")]
        [DataRow(BrandVisa, "OMR")] [DataRow(BrandMastercard, "OMR")]
        public void CreditCapture_Exponent3(string brand, string currency) {
            var card = NewCard(brand);
            var auth = card.Authorize(Amount).WithCurrency(currency).Execute();
            AssertTransactionResponse(auth, TransactionStatus.Preauthorized);
            var capture = auth.Capture(Amount).Execute();
            AssertTransactionResponse(capture, TransactionStatus.Captured);
        }

        [DataTestMethod]
        [DataRow(BrandVisa, "ISK")] [DataRow(BrandMastercard, "ISK")]
        [DataRow(BrandVisa, "KRW")] [DataRow(BrandMastercard, "KRW")]
        [DataRow(BrandVisa, "VND")] [DataRow(BrandMastercard, "VND")]
        public void CreditCapture_Exponent0(string brand, string currency) {
            var card = NewCard(brand);
            var auth = card.Authorize(Amount).WithCurrency(currency).Execute();
            AssertTransactionResponse(auth, TransactionStatus.Preauthorized);
            var capture = auth.Capture(Amount).Execute();
            AssertTransactionResponse(capture, TransactionStatus.Captured);
        }

        #endregion

        #region Auth Reversal Tests

        [DataTestMethod]
        [DataRow(BrandVisa, "SGD")] [DataRow(BrandMastercard, "SGD")]
        [DataRow(BrandVisa, "HKD")] [DataRow(BrandMastercard, "HKD")]
        [DataRow(BrandVisa, "MOP")] [DataRow(BrandMastercard, "MOP")]
        [DataRow(BrandVisa, "PHP")] [DataRow(BrandMastercard, "PHP")]
        [DataRow(BrandVisa, "MYR")] [DataRow(BrandMastercard, "MYR")]
        [DataRow(BrandVisa, "JPY")] [DataRow(BrandMastercard, "JPY")]
        [DataRow(BrandVisa, "AED")] [DataRow(BrandMastercard, "AED")]
        [DataRow(BrandVisa, "AUD")] [DataRow(BrandMastercard, "AUD")]
        [DataRow(BrandVisa, "BDT")] [DataRow(BrandMastercard, "BDT")]
        [DataRow(BrandVisa, "BND")] [DataRow(BrandMastercard, "BND")]
        [DataRow(BrandVisa, "BRL")] [DataRow(BrandMastercard, "BRL")]
        [DataRow(BrandVisa, "CAD")] [DataRow(BrandMastercard, "CAD")]
        [DataRow(BrandVisa, "CHF")] [DataRow(BrandMastercard, "CHF")]
        [DataRow(BrandVisa, "CNY")] [DataRow(BrandMastercard, "CNY")]
        [DataRow(BrandVisa, "DKK")] [DataRow(BrandMastercard, "DKK")]
        [DataRow(BrandVisa, "EGP")] [DataRow(BrandMastercard, "EGP")]
        [DataRow(BrandVisa, "EUR")] [DataRow(BrandMastercard, "EUR")]
        [DataRow(BrandVisa, "GBP")] [DataRow(BrandMastercard, "GBP")]
        [DataRow(BrandVisa, "IDR")] [DataRow(BrandMastercard, "IDR")]
        [DataRow(BrandVisa, "ILS")] [DataRow(BrandMastercard, "ILS")]
        [DataRow(BrandVisa, "INR")] [DataRow(BrandMastercard, "INR")]
        [DataRow(BrandVisa, "LKR")] [DataRow(BrandMastercard, "LKR")]
        [DataRow(BrandVisa, "MUR")] [DataRow(BrandMastercard, "MUR")]
        [DataRow(BrandVisa, "MVR")] [DataRow(BrandMastercard, "MVR")]
        [DataRow(BrandVisa, "MXN")] [DataRow(BrandMastercard, "MXN")]
        [DataRow(BrandVisa, "NOK")] [DataRow(BrandMastercard, "NOK")]
        [DataRow(BrandVisa, "NZD")] [DataRow(BrandMastercard, "NZD")]
        [DataRow(BrandVisa, "PGK")] [DataRow(BrandMastercard, "PGK")]
        [DataRow(BrandVisa, "PKR")] [DataRow(BrandMastercard, "PKR")]
        [DataRow(BrandVisa, "QAR")] [DataRow(BrandMastercard, "QAR")]
        [DataRow(BrandVisa, "RUB")] [DataRow(BrandMastercard, "RUB")]
        [DataRow(BrandVisa, "SAR")] [DataRow(BrandMastercard, "SAR")]
        [DataRow(BrandVisa, "SEK")] [DataRow(BrandMastercard, "SEK")]
        [DataRow(BrandVisa, "THB")] [DataRow(BrandMastercard, "THB")]
        [DataRow(BrandVisa, "TRY")] [DataRow(BrandMastercard, "TRY")]
        [DataRow(BrandVisa, "TWD")] [DataRow(BrandMastercard, "TWD")]
        [DataRow(BrandVisa, "USD")] [DataRow(BrandMastercard, "USD")]
        [DataRow(BrandVisa, "ZAR")] [DataRow(BrandMastercard, "ZAR")]
        // CLP — exponent 2 per 24713 analysis (overrides ISO 4217 exponent 0)
        [DataRow(BrandVisa, "CLP")] [DataRow(BrandMastercard, "CLP")]
        public void CreditAuthReversal_Exponent2(string brand, string currency) {
            var card = NewCard(brand);
            var auth = card.Authorize(Amount).WithCurrency(currency).Execute();
            AssertTransactionResponse(auth, TransactionStatus.Preauthorized);
            var reversal = auth.Reverse(Amount).Execute();
            AssertTransactionResponse(reversal, TransactionStatus.Reversed);
        }

        [DataTestMethod]
        [DataRow(BrandVisa, "BHD")] [DataRow(BrandMastercard, "BHD")]
        [DataRow(BrandVisa, "KWD")] [DataRow(BrandMastercard, "KWD")]
        [DataRow(BrandVisa, "OMR")] [DataRow(BrandMastercard, "OMR")]
        public void CreditAuthReversal_Exponent3(string brand, string currency) {
            var card = NewCard(brand);
            var auth = card.Authorize(Amount).WithCurrency(currency).Execute();
            AssertTransactionResponse(auth, TransactionStatus.Preauthorized);
            var reversal = auth.Reverse(Amount).Execute();
            AssertTransactionResponse(reversal, TransactionStatus.Reversed);
        }

        [DataTestMethod]
        [DataRow(BrandVisa, "ISK")] [DataRow(BrandMastercard, "ISK")]
        [DataRow(BrandVisa, "KRW")] [DataRow(BrandMastercard, "KRW")]
        [DataRow(BrandVisa, "VND")] [DataRow(BrandMastercard, "VND")]
        public void CreditAuthReversal_Exponent0(string brand, string currency) {
            var card = NewCard(brand);
            var auth = card.Authorize(Amount).WithCurrency(currency).Execute();
            AssertTransactionResponse(auth, TransactionStatus.Preauthorized);
            var reversal = auth.Reverse(Amount).Execute();
            AssertTransactionResponse(reversal, TransactionStatus.Reversed);
        }

        #endregion

        #region Refund Tests — Linked

        [DataTestMethod]
        [DataRow(BrandVisa, "SGD")] [DataRow(BrandMastercard, "SGD")]
        [DataRow(BrandVisa, "HKD")] [DataRow(BrandMastercard, "HKD")]
        [DataRow(BrandVisa, "MOP")] [DataRow(BrandMastercard, "MOP")]
        [DataRow(BrandVisa, "PHP")] [DataRow(BrandMastercard, "PHP")]
        [DataRow(BrandVisa, "MYR")] [DataRow(BrandMastercard, "MYR")]
        [DataRow(BrandVisa, "JPY")] [DataRow(BrandMastercard, "JPY")]
        [DataRow(BrandVisa, "AED")] [DataRow(BrandMastercard, "AED")]
        [DataRow(BrandVisa, "AUD")] [DataRow(BrandMastercard, "AUD")]
        [DataRow(BrandVisa, "BDT")] [DataRow(BrandMastercard, "BDT")]
        [DataRow(BrandVisa, "BND")] [DataRow(BrandMastercard, "BND")]
        [DataRow(BrandVisa, "BRL")] [DataRow(BrandMastercard, "BRL")]
        [DataRow(BrandVisa, "CAD")] [DataRow(BrandMastercard, "CAD")]
        [DataRow(BrandVisa, "CHF")] [DataRow(BrandMastercard, "CHF")]
        [DataRow(BrandVisa, "CNY")] [DataRow(BrandMastercard, "CNY")]
        [DataRow(BrandVisa, "DKK")] [DataRow(BrandMastercard, "DKK")]
        [DataRow(BrandVisa, "EGP")] [DataRow(BrandMastercard, "EGP")]
        [DataRow(BrandVisa, "EUR")] [DataRow(BrandMastercard, "EUR")]
        [DataRow(BrandVisa, "GBP")] [DataRow(BrandMastercard, "GBP")]
        [DataRow(BrandVisa, "IDR")] [DataRow(BrandMastercard, "IDR")]
        [DataRow(BrandVisa, "ILS")] [DataRow(BrandMastercard, "ILS")]
        [DataRow(BrandVisa, "INR")] [DataRow(BrandMastercard, "INR")]
        [DataRow(BrandVisa, "LKR")] [DataRow(BrandMastercard, "LKR")]
        [DataRow(BrandVisa, "MUR")] [DataRow(BrandMastercard, "MUR")]
        [DataRow(BrandVisa, "MVR")] [DataRow(BrandMastercard, "MVR")]
        [DataRow(BrandVisa, "MXN")] [DataRow(BrandMastercard, "MXN")]
        [DataRow(BrandVisa, "NOK")] [DataRow(BrandMastercard, "NOK")]
        [DataRow(BrandVisa, "NZD")] [DataRow(BrandMastercard, "NZD")]
        [DataRow(BrandVisa, "PGK")] [DataRow(BrandMastercard, "PGK")]
        [DataRow(BrandVisa, "PKR")] [DataRow(BrandMastercard, "PKR")]
        [DataRow(BrandVisa, "QAR")] [DataRow(BrandMastercard, "QAR")]
        [DataRow(BrandVisa, "RUB")] [DataRow(BrandMastercard, "RUB")]
        [DataRow(BrandVisa, "SAR")] [DataRow(BrandMastercard, "SAR")]
        [DataRow(BrandVisa, "SEK")] [DataRow(BrandMastercard, "SEK")]
        [DataRow(BrandVisa, "THB")] [DataRow(BrandMastercard, "THB")]
        [DataRow(BrandVisa, "TRY")] [DataRow(BrandMastercard, "TRY")]
        [DataRow(BrandVisa, "TWD")] [DataRow(BrandMastercard, "TWD")]
        [DataRow(BrandVisa, "USD")] [DataRow(BrandMastercard, "USD")]
        [DataRow(BrandVisa, "ZAR")] [DataRow(BrandMastercard, "ZAR")]
        // CLP — exponent 2 per 24713 analysis (overrides ISO 4217 exponent 0)
        [DataRow(BrandVisa, "CLP")] [DataRow(BrandMastercard, "CLP")]
        public void CreditRefund_Linked_Exponent2(string brand, string currency) {
            var card = NewCard(brand);
            var sale = card.Charge(Amount).WithCurrency(currency).Execute();
            AssertTransactionResponse(sale, TransactionStatus.Captured);
            var refund = sale.Refund(Amount).WithCurrency(currency).Execute();
            AssertTransactionResponse(refund, TransactionStatus.Captured);
        }

        [DataTestMethod]
        [DataRow(BrandVisa, "BHD")] [DataRow(BrandMastercard, "BHD")]
        [DataRow(BrandVisa, "KWD")] [DataRow(BrandMastercard, "KWD")]
        [DataRow(BrandVisa, "OMR")] [DataRow(BrandMastercard, "OMR")]
        public void CreditRefund_Linked_Exponent3(string brand, string currency) {
            var card = NewCard(brand);
            var sale = card.Charge(Amount).WithCurrency(currency).Execute();
            AssertTransactionResponse(sale, TransactionStatus.Captured);
            var refund = sale.Refund(Amount).WithCurrency(currency).Execute();
            AssertTransactionResponse(refund, TransactionStatus.Captured);
        }

        [DataTestMethod]
        [DataRow(BrandVisa, "ISK")] [DataRow(BrandMastercard, "ISK")]
        [DataRow(BrandVisa, "KRW")] [DataRow(BrandMastercard, "KRW")]
        [DataRow(BrandVisa, "VND")] [DataRow(BrandMastercard, "VND")]
        public void CreditRefund_Linked_Exponent0(string brand, string currency) {
            var card = NewCard(brand);
            var sale = card.Charge(Amount).WithCurrency(currency).Execute();
            AssertTransactionResponse(sale, TransactionStatus.Captured);
            var refund = sale.Refund(Amount).WithCurrency(currency).Execute();
            AssertTransactionResponse(refund, TransactionStatus.Captured);
        }

        #endregion

        #region Refund Tests — Standalone

        [DataTestMethod]
        [DataRow(BrandVisa, "SGD")] [DataRow(BrandMastercard, "SGD")]
        [DataRow(BrandVisa, "HKD")] [DataRow(BrandMastercard, "HKD")]
        [DataRow(BrandVisa, "MOP")] [DataRow(BrandMastercard, "MOP")]
        [DataRow(BrandVisa, "PHP")] [DataRow(BrandMastercard, "PHP")]
        [DataRow(BrandVisa, "MYR")] [DataRow(BrandMastercard, "MYR")]
        [DataRow(BrandVisa, "JPY")] [DataRow(BrandMastercard, "JPY")]
        [DataRow(BrandVisa, "AED")] [DataRow(BrandMastercard, "AED")]
        [DataRow(BrandVisa, "AUD")] [DataRow(BrandMastercard, "AUD")]
        [DataRow(BrandVisa, "BDT")] [DataRow(BrandMastercard, "BDT")]
        [DataRow(BrandVisa, "BND")] [DataRow(BrandMastercard, "BND")]
        [DataRow(BrandVisa, "BRL")] [DataRow(BrandMastercard, "BRL")]
        [DataRow(BrandVisa, "CAD")] [DataRow(BrandMastercard, "CAD")]
        [DataRow(BrandVisa, "CHF")] [DataRow(BrandMastercard, "CHF")]
        [DataRow(BrandVisa, "CNY")] [DataRow(BrandMastercard, "CNY")]
        [DataRow(BrandVisa, "DKK")] [DataRow(BrandMastercard, "DKK")]
        [DataRow(BrandVisa, "EGP")] [DataRow(BrandMastercard, "EGP")]
        [DataRow(BrandVisa, "EUR")] [DataRow(BrandMastercard, "EUR")]
        [DataRow(BrandVisa, "GBP")] [DataRow(BrandMastercard, "GBP")]
        [DataRow(BrandVisa, "IDR")] [DataRow(BrandMastercard, "IDR")]
        [DataRow(BrandVisa, "ILS")] [DataRow(BrandMastercard, "ILS")]
        [DataRow(BrandVisa, "INR")] [DataRow(BrandMastercard, "INR")]
        [DataRow(BrandVisa, "LKR")] [DataRow(BrandMastercard, "LKR")]
        [DataRow(BrandVisa, "MUR")] [DataRow(BrandMastercard, "MUR")]
        [DataRow(BrandVisa, "MVR")] [DataRow(BrandMastercard, "MVR")]
        [DataRow(BrandVisa, "MXN")] [DataRow(BrandMastercard, "MXN")]
        [DataRow(BrandVisa, "NOK")] [DataRow(BrandMastercard, "NOK")]
        [DataRow(BrandVisa, "NZD")] [DataRow(BrandMastercard, "NZD")]
        [DataRow(BrandVisa, "PGK")] [DataRow(BrandMastercard, "PGK")]
        [DataRow(BrandVisa, "PKR")] [DataRow(BrandMastercard, "PKR")]
        [DataRow(BrandVisa, "QAR")] [DataRow(BrandMastercard, "QAR")]
        [DataRow(BrandVisa, "RUB")] [DataRow(BrandMastercard, "RUB")]
        [DataRow(BrandVisa, "SAR")] [DataRow(BrandMastercard, "SAR")]
        [DataRow(BrandVisa, "SEK")] [DataRow(BrandMastercard, "SEK")]
        [DataRow(BrandVisa, "THB")] [DataRow(BrandMastercard, "THB")]
        [DataRow(BrandVisa, "TRY")] [DataRow(BrandMastercard, "TRY")]
        [DataRow(BrandVisa, "TWD")] [DataRow(BrandMastercard, "TWD")]
        [DataRow(BrandVisa, "USD")] [DataRow(BrandMastercard, "USD")]
        [DataRow(BrandVisa, "ZAR")] [DataRow(BrandMastercard, "ZAR")]
        // CLP — exponent 2 per 24713 analysis (overrides ISO 4217 exponent 0)
        [DataRow(BrandVisa, "CLP")] [DataRow(BrandMastercard, "CLP")]
        public void CreditRefund_Standalone_Exponent2(string brand, string currency) {
            var card = NewCard(brand);
            var refund = card.Refund(Amount).WithCurrency(currency).Execute();
            AssertTransactionResponse(refund, TransactionStatus.Captured);
        }

        [DataTestMethod]
        [DataRow(BrandVisa, "BHD")] [DataRow(BrandMastercard, "BHD")]
        [DataRow(BrandVisa, "KWD")] [DataRow(BrandMastercard, "KWD")]
        [DataRow(BrandVisa, "OMR")] [DataRow(BrandMastercard, "OMR")]
        public void CreditRefund_Standalone_Exponent3(string brand, string currency) {
            var card = NewCard(brand);
            var refund = card.Refund(Amount).WithCurrency(currency).Execute();
            AssertTransactionResponse(refund, TransactionStatus.Captured);
        }

        [DataTestMethod]
        [DataRow(BrandVisa, "ISK")] [DataRow(BrandMastercard, "ISK")]
        [DataRow(BrandVisa, "KRW")] [DataRow(BrandMastercard, "KRW")]
        [DataRow(BrandVisa, "VND")] [DataRow(BrandMastercard, "VND")]
        public void CreditRefund_Standalone_Exponent0(string brand, string currency) {
            var card = NewCard(brand);
            var refund = card.Refund(Amount).WithCurrency(currency).Execute();
            AssertTransactionResponse(refund, TransactionStatus.Captured);
        }

        #endregion

        #region Partial Capture Tests (via Virtual Terminal)

        [DataTestMethod]
        [DataRow(BrandVisa, "SGD")] [DataRow(BrandMastercard, "SGD")]
        [DataRow(BrandVisa, "HKD")] [DataRow(BrandMastercard, "HKD")]
        [DataRow(BrandVisa, "MOP")] [DataRow(BrandMastercard, "MOP")]
        [DataRow(BrandVisa, "PHP")] [DataRow(BrandMastercard, "PHP")]
        [DataRow(BrandVisa, "MYR")] [DataRow(BrandMastercard, "MYR")]
        [DataRow(BrandVisa, "JPY")] [DataRow(BrandMastercard, "JPY")]
        [DataRow(BrandVisa, "AED")] [DataRow(BrandMastercard, "AED")]
        [DataRow(BrandVisa, "AUD")] [DataRow(BrandMastercard, "AUD")]
        [DataRow(BrandVisa, "BDT")] [DataRow(BrandMastercard, "BDT")]
        [DataRow(BrandVisa, "BND")] [DataRow(BrandMastercard, "BND")]
        [DataRow(BrandVisa, "BRL")] [DataRow(BrandMastercard, "BRL")]
        [DataRow(BrandVisa, "CAD")] [DataRow(BrandMastercard, "CAD")]
        [DataRow(BrandVisa, "CHF")] [DataRow(BrandMastercard, "CHF")]
        [DataRow(BrandVisa, "CNY")] [DataRow(BrandMastercard, "CNY")]
        [DataRow(BrandVisa, "DKK")] [DataRow(BrandMastercard, "DKK")]
        [DataRow(BrandVisa, "EGP")] [DataRow(BrandMastercard, "EGP")]
        [DataRow(BrandVisa, "EUR")] [DataRow(BrandMastercard, "EUR")]
        [DataRow(BrandVisa, "GBP")] [DataRow(BrandMastercard, "GBP")]
        [DataRow(BrandVisa, "IDR")] [DataRow(BrandMastercard, "IDR")]
        [DataRow(BrandVisa, "ILS")] [DataRow(BrandMastercard, "ILS")]
        [DataRow(BrandVisa, "INR")] [DataRow(BrandMastercard, "INR")]
        [DataRow(BrandVisa, "LKR")] [DataRow(BrandMastercard, "LKR")]
        [DataRow(BrandVisa, "MUR")] [DataRow(BrandMastercard, "MUR")]
        [DataRow(BrandVisa, "MVR")] [DataRow(BrandMastercard, "MVR")]
        [DataRow(BrandVisa, "MXN")] [DataRow(BrandMastercard, "MXN")]
        [DataRow(BrandVisa, "NOK")] [DataRow(BrandMastercard, "NOK")]
        [DataRow(BrandVisa, "NZD")] [DataRow(BrandMastercard, "NZD")]
        [DataRow(BrandVisa, "PGK")] [DataRow(BrandMastercard, "PGK")]
        [DataRow(BrandVisa, "PKR")] [DataRow(BrandMastercard, "PKR")]
        [DataRow(BrandVisa, "QAR")] [DataRow(BrandMastercard, "QAR")]
        [DataRow(BrandVisa, "RUB")] [DataRow(BrandMastercard, "RUB")]
        [DataRow(BrandVisa, "SAR")] [DataRow(BrandMastercard, "SAR")]
        [DataRow(BrandVisa, "SEK")] [DataRow(BrandMastercard, "SEK")]
        [DataRow(BrandVisa, "THB")] [DataRow(BrandMastercard, "THB")]
        [DataRow(BrandVisa, "TRY")] [DataRow(BrandMastercard, "TRY")]
        [DataRow(BrandVisa, "TWD")] [DataRow(BrandMastercard, "TWD")]
        [DataRow(BrandVisa, "USD")] [DataRow(BrandMastercard, "USD")]
        [DataRow(BrandVisa, "ZAR")] [DataRow(BrandMastercard, "ZAR")]
        // CLP — exponent 2 per 24713 analysis (overrides ISO 4217 exponent 0)
        [DataRow(BrandVisa, "CLP")] [DataRow(BrandMastercard, "CLP")]
        public void CreditPartialCapture_Exponent2(string brand, string currency) {
            var card = NewCard(brand);
            var auth = card.Authorize(Amount).WithCurrency(currency).Execute();
            AssertTransactionResponse(auth, TransactionStatus.Preauthorized);
            var capture = auth.Capture(5.00m).WithGratuity(1.00m).Execute();
            AssertTransactionResponse(capture, TransactionStatus.Captured);
        }

        [DataTestMethod]
        [DataRow(BrandVisa, "BHD")] [DataRow(BrandMastercard, "BHD")]
        [DataRow(BrandVisa, "KWD")] [DataRow(BrandMastercard, "KWD")]
        [DataRow(BrandVisa, "OMR")] [DataRow(BrandMastercard, "OMR")]
        public void CreditPartialCapture_Exponent3(string brand, string currency) {
            var card = NewCard(brand);
            var auth = card.Authorize(Amount).WithCurrency(currency).Execute();
            AssertTransactionResponse(auth, TransactionStatus.Preauthorized);
            var capture = auth.Capture(5.00m).WithGratuity(1.00m).Execute();
            AssertTransactionResponse(capture, TransactionStatus.Captured);
        }

        [DataTestMethod]
        [DataRow(BrandVisa, "ISK")] [DataRow(BrandMastercard, "ISK")]
        [DataRow(BrandVisa, "KRW")] [DataRow(BrandMastercard, "KRW")]
        [DataRow(BrandVisa, "VND")] [DataRow(BrandMastercard, "VND")]
        public void CreditPartialCapture_Exponent0(string brand, string currency) {
            var card = NewCard(brand);
            var auth = card.Authorize(Amount).WithCurrency(currency).Execute();
            AssertTransactionResponse(auth, TransactionStatus.Preauthorized);
            var capture = auth.Capture(5.00m).WithGratuity(1.00m).Execute();
            AssertTransactionResponse(capture, TransactionStatus.Captured);
        }

        #endregion

        #region Automatic Partial Authorization Reversal Tests

        [DataTestMethod]
        [DataRow(BrandVisa, "SGD")] [DataRow(BrandMastercard, "SGD")]
        [DataRow(BrandVisa, "HKD")] [DataRow(BrandMastercard, "HKD")]
        [DataRow(BrandVisa, "MOP")] [DataRow(BrandMastercard, "MOP")]
        [DataRow(BrandVisa, "PHP")] [DataRow(BrandMastercard, "PHP")]
        [DataRow(BrandVisa, "MYR")] [DataRow(BrandMastercard, "MYR")]
        [Ignore("APAC sandbox MID does not accept authorization_mode=PARTIAL (MANDATORY_DATA_MISSING/50021). SDK wiring verified. Enable when MID is provisioned for partial approvals.")]
        public void CreditAutoPartialAuthReversal(string brand, string currency) {
            var card = NewCard(brand);
            var sale = card.Charge(Amount).WithCurrency(currency).WithAllowPartialAuth(true).Execute();
            AssertTransactionResponse(sale, TransactionStatus.Captured);
            var reversal = sale.Reverse(Amount).Execute();
            AssertTransactionResponse(reversal, TransactionStatus.Reversed);
        }

        #endregion

        #region MOTO Tests (Manual Key-In)

        [DataTestMethod]
        [DataRow(BrandVisa, "SGD")] [DataRow(BrandMastercard, "SGD")]
        [DataRow(BrandVisa, "HKD")] [DataRow(BrandMastercard, "HKD")]
        [DataRow(BrandVisa, "MOP")] [DataRow(BrandMastercard, "MOP")]
        [DataRow(BrandVisa, "PHP")] [DataRow(BrandMastercard, "PHP")]
        [DataRow(BrandVisa, "MYR")] [DataRow(BrandMastercard, "MYR")]
        [DataRow(BrandVisa, "JPY")] [DataRow(BrandMastercard, "JPY")]
        [DataRow(BrandVisa, "AED")] [DataRow(BrandMastercard, "AED")]
        [DataRow(BrandVisa, "AUD")] [DataRow(BrandMastercard, "AUD")]
        [DataRow(BrandVisa, "BDT")] [DataRow(BrandMastercard, "BDT")]
        [DataRow(BrandVisa, "BND")] [DataRow(BrandMastercard, "BND")]
        [DataRow(BrandVisa, "BRL")] [DataRow(BrandMastercard, "BRL")]
        [DataRow(BrandVisa, "CAD")] [DataRow(BrandMastercard, "CAD")]
        [DataRow(BrandVisa, "CHF")] [DataRow(BrandMastercard, "CHF")]
        [DataRow(BrandVisa, "CNY")] [DataRow(BrandMastercard, "CNY")]
        [DataRow(BrandVisa, "DKK")] [DataRow(BrandMastercard, "DKK")]
        [DataRow(BrandVisa, "EGP")] [DataRow(BrandMastercard, "EGP")]
        [DataRow(BrandVisa, "EUR")] [DataRow(BrandMastercard, "EUR")]
        [DataRow(BrandVisa, "GBP")] [DataRow(BrandMastercard, "GBP")]
        [DataRow(BrandVisa, "IDR")] [DataRow(BrandMastercard, "IDR")]
        [DataRow(BrandVisa, "ILS")] [DataRow(BrandMastercard, "ILS")]
        [DataRow(BrandVisa, "INR")] [DataRow(BrandMastercard, "INR")]
        [DataRow(BrandVisa, "LKR")] [DataRow(BrandMastercard, "LKR")]
        [DataRow(BrandVisa, "MUR")] [DataRow(BrandMastercard, "MUR")]
        [DataRow(BrandVisa, "MVR")] [DataRow(BrandMastercard, "MVR")]
        [DataRow(BrandVisa, "MXN")] [DataRow(BrandMastercard, "MXN")]
        [DataRow(BrandVisa, "NOK")] [DataRow(BrandMastercard, "NOK")]
        [DataRow(BrandVisa, "NZD")] [DataRow(BrandMastercard, "NZD")]
        [DataRow(BrandVisa, "PGK")] [DataRow(BrandMastercard, "PGK")]
        [DataRow(BrandVisa, "PKR")] [DataRow(BrandMastercard, "PKR")]
        [DataRow(BrandVisa, "QAR")] [DataRow(BrandMastercard, "QAR")]
        [DataRow(BrandVisa, "RUB")] [DataRow(BrandMastercard, "RUB")]
        [DataRow(BrandVisa, "SAR")] [DataRow(BrandMastercard, "SAR")]
        [DataRow(BrandVisa, "SEK")] [DataRow(BrandMastercard, "SEK")]
        [DataRow(BrandVisa, "THB")] [DataRow(BrandMastercard, "THB")]
        [DataRow(BrandVisa, "TRY")] [DataRow(BrandMastercard, "TRY")]
        [DataRow(BrandVisa, "TWD")] [DataRow(BrandMastercard, "TWD")]
        [DataRow(BrandVisa, "USD")] [DataRow(BrandMastercard, "USD")]
        [DataRow(BrandVisa, "ZAR")] [DataRow(BrandMastercard, "ZAR")]
        // CLP — exponent 2 per 24713 analysis (overrides ISO 4217 exponent 0)
        [DataRow(BrandVisa, "CLP")] [DataRow(BrandMastercard, "CLP")]
        public void CreditMoto_Sale_Exponent2(string brand, string currency) {
            var card = NewMotoCard(brand);
            var response = card.Charge(Amount).WithCurrency(currency).Execute();
            AssertTransactionResponse(response, TransactionStatus.Captured);
        }

        [DataTestMethod]
        [DataRow(BrandVisa, "BHD")] [DataRow(BrandMastercard, "BHD")]
        [DataRow(BrandVisa, "KWD")] [DataRow(BrandMastercard, "KWD")]
        [DataRow(BrandVisa, "OMR")] [DataRow(BrandMastercard, "OMR")]
        public void CreditMoto_Sale_Exponent3(string brand, string currency) {
            var card = NewMotoCard(brand);
            var response = card.Charge(Amount).WithCurrency(currency).Execute();
            AssertTransactionResponse(response, TransactionStatus.Captured);
        }

        [DataTestMethod]
        [DataRow(BrandVisa, "ISK")] [DataRow(BrandMastercard, "ISK")]
        [DataRow(BrandVisa, "KRW")] [DataRow(BrandMastercard, "KRW")]
        [DataRow(BrandVisa, "VND")] [DataRow(BrandMastercard, "VND")]
        public void CreditMoto_Sale_Exponent0(string brand, string currency) {
            var card = NewMotoCard(brand);
            var response = card.Charge(Amount).WithCurrency(currency).Execute();
            AssertTransactionResponse(response, TransactionStatus.Captured);
        }

        [DataTestMethod]
        [DataRow(BrandVisa, "SGD")] [DataRow(BrandMastercard, "SGD")]
        [DataRow(BrandVisa, "HKD")] [DataRow(BrandMastercard, "HKD")]
        [DataRow(BrandVisa, "MOP")] [DataRow(BrandMastercard, "MOP")]
        [DataRow(BrandVisa, "PHP")] [DataRow(BrandMastercard, "PHP")]
        [DataRow(BrandVisa, "MYR")] [DataRow(BrandMastercard, "MYR")]
        [DataRow(BrandVisa, "JPY")] [DataRow(BrandMastercard, "JPY")]
        [DataRow(BrandVisa, "AED")] [DataRow(BrandMastercard, "AED")]
        [DataRow(BrandVisa, "AUD")] [DataRow(BrandMastercard, "AUD")]
        [DataRow(BrandVisa, "BDT")] [DataRow(BrandMastercard, "BDT")]
        [DataRow(BrandVisa, "BND")] [DataRow(BrandMastercard, "BND")]
        [DataRow(BrandVisa, "BRL")] [DataRow(BrandMastercard, "BRL")]
        [DataRow(BrandVisa, "CAD")] [DataRow(BrandMastercard, "CAD")]
        [DataRow(BrandVisa, "CHF")] [DataRow(BrandMastercard, "CHF")]
        [DataRow(BrandVisa, "CNY")] [DataRow(BrandMastercard, "CNY")]
        [DataRow(BrandVisa, "DKK")] [DataRow(BrandMastercard, "DKK")]
        [DataRow(BrandVisa, "EGP")] [DataRow(BrandMastercard, "EGP")]
        [DataRow(BrandVisa, "EUR")] [DataRow(BrandMastercard, "EUR")]
        [DataRow(BrandVisa, "GBP")] [DataRow(BrandMastercard, "GBP")]
        [DataRow(BrandVisa, "IDR")] [DataRow(BrandMastercard, "IDR")]
        [DataRow(BrandVisa, "ILS")] [DataRow(BrandMastercard, "ILS")]
        [DataRow(BrandVisa, "INR")] [DataRow(BrandMastercard, "INR")]
        [DataRow(BrandVisa, "LKR")] [DataRow(BrandMastercard, "LKR")]
        [DataRow(BrandVisa, "MUR")] [DataRow(BrandMastercard, "MUR")]
        [DataRow(BrandVisa, "MVR")] [DataRow(BrandMastercard, "MVR")]
        [DataRow(BrandVisa, "MXN")] [DataRow(BrandMastercard, "MXN")]
        [DataRow(BrandVisa, "NOK")] [DataRow(BrandMastercard, "NOK")]
        [DataRow(BrandVisa, "NZD")] [DataRow(BrandMastercard, "NZD")]
        [DataRow(BrandVisa, "PGK")] [DataRow(BrandMastercard, "PGK")]
        [DataRow(BrandVisa, "PKR")] [DataRow(BrandMastercard, "PKR")]
        [DataRow(BrandVisa, "QAR")] [DataRow(BrandMastercard, "QAR")]
        [DataRow(BrandVisa, "RUB")] [DataRow(BrandMastercard, "RUB")]
        [DataRow(BrandVisa, "SAR")] [DataRow(BrandMastercard, "SAR")]
        [DataRow(BrandVisa, "SEK")] [DataRow(BrandMastercard, "SEK")]
        [DataRow(BrandVisa, "THB")] [DataRow(BrandMastercard, "THB")]
        [DataRow(BrandVisa, "TRY")] [DataRow(BrandMastercard, "TRY")]
        [DataRow(BrandVisa, "TWD")] [DataRow(BrandMastercard, "TWD")]
        [DataRow(BrandVisa, "USD")] [DataRow(BrandMastercard, "USD")]
        [DataRow(BrandVisa, "ZAR")] [DataRow(BrandMastercard, "ZAR")]
        // CLP — exponent 2 per 24713 analysis (overrides ISO 4217 exponent 0)
        [DataRow(BrandVisa, "CLP")] [DataRow(BrandMastercard, "CLP")]
        public void CreditMoto_Authorization_Exponent2(string brand, string currency) {
            var card = NewMotoCard(brand);
            var response = card.Authorize(Amount).WithCurrency(currency).Execute();
            AssertTransactionResponse(response, TransactionStatus.Preauthorized);
        }

        [DataTestMethod]
        [DataRow(BrandVisa, "BHD")] [DataRow(BrandMastercard, "BHD")]
        [DataRow(BrandVisa, "KWD")] [DataRow(BrandMastercard, "KWD")]
        [DataRow(BrandVisa, "OMR")] [DataRow(BrandMastercard, "OMR")]
        public void CreditMoto_Authorization_Exponent3(string brand, string currency) {
            var card = NewMotoCard(brand);
            var response = card.Authorize(Amount).WithCurrency(currency).Execute();
            AssertTransactionResponse(response, TransactionStatus.Preauthorized);
        }

        [DataTestMethod]
        [DataRow(BrandVisa, "ISK")] [DataRow(BrandMastercard, "ISK")]
        [DataRow(BrandVisa, "KRW")] [DataRow(BrandMastercard, "KRW")]
        [DataRow(BrandVisa, "VND")] [DataRow(BrandMastercard, "VND")]
        public void CreditMoto_Authorization_Exponent0(string brand, string currency) {
            var card = NewMotoCard(brand);
            var response = card.Authorize(Amount).WithCurrency(currency).Execute();
            AssertTransactionResponse(response, TransactionStatus.Preauthorized);
        }

        #endregion

        #region Amount Encoding / Rounding (offline — exponent + rounding verification)

        // -------------------------------------------------------------------------
        // Deterministic verification of the SDK currency-amount encode/decode contract
        // (AH-2189 / Wave 24713 analysis table). These tests are network-free: they
        // assert the exact digit string the SDK sends to GP-API and the exact decimal
        // the SDK reconstructs from a GP-API response, for merchant input 1235.876.
        //
        // Rounding rule (Q3): excess precision is rounded half-away-from-zero to the
        // currency's ISO 4217 exponent — never truncated, never rejected.
        //
        //   Currency  Exp   Merchant typed   SDK -> GP-API   GP-API -> SDK
        //   JPY       2 *    1235.876         "123588"        1235.88     (* GP-API ×100 exception)
        //   KRW       0      1235.876         "1236"          1236
        //   VND       0      1235.876         "1236"          1236
        //   ISK       0      1235.876         "1236"          1236
        //   CLP       2 **   1235.876         "123588"        1235.88     (** exp 2 per 24713, not ISO 0)
        //   USD       2      1235.876         "123588"        1235.88
        //   BHD       3      1235.876         "1235876"       1235.876
        //   KWD       3      1235.876         "1235876"       1235.876
        //   OMR       3      1235.876         "1235876"       1235.876
        // -------------------------------------------------------------------------
        [DataTestMethod]
        [DataRow("JPY", "1235.876", "123588",  "1235.88")]
        [DataRow("KRW", "1235.876", "1236",    "1236")]
        [DataRow("VND", "1235.876", "1236",    "1236")]
        [DataRow("ISK", "1235.876", "1236",    "1236")]
        [DataRow("CLP", "1235.876", "123588",  "1235.88")]
        [DataRow("USD", "1235.876", "123588",  "1235.88")]
        [DataRow("BHD", "1235.876", "1235876", "1235.876")]
        [DataRow("KWD", "1235.876", "1235876", "1235.876")]
        [DataRow("OMR", "1235.876", "1235876", "1235.876")]
        public void AmountEncoding_RoundsAndScalesPerExponent(
                string currency, string merchantInput, string expectedWire, string expectedDecoded) {
            var typed = decimal.Parse(merchantInput, CultureInfo.InvariantCulture);

            // SDK -> GP-API: scaled to minor units with half-away-from-zero rounding.
            var wire = typed.ToNumericCurrencyString(currency);
            Assert.AreEqual(expectedWire, wire,
                $"{currency}: SDK -> GP-API wire string for merchant input {merchantInput}.");

            // GP-API -> SDK: reconstructed major-unit amount.
            var decoded = wire.FromMinorUnits(currency);
            Assert.AreEqual(decimal.Parse(expectedDecoded, CultureInfo.InvariantCulture), decoded,
                $"{currency}: GP-API -> SDK decoded amount for wire string {wire}.");
        }

        // 10.00 baseline — every exponent family must encode the round figure exactly.
        [DataTestMethod]
        [DataRow("JPY", "1000")]
        [DataRow("KRW", "10")]
        [DataRow("VND", "10")]
        [DataRow("ISK", "10")]
        [DataRow("CLP", "1000")]
        [DataRow("USD", "1000")]
        [DataRow("BHD", "10000")]
        [DataRow("KWD", "10000")]
        [DataRow("OMR", "10000")]
        public void AmountEncoding_TenUnits_Baseline(string currency, string expectedWire) {
            Assert.AreEqual(expectedWire, (10.00m).ToNumericCurrencyString(currency),
                $"{currency}: 10.00 baseline encode.");
        }

        #endregion

        #region Fractional Amount Precision — live round-trip (per exponent)

        // -------------------------------------------------------------------------
        // Live end-to-end verification that the SDK encodes the merchant-typed
        // amount to the correct ISO 4217 minor-unit precision AND decodes the
        // GP-API response back to the same major-unit value:
        //   exponent 2 -> 2 decimal places   (e.g. 12.34)
        //   exponent 3 -> 3 decimal places   (e.g. 12.345)
        //   exponent 0 -> whole units        (e.g. 12345)
        // Each row asserts response.AuthorizedAmount equals the typed amount,
        // exercising both GpApiAuthorizationRequestBuilder (encode) and
        // GpApiMapping (decode).
        // -------------------------------------------------------------------------

        [DataTestMethod]
        // exponent 2 — two decimal places
        [DataRow(BrandVisa, "SGD", "12.34")] [DataRow(BrandMastercard, "SGD", "12.34")]
        [DataRow(BrandVisa, "HKD", "12.34")]
        [DataRow(BrandVisa, "USD", "12.34")]
        [DataRow(BrandVisa, "EUR", "12.34")]
        // JPY — exponent 2 exception (GP-API ×100)
        [DataRow(BrandVisa, "JPY", "12.34")]
        // CLP — exponent 2 per 24713 (overrides ISO 0)
        [DataRow(BrandVisa, "CLP", "12.34")]
        // exponent 3 — three decimal places
        [DataRow(BrandVisa, "BHD", "12.345")] [DataRow(BrandMastercard, "BHD", "12.345")]
        [DataRow(BrandVisa, "KWD", "12.345")]
        [DataRow(BrandVisa, "OMR", "12.345")]
        // exponent 0 — whole units
        [DataRow(BrandVisa, "KRW", "12345")] [DataRow(BrandMastercard, "KRW", "12345")]
        [DataRow(BrandVisa, "ISK", "12345")]
        [DataRow(BrandVisa, "VND", "12345")]
        public void CreditSale_FractionalAmount_RoundTripsPerExponent(string brand, string currency, string typedAmount) {
            var amount = decimal.Parse(typedAmount, CultureInfo.InvariantCulture);
            var card = NewCard(brand);
            var response = card.Charge(amount).WithCurrency(currency).Execute();
            AssertTransactionResponse(response, TransactionStatus.Captured);
            Assert.AreEqual(amount, response.AuthorizedAmount,
                $"{currency}: sale round-trip amount mismatch for merchant input {typedAmount}.");
        }

        [DataTestMethod]
        // exponent 2 — two decimal places
        [DataRow(BrandVisa, "SGD", "12.34")] [DataRow(BrandMastercard, "SGD", "12.34")]
        [DataRow(BrandVisa, "USD", "12.34")]
        [DataRow(BrandVisa, "JPY", "12.34")]
        // exponent 3 — three decimal places
        [DataRow(BrandVisa, "BHD", "12.345")] [DataRow(BrandMastercard, "BHD", "12.345")]
        [DataRow(BrandVisa, "KWD", "12.345")]
        [DataRow(BrandVisa, "OMR", "12.345")]
        // exponent 0 — whole units
        [DataRow(BrandVisa, "KRW", "12345")] [DataRow(BrandMastercard, "KRW", "12345")]
        [DataRow(BrandVisa, "ISK", "12345")]
        [DataRow(BrandVisa, "VND", "12345")]
        public void CreditAuthorization_FractionalAmount_RoundTripsPerExponent(string brand, string currency, string typedAmount) {
            var amount = decimal.Parse(typedAmount, CultureInfo.InvariantCulture);
            var card = NewCard(brand);
            var response = card.Authorize(amount).WithCurrency(currency).Execute();
            AssertTransactionResponse(response, TransactionStatus.Preauthorized);
            Assert.AreEqual(amount, response.AuthorizedAmount,
                $"{currency}: authorization round-trip amount mismatch for merchant input {typedAmount}.");
        }

        [DataTestMethod]
        // exponent 2 — two decimal places
        [DataRow(BrandVisa, "SGD", "12.34")]
        [DataRow(BrandVisa, "JPY", "12.34")]
        // exponent 3 — three decimal places
        [DataRow(BrandVisa, "BHD", "12.345")]
        [DataRow(BrandVisa, "KWD", "12.345")]
        // exponent 0 — whole units
        [DataRow(BrandVisa, "KRW", "12345")]
        [DataRow(BrandVisa, "VND", "12345")]
        public void CreditRefund_Linked_FractionalAmount_RoundTripsPerExponent(string brand, string currency, string typedAmount) {
            var amount = decimal.Parse(typedAmount, CultureInfo.InvariantCulture);
            var card = NewCard(brand);
            var sale = card.Charge(amount).WithCurrency(currency).Execute();
            AssertTransactionResponse(sale, TransactionStatus.Captured);
            Assert.AreEqual(amount, sale.AuthorizedAmount,
                $"{currency}: sale round-trip amount mismatch for merchant input {typedAmount}.");
            var refund = sale.Refund(amount).WithCurrency(currency).Execute();
            AssertTransactionResponse(refund, TransactionStatus.Captured);
            Assert.AreEqual(amount, refund.AuthorizedAmount,
                $"{currency}: refund round-trip amount mismatch for merchant input {typedAmount}.");
        }

        #endregion

        #region Excess-Precision Rounding — live round-trip (half-away-from-zero per exponent)

        // -------------------------------------------------------------------------
        // Live verification that a merchant amount carrying MORE fractional digits
        // than the currency's ISO 4217 exponent is rounded half-away-from-zero by
        // the SDK before transmission, and that the GP-API response decodes back to
        // the ROUNDED value (not the originally typed value).
        //
        //   currency  exp  typed      wire (rounded)  decoded back
        //   USD       2    1.235   -> "124"        -> 1.24   (123.5 rounds up)
        //   SGD       2    1.245   -> "125"        -> 1.25   (124.5 rounds up)
        //   JPY       2    1.235   -> "124"        -> 1.24   (×100 exception)
        //   BHD       3    1.2345  -> "1235"       -> 1.235  (1234.5 rounds up)
        //   KWD       3    1.2344  -> "1234"       -> 1.234  (1234.4 rounds down)
        //   KRW       0    12.5    -> "13"         -> 13     (rounds up to whole)
        //   VND       0    12.4    -> "12"         -> 12     (rounds down to whole)
        // -------------------------------------------------------------------------
        // exponent 2 — third decimal is rounded half-away-from-zero to 2 places
        [TestMethod]
        public void CreditSale_ExcessPrecision_Exponent2() {
            var card = NewCard(BrandVisa);
            var response = card.Charge(1.235m).WithCurrency("USD").Execute();
            AssertTransactionResponse(response, TransactionStatus.Captured);
            Assert.AreEqual(1.24m, response.AuthorizedAmount,
                "USD: merchant input 1.235 should encode+decode to rounded 1.24.");
        }

        // exponent 3 — fourth decimal is rounded half-away-from-zero to 3 places
        [TestMethod]
        public void CreditSale_ExcessPrecision_Exponent3() {
            var card = NewCard(BrandVisa);
            var response = card.Charge(1.2345m).WithCurrency("BHD").Execute();
            AssertTransactionResponse(response, TransactionStatus.Captured);
            Assert.AreEqual(1.235m, response.AuthorizedAmount,
                "BHD: merchant input 1.2345 should encode+decode to rounded 1.235.");
        }

        // exponent 0 — fractional part is rounded half-away-from-zero to whole units
        [TestMethod]
        public void CreditSale_ExcessPrecision_Exponent0() {
            var card = NewCard(BrandVisa);
            var response = card.Charge(12.5m).WithCurrency("KRW").Execute();
            AssertTransactionResponse(response, TransactionStatus.Captured);
            Assert.AreEqual(13m, response.AuthorizedAmount,
                "KRW: merchant input 12.5 should encode+decode to rounded 13.");
        }

        [TestMethod]
        public void CreditSale_DefaultExponent_PLN() {
            var card = NewCard(BrandVisa);
            var response = card.Charge(10m)
                .WithCurrency("PLN")
                .Execute(DefaultConfigName);

            Assert.IsNotNull(response);
        }

        #endregion

        #region Helper Methods

        private static CreditCardData NewCard(string brand) {
            return brand == BrandVisa
                ? new CreditCardData {
                    Number = VisaCardNumber,
                    ExpMonth = ExpMonth,
                    ExpYear = ExpYear,
                    Cvn = "123",
                    CardHolderName = "John Smith"
                }
                : new CreditCardData {
                    Number = MastercardCardNumber,
                    ExpMonth = ExpMonth,
                    ExpYear = ExpYear,
                    Cvn = "123",
                    CardHolderName = "Jane Doe"
                };
        }

        private static CreditCardData NewMotoCard(string brand) {
            var card = NewCard(brand);
            card.EntryMethod = ManualEntryMethod.Moto;
            return card;
        }

        private void AssertTransactionResponse(Transaction transaction, TransactionStatus transactionStatus) {
            Assert.IsNotNull(transaction);
            Assert.AreEqual("00", transaction.ResponseCode);
            Assert.AreEqual(GetMapping(transactionStatus), transaction.ResponseMessage);
        }

        private static void AssertRoundTripAmount(Transaction transaction) {
            Assert.AreEqual(Amount, transaction.AuthorizedAmount,
                "Round-trip amount mismatch — SDK currency-exponent encode/decode is asymmetric.");
        }

        #endregion
    }
}
