using System.Collections.Generic;
using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Reporting;

namespace GlobalPayments.Api.Services {
    public class ReportingService {
        public static ReportBuilder<List<TransactionSummary>> FindTransactions(string transactionId = null) {
            return new TransactionReportBuilder<List<TransactionSummary>>(ReportType.FindTransactions)
                .WithTransactionId(transactionId);
        }

        public static TransactionReportBuilder<List<DepositSummary>> FindDeposits() {
            return new TransactionReportBuilder<List<DepositSummary>>(ReportType.FindDeposits);
        }
        public static TransactionReportBuilder<TokenUpdaterHistoryResponse> TokenUpdaterHistory() {
            return new TransactionReportBuilder<TokenUpdaterHistoryResponse>(ReportType.TokenUpdaterHistory);
        }
        public static TransactionReportBuilder<List<DisputeSummary>> FindDisputes() {
            return new TransactionReportBuilder<List<DisputeSummary>>(ReportType.FindDisputes);
        }

        public static TransactionReportBuilder<List<TransactionSummary>> Activity() {
            return new TransactionReportBuilder<List<TransactionSummary>>(ReportType.Activity);
        }

        public static TransactionReportBuilder<List<TransactionSummary>> BatchDetail()
        {
            return new TransactionReportBuilder<List<TransactionSummary>>(ReportType.BatchDetail);
        }

        public static TransactionReportBuilder<List<TransactionSummary>> OpenAuths()
        {
            return new TransactionReportBuilder<List<TransactionSummary>>(ReportType.OpenAuths);
        }

        public static ReportBuilder<TransactionSummary> TransactionDetail(string transactionId) {
            return new TransactionReportBuilder<TransactionSummary>(ReportType.TransactionDetail)
                .WithTransactionId(transactionId);
        }

        public static ReportBuilder<DepositSummary> DepositDetail(string depositId) {
            return new TransactionReportBuilder<DepositSummary>(ReportType.DepositDetail)
                .WithDepositReference(depositId);
        }
       
        public static TransactionReportBuilder<DisputeSummary> DisputeDetail(string disputeId) {
            return new TransactionReportBuilder<DisputeSummary>(ReportType.DisputeDetail)
                .WithDisputeId(disputeId);
        }

        public static TransactionReportBuilder<DisputeDocument> DocumentDisputeDetail(string disputeId)
        {
            return new TransactionReportBuilder<DisputeDocument>(ReportType.DocumentDisputeDetail)
                .WithDisputeId(disputeId);
        }

        public static TransactionReportBuilder<DisputeSummary> SettlementDisputeDetail(string settlementDisputeId) {
            return new TransactionReportBuilder<DisputeSummary>(ReportType.SettlementDisputeDetail)
                .WithSettlementDisputeId(settlementDisputeId);
        }

        public static TransactionReportBuilder<StoredPaymentMethodSummary> StoredPaymentMethodDetail(string storedPaymentMethodId) {
            return new TransactionReportBuilder<StoredPaymentMethodSummary>(ReportType.StoredPaymentMethodDetail)
                .WithStoredPaymentMethodId(storedPaymentMethodId);
        }

        public static TransactionReportBuilder<ActionSummary> ActionDetail(string actionId) {
            return new TransactionReportBuilder<ActionSummary>(ReportType.ActionDetail)
                .WithActionId(actionId);
        }

        public static ReportBuilder<PagedResult<TransactionSummary>> BankPaymentDetail(string bankPaymentId, int page, int pageSize)
        {
            return new TransactionReportBuilder<PagedResult<TransactionSummary>>(ReportType.FindBankPayment)
                .WithBankPaymentId(bankPaymentId)
                .WithPaging(page, pageSize);
        }

        public static ReportBuilder<PagedResult<TransactionSummary>> FindBankPaymentTransactions(int page, int pageSize)
        {
            return new TransactionReportBuilder<PagedResult<TransactionSummary>>(ReportType.FindBankPayment)
                .WithPaging(page, pageSize);
        }

        public static ReportBuilder<PagedResult<TransactionSummary>> FindTransactionsPaged(int page, int pageSize, string transactionId = null) {
            return new TransactionReportBuilder<PagedResult<TransactionSummary>>(ReportType.FindTransactionsPaged)
                .WithTransactionId(transactionId)
                .WithPaging(page, pageSize);
        }

        public static ReportBuilder<PagedResult<TransactionSummary>> FindSettlementTransactionsPaged(int page, int pageSize) {
            return new TransactionReportBuilder<PagedResult<TransactionSummary>>(ReportType.FindSettlementTransactionsPaged)
                .WithPaging(page, pageSize);
        }

        public static ReportBuilder<PagedResult<DepositSummary>> FindDepositsPaged(int page, int pageSize) {
            return new TransactionReportBuilder<PagedResult<DepositSummary>>(ReportType.FindDepositsPaged)
                .WithPaging(page, pageSize);
        }

        public static ReportBuilder<PagedResult<DisputeSummary>> FindDisputesPaged(int page, int pageSize) {
            return new TransactionReportBuilder<PagedResult<DisputeSummary>>(ReportType.FindDisputesPaged)
                .WithPaging(page, pageSize);
        }

        public static ReportBuilder<PagedResult<DisputeSummary>> FindSettlementDisputesPaged(int page, int pageSize) {
            return new TransactionReportBuilder<PagedResult<DisputeSummary>>(ReportType.FindSettlementDisputesPaged)
                .WithPaging(page, pageSize);
        }

        public static ReportBuilder<PagedResult<StoredPaymentMethodSummary>> FindStoredPaymentMethodsPaged(int page, int pageSize) {
            return new TransactionReportBuilder<PagedResult<StoredPaymentMethodSummary>>(ReportType.FindStoredPaymentMethodsPaged)
                .WithPaging(page, pageSize);
        }

        public static ReportBuilder<PagedResult<ActionSummary>> FindActionsPaged(int page, int pageSize) {
            return new TransactionReportBuilder<PagedResult<ActionSummary>>(ReportType.FindActionsPaged)
                .WithPaging(page, pageSize);
        }

        public ReportBuilder<PagedResult<MerchantSummary>> FindMerchants(int page, int pageSize) {
            return new UserReportBuilder<PagedResult<MerchantSummary>>(ReportType.FindMerchantsPaged)
                .WithModifier(TransactionModifier.Merchant)
                .WithPaging(page, pageSize);
        }

        public static ReportBuilder<PagedResult<MerchantAccountSummary>> FindAccounts(int page, int pageSize) {
            return (new UserReportBuilder<PagedResult<MerchantAccountSummary>>(ReportType.FindAccountsPaged))
                .WithPaging(page, pageSize);
        }

        public static UserReportBuilder<MerchantAccountSummary> AccountDetail(string accountId) {
            return (new UserReportBuilder<MerchantAccountSummary>(ReportType.FindAccountDetail))
                .WithAccountId(accountId);
        }
    }
}
