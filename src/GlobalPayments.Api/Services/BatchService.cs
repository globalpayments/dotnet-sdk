using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;

namespace GlobalPayments.Api.Services {
    public class BatchService {
        public static BatchSummary CloseBatch() {
            var response = new ManagementBuilder(TransactionType.BatchClose).Execute();
            return response.BatchSummary;
        }
    }
}
