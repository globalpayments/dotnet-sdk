using GlobalPayments.Api.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Utils {
    public static class ResponseNormalizer {
        private const string Approved = "00";
        private const string PartialApproval = "10";

        private static readonly Dictionary<string, string> _map =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "0", Approved },
            { "85", Approved },
            { "A0000", Approved },
            { "A0014", Approved },
            { "A3200", Approved },

            { "A0002", PartialApproval },
            { "A3207", PartialApproval }
        };

        /// <summary>
        /// Evaluate the string with the original response code received.
        /// Returns the normalized object.
        /// </summary>
        /// <param name="origRespCode"></param>
        /// <returns>NormalizeResponse object</returns>
        public static NormalizeResponse Evaluate(string origRespCode) {
            var normalized = Normalize(origRespCode);

            var isPartial = normalized == PartialApproval;
            var isApproved = normalized == Approved || isPartial;

            return new NormalizeResponse(
                originalResponseCode: origRespCode,
                normalizedResponseCode: normalized,
                isApproved: isApproved,
                isPartial: isPartial
            );
        }

        private static string Normalize(string input) {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            return _map.TryGetValue(input, out var normalized)
                ? normalized
                : input;
        }
    }
}
