using GlobalPayments.Api.Network.Entities;
using System;

namespace GlobalPayments.Api.Network.Elements {
    public class DE63_ProductDataEntry {
        public ProductCodeSet? CodeSet { get; set; }
        public string Code { get; set; }
        public UnitOfMeasure? UnitOfMeasure { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Amount { get; set; }
        public string CouponStatus { get; set; }
        public string CouponMarkdownType { get; set; }
        public decimal? CouponValue { get; set; }
        public ProductCodeSet? CouponProductSetCode { get; set; }
        public string CouponCode { get; set; }
        public string CouponExtendedCode { get; set; }
    }
}
