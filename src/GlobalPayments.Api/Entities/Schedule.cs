using System;

namespace GlobalPayments.Api.Entities {
    /// <summary>
    /// A recurring schedule record.
    /// </summary>
    public class Schedule : RecurringEntity<Schedule> {
        /// <summary>
        /// The schedule's amount
        /// </summary>
        public decimal? Amount { get; set; }

        /// <summary>
        /// The date/time the schedule was cancelled.
        /// </summary>
        public DateTime? CancellationDate { get; set; }

        /// <summary>
        /// The schedule's currency.
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// The identifier of the customer associated
        /// with the schedule.
        /// </summary>
        public string CustomerKey { get; set; }

        /// <summary>
        /// The description of the schedule.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The device ID associated with a schedule's
        /// transactions.
        /// </summary>
        public int DeviceId { get; set; }

        /// <summary>
        /// Indicates if email notifications should be sent.
        /// </summary>
        public bool EmailNotification { get; set; }

        /// <summary>
        /// Indicates when email notifications should be sent.
        /// </summary>
        public EmailReceipt EmailReceipt { get; set; }

        /// <summary>
        /// The end date of a schedule, if any.
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// The schedule's frequency.
        /// </summary>
        /// <seealso>ScheduleFrequency</seealso>
        public string Frequency { get; set; }

        /// <summary>
        /// Indicates if the schedule has started processing.
        /// </summary>
        public bool HasStarted { get; set; }

        /// <summary>
        /// The invoice number associated with the schedule.
        /// </summary>
        public string InvoiceNumber { get; set; }

        /// <summary>
        /// The schedule's name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The date/time when the schedule should process next.
        /// </summary>
        public DateTime? NextProcessingDate { get; set; }

        /// <summary>
        /// The number of payments made to date on the schedule.
        /// </summary>
        public int? NumberOfPayments { get; set; }

        /// <summary>
        /// The purchase order (PO) number associated with the schedule.
        /// </summary>
        public string PoNumber { get; set; }

        /// <summary>
        /// The identifier of the payment method associated with
        /// the schedule.
        /// </summary>
        public string PaymentKey { get; set; }

        /// <summary>
        /// Indicates when in the month a recurring schedule should run.
        /// </summary>
        public PaymentSchedule PaymentSchedule { get; set; }

        /// <summary>
        /// The number of times a failed schedule payment should be
        /// reprocessed.
        /// </summary>
        public int? ReprocessingCount { get; set; }

        /// <summary>
        /// The start date of a schedule.
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// The schedule's status.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// The schedule's tax amount.
        /// </summary>
        public decimal? TaxAmount { get; set; }

        /// <summary>
        /// The total amount for the schedule (`Amount` + `TaxAmount`).
        /// </summary>
        public decimal? TotalAmount {
            get {
                return Amount + TaxAmount;
            }
        }

        /// <summary>
        /// Sets the schedule's amount.
        /// </summary>
        /// <param name="value">The amount</param>
        /// <returns>Schedule</returns>
        public Schedule WithAmount(decimal? value) {
            Amount = value; return this;
        }

        /// <summary>
        /// Sets the schedule's currency.
        /// </summary>
        /// <param name="value">The currency</param>
        /// <returns>Schedule</returns>
        public Schedule WithCurrency(string value) {
            Currency = value;
            return this;
        }

        /// <summary>
        /// Sets the schedule's customer.
        /// </summary>
        /// <param name="value">The customer's key</param>
        /// <returns>Schedule</returns>
        public Schedule WithCustomerKey(string value) {
            CustomerKey = value; return this;
        }

        /// <summary>
        /// Sets the schedule's description.
        /// </summary>
        /// <param name="value">The description</param>
        /// <returns>Schedule</returns>
        public Schedule WithDescription(string value) {
            Description = value;
            return this;
        }

