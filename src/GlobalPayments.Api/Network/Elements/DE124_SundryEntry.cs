using GlobalPayments.Api.Network.Entities;

namespace GlobalPayments.Api.Network.Elements {
    public class DE124_SundryEntry {
        public DE124_SundryDataTag Tag{ get; set; }
        public string CustomerData{ get; set; }
        public string PrimaryAccountNumber{ get; set; } // DE2
        public DE3_ProcessingCode ProcessingCode{ get; set; } // DE3
        public decimal TransactionAmount{ get; set; } // DE4
        public string SystemTraceAuditNumber{ get; set; } // DE11
        public string TransactionLocalDateTime{ get; set; } // DE12
        public string ExpirationDate{ get; set; } // DE14
        public DE22_PosDataCode PosDataCode{ get; set; } // DE22
        public string FunctionCode{ get; set; } // DE24
        public string MessageReasonCode{ get; set; } //DE25
        public string ApprovalCode{ get; set; } // DE38
        public string BatchNumber{ get; set; } // DE48-4
        public string CardType{ get; set; } // DE48-11
        public string MessageTypeIndicator{ get; set; } // DE56.1
        public string OriginalStan{ get; set; } // DE56.2
        public string OriginalDateTime{ get; set; } // DE56.3
        public DE62_CardIssuerData CardIssuerData{ get; set; } // DE62
        public DE63_ProductData ProductData{ get; set; } // DE63
    }
}
