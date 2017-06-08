namespace GlobalPayments.Api.Entities
{
    public class BatchSummary
    {
        public int Id { get; set; }
        public int TransactionCount { get; set; }
        public decimal TotalAmount { get; set; }
        public string SequenceNumber { get; set; }
    }
}
