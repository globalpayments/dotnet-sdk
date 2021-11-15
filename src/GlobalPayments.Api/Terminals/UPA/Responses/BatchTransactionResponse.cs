namespace GlobalPayments.Api.Terminals.UPA
{
    public class BatchTransactionResponse
    {
        public string CardType { get; set; }
        public decimal TotalAmount { get; set; }
        public int TotalCnt { get; set; }
        public int CreditCnt { get; set; }
        public decimal CreditAmt { get; set; }
        public int DebitCnt { get; set; }
        public decimal DebitAmt { get; set; }
        public int SaleCnt { get; set; }
        public decimal SaleAmt { get; set; }
        public int ReturnCnt { get; set; }
        public decimal ReturnAmt { get; set; }
        public decimal TotalGratuityAmt { get; set; }
    }
}
