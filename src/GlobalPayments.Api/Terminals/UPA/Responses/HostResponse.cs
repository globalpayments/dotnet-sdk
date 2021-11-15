namespace GlobalPayments.Api.Terminals.UPA
{
    public class HostResponse
    {
        public long ResponseId { get; set; }
        public int TranNo { get; set; }
        public string RespDateTime { get; set; }
        public int GatewayResponseCode { get; set; }
        public string GatewayResponsemessage { get; set; }
        public int ResponseCode { get; set; }
        public string ResponseText { get; set; }
        public string ApprovalCode { get; set; }
        public long ReferenceNumber { get; set; }
        public string AvsResultCode { get; set; }
        public string CvvResultCode { get; set; }
        public string AvsResultText { get; set; }
        public string CvvResultText { get; set; }
        public decimal AdditionalTipAmount { get; set; }
        public decimal BaseAmount { get; set; }
        public decimal? TipAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal? CashBackAmount { get; set; }
        public decimal AuthorizedAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Surcharge { get; set; }
        public int TokenRspCode { get; set; }
        public int TokenRspMsg { get; set; }
        public string TokenValue { get; set; }
        public string TxnDescriptor { get; set; }
        public int RecurringDataCode { get; set; }
        public int CavvResultCode { get; set; }
        public int TokenPANLast { get; set; }
        public int PartialApproval { get; set; }
        public int TraceNumber { get; set; }
        public decimal BalanceDue { get; set; }
        public decimal BaseDue { get; set; }
        public decimal TaxDue { get; set; }
        public decimal TipDue { get; set; }
        public decimal AvailableBalance { get; set; }
        public string EmvIssuerResp { get; set; }
    }
}
