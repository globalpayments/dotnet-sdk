namespace GlobalPayments.Api.Entities {
    public enum TransactionApiStatusCode {
        TooManyRequests = 429,
        Declined = 470,
        PartiallyApproved = 471,
        CallIssuer = 472,
        CreatedValidationMismatch = 473,
        PartiallyApprovedValidationMismatch = 474,
    }
}
