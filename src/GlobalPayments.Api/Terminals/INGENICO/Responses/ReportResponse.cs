using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Utils;
using System.Text;

namespace GlobalPayments.Api.Terminals.Ingenico.Responses {
    public class ReportResponse : IngenicoTerminalResponse, ITerminalReport {
        internal ReportResponse(byte[] buffer) : base(buffer) {
            ParseResponse(buffer);
        }
    }
}
