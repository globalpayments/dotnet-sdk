namespace GlobalPayments.Api.Entities.TransactionApi.Response  {
    public class PaymentResponse {
        public string Amount { get; set; }
        public string CurrencyCode { get; set; }
        public string InvoiceNumber { get; set; }
        public string Type { get; set; }
        public string GratuityAmount { get; set; }
    }
}
