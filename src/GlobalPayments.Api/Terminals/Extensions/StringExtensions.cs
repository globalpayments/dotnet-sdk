using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace GlobalPayments.Api.Terminals.Extensions {
    internal static class StringExtensions {
        public static string FormatWith(this string pattern, params object[] values) {
            return string.Format(pattern, values);
        }

        public static string ToNumeric(this string str) {
            return Regex.Replace(str, "[^0-9]", "");
        }

        public static string ToNumericString(this decimal dec) {
            return Regex.Replace(dec.ToString(), "[^0-9]", "");
        }

        public static decimal? ToAmount(this string str) {
            if (string.IsNullOrEmpty(str))
                return null;

            decimal amount = 0m;
            if (decimal.TryParse(str, out amount)) {
                return amount / 100;
            }
            return null;
        }

        public static int? ToInt32(this string str) {
            if (string.IsNullOrEmpty(str))
                return null;

            int rvalue = default(int);
            if (Int32.TryParse(str, out rvalue))
                return rvalue;
            return null;
        }

        public static DateTime? ToDateTime(this string str) {
            if (string.IsNullOrEmpty(str))
                return null;

            DateTime rvalue;
            if (DateTime.TryParseExact(str, "yyyyMMddhhmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out rvalue))
                return rvalue;
            return null;
        }
    }
}
