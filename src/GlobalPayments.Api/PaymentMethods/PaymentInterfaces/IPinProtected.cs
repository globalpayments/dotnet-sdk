namespace GlobalPayments.Api.PaymentMethods {
    interface IPinProtected {
        string PinBlock { get; set; }
    }
}
