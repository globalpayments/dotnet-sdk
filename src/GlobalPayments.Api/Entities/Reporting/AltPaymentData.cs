using System;
using System.Collections.Generic;

namespace GlobalPayments.Api.Entities {
    public class AltPaymentData {
        public string Status { get; set; }
        public string StatusMessage { get; set; }
        public string BuyerEmailAddress { get; set; }
        public DateTime? StateDate { get; set; }
        public List<AltPaymentProcessorInfo> ProcessorResponseInfo { get; set; }
    }

    public class AltPaymentProcessorInfo {
        public string Code { get; set; }
        public string Message { get; set; }
        public string Type { get; set; }
    }
}
