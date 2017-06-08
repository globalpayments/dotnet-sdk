using System;
using GlobalPayments.Api.Entities;

namespace GlobalPayments.Api.Builders {
    public class TransactionReportBuilder<TResult> : ReportBuilder<TResult> where TResult : class {
        internal string DeviceId { get; set; }
        internal DateTime? EndDate { get; set; }
        internal DateTime? StartDate { get; set; }
        internal string TransactionId { get; set; }

        public TransactionReportBuilder<TResult> WithDeviceId(string value) {
            DeviceId = value;
            return this;
        }
        public TransactionReportBuilder<TResult> WithEndDate(DateTime? value) {
            EndDate = value;
            return this;
        }
        public TransactionReportBuilder<TResult> WithStartDate(DateTime? value) {
            StartDate = value;
            return this;
        }
        public TransactionReportBuilder<TResult> WithTransactionId(string value) {
            TransactionId = value;
            return this;
        }

        public TransactionReportBuilder(ReportType type) : base(type) { }

        protected override void SetupValidations() {
            Validations.For(ReportType.TransactionDetail)
                .Check(() => TransactionId).IsNotNull()
                .Check(() => DeviceId).IsNull()
                .Check(() => StartDate).IsNull()
                .Check(() => EndDate).IsNull();

            Validations.For(ReportType.Activity).Check(() => TransactionId).IsNull();
        }
    }
}
