namespace GlobalPayments.Api.Terminals.Diamond.Entities.Enums {
    public enum DiamondCloudTransactionType {
        UNKNOWN = 0,
        SALE = 1,
        PREAUTH = 4,
        CAPTURE = 5,
        REFUND = 6,
        VOID = 10,
        REPORT = 66,
        PREAUTH_CANCEL = 82,
        INCR_AUTH = 86
    }
}
