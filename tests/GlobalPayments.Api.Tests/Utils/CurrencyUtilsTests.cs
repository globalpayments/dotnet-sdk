using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.Utils {
    /// <summary>
    /// Unit tests for ISO 4217 currency-exponent normalization used by GP-API amount encoding.
    /// Validates <see cref="CurrencyUtils.GetExponent(string)"/> and the currency-aware
    /// overloads on <see cref="Extensions"/> for APAC and other markets.
    /// </summary>
    [TestClass]
    public class CurrencyUtilsTests {
        /// <summary>
        /// Two-decimal currencies (USD, EUR, HKD, SGD, etc.) return exponent 2.
        /// JPY is included here because GP-API requires it sent as ×100 on the wire
        /// (ISO 4217 exponent 0, but GP-API exception).
        /// </summary>
        [TestMethod]
        public void GetExponent_TwoDecimalCurrencies_ReturnsTwo() {
            Assert.AreEqual(2, CurrencyUtils.GetExponent("USD"));
            Assert.AreEqual(2, CurrencyUtils.GetExponent("EUR"));
            Assert.AreEqual(2, CurrencyUtils.GetExponent("HKD"));
            Assert.AreEqual(2, CurrencyUtils.GetExponent("SGD"));
            Assert.AreEqual(2, CurrencyUtils.GetExponent("MYR"));
            Assert.AreEqual(2, CurrencyUtils.GetExponent("PHP"));
            Assert.AreEqual(2, CurrencyUtils.GetExponent("MOP"));
            Assert.AreEqual(2, CurrencyUtils.GetExponent("JPY"));
            Assert.AreEqual(2, CurrencyUtils.GetExponent("CLP"));
        }

        /// <summary>
        /// Zero-decimal currencies (KRW, VND, ISK) return exponent 0 on the GP-API wire.
        /// </summary>
        [TestMethod]
        public void GetExponent_ZeroDecimalCurrencies_ReturnsZero() {
            Assert.AreEqual(0, CurrencyUtils.GetExponent("KRW"));
            Assert.AreEqual(0, CurrencyUtils.GetExponent("VND"));
            Assert.AreEqual(0, CurrencyUtils.GetExponent("ISK"));
        }

        /// <summary>
        /// Three-decimal currencies (BHD, KWD, OMR) return exponent 3.
        /// </summary>
        [TestMethod]
        public void GetExponent_ThreeDecimalCurrencies_ReturnsThree() {
            Assert.AreEqual(3, CurrencyUtils.GetExponent("BHD"));
            Assert.AreEqual(3, CurrencyUtils.GetExponent("KWD"));
            Assert.AreEqual(3, CurrencyUtils.GetExponent("OMR"));
        }

        /// <summary>
        /// Currency lookup is case-insensitive.
        /// </summary>
        [TestMethod]
        public void GetExponent_CaseInsensitive() {
            Assert.AreEqual(2, CurrencyUtils.GetExponent("jpy"));
            Assert.AreEqual(2, CurrencyUtils.GetExponent("Usd"));
            Assert.AreEqual(3, CurrencyUtils.GetExponent("bhd"));
        }

        /// <summary>
        /// Null or unknown currency codes fall back to the default exponent of 2.
        /// </summary>
        [TestMethod]
        public void GetExponent_NullOrUnknown_ReturnsDefault() {
            Assert.AreEqual(2, CurrencyUtils.GetExponent(null));
            Assert.AreEqual(2, CurrencyUtils.GetExponent(""));
            Assert.AreEqual(2, CurrencyUtils.GetExponent("ZZZ"));
        }

        /// <summary>
        /// USD amount encodes as cents (×100) for the GP-API wire format.
        /// </summary>
        [TestMethod]
        public void ToNumericCurrencyString_USD_EncodesAsCents() {
            Assert.AreEqual("1234", 12.34m.ToNumericCurrencyString("USD"));
            Assert.AreEqual("100", 1m.ToNumericCurrencyString("USD"));
            Assert.AreEqual("50", 0.50m.ToNumericCurrencyString("USD"));
        }

        /// <summary>
        /// JPY amount encodes as ×100 for the GP-API wire (exception to ISO 4217 exponent 0).
        /// </summary>
        [TestMethod]
        public void ToNumericCurrencyString_JPY_EncodesAsHundredths() {
            Assert.AreEqual("123400", 1234m.ToNumericCurrencyString("JPY"));
            Assert.AreEqual("10000", 100m.ToNumericCurrencyString("JPY"));
            Assert.AreEqual("1000", 10m.ToNumericCurrencyString("JPY"));
        }

        /// <summary>
        /// KRW (true exponent 0) encodes as whole units — no scaling.
        /// </summary>
        [TestMethod]
        public void ToNumericCurrencyString_KRW_EncodesAsWholeUnits() {
            Assert.AreEqual("1234", 1234m.ToNumericCurrencyString("KRW"));
            Assert.AreEqual("100", 100m.ToNumericCurrencyString("KRW"));
        }

        /// <summary>
        /// BHD amount encodes as thousandths (×1000).
        /// </summary>
        [TestMethod]
        public void ToNumericCurrencyString_BHD_EncodesAsThousandths() {
            Assert.AreEqual("1234", 1.234m.ToNumericCurrencyString("BHD"));
            Assert.AreEqual("100", 0.100m.ToNumericCurrencyString("BHD"));
        }

        /// <summary>
        /// Excess fractional digits are rounded silently (half-away-from-zero) per IO guidance,
        /// matching legacy SDK behavior. No BuilderException is thrown.
        /// </summary>
        [TestMethod]
        public void ToNumericCurrencyString_ExcessPrecision_RoundsSilently() {
            Assert.AreEqual("123588", 1235.876m.ToNumericCurrencyString("USD"));
            Assert.AreEqual("1236", 1235.876m.ToNumericCurrencyString("KRW"));
            Assert.AreEqual("123588", 1235.876m.ToNumericCurrencyString("JPY"));
            Assert.AreEqual("1235876", 1235.876m.ToNumericCurrencyString("BHD"));
        }

        /// <summary>
        /// Nullable decimal overload returns null when input is null.
        /// </summary>
        [TestMethod]
        public void ToNumericCurrencyString_NullableNull_ReturnsNull() {
            decimal? amount = null;
            Assert.IsNull(amount.ToNumericCurrencyString("USD"));
        }

        /// <summary>
        /// FromMinorUnits decodes USD cents into major-unit decimals.
        /// </summary>
        [TestMethod]
        public void FromMinorUnits_USD_DecodesFromCents() {
            Assert.AreEqual(12.34m, "1234".FromMinorUnits("USD"));
            Assert.AreEqual(1.00m, "100".FromMinorUnits("USD"));
        }

        /// <summary>
        /// FromMinorUnits returns whole units for true zero-decimal currencies (KRW, VND, ISK).
        /// </summary>
        [TestMethod]
        public void FromMinorUnits_KRW_ReturnsWholeUnits() {
            Assert.AreEqual(1234m, "1234".FromMinorUnits("KRW"));
        }

        /// <summary>
        /// FromMinorUnits decodes JPY as ÷100 — the GP-API wire exception requires it,
        /// even though ISO 4217 exponent for JPY is 0.
        /// </summary>
        [TestMethod]
        public void FromMinorUnits_JPY_DecodesAsHundredths() {
            Assert.AreEqual(12.34m, "1234".FromMinorUnits("JPY"));
            Assert.AreEqual(10m, "1000".FromMinorUnits("JPY"));
        }

        /// <summary>
        /// FromMinorUnits returns thousandths for three-decimal currencies.
        /// </summary>
        [TestMethod]
        public void FromMinorUnits_BHD_ReturnsThousandths() {
            Assert.AreEqual(1.234m, "1234".FromMinorUnits("BHD"));
        }

        /// <summary>
        /// FromMinorUnits returns null on null, empty or unparseable input.
        /// </summary>
        [TestMethod]
        public void FromMinorUnits_NullOrEmpty_ReturnsNull() {
            Assert.IsNull(((string)null).FromMinorUnits("USD"));
            Assert.IsNull("".FromMinorUnits("USD"));
            Assert.IsNull("not-a-number".FromMinorUnits("USD"));
        }

        /// <summary>
        /// Round-trip encoding and decoding through GP-API minor-units format preserves the amount.
        /// </summary>
        [TestMethod]
        public void RoundTrip_USD_PreservesAmount() {
            var encoded = 19.99m.ToNumericCurrencyString("USD");
            Assert.AreEqual(19.99m, encoded.FromMinorUnits("USD"));
        }

        /// <summary>
        /// Round-trip encoding and decoding for JPY preserves the major-unit amount
        /// (encoded ×100 and decoded ÷100 per the GP-API wire exception).
        /// </summary>
        [TestMethod]
        public void RoundTrip_JPY_PreservesAmount() {
            var encoded = 5000m.ToNumericCurrencyString("JPY");
            Assert.AreEqual("500000", encoded);
            Assert.AreEqual(5000m, encoded.FromMinorUnits("JPY"));
        }
    }
}
