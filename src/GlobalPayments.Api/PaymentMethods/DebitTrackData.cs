using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.PaymentMethods {
    /// <summary>
    /// Use PIN debit track data as a payment method.
    /// </summary>
    public class DebitTrackData : Debit, ITrackData {
        private string _trackData;
        private string _value;

        public string DiscretionaryData { get; set; }
        public EntryMethod EntryMethod { get; set; }
        public string Expiry { get; set; }
        public string Pan { get; set; }
        public string PurchaseDeviceSequenceNumber { get; set; }
        public TrackNumber TrackNumber { get; set; }
        public string TrackData {
            get { return _trackData; }
            set {
                if (_value == null) {
                    Value = value;
                }
                else _trackData = value;
            }
        }
        public string Value {
            get { return _value; }
            set {
                _value = value;
                CardUtils.ParseTrackData(this);
                CardType = CardUtils.MapCardType(Pan);
            }
        }

        public DebitTrackData() : base() { }
    }
}
