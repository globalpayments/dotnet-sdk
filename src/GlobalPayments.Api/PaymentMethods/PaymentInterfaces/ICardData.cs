using GlobalPayments.Api.Entities;

namespace GlobalPayments.Api.PaymentMethods {
    public interface ICardData {
        bool CardPresent { get; set; }
        string CardType { get; set; }
        string CardHolderName { get; set; }
        string Cvn { get; set; }
        CvnPresenceIndicator CvnPresenceIndicator { get; set; }
        string Number { get; set; }
        int? ExpMonth { get; set; }
        int? ExpYear { get; set; }
        bool ReaderPresent { get; set; }
        string ShortExpiry { get; }
        ManualEntryMethod? EntryMethod { get; set; }
        EntryMethod? OriginalEntryMethod { get; set; }
    }
}
