namespace GlobalPayments.Api.Network.Entities {
    public class TransactionMatchingData {
        public string OriginalBatchNumber { get; set; }
        public string OriginalDate { get; set; }
        
        public string GetElementData() {
            if (OriginalBatchNumber != null && OriginalDate != null) {
                return OriginalBatchNumber + OriginalDate;
            }
            return null;
        }

        public TransactionMatchingData(string batchNumber, string date) {
            OriginalBatchNumber = batchNumber;
            OriginalDate = date;
        }
    }
}
