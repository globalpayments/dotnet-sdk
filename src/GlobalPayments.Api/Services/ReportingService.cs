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
            return new TransactionReportBuilder<List<DepositSummary>>(ReportType.FindDeposits);
        }

        public static TransactionReportBuilder<List<DisputeSummary>> FindDisputes() {
            return new TransactionReportBuilder<List<DisputeSummary>>(ReportType.FindDisputes);
        }

        public static TransactionReportBuilder<List<DisputeSummary>> FindSettlementDisputes() {
            return new TransactionReportBuilder<List<DisputeSummary>>(ReportType.FindSettlementDisputes);
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
            return new TransactionReportBuilder<TransactionSummary>(ReportType.TransactionDetail)
                .WithTransactionId(transactionId);
        }

        public static TransactionReportBuilder<DepositSummary> DepositDetail(string depositId) {
            return new TransactionReportBuilder<DepositSummary>(ReportType.DepositDetail)
                .WithDepositId(depositId);
        }

        public static TransactionReportBuilder<DisputeSummary> DisputeDetail(string disputeId) {
            return new TransactionReportBuilder<DisputeSummary>(ReportType.DisputeDetail)
                .WithDisputeId(disputeId);
        }

        public static TransactionReportBuilder<DisputeSummary> SettlementDisputeDetail(string settlementDisputeId) {
            return new TransactionReportBuilder<DisputeSummary>(ReportType.SettlementDisputeDetail)
                .WithSettlementDisputeId(settlementDisputeId);
        }

        public static TransactionReportBuilder<List<TransactionSummary>> FindSettlementTransactions() {
            return new TransactionReportBuilder<List<TransactionSummary>>(ReportType.FindSettlementTransactions);
        }
    }
}
