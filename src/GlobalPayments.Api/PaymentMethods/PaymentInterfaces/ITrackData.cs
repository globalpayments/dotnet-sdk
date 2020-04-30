using GlobalPayments.Api.Entities;

namespace GlobalPayments.Api.PaymentMethods {
    public interface ITrackData {
        string Expiry { get; set; }
        string Pan { get; set; }
        TrackNumber TrackNumber { get; set; }
        string TrackData { get; set; }
        string DiscretionaryData { get; set; }
        string Value { get; set; }
        EntryMethod EntryMethod { get; set; }
    }
}
