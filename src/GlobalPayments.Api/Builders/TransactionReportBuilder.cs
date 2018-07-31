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
        
        private SearchCriteriaBuilder<TResult> _searchBuilder;
        internal SearchCriteriaBuilder<TResult> SearchBuilder {
            get {
                if (_searchBuilder == null)
                    _searchBuilder = new SearchCriteriaBuilder<TResult>(this);
                return _searchBuilder;
            }
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

        /// <summary>
        /// Sets the gateway transaction ID as criteria for the report.
        /// </summary>
        /// <param name="value">The gateway transaction ID</param>
        /// <returns>TResult</returns>
        public TransactionReportBuilder<TResult> WithTransactionId(string value) {
            TransactionId = value;
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
