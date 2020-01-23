using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Entities {
    public enum EmvLastChipRead {
        [Map(Target.Transit, "SUCCESSFUL")]
        [Map(Target.Portico, "CHIP_FAILED_PREV_SUCCESS")]
        Successful,

        [Map(Target.Transit, "FAILED")]
        [Map(Target.Portico, "CHIP_FAILED_PREV_FAILED")]
        Failed,

        [Map(Target.Transit, "NOT_A_CHIP_TRANSACTION")]
        NotAChipTransaction,

        [Map(Target.Transit, "UNKNOWN")]
        Unknown
    }
}
