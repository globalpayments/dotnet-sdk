using GlobalPayments.Api.Entities;

namespace GlobalPayments.Api.PaymentMethods {
    /// <summary>
    /// Use EBT manual entry data as a payment method.
    /// </summary>
    public class EBTCardData : EBT, ICardData {
        public string ApprovalCode { get; set; }
        public bool CardPresent { get; set; }
        public string CardType { get; set; }
        public string Cvn { get; set; }
        public CvnPresenceIndicator CvnPresenceIndicator { get; set; }
        public int? ExpMonth { get; set; }
        public int? ExpYear { get; set; }
        public string Number { get; set; }
        public bool ReaderPresent { get; set; }
        public string SerialNumber { get; set; }
        public ManualEntryMethod? EntryMethod { get; set; }
        public EntryMethod? OriginalEntryMethod { get; set; }
        public string ShortExpiry {
            get {
                var month = (ExpMonth.HasValue) ? ExpMonth.ToString().PadLeft(2, '0') : string.Empty;
                var year = (ExpYear.HasValue) ? ExpYear.ToString().PadLeft(4, '0').Substring(2, 2) : string.Empty;
                return year + month;
            }
        }
        public EBTCardData() {  }
        public EBTCardData(EbtCardType cardType) {
            EbtCardType = cardType;
        }
    }
}
