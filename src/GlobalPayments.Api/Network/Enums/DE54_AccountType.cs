using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Network.Entities {
    [MapTarget(Target.NWS)]
    public enum DE54_AccountType {
        [Map(Target.NWS, "00")]
        Unspecified,
        [Map(Target.NWS, "01")]
        AccountLedgerBalance,
        [Map(Target.NWS, "02")]
        AccountAvailableBalance,
        [Map(Target.NWS, "03")]
        AmountOwing,
        [Map(Target.NWS, "04")]
        AmountDue,
        [Map(Target.NWS, "05")]
        AccountAvailableCredit,
        [Map(Target.NWS, "17")]
        AccountMaximumLimit,
        [Map(Target.NWS, "20")]
        AmountRemainingThisCycle
    }
}
