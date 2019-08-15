namespace GlobalPayments.Api.Entities {
    public enum CustomerAuthenticationMethod {
        NOT_AUTHENTICATED,
        MERCHANT_SYSTEM_AUTHENTICATION,
        FEDERATED_ID_AUTHENTICATION,
        ISSUER_CREDENTIAL_AUTHENTICATION,
        THIRD_PARTY_AUTHENTICATION,
        FIDO_AUTHENTICATION
    }
}
