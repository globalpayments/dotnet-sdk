using System;
using System.Collections.Generic;

namespace GlobalPayments.Api.Utils {
    /// <summary>
    /// ISO 4217 currency-exponent (minor-unit) lookup for amount normalization.
    /// </summary>
    public static class CurrencyUtils {
        /// <summary>
        /// Default ISO 4217 minor-unit exponent applied when a currency code is not in the table.
        /// </summary>
        public const int DefaultExponent = 2;

        /// <summary>
        /// Currency-code → minor-unit exponent. Keys are upper-case ISO 4217 alphabetic codes.
        /// </summary>
        private static readonly Dictionary<string, int> ExponentDict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase) {
            // Exponent 0 — no minor unit on the GP-API wire.
            { "ISK", 0 },
            { "KRW", 0 },
            { "VND", 0 },

            // Exponent 3 — milli-unit (e.g., fils, dirhams)
            { "BHD", 3 },
            { "KWD", 3 },
            { "OMR", 3 },

            // Exponent 2 — standard two-decimal currencies.
            // JPY is ISO 4217 exponent 0, but GP-API requires it sent and received
            // as ×100 (e.g. ¥10 → "1000"); treat as exponent 2 for wire encoding.
            { "JPY", 2 },
            { "AED", 2 },
            { "AUD", 2 },
            { "BDT", 2 },
            { "BND", 2 },
            { "BRL", 2 },
            { "CAD", 2 },
            { "CHF", 2 },
            // CLP is ISO 4217 exponent 0, but the 24713 analysis defines it as exponent 2 on the GP-API wire.
            { "CLP", 2 },
            { "CNY", 2 },
            { "DKK", 2 },
            { "EGP", 2 },
            { "EUR", 2 },
            { "GBP", 2 },
            { "HKD", 2 },
            { "IDR", 2 },
            { "ILS", 2 },
            { "INR", 2 },
            { "LKR", 2 },
            { "MOP", 2 },
            { "MUR", 2 },
            { "MVR", 2 },
            { "MXN", 2 },
            { "MYR", 2 },
            { "NOK", 2 },
            { "NZD", 2 },
            { "PGK", 2 },
            { "PHP", 2 },
            { "PKR", 2 },
            { "QAR", 2 },
            { "RUB", 2 },
            { "SAR", 2 },
            { "SEK", 2 },
            { "SGD", 2 },
            { "THB", 2 },
            { "TRY", 2 },
            { "TWD", 2 },
            { "USD", 2 },
            { "VEF", 2 },
            { "ZAR", 2 }
        };

        /// <summary>
        /// Returns the ISO 4217 minor-unit exponent for the given currency code,
        /// or <see cref="DefaultExponent"/> when the code is null/empty or unknown.
        /// </summary>
        /// <param name="currency">ISO 4217 alphabetic currency code (e.g., "USD", "JPY", "BHD").</param>
        /// <returns>0, 2, or 3 depending on the currency.</returns>
        public static int GetExponent(string currency) {
            if (string.IsNullOrEmpty(currency)) {
                return DefaultExponent;
            }
            int exponent;
            if (ExponentDict.TryGetValue(currency, out exponent)) {
                return exponent;
            }
            return DefaultExponent;
        }

        /// <summary>
        /// Indicates whether the given currency code is in the supported APAC multi-currency table.
        /// </summary>
        /// <param name="currency">ISO 4217 alphabetic currency code.</param>
        /// <returns><c>true</c> when the currency is known; otherwise <c>false</c>.</returns>
        internal static bool IsSupported(string currency) {
            return !string.IsNullOrEmpty(currency) && ExponentDict.ContainsKey(currency);
        }
    }
}
