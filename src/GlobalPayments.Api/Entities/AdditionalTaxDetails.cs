namespace GlobalPayments.Api.Entities {
    public class AdditionalTaxDetails {
        public decimal? TaxAmount { get; set; }
        public TaxCategory? TaxCategory { get; set; }
        public decimal? TaxRate { get; set; }
        public string TaxType { get; set; }
    }
}
