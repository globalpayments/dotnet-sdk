using GlobalPayments.Api.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Entities
{
    public class BankPaymentResponse {
        public string Id { get; set; }
        public string RedirectUrl { get; set; }
        public string PaymentStatus { get; set; }
        public BankPaymentType? Type { get; set; }
        public string TokenRequestId { get; set; }
        public string SortCode { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public string Iban { get; set; }
        public string RemittanceReferenceValue { get; set; }
        public string RemittanceReferenceType { get; set; }
        public decimal? Amount { get; set; }
        public string Currency { get; set; }
        public string MaskedIbanLast4 { get; set; }
    }
}
