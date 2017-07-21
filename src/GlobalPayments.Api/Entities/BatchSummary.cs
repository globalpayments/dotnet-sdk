namespace GlobalPayments.Api.Entities
{
    /// <summary>
    /// Details a closed batch.
    /// </summary>
    public class BatchSummary
    {
        /// <summary>
        /// The batch's ID.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The batch's transaction count.
        /// </summary>
        public int TransactionCount { get; set; }

        /// <summary>
        /// The batch's total amount to be settled.
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// The batch's sequence number; where applicable.
        /// </summary>
        public string SequenceNumber { get; set; }
    }
}
