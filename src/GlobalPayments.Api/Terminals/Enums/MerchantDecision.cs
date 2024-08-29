using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Terminals.Enums {
    public enum MerchantDecision {
        [Description("Approved")]
        APPROVED,
        [Description("Force Online")]
        FORCE_ONLINE,
        [Description("Force Decline - AAC")]
        FORCE_DECLINE
    }
}
