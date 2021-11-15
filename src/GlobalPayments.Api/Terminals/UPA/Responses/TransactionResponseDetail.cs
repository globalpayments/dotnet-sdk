namespace GlobalPayments.Api.Terminals.UPA
{
    public class TransactionResponseDetail
    {
        public decimal TipAmount { get; set; }
        public decimal Surcharge { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
