using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Network.Entities {
    [MapTarget(Target.NWS)]
    public enum DE3_AccountType {
        [Map(Target.NWS, "00")]
        Unspecified,
        [Map(Target.NWS, "08")]
        PinDebitAccount,
        [Map(Target.NWS, "09")]
        FleetAccount,
        [Map(Target.NWS, "10")]
        SavingsAccount,
        [Map(Target.NWS, "20")]
        CheckingAccount,
        [Map(Target.NWS, "30")]
        CreditAccount,
        [Map(Target.NWS, "38")]
        PurchaseAccount,
        [Map(Target.NWS, "39")]
        PrivateLabelAccount,
        [Map(Target.NWS, "40")]
        UniversalAccount,
        [Map(Target.NWS, "50")]
        InvestmentAccount,
        [Map(Target.NWS, "60")]
        CashCardAccount,
        [Map(Target.NWS, "65")]
        CashCard_CashAccount,
        [Map(Target.NWS, "66")]
        CashCard_CreditAccount,
        [Map(Target.NWS, "80")]
        FoodStampsAccount,
        [Map(Target.NWS, "81")]
        CashBenefitAccount,
        [Map(Target.NWS, "90")]
        LoyaltyAccount,
        [Map(Target.NWS, "91")]
        AchAccount,
        [Map(Target.NWS, "97")]
        EWIC
    }
}
