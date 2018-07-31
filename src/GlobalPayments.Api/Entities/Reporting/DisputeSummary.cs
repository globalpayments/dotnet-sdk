using System;

namespace GlobalPayments.Api.Entities {
    public class DisputeSummary {
        public string MerchantHierarchy { get; set; }
        public string MerchantName { get; set; }
        public string MerchantDbaName { get; set; }
        public string MerchantNumber { get; set; }
        public string MerchantCategory { get; set; }
        public DateTime? DepositDate { get; set; }
        public string DepositReference { get; set; }
        public string DepositType { get; set; }
        public string Type { get; set; }
        public decimal? CaseAmount { get; set; }
        public string CaseCurrency { get; set; }
        public string CaseStatus { get; set; }
        public string CaseDescription { get; set; }
        public string TransactionOrderId { get; set; }
        public DateTime? TransactionLocalTime { get; set; }
        public DateTime? TransactionTime { get; set; }
        public string TransactionType { get; set; }
        public decimal? TransactionAmount { get; set; }
        public string TransactionCurrency { get; set; }
        public string CaseNumber { get; set; }
        public DateTime? CaseTime { get; set; }
        public string CaseId { get; set; }
        public DateTime? CaseIdTime { get; set; }
        public string CaseMerchantId { get; set; }
        public string CaseTerminalId { get; set; }
        public string TransactionARN { get; set; }
        public string TransactionReferenceNumber { get; set; }
        public string TransactionSRD { get; set; }
        public string TransactionAuthCode { get; set; }
        public string TransactionCardType { get; set; }
        public string TransactionMaskedCardNumber { get; set; }
        public string Reason { get; set; }
        public string IssuerComment { get; set; }
        public string IssuerCaseNumber { get; set; }
        public decimal? DisputeAmount { get; set; }
        public string DisputeCurrency { get; set; }
        public decimal? DisputeCustomerAmount { get; set; }
        public string DisputeCustomerCurrency { get; set; }
        public DateTime? RespondByDate { get; set; }
        public string CaseOriginalReference { get; set; }
    }
}
