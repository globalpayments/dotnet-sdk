using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Builders
{
    public class UserReportBuilder<TResult> : ReportBuilder<TResult> where TResult : class
    {
        
        public SortDirection Order { get; set; }

        public TransactionType TransactionType { get; set; }

        public TransactionModifier TransactionModifier { get; set; } = TransactionModifier.None;

        public UserReportBuilder<TResult> WithModifier(TransactionModifier transactionModifier) {
            TransactionModifier = transactionModifier;
            return this;
        }

        public UserReportBuilder<TResult> WithAccountId(string accountId) {
            SearchBuilder.AccountId = accountId;
            return this;
        }       
        public UserReportBuilder(ReportType reportType) : base(reportType) { }

        protected override void SetupValidations()
        {
           
        }       
    }
}
