using GlobalPayments.Api.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.GpApi {

    /// <summary>
    /// Offline unit tests for the ISO 4217 multi-currency amount encoding/decoding
    /// infrastructure used by GP-API (Phase 1 — basic transactions).
    ///
    /// Phase 1 transaction types require correct minor-unit scaling for:
    ///   Sale, Auth, Pre-Auth, Capture, Void, Auth Reversal, Refund (linked + standalone),
    ///   Partial Capture (VT), MOTO.
    ///
    /// Three exponent families:
    ///   Exponent 0 — KRW, ISK, VND      (whole units; no decimal places on the wire)
    ///   Exponent 2 — USD, GBP, EUR, CLP, ...  (standard two-decimal; ×100 on the wire)
    ///   Exponent 3 — BHD, KWD, OMR  (milli-unit; ×1000 on the wire)
    ///
    /// JPY exception: ISO 4217 lists JPY as exponent 0, but GP-API requires ×100 on the
    /// wire (treating it as exponent 2) for both encode and decode.
    ///
    /// These tests are network-free and do not require credentials.
    /// </summary>
    [TestClass]
    public class GpApiCurrencyAmountEncodingTests {

        #region CurrencyUtils.GetExponent

        [TestMethod]
        public void GetExponent_Usd_Returns2() {
            Assert.AreEqual(2, CurrencyUtils.GetExponent("USD"));
        }

        [TestMethod]
        public void GetExponent_Gbp_Returns2() {
            Assert.AreEqual(2, CurrencyUtils.GetExponent("GBP"));
        }

        [TestMethod]
        public void GetExponent_Eur_Returns2() {
            Assert.AreEqual(2, CurrencyUtils.GetExponent("EUR"));
        }

        [TestMethod]
        public void GetExponent_Krw_Returns0() {
            Assert.AreEqual(0, CurrencyUtils.GetExponent("KRW"));
        }

        [TestMethod]
        public void GetExponent_Isk_Returns0() {
            Assert.AreEqual(0, CurrencyUtils.GetExponent("ISK"));
        }

        [TestMethod]
        public void GetExponent_Clp_Returns2() {
            Assert.AreEqual(2, CurrencyUtils.GetExponent("CLP"));
        }

        [TestMethod]
        public void GetExponent_Vnd_Returns0() {
            Assert.AreEqual(0, CurrencyUtils.GetExponent("VND"));
        }

        [TestMethod]
        public void GetExponent_Bhd_Returns3() {
            Assert.AreEqual(3, CurrencyUtils.GetExponent("BHD"));
        }

        [TestMethod]
        public void GetExponent_Kwd_Returns3() {
            Assert.AreEqual(3, CurrencyUtils.GetExponent("KWD"));
        }

        [TestMethod]
        public void GetExponent_Omr_Returns3() {
            Assert.AreEqual(3, CurrencyUtils.GetExponent("OMR"));
        }

        /// <summary>
        /// JPY is ISO 4217 exponent 0 but GP-API mandates ×100 on the wire — must return 2.
        /// </summary>
        [TestMethod]
        public void GetExponent_Jpy_Returns2_NotZero() {
            Assert.AreEqual(2, CurrencyUtils.GetExponent("JPY"),
                "JPY must be treated as exponent 2 on the GP-API wire despite ISO 4217 listing it as exponent 0.");
        }

        [TestMethod]
        public void GetExponent_NullCurrency_ReturnsDefault2() {
            Assert.AreEqual(CurrencyUtils.DefaultExponent, CurrencyUtils.GetExponent(null));
        }

        [TestMethod]
        public void GetExponent_EmptyCurrency_ReturnsDefault2() {
            Assert.AreEqual(CurrencyUtils.DefaultExponent, CurrencyUtils.GetExponent(""));
        }

        [TestMethod]
        public void GetExponent_UnknownCurrency_ReturnsDefault2() {
            Assert.AreEqual(CurrencyUtils.DefaultExponent, CurrencyUtils.GetExponent("XYZ"));
        }

        [TestMethod]
        public void GetExponent_LowerCaseCode_ReturnsCorrectExponent() {
            Assert.AreEqual(0, CurrencyUtils.GetExponent("krw"));
            Assert.AreEqual(3, CurrencyUtils.GetExponent("bhd"));
            Assert.AreEqual(2, CurrencyUtils.GetExponent("usd"));
        }

        #endregion

        #region ToNumericCurrencyString — Encoding

        // --- Exponent 2 (standard) ---

        [TestMethod]
        public void Encode_Usd_Standard_CorrectMinorUnits() {
            Assert.AreEqual("1000", (10.00m).ToNumericCurrencyString("USD"));
        }

        [TestMethod]
        public void Encode_Usd_FractionalCents_RoundsAwayFromZero() {
            // 1235.876 → ×100 = 123587.6 → rounds to 123588
            Assert.AreEqual("123588", (1235.876m).ToNumericCurrencyString("USD"));
        }

        [TestMethod]
        public void Encode_Usd_ExactCents_NoRounding() {
            Assert.AreEqual("123588", (1235.88m).ToNumericCurrencyString("USD"));
        }

        [TestMethod]
        public void Encode_Gbp_Standard_CorrectMinorUnits() {
            Assert.AreEqual("780", (7.80m).ToNumericCurrencyString("GBP"));
        }

        // --- Exponent 0 (KRW, ISK, VND) ---

        [TestMethod]
        public void Encode_Krw_WholeUnit_SentAsIs() {
            Assert.AreEqual("1236", (1236m).ToNumericCurrencyString("KRW"));
        }

        [TestMethod]
        public void Encode_Krw_FractionalInput_RoundsToWholeUnit() {
            // 1235.876 → ×1 = 1235.876 → rounds to 1236
            Assert.AreEqual("1236", (1235.876m).ToNumericCurrencyString("KRW"));
        }

        [TestMethod]
        public void Encode_Isk_WholeUnit_SentAsIs() {
            Assert.AreEqual("5000", (5000m).ToNumericCurrencyString("ISK"));
        }

        [TestMethod]
        public void Encode_Vnd_FractionalInput_RoundsToWholeUnit() {
            Assert.AreEqual("25000", (25000.4m).ToNumericCurrencyString("VND"));
        }

        // --- Exponent 3 (BHD, KWD, OMR) ---

        [TestMethod]
        public void Encode_Bhd_MilliUnit_CorrectScaling() {
            // 10.500 → ×1000 = 10500
            Assert.AreEqual("10500", (10.500m).ToNumericCurrencyString("BHD"));
        }

        [TestMethod]
        public void Encode_Kwd_FractionalInput_RoundsToMilliUnit() {
            // 1235.876 → ×1000 = 1235876
            Assert.AreEqual("1235876", (1235.876m).ToNumericCurrencyString("KWD"));
        }

        [TestMethod]
        public void Encode_Omr_ThreeDecimalPlaces_ExactMilliUnit() {
            Assert.AreEqual("1001", (1.001m).ToNumericCurrencyString("OMR"));
        }

        // --- JPY exception ---

        [TestMethod]
        public void Encode_Jpy_TreatedAsExponent2_NotZero() {
            // JPY ISO exponent 0 — but GP-API wire requires ×100
            // ¥10 → "1000", not "10"
            Assert.AreEqual("1000", (10m).ToNumericCurrencyString("JPY"),
                "JPY must be encoded as ×100 (exponent 2) on the GP-API wire.");
        }

        [TestMethod]
        public void Encode_Jpy_FractionalInput_RoundsCorrectly() {
            // 1235.876 → ×100 = 123587.6 → rounds to 123588
            Assert.AreEqual("123588", (1235.876m).ToNumericCurrencyString("JPY"));
        }

        // --- Null / currency-less ---

        [TestMethod]
        public void Encode_NullCurrency_FallsBackToExponent2() {
            Assert.AreEqual("1000", (10.00m).ToNumericCurrencyString(null));
        }

        [TestMethod]
        public void Encode_NullableDecimal_WithCurrency_ReturnsNull_WhenNull() {
            decimal? value = null;
            Assert.IsNull(value.ToNumericCurrencyString("USD"));
        }

        [TestMethod]
        public void Encode_NullableDecimal_WithCurrency_ReturnsEncoded_WhenNotNull() {
            decimal? value = 10.00m;
            Assert.AreEqual("1000", value.ToNumericCurrencyString("USD"));
        }

        #endregion

        #region FromMinorUnits — Decoding

        // --- Exponent 2 ---

        [TestMethod]
        public void Decode_Usd_MinorUnitsString_ReturnsCorrectMajorUnits() {
            Assert.AreEqual(10.00m, "1000".FromMinorUnits("USD"));
        }

        [TestMethod]
        public void Decode_Gbp_MinorUnitsString_ReturnsCorrectMajorUnits() {
            Assert.AreEqual(7.80m, "780".FromMinorUnits("GBP"));
        }

        [TestMethod]
        public void Decode_Usd_RoundTrip_Preserves2DecimalAmount() {
            const decimal original = 1235.88m;
            var encoded = original.ToNumericCurrencyString("USD");
            var decoded = encoded.FromMinorUnits("USD");
            Assert.AreEqual(original, decoded);
        }

        // --- Exponent 0 ---

        [TestMethod]
        public void Decode_Krw_MinorUnitsString_ReturnsSameWholeUnit() {
            Assert.AreEqual(1236m, "1236".FromMinorUnits("KRW"));
        }

        [TestMethod]
        public void Decode_Isk_RoundTrip_PreservesWholeUnit() {
            const decimal original = 5000m;
            var encoded = original.ToNumericCurrencyString("ISK");
            var decoded = encoded.FromMinorUnits("ISK");
            Assert.AreEqual(original, decoded);
        }

        // --- Exponent 3 ---

        [TestMethod]
        public void Decode_Bhd_MilliUnitString_ReturnsCorrectMajorUnits() {
            Assert.AreEqual(10.500m, "10500".FromMinorUnits("BHD"));
        }

        [TestMethod]
        public void Decode_Kwd_RoundTrip_Preserves3DecimalAmount() {
            const decimal original = 1235.876m;
            var encoded = original.ToNumericCurrencyString("KWD");
            var decoded = encoded.FromMinorUnits("KWD");
            Assert.AreEqual(original, decoded);
        }

        // --- JPY exception ---

        [TestMethod]
        public void Decode_Jpy_TreatedAsExponent2_DividesBy100() {
            // Gateway returns "1000" for ¥10 — must decode to 10m, not 1000m
            Assert.AreEqual(10m, "1000".FromMinorUnits("JPY"),
                "JPY must be decoded as ÷100 (exponent 2) from the GP-API wire response.");
        }

        [TestMethod]
        public void Decode_Jpy_RoundTrip_Preserves2DecimalAmount() {
            const decimal original = 1235.88m;
            var encoded = original.ToNumericCurrencyString("JPY");
            var decoded = encoded.FromMinorUnits("JPY");
            Assert.AreEqual(original, decoded,
                "JPY round-trip must preserve the 2-decimal major-unit value.");
        }

        // --- Null / empty / unparseable ---

        [TestMethod]
        public void Decode_NullString_ReturnsNull() {
            Assert.IsNull(((string)null).FromMinorUnits("USD"));
        }

        [TestMethod]
        public void Decode_EmptyString_ReturnsNull() {
            Assert.IsNull("".FromMinorUnits("USD"));
        }

        [TestMethod]
        public void Decode_UnparseableString_ReturnsNull() {
            Assert.IsNull("abc".FromMinorUnits("USD"));
        }

        [TestMethod]
        public void Decode_NullCurrency_FallsBackToExponent2() {
            Assert.AreEqual(10.00m, "1000".FromMinorUnits(null));
        }

        #endregion

        #region Symmetry — Encode then Decode for all Phase 1 currencies

        /// <summary>
        /// Verifies encode→decode round-trips are perfectly symmetric for a representative
        /// amount across every currency used by Phase 1 basic transactions.
        /// </summary>
        [DataTestMethod]
        // Exponent 2 — APAC settlement currencies
        [DataRow("SGD", 10.00)]
        [DataRow("HKD", 10.00)]
        [DataRow("MOP", 10.00)]
        [DataRow("PHP", 10.00)]
        [DataRow("MYR", 10.00)]
        // Exponent 2 — standard global currencies
        [DataRow("USD", 10.00)]
        [DataRow("EUR", 10.00)]
        [DataRow("GBP", 10.00)]
        [DataRow("AUD", 10.00)]
        [DataRow("CAD", 10.00)]
        // CLP — exponent 2 per 24713 analysis (overrides ISO 4217 exponent 0)
        [DataRow("CLP", 5000.00)]
        // Exponent 0 — zero-decimal currencies
        [DataRow("KRW", 10000.0)]
        [DataRow("ISK", 1000.0)]
        [DataRow("VND", 25000.0)]
        // Exponent 3 — milli-unit currencies
        [DataRow("BHD", 10.000)]
        [DataRow("KWD", 10.000)]
        [DataRow("OMR", 10.000)]
        // JPY exception
        [DataRow("JPY", 1000.0)]
        public void Encode_Decode_RoundTrip_IsSymmetric(string currency, double amountDouble) {
            var amount = (decimal)amountDouble;
            var encoded = amount.ToNumericCurrencyString(currency);
            var decoded = encoded.FromMinorUnits(currency);
            Assert.AreEqual(amount, decoded,
                $"Round-trip mismatch for {currency}: {amount} → \"{encoded}\" → {decoded}");
        }

        #endregion
    }
}
