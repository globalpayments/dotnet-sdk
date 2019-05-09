using System;

namespace GlobalPayments.Api.Terminals.PAX {
    internal class PAX_MSG_ID {
        // ADMIN REQUESTS
        public const string A00_INITIALIZE = "A00";
        public const string A02_GET_VARIABLE = "A02";
        public const string A04_SET_VARIABLE = "A04";
        public const string A06_SHOW_DIALOG = "A06";
        public const string A08_GET_SIGNATURE = "A08";
        public const string A10_SHOW_MESSAGE = "A10";
        public const string A12_CLEAR_MESSAGE = "A12";
        public const string A14_CANCEL = "A14";
        public const string A16_RESET = "A16";
        public const string A18_UPDATE_RESOURCE_FILE = "A18";
        public const string A20_DO_SIGNATURE = "A20";
        public const string A22_DELETE_IMAGE = "A22";
        public const string A24_SHOW_MESSAGE_CENTER_ALIGNED = "A24";
        public const string A26_REBOOT = "A26";
        public const string A28_GET_PIN_BLOCK = "A28";
        public const string A30_INPUT_ACCOUNT = "A30";
        public const string A32_RESET_MSR = "A32";
        public const string A36_INPUT_TEXT = "A36";
        public const string A38_CHECK_FILE = "A38";
        public const string A40_AUTHORIZE_CARD = "A40";
        public const string A42_COMPLETE_ONLINE_EMV = "A42";
        public const string A44_REMOVE_CARD = "A44";
        public const string A46_GET_EMV_TLV_DATA = "A46";
        public const string A48_SET_EMV_TLV_DATA = "A48";
        public const string A50_INPUT_ACCOUNT_WITH_EMV = "A50";
        public const string A52_COMPLETE_CONTACTLESS_EMV = "A52";
        public const string A54_SET_SAF_PARAMETERS = "A54";
        public const string A56_SHOW_TEXTBOX = "A56";

        // TRANSACTION REQUESTS
        public const string T00_DO_CREDIT = "T00";
        public const string T02_DO_DEBIT = "T02";
        public const string T04_DO_EBT = "T04";
        public const string T06_DO_GIFT = "T06";
        public const string T08_DO_LOYALTY = "T08";
        public const string T10_DO_CASH = "T10";
        public const string T12_DO_CHECK = "T12";

        // BATCH REQUESTS
        public const string B00_BATCH_CLOSE = "B00";
        public const string B02_FORCE_BATCH_CLOSE = "B02";
        public const string B04_BATCH_CLEAR = "B04";
        public const string B06_PURGE_BATCH = "B06";
        public const string B08_SAF_UPLOAD = "B08";
        public const string B10_DELETE_SAF_FILE = "B10";

        // REPORT REQUESTS
        public const string R00_LOCAL_TOTAL_REPORT = "R00";
        public const string R02_LOCAL_DETAIL_REPORT = "R02";
        public const string R04_LOCAL_FAILED_REPORT = "R04";
        public const string R06_HOST_REPORT = "R06";
        public const string R08_HISTORY_REPORT = "R08";
        public const string R10_SAF_SUMMARY_REPORT = "R10";

        // ADMIN RESPONSES
        public const string A01_RSP_INITIALIZE = "A01";
        public const string A03_RSP_GET_VARIABLE = "A03";
        public const string A05_RSP_SET_VARIABLE = "A05";
        public const string A07_RSP_SHOW_DIALOG = "A07";
        public const string A09_RSP_GET_SIGNATURE = "A09";
        public const string A11_RSP_SHOW_MESSAGE = "A11";
        public const string A13_RSP_CLEAR_MESSAGE = "A13";
        public const string A17_RSP_RESET = "A17";
        public const string A19_RSP_UPDATE_RESOURCE_FILE = "A19";
        public const string A21_RSP_DO_SIGNATURE = "A21";
        public const string A23_RSP_DELETE_IMAGE = "A23";
        public const string A25_RSP_SHOW_MESSAGE_CENTER_ALIGNED = "A25";
        public const string A27_RSP_REBOOT = "A27";
        public const string A29_RSP_GET_PIN_BLOCK = "A29";
        public const string A31_RSP_INPUT_ACCOUNT = "A31";
        public const string A33_RSP_RESET_MSR = "A33";
        public const string A35_RSP_REPORT_STATUS = "A35";
        public const string A37_RSP_INPUT_TEXT = "A37";
        public const string A38_RSP_CHECK_FILE = "A39";
        public const string A41_RSP_AUTHORIZE_CARD = "A41";
        public const string A43_RSP_COMPLETE_ONLINE_EMV = "A43";
        public const string A45_RSP_REMOVE_CARD = "A45";
        public const string A47_RSP_GET_EMV_TLV_DATA = "A47";
        public const string A49_RSP_SET_EMV_TLV_DATA = "A49";
        public const string A51_RSP_INPUT_ACCOUNT_WITH_EMV = "A51";
        public const string A53_RSP_COMPLETE_CONTACTLESS_EMV = "A53";
        public const string A55_RSP_SET_SAF_PARAMETERS = "A55";
        public const string A57_RSP_SHOW_TEXTBOX = "A57";

