namespace GlobalPayments.Api.Entities.TransactionApi.Request {
    public class TransactionData {
        public CountryCode? CountryCode { get; set; }
		public EcomIndicator? EcommerceIndicator { get; set; }
		public LanguageEnum? Language { get; set; }
		public string SoftDescriptor { get; set; }
		public bool AddressVerificationService { get; set; }
		public bool CreateToken { get; set; }
		public bool GenerateReceipt { get; set; }
		public bool PartialApproval { get; set; }
        public string EntryClass { get; set; }
        public string PaymentPurposeCode { get; set; }
    }
}
