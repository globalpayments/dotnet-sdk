
using GlobalPayments.Api.Utils;
using System;

namespace GlobalPayments.Api.Terminals.Ingenico {
    internal class INGENICO_REQ_CMD {
        // Request Transactions
        public const string AUTHCODE = "AUTHCODE={0}";
        public const string CASHBACK = "CASHB={0};";

        // Request Commands
        /**
         * REQUEST_MESSAGE is hard-coded in order to fulfill the request message frame 3
         * and values in here are ignored in the terminal.
         */
        public const string REQUEST_MESSAGE = "0100000001100826EXT0100000A010B010";
        public const string CANCEL = "CMD=CANCEL";
        public const string DUPLICATE = "CMD=DUPLIC";
        public const string REVERSE = "CMD=REVERSE";
        public const string REVERSE_WITH_ID = "CMD=REV{0}";
        public const string TABLE_WITH_ID = "CMD=ID{0}";

        // Terminal Management Commands
        public const string STATE = "CMD=STATE";
        public const string PID = "CMD=PID";
        public const string LOGON = "CMD=LOGON";
        public const string RESET = "CMD=RESET";
        public const string CALLTMS = "CMD=CALLTMS";

        // Request Report
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
        public const int RAW_RESPONSE_LENGTH = 80;
        public const string MGMT_SCOPE = "root\\CIMV2";
        public const string MGMT_QUERY = "SELECT * FROM Win32_PnPEntity WHERE ClassGuid=\"{4d36e978-e325-11ce-bfc1-08002be10318}\"";
        public const int MSG_FRAME_TWO_LEN = 80;
        public const string XML_TAG = "<?xml";
        public const string ADDITIONAL_MSG = "ADDITIONAL_DATA";
        public const string TRANSFER_DATA = "DATA_TRANSFER";
        public const string TRANSACTION_XML = "CREDIT_CARD_RECEIPT";
        public const string EPOS_TABLE_LIST = "EPOS_TABLE_LIST";
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

    internal static class PAYATTABLE_RESP {
        public readonly static string PAT_EPOS_NUMBER = "00";
        public readonly static string PAT_STATUS = "0";
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
        CANCEL_FAILED = 7
    }

    public enum DynamicCurrencyStatus {
        CONVERSION_APPLIED = 1,
        REJECTED = 0
    }

    public enum TransactionSubTypes {
        [Description("S")]
        SPLIT_SALE_TXN,

        [Description("D")]
        DCC_TXN,

        [Description("R")]
        REFERRAL_RESULT
    }

    public enum TerminalStatus {
        NOT_READY = 0,
        READY = 1
    }

    public enum SalesMode {
        STANDARD_SALE_MODE = 0,
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

    public enum TaxFreeType {
        CREDIT = 0,
        CASH = 1
    }

    // Codes in Response field for TLV format
    public enum RepFieldCode {
        AuthCode = 67, // C
        CashbackAmount = 90, // Z
        GratuityAmount = 89, // Y
        FinalTransactionAmount = 77, // M
        AvailableAmount = 65, // A
        DccCurrency = 85, // U
        DccConvertedAmount = 79, // O
        PaymentMethod = 80, // P
        TransactionSubType = 84, // T
        SplitSalePaidAmount = 83, // S
        DccOperationStatus = 68, // D
    }

    public enum StateResponseCode {
        Status = 83, // S
        AppVersionNumber = 86, // V
        HandsetNumber = 72, // H
        TerminalId = 84, // T
    }

    public enum TLVFormat {
        /// <summary>
        /// Format for transaction request.
        /// </summary>
        Standard,

        /// <summary>
        /// Format for State command request.
        /// </summary>
        State,

        /// <summary>
        /// Format ffor PayAtTable parsing
        /// </summary>
        PayAtTable
    }

    public enum ParseFormat {
        /// <summary>
        /// For Transaction response parsing format
        /// </summary>
        Transaction = 0,

        /// <summary>
        /// For State Command response parsing format
        /// </summary>
        State = 1,

        /// <summary>
        /// For PID Command response parsing format
        /// </summary>
        PID = 2,

        /// <summary>
        /// For Pay@Table functionalities in terminal
        /// </summary>
        PayAtTableRequest,
        
        /// <summary>
        /// For XML commands response format
        /// </summary>
        XML
    }

    /// <summary>
    /// Type of request message from terminal during Pay@Table mode.
    /// </summary>
    public enum PATRequestType {
        /// <summary>
        /// Indicates a Table Lock
        /// </summary>
        TableLock = 1,

        /// <summary>
        /// Indicates a Table Unlock
        /// </summary>
        TableUnlock = 2,

        /// <summary>
        /// Indicates a Receipt for table 
        /// </summary>
        TableReceipt = 3,

        /// <summary>
        /// Indicates a List of Table.
        /// </summary>
        TableList = 4,

        /// <summary>
        /// Indicates a Transaction Outcome request
        /// </summary>
        TransactionOutcome,

        /// <summary>
        /// Indicates a Additional Message XML request
        /// </summary>
        AdditionalMessage,

        /// <summary>
        /// Indicates a Transfer of Data request
        /// </summary>
        TransferData,

        /// <summary>
        /// Indicates a Split Sale Report XML request
        /// </summary>
        SplitSaleReport,

        /// <summary>
        /// Indicates a Ticket XML request
        /// </summary>
        Ticket,

        /// <summary>
        /// Indicates a End of Day Report XML request
        /// </summary>
        EndOfDayReport
    }

    /// <summary>
    /// Confirmation options
    /// </summary>
    public enum PATResponseType {
        /// <summary>
        /// Positive confirmation
        /// </summary>
        CONF_OK,

        /// <summary>
        /// Negative confirmation
        /// </summary>
        CONF_NOK
    }

    /// <summary>
    /// Indicates if the EPOS want to uses the additional Message
    /// </summary>
    public enum PATPaymentMode {
        NO_ADDITIONAL = 0,

        USE_ADDITIONAL = 1
    }

    public enum PATPrivateDataCode {
        WaiterId = 79,
        TableId = 76,
        TerminalId = 84,
        TerminalCurrency = 67
    }
}