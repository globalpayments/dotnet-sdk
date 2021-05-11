using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Entities {
    public class StoredPaymentMethodSummary {
        public string Id { get; internal set; }
        public DateTime TimeCreated { get; internal set; }
        public string Status { get; internal set; }
        public string Reference { get; internal set; }
        public string Name { get; internal set; }
        public string CardLast4 { get; internal set; }
        public string CardType { get; internal set; }
        public string CardExpMonth { get; internal set; }
        public string CardExpYear { get; internal set; }
    }
}
