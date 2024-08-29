using GlobalPayments.Api.Entities.Enums;

namespace GlobalPayments.Api.Entities.UPA {
    public class ScanData {
        public string Header { get; set; }
        public string Prompt1 { get; set; }
        public string Prompt2 { get; set; }
        public DisplayOption? DisplayOption { get; set; }
        public int? TimeOut { get; set; }
    }
}
