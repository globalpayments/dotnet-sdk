using GlobalPayments.Api.Utils;
using System;
using System.Text;

namespace GlobalPayments.Api.Entities {
    public class Product
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public int? Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? NetUnitPrice { get; set; }
        public bool? Gift { get; set; }
        public string UnitCurrency { get; set; }
        public string Type { get; set; }
        public string Risk { get; set; }
        public decimal? TaxAmount { get; set; }
        public decimal? TaxPercentage { get; set; }
        public decimal? DiscountAmount { get; set; }
        public string Url { get; set; }
        public string ImageUrl { get; set; }
        public decimal? NetUnitAmount { get; set; }
        public string GiftCardCurrency { get; set; }
    }
}