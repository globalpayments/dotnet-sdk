namespace GlobalPayments.Api.Entities {
    public enum PriorAuthenticationMethod {
        FRICTIONLESS_AUTHENTICATION,
        CHALLENGE_OCCURRED,
        AVS_VERIFIED,
        OTHER_ISSUER_METHOD
    }
}
