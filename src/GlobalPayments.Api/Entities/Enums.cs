using System;

namespace GlobalPayments.Api.Entities {
    internal enum AliasAction {
        CREATE,
        ADD,
        DELETE
    }

    public enum AddressType {
        Billing,
        Shipping
    }

    public enum DeviceType {
        PaxS300
    }

    public enum InquiryType {
        FOODSTAMP,
        CASH
    }

    public enum PaymentMethodType {
        Reference = 0,
        Credit = 1 << 1,
        Debit = 1 << 2,
        EBT = 1 << 3,
        Cash = 1 << 4,
        ACH = 1 << 5,
        Gift = 1 << 6,
        Recurring = 1 << 7
    }

    public enum EntryMethod {
        Manual,
        Swipe,
        Proximity
    }

    public enum GiftEntryMethod {
        Swipe,
        Alias,
        Manual
    }

    [Flags]
    public enum TransactionType {
        Decline = 0,
        Verify = 1 << 0,
        Capture = 1 << 1,
        Auth = 1 << 2,
        Refund = 1 << 3,
        Reversal = 1 << 4,
        Sale = 1 << 5,
        Edit = 1 << 6,
        Void = 1 << 7,
        AddValue = 1 << 8,
        Balance = 1 << 9,
        Activate = 1 << 10,
        Alias = 1 << 11,
        Replace = 1 << 12,
        Reward = 1 << 13,
        Deactivate = 1 << 14,
        BatchClose = 1 << 15,
        Create = 1 << 16,
        Delete = 1 << 17,
        BenefitWithdrawal = 1 << 18,
        Fetch = 1 << 19,
        Search = 1 << 20,
        Hold = 1 << 21,
        Release = 1 << 22
    }

    public enum TransactionModifier {
        None = 0,
        Incremental = 1 << 1,
        Additional = 1 << 2,
        Offline = 1 << 3,
        LevelII = 1 << 4,
        FraudDecline = 1 << 5,
        ChipDecline = 1 << 6,
        CashBack = 1 << 7,
        Voucher = 1 << 8,
        Secure3D = 1 << 9,
        HostedRequest = 1 << 10,
        Recurring = 1 << 11
    }

    public enum CvnPresenceIndicator {
        Present = 1,
        Illegible,
        NotOnCard,
        NotRequested
    }

    public enum TaxType {
        NOTUSED,
        SALESTAX,
        TAXEXEMPT
    }

    public enum CurrencyType {
        CURRENCY,
        POINTS
    }

    public enum AccountType {
        CHECKING,
        SAVINGS
    }

    public enum CheckType {
        PERSONAL,
        BUSINESS,
        PAYROLL
    }
    public abstract class SecCode {
        public const string PPD = "PPD";
        public const string CCD = "CCD";
        public const string POP = "POP";
        public const string WEB = "WEB";
        public const string TEL = "TEL";
        public const string EBRONZE = "EBRONZE";
    }

    [Flags]
    public enum ReportType {
        FindTransactions = 0,
        Activity = 1 << 1,
        BatchDetail = 1 << 2,
        BatchHistory = 1 << 3,
        BatchSummary = 1 << 4,
        OpenAuths = 1 << 5,
        Search = 1 << 6,
        TransactionDetail = 1 << 7
    }

    public enum TimeZoneConversion {
        UTC,
        Merchant,
        Datacenter
    }

    public enum RecurringType {
        Fixed,
        Variable
    }

    public enum RecurringSequence {
        First,
        Subsequent,
        Last
    }

    public enum EmailReceipt {
        Never,
        All,
        Approvals,
        Declines
    }

    public enum PaymentSchedule {
        Dynamic,
        FirstDayOfTheMonth,
        LastDayOfTheMonth
    }

    public static class ScheduleFrequency {
        public const string WEEKLY = "Weekly";
        public const string BI_WEEKLY = "Bi-Weekly";
        public const string BI_MONTHLY = "Bi-Monthly";
        public const string SEMI_MONTHLY = "Semi-Monthly";
        public const string MONTHLY = "Monthly";
        public const string QUARTERLY = "Quarterly";
        public const string SEMI_ANNUALLY = "Semi-Annually";
        public const string ANNUALLY = "Annually";
    }

    public enum ReasonCode {
        FRAUD,
        FALSEPOSITIVE,
        OUTOFSTOCK,
        INSTOCK,
        OTHER,
        NOTGIVEN
    }
}
