using GlobalPayments.Api.Entities.Enums;

namespace GlobalPayments.Api.Entities.UPA {
    public class UpaParam {
        public int Timeout { get; set; }
        public AcquisitionType AcquisitionTypes { get; set; }
        public string Header { get; set; }
        public string DisplayTotalAmount { get; set; }
        public bool PromptForManual { get; set; }
        public int BrandIcon1 { get; set; }
        public int BrandIcon2 { get; set; }
    }
}
