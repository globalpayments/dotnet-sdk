using System;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Enums;

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
       
        internal string DisputeDocumentId { get; set; }
        internal string PayLinkId { get; set; }
       

        /// <summary>
        /// Sets the gateway action id as criteria for the report.
        /// </summary>
        /// <param name="value">The gateway action id</param>
        /// <returns>TResult</returns>
        public TransactionReportBuilder<TResult> WithActionId(string value) {
            SearchBuilder.ActionId = value;
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
        /// Sets the gateway DisputeDocumentId as criteria for the report
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public TransactionReportBuilder<TResult> WithDisputeDocumentId(string value) {
            DisputeDocumentId = value;
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
        /// Sets the gateway settlement dispute id as criteria for the report.
        /// </summary>
        /// <param name="value">The gateway settlement dispute id</param>
        /// <returns>TResult</returns>
        public TransactionReportBuilder<TResult> WithSettlementDisputeId(string value) {
            SearchBuilder.SettlementDisputeId = value;
            return this;
        }

        /// <summary>
        /// Sets the gateway stored payment method id as a critria for the report.
        /// </summary>
        /// <param name="value">The stored payment method id</param>
        /// <returns></returns>
        public TransactionReportBuilder<TResult> WithStoredPaymentMethodId(string value) {
            SearchBuilder.StoredPaymentMethodId = value;
            return this;
        }

        public TransactionReportBuilder<TResult> WithBankPaymentId(string bankPaymentId) {
            SearchBuilder.BankPaymentId = bankPaymentId;
            return this;
        }

        public TransactionReportBuilder<TResult> WithPayLinkId(string payLinkId)
        {
            SearchBuilder.PayLinkId = payLinkId;
            PayLinkId = payLinkId;
            return this;
        }

        public TransactionReportBuilder(ReportType type) : base(type) { }

        protected override void SetupValidations() {
            
            #region ENUM VALIDATION WITH FLAG ATTRIBUTE 
            
            Validations.For(ReportType.TransactionDetail)
                .Check(() => TransactionId).IsNotNull();

            Validations.For(ReportType.Activity).Check(() => TransactionId).IsNull();
            Validations.For(ReportType.DocumentDisputeDetail).Check(() => DisputeDocumentId).IsNotNull();
            Validations.For(ReportType.PayLinkDetail).Check(() => PayLinkId).IsNotNull();

            #endregion
        }
    }
}
