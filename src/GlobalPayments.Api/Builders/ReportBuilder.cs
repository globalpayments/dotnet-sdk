using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Enums;
using GlobalPayments.Api.Gateways;
using System;

namespace GlobalPayments.Api.Builders {
    public abstract class ReportBuilder<TResult> : BaseBuilder<TResult> where TResult : class {
        internal ReportType ReportType { get; set; }
        internal TimeZoneConversion TimeZoneConversion { get; set; }
        internal SortDirection? Order { get; set; }
        internal MerchantAccountsSortProperty? AccountOrderBy { get; set; }
        internal TransactionSortProperty? TransactionOrderBy { get; set; }
        internal DepositSortProperty? DepositOrderBy { get; set; }
        internal DisputeSortProperty? DisputeOrderBy { get; set; }
        internal StoredPaymentMethodSortProperty? StoredPaymentMethodOrderBy { get; set; }
        internal ActionSortProperty? ActionOrderBy { get; set; }
        internal PayByLinkSortProperty? PayByLinkOrderBy { get; set; }
        internal string TransactionId { get; set; }

        internal DateTime? StartDate { get; set; }
        internal DateTime? EndDate { get; set; }

        private SearchCriteriaBuilder<TResult> _searchBuilder;
        internal SearchCriteriaBuilder<TResult> SearchBuilder {
            get {
                if (_searchBuilder == null)
                    _searchBuilder = new SearchCriteriaBuilder<TResult>(this);
                return _searchBuilder;
            }
        }
        internal int? Page { get; set; }
        internal int? PageSize { get; set; }

        public ReportBuilder(ReportType type) : base() {
            ReportType = type;
        }

        /// <summary>
        /// Executes the builder against the gateway.
        /// </summary>
        /// <returns>TResult</returns>
        public override TResult Execute(string configName = "default") {
            base.Execute(configName);
            object client;
            switch (ReportType) {
                case ReportType.FindBankPayment:
                    client = ServicesContainer.Instance.GetOpenBanking(configName);                    
                    break;
                default:
                    client = ServicesContainer.Instance.GetReportingClient(configName);
                    break;

            }
            return ((IReportingService)client).ProcessReport(this);
        }

        /// <summary>
        /// Set the gateway transaction order by criteria for the report.
        /// </summary>
        /// <param name="orderBy">Order by property</param>
        /// <param name="direction">Order by direction</param>
        /// <returns>TResult</returns>
        public ReportBuilder<TResult> OrderBy(TransactionSortProperty orderBy, SortDirection direction = SortDirection.Ascending) {
            TransactionOrderBy = orderBy;
            Order = direction;
            return this;
        }

        /// <summary>
        /// Set the gateway deposit order by criteria for the report.
        /// </summary>
        /// <param name="orderBy">Order by property</param>
        /// <param name="direction">Order by direction</param>
        /// <returns>TResult</returns>
        public ReportBuilder<TResult> OrderBy(DepositSortProperty orderBy, SortDirection direction = SortDirection.Ascending) {
            DepositOrderBy = orderBy;
            Order = direction;
            return this;
        }

        /// <summary>
        /// Set the gateway dispute order by criteria for the report.
        /// </summary>
        /// <param name="orderBy">Order by property</param>
        /// <param name="direction">Order by direction</param>
        /// <returns>TResult</returns>
        public ReportBuilder<TResult> OrderBy(DisputeSortProperty orderBy, SortDirection direction = SortDirection.Ascending) {
            DisputeOrderBy = orderBy;
            Order = direction;
            return this;
        }

        /// <summary>
        /// Set the gateway stored payment method order by criteria for the report.
        /// </summary>
        /// <param name="orderBy">Order by property</param>
        /// <param name="direction">Order by direction</param>
        /// <returns></returns>
        public ReportBuilder<TResult> OrderBy(StoredPaymentMethodSortProperty orderBy, SortDirection direction = SortDirection.Ascending) {
            StoredPaymentMethodOrderBy = orderBy;
            Order = direction;
            return this;
        }

        /// <summary>
        /// Set the gateway action order by criteria for the report.
        /// </summary>
        /// <param name="orderBy">Order by property</param>
        /// <param name="direction">Order by direction</param>
        /// <returns></returns>
        public ReportBuilder<TResult> OrderBy(ActionSortProperty orderBy, SortDirection direction = SortDirection.Ascending) {
            ActionOrderBy = orderBy;
            Order = direction;
            return this;
        }

        public ReportBuilder<TResult> OrderBy(PayByLinkSortProperty orderBy, SortDirection direction = SortDirection.Ascending) {
            PayByLinkOrderBy = orderBy;
            Order = direction;
            return this;
        }

        /// <summary>
        /// Set the gateway action order by criteria for the Merchant and Account report.
        /// </summary>
        /// <param name="orderBy"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public ReportBuilder<TResult> OrderBy(MerchantAccountsSortProperty orderBy, SortDirection direction = SortDirection.Ascending) {
            AccountOrderBy = orderBy;
            Order = direction;
            return this;
        }

        public SearchCriteriaBuilder<TResult> Where<T>(SearchCriteria criteria, T value) {
            return SearchBuilder.And(criteria, value);
        }

        public SearchCriteriaBuilder<TResult> Where<T>(DataServiceCriteria criteria, T value) {
            return SearchBuilder.And(criteria, value);
        }

        /// <summary>
        /// Sets the gateway transaction ID as criteria for the report.
        /// </summary>
        /// <param name="value">The gateway transaction ID</param>
        /// <returns>TResult</returns>
        public ReportBuilder<TResult> WithTransactionId(string value) {
            TransactionId = value;
            return this;
        }
        public ReportBuilder<TResult> WithTimeZoneConversion(TimeZoneConversion value) {
            TimeZoneConversion = value;
            return this;
        }

        public ReportBuilder<TResult> WithStartDate(DateTime? value)
        {
            StartDate = value;
            return this;
        }

        public ReportBuilder<TResult> WithEndDate(DateTime? value)
        {
            EndDate = value;
            return this;
        }
        /// <summary>
        /// Sets the gateway deposit reference as criteria for the report.
        /// </summary>
        /// <param name="value">The gateway deposit reference</param>
        /// <returns>TResult</returns>
        public ReportBuilder<TResult> WithDepositReference(string value) {
            SearchBuilder.DepositReference = value;
            return this;
        }

        /// <summary>
        /// Set the gateway paging criteria for the report.
        /// </summary>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>TResult</returns>
        public ReportBuilder<TResult> WithPaging(int page, int pageSize) {
            Page = page;
            PageSize = pageSize;
            return this;
        }
    }
}
