using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Network.Entities {
    [MapTarget(Target.NWS)]
    public enum DE123_TotalType {
        [Map(Target.NWS, "   ")]
        NotSpecific,
        [Map(Target.NWS, "001")]
        AccountLedgerBalance,
        [Map(Target.NWS, "002")]
        AccountAvailableBalance,
        [Map(Target.NWS, "003")]
        AmountOwing,
        [Map(Target.NWS, "004")]
        AmountDue,
        [Map(Target.NWS, "005")]
        AccountAvailableCredit,
        [Map(Target.NWS, "040")]
        AmountCash,
        [Map(Target.NWS, "041")]
        AmountGoodsAndServices,
        [Map(Target.NWS, "056")]
        AmountTax,
        [Map(Target.NWS, "057")]
        AmountDiscount,
        [Map(Target.NWS, "058")]
        AmountFundsForDeposit,
        [Map(Target.NWS, "100")]
        TransactionFee,
        [Map(Target.NWS, "200")]
        PointBalance,
        [Map(Target.NWS, "201")]
        AwardBalance
    }
}
