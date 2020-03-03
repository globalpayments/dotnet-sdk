
using System;

namespace GlobalPayments.Api.Terminals.INGENICO {
    internal class INGENICO_REQ_CMD {
        public const string CANCEL = "CMD=CANCEL";
        public const string DUPLICATE = "CMD=DUPLIC";
        public const string REVERSE = "CMD=REVERSE";
        public const string REVERSE_WITH_ID = "CMD=REV{0}";
        public const string REPORT = "0100000001100826EXT0100000A010B010CMD={0}";
        public const string RECEIPT = "0100000001100826EXT0100000A010B010CMD={0}";
    }

    internal class INGENICO_GLOBALS {
        public const string BROADCAST = "BROADCAST CODE";
        public const string CANCEL = "CMD=CANCEL";
        public const string TID_CODE = "TID CODE";
        public const string KEEP_ALIVE_RESPONSE = "<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?> <TID CODE=\"{0}\">OK</TID>";
        public static bool KeepAlive = true;
        public const int IP_PORT = 18101;
    }

    internal static class INGENICO_RESP {
        public readonly static string ACKNOWLEDGE = ((char)ControlCodes.ACK).ToString();
        public readonly static string ENQUIRY = ((char)ControlCodes.ENQ).ToString();
        public readonly static string NOTACKNOWLEDGE = ((char)ControlCodes.NAK).ToString();
        public readonly static string ENDOFTXN = ((char)ControlCodes.EOT).ToString();
        public readonly static string[] XML = { "<CREDIT_CARD_RECEIPT>", "LF" };
        public readonly static string INVALID = "\u0005\u0004";
        public readonly static string ENDXML = "</CREDIT_CARD_RECEIPT>";
    }

    public enum ReceiptType {
        TICKET,
        SPLITR,
        TAXFREE,
        REPORT
    }

    public enum ReportType {
        EOD,
        BANKING,
        XBAL,
        ZBAL
    }

    public enum Environment {
        TEST,
        PRODUCTION
    }
    public enum TransactionStatus {
        SUCCESS = 0,
        REFERRAL = 2,
        CANCELLED_BY_USER = 6,
        FAILED = 7,
        RECEIVED = 9
    }
    public enum ReverseStatus {
        REVERSAL_SUCCESS = 0,
        REVERSAL_FAILED = 7,
        NOTHING_TO_REVERSE = 9
    }
    public enum CancelStatus {
        CANCEL_DONE = 9,
        CANCE_FAILED = 7
    }

    public enum DynamicCurrencyStatus {
        CONVERSION_APPLIED = 1,
        REJECTED = 0
    }
    public enum TransactionSubTypes {
        SPLIT_SALE_TXN = 0x53, // 0x53 byte for 'S'
        DCC_TXN = 0x44, // 0x44 byte for 'D'
        REFERRAL_RESULT = 0x82 // 0x52 byte for 'R'
    }

    public enum ReportTypes {
        BANKING,
        EOD,
        XBAL,
        ZBAL
    }
    public enum TerminalStatus {
        NOT_READY = 0,
        READY = 1
    }
    public enum TerminalModes {
        STANDARD_MODE = 0,
        VENDING_MODE = 1
    }

    public enum PaymentMethod {
        Keyed = 1,
        Swiped = 2,
        Chip = 3,
        Conctactless = 4
    }

    public enum PaymentType {
        Sale = 0,
        Refund = 1,
        CompletionMode = 2,
        PreAuthMode = 3,
        TaxFreeCreditRefund = 4,
        TaxFreeCashRefund = 5,
        AccountVerification = 6,
        ReferralConfirmation = 9
    }
    public enum PaymentMode {
        APPLICATION = 0,
        MAILORDER = 1
    }

    public enum ExtendedDataTags {
        CASHB,
        AUTHCODE,
        TABLE_NUMBER,
        TXN_COMMANDS,
        TXN_COMMANDS_PARAMS
    }
}