        // TRANSACTION RESPONSES
        public const string T01_RSP_DO_CREDIT = "T01";
        public const string T03_RSP_DO_DEBIT = "T03";
        public const string T05_RSP_DO_EBT = "T05";
        public const string T07_RSP_DO_GIFT = "T07";
        public const string T09_RSP_DO_LOYALTY = "T09";
        public const string T11_RSP_DO_CASH = "T11";
        public const string T13_RSP_DO_CHECK = "T13";

        // BATCH RESPONSES
        public const string B01_RSP_BATCH_CLOSE = "B01";
        public const string B03_RSP_FORCE_BATCH_CLOSE = "B03";
        public const string B05_RSP_BATCH_CLEAR = "B05";
        public const string B07_RSP_PURGE_BATCH = "B07";
        public const string B09_RSP_SAF_UPLOAD = "B09";
        public const string B11_RSP_DELETE_SAF_FILE = "B11";

        // REPORT RESPONSES
        public const string R01_RSP_LOCAL_TOTAL_REPORT = "R01";
        public const string R03_RSP_LOCAL_DETAIL_REPORT = "R03";
        public const string R05_RSP_LOCAL_FAILED_REPORT = "R05";
        public const string R07_RSP_HOST_REPORT = "R07";
        public const string R09_RSP_HISTORY_REPORT = "R09";
        public const string R11_RSP_SAF_SUMMARY_REPORT = "R11";
    }

    internal class PAX_TXN_TYPE {
        public const string MENU = "00";
        public const string SALE_REDEEM = "01";
        public const string RETURN = "02";
        public const string AUTH = "03";
        public const string POSTAUTH = "04";
        public const string FORCED = "05";
        public const string ADJUST = "06";
        public const string WITHDRAWAL = "07";
        public const string ACTIVATE = "08";
        public const string ISSUE = "09";
        public const string ADD = "10";
        public const string CASHOUT = "11";
        public const string DEACTIVATE = "12";
        public const string REPLACE = "13";
        public const string MERGE = "14";
        public const string REPORTLOST = "15";
        public const string VOID = "16";
        public const string V_SALE = "17";
        public const string V_RTRN = "18";
        public const string V_AUTH = "19";
        public const string V_POST = "20";
        public const string V_FRCD = "21";
        public const string V_WITHDRAW = "22";
        public const string BALANCE = "23";
        public const string VERIFY = "24";
        public const string REACTIVATE = "25";
        public const string FORCED_ISSUE = "26";
        public const string FORCED_ADD = "27";
        public const string UNLOAD = "28";
        public const string RENEW = "29";
        public const string GET_CONVERT_DETAIL = "30";
        public const string CONVERT = "31";
        public const string TOKENIZE = "32";
        public const string REVERSAL = "99";
    }

    internal class PAX_ECOM_MODE {
        public const string MAIL_ORDER = "M";
        public const string TELE_ORDER = "T";
        public const string ECOMMERCE = "E";
    }

    internal class PAX_ECOM_TXN_TYPE {
        public const string SINGLE = "S";
        public const string INSTALLMENT = "I";
        public const string RECURRING = "R";
    }

    internal class PAX_ECOM_SECURE_TYPE {
        public const string SECURE = "S";
        public const string NON_SECURE = "N";
    }

    internal class PAX_EBT_TYPE {
        public const string CASH_BENEFITS = "C";
        public const string FOOD_STAMP = "F";
        public const string VOUCHER = "V";
    }

    internal class PAX_CARD_TYPE {
        public const string VISA = "01";
        public const string MASTERCARD = "02";
        public const string AMEX = "03";
        public const string DiSCOVER = "04";
        public const string DINER_CLUB = "05";
        public const string EN_ROUTE = "06";
        public const string JCB = "07";
        public const string REVOLUTION_CARD = "08";
        public const string VISA_FLEET = "09";
        public const string MASTERCARD_FLEET = "10";
        public const string FLEET_ONE = "11";
        public const string FLEET_WIDE = "12";
        public const string FUEL_MAN = "13";
        public const string GAS_CARD = "14";
        public const string VOYAGER = "15";
        public const string WRIGHT_EXPRESS = "16";
        public const string OTHER = "99";
    }

