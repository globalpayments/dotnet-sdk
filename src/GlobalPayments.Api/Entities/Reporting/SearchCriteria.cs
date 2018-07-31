using GlobalPayments.Api.Builders;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace GlobalPayments.Api.Entities {
    public enum SearchCriteria {
        AccountNumberLastFour,
        AltPaymentStatus,
        AuthCode,
        BankRoutingNumber,
        BatchId,
        BatchSequenceNumber,
        BuyerEmailAddress,
        CardHolderFirstName,
        CardHolderLastName,
        CardHolderPoNumber,
        CardNumberFirstSix,
        CardNumberLastFour,
        CheckFirstName,
        CheckLastName,
        CheckName,
        CheckNumber,
        ClerkId,
        ClientTransactionId,
        CustomerId,
        DisplayName,
        EndDate,
        FullyCaptured,
        GiftCurrency,
        GiftMaskedAlias,
        InvoiceNumber,
        IssuerResult,
        IssuerTransactionId,
        OneTime,
        PaymentMethodKey,
        ReferenceNumber,
        SettlementAmount,
        ScheduleId,
        SiteTrace,
        StartDate,
        UniqueDeviceId,
        Username,
    }

    public enum DataServiceCriteria {
        Amount, // Data Services
        BankAccountNumber, // Data Services
        CaseId, // Data Services
        CardNumberFirstSix, // Data Services
        CardNumberLastFour, // Data Services
        CaseNumber, // Data Services
        DepositReference, // Data Services
        EndDepositDate, // Data Services
        Hierarchy, // Data Services
        LocalTransactionEndTime, // Data Services
        LocalTransactionStartTime, // Data Services
        MerchantId, // Data Services
        OrderId, // Data Services
        StartDepositDate, // Data Services
        Timezone // Data Services
    }

    public class SearchCriteriaBuilder<TResult> where TResult : class {
        private TransactionReportBuilder<TResult> _reportBuilder;

        internal string AccountNumberLastFour { get; set; }

        internal string AltPaymentStatus { get; set; }

        internal decimal? Amount { get; set; }

        internal string AuthCode { get; set; }

        internal string BankAccountNumber { get; set; }

        internal string BankRoutingNumber { get; set; }

        internal string BatchId { get; set; }

        internal string BatchSequenceNumber { get; set; }

        internal string BuyerEmailAddress { get; set; }

        internal string CardHolderFirstName { get; set; }

        internal string CardHolderLastName { get; set; }

        internal string CardHolderPoNumber { get; set; }

        internal string CardNumberFirstSix { get; set; }

        internal string CardNumberLastFour { get; set; }

        internal string CaseId { get; set; }

        internal string CaseNumber { get; set; }

        internal IEnumerable<CardType> CardTypes { get; set; }

        internal string CheckFirstName { get; set; }

        internal string CheckLastName { get; set; }

        internal string CheckName { get; set; }

        internal string CheckNumber { get; set; }

        internal string ClerkId { get; set; }

        internal string ClientTransactionId { get; set; }

        internal string CustomerId { get; set; }

        internal string DepositReference { get; set; }

        internal string DisplayName { get; set; }

        internal DateTime? EndDate { get; set; }

        internal DateTime? EndDepositDate { get; set; }

        internal bool? FullyCaptured { get; set; }

        internal string GiftCurrency { get; set; }

        internal string GiftMaskedAlias { get; set; }

        internal string Hierarchy { get; set; }

        internal string InvoiceNumber { get; set; }

        internal string IssuerResult { get; set; }

        internal string IssuerTransactionId { get; set; }

        internal DateTime? LocalTransactionEndTime { get; set; }

        internal DateTime? LocalTransactionStartTime { get; set; }

        internal string MerchantId { get; set; }

        internal bool? OneTime { get; set; }

        internal string OrderId { get; set; }

        internal string PaymentMethodKey { get; set; }

        internal IEnumerable<PaymentMethodType> PaymentTypes { get; set; }

        internal string ReferenceNumber { get; set; }

        internal IEnumerable<TransactionType> TransactionType { get; set; }

        internal decimal? SettlementAmount { get; set; }

        internal string ScheduleId { get; set; }

        internal string SiteTrace { get; set; }

        internal DateTime? StartDate { get; set; }

        internal DateTime? StartDepositDate { get; set; }

        internal string UniqueDeviceId { get; set; }

        internal string Username { get; set; }

        internal string Timezone { get; set; }

        internal SearchCriteriaBuilder(TransactionReportBuilder<TResult> reportBuilder) {
            _reportBuilder = reportBuilder;
        }

        public SearchCriteriaBuilder<TResult> And<T>(SearchCriteria criteria, T value) {
            SetProperty(criteria.ToString(), value);
            return this;
        }

        public SearchCriteriaBuilder<TResult> And<T>(DataServiceCriteria criteria, T value) {
            SetProperty(criteria.ToString(), value);
            return this;
        }

        public TResult Execute(string configName = "default") {
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
                    var convertedValue = Convert.ChangeType(value, prop.PropertyType.GenericTypeArguments[0]);
                    prop.SetValue(this, convertedValue);
                }
            }
        }
    }
}
