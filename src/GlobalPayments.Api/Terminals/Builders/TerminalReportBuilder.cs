using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Terminals.Enums;
using GlobalPayments.Api.Terminals.Genius.Enums;
using GlobalPayments.Api.Terminals.Diamond.Entities.Enums;
using GlobalPayments.Api.Terminals.PAX;
using GlobalPayments.Api.Terminals.UPA;
using System;
using System.Linq;
using System.Reflection;

namespace GlobalPayments.Api.Terminals.Builders {
    /// <summary>
    /// Fluent builder for constructing terminal report requests across device families (UPA, PAX, Genius, Diamond).
    /// </summary>
    public class TerminalReportBuilder {
        /// <summary>
        /// The type of report to retrieve from the terminal.
        /// </summary>
        internal TerminalReportType ReportType { get; set; }

        /// <summary>
        /// The transaction identifier used to scope the report to a specific transaction.
        /// </summary>
        internal string TransactionId { get; set; }

        /// <summary>
        /// The transaction type used when querying by transaction.
        /// </summary>
        internal TransactionType TransactionType { get; set; }

        /// <summary>
        /// Indicates whether <see cref="TransactionId"/> refers to a gateway, terminal, or ECR transaction ID.
        /// </summary>
        internal TransactionIdType TransactionIdType { get; set; }

        private TerminalSearchBuilder _searchBuilder;

        /// <summary>
        /// Lazily-initialised search builder for applying filter criteria to the report request.
        /// </summary>
        internal TerminalSearchBuilder SearchBuilder {
            get {
                if (_searchBuilder == null) {
                    _searchBuilder = new TerminalSearchBuilder(this);
                }
                return _searchBuilder;
            }
        }

        /// <summary>
        /// Initialises a report builder for a specific report type.
        /// </summary>
        /// <param name="reportType">The type of report to retrieve.</param>
        public TerminalReportBuilder(TerminalReportType reportType) {
            ReportType = reportType;
        }

        /// <summary>
        /// Initialises a report builder scoped to a specific transaction.
        /// </summary>
        /// <param name="transactionType">The transaction type of the target transaction.</param>
        /// <param name="transactionId">The identifier of the target transaction.</param>
        /// <param name="transactionIdType">The ID type — gateway, terminal, or ECR reference.</param>
        public TerminalReportBuilder(TransactionType transactionType, string transactionId, TransactionIdType transactionIdType) {
            TransactionType = transactionType;
            TransactionId = transactionId;
            TransactionIdType = transactionIdType;
        }

        /// <summary>
        /// Adds a PAX-specific search filter criterion to the report request.
        /// </summary>
        /// <typeparam name="T">The type of the filter value.</typeparam>
        /// <param name="criteria">The PAX search criterion to apply.</param>
        /// <param name="value">The value to filter by.</param>
        /// <returns>A <see cref="TerminalSearchBuilder"/> for further filter chaining.</returns>
        public TerminalSearchBuilder Where<T>(PaxSearchCriteria criteria, T value) {
            return SearchBuilder.And(criteria, value);
        }

        /// <summary>
        /// Adds a Diamond Cloud-specific search filter criterion to the report request.
        /// </summary>
        /// <typeparam name="T">The type of the filter value.</typeparam>
        /// <param name="criteria">The Diamond Cloud search criterion to apply.</param>
        /// <param name="value">The value to filter by.</param>
        /// <returns>A <see cref="TerminalSearchBuilder"/> for further filter chaining.</returns>
        public TerminalSearchBuilder Where<T>(DiamondCloudSearchCriteria criteria, T value) {
            return SearchBuilder.And(criteria, value);
        }

        /// <summary>
        /// Adds a UPA-specific search filter criterion to the report request.
        /// </summary>
        /// <typeparam name="T">The type of the filter value.</typeparam>
        /// <param name="criteria">The UPA search criterion to apply.</param>
        /// <param name="value">The value to filter by.</param>
        /// <returns>A <see cref="TerminalSearchBuilder"/> for further filter chaining.</returns>
        public TerminalSearchBuilder Where<T>(UpaSearchCriteria criteria, T value) {
            return SearchBuilder.And(criteria, value);
        }

        /// <summary>
        /// Executes the report request against the configured terminal device.
        /// </summary>
        /// <param name="configName">The named service container configuration to use. Defaults to <c>"default"</c>.</param>
        /// <returns>An <see cref="ITerminalReport"/> containing the report data returned by the device.</returns>
        public ITerminalReport Execute(string configName = "default") {
            var device = ServicesContainer.Instance.GetDeviceController(configName);
            return device.ProcessReport(this);
        }
    }

    /// <summary>
    /// Fluent builder for applying search filter criteria to a <see cref="TerminalReportBuilder"/> request.
    /// </summary>
    public class TerminalSearchBuilder {
        private TerminalReportBuilder _reportBuilder;

        /// <summary>
        /// The terminal transaction type to filter by.
        /// </summary>
        internal TerminalTransactionType? TransactionType { get; set; }

        /// <summary>
        /// The card type to filter by (e.g., Visa, Mastercard).
        /// </summary>
        internal TerminalCardType? CardType { get; set; }

        /// <summary>
        /// The record number to retrieve from the terminal's transaction log.
        /// </summary>
        internal int? RecordNumber { get; set; }

