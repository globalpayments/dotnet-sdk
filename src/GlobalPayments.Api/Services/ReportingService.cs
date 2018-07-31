using System.Collections.Generic;
using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;

namespace GlobalPayments.Api.Services {
    public class ReportingService {
        public static TransactionReportBuilder<List<TransactionSummary>> FindTransactions(string transactionId = null) {
            return new TransactionReportBuilder<List<TransactionSummary>>(ReportType.FindTransactions)
                .WithTransactionId(transactionId);
        }

        public static TransactionReportBuilder<List<DepositSummary>> FindDeposits() {
            return new TransactionReportBuilder<List<DepositSummary>>(ReportType.FindDepoits);
        }

        public static TransactionReportBuilder<List<DisputeSummary>> FindDisputes() {
            return new TransactionReportBuilder<List<DisputeSummary>>(ReportType.FindDisputes);
        }

        public static TransactionReportBuilder<List<TransactionSummary>> Activity() {
            return new TransactionReportBuilder<List<TransactionSummary>>(ReportType.Activity);
        }

        //public static BatchReportBuilder BatchDetail() {
        //    return new BatchReportBuilder(ReportType.BatchDetail);
        //}

        //public static BatchReportBuilder BatchHistory() {
        //    return new BatchReportBuilder(ReportType.BatchHistory);
        //}

        //public static BatchReportBuilder BatchSummary() {
        //    return new BatchReportBuilder(ReportType.BatchSummary);
        //}

        //public static ActivityReportBuilder OpenAuths() {
        //    return new ActivityReportBuilder(ReportType.OpenAuths);
        //}

        //public static ActivityReportBuilder Search() {
        //    return new ActivityReportBuilder(ReportType.Search);
        //}

        public static TransactionReportBuilder<TransactionSummary> TransactionDetail(string transactionId) {
            return new TransactionReportBuilder<TransactionSummary>(ReportType.TransactionDetail).WithTransactionId(transactionId);
        }
    }
}
