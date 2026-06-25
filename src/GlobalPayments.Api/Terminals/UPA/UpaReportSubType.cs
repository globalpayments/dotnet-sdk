using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Terminals.UPA {
    /// <summary>
    /// Determines the filter applied when printing UPA batch reports.
    /// Maps to the <c>reportSubType</c> parameter in the GetBatchDetails command.
    /// </summary>
    public enum UpaReportSubType {
        /// <summary>Filter reports by reference number. Sends <c>"1"</c> to the device.</summary>
        [Description("1")]
        ByReference = 1,
        /// <summary>Filter reports by clerk. Sends <c>"2"</c> to the device.</summary>
        [Description("2")]
        ByClerk = 2,
        /// <summary>Filter reports by all clerks. Sends <c>"3"</c> to the device.</summary>
        [Description("3")]
        ByAllClerks = 3
    }
}
