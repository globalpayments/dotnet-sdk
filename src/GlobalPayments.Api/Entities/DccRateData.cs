using GlobalPayments.Api.Entities;
using System;

namespace GlobalPayments.Api.Entities {
    public class DccRateData {
        public decimal? CardHolderAmount { get; set; }
        public string CardHolderCurrency { get; set; }
        public decimal? CardHolderRate { get; set; }
        public string CommissionPercentage { get; set; }
        public DccProcessor DccProcessor { get; set; }
        public DccRateType DccRateType { get; set; }
        public string OrderId { get; set; }
        public string DccId { get; set; }
        public string ExchangeRateSourceName { get; set; }
        public DateTime? ExchangeRateSourceTimestamp { get; set; }
        public decimal? MerchantAmount { get; set; }
        public string MerchantCurrency { get; set; }
        public string MarginRatePercentage { get; set; }
    }
}