        /// <summary>
        /// Sets the schedule's device ID.
        /// </summary>
        /// <param name="value">The device ID</param>
        /// <returns>Schedule</returns>
        public Schedule WithDeviceId(int value) {
            DeviceId = value;
            return this;
        }

        /// <summary>
        /// Sets whether the schedule should send email notifications.
        /// </summary>
        /// <param name="value">The email notification flag</param>
        /// <returns>Schedule</returns>
        public Schedule WithEmailNotification(bool value) {
            EmailNotification = value;
            return this;
        }

        /// <summary>
        /// Sets when the schedule should email receipts.
        /// </summary>
        /// <param name="value">When the schedule should email receipts</param>
        /// <returns>Schedule</returns>
        public Schedule WithEmailReceipt(EmailReceipt value) {
            EmailReceipt = value;
            return this;
        }

        /// <summary>
        /// Sets the schedule's end date.
        /// </summary>
        /// <param name="value">The end date</param>
        /// <returns>Schedule</returns>
        public Schedule WithEndDate(DateTime? value) {
            EndDate = value;
            return this;
        }

        /// <summary>
        /// Sets the schedule's frequency.
        /// </summary>
        /// <param name="value">The frequency</param>
        /// <returns>Schedule</returns>
        public Schedule WithFrequency(string value) {
            Frequency = value;
            return this;
        }

        /// <summary>
        /// Sets the schedule's invoice number.
        /// </summary>
        /// <param name="value">The invoice number</param>
        /// <returns>Schedule</returns>
        public Schedule WithInvoiceNumber(string value) {
            InvoiceNumber = value;
            return this;
        }

        /// <summary>
        /// Sets the schedule's name.
        /// </summary>
        /// <param name="value">The name</param>
        /// <returns>Schedule</returns>
        public Schedule WithName(string value) {
            Name = value;
            return this;
        }

        /// <summary>
        /// Sets the schedule's number of payments.
        /// </summary>
        /// <param name="value">The number of payments</param>
        /// <returns>Schedule</returns>
        public Schedule WithNumberOfPayments(int? value) {
            NumberOfPayments = value;
            return this;
        }

        /// <summary>
        /// Sets the schedule's purchase order (PO) number.
        /// </summary>
        /// <param name="value">The purchase order (PO) number</param>
        /// <returns>Schedule</returns>
        public Schedule WithPoNumber(string value) {
            PoNumber = value;
            return this;
        }

        /// <summary>
        /// Sets the schedule's payment method.
        /// </summary>
        /// <param name="value">The payment method's key</param>
        /// <returns>Schedule</returns>
        public Schedule WithPaymentKey(string value) {
            PaymentKey = value;
            return this;
        }

        /// <summary>
        /// Sets the schedule's recurring schedule.
        /// </summary>
        /// <param name="value">The recurring schedule</param>
        /// <returns>Schedule</returns>
        public Schedule WithPaymentSchedule(PaymentSchedule value) {
            PaymentSchedule = value;
            return this;
        }

        /// <summary>
        /// Sets the schedule's reprocessing count.
        /// </summary>
        /// <param name="value">The reprocessing count</param>
        /// <returns>Schedule</returns>
        public Schedule WithReprocessingCount(int? value) {
            ReprocessingCount = value;
            return this;
        }

        /// <summary>
        /// Sets the schedule's start date.
        /// </summary>
        /// <param name="value">The start date</param>
        /// <returns>Schedule</returns>
        public Schedule WithStartDate(DateTime? value) {
            StartDate = value;
            return this;
        }

        /// <summary>
        /// Sets the schedule's status.
        /// </summary>
        /// <param name="value">The new status</param>
        /// <returns>Schedule</returns>
        public Schedule WithStatus(string value) {
            Status = value;
            return this;
        }

        /// <summary>
        /// Sets the schedule's tax amount.
        /// </summary>
        /// <param name="value">The tax amount</param>
        /// <returns>Schedule</returns>
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
