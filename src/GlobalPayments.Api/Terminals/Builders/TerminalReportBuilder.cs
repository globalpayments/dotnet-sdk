using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Terminals.Genius.Enums;
using GlobalPayments.Api.Terminals.Diamond.Entities.Enums;
using GlobalPayments.Api.Terminals.PAX;
using GlobalPayments.Api.Terminals.UPA;
using System;
using System.Linq;
using System.Reflection;

namespace GlobalPayments.Api.Terminals.Builders {
    public class TerminalReportBuilder {
        internal TerminalReportType ReportType { get; set; }
        internal string TransactionId { get; set; }
        internal TransactionType TransactionType { get; set; }
        internal TransactionIdType TransactionIdType { get; set; }

        private TerminalSearchBuilder _searchBuilder;
        internal TerminalSearchBuilder SearchBuilder {
            get {
                if (_searchBuilder == null) {
                    _searchBuilder = new TerminalSearchBuilder(this);
                }
                return _searchBuilder;
            }
        }

        public TerminalReportBuilder(TerminalReportType reportType) {
            ReportType = reportType;
        }
        public TerminalReportBuilder(TransactionType transactionType, string transactionId, TransactionIdType transactionIdType) {
            TransactionType = transactionType;
            TransactionId = transactionId;
            TransactionIdType = transactionIdType;
        }

        public TerminalSearchBuilder Where<T>(PaxSearchCriteria criteria, T value) {
            return SearchBuilder.And(criteria, value);
        }
        public TerminalSearchBuilder Where<T>(DiamondCloudSearchCriteria criteria, T value) {
            return SearchBuilder.And(criteria, value);
        }
        public TerminalSearchBuilder Where<T>(UpaSearchCriteria criteria, T value) {
            return SearchBuilder.And(criteria, value);
        }
        public ITerminalReport Execute(string configName = "default") {
            var device = ServicesContainer.Instance.GetDeviceController(configName);
            return device.ProcessReport(this);
        }
    }

    public class TerminalSearchBuilder {
        private TerminalReportBuilder _reportBuilder;

        internal TerminalTransactionType? TransactionType { get; set; }
        internal TerminalCardType? CardType { get; set; }
        internal int? RecordNumber { get; set; }
        internal int? TerminalReferenceNumber { get; set; }
        internal string AuthCode { get; set; }
        public string ReferenceNumber { get; set; }
        internal int? MerchantId { get; set; }
        internal string MerchantName { get; set; }
        internal int Batch { get; set; }
        public string EcrId { get; set; }
        internal string ReportOutput { get; set; }
        internal TerminalSearchBuilder(TerminalReportBuilder reportBuilder) {
            _reportBuilder = reportBuilder;
        }

        public TerminalSearchBuilder And<T>(PaxSearchCriteria criteria, T value) {
            SetProperty(criteria.ToString(), value);
            return this;
        }

        public TerminalSearchBuilder And<T>(UpaSearchCriteria criteria, T value) {
            SetProperty(criteria.ToString(), value);
            return this;
        }

        public TerminalSearchBuilder And<T>(DiamondCloudSearchCriteria criteria, T value) {
            SetProperty(criteria.ToString(), value);
            return this;
        }

        public ITerminalReport Execute(string configName = "default") {
            return _reportBuilder.Execute(configName);
        }

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
