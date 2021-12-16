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
        public bool? Gift { get; set; }
        public string UnitCurrency { get; set; }
        public string Type { get; set; }
        public string Risk { get; set; }
        public decimal? TaxAmount { get; set; }
    }
}