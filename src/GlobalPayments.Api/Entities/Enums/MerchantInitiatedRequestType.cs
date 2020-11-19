namespace GlobalPayments.Api.Entities {
    public enum MerchantInitiatedRequestType {
        RECURRING_TRANSACTION,
        INSTALLMENT_TRANSACTION,
        ADD_CARD,
        MAINTAIN_CARD_INFORMATION,
        ACCOUNT_VERIFICATION,
        SPLIT_OR_DELAYED_SHIPMENT,
        TOP_UP,
        MAIL_ORDER,
        TELEPHONE_ORDER,
        WHITELIST_STATUS_CHECK,
        OTHER_PAYMENT,
        BILLING_AGREEMENT
    }
}
