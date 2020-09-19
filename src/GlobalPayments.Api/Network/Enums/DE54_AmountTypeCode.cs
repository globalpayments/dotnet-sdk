using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Network.Entities {
    [MapTarget(Target.NWS)]
    public enum DE54_AmountTypeCode {
        [Map(Target.NWS, "02")]
        AccountAvailableBalance,
        [Map(Target.NWS, "40")]
        AmountCash,
        [Map(Target.NWS, "41")]
        AmountGoodsAndServices,
        [Map(Target.NWS, "52")]
        WIC_GenericDiscountAmount,
        [Map(Target.NWS, "56")]
        AmountTax,
        [Map(Target.NWS, "57")]
        AmountDiscount,
        [Map(Target.NWS, "58")]
        AmountFundsForDeposit,
        [Map(Target.NWS, "59")]
        AmountInvoice,
        [Map(Target.NWS, "60")]
        WIC_CommissaryFee,
        [Map(Target.NWS, "61")]
        WIC_VolumeDiscount,
        [Map(Target.NWS, "62")]
        WIC_VendorLoyaltyDiscount,
        [Map(Target.NWS, "63")]
        Coupon,
        [Map(Target.NWS, "90")]
        AmountGratuity,
        [Map(Target.NWS, "91")]
        RetailAmount,
        [Map(Target.NWS, "92")]
        WholesaleAmount,
        [Map(Target.NWS, "93")]
        LastPaymentAmount,
        [Map(Target.NWS, "94")]
        ShadowLimit,
        [Map(Target.NWS, "95")]
        MiscTax_1,
        [Map(Target.NWS, "96")]
        MiscTax_2
    }
}
