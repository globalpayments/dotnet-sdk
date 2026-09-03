namespace GlobalPayments.Api.Entities {
    /// <summary>
    /// Shipping methods supported by the Cashpresso (BNPL) alternative payment method.
    /// </summary>
    public enum CashpressoShippingMethod {
        DELIVERY,
        PICKUP,
        PICKUP_BOX,
        POSTOFFICE
    }
}
