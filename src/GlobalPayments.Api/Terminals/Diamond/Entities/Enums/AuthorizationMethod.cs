namespace GlobalPayments.Api.Terminals.Diamond.Entities.Enums {
    public enum AuthorizationMethod {
        PIN = 'A',
        SIGNATURE = '@',
        PIN_AND_SIGNATURE = 'B',
        NO_AUTH_METHOD = '?'
    }
}

