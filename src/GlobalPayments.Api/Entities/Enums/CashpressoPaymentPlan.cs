namespace GlobalPayments.Api.Entities {
    /// <summary>
    /// Payment plans supported by the Cashpresso (BNPL) alternative payment method.
    /// </summary>
    public enum CashpressoPaymentPlan {
        PAY_IN_3_INSTALLMENTS,
        PAY_30_DAYS,
        FLEXIBLE
    }
}
