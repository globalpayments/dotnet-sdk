using GlobalPayments.Api.Entities.Enums;

namespace GlobalPayments.Api.Entities.UPA {
    public class PrintData {
        public string FilePath { get; set; }
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public DisplayOption? DisplayOption { get; set; }
    }
}
