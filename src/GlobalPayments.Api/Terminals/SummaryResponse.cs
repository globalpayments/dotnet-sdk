using GlobalPayments.Api.Entities;
using System.Collections.Generic;

namespace GlobalPayments.Api.Terminals {
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

    public class SummaryResponse {
        public decimal? Amount { get; set; }
        public decimal? AmountDue { get; set; }
        public decimal? AuthorizedAmount { get; set; }
        public int Count { get; set; }
        public SummaryType SummaryType { get; set; }
        public decimal? TotalAmount { get; set; }
        public List<TransactionSummary> Transactions { get; set; }

        public SummaryResponse() {
            Transactions = new List<TransactionSummary>();
        }
    }
}
