using System;

namespace GlobalPayments.Api.Entities {
    public class DepositSummary {
        public string MerchantHierarchy { get; set; }
        public string MerchantName { get; set; }
        public string MerchantDbaName { get; set; }
        public string MerchantNumber { get; set; }
        public string MerchantCategory { get; set; }
        public DateTime? DepositDate { get; set; }
        public string Reference { get; set; }
        public decimal? Amount { get; set; }
        public string Currency { get; set; }
        public string Type { get; set; }
        public string RoutingNumber { get; set; }
        public string AccountNumber { get; set; }
        public string Mode { get; set; }
        public string SummaryModel { get; set; }
        public int SalesTotalCount { get; set; }
        public decimal? SalesTotalAmount { get; set; }
        public string SalesTotalCurrency { get; set; }
        public int RefundsTotalCount { get; set; }
        public decimal? RefundsTotalAmount { get; set; }
        public string RefundsTotalCurrency { get; set; }
        public int ChargebackTotalCount { get; set; }
        public decimal? ChargebackTotalAmount { get; set; }
        public string ChargebackTotalCurrency { get; set; }
        public int RepresentmentTotalCount { get; set; }
        public decimal? RepresentmentTotalAmount { get; set; }
        public string RepresentmentTotalCurrency { get; set; }
        public decimal? FeesTotalAmount { get; set; }
        public string FeesTotalCurrency { get; set; }
        public int AdjustmentTotalCount { get; set; }
        public decimal? AdjustmentTotalAmount { get; set; }
        public string AdjustmentTotalCurrency { get; set; }
    }
}
