namespace GlobalPayments.Api.Terminals.UPA {
    internal class UpaTransType {
        public const string SALE_REDEEM = "Sale";
        public const string Void = "Void";
        public const string Refund = "Refund";
        public const string BalanceInquiry = "BalanceInquiry";
        public const string CardVerify = "CardVerify";
        public const string TipAdjust = "TipAdjust";
        public const string EodProcessing = "EODProcessing";
        public const string CancelTransaction = "CancelTransaction";
        public const string Reboot = "Reboot";
        public const string LineItemDisplay = "LineItemDisplay";
        public const string SendSAF = "SendSAF";
        public const string GetSAFReport = "GetSAFReport";
        public const string GetBatchReport = "GetBatchReport";
        public const string Tokenize = "Tokenize";
        public const string PreAuth = "PreAuth";
        public const string AuthCompletion = "AuthCompletion";
        public const string DeletePreAuth = "DeletePreAuth";
        public const string GetOpenTabDetails = "GetOpenTabDetails";
        public const string StartCardTransaction = "StartCardTransaction";
        public const string DeleteSAF = "DeleteSAF";
        public const string RegisterPOS = "RegisterPOS";
        public const string GetSignature = "GetSignature";

    }
}
