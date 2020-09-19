using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Network.Entities {
    [MapTarget(Target.NWS)]
    public enum DE3_TransactionType {
        [Map(Target.NWS, "00")]
        GoodsAndService,
        [Map(Target.NWS, "01")]
        Cash,
        [Map(Target.NWS, "02")]
        Debit_Adjustment,
        [Map(Target.NWS, "03")]
        CheckGuarantee,
        [Map(Target.NWS, "04")]
        CheckVerification,
        [Map(Target.NWS, "09")]
        GoodsAndServiceWithCashDisbursement,
        [Map(Target.NWS, "10")]
        AccountFunding_NonCashFinancialInstrument,
        [Map(Target.NWS, "11")]
        QuasiCashAndScrip,
        [Map(Target.NWS, "17")]
        CashSale,
        [Map(Target.NWS, "18")]
        Debit_FrequencyBenefit,
        [Map(Target.NWS, "20")]
        Return,
        [Map(Target.NWS, "21")]
        Deposit,
        [Map(Target.NWS, "22")]
        Credit_Adjustment,
        [Map(Target.NWS, "23")]
        CheckDepositGuarantee,
        [Map(Target.NWS, "24")]
        CheckDeposit,
        [Map(Target.NWS, "28")]
        Credit_FrequencyBenefit,
        [Map(Target.NWS, "30")]
        AvailableFundsInquiry,
        [Map(Target.NWS, "31")]
        BalanceInquiry,
        [Map(Target.NWS, "32")]
        LedgerBalanceInquiry,
        [Map(Target.NWS, "33")]
        AddressOrAccountVerification,
        [Map(Target.NWS, "38")]
        MiscInquiryVerification,
        [Map(Target.NWS, "40")]
        CardHolderAccountsTransfer,
        [Map(Target.NWS, "48")]
        TransferBetweenCardholders,
        [Map(Target.NWS, "50")]
        Payment,
        [Map(Target.NWS, "60")]
        LoadValue,
        [Map(Target.NWS, "61")]
        UnloadValue,
        [Map(Target.NWS, "90")]
        Activate,
        [Map(Target.NWS, "91")]
        Application,
        [Map(Target.NWS, "92")]
        CashCard_Adjustment,
        [Map(Target.NWS, "93")]
        Activate_PreValuedCard
    }
}
