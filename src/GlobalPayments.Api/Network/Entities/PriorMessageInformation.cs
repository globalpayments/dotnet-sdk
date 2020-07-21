using GlobalPayments.Api.Utils;
using System;

namespace GlobalPayments.Api.Network.Entities {
    public class PriorMessageInformation {
        private string cardType = "    ";
        public string ResponseTime { get; set; } = "999";
        public string FunctionCode { get; set; } = "000";
        public string ProcessingCode { get; set; } = "000000";
        public string MessageReasonCode { get; set; }
        public string MessageTransactionIndicator { get; set; } = "0000";
        public string SystemTraceAuditNumber { get; set; } = "000000";
        
        public string GetCardType() {
            return StringUtils.PadRight(cardType,4,' ');
        }

        public void SetCardType(string cardType) {
            this.cardType = cardType;
        }
    }
}
