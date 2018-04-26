namespace GlobalPayments.Api.Entities.OnlineBoarding {
    public class BoardingResponse {
        public int? ApplicationId { get; set; }
        public string Message { get; set; }
        public bool HasSignatureLink { get { return !string.IsNullOrEmpty(SignatureUrl); } }
        public string SignatureUrl { get; set; }
    }
}
