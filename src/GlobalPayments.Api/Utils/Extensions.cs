using System;
using System.Text.RegularExpressions;

namespace GlobalPayments.Api.Utils {
    public static class Extensions {
        public static string ToNumericString(this decimal? dec) {
            return Regex.Replace(string.Format("{0:c}", dec), "[^0-9]", "");
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

        public static string ToInitialCase(this Enum value) {
            var initial = value.ToString();
            return initial.Substring(0, 1).ToUpper() + initial.Substring(1).ToLower();
        }
    }
}
