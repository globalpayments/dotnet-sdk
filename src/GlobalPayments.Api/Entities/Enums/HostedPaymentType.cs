namespace GlobalPayments.Api.Entities.Enums {
    /// <summary>
    /// Specifies the Bill Pay Hosted Payment Type
    /// </summary>
    public enum HostedPaymentType {
        None,
        MakePayment,
        MakePaymentReturnToken,
        GetToken,
        MyAccount
    }
}
