using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Network.Entities {
    [MapTarget(Target.NWS)]
    public enum DE123_TransactionType {
        [Map(Target.NWS, "000")]
        Debits,
        [Map(Target.NWS, "001")]
        DebitReversals,
        [Map(Target.NWS, "002")]
        DebitLessReversals,
        [Map(Target.NWS, "005")]
        Credits,
        [Map(Target.NWS, "006")]
        CreditReversals,
        [Map(Target.NWS, "007")]
        CreditLessReversals,
        [Map(Target.NWS, "010")]
        Inquiry,
        [Map(Target.NWS, "100")]
        GoodsAndService,
        [Map(Target.NWS, "101")]
        Cash,
        [Map(Target.NWS, "103")]
        CheckGuarantee,
        [Map(Target.NWS, "104")]
        CheckVerification,
        [Map(Target.NWS, "109")]
        GoodsAndService_CashDisbursement,
        [Map(Target.NWS, "114")]
        ElectronicBenefitsTransfer_Debit,
        [Map(Target.NWS, "120")]
        Return,
        [Map(Target.NWS, "121")]
        Deposit,
        [Map(Target.NWS, "122")]
        Adjustment,
        [Map(Target.NWS, "127")]
        ElectronicBenefitsTransfer_Credit,
        [Map(Target.NWS, "130")]
        AvailableFundsInquiry,
        [Map(Target.NWS, "131")]
        BalanceInquiry,
        [Map(Target.NWS, "132")]
        LedgerBalanceInquiry,
        [Map(Target.NWS, "133")]
        AddressVerification,
        [Map(Target.NWS, "136")]
        ElectronicBenefitsTransfer_Inquiry,
        [Map(Target.NWS, "150")]
        Payment,
        [Map(Target.NWS, "160")]
        LoadValue,
        [Map(Target.NWS, "161")]
        UnloadValue,
        [Map(Target.NWS, "190")]
        Activate,
        //[Map(Target.NWS, "192")]
        //Adjustment,
        [Map(Target.NWS, "298")]
        AllVoids_Reversals,
        [Map(Target.NWS, "299")]
        AllVoids_Voids

    }
}
