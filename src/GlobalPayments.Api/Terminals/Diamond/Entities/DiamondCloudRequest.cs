namespace GlobalPayments.Api.Terminals.Diamond.Entities {
    internal class DiamondCloudRequest {
        internal const string SALE = "/sale";
        internal const string SALE_RETURN = "/return";
        internal const string AUTH = "/auth";
        internal const string VOID = "/void";
        internal const string CAPTURE_EU = "/authComplete";
        internal const string CAPTURE = "/capture";
        internal const string CANCEL_AUTH = "/authCancel";
        internal const string INCREASE_AUTH = "/authIncreasing";
        internal const string TIP_ADJUST = "/tip";
        internal const string EBT_FOOD = "/ebtFood";
        internal const string EBT_RETURN = "/ebtReturn";
        internal const string EBT_BALANCE = "/ebtBalance";
        internal const string GIFT_REDEEM = "/giftRedeem";
        internal const string GIFT_BALANCE = "/giftBalance";
        internal const string GIFT_RELOAD = "/giftReload";
        internal const string RECONCILIATION = "/reconciliation";
    }
}
