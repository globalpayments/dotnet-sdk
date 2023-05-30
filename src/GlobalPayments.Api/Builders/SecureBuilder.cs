using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Builders
{
    public abstract class SecureBuilder<TResult> : BaseBuilder<TResult>
    {
        internal decimal? Amount { get; set; }
        internal string Currency { get; set; }
        internal DateTime? OrderCreateDate { get; set; }
        internal OrderTransactionType? OrderTransactionType { get; set; }
        internal string OrderId { get; set; }
        internal string ReferenceNumber { get; set; }
        internal bool AddressMatchIndicator { get; set; }
        internal Address ShippingAddress { get; set; }
        internal ShippingMethod? ShippingMethod { get; set; }
        internal bool? ShippingNameMatchesCardHolderName { get; set; }
        internal DateTime? ShippingAddressCreateDate { get; set; }
        internal AgeIndicator? ShippingAddressUsageIndicator { get; set; }
        internal decimal GiftCardAmount { get; set; }
        internal int? GiftCardCount { get; set; }
        internal string GiftCardCurrency { get; set; }
        internal string DeliveryEmail { get; set; }
        internal DeliveryTimeFrame? DeliveryTimeframe { get; set; }
        internal DateTime? PreOrderAvailabilityDate { get; set; }
        internal PreOrderIndicator? PreOrderIndicator { get; set; }
        internal ReorderIndicator? ReorderIndicator { get; set; }
        internal string CustomerAccountId { get; set; }
        internal AgeIndicator? AccountAgeIndicator { get; set; }
        internal DateTime? AccountChangeDate { get; set; }
        internal DateTime? AccountCreateDate { get; set; }
        internal AgeIndicator? AccountChangeIndicator { get; set; }
        internal DateTime? PasswordChangeDate { get; set; }
        internal AgeIndicator? PasswordChangeIndicator { get; set; }
        internal Dictionary<PhoneNumberType, PhoneNumber> PhoneList { get; set; }
        internal string HomeCountryCode { get; set; }
        internal string HomeNumber { get; set; }
        internal string WorkCountryCode { get; set; }
        internal string WorkNumber { get; set; }
        internal string MobileCountryCode { get; set; }
        internal string MobileNumber { get; set; }
        internal DateTime? PaymentAccountCreateDate { get; set; }
        internal AgeIndicator? PaymentAgeIndicator { get; set; }
        internal bool? PreviousSuspiciousActivity { get; set; }
        internal int? NumberOfPurchasesInLastSixMonths { get; set; }
        internal int? NumberOfTransactionsInLast24Hours { get; set; }
        internal int? NumberOfAddCardAttemptsInLast24Hours { get; set; }
        internal int? NumberOfTransactionsInLastYear { get; set; }
        internal BrowserData BrowserData { get; set; }
        internal string IdempotencyKey { get; set; }
        internal string PriorAuthenticationData { get; set; }
        internal PriorAuthenticationMethod? PriorAuthenticationMethod { get; set; }
        internal string PriorAuthenticationTransactionId { get; set; }
        internal DateTime? PriorAuthenticationTimestamp { get; set; }
        internal int? MaxNumberOfInstallments { get; set; }
        internal DateTime? RecurringAuthorizationExpiryDate { get; set; }
        internal int? RecurringAuthorizationFrequency { get; set; }
        internal string CustomerAuthenticationData { get; set; }
        internal CustomerAuthenticationMethod? CustomerAuthenticationMethod { get; set; }
        internal DateTime? CustomerAuthenticationTimestamp { get; set; }
        internal AuthenticationSource? AuthenticationSource { get; set; }
        internal IPaymentMethod PaymentMethod { get; set; }
        internal TransactionType TransactionType { get; set; }
        internal Address BillingAddress { get; set; }
       
    }
}
