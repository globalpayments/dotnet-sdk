using GlobalPayments.Api.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Entities.Reporting
{
    public class PayByLinkSummary
    {
        public string MerchantId { get; set; }
        
        public string MerchantName { get; set; }
        
        public string AccountId { get; set; }
        
        public string AccountName { get; set; }
        
        public string Id { get; set; }
       
        public string Url { get; set; }
        
        public PayByLinkStatus? Status { get; set; }
       
        public PayByLinkType? Type { get; set; }
        
        public List<PaymentMethodName> AllowedPaymentMethods { get; set; }
        
        public PaymentMethodUsageMode? UsageMode { get; set; }
        
        public string UsageCount { get; set; }

        public string UsageLimit { get; set; }

        public string Reference { get; set; }
        
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        public string Shippable { get; set; }

        public string ShippingAmount { get; set; }
        
        public string ViewedCount { get; set; }
        
        public DateTime? ExpirationDate { get; set; }
        
        public string[] Images { get; set; }

        public decimal? Amount { get; set; }

        public string Currency { get; set; }
       
        public List<TransactionSummary> Transactions { get; set; }
    }
}
