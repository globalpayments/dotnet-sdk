namespace GlobalPayments.Api.Entities.TransactionApi.Response {
    public class CardResponse {
        public string MaskedCardNnumber { get; set; }
        public string CardholderName { get; set; }
        public string ExpiryMonth { get; set; }
        public string ExpiryYear { get; set; }
        public string Type { get; set; }
        public string Balance { get; set; }
        public string Token { get; set; }
    }
}
