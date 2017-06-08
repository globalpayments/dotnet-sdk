using System.Collections.Generic;

namespace GlobalPayments.Api.Entities {
    public class HostedPaymentData {
        public bool CustomerExists { get; set; }
        public string CustomerKey { get; set; }
        public string CustomerNumber { get; set; }
        public bool OfferToSaveCard { get; set; }
        public string PaymentKey { get; set; }
        public string ProductId { get; set; }
        public Dictionary<string, string> SupplementaryData { get; set; }

        public HostedPaymentData() {
            SupplementaryData = new Dictionary<string, string>();
        }
    }
}
