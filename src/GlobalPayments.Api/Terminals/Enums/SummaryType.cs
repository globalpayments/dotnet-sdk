namespace GlobalPayments.Api.Terminals.Enums {
    public enum SummaryType {
        Approved,
        PartiallyApproved,
        VoidApproved,
        Pending,
        VoidPending,
        Declined,
        VoidDeclined,
        OfflineApproved,
        Provsional,
        Discarded,
        VoidProvisional,
        VoidDiscarded,
        Reversal,
        EmvDeclined,
        Attachment,
        Unknown
    }
}
