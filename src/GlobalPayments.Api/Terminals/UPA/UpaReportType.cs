using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Terminals.UPA {
    /// <summary>
    /// Determines the type of batch report to be printed by the UPA device.
    /// Maps to the <c>reportType</c> parameter in the GetBatchDetails command.
    /// </summary>
    public enum UpaReportType {
        /// <summary>Prints a summary report.</summary>
        [Description("summary")]
        Summary,
        /// <summary>Prints a detailed report.</summary>
        [Description("detail")]
        Detail
    }
}
