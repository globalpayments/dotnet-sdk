using System;
using GlobalPayments.Api.Entities;

namespace GlobalPayments.Api.Builders {
    public class TransactionReportBuilder<TResult> : ReportBuilder<TResult> where TResult : class {
        internal string DeviceId {
            get {
                return SearchBuilder.UniqueDeviceId;
            }
        }
        internal DateTime? EndDate {
            get {
                return SearchBuilder.EndDate;
            }
        }
        internal DateTime? StartDate {
            get {
                return SearchBuilder.StartDate;
            }
        }
        internal string TransactionId { get; set; }
        internal int? Page { get; set; }
        internal int? PageSize { get; set; }
        internal TransactionSortProperty? TransactionOrderBy { get; set; }
        internal SortDirection? TransactionOrder { get; set; }
        internal DepositSortProperty? DepositOrderBy { get; set; }
        internal SortDirection? DepositOrder { get; set; }
        internal DisputeSortProperty? DisputeOrderBy { get; set; }
        internal SortDirection? DisputeOrder { get; set; }

        private SearchCriteriaBuilder<TResult> _searchBuilder;
        internal SearchCriteriaBuilder<TResult> SearchBuilder {
            get {
                if (_searchBuilder == null)
                    _searchBuilder = new SearchCriteriaBuilder<TResult>(this);
                return _searchBuilder;
            }
        }

        /// <summary>
        /// Sets the gateway deposit reference as criteria for the report.
        /// </summary>
        /// <param name="value">The gateway deposit reference</param>
        /// <returns>TResult</returns>
        public TransactionReportBuilder<TResult> WithDepositReference(string value) {
            SearchBuilder.DepositReference = value;
            return this;
        }

        /// <summary>
        /// Sets the device ID as criteria for the report.
        /// </summary>
        /// <param name="value">The device ID</param>
        /// <returns>TResult</returns>
        public TransactionReportBuilder<TResult> WithDeviceId(string value) {
            SearchBuilder.UniqueDeviceId = value;
            return this;
        }

        /// <summary>
        /// Sets the gateway dispute id as criteria for the report.
        /// </summary>
        /// <param name="value">The gateway dispute id</param>
        /// <returns>TResult</returns>
        public TransactionReportBuilder<TResult> WithDisputeId(string value) {
            SearchBuilder.DisputeId = value;
            return this;
        }

        /// <summary>
        /// Sets the end date ID as criteria for the report.
        /// </summary>
        /// <param name="value">The end date ID</param>
        /// <returns>TResult</returns>
        public TransactionReportBuilder<TResult> WithEndDate(DateTime? value) {
            SearchBuilder.EndDate = value;
            return this;
        }

        /// <summary>
        /// Sets the start date ID as criteria for the report.
        /// </summary>
        /// <param name="value">The start date ID</param>
        /// <returns>TResult</returns>
        public TransactionReportBuilder<TResult> WithStartDate(DateTime? value) {
            SearchBuilder.StartDate = value;
            return this;
        }

        public TransactionReportBuilder<TResult> WithTimeZoneConversion(TimeZoneConversion value) {
            TimeZoneConversion = value;
            return this;
        }

        /// <summary>
        /// Sets the gateway transaction ID as criteria for the report.
        /// </summary>
        /// <param name="value">The gateway transaction ID</param>
        /// <returns>TResult</returns>
        public TransactionReportBuilder<TResult> WithTransactionId(string value) {
            TransactionId = value;
            return this;
        }

        /// <summary>
        /// Set the gateway paging criteria for the report.
        /// </summary>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>TResult</returns>
        public TransactionReportBuilder<TResult> WithPaging(int page, int pageSize) {
            Page = page;
            PageSize = pageSize;
            return this;
        }

        /// <summary>
        /// Sets the gateway settlement dispute id as criteria for the report.
        /// </summary>
        /// <param name="value">The gateway settlement dispute id</param>
        /// <returns>TResult</returns>
        public TransactionReportBuilder<TResult> WithSettlementDisputeId(string value) {
            SearchBuilder.SettlementDisputeId = value;
            return this;
        }

        /// <summary>
        /// Set the gateway transaction order by criteria for the report.
        /// </summary>
        /// <param name="orderBy">Order by property</param>
        /// <param name="direction">Order by direction</param>
        /// <returns>TResult</returns>
        public TransactionReportBuilder<TResult> OrderBy(TransactionSortProperty orderBy, SortDirection direction = SortDirection.Ascending) {
            TransactionOrderBy = orderBy;
            TransactionOrder = direction;
            return this;
        }

        /// <summary>
        /// Set the gateway deposit order by criteria for the report.
        /// </summary>
        /// <param name="orderBy">Order by property</param>
        /// <param name="direction">Order by direction</param>
        /// <returns>TResult</returns>
        public TransactionReportBuilder<TResult> OrderBy(DepositSortProperty orderBy, SortDirection direction = SortDirection.Ascending) {
            DepositOrderBy = orderBy;
            DepositOrder = direction;
            return this;
        }

        /// <summary>
        /// Set the gateway dispute order by criteria for the report.
        /// </summary>
        /// <param name="orderBy">Order by property</param>
        /// <param name="direction">Order by direction</param>
        /// <returns>TResult</returns>
        public TransactionReportBuilder<TResult> OrderBy(DisputeSortProperty orderBy, SortDirection direction = SortDirection.Ascending) {
            DisputeOrderBy = orderBy;
            DisputeOrder = direction;
            return this;
        }

        public SearchCriteriaBuilder<TResult> Where<T>(SearchCriteria criteria, T value) {
            return SearchBuilder.And(criteria, value);
        }

        public SearchCriteriaBuilder<TResult> Where<T>(DataServiceCriteria criteria, T value) {
            return SearchBuilder.And(criteria, value);
        }

        public TransactionReportBuilder(ReportType type) : base(type) { }

        protected override void SetupValidations() {
            Validations.For(ReportType.TransactionDetail)
                .Check(() => TransactionId).IsNotNull();

            Validations.For(ReportType.Activity).Check(() => TransactionId).IsNull();
        }
    }
}
