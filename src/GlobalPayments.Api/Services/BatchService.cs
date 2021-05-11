using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;

namespace GlobalPayments.Api.Services {
    public class BatchService {
        public static BatchSummary CloseBatch() {
            var response = new ManagementBuilder(TransactionType.BatchClose).Execute();
            return response.BatchSummary;
        }
        public static BatchSummary CloseBatch(BatchCloseType closeType) {
            return CloseBatch(closeType, "default");
        }
        public static BatchSummary CloseBatch(BatchCloseType closeType, string configName) {
            Transaction response = new ManagementBuilder(TransactionType.BatchClose)
                    .WithBatchCloseType(closeType)
                    .Execute(configName);
            return response.BatchSummary;
        }

        public static BatchSummary CloseBatch(int batchNumber) {
            return CloseBatch(batchNumber, "default");
        }
        public static BatchSummary CloseBatch(int batchNumber, string configName) {
            Transaction response = new ManagementBuilder(TransactionType.BatchClose)
                        .WithBatchNumber(batchNumber)
                        .Execute(configName);
            return response.BatchSummary;
        }

        public static BatchSummary CloseBatch(int batchNumber, int sequenceNumber) {
            return CloseBatch(batchNumber, sequenceNumber, "default");
        }
        public static BatchSummary CloseBatch(int batchNumber, int sequenceNumber, string configName) {
            Transaction response = new ManagementBuilder(TransactionType.BatchClose)
                        .WithBatchNumber(batchNumber, sequenceNumber)
                        .Execute(configName);
            return response.BatchSummary;
        }

        public static BatchSummary CloseBatch(BatchCloseType closeType, int batchNumber) {
            return CloseBatch(closeType, batchNumber, "default");
        }
        public static BatchSummary CloseBatch(BatchCloseType closeType, int batchNumber, string configName) {
            Transaction response = new ManagementBuilder(TransactionType.BatchClose)
                        .WithBatchNumber(batchNumber)
                        .WithBatchCloseType(closeType)
                        .Execute(configName);
            return response.BatchSummary;
        }

        public static BatchSummary CloseBatch(BatchCloseType closeType, int batchNumber, int sequenceNumber) {
            return CloseBatch(closeType, batchNumber, sequenceNumber, "default");
        }
        public static BatchSummary CloseBatch(BatchCloseType closeType, int batchNumber, int sequenceNumber, string configName) {
            Transaction response = new ManagementBuilder(TransactionType.BatchClose)
                        .WithBatchNumber(batchNumber, sequenceNumber)
                        .WithBatchCloseType(closeType)
                        .Execute(configName);
            return response.BatchSummary;
        }

        public static BatchSummary CloseBatch(int transactionTotal, decimal totalCredits, decimal totalDebits) {
            return CloseBatch(transactionTotal, totalCredits, totalDebits, "default");
        }
        public static BatchSummary CloseBatch(int transactionTotal, decimal totalCredits, decimal totalDebits, string configName) {
            Transaction response = new ManagementBuilder(TransactionType.BatchClose)
                        .WithBatchTotals(transactionTotal, totalDebits, totalCredits)
                        .Execute(configName);
            return response.BatchSummary;
        }

        public static BatchSummary CloseBatch(BatchCloseType closeType, int transactionTotal, decimal totalCredits, decimal totalDebits) {
            return CloseBatch(closeType, transactionTotal, totalCredits, totalDebits, "default");
        }
        public static BatchSummary CloseBatch(BatchCloseType closeType, int transactionTotal, decimal totalCredits, decimal totalDebits, string configName) {
            Transaction response = new ManagementBuilder(TransactionType.BatchClose)
                        .WithBatchTotals(transactionTotal, totalDebits, totalCredits)
                        .WithBatchCloseType(closeType)
                        .Execute(configName);
            return response.BatchSummary;
        }

        public static BatchSummary CloseBatch(int batchNumber, int transactionTotal, decimal totalCredits, decimal totalDebits) {
            return CloseBatch(batchNumber, transactionTotal, totalCredits, totalDebits, "default");
        }
        public static BatchSummary CloseBatch(int batchNumber, int transactionTotal, decimal totalCredits, decimal totalDebits, string configName) {
            Transaction response = new ManagementBuilder(TransactionType.BatchClose)
                        .WithBatchNumber(batchNumber)
                        .WithBatchTotals(transactionTotal, totalDebits, totalCredits)
                        .Execute(configName);
            return response.BatchSummary;
        }

        public static BatchSummary CloseBatch(int batchNumber, int sequenceNumber, int transactionTotal, decimal totalCredits, decimal totalDebits) {
            return CloseBatch(batchNumber, sequenceNumber, transactionTotal, totalCredits, totalDebits, "default");
        }
        public static BatchSummary CloseBatch(int batchNumber, int sequenceNumber, int transactionTotal, decimal totalCredits, decimal totalDebits, string configName) {
            Transaction response = new ManagementBuilder(TransactionType.BatchClose)
                        .WithBatchNumber(batchNumber, sequenceNumber)
                        .WithBatchTotals(transactionTotal, totalDebits, totalCredits)
                        .Execute(configName);
            return response.BatchSummary;
        }

        public static BatchSummary CloseBatch(BatchCloseType closeType, int batchNumber, int transactionTotal, decimal totalCredits, decimal totalDebits) {
            return CloseBatch(closeType, batchNumber, transactionTotal, totalCredits, totalDebits, "default");
        }
        public static BatchSummary CloseBatch(BatchCloseType closeType, int batchNumber, int transactionTotal, decimal totalCredits, decimal totalDebits, string configName) {
            Transaction response = new ManagementBuilder(TransactionType.BatchClose)
                        .WithBatchNumber(batchNumber)
                        .WithBatchTotals(transactionTotal, totalDebits, totalCredits)
                        .WithBatchCloseType(closeType)
                        .Execute(configName);
            return response.BatchSummary;
        }

        public static BatchSummary CloseBatch(BatchCloseType closeType, int batchNumber, int sequenceNumber, int transactionTotal, decimal totalCredits, decimal totalDebits) {
            return CloseBatch(closeType, batchNumber, sequenceNumber, transactionTotal, totalCredits, totalDebits, "default");
        }
        public static BatchSummary CloseBatch(BatchCloseType closeType, int batchNumber, int sequenceNumber, int transactionTotal, decimal totalCredits, decimal totalDebits, string configName) {
            Transaction response = new ManagementBuilder(TransactionType.BatchClose)
                        .WithBatchNumber(batchNumber, sequenceNumber)
                        .WithBatchTotals(transactionTotal, totalDebits, totalCredits)
                        .WithBatchCloseType(closeType)
                        .Execute(configName);
            return response.BatchSummary;
        }

        public static BatchSummary CloseBatch(string batchReference) {
            return CloseBatch(batchReference, "default");
        }
        public static BatchSummary CloseBatch(string batchReference, string configName) {
            Transaction response = new ManagementBuilder(TransactionType.BatchClose)
                .WithBatchReference(batchReference)
                .Execute(configName);
            return response.BatchSummary;
        }
    }
}
