using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Entities {
    public class NormalizeResponse {
        public string OriginalResponseCode { get; }
        public string NormalizedResponseCode { get; }
        public bool IsApproved { get; }
        public bool IsPartial { get; }

        public NormalizeResponse(string originalResponseCode,
            string normalizedResponseCode,
            bool isApproved, bool isPartial) {
            OriginalResponseCode = originalResponseCode;
            NormalizedResponseCode = normalizedResponseCode;
            IsApproved = isApproved;
            IsPartial = isPartial;
        }
    }
}
