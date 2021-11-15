namespace GlobalPayments.Api.Terminals.UPA
{
    public class SafRecordResponse
    {
        public decimal SafTotal { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal AuthorizedAmount { get; set; }
        public decimal TransId { get; set; }
        public int TransactionTime { get; set; }
        public int TransactionType { get; set; }
        public string MaskedPan { get; set; }
        public string CardType { get; set; }
        public string CardAcquisition { get; set; }
        public string ApprovalCode { get; set; }
        public int ResponseCode { get; set; }
        public string ResponseText { get; set; }
        public int HostTimeOut { get; set; }
        public decimal BaseAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TipAmount { get; set; }
        public decimal RequestAmount { get; set; }
    }
}
