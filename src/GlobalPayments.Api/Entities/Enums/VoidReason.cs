using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Entities {
    public enum VoidReason {
        [Map(Target.Transit, "POST_AUTH_USER_DECLINE")]
        PostAuth_UserDeclined,

        [Map(Target.Transit, "DEVICE_TIMEOUT")]
        DeviceTimeout,

        [Map(Target.Transit, "DEVICE_UNAVAILABLE")]
        DeviceUnavailable,

        [Map(Target.Transit, "PARTIAL_REVERSAL")]
        PartialReversal,

        [Map(Target.Transit, "TORN_TRANSACTIONS")]
        TornTransactions,

        [Map(Target.Transit, "POST_AUTH_CHIP_DECLINE")]
        PostAuth_ChipDecline
    }
}
