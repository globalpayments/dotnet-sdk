using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Enums;
using GlobalPayments.Api.PaymentMethods;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Builders
{
    public class BankPaymentBuilder : BaseBuilder<Transaction> {
        internal decimal? Amount { get; set; }
        internal IPaymentMethod PaymentMethod { get; set; }
        internal string Currency { get; set; }
        internal TransactionType TransactionType { get; set; }
        internal TransactionModifier TransactionModifier { get; set; } = TransactionModifier.None;
        internal string Description { get; set; }
        internal string OrderId { get; set; }
        internal string Timestamp { get; set; }
        internal string RemittanceReferenceValue { get; set; }
        internal RemittanceReferenceType? RemittanceReferenceType { get; set; }

        public BankPaymentBuilder(TransactionType transactionType, IPaymentMethod paymentMethod) {
            this.TransactionType = transactionType;
            if(paymentMethod != null) {
                this.WithPaymentMethod(paymentMethod);
            }
        }

        public override Transaction Execute(string configName = "default") {
            base.Execute(configName);

            var client = ServicesContainer.Instance.GetOpenBanking(configName);

            return client.ProcessOpenBanking(this);
        }

        public string Serialize(string configName = "default") {
            TransactionModifier = TransactionModifier.HostedRequest;
            base.Execute();

            var client = ServicesContainer.Instance.GetOpenBanking(configName);

            if (client.SupportsHostedPayments) {
                return client.SerializeRequest(this);
            }

            throw new UnsupportedTransactionException("Your current gateway does not support hosted payments.");
        }

        public BankPaymentBuilder WithCurrency(string value) {
            Currency = value;
            return this;
        }

        public BankPaymentBuilder WithAmount(decimal? value) {
            Amount = value;
            return this;
        }

        public BankPaymentBuilder WithDescription(string value)
        {
            Description = value;
            return this;
        }

        public BankPaymentBuilder WithPaymentMethod(IPaymentMethod value)
        {
            PaymentMethod = value;
            return this;
        }

        public BankPaymentBuilder WithOrderId(string value) {
            OrderId = value;
            return this;
        }

        public BankPaymentBuilder WithModifier(TransactionModifier value) {
            TransactionModifier = value;
            return this;
        }
                
        public BankPaymentBuilder WithTimeStamp(string value) {
            Timestamp = value;
            return this;
        }

        public BankPaymentBuilder WithRemittanceReference(RemittanceReferenceType remittanceReferenceType, string remittanceReferenceValue) {
            RemittanceReferenceType = remittanceReferenceType;
            RemittanceReferenceValue = remittanceReferenceValue;

            return this;
        }

        public void setupValidations() {
            Validations.For(TransactionType.Sale)
                .Check(() => PaymentMethod).IsNotNull()
                .Check(() => Amount).IsNotNull()
                .Check(() => Currency).IsNotNull();
        }
    }
}
