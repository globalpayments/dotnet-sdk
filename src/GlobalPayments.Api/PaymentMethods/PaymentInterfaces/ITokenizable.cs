namespace GlobalPayments.Api.PaymentMethods {
    interface ITokenizable {
        string Token { get; set; }
        string Tokenize(string configName = "default", string idempotencyKey = null);
        string Tokenize(bool validateCard, string configName = "default", string idempotencyKey = null);
        bool UpdateTokenExpiry(string configName = "default", string idempotencyKey = null);
        bool DeleteToken(string configName = "default", string idempotencyKey = null);
        ITokenizable Detokenize(string configName = "default", string idempotencyKey = null);
    }
}
