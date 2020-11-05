using System;

namespace GlobalPayments.Api.Terminals.HPA {
    public enum MessageFormat {
        HPA,
        Visa2nd
    }

    internal class HPA_MSG_ID {
        public const string LANE_OPEN = "LaneOpen";
        public const string LANE_CLOSE = "LaneClose";
        public const string RESET = "Reset";
        public const string REBOOT = "Reboot";
        public const string BATCH_CLOSE = "BatchClose";
        public const string GET_BATCH_REPORT = "GetBatchReport";
        public const string CREDIT_SALE = "Sale";
        public const string CREDIT_REFUND = "Refund";
        public const string CREDIT_VOID = "Void";
        public const string CARD_VERIFY = "CardVerify";
        public const string CREDIT_AUTH = "CreditAuth";
        public const string BALANCE = "BalanceInquiry";
        public const string ADD_VALUE = "AddValue";
        public const string TIP_ADJUST = "TipAdjust";
        public const string GET_INFO_REPORT = "GetAppInfoReport";
        public const string CAPTURE = "CreditAuthComplete";
        public const string SIGNATURE_FORM = "SIGNATUREFORM";
        public const string STARTCARD = "StartCard";
        public const string LINEITEM = "LineItem";
        public const string GETPARAMETERREPORT = "GetParameterReport";
        public const string SETPARAMETERREPORT = "SetParameter";
        public const string ENDOFDAY = "EOD";
        public const string REVERSAL = "Reversal";
        public const string EMVOFFLINEDECLINE = "EMVOfflineDecline";
        public const string TRANSACTIONCERTIFICATE = "TransactionCertificate";
        public const string ATTACHMENT = "Attachment";
        public const string SENDSAF = "SendSAF";
        public const string HEARTBEAT = "Heartbeat";
        public const string EMV_PARAMETER_DOWNLOAD = "EMVPDL";
        public const string EMVCRYPTOGRAMTYPE = "EMVTC";
        public const string SENDFILE = "SendFile";
        public const string NOTIFICATION = "Notification";
    }
}
