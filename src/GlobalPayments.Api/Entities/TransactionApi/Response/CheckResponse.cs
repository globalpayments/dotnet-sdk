namespace GlobalPayments.Api.Entities.TransactionApi.Response {
    public class CheckResponse {
        public string MaskedCardNnumber { get; set; }
        public string RoutingNumber { get; set; }
        public string CheckNumber { get; set; }
        public string Token { get; set; }
    }
}
