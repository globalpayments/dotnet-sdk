using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Entities
{
    public class OpenPathTransactionUpdate {
        public string ApiKey { get; set; }
        public string PaymentTransactionId { get; set; }
        public long OpenPathTransactionId { get; set; }
        public string InvoiceNumber { get; set; }
        public string ResponseMessage { get; set; }
    }
}
