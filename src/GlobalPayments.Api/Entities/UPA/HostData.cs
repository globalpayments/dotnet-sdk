using GlobalPayments.Api.Terminals.Enums;

namespace GlobalPayments.Api.Entities.UPA {
    public class HostData {
        public HostDecision? HostDecision { get; set; }
        public string IssuerScripts { get; set; }
        public string IssuerAuthData { get; set; }
    }
}
