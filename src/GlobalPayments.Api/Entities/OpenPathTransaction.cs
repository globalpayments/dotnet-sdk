using GlobalPayments.Api.Builders;
using GlobalPayments.Api.PaymentMethods;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Entities
{
    public class OpenPathTransaction
    {
        public string OpenPathApiKey { get; set; }
        public AccountType? AccountType { get; set; }
        public string Alias { get; set; }
        public bool AllowDuplicates { get; set; }
        public bool AllowPartialAuth { get; set; }
        public decimal? Amount { get; set; }
        public decimal? AuthAmount { get; set; }
        public AutoSubstantiation AutoSubstantiation { get; set; }
        public InquiryType? BalanceInquiryType { get; set; }
        public Address BillingAddress { get; set; }
        public decimal? CashBackAmount { get; set; }
        public EmvChipCondition? ChipCondition { get; set; }
        public string ClientTransactionId { get; set; }
        public string Currency { get; set; }
        public string CustomerId { get; set; }
        public string CustomerIpAddress { get; set; }
        public string Cvn { get; set; }
        public string Description { get; set; }
        public string DynamicDescriptor { get; set; }
        public EcommerceInfo EcommerceInfo { get; set; }
        public decimal? Gratuity { get; set; }
        public decimal? ConvenienceAmt { get; set; }
        public decimal? ShippingAmt { get; set; }
        public HostedPaymentData HostedPaymentData { get; set; }
        public string InvoiceNumber { get; set; }
        public bool Level2Request { get; set; }
        public string MessageAuthenticationCode { get; set; }
        public string OfflineAuthCode { get; set; }
        public bool OneTimePayment { get; set; }
        public string OrderId { get; set; }
        public string PosSequenceNumber { get; set; }
        public string ProductId { get; set; }
        public RecurringSequence? RecurringSequence { get; set; }
        public RecurringType? RecurringType { get; set; }
        public bool RequestMultiUseToken { get; set; }
        public GiftCard ReplacementCard { get; set; }
        public ReversalReasonCode? ReversalReasonCode { get; set; }
        public string ScheduleId { get; set; }
        public Address ShippingAddress { get; set; }
        public string TagData { get; set; }
        public string Timestamp { get; set; }
        public TransactionType TransactionType { get; set; }
        public TransactionModifier TransactionModifier { get; set; }
        public IPaymentMethod PaymentMethod { get; set; }

        public OpenPathTransaction MapData(AuthorizationBuilder authorizationBuilder)
        {
            return new OpenPathTransaction
            {
                AccountType = authorizationBuilder.AccountType,
                Alias = authorizationBuilder.Alias,
                AllowDuplicates = authorizationBuilder.AllowDuplicates,
                AllowPartialAuth = authorizationBuilder.AllowPartialAuth,
                Amount = authorizationBuilder.Amount,
                AuthAmount = authorizationBuilder.AuthAmount,
                AutoSubstantiation = authorizationBuilder.AutoSubstantiation,
                BalanceInquiryType = authorizationBuilder.BalanceInquiryType,
                BillingAddress = authorizationBuilder.BillingAddress,
                CashBackAmount = authorizationBuilder.CashBackAmount,
                ChipCondition = authorizationBuilder.ChipCondition,
                ClientTransactionId = authorizationBuilder.ClientTransactionId,
                ConvenienceAmt = authorizationBuilder.ConvenienceAmt,
                Currency = authorizationBuilder.Currency,
                CustomerId = authorizationBuilder.CustomerId,
                CustomerIpAddress = authorizationBuilder.CustomerIpAddress,
                Cvn = authorizationBuilder.Cvn,
                Description = authorizationBuilder.Description,
                DynamicDescriptor = authorizationBuilder.DynamicDescriptor,
                EcommerceInfo = authorizationBuilder.EcommerceInfo,
                Gratuity = authorizationBuilder.Gratuity,
                HostedPaymentData = authorizationBuilder.HostedPaymentData,
                InvoiceNumber = authorizationBuilder.InvoiceNumber,
                Level2Request = authorizationBuilder.Level2Request,
                MessageAuthenticationCode = authorizationBuilder.MessageAuthenticationCode,
                OfflineAuthCode = authorizationBuilder.OfflineAuthCode,
                OneTimePayment = authorizationBuilder.OneTimePayment,
                OrderId = authorizationBuilder.OrderId,
                PosSequenceNumber = authorizationBuilder.PosSequenceNumber,
                ProductId = authorizationBuilder.ProductId,
                RecurringSequence = authorizationBuilder.RecurringSequence,
                RecurringType = authorizationBuilder.RecurringType,
                ReplacementCard = authorizationBuilder.ReplacementCard,
                RequestMultiUseToken = authorizationBuilder.RequestMultiUseToken,
                ReversalReasonCode = authorizationBuilder.ReversalReasonCode,
                ScheduleId = authorizationBuilder.ScheduleId,
                ShippingAddress = authorizationBuilder.ShippingAddress,
                ShippingAmt = authorizationBuilder.ShippingAmt,
                TagData = authorizationBuilder.TagData,
                Timestamp = authorizationBuilder.Timestamp,
                PaymentMethod = authorizationBuilder.PaymentMethod,
                TransactionModifier = authorizationBuilder.TransactionModifier,
                TransactionType = authorizationBuilder.TransactionType
            };
        }
    }
}
