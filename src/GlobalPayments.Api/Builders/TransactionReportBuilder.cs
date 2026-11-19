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
        /// <summary>
        /// The batch ID used by GP-API batch reports.
        /// </summary>
        internal string BatchId {
            get {
                return SearchBuilder.BatchId;
            }
        }
        internal string TransactionId {
            get { return SearchBuilder.TransactionId; }
        }

        internal string DisputeDocumentId { get; set; }
        internal string PayByLinkId { get; set; }


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

        public TransactionReportBuilder<TResult> WithPayByLinkId(string payByLinkId) {
            SearchBuilder.PayByLinkId = payByLinkId;
            PayByLinkId = payByLinkId;
            return this;
        }

        public TransactionReportBuilder(ReportType type) : base(type) { }

        protected override void SetupValidations() {

            #region ENUM VALIDATION WITH FLAG ATTRIBUTE 

            Validations.For(ReportType.TransactionDetail)
                .Check(() => TransactionId).IsNotNull();

            Validations.For(ReportType.BatchById)
                .Check(() => BatchId).IsNotNull();

            Validations.For(ReportType.BatchTransactions)
                .Check(() => BatchId).IsNotNull();

            Validations.For(ReportType.Activity).Check(() => TransactionId).IsNull();
            Validations.For(ReportType.DocumentDisputeDetail).Check(() => DisputeDocumentId).IsNotNull();
            Validations.For(ReportType.PayByLinkDetail).Check(() => PayByLinkId).IsNotNull();

            #endregion
        }
    }

    /// <summary>
    /// Builds GP-API authentication list and detail requests.
    /// </summary>
    /// <typeparam name="TResult">The authentication response type.</typeparam>
    public class AuthenticationReportBuilder<TResult> : TransactionReportBuilder<TResult> where TResult : class {
        internal string AuthenticationId { get; private set; }
        internal decimal? AuthenticationAmount { get; private set; }
        internal string AuthenticationCountry { get; private set; }
        internal string AuthenticationCurrency { get; private set; }
        internal string AuthenticationSource { get; private set; }
        internal string DirectoryServerTransactionReference { get; private set; }
        internal string ServerTransactionReference { get; private set; }
        internal string LiabilityShift { get; private set; }
        internal string AuthenticationValue { get; private set; }
        internal string Eci { get; private set; }
        internal string MessageVersion { get; private set; }
        internal string AuthenticationOrderBy { get; private set; }

        /// <summary>
        /// Initializes an authentication report builder.
        /// </summary>
        /// <param name="type">The authentication report operation.</param>
        public AuthenticationReportBuilder(ReportType type) : base(type) { }

        protected override void SetupValidations() {
            base.SetupValidations();
            Validations.For(ReportType.AuthenticationDetail)
                .Check(() => AuthenticationId).IsNotNull();
        }

        /// <summary>Sets the requested page and page size.</summary>
        /// <param name="page">The page number, starting at one.</param>
        /// <param name="pageSize">The maximum number of records to return.</param>
        /// <returns>The current builder.</returns>
        public new AuthenticationReportBuilder<TResult> WithPaging(int page, int pageSize) {
            base.WithPaging(page, pageSize);
            return this;
        }

        /// <summary>Sets the inclusive start of the authentication creation-date range.</summary>
        /// <param name="value">The start date.</param>
        /// <returns>The current builder.</returns>
        public new AuthenticationReportBuilder<TResult> WithStartDate(DateTime? value) {
            base.WithStartDate(value);
            return this;
        }

        /// <summary>Sets the inclusive end of the authentication creation-date range.</summary>
        /// <param name="value">The end date.</param>
        /// <returns>The current builder.</returns>
        public new AuthenticationReportBuilder<TResult> WithEndDate(DateTime? value) {
            base.WithEndDate(value);
            return this;
        }

        /// <summary>Filters by authentication identifier.</summary>
        /// <param name="value">The authentication identifier.</param>
        /// <returns>The current builder.</returns>
        public AuthenticationReportBuilder<TResult> WithAuthenticationId(string value) {
            AuthenticationId = value;
            return this;
        }

        /// <summary>Filters by amount.</summary>
        /// <param name="value">The amount in major currency units.</param>
        /// <returns>The current builder.</returns>
        public AuthenticationReportBuilder<TResult> WithAmount(decimal value) {
            AuthenticationAmount = value;
            return this;
        }

        /// <summary>Filters by country code.</summary>
        /// <param name="value">The ISO 3166-1 alpha-2 country code.</param>
        /// <returns>The current builder.</returns>
        public AuthenticationReportBuilder<TResult> WithCountry(string value) {
            AuthenticationCountry = value;
            return this;
        }

        /// <summary>Filters by currency code.</summary>
        /// <param name="value">The ISO 4217 currency code.</param>
        /// <returns>The current builder.</returns>
        public AuthenticationReportBuilder<TResult> WithCurrency(string value) {
            AuthenticationCurrency = value;
            return this;
        }

        /// <summary>Filters by authentication source.</summary>
        /// <param name="value">The source value, such as BROWSER or MOBILE_SDK.</param>
        /// <returns>The current builder.</returns>
        public AuthenticationReportBuilder<TResult> WithSource(string value) {
            AuthenticationSource = value;
            return this;
        }

        /// <summary>Filters by the 3DS Directory Server transaction reference.</summary>
        /// <param name="value">The Directory Server transaction reference.</param>
        /// <returns>The current builder.</returns>
        public AuthenticationReportBuilder<TResult> WithDirectoryServerTransactionReference(string value) {
            DirectoryServerTransactionReference = value;
            return this;
        }

        /// <summary>Filters by the 3DS server transaction reference.</summary>
        /// <param name="value">The server transaction reference.</param>
        /// <returns>The current builder.</returns>
        public AuthenticationReportBuilder<TResult> WithServerTransactionReference(string value) {
            ServerTransactionReference = value;
            return this;
        }

        /// <summary>Filters by liability-shift status.</summary>
        /// <param name="value">The liability-shift value.</param>
        /// <returns>The current builder.</returns>
        public AuthenticationReportBuilder<TResult> WithLiabilityShift(string value) {
            LiabilityShift = value;
            return this;
        }

        /// <summary>Filters by 3DS authentication value.</summary>
        /// <param name="value">The authentication value.</param>
        /// <returns>The current builder.</returns>
        public AuthenticationReportBuilder<TResult> WithAuthenticationValue(string value) {
            AuthenticationValue = value;
            return this;
        }

        /// <summary>Filters by 3DS electronic commerce indicator.</summary>
        /// <param name="value">The ECI value.</param>
        /// <returns>The current builder.</returns>
        public AuthenticationReportBuilder<TResult> WithEci(string value) {
            Eci = value;
            return this;
        }

        /// <summary>Filters by 3DS message version.</summary>
        /// <param name="value">The message version.</param>
        /// <returns>The current builder.</returns>
        public AuthenticationReportBuilder<TResult> WithMessageVersion(string value) {
            MessageVersion = value;
            return this;
        }

        /// <summary>Sets the result ordering.</summary>
        /// <param name="orderBy">The API field to order by.</param>
        /// <param name="direction">The sort direction.</param>
        /// <returns>The current builder.</returns>
        public AuthenticationReportBuilder<TResult> OrderBy(string orderBy, SortDirection direction = SortDirection.Ascending) {
            AuthenticationOrderBy = orderBy;
            Order = direction;
            return this;
        }
    }
}
