using GlobalPayments.Api.Entities.Enums;

namespace GlobalPayments.Api.Entities.UPA {
    public class ProcessingIndicator {
        public string QuickChip { get; set; }
        public string CheckLuhn { get; set; }
        public string SecurityCode { get; set; }
        public CardTypeFilter CardTypeFilter { get; set; }
    }
}
