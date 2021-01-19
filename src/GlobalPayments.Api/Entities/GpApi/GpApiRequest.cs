namespace GlobalPayments.Api.Entities {
    internal class GpApiRequest {
        public string Endpoint { get; set; }
        public string RequestBody { get; set; }
        public string ResultsField { get; set; }
    }
}
