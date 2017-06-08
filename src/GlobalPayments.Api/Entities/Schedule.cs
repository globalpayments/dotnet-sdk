using System;
using GlobalPayments.Api.Entities;

namespace GlobalPayments.Api.Entities {
    public class Schedule : RecurringEntity<Schedule> {
        public decimal? Amount { get; set; }
        public DateTime? CancellationDate { get; set; }
        public string Currency { get; set; }
        public string CustomerKey { get; set; }
        public string Description { get; set; }
        public int DeviceId { get; set; }
        public bool EmailNotification { get; set; }
        public EmailReceipt EmailReceipt { get; set; }
        public DateTime? EndDate { get; set; }
        public string Frequency { get; set; }
        public bool HasStarted { get; set; }
        public string InvoiceNumber { get; set; }
        public string Name { get; set; }
        public DateTime? NextProcessingDate { get; set; }
        public int? NumberOfPayments { get; set; }
        public string PoNumber { get; set; }
        public string PaymentKey { get; set; }
        public PaymentSchedule PaymentSchedule { get; set; }
        public int? ReprocessingCount { get; set; }
        public DateTime? StartDate { get; set; }
        public string Status { get; set; }
        public decimal? TaxAmount { get; set; }
        public decimal? TotalAmount {
            get {
                return Amount + TaxAmount;
            }
        }

        public Schedule WithAmount(decimal? value) {
            Amount = value; return this;
        }
        public Schedule WithCurrency(string value) {
            Currency = value;
            return this;
        }
        public Schedule WithCustomerKey(string value) {
            CustomerKey = value; return this;
        }
        public Schedule WithDescription(string value) {
            Description = value;
            return this;
        }
        public Schedule WithDeviceId(int value) {
            DeviceId = value;
            return this;
        }
        public Schedule WithEmailNotification(bool value) {
            EmailNotification = value;
            return this;
        }
        public Schedule WithEmailReceipt(EmailReceipt value) {
            EmailReceipt = value;
            return this;
        }
        public Schedule WithEndDate(DateTime? value) {
            EndDate = value;
            return this;
        }
        public Schedule WithFrequency(string value) {
            Frequency = value;
            return this;
        }
        public Schedule WithInvoiceNumber(string value) {
            InvoiceNumber = value;
            return this;
        }
        public Schedule WithName(string value) {
            Name = value;
            return this;
        }
        public Schedule WithNumberOfPayments(int? value) {
            NumberOfPayments = value;
            return this;
        }
        public Schedule WithPoNumber(string value) {
            PoNumber = value;
            return this;
        }
        public Schedule WithPaymentKey(string value) {
            PaymentKey = value;
            return this;
        }
        public Schedule WithPaymentSchedule(PaymentSchedule value) {
            PaymentSchedule = value;
            return this;
        }
        public Schedule WithReprocessingCount(int? value) {
            ReprocessingCount = value;
            return this;
        }
        public Schedule WithStartDate(DateTime? value) {
            StartDate = value;
            return this;
        }
        public Schedule WithStatus(string value) {
            Status = value;
            return this;
        }
        public Schedule WithTaxAmount(decimal? value) {
            TaxAmount = value;
            return this;
        }

        public Schedule() { }
        public Schedule(string customerKey, string paymentKey) {
            CustomerKey = customerKey;
            PaymentKey = paymentKey;
        }
    }
}
