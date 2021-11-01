using GlobalPayments.Api.Builders;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace GlobalPayments.Api.Entities {
    public enum SearchCriteria {
        AccountName,
        AccountNumberLastFour,
        ActionId,
        ActionType,
        AltPaymentStatus,
        AppName,
        AquirerReferenceNumber,
        AuthCode,
        BankRoutingNumber,
        BatchId,
        BatchSequenceNumber,
        BrandReference,
        BuyerEmailAddress,
        CardBrand,
        CardHolderFirstName,
        CardHolderLastName,
        CardHolderPoNumber,
        CardNumberFirstSix,
        CardNumberLastFour,
        Channel,
        CheckFirstName,
        CheckLastName,
        CheckName,
        CheckNumber,
        ClerkId,
        ClientTransactionId,
        CustomerId,
        DepositStatus,
        DisplayName,
        EndDate,
        FullyCaptured,
        GiftCurrency,
        GiftMaskedAlias,
        HttpResponseCode,
        InvoiceNumber,
        IssuerResult,
        IssuerTransactionId,
        MerchantName,
        Name,
        OneTime,
        PaymentEntryMode,
        PaymentMethodKey,
        PaymentType,
        PaymentMethod,
        ReferenceNumber,
        Resource,
        ResourceStatus,
        ResourceId,
        ResponseCode,
        SettlementAmount,
        ScheduleId,
        SiteTrace,
        StartDate,
        StoredPaymentMethodStatus,
        TokenFirstSix,
        TokenLastFour,
        TransactionStatus,
        DisputeStage,
        DisputeStatus,
        UniqueDeviceId,
        Username,
        Version,
    }

    public enum DataServiceCriteria {
        Amount, // Data Services
        BankAccountNumber, // Data Services
        CaseId, // Data Services
        CardNumberFirstSix, // Data Services
        CardNumberLastFour, // Data Services
        CaseNumber, // Data Services
        Country, //Data Services
        Currency, // Data Services
        DepositReference, // Data Services
        EndBatchDate, // Data Services
        EndDepositDate, // Data Services
        EndLastUpdatedDate, // Data Services
        EndStageDate, // Data Services
        Hierarchy, // Data Services
        LocalTransactionEndTime, // Data Services
        LocalTransactionStartTime, // Data Services
        MerchantId, // Data Services
        OrderId, // Data Services
        StartBatchDate, // Data Services
        StartDepositDate, // Data Services
        StartLastUpdatedDate, // Data Services
        StartStageDate, // Data Services
        StoredPaymentMethodId, //Data Services
        SystemHierarchy, // Data Services
        Timezone // Data Services
    }

    public class SearchCriteriaBuilder<TResult> where TResult : class {
        private TransactionReportBuilder<TResult> _reportBuilder;

        internal string AccountName { get; set; }

        internal string AccountNumberLastFour { get; set; }

        internal string ActionId { get; set; }

        internal string ActionType { get; set; }

        internal string AltPaymentStatus { get; set; }

        internal decimal? Amount { get; set; }

        internal string AppName { get; set; }

        internal string AquirerReferenceNumber { get; set; }

        internal string AuthCode { get; set; }

        internal string BankAccountNumber { get; set; }

        internal string BankRoutingNumber { get; set; }

        internal string BatchId { get; set; }

        internal string BatchSequenceNumber { get; set; }

        internal string BrandReference { get; set; }

        internal string BuyerEmailAddress { get; set; }

        internal string CardBrand { get; set; }

        internal string CardHolderFirstName { get; set; }

        internal string CardHolderLastName { get; set; }

        internal string CardHolderPoNumber { get; set; }

        internal string CardNumberFirstSix { get; set; }

        internal string CardNumberLastFour { get; set; }

        internal string CaseId { get; set; }

        internal string CaseNumber { get; set; }

        internal IEnumerable<CardType> CardTypes { get; set; }

        internal Channel? Channel { get; set; }

        internal string CheckFirstName { get; set; }

        internal string CheckLastName { get; set; }

        internal string CheckName { get; set; }

        internal string CheckNumber { get; set; }

        internal string ClerkId { get; set; }

        internal string ClientTransactionId { get; set; }

        internal string Country { get; set; }

        internal string Currency { get; set; }

        internal string CustomerId { get; set; }

        internal string DepositReference { get; set; }
        
        internal DepositStatus? DepositStatus { get; set; }

        internal string DisplayName { get; set; }

        internal string DisputeId { get; set; }

        internal DisputeStage? DisputeStage { get; set;  }

        internal DisputeStatus? DisputeStatus { get; set; }

        internal DateTime? EndBatchDate { get; set; }

        internal DateTime? EndDate { get; set; }

        internal DateTime? EndDepositDate { get; set; }

        internal DateTime? EndLastUpdatedDate { get; set; }

        internal DateTime? EndStageDate { get; set; }

        internal bool? FullyCaptured { get; set; }

        internal string GiftCurrency { get; set; }

        internal string GiftMaskedAlias { get; set; }

        internal string Hierarchy { get; set; }

        internal string HttpResponseCode { get; set; }

        internal string InvoiceNumber { get; set; }

        internal string IssuerResult { get; set; }

        internal string IssuerTransactionId { get; set; }

        internal DateTime? LocalTransactionEndTime { get; set; }

        internal DateTime? LocalTransactionStartTime { get; set; }

        internal string MerchantId { get; set; }

        internal string MerchantName { get; set; }

        internal string Name { get; set; }

        internal bool? OneTime { get; set; }

        internal string OrderId { get; set; }

        internal PaymentEntryMode? PaymentEntryMode { get; set; }

        internal string PaymentMethodKey { get; set; }

        internal PaymentType? PaymentType { get; set; }

        internal PaymentMethodName? PaymentMethod { get; set; }
        internal IEnumerable<PaymentMethodType> PaymentTypes { get; set; }

        internal string ReferenceNumber { get; set; }

        internal string Resource { get; set; }

        internal string ResourceStatus { get; set; }

        internal string ResourceId { get; set; }

        internal string ResponseCode { get; set; }

        internal IEnumerable<TransactionType> TransactionType { get; set; }

        internal decimal? SettlementAmount { get; set; }

        internal string SettlementDisputeId { get; set; }

        internal string ScheduleId { get; set; }

        internal string SiteTrace { get; set; }

        internal DateTime? StartBatchDate { get; set; }

        internal DateTime? StartDate { get; set; }

        internal DateTime? StartDepositDate { get; set; }

        internal DateTime? StartLastUpdatedDate { get; set; }

        internal DateTime? StartStageDate { get; set; }

        internal string StoredPaymentMethodId { get; set; }

        internal StoredPaymentMethodStatus? StoredPaymentMethodStatus { get; set; }

        internal string SystemHierarchy { get; set; }

        internal string TokenFirstSix { get; set; }

        internal string TokenLastFour { get; set; }

        internal TransactionStatus? TransactionStatus { get; set; }

        internal string UniqueDeviceId { get; set; }

        internal string Username { get; set; }

        internal string Timezone { get; set; }

        internal string Version { get; set; }

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
