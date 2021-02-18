using GlobalPayments.Api.Entities.Enums;
using System;

namespace GlobalPayments.Api.Entities.Billing {
    /// <summary>
    /// Represents a bill to be paid in a transaction.
    /// Consists of a type and one to four identifiers.
    /// </summary>
    public class Bill {
        /// <summary>
        /// The name of the bill type
        /// </summary>
        public string BillType { get; set; }

        /// <summary>
        /// The first bill identifier
        /// </summary>
        public string Identifier1 { get; set; }
        /// <summary>
        /// The second identifier
        /// </summary>
        public string Identifier2 { get; set; }
        /// <summary>
        /// The third identifier
        /// </summary>
        public string Identifier3 { get; set; }
        /// <summary>
        /// The fourth identifier
        /// </summary>
        public string Identifier4 { get; set; }
        /// <summary>
        /// The amount to apply to the bill
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// The Customer information for the bill
        /// </summary>
        public Customer Customer { get; set; }
        /// <summary>
        /// The Presentment Status of the bill
        /// </summary>
        public BillPresentment BillPresentment { get; set; }
        /// <summary>
        /// The date the bill is due
        /// </summary>
        public DateTime DueDate { get; set; }

        //public AuthorizationBuilder Pay() {
        //    return BillPayService.PayBill(this);
        //}
    }
}
