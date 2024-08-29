using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Terminals.Enums {
    public enum HostDecision {
        [Description("Approved")]
        APPROVED,
        [Description("Declined")]
        DECLINED,
        [Description("Failed to Connect")]
        FAILED_TO_CONNECT,
        [Description("Stand IN")]
        STAND_IN
    }
}