    internal class EXT_DATA {
        public static string TABLE_NUMBER = "TABLE";
        public static string GUEST_NUMBER = "GUEST";
        public static string SIGNATURE_CAPTURE = "SIGN";
        public static string TICKET_NUMBER = "TICKET";
        public static string HOST_REFERENCE_NUMBER = "HREF";
        public static string TIP_REQUEST = "TIPREQ";
        public static string SIGNATURE_UPLOAD = "SIGNUPLOAD";
        public static string REPORT_STATUS = "REPORTSTATUS";
        public static string TOKEN_REQUEST = "TOKENREQUEST";
        public static string TOKEN = "TOKEN";
        public static string CARD_TYPE = "CARDTYPE";
        public static string CARD_TYPE_BITMAP = "CARDTYPEBITMAP";
        // TODO: This will probably come in a later release
        //public static string PASS_THROUGH_DATA = "PASSTHRUDATA";
        public static string RETURN_REASON = "RETURNREASON";
        public static string ORIGINAL_TRANSACTION_DATE = "ORIGTRANSDATE";
        public static string ORIGINAL_PAN = "ORIGPAN";
        public static string ORIGINAL_EXPIRATION_DATE = "ORIGEXPIRYDATE";
        public static string ODOMETER_READING = "ODOMETER";
        public static string VEHICLE_NUMBER = "VEHICLENO";
        public static string JOB_NUMBER = "JOBNO";
        public static string DRIVER_ID = "DRIVERID";
        public static string EMPLOYEE_NUMBER = "EMPLOYEENO";
        public static string LICENSE_NUMBER = "LICENSENO";
        public static string JOB_ID = "JOBID";
        public static string DEPARTMENT_NUMBER = "DEPARTMENTNO";
        public static string CUSTOMER_DATA = "CUSTOMERDATA";
        public static string USER_ID = "USERID";
        public static string VEHECLE_ID = "VEHICLEID";
        public static string APPLICATION_PREFERRED_NAME = "APPPN";
        public static string APPLICATION_LABEL = "APPLAB";
        public static string APPLICATION_ID = "AID";
        public static string CUSTOMER_VERIFICATION_METHOD = "CVM";
        public static string TRANSACTION_CERTIFICATE = "TC";
        public static string CARD_BIN = "CARDBIN";
        public static string SIGNATURE_STATUS = "SIGNSTATUS";
        public static string TERMINAL_VERIFICATION_RESULTS = "TVR";
        public static string MERCHANT_ID = "MM_ID";
        public static string MERCHANT_NAME = "MM_NAME";
    }

    internal class PAX_CHECK_SALE_TYPE {
        public static string VERIFICATION = "V";
        public static string CONVERSION = "C";
        public static string GUARANTEE = "G";
    }

    internal class PAX_CHECK_TYPE {
        public static string PERSONAL = "P";
        public static string BUSINESS = "B";
        public static string GOVERNMENT = "G";
        public static string PAYROLL = "R";
        public static string TWO_PARTY = "T";
    }

    internal class PAX_ID_TYPE {
        public static string DRIVERS_LICENSE = "D";
        public static string SSN = "S";
        public static string MILITARY_ID = "M";
        public static string COURTESY_CARD = "C";
        public static string PROPRIETARY_CARD = "P";
        public static string MILITARY_BASE = "B";
        public static string PASSPORT_NUMBER = "A";
    }

    internal enum EntryMode {
        Manual = 0,
        Swipe,
        Contactless,
        Scanner,
        Chip,
        ChipFallBackSwipe
    }

    public enum TerminalCardType {
        VISA = 01,
        MASTERCARD = 02,
        AMEX = 03,
        DiSCOVER = 04,
        DINER_CLUB = 05,
        EN_ROUTE = 06,
        JCB = 07,
        REVOLUTION_CARD = 08,
        VISA_FLEET = 09,
        MASTERCARD_FLEET = 10,
        FLEET_ONE = 11,
        FLEET_WIDE = 12,
        FUEL_MAN = 13,
        GAS_CARD = 14,
        VOYAGER = 15,
        WRIGHT_EXPRESS = 16,
        OTHER = 99,
    }

    public enum TerminalTransactionType {
        MENU = 00,
        SALE = 01,
        RETURN = 02,
        AUTH = 03,
        POSTAUTH = 04,
        FORCED = 05,
        ADJUST = 06,
        WITHDRAWAL = 07,
        ACTIVATE = 08,
        ISSUE = 09,
        ADD = 10,
        CASHOUT = 11,
        DEACTIVATE = 12,
        REPLACE = 13,
        MERGE = 14,
        REPORTLOST = 15,
        VOID = 16,
        VOID_SALE = 17,
        VOID_RTRN = 18,
        VOID_AUTH = 19,
        VOID_POST = 20,
        VOID_FRCD = 21,
        VOID_WITHDRAW = 22,
        BALANCE = 23,
        VERIFY = 24,
        REACTIVATE = 25,
        FORCED_ISSUE = 26,
        FORCED_ADD = 27,
        UNLOAD = 28,
        RENEW = 29,
        GET_CONVERT_DETAIL = 30,
        CONVERT = 31,
        TOKENIZE = 32,
        REVERSAL = 99,
    }

    public enum PaxSearchCriteria {
        TransactionType,
        CardType,
        RecordNumber,
        TerminalReferenceNumber,
        AuthCode,
        ReferenceNumber,
        MerchantId,
        MerchantName
    }
}
