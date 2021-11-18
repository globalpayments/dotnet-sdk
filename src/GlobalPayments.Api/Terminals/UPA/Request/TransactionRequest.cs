namespace GlobalPayments.Api.Terminals.UPA
{
    internal class TransactionRequest
    {
        public string BaseAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TipAmount { get; set; }
        public int TaxIndicator { get; set; }
        public decimal CashbackAmount { get; set; }
        public int InvoiceNbr { get; set; }
        public int TranNo { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
