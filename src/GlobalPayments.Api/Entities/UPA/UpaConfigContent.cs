using GlobalPayments.Api.Entities.Enums;

namespace GlobalPayments.Api.Entities.UPA {
    public class UpaConfigContent {
        public TerminalConfigType ConfigType { get; set; }
        public string FileContent { get; set; }
        public int Length { get; set; }
        public Reinitialize? Reinitialize { get; set; }
    }
}
