namespace GlobalPayments.Api.Entities {
    public class DebitMac {
        public string TransactionCode { get; set; }
        public string TransmissionNumber { get; set; }
        public string BankResponseCode { get; set; }
        public string MacKey { get; set; }
        public string PinKey { get; set; }
        public string FieldKey { get; set; }
        public string TraceNumber { get; set; }
        public string MessageAuthenticationCode { get; set; }
    }
}