        /// <summary>
        /// The terminal-assigned reference number of the transaction to retrieve.
        /// </summary>
        internal int? TerminalReferenceNumber { get; set; }

        /// <summary>
        /// The authorization code to filter by.
        /// </summary>
        internal string AuthCode { get; set; }

        /// <summary>
        /// The reference number to filter by.
        /// </summary>
        public string ReferenceNumber { get; set; }

        /// <summary>
        /// The merchant identifier to filter by.
        /// </summary>
        internal int? MerchantId { get; set; }

        /// <summary>
        /// The merchant name to filter by.
        /// </summary>
        internal string MerchantName { get; set; }

        /// <summary>
        /// The batch number to include in the report request.
        /// </summary>
        internal int Batch { get; set; }

        /// <summary>
        /// The ECR (Electronic Cash Register) identifier associated with the report request.
        /// </summary>
        public string EcrId { get; set; }

        /// <summary>
        /// Specifies the output format of the report.
        /// </summary>
        internal string ReportOutput { get; set; }

        /// <summary>
        /// The report type identifier sent to the device.
        /// </summary>
        internal UpaReportType? ReportType { get; set; }

        /// <summary>
        /// The report sub-type identifier sent to the device.
        /// </summary>
        internal UpaReportSubType? ReportSubType { get; set; }

        /// <summary>
        /// Instructs the device to return both the current and previous batch reports.
        /// </summary>
        internal bool? BothReports { get; set; }

        /// <summary>
        /// The clerk identifier to filter transactions by.
        /// </summary>
        internal int? ClerkId { get; set; }

        /// <summary>
        /// Instructs the device to include the previous batch report in the response.
        /// </summary>
        internal bool? PreviousBatchReport { get; set; }

        /// <summary>
        /// Initialises the search builder with a reference to the owning report builder.
        /// </summary>
        /// <param name="reportBuilder">The <see cref="TerminalReportBuilder"/> that owns this search builder.</param>
        internal TerminalSearchBuilder(TerminalReportBuilder reportBuilder) {
            _reportBuilder = reportBuilder;
        }

        /// <summary>
        /// Adds a PAX-specific filter criterion to the search.
        /// </summary>
        /// <typeparam name="T">The type of the filter value.</typeparam>
        /// <param name="criteria">The PAX search criterion to apply.</param>
        /// <param name="value">The value to filter by.</param>
        /// <returns>This <see cref="TerminalSearchBuilder"/> for further chaining.</returns>
        public TerminalSearchBuilder And<T>(PaxSearchCriteria criteria, T value) {
            SetProperty(criteria.ToString(), value);
            return this;
        }

        /// <summary>
        /// Adds a UPA-specific filter criterion to the search.
        /// </summary>
        /// <typeparam name="T">The type of the filter value.</typeparam>
        /// <param name="criteria">The UPA search criterion to apply.</param>
        /// <param name="value">The value to filter by.</param>
        /// <returns>This <see cref="TerminalSearchBuilder"/> for further chaining.</returns>
        public TerminalSearchBuilder And<T>(UpaSearchCriteria criteria, T value) {
            SetProperty(criteria.ToString(), value);
            return this;
        }

        /// <summary>
        /// Adds a Diamond Cloud-specific filter criterion to the search.
        /// </summary>
        /// <typeparam name="T">The type of the filter value.</typeparam>
        /// <param name="criteria">The Diamond Cloud search criterion to apply.</param>
        /// <param name="value">The value to filter by.</param>
        /// <returns>This <see cref="TerminalSearchBuilder"/> for further chaining.</returns>
        public TerminalSearchBuilder And<T>(DiamondCloudSearchCriteria criteria, T value) {
            SetProperty(criteria.ToString(), value);
            return this;
        }

        /// <summary>
        /// Executes the report request with the accumulated search criteria.
        /// </summary>
        /// <param name="configName">The named service container configuration to use. Defaults to <c>"default"</c>.</param>
        /// <returns>An <see cref="ITerminalReport"/> containing the report data returned by the device.</returns>
        public ITerminalReport Execute(string configName = "default") {
            return _reportBuilder.Execute(configName);
        }

        /// <summary>
        /// Sets a property on this builder by name using reflection, with type coercion for nullable and convertible types.
        /// </summary>
        /// <typeparam name="T">The type of the value to set.</typeparam>
        /// <param name="propertyName">The name of the property to set.</param>
        /// <param name="value">The value to assign.</param>
        private void SetProperty<T>(string propertyName, T value) {
            var prop = GetType().GetRuntimeProperties().FirstOrDefault(p => p.Name == propertyName);
            if (prop != null) {
                if (prop.PropertyType == typeof(T))
                    prop.SetValue(this, value);
                else if (prop.PropertyType.Name == "Nullable`1") {
                    if (prop.PropertyType.GenericTypeArguments[0] == typeof(T))
                        prop.SetValue(this, value);
                    else {
                        var convertedValue = Convert.ChangeType(value, prop.PropertyType.GenericTypeArguments[0]);
                        prop.SetValue(this, convertedValue);
                    }
                }
                else {
                    var convertedValue = Convert.ChangeType(value, prop.PropertyType);
                    prop.SetValue(this, convertedValue);
                }
            }
        }
    }
}
