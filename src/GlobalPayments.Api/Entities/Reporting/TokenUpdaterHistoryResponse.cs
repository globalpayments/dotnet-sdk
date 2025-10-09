using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Entities.Reporting {

    /// <summary>
    /// Represents the response for a token updater history report.
    /// Contains summary counts and a list of token update details for the specified reporting period.
    /// </summary>
    public class TokenUpdaterHistoryResponse {
        /// <summary>
        /// Start date of the reporting period.
        /// </summary>
        public DateTime ReportStartDate { get; set; }

        /// <summary>
        /// End date of the reporting period.
        /// </summary>
        public DateTime ReportEndDate { get; set; }

        /// <summary>
        /// Optional offset for paginated results.
        /// </summary>
        public int? Offset { get; set; }

        /// <summary>
        /// Optional limit for paginated results.
        /// </summary>
        public int? Limit { get; set; }

        /// <summary>
        /// Total number of records matching the query.
        /// </summary>
        public int TotalMatchingRecords { get; set; }

        /// <summary>
        /// Optional count of card numbers updated during the period.
        /// </summary>
        public int? CountUpdateCardNumber { get; set; }

        /// <summary>
        /// Optional count of expiration dates updated during the period.
        /// </summary>
        public int? CountUpdateExpirationDate { get; set; }

        /// <summary>
        /// Optional count of accounts closed during the period.
        /// </summary>
        public int? CountAccountsClosed { get; set; }

        /// <summary>
        /// Optional count of cardholders contacted during the period.
        /// </summary>
        public int? CountContactCardholder { get; set; }

        /// <summary>
        /// List of token update details for the reporting period.
        /// </summary>
        public List<TokenDetails> Results { get; set; }
    }
}